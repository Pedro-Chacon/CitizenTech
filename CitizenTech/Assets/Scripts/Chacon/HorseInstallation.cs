using System.Collections.Generic;
using UnityEngine;

public enum HorseState
{
    Idle,      // parado na fazenda, pronto pra receber uma carga
    Away,      // levando os produtos pra cidade
    Returned   // voltou: lucro esperando o jogador coletar
}

public class HorseInstallation : Installation
{
    [Header("Referências de cena")]
    [SerializeField] private ExportPanel exportPanel;
    [SerializeField] private Sprite extraActionIcon;

    [Header("Visuais por nível (cavalo, carroça, carro...), em ordem")]
    [SerializeField] private GameObject[] levelVisuals;

    public HorseState State { get; private set; } = HorseState.Idle;
    public int TripDaysRemaining { get; private set; }
    public int PendingProfit { get; private set; }

    private HorseDataSO HorseData => data as HorseDataSO;
    // Dias de viagem no nível atual (os upgrades encurtam).
    public int TripDays => HorseData == null
        ? 1
        : Mathf.Max(1, HorseData.baseTripDays + GetUpgradeBonus(u => u.tripDaysChange));

    // Fora da fazenda, não dá pra interagir.
    public override bool CanMove => State != HorseState.Away;
    public override bool CanUpgrade => State != HorseState.Away && base.CanUpgrade;

    // Bolha extra do menu: abrir a cidade.
    public override bool HasExtraAction => true;
    public override bool CanDoExtraAction => State == HorseState.Idle;
    public override Sprite ExtraActionIcon => extraActionIcon;

    protected override void Awake()
    {
        base.Awake();

        if (HorseData == null)
            Debug.LogError($"[{name}] O campo Data precisa ser um HorseDataSO.", this);

        UpdateVisual();
    }

    public override void DoExtraAction()
    {
        if (State == HorseState.Idle && exportPanel != null)
            exportPanel.Open(this);
    }

    // Clique rápido: coleta o lucro quando voltou; ignora cliques enquanto está fora.
    public override bool TryQuickAction()
    {
        switch (State)
        {
            case HorseState.Returned: return CollectProfit();
            case HorseState.Away: return true;
            default: return false;
        }
    }

    // Chamado pelo painel da cidade. Tira os produtos do inventário e manda o cavalo.
    public bool TryStartTrip(IReadOnlyDictionary<ProductSO, int> items)
    {
        if (State != HorseState.Idle || InventoryManager.instance == null)
            return false;

        // Copia a carga: remover do inventário avisa o painel, que mexe na lista original.
        List<KeyValuePair<ProductSO, int>> cargo = new();
        foreach (var pair in items)
        {
            if (pair.Value > 0)
                cargo.Add(pair);
        }

        if (cargo.Count == 0)
            return false;

        // Valida tudo antes de remover qualquer coisa.
        int profit = 0;
        foreach (var pair in cargo)
        {
            if (!InventoryManager.instance.Has(pair.Key, pair.Value))
                return false;

            profit += pair.Key.sellPrice * pair.Value;
        }

        foreach (var pair in cargo)
            InventoryManager.instance.TryRemove(pair.Key, pair.Value);

        PendingProfit = profit;
        TripDaysRemaining = TripDays;
        State = HorseState.Away;
        UpdateVisual();
        NotifyChanged();
        return true;
    }

    public bool CollectProfit()
    {
        if (State != HorseState.Returned)
            return false;

        if (EconomyManager.instance != null)
            EconomyManager.instance.Add(PendingProfit);

        PendingProfit = 0;
        State = HorseState.Idle;
        UpdateVisual();
        NotifyChanged();
        return true;
    }

    protected override void OnDayTick()
    {
        if (State != HorseState.Away)
            return;

        TripDaysRemaining--;
        if (TripDaysRemaining <= 0)
        {
            TripDaysRemaining = 0;
            State = HorseState.Returned;
        }

        UpdateVisual();
        NotifyChanged();
    }

    protected override void OnUpgraded(int newLevel)
    {
        UpdateVisual(); // troca cavalo -> carroça -> carro
    }

    private void UpdateVisual()
    {
        if (levelVisuals == null || levelVisuals.Length == 0)
            return;

        int visible = Mathf.Min(CurrentLevel, levelVisuals.Length - 1);
        bool away = State == HorseState.Away;

        for (int i = 0; i < levelVisuals.Length; i++)
        {
            if (levelVisuals[i] != null)
                levelVisuals[i].SetActive(i == visible && !away);
        }
    }

    // ---- Testes rápidos: botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: status do cavalo")]
    private void DebugHorseStatus()
    {
        Debug.Log($"[{name}] Estado: {State} | Dias fora restantes: {TripDaysRemaining} | " +
                  $"Lucro pendente: {PendingProfit} | Nível: {CurrentLevel} | Viagem: {TripDays} dias", this);
    }

    [ContextMenu("Debug: coletar lucro")]
    private void DebugCollect()
    {
        Debug.Log($"[{name}] Coletou? {CollectProfit()}", this);
    }
}