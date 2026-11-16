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

    [Header("Limites do mapa (opcional)")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-10f, -10f);
    [SerializeField] private Vector2 maxBounds = new Vector2(30f, 30f);

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
        ClampPosition();
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

    private void ClampPosition()
    {
        if (!useBounds)
            return;

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x, minBounds.x, maxBounds.x);
        position.z = Mathf.Clamp(position.z, minBounds.y, maxBounds.y);
        transform.position = position;
    }

    private bool TryGetGroundPoint(out Vector3 point)
    {
        Plane ground = new Plane(Vector3.up, new Vector3(0f, groundY, 0f));
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

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
}