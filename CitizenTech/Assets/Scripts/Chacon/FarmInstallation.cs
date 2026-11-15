using UnityEngine;

public enum PlantationState
{
    Idle,     // terra arada, esperando o jogador plantar
    Growing,  // plantada, crescendo a cada dia
    Ready     // pronta pra colher
}

public class FarmInstallation : Installation
{
    [Header("Visuais (filhos do prefab)")]
    [Tooltip("Terra arada vazia, mostrada enquanto espera.")]
    [SerializeField] private GameObject idleVisual;

    [Tooltip("Em ordem: terra com semente, fase 1, fase 2... O ÚLTIMO é o visual de 'pronta'.")]
    [SerializeField] private GameObject[] growthVisuals;

    public PlantationState State { get; private set; } = PlantationState.Idle;
    public int DaysGrown { get; private set; }

    private PlantationDataSO PlantationData => data as PlantationDataSO;

    public int GrowthDays => PlantationData != null ? PlantationData.growthDays : 1;
    public int DaysRemaining => Mathf.Max(0, GrowthDays - DaysGrown);

    // Plantação esperando o jogador conta como "parada" no aviso de passar o dia.
    public override bool IsIdle => State == PlantationState.Idle;

    protected override void Awake()
    {
        base.Awake();

        if (PlantationData == null)
            Debug.LogError($"[{name}] O campo Data precisa ser um PlantationDataSO.", this);

        UpdateVisual(); // também faz o preview (cópia do prefab) aparecer só como terra arada
    }

    // Chamado pelo ícone de "plantar" do billboard.
    public bool Plant()
    {
        if (State != PlantationState.Idle)
            return false;

        State = PlantationState.Growing;
        DaysGrown = 0;
        UpdateVisual();
        NotifyChanged();
        return true;
    }

    // Chamado pelo ícone de "colher" do billboard.
    public bool Harvest()
    {
        if (State != PlantationState.Ready || InventoryManager.instance == null || PlantationData == null)
            return false;

        InventoryManager.instance.Add(PlantationData.product, PlantationData.yieldPerHarvest);

        State = PlantationState.Idle;
        DaysGrown = 0;
        UpdateVisual();
        NotifyChanged();
        return true;
    }

    protected override void OnDayTick()
    {
        if (State != PlantationState.Growing)
            return;

        DaysGrown++;
        if (DaysGrown >= GrowthDays)
            State = PlantationState.Ready;

        UpdateVisual();
        NotifyChanged();
    }

    private void UpdateVisual()
    {
        if (idleVisual != null)
            idleVisual.SetActive(State == PlantationState.Idle);

        if (growthVisuals == null || growthVisuals.Length == 0)
            return;

        int last = growthVisuals.Length - 1;
        int shown = -1;

        if (State == PlantationState.Ready)
        {
            shown = last;
        }
        else if (State == PlantationState.Growing)
        {
            // Distribui as fases antes da última ao longo dos dias de crescimento.
            float progress = Mathf.Clamp01((float)DaysGrown / GrowthDays);
            shown = Mathf.Min(Mathf.FloorToInt(progress * last), last);
        }

        for (int i = 0; i < growthVisuals.Length; i++)
        {
            if (growthVisuals[i] != null)
                growthVisuals[i].SetActive(i == shown);
        }
    }

    public override bool TryQuickAction()
    {
        if (State == PlantationState.Idle) return Plant();
        if (State == PlantationState.Ready) return Harvest();
        return false;
    }

    // ---- Testes rápidos: botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: plantar")]
    private void DebugPlant()
    {
        Debug.Log($"[{name}] Plantou? {Plant()} | Estado: {State}", this);
    }

    [ContextMenu("Debug: colher")]
    private void DebugHarvest()
    {
        Debug.Log($"[{name}] Colheu? {Harvest()} | Estado: {State}", this);
    }

    [ContextMenu("Debug: status da plantação")]
    private void DebugFarmStatus()
    {
        Debug.Log($"[{name}] Estado: {State} | Dias: {DaysGrown}/{GrowthDays}", this);
    }
}