using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [Header("Textos")]
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private TMP_Text dayText;
    [SerializeField] private TMP_Text tributeText;

    [Header("Next Day Button")]
    [SerializeField] private Button endDayButton;

    private void Start()
    {
        EconomyManager economy = EconomyManager.instance;
        if (economy != null)
        {
            economy.OnMoneyChanged += UpdateMoney;
            UpdateMoney(economy.Money);
        }
        else
        {
            Debug.LogWarning("[HUDController] EconomyManager não encontrado na cena.", this);
        }

        TurnManager turns = TurnManager.instance;
        if (turns != null)
        {
            turns.OnDayStarted += UpdateDay;
            turns.OnBusyChanged += UpdateEndDayButton;
            UpdateDay(turns.CurrentDay);
            UpdateEndDayButton(turns.IsBusy);
        }
        else
        {
            Debug.LogWarning("[HUDController] TurnManager não encontrado na cena.", this);
        }

        TributeManager tribute = TributeManager.instance;
        if (tribute != null)
            tribute.OnTributeChanged += RefreshTribute;
        else
            Debug.LogWarning("[HUDController] TributeManager não encontrado na cena.", this);

        RefreshTribute();
    }

    private void OnDestroy()
    {
        if (EconomyManager.instance != null)
            EconomyManager.instance.OnMoneyChanged -= UpdateMoney;

        if (TurnManager.instance != null)
        {
            TurnManager.instance.OnDayStarted -= UpdateDay;
            TurnManager.instance.OnBusyChanged -= UpdateEndDayButton;
        }

        if (TributeManager.instance != null)
            TributeManager.instance.OnTributeChanged -= RefreshTribute;
    }

    private void UpdateMoney(int money)
    {
        if (moneyText != null)
            moneyText.text = $"Moedas: {money}";
    }

    private void UpdateDay(int day)
    {
        if (dayText != null)
            dayText.text = $"Dia {day}";

        RefreshTribute(); // os dias que faltam dependem do dia atual
    }

    private void RefreshTribute()
    {
        if (tributeText == null)
            return;

        TributeManager tribute = TributeManager.instance;
        if (tribute == null || !tribute.HasTribute)
        {
            tributeText.text = "Tributo: -";
            return;
        }

        int day = TurnManager.instance != null ? TurnManager.instance.CurrentDay : 1;
        int left = tribute.DaysUntilDue(day);
        string when = left == 0 ? "hoje" : left == 1 ? "em 1 dia" : $"em {left} dias";
        tributeText.text = $"Tributo: {tribute.CurrentAmount} ({when})";
    }

    private void UpdateEndDayButton(bool busy)
    {
        if (endDayButton != null)
            endDayButton.interactable = !busy;
    }
}