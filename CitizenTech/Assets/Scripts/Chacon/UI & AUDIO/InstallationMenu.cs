using System;
using UnityEngine;
using UnityEngine.UI;

public class InstallationMenu : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O Canvas world-space com as bolhas (filho deste objeto).")]
    [SerializeField] private GameObject visualRoot;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;

    [Header("Bolha extra (silo, cavalo...) - opcional")]
    [SerializeField] private Button extraButton;
    [SerializeField] private Image extraIcon;

    [Header("Posição")]
    [SerializeField] private float heightOffset = 0.5f;

    // Quem faz o trabalho de verdade (mover, painel de upgrade, painel de venda) assina aqui.
    public event Action<Installation> OnMoveRequested;
    public event Action<Installation> OnUpgradeRequested;
    public event Action<Installation> OnSellRequested;

    private Installation current;

    private void Start()
    {
        canvas.worldCamera = Camera.main;

        moveButton.onClick.AddListener(() => Request(OnMoveRequested, "Mover"));
        upgradeButton.onClick.AddListener(() => Request(OnUpgradeRequested, "Melhorar"));
        sellButton.onClick.AddListener(() => Request(OnSellRequested, "Vender"));

        if (extraButton != null)
            extraButton.onClick.AddListener(OnExtraClicked);

        Close();
    }

    public void Open(Installation installation, Vector3 anchor)
    {
        Close();

        current = installation;
        current.OnChanged += Refresh;

        transform.position = anchor + Vector3.up * heightOffset;
        Refresh(current);
        visualRoot.SetActive(true);
    }

    public void Close()
    {
        if (current != null)
            current.OnChanged -= Refresh;

        current = null;

        if (visualRoot != null)
            visualRoot.SetActive(false);
    }

    // Bolhas bloqueadas ficam visíveis, só não clicáveis.
    private void Refresh(Installation installation)
    {
        moveButton.interactable = installation.CanMove;
        upgradeButton.interactable = installation.CanUpgrade;
        sellButton.interactable = installation.CanSell;

        if (extraButton != null)
        {
            bool hasExtra = installation.HasExtraAction;
            extraButton.gameObject.SetActive(hasExtra);

            if (hasExtra)
            {
                extraButton.interactable = installation.CanDoExtraAction;

                if (extraIcon != null)
                {
                    extraIcon.sprite = installation.ExtraActionIcon;
                    extraIcon.enabled = extraIcon.sprite != null;
                }
            }
        }
    }

    private void OnExtraClicked()
    {
        if (current == null)
            return;

        Installation target = current;
        Close();
        target.DoExtraAction();
    }

    private void Request(Action<Installation> requestEvent, string label)
    {
        if (current == null)
            return;

        Installation target = current;
        Debug.Log($"[InstallationMenu] {label}: {target.name}");

        Close();
        requestEvent?.Invoke(target);
    }
}