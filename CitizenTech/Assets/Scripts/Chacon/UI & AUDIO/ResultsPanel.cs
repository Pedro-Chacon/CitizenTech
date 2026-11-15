using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultsPanel : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private JuicyPanel panel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    [Tooltip("Reaproveita o ResetScene e o GoToMainMenu que já existem no menu de pause.")]
    [SerializeField] private PauseMenuManager pauseMenu;

    [Header("Textos")]
    [SerializeField] private string gameOverTitle = "Game Over";
    [SerializeField, TextArea] private string gameOverMessage = "Você não conseguiu pagar o tributo de {0} moedas.";
    [SerializeField] private string victoryTitle = "Você venceu!";
    [SerializeField, TextArea] private string victoryMessage = "Todos os tributos foram pagos. A fazenda está salva!";

    private void Start()
    {
        retryButton.onClick.AddListener(pauseMenu.ResetScene);
        menuButton.onClick.AddListener(pauseMenu.GoToMainMenu);

        TributeManager tribute = TributeManager.instance;
        if (tribute != null)
        {
            tribute.OnGameOver += ShowGameOver;
            tribute.OnVictory += ShowVictory;
        }

        panel.Hide(true);
    }

    private void OnDestroy()
    {
        if (TributeManager.instance != null)
        {
            TributeManager.instance.OnGameOver -= ShowGameOver;
            TributeManager.instance.OnVictory -= ShowVictory;
        }
    }

    private void ShowGameOver()
    {
        Show(gameOverTitle, string.Format(gameOverMessage, TributeManager.instance.CurrentAmount));
    }

    private void ShowVictory()
    {
        Show(victoryTitle, victoryMessage);
    }

    private void Show(string title, string message)
    {
        titleText.text = title;
        messageText.text = message;
        panel.Show();
    }
}