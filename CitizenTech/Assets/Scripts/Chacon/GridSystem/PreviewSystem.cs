using UnityEngine;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f; 

    [SerializeField] private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField] private Material previewMaterialPrefab;
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;


    private Vector3 previewOffset = Vector3.zero;
    private Quaternion previewRotation = Quaternion.identity;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }

    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size,
        Installation visualSource = null)
    {

        previewObject = Instantiate(prefab);

   

        foreach (Installation installation in previewObject.GetComponentsInChildren<Installation>())
            installation.enabled = false;


        if (visualSource != null)
        {
            Installation ghostInstallation = previewObject.GetComponent<Installation>();
            if (ghostInstallation != null)
                ghostInstallation.CopyLevelFrom(visualSource);
        }
        previewOffset = Vector3.zero;
        previewRotation = Quaternion.identity;

        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }

  
    public void SetRotation(int rotationIndex, Vector2Int baseSize, Vector3 cellSize)
    {
        previewRotation = GridRotation.Rotation(rotationIndex);
        previewOffset = GridRotation.PivotOffset(baseSize, rotationIndex, cellSize);
        PrepareCursor(GridRotation.RotatedSize(baseSize, rotationIndex));

        if (previewObject != null)
            previewObject.transform.rotation = previewRotation;
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }

    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
        {
            Destroy(previewObject);
        }
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            MovePreview(position);
            ApplyFeedbackToPreview(validity);
        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;

        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.SetPositionAndRotation(
            position + previewOffset + Vector3.up * previewYOffset,
            previewRotation);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}