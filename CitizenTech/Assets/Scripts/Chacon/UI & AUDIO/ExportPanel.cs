using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExportPanel : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private ExportRowUI rowPrefab;
    [SerializeField] private Transform rowsContainer;
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private TMP_Text tripText;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button cancelButton;

    private readonly Dictionary<ProductSO, int> selected = new();
    private readonly Dictionary<ProductSO, ExportRowUI> rows = new();
    private HorseInstallation horse;

    // O SelectionManager consulta isso pra ignorar cliques no mundo enquanto a janela está aberta.
    public bool IsOpen { get; private set; }

    private void Start()
    {
        sendButton.onClick.AddListener(Send);
        cancelButton.onClick.AddListener(Close);

        BuildRows();

        if (InventoryManager.instance != null)
            InventoryManager.instance.OnInventoryChanged += Refresh;

        panel.Hide(true);
    }

    private void OnDestroy()
    {
        if (InventoryManager.instance != null)
            InventoryManager.instance.OnInventoryChanged -= Refresh;
    }

    private void BuildRows()
    {
        if (InventoryManager.instance == null)
        {
            Debug.LogError("[ExportPanel] InventoryManager não encontrado na cena.", this);
            return;
        }

        foreach (ProductSO product in InventoryManager.instance.AllProducts)
        {
            if (product == null)
                continue;

            ExportRowUI row = Instantiate(rowPrefab, rowsContainer);
            ProductSO captured = product; // cópia pro lambda
            row.Setup(product, () => Change(captured, -1), () => Change(captured, +1));

            rows[product] = row;
            selected[product] = 0;
        }
    }

    public void Open(HorseInstallation targetHorse)
    {
        horse = targetHorse;
        IsOpen = true;

        foreach (ProductSO product in new List<ProductSO>(selected.Keys))
            selected[product] = 0;

        Refresh();
        panel.Show();
    }

    public void Close()
    {
        IsOpen = false;
        horse = null;
        panel.Hide();
    }

    private void Change(ProductSO product, int delta)
    {
        if (!selected.ContainsKey(product))
            return;

        int available = InventoryManager.instance.GetAmount(product);
        selected[product] = Mathf.Clamp(selected[product] + delta, 0, available);
        Refresh();
    }

    private void Refresh()
    {
        if (!IsOpen)
            return;

        int total = 0;
        foreach (var pair in rows)
        {
            ProductSO product = pair.Key;
            int available = InventoryManager.instance.GetAmount(product);

            // O estoque pode ter mudado com a janela aberta.
            selected[product] = Mathf.Min(selected[product], available);

            pair.Value.SetValues(available, selected[product]);
            total += selected[product] * product.sellPrice;
        }

        totalText.text = $"Total: {total} moedas";
        tripText.text = horse != null ? $"Viagem: {horse.TripDays} dias" : "";
        sendButton.interactable = total > 0;
    }

    private void Send()
    {
        // Não envia enquanto o dia está passando.
        if (TurnManager.instance != null && TurnManager.instance.IsBusy)
            return;

        if (horse != null && horse.TryStartTrip(selected))
            Close();
    }
}