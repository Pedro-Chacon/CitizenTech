using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Reference Buttons")]
    [SerializeField] GameObject painelOptions;
    [SerializeField] GameObject painelCanvasMenu;

    [SerializeField] Button buttonPlay;
    [SerializeField] Button buttonOptions;
    [SerializeField] Button buttonReset;
    [SerializeField] Button buttonExitGame;

    //[SerializeField] Button buttonResetTutorial;

    [SerializeField] Button buttonCloseOptions;

    [Header("References")]


    private const string LoadingSceneName = "LoadingScreenCitizenTech";
    private const string GameSceneName = "ChaconGameScene";

    private void Start()
    {
        buttonPlay.onClick.AddListener(PlayGame);
        buttonOptions.onClick.AddListener(AbrirOptions);
        buttonReset.onClick.AddListener(ResetarScene);
        buttonCloseOptions.onClick.AddListener(FecharOptions);
        buttonExitGame.onClick.AddListener(IrParaMainMenu);
        //buttonResetTutorial.onClick.AddListener(ResetarSceneTutorial);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (painelOptions.activeSelf)
            {
                FecharOptions();
                return;
            }

            if (painelCanvasMenu.activeSelf)
                FecharMenuPause();
            else
                AbrirMenuPause();
        }
    }


    public void AbrirMenuPause()
    {
        painelOptions.SetActive(false);
        painelCanvasMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void FecharMenuPause()
    {
        painelOptions.SetActive(false);
        painelCanvasMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void PlayGame()
    {
        FecharMenuPause();
    }

    public void AbrirOptions()
    {
        painelOptions.SetActive(true);
        painelCanvasMenu.SetActive(false);
    }

    public void ResetarScene()
    {
        Time.timeScale = 1f; 
        MMSceneLoadingManager.LoadScene(GameSceneName, LoadingSceneName);
        DialogueManager.StopAllConversations();
    }

    /*
    public void ResetarSceneTutorial()
    {
        Time.timeScale = 1f; 
        MMSceneLoadingManager.LoadScene(TutorialGameSceneName, LoadingSceneName);
        DialogueManager.StopAllConversations();
    }
    */

    public void FecharOptions()
    {
        painelOptions.SetActive(false);
        painelCanvasMenu.SetActive(true);
    }

    public void IrParaMainMenu()
    {
        Time.timeScale = 1f; 
        MMSceneLoadingManager.LoadScene("MainMenuScene", LoadingSceneName);
    }
}