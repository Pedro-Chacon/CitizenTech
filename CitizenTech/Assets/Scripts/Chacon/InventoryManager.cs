using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance { get; private set; }

    [Tooltip("Todos os tipos de produto do jogo. O painel do inventário usa essa lista pra mostrar todos os slots.")]
    [SerializeField] private List<ProductSO> allProducts = new();

    private readonly Dictionary<ProductSO, int> amounts = new();

    public IReadOnlyList<ProductSO> AllProducts => allProducts;

    // O painel do inventário (e qualquer UI) escuta isso pra se atualizar.
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Tem mais de um InventoryManager na cena!");
            return;
        }
        instance = this;

        foreach (ProductSO product in allProducts)
        {
            if (product != null)
                amounts[product] = 0;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public int GetAmount(ProductSO product)
    {
        if (product == null)
            return 0;
        return amounts.TryGetValue(product, out int value) ? value : 0;
    }

    public bool Has(ProductSO product, int amount) => GetAmount(product) >= amount;

    public void Add(ProductSO product, int amount)
    {
        if (product == null || amount <= 0)
            return;

        amounts[product] = GetAmount(product) + amount;
        OnInventoryChanged?.Invoke();
    }

    // Devolve false (e não remove nada) se não houver quantidade suficiente.
    public bool TryRemove(ProductSO product, int amount)
    {
        if (product == null || amount <= 0 || !Has(product, amount))
            return false;

        amounts[product] -= amount;
        OnInventoryChanged?.Invoke();
        return true;
    }

    // ---- Testes rápidos: botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: +5 de cada produto")]
    private void DebugAddAll()
    {
        foreach (ProductSO product in allProducts)
            Add(product, 5);
        DebugStatus();
    }

    [ContextMenu("Debug: status")]
    private void DebugStatus()
    {
        foreach (ProductSO product in allProducts)
        {
            if (product != null)
                Debug.Log($"{product.displayName}: {GetAmount(product)}");
        }
    }
}