using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private InventoryRowUI rowPrefab;
    [SerializeField] private Transform rowsContainer;
    [SerializeField] private Button closeButton;

    [Tooltip("Opcional: objeto (ex.: um texto 'Vazio') mostrado quando não há nenhum produto.")]
    [SerializeField] private GameObject emptyMessage;

    private readonly Dictionary<ProductSO, InventoryRowUI> rows = new();

    // O SelectionManager consulta isso pra ignorar cliques no mundo enquanto a janela está aberta.
    public bool IsOpen { get; private set; }

    private void Start()
    {
        closeButton.onClick.AddListener(Close);

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
            Debug.LogError("[InventoryPanel] InventoryManager não encontrado na cena.", this);
            return;
        }

        foreach (ProductSO product in InventoryManager.instance.AllProducts)
        {
            if (product == null)
                continue;

            InventoryRowUI row = Instantiate(rowPrefab, rowsContainer);
            row.Setup(product);
            rows[product] = row;
        }
    }

    public void Open()
    {
        IsOpen = true;
        Refresh();
        panel.Show();
    }

    public void Close()
    {
        IsOpen = false;
        panel.Hide();
    }

    // Mostra só os produtos que o jogador tem.
    private void Refresh()
    {
        if (!IsOpen)
            return;

        bool anyProduct = false;
        foreach (var pair in rows)
        {
            int amount = InventoryManager.instance.GetAmount(pair.Key);
            pair.Value.gameObject.SetActive(amount > 0);
            pair.Value.SetAmount(amount);

            if (amount > 0)
                anyProduct = true;
        }

        if (emptyMessage != null)
            emptyMessage.SetActive(!anyProduct);
    }
}