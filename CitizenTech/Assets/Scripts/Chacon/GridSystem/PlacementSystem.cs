using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;
    [SerializeField] Grid grid;

    [SerializeField] private ObjectsDatabaseSO database;

    [SerializeField] private GameObject gridVisualization;

    private GridData floorData, furnitureData;

    [SerializeField] private PreviewSystem preview;

    private Vector3Int lastDetectedPosition = Vector3Int.zero;

    [SerializeField] private ObjectPlacer objectPlacer;

    [Header("Limites da grid")]
    [SerializeField] private bool limitToBounds = true;
    [Tooltip("Célula do canto inferior esquerdo da área jogável (X, Z).")]
    [SerializeField] private Vector2Int boundsMinCell = new Vector2Int(0, 0);
    [Tooltip("Largura e profundidade da área jogável, em células.")]
    [SerializeField] private Vector2Int boundsSize = new Vector2Int(10, 10);

    [Tooltip("Tecla que gira o objeto em passos de 90° durante o posicionamento.")]
    [SerializeField] private KeyCode rotateKey = KeyCode.R;

    IBuildingState buildingState;

    public bool IsPlacing => buildingState != null;

    void Start()
    {
        StopPlacement();
        floorData = new();
        furnitureData = new();

        if (limitToBounds)
        {
            floorData.SetBounds(boundsMinCell, boundsSize);
            furnitureData.SetBounds(boundsMinCell, boundsSize);
        }

        RegisterSceneInstallations();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID,
                                           grid,
                                           preview,
                                           database,
                                           floorData,
                                           furnitureData,
                                           objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    // Dados (nome, ícone, preço) da instalação que o prefab do database carrega.
    public InstallationDataSO GetData(int ID)
    {
        ObjectData entry = database.objectsData.Find(d => d.ID == ID);
        if (entry == null || entry.Prefab == null)
            return null;

        Installation installation = entry.Prefab.GetComponent<Installation>();
        return installation != null ? installation.Data : null;
    }

    // Compra: posiciona UMA instalação e cobra só quando o jogador confirma o lugar.
    public bool StartPurchase(int ID)
    {
        InstallationDataSO itemData = GetData(ID);
        if (itemData == null)
        {
            Debug.LogError($"[PlacementSystem] ID {ID} sem prefab/Installation/Data no database.", this);
            return false;
        }

        if (EconomyManager.instance != null && !EconomyManager.instance.CanAfford(itemData.price))
            return false;

        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new PlacementState(ID, grid, preview, database,
                                           floorData, furnitureData, objectPlacer,
                                           StopPlacement, itemData.price);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
        return true;
    }

    public void StartRemoving()
    {
        StopPlacement();
        gridVisualization.SetActive(true);
        buildingState = new RemovingState(grid, preview, floorData, furnitureData, objectPlacer);
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    public void StartMoving(Installation installation)
    {
        StopPlacement();

        MovingState state = new MovingState(installation, grid, preview, database,
                                            floorData, furnitureData, objectPlacer, StopPlacement);
        if (!state.IsValid)
            return;

        gridVisualization.SetActive(true);
        buildingState = state;
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    // Vende a instalação: libera as células, destrói o objeto e paga o valor de venda.
    public bool SellInstallation(Installation installation)
    {
        if (installation == null || !installation.CanSell)
            return false;

        int placedIndex = objectPlacer.GetIndexOf(installation.gameObject);
        if (placedIndex < 0)
        {
            Debug.LogError($"[PlacementSystem] {installation.name} não está registrada no ObjectPlacer.", installation);
            return false;
        }

        GridData data = furnitureData;
        PlacementData placement = furnitureData.FindByPlacedIndex(placedIndex);
        if (placement == null)
        {
            data = floorData;
            placement = floorData.FindByPlacedIndex(placedIndex);
        }

        if (placement == null)
        {
            Debug.LogError($"[PlacementSystem] {installation.name} não está registrada na grid.", installation);
            return false;
        }

        // O valor precisa ser calculado antes de destruir o objeto.
        int value = installation.GetSellValue();

        data.RemoveObjectAt(placement.Anchor);
        objectPlacer.RemoveObjectAt(placedIndex);

        if (EconomyManager.instance != null)
            EconomyManager.instance.Add(value);

        if (AudioManager.instance != null)
        {

                  AudioManager.instance.ObjectPlacementSFX(); 
        }
        return true;
    }

    public void RotateSelection()
    {
        if (buildingState == null)
            return;

        buildingState.Rotate();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayRotateSFX();

        Vector3Int gridPosition = grid.WorldToCell(inputManager.GetSelectedMapPosition());
        buildingState.UpdateState(gridPosition);
        lastDetectedPosition = gridPosition;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPonterOverUI())
        {
            return;
        }
        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);

        buildingState.OnAction(gridPosition);
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;
        gridVisualization.SetActive(false);
        buildingState.EndState();
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
        lastDetectedPosition = Vector3Int.zero;
        buildingState = null;
    }

    private void Update()
    {
        if (buildingState == null)
            return;

        if (Input.GetKeyDown(rotateKey))
            RotateSelection();

        Vector3 mousePosition = inputManager.GetSelectedMapPosition();
        Vector3Int gridPosition = grid.WorldToCell(mousePosition);
        if (lastDetectedPosition != gridPosition)
        {
            buildingState.UpdateState(gridPosition);
            lastDetectedPosition = gridPosition;
        }
    }


    private void RegisterSceneInstallations()
    {
        foreach (Installation installation in FindObjectsByType<Installation>(FindObjectsSortMode.None))
        {
            if (installation.SceneDatabaseId >= 0)
                RegisterExisting(installation);
        }
    }

    private void RegisterExisting(Installation installation)
    {
        ObjectData entry = database.objectsData.Find(d => d.ID == installation.SceneDatabaseId);
        if (entry == null)
        {
            Debug.LogError($"[PlacementSystem] {installation.name}: nenhum objeto com ID {installation.SceneDatabaseId} no Database.", installation);
            return;
        }

        int rotation = Mathf.RoundToInt(installation.transform.eulerAngles.y / 90f) % 4;
        Vector2Int rotatedSize = GridRotation.RotatedSize(entry.Size, rotation);
        Vector3 offset = GridRotation.PivotOffset(entry.Size, rotation, grid.cellSize);


        Vector3 corner = installation.transform.position - offset;
        Vector3Int anchor = grid.WorldToCell(corner + new Vector3(grid.cellSize.x, 0f, grid.cellSize.z) * 0.5f);
        anchor.y = 0;

        GridData data = entry.ID == 0 ? floorData : furnitureData;
        if (!data.CanPlaceObjectAt(anchor, rotatedSize))
        {
            Debug.LogWarning($"[PlacementSystem] {installation.name} sobrepõe outra instalação na célula {anchor}.", installation);
            return;
        }

        installation.transform.SetPositionAndRotation(
            grid.CellToWorld(anchor) + offset,
            GridRotation.Rotation(rotation));

        int index = objectPlacer.Register(installation.gameObject);
        data.AddObjectAt(anchor, rotatedSize, entry.ID, index, rotation);
    }

    private void OnDrawGizmosSelected()
    {
        if (!limitToBounds || grid == null)
            return;

        Vector3 a = grid.CellToWorld(new Vector3Int(boundsMinCell.x, 0, boundsMinCell.y));
        Vector3 b = grid.CellToWorld(new Vector3Int(boundsMinCell.x + boundsSize.x, 0, boundsMinCell.y + boundsSize.y));

        Vector3 center = (a + b) * 0.5f + Vector3.up * 0.05f;
        Vector3 size = new Vector3(Mathf.Abs(b.x - a.x), 0.05f, Mathf.Abs(b.z - a.z));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);
    }

    [ContextMenu("Preencher limites a partir do Grid Visualization")]
    private void FillBoundsFromVisualization()
    {
        Renderer r = gridVisualization != null ? gridVisualization.GetComponentInChildren<Renderer>(true) : null;
        if (r == null || grid == null)
        {
            Debug.LogWarning("[PlacementSystem] Grid Visualization sem Renderer (ou Grid não atribuído).", this);
            return;
        }

        Bounds b = r.bounds;
        Vector3 half = new Vector3(grid.cellSize.x, 0f, grid.cellSize.z) * 0.5f;


        Vector3Int min = grid.WorldToCell(new Vector3(b.min.x, 0f, b.min.z) + half);
        Vector3Int max = grid.WorldToCell(new Vector3(b.max.x, 0f, b.max.z) - half);

        boundsMinCell = new Vector2Int(min.x, min.z);
        boundsSize = new Vector2Int(max.x - min.x + 1, max.z - min.z + 1);

        Debug.Log($"[PlacementSystem] Limites: min {boundsMinCell}, tamanho {boundsSize}", this);
    }
}