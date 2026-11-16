using System;
using System.Collections;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance { get; private set; }

    [SerializeField] private int startingDay = 1;

    [SerializeField] private DayTransition dayTransition;

    public int CurrentDay { get; private set; }
    public bool IsBusy { get; private set; }

    // Disparado quando o jogador encerra o dia: é aqui que as instalações avançam (Tick).
    public event Action<int> OnDayEnded;

    // Disparado quando o novo dia começa: HUD, billboards e recompensas reagem aqui.
    public event Action<int> OnDayStarted;

    // Pro botão de passar dia (e outros) se desabilitarem enquanto o dia está passando.
    public event Action<bool> OnBusyChanged;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogError("Tem mais de um TurnManager na cena!");
            return;
        }
        instance = this;
        CurrentDay = startingDay;
    }

    private void Start()
    {
        // Avisa o dia inicial pra quem já estiver escutando.
        OnDayStarted?.Invoke(CurrentDay);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    // Chamado pelo botão de passar o dia.
    public void EndDay()
    {
        if (IsBusy)
            return;

        StartCoroutine(EndDayRoutine());
    }

    private IEnumerator EndDayRoutine()
    {
        SetBusy(true);

        // 1) As instalações avançam um turno (construção, plantio, transporte).
        OnDayEnded?.Invoke(CurrentDay);

        // 2) Tributo: cobra no fim do dia. Se o jogo acabou, trava aqui.
        if (TributeManager.instance != null)
        {
            TributeManager.instance.ResolveEndOfDay(CurrentDay);
            if (TributeManager.instance.IsFinished)
                yield break; // fica ocupado de propósito: o painel de resultado assume
        }

        // 3) Transição pra noite + texto "Dia X".
        if (dayTransition != null)
            yield return dayTransition.PlayNight(CurrentDay + 1);
        else
            yield return null;

        // 4) O novo dia começa.
        CurrentDay++;
        OnDayStarted?.Invoke(CurrentDay);

        // 5) Volta pro dia.
        if (dayTransition != null)
            yield return dayTransition.PlayDay();

        SetBusy(false);
    }

    private void SetBusy(bool value)
    {
        IsBusy = value;
        OnBusyChanged?.Invoke(value);
    }

    // ---- Teste rápido: clique com o botão direito no componente, em Play Mode ----

    [ContextMenu("Debug: passar dia")]
    private void DebugEndDay()
    {
        EndDay();
    }
}