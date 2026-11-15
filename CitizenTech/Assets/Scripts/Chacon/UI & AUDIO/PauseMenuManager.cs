using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] JuicyPanel panelPause;
    [SerializeField] JuicyPanel panelOptions;

    [Header("Pause Buttons")]
    [SerializeField] Button buttonPlay;
    [SerializeField] Button buttonOptions;
    [SerializeField] Button buttonReset;
    [SerializeField] Button buttonExitGame;

    [Header("Options Buttons")]
    [SerializeField] Button buttonCloseOptions;

    private const string LoadingSceneName = "LoadingScreenCitizenTech";
    private const string GameSceneName = "ChaconGameScene";
    private const string MainMenuSceneName = "MainMenuScene";

    bool _paused;
    bool _optionsOpen;

    private void Start()
    {
        Bind(buttonPlay, ResumeGame, nameof(buttonPlay));
        Bind(buttonOptions, OpenOptions, nameof(buttonOptions));
        Bind(buttonReset, ResetScene, nameof(buttonReset));
        Bind(buttonExitGame, GoToMainMenu, nameof(buttonExitGame));
        Bind(buttonCloseOptions, CloseOptions, nameof(buttonCloseOptions));

        if (panelPause != null) panelPause.Hide(true);
        if (panelOptions != null) panelOptions.Hide(true);
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (!_paused) OpenPauseMenu();
        else if (_optionsOpen) CloseOptions();
        else ResumeGame();
    }

    public void OpenPauseMenu()
    {
        _paused = true;
        _optionsOpen = false;
        Time.timeScale = 0f;

        if (panelOptions != null) panelOptions.Hide(true);
        if (panelPause != null) panelPause.Show();
    }

    public void ResumeGame()
    {
        _paused = false;
        _optionsOpen = false;
        Time.timeScale = 1f;

        if (panelOptions != null) panelOptions.Hide();
        if (panelPause != null) panelPause.Hide();
    }

    public void OpenOptions()
    {
        _optionsOpen = true;

        if (panelOptions != null) panelOptions.Show();
    }

    public void CloseOptions()
    {
        _optionsOpen = false;

        if (panelOptions != null) panelOptions.Hide();

    }

    public void ResetScene()
    {
        Time.timeScale = 1f;
        MMSceneLoadingManager.LoadScene(GameSceneName, LoadingSceneName);
        DialogueManager.StopAllConversations();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        MMSceneLoadingManager.LoadScene(MainMenuSceneName, LoadingSceneName);
    }

    void Bind(Button button, UnityAction action, string label)
    {
        if (button == null)
        {
            Debug.LogWarning($"[PauseMenuManager] Botão não atribuído no Inspector: {label}", this);
            return;
        }
        button.onClick.AddListener(action);
    }
}