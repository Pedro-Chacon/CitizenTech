using System;
using UnityEngine;

public class Installation : MonoBehaviour
{
    [SerializeField] protected InstallationDataSO data;

    [Tooltip("ID do ObjectsDatabaseSO, se esta instalação já nasce na cena (silo, casa, cavalo). -1 = não é de cena.")]
    [SerializeField] private int sceneDatabaseId = -1;
    public int SceneDatabaseId => sceneDatabaseId;
    public InstallationDataSO Data => data;

    // Quantos upgrades já foram comprados (também é o índice do PRÓXIMO upgrade).
    public int CurrentLevel { get; private set; }

    // Compra + upgrades já pagos. Base do valor de venda.
    public int TotalSpent { get; private set; }

    // A UI (menu, billboard) escuta isso pra se atualizar.
    public event Action<Installation> OnChanged;

    // ---- Regras que o menu de bolhas pergunta ----
    public virtual bool CanMove => true;

    [Header("Regras")]
    [Tooltip("Desmarque para bloquear o botão de vender (casa, silo, cavalo...).")]
    [SerializeField] private bool sellable = true;

    public virtual bool CanSell => sellable;

    public virtual bool CanUpgrade => HasNextUpgrade;

    // Usado no aviso de "instalações paradas" ao passar o dia. Subclasses sobrescrevem.
    public virtual bool IsIdle => false;

    // Ação rápida ao clicar na instalação (ex.: plantar/colher). Devolve true se fez algo.
    public virtual bool TryQuickAction() => false;

    // Bolha extra do menu (ex.: silo abre o inventário, cavalo abre a cidade).
    public virtual bool HasExtraAction => false;
    public virtual bool CanDoExtraAction => true;
    public virtual Sprite ExtraActionIcon => null;
    public virtual void DoExtraAction() { }

    public bool HasNextUpgrade => data != null && CurrentLevel < data.upgrades.Count;
    public UpgradeLevelData NextUpgrade => HasNextUpgrade ? data.upgrades[CurrentLevel] : null;

    protected virtual void Awake()
    {
        if (data != null)
            TotalSpent = data.price;
    }

    private void Start()
    {
        // Assina no Start: o TurnManager já existe, e uma cópia desativada (ex.: preview) nunca assina.
        if (TurnManager.instance != null)
            TurnManager.instance.OnDayEnded += HandleDayEnded;
    }

    private void OnDestroy()
    {
        if (TurnManager.instance != null)
            TurnManager.instance.OnDayEnded -= HandleDayEnded;
    }

    private void HandleDayEnded(int day)
    {
        OnDayTick();
    }

    public bool TryUpgrade()
    {
        if (!CanUpgrade || EconomyManager.instance == null)
            return false;

        UpgradeLevelData next = NextUpgrade;
        if (!EconomyManager.instance.TrySpend(next.cost))
            return false;

        TotalSpent += next.cost;
        CurrentLevel++;
        OnUpgraded(CurrentLevel);
        OnChanged?.Invoke(this);
        return true;
    }

    // Soma um efeito de todos os upgrades JÁ comprados.
    public int GetUpgradeBonus(Func<UpgradeLevelData, int> selector)
    {
        if (data == null)
            return 0;

        int total = 0;
        for (int i = 0; i < CurrentLevel && i < data.upgrades.Count; i++)
            total += selector(data.upgrades[i]);
        return total;
    }

    // Mesmo valor que a janela de confirmação mostra e que o jogador recebe.
    public int GetSellValue()
    {
        if (data == null)
            return 0;
        return Mathf.FloorToInt(TotalSpent * data.refundPercent);
    }

    // Dispara a UI de novo (útil pras subclasses quando o estado delas muda).
    protected void NotifyChanged()
    {
        OnChanged?.Invoke(this);
    }

    // ---- Ganchos pras instalações específicas (FarmInstallation, etc.) ----
    protected virtual void OnDayTick() { }
    protected virtual void OnUpgraded(int newLevel) { }

    // ---- Testes rápidos: botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: tentar upgrade")]
    private void DebugUpgrade()
    {
        bool ok = TryUpgrade();
        Debug.Log($"[{name}] Upgrade? {ok} | Nível: {CurrentLevel} | Gasto total: {TotalSpent}", this);
    }

    [ContextMenu("Debug: status")]
    private void DebugStatus()
    {
        Debug.Log($"[{name}] Nível: {CurrentLevel} | " +
                  $"CanMove: {CanMove} | CanUpgrade: {CanUpgrade} | CanSell: {CanSell} | " +
                  $"Venda: {GetSellValue()}", this);
    }
}