using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private InstallationMenu menu;
    [SerializeField] private Camera sceneCamera;

    [Header("Raycast")]
    [Tooltip("Layer das instalações (crie uma layer 'Installation').")]
    [SerializeField] private LayerMask installationLayerMask;

    [SerializeField] private SellConfirmationPanel sellPanel;

    private void Start()
    {
        if (sceneCamera == null)
            sceneCamera = Camera.main;

        inputManager.OnClicked += HandleClick;
        menu.OnMoveRequested += placementSystem.StartMoving;
    }

    private void OnDestroy()
    {
        if (inputManager != null)
            inputManager.OnClicked -= HandleClick;
        if (menu != null)
            menu.OnMoveRequested -= placementSystem.StartMoving;
    }

    private void HandleClick()
    {
        // Enquanto está posicionando algo, o clique é do sistema de posicionamento.
        if (placementSystem.IsPlacing)
            return;

        if (sellPanel != null && sellPanel.IsOpen)
            return;

        // Pausado ou com o dia passando: sem interação.
        if (Time.timeScale == 0f)
            return;
        if (TurnManager.instance != null && TurnManager.instance.IsBusy)
            return;

        // Cliques em botões, bolhas e billboards são da UI.
        if (inputManager.IsPonterOverUI())
            return;

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, installationLayerMask))
        {
            menu.Close();
            return;
        }

        Installation installation = hit.collider.GetComponentInParent<Installation>();
        if (installation == null)
        {
            menu.Close();
            return;
        }

        // Ação rápida (ex.: plantar/colher) tem prioridade sobre abrir o menu.
        if (installation.TryQuickAction())
        {
            menu.Close();
            return;
        }

        // O menu abre no topo do collider, que não depende de onde está o pivô do prefab.
        Bounds bounds = hit.collider.bounds;
        Vector3 anchor = new Vector3(bounds.center.x, bounds.max.y, bounds.center.z);
        menu.Open(installation, anchor);
    }
}