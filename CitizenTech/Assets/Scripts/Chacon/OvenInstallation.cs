using UnityEngine;

public enum OvenState
{
    Idle,     // vazio, esperando uma fornada
    Baking,   // assando (conta os dias)
    Ready     // pronto: o jogador coleta o produto
}

public class OvenInstallation : Installation
{
    [Header("Visual / ícone")]
    [Tooltip("Opcional: objeto (fumaça, fogo...) ligado só enquanto está assando.")]
    [SerializeField] private GameObject bakingEffect;
    [SerializeField] private Sprite extraActionIcon;

    public OvenState State { get; private set; } = OvenState.Idle;
    public int DaysRemaining { get; private set; }

    private OvenDataSO OvenData => data as OvenDataSO;

    // Dados da receita, pro painel e o billboard lerem.
    public ProductSO InputProduct => OvenData != null ? OvenData.inputProduct : null;
    public ProductSO OutputProduct => OvenData != null ? OvenData.outputProduct : null;
    public int InputAmount => OvenData != null ? OvenData.inputAmount : 0;
    public int OutputAmount => OvenData != null
        ? Mathf.Max(1, OvenData.outputAmount + GetUpgradeBonus(u => u.ovenOutputChange)) : 0;
    public int BakeDays => OvenData != null
        ? Mathf.Max(1, OvenData.bakeDays + GetUpgradeBonus(u => u.bakeDaysChange)) : 1;

    // Bolha extra do menu: abrir a janela da fornada.
    public override bool HasExtraAction => true;
    public override bool CanDoExtraAction => State == OvenState.Idle;
    public override Sprite ExtraActionIcon => extraActionIcon;



    
    /*
    public int OutputAmount => OvenData != null
    ? Mathf.Max(1, OvenData.outputAmount + GetUpgradeBonus(u => u.ovenOutputChange)) : 0;
    public int BakeDays => OvenData != null
        ? Mathf.Max(1, OvenData.bakeDays + GetUpgradeBonus(u => u.bakeDaysChange)) : 1;
    */
    // Parado só conta no aviso de "passar o dia" se dava pra assar (tem ingredientes).
    public override bool IsIdle
    {
        get
        {
            if (State != OvenState.Idle || OvenData == null || InventoryManager.instance == null)
                return false;
            return InventoryManager.instance.Has(OvenData.inputProduct, OvenData.inputAmount);
        }
    }

    protected override void Awake()
    {
        base.Awake();

        if (OvenData == null)
            Debug.LogError($"[{name}] O campo Data precisa ser um OvenDataSO.", this);

        UpdateVisual();
    }

    public override void DoExtraAction()
    {
        // O forno é comprado (prefab instanciado), então não guarda referência de cena: usa o singleton.
        if (State == OvenState.Idle && OvenPanel.instance != null)
            OvenPanel.instance.Open(this);
    }

    // Clique rápido: coleta quando está pronto. Nos outros estados, abre o menu normal.
    public override bool TryQuickAction()
    {
        return State == OvenState.Ready && CollectOutput();
    }

    // Chamado pelo painel do forno: tira os ingredientes e começa a assar.
    public bool TryStartBaking()
    {
        if (State != OvenState.Idle || OvenData == null || InventoryManager.instance == null)
            return false;

        if (!InventoryManager.instance.TryRemove(OvenData.inputProduct, OvenData.inputAmount))
            return false;

        State = OvenState.Baking;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayBakeStartSFX();

        DaysRemaining = BakeDays;
        UpdateVisual();
        NotifyChanged();
        return true;
    }

    public bool CollectOutput()
    {
        if (State != OvenState.Ready || OvenData == null || InventoryManager.instance == null)
            return false;

        InventoryManager.instance.Add(OvenData.outputProduct, OutputAmount);

        State = OvenState.Idle;
        UpdateVisual();
        NotifyChanged();
        return true;
    }

    protected override void OnDayTick()
    {
        if (State != OvenState.Baking)
            return;

        DaysRemaining--;
        if (DaysRemaining <= 0)
        {
            DaysRemaining = 0;
            State = OvenState.Ready;

            if (AudioManager.instance != null)
                AudioManager.instance.PlayBakeDoneSFX();
        }

        UpdateVisual();
        NotifyChanged();
    }

    private void UpdateVisual()
    {
        if (bakingEffect != null)
            bakingEffect.SetActive(State == OvenState.Baking);
    }

    // ---- Testes rápidos: botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: status do forno")]
    private void DebugOvenStatus()
    {
        Debug.Log($"[{name}] Estado: {State} | Dias restantes: {DaysRemaining}", this);
    }

    [ContextMenu("Debug: coletar")]
    private void DebugCollect()
    {
        Debug.Log($"[{name}] Coletou? {CollectOutput()}", this);
    }
}