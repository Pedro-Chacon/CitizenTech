using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private Button openButton;
    [SerializeField] private Button closeButton;

    [Header("Itens")]
    [Tooltip("IDs do ObjectsDatabaseSO que aparecem na loja (não use o ID 0).")]
    [SerializeField] private List<int> shopItemIds = new();
    [SerializeField] private ShopItemUI itemPrefab;
    [SerializeField] private Transform itemsContainer;

    private readonly List<(int id, ShopItemUI ui)> items = new();
    private bool isOpen;

    private void Start()
    {
        BuildItems();

        openButton.onClick.AddListener(Open);
        closeButton.onClick.AddListener(Close);

        if (EconomyManager.instance != null)
        {
            EconomyManager.instance.OnMoneyChanged += OnMoneyChanged;
            OnMoneyChanged(EconomyManager.instance.Money);
        }

        panel.Hide(true);
    }

    private void OnDestroy()
    {
        if (EconomyManager.instance != null)
            EconomyManager.instance.OnMoneyChanged -= OnMoneyChanged;
    }

    private void Update()
    {
        // O botão de construir só aparece quando o jogador não está fazendo outra coisa.
        bool busy = TurnManager.instance != null && TurnManager.instance.IsBusy;
        openButton.gameObject.SetActive(!isOpen && !placementSystem.IsPlacing && !busy);
    }

    private void BuildItems()
    {
        foreach (int id in shopItemIds)
        {
            InstallationDataSO data = placementSystem.GetData(id);
            if (data == null)
            {
                Debug.LogWarning($"[ShopPanel] ID {id} sem Data no database, ignorado.", this);
                continue;
            }

            ShopItemUI ui = Instantiate(itemPrefab, itemsContainer);
            int itemId = id; // cópia pro lambda
            string itemName = string.IsNullOrEmpty(data.displayName) ? $"Item {id}" : data.displayName;
            ui.Setup(itemName, data.icon, data.price, () => Buy(itemId));
            items.Add((id, ui));
        }
    }

    private void OnMoneyChanged(int money)
    {
        foreach (var (id, ui) in items)
        {
            InstallationDataSO data = placementSystem.GetData(id);
            ui.SetAffordable(data != null && money >= data.price);
        }
    }

    private void Open()
    {
        isOpen = true;
        panel.Show();
    }

    public void Close()
    {
        isOpen = false;
        panel.Hide();
    }

    private void Buy(int id)
    {
        bool busy = TurnManager.instance != null && TurnManager.instance.IsBusy;
        if (busy)
            return;

        if (placementSystem.StartPurchase(id))
            Close(); // fecha a loja pra deixar a grid livre pro posicionamento
    }
}