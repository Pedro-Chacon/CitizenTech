using System;
using UnityEngine;

public class Installation : MonoBehaviour
{
    [SerializeField] protected InstallationDataSO data;

    [Tooltip("ID do ObjectsDatabaseSO, se esta instalação já nasce na cena (silo, casa, cavalo). -1 = não é de cena.")]
    [SerializeField] private int sceneDatabaseId = -1;
    public int SceneDatabaseId => sceneDatabaseId;
    public InstallationDataSO Data => data;


    public int CurrentLevel { get; private set; }


    public int TotalSpent { get; private set; }


    public event Action<Installation> OnChanged;


    public virtual bool CanMove => true;

    [Header("Regras")]
    [Tooltip("Desmarque para bloquear o botão de vender (casa, silo, cavalo...).")]
    [SerializeField] private bool sellable = true;

    public virtual bool CanSell => sellable;

    public virtual bool CanUpgrade => HasNextUpgrade;

    public virtual bool IsIdle => false;

   
    public virtual bool TryQuickAction() => false;

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


    public int GetUpgradeBonus(Func<UpgradeLevelData, int> selector)
    {
        if (data == null)
            return 0;

        int total = 0;
        for (int i = 0; i < CurrentLevel && i < data.upgrades.Count; i++)
            total += selector(data.upgrades[i]);
        return total;
    }


    public int GetSellValue()
    {
        if (data == null)
            return 0;
        return Mathf.FloorToInt(TotalSpent * data.refundPercent);
    }
   
    public void CopyLevelFrom(Installation source)
    {
        CurrentLevel = source.CurrentLevel;
        OnUpgraded(CurrentLevel);
    }

    protected void NotifyChanged()
    {
        OnChanged?.Invoke(this);
    }


    protected virtual void OnDayTick() { }
    protected virtual void OnUpgraded(int newLevel) { }


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