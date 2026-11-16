using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Intercepta o botão de passar o dia: se houver instalações paradas, pergunta antes (regra do GDD).
public class EndDayConfirmationPanel : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("O botão de passar o dia do HUD. Remova o OnClick antigo dele no Inspector.")]
    [SerializeField] private Button endDayButton;
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    // O SelectionManager consulta isso pra ignorar cliques no mundo enquanto a janela está aberta.
    public bool IsOpen { get; private set; }

    private void Start()
    {
        endDayButton.onClick.AddListener(RequestEndDay);
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Close);

        panel.Hide(true);
    }

    // Pode ser chamado por outros botões também.
    public void RequestEndDay()
    {
        if (IsOpen)
            return;
        if (TurnManager.instance == null || TurnManager.instance.IsBusy)
            return;

        string summary = GetIdleSummary(out int idleCount);
        if (idleCount == 0)
        {
            TurnManager.instance.EndDay();
            return;
        }

        messageText.text =
            $"Há {idleCount} instalação(ões) parada(s):\n{summary}\n\n" +
            "Passar o dia mesmo assim?";

        IsOpen = true;
        panel.Show();
    }

    private void Confirm()
    {
        Close();

        if (TurnManager.instance != null)
            TurnManager.instance.EndDay();
    }

    public void Close()
    {
        IsOpen = false;
        panel.Hide();
    }

    // Junta as instalações paradas por nome, ex.: "Trigo (x2), Forno".
    private static string GetIdleSummary(out int count)
    {
        Dictionary<string, int> groups = new();
        count = 0;

        foreach (Installation installation in FindObjectsByType<Installation>(FindObjectsSortMode.None))
        {
            // Ignora a cópia do fantasma do posicionamento (componente desativado).
            if (!installation.enabled || !installation.IsIdle)
                continue;

            string label = installation.Data != null && !string.IsNullOrEmpty(installation.Data.displayName)
                ? installation.Data.displayName
                : installation.name;

            groups[label] = groups.TryGetValue(label, out int amount) ? amount + 1 : 1;
            count++;
        }

        List<string> parts = new();
        foreach (var pair in groups)
            parts.Add(pair.Value > 1 ? $"{pair.Key} (x{pair.Value})" : pair.Key);

        return string.Join(", ", parts);
    }
}