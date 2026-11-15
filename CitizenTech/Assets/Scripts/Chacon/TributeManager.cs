using System;
using UnityEngine;

public enum TributeResult
{
    NotDue,     // ainda não é o dia do tributo
    Paid,       // pagou e ainda há tributos pela frente
    Failed,     // sem dinheiro: game over
    FinalPaid   // pagou o último: vitória
}

public class TributeManager : MonoBehaviour
{
    public static TributeManager instance { get; private set; }

    [SerializeField] private TributeScheduleSO schedule;

    private int currentIndex;

    public bool IsFinished { get; private set; }

    public bool HasTribute => schedule != null && currentIndex < schedule.tributes.Count;
    public int CurrentAmount => HasTribute ? schedule.tributes[currentIndex].amount : 0;
    public int CurrentDueDay => HasTribute ? schedule.tributes[currentIndex].dueDay : 0;

    public int DaysUntilDue(int currentDay) => Mathf.Max(0, CurrentDueDay - currentDay);

    // O próximo tributo mudou (HUD escuta).
    public event Action OnTributeChanged;
    public event Action<int> OnTributePaid;
    public event Action OnGameOver;
    public event Action OnVictory;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Tem mais de um TributeManager na cena!");
            return;
        }
        instance = this;
    }

    private void Start()
    {
        if (schedule == null || schedule.tributes.Count == 0)
            Debug.LogError("[TributeManager] Nenhum TributeSchedule com tributos configurado.", this);

        OnTributeChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // Chamado pelo TurnManager ao fim do dia, depois que as instalações trabalharam.
    public TributeResult ResolveEndOfDay(int day)
    {
        if (IsFinished || !HasTribute || day < CurrentDueDay)
            return TributeResult.NotDue;

        int amount = CurrentAmount;

        if (EconomyManager.instance == null || !EconomyManager.instance.TrySpend(amount))
        {
            IsFinished = true;
            OnGameOver?.Invoke();
            return TributeResult.Failed;
        }

        OnTributePaid?.Invoke(amount);
        currentIndex++;

        if (!HasTribute)
        {
            IsFinished = true;
            OnVictory?.Invoke();
            return TributeResult.FinalPaid;
        }

        OnTributeChanged?.Invoke();
        return TributeResult.Paid;
    }
}