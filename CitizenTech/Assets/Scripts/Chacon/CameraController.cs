using UnityEngine;
using UnityEngine.EventSystems;

// Câmera "estilo Project Zomboid": arrasta com o botão direito, zoom com o scroll.
// Não gira nem inclina: a rotação da câmera nunca muda.
public class CameraController : MonoBehaviour
{
    [Header("Câmera")]
    [SerializeField] private Camera cam;

    [Header("Arrastar (botão direito)")]
    [Tooltip("Altura do chão do jogo (o plano que o mouse 'agarra').")]
    [SerializeField] private float groundY = 0f;

    [Header("Zoom (scroll)")]
    [Tooltip("Quanto cada 'clique' do scroll muda o zoom.")]
    [SerializeField] private float zoomStep = 1f;
    [SerializeField] private float zoomSmoothing = 12f;

    [Header("Zoom: se a câmera for ORTOGRÁFICA")]
    [SerializeField] private float minOrthographicSize = 3f;
    [SerializeField] private float maxOrthographicSize = 12f;

    [Header("Zoom: se a câmera for em PERSPECTIVA (altura da câmera)")]
    [SerializeField] private float minHeight = 4f;
    [SerializeField] private float maxHeight = 20f;

    [Header("Limites (4 objetos vazios nos cantos)")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private Transform cornerTopLeft;
    [SerializeField] private Transform cornerTopRight;
    [SerializeField] private Transform cornerBottomRight;
    [SerializeField] private Transform cornerBottomLeft;
    [Tooltip("Folga para dentro do retângulo (0 = vai até a borda exata).")]
    [SerializeField] private float margin = 0f;

    private bool dragging;
    private Vector3 dragStartPoint;
    private float targetOrthographicSize;
    private float targetHeight;

    private void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();
        if (cam == null)
            cam = Camera.main;

        targetOrthographicSize = Mathf.Clamp(cam.orthographicSize, minOrthographicSize, maxOrthographicSize);
        targetHeight = Mathf.Clamp(transform.position.y, minHeight, maxHeight);
    }

    private void Update()
    {
        // Pausado: sem movimento de câmera.
        if (Time.timeScale == 0f)
        {
            dragging = false;
            return;
        }

        HandlePan();
        HandleZoom();
        ClampToBounds();
    }

    private void HandlePan()
    {
        if (Input.GetMouseButtonDown(1) && !IsPointerOverUI())
        {
            if (TryGetGroundPoint(out dragStartPoint))
                dragging = true;
        }

        if (Input.GetMouseButtonUp(1))
            dragging = false;

        if (!dragging || !Input.GetMouseButton(1))
            return;

        // O ponto do chão que você "agarrou" tem que continuar embaixo do mouse.
        if (TryGetGroundPoint(out Vector3 currentPoint))
        {
            Vector3 delta = dragStartPoint - currentPoint;
            delta.y = 0f;
            transform.position += delta;
        }
    }

    private void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;
        if (!Mathf.Approximately(scroll, 0f) && !IsPointerOverUI())
        {
            // Scroll pra cima aproxima (zoom in), pra baixo afasta.
            targetOrthographicSize = Mathf.Clamp(
                targetOrthographicSize - scroll * zoomStep, minOrthographicSize, maxOrthographicSize);
            targetHeight = Mathf.Clamp(
                targetHeight - scroll * zoomStep, minHeight, maxHeight);
        }

        float smoothing = 1f - Mathf.Exp(-zoomSmoothing * Time.unscaledDeltaTime);

        if (cam.orthographic)
        {
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetOrthographicSize, smoothing);
        }
        else
        {
            // Aproxima/afasta andando pra frente ou pra trás, sem mudar o ângulo da câmera.
            float currentY = transform.position.y;
            float newY = Mathf.Lerp(currentY, targetHeight, smoothing);
            float forwardY = transform.forward.y;

            if (Mathf.Abs(forwardY) > 0.01f)
                transform.position += transform.forward * ((newY - currentY) / forwardY);
        }
    }

    // Limita o PONTO DO CHÃO que está no centro da tela, não a posição da câmera.
    private void ClampToBounds()
    {
        if (!useBounds || !TryGetBounds(out float minX, out float maxX, out float minZ, out float maxZ))
            return;

        if (!TryGetScreenCenterGroundPoint(out Vector3 focus))
            return;

        float cx = ClampAxis(focus.x, minX + margin, maxX - margin);
        float cz = ClampAxis(focus.z, minZ + margin, maxZ - margin);

        transform.position += new Vector3(cx - focus.x, 0f, cz - focus.z);
    }

    // Se a margem for maior que a área, trava no centro em vez de dar erro.
    private static float ClampAxis(float value, float min, float max)
    {
        if (min > max)
            return (min + max) * 0.5f;
        return Mathf.Clamp(value, min, max);
    }

    // Lê os 4 cantos e monta o retângulo (não importa a ordem em que foram arrastados).
    private bool TryGetBounds(out float minX, out float maxX, out float minZ, out float maxZ)
    {
        minX = maxX = minZ = maxZ = 0f;

        if (cornerTopLeft == null || cornerTopRight == null ||
            cornerBottomRight == null || cornerBottomLeft == null)
            return false;

        Transform[] corners = { cornerTopLeft, cornerTopRight, cornerBottomRight, cornerBottomLeft };

        minX = minZ = float.MaxValue;
        maxX = maxZ = float.MinValue;

        foreach (Transform corner in corners)
        {
            Vector3 p = corner.position;
            minX = Mathf.Min(minX, p.x);
            maxX = Mathf.Max(maxX, p.x);
            minZ = Mathf.Min(minZ, p.z);
            maxZ = Mathf.Max(maxZ, p.z);
        }
        return true;
    }

    private bool TryGetScreenCenterGroundPoint(out Vector3 point)
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        return RaycastGround(ray, out point);
    }

    private bool TryGetGroundPoint(out Vector3 point)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        return RaycastGround(ray, out point);
    }

    private bool RaycastGround(Ray ray, out Vector3 point)
    {
        Plane ground = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));

        if (ground.Raycast(ray, out float distance))
        {
            point = ray.GetPoint(distance);
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    // Retângulo verde na Scene View mostrando a área da câmera.
    private void OnDrawGizmos()
    {
        if (!useBounds || !TryGetBounds(out float minX, out float maxX, out float minZ, out float maxZ))
            return;

        Gizmos.color = Color.green;
        Vector3 center = new Vector3((minX + maxX) * 0.5f, groundY, (minZ + maxZ) * 0.5f);
        Vector3 size = new Vector3(maxX - minX, 0.1f, maxZ - minZ);
        Gizmos.DrawWireCube(center, size);
    }
}