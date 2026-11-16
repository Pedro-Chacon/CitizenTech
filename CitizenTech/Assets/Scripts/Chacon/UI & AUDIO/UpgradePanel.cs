using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradePanel : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private InstallationMenu menu;
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text titleText;

    [Header("Cards")]
    [SerializeField] private UpgradeCardUI cardPrefab;
    [SerializeField] private Transform cardsContainer;

    private readonly List<UpgradeCardUI> cards = new();
    private Installation target;

    // O SelectionManager consulta isso pra ignorar cliques no mundo enquanto a janela está aberta.
    public bool IsOpen { get; private set; }

    private void Start()
    {
        menu.OnUpgradeRequested += Open;
        closeButton.onClick.AddListener(Close);

        if (EconomyManager.instance != null)
            EconomyManager.instance.OnMoneyChanged += OnMoneyChanged;

        panel.Hide(true);
    }

    private void OnDestroy()
    {
        if (menu != null)
            menu.OnUpgradeRequested -= Open;

        if (EconomyManager.instance != null)
            EconomyManager.instance.OnMoneyChanged -= OnMoneyChanged;

        if (target != null)
            target.OnChanged -= OnTargetChanged;
    }

    private void Open(Installation installation)
    {
        if (installation == null || installation.Data == null)
            return;

        target = installation;
        target.OnChanged += OnTargetChanged;
        IsOpen = true;

        if (titleText != null)
        {
            string itemName = string.IsNullOrEmpty(installation.Data.displayName)
                ? installation.name
                : installation.Data.displayName;
            titleText.text = $"Melhorias: {itemName}";
        }

        Rebuild();
        panel.Show();

        // Garante que os cards ocupem o espaço certo assim que a janela aparece.
        if (cardsContainer is RectTransform rect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void Close()
    {
        if (target != null)
            target.OnChanged -= OnTargetChanged;

        target = null;
        IsOpen = false;
        panel.Hide();
    }

    private void Rebuild()
    {
        foreach (UpgradeCardUI card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }
        cards.Clear();

        if (cardPrefab == null || cardsContainer == null)
        {
            Debug.LogError(
                $"[UpgradePanel] Falta ligar no Inspector: " +
                $"Card Prefab = {(cardPrefab != null ? "ok" : "VAZIO")}, " +
                $"Cards Container = {(cardsContainer != null ? "ok" : "VAZIO")}.", this);
            return;
        }

        List<UpgradeLevelData> upgrades = target.Data.upgrades;
        for (int i = 0; i < upgrades.Count; i++)
        {
            UpgradeCardUI card = Instantiate(cardPrefab, cardsContainer);

            // Se o prefab foi salvo desativado, o card nasceria invisível.
            card.gameObject.SetActive(true);

            int index = i; // cópia pro lambda
            card.Setup(upgrades[i], () => Buy(index));
            cards.Add(card);
        }

        Debug.Log(
            $"[UpgradePanel] {target.name}: {upgrades.Count} upgrade(s) no Data, " +
            $"{cards.Count} card(s) criados dentro de '{cardsContainer.name}'.", this);

        Refresh();
    }

    private void Refresh()
    {
        if (!IsOpen || target == null)
            return;

        List<UpgradeLevelData> upgrades = target.Data.upgrades;
        int money = EconomyManager.instance != null ? EconomyManager.instance.Money : 0;

        for (int i = 0; i < cards.Count; i++)
        {
            UpgradeCardUI.CardState state =
                i < target.CurrentLevel ? UpgradeCardUI.CardState.Purchased :
                i == target.CurrentLevel ? UpgradeCardUI.CardState.Available :
                UpgradeCardUI.CardState.Locked;

            cards[i].SetState(state, money >= upgrades[i].cost);
        }
    }

    private void Buy(int index)
    {
        // Não compra enquanto o dia está passando; só o próximo upgrade da fila.
        if (TurnManager.instance != null && TurnManager.instance.IsBusy)
            return;
        if (target == null || index != target.CurrentLevel)
            return;

        target.TryUpgrade(); // gasta o dinheiro, sobe o nível e dispara OnChanged
        Refresh();
    }

    private void OnMoneyChanged(int money) => Refresh();
    private void OnTargetChanged(Installation installation) => Refresh();
}