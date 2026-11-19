using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellConfirmationPanel : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private InstallationMenu menu;
    [SerializeField] private PlacementSystem placementSystem;
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Installation target;

    // O SelectionManager consulta isso pra ignorar cliques no mundo enquanto a janela está aberta.
    public bool IsOpen { get; private set; }

    private void Start()
    {
        menu.OnSellRequested += Open;
        confirmButton.onClick.AddListener(Confirm);
        cancelButton.onClick.AddListener(Close);

        panel.Hide(true);
    }

    private void OnDestroy()
    {
        if (menu != null)
            menu.OnSellRequested -= Open;
    }

    private void Open(Installation installation)
    {
        if (installation == null || !installation.CanSell)
            return;

        target = installation;
        IsOpen = true;

        string itemName = installation.Data != null && !string.IsNullOrEmpty(installation.Data.displayName)
            ? installation.Data.displayName
            : installation.name;

        messageText.text =
            $"Vender {itemName}?\n" +
            $"A construção será deletada e você receberá {installation.GetSellValue()} moedas.";

        panel.Show();
    }

    private void Confirm()
    {
        // Não vende enquanto o dia está passando.
        bool busy = TurnManager.instance != null && TurnManager.instance.IsBusy;
        if (!busy && target != null)
        {
            placementSystem.SellInstallation(target);

            if (AudioManager.instance != null)
                AudioManager.instance.PlayDeleteSFX();   // NOVO
        }

        Close();
    }

    public void Close()
    {
        target = null;
        IsOpen = false;
        panel.Hide();
    }
}