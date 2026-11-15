using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager instance { get; private set; }

    [SerializeField] private int startingMoney = 100;

    public int Money { get; private set; }

    // Quem quiser reagir a mudanças de saldo (HUD, botões da loja...) assina aqui.
    public event Action<int> OnMoneyChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Tem mais de um EconomyManager na cena!");
            return;
        }
        instance = this;
        Money = startingMoney;
    }

    private void Start()
    {
        // Avisa o saldo inicial pra quem já estiver escutando.
        OnMoneyChanged?.Invoke(Money);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public bool CanAfford(int amount) => amount <= Money;

    public bool TrySpend(int amount)
    {
        if (amount < 0 || !CanAfford(amount))
            return false;

        Money -= amount;
        OnMoneyChanged?.Invoke(Money);
        return true;
    }

    public void Add(int amount)
    {
        if (amount <= 0)
            return;

        Money += amount;
        OnMoneyChanged?.Invoke(Money);
    }

    // ---- Testes rápidos: clique com o botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: +100 moedas")]
    private void DebugAdd()
    {
        Add(100);
        Debug.Log($"Dinheiro: {Money}");
    }

    [ContextMenu("Debug: gastar 30")]
    private void DebugSpend()
    {
        bool ok = TrySpend(30);
        Debug.Log($"Gastou 30? {ok} | Dinheiro: {Money}");
    }
}