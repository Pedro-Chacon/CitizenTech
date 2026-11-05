using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MoreMountains.Tools;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] Button buttonPlay;
    [SerializeField] Button buttonConfigurations;
    [SerializeField] Button buttonCredits;
    [SerializeField] Button buttonExitGame;
    //[SerializeField] Button buttonTutorial;


    [Header("Configurations Panel")]
    [SerializeField] GameObject panelConfigurations;
    [SerializeField] Button buttonCloseConfigurations; 
    [SerializeField] Button buttonOkConfigurations;    

    [Header("Credits Panel")]
    [SerializeField] GameObject panelCredits;
    [SerializeField] Button buttonCloseCredits;

    [SerializeField] ScrollRect creditsScroll;


    private void Start()
    {
        Bind(buttonPlay, PlayGame, nameof(buttonPlay));
        //Bind(buttonTutorial, PlayTutorial, nameof(buttonTutorial));
        Bind(buttonConfigurations, OpenConfigurations, nameof(buttonConfigurations));
        Bind(buttonCredits, OpenCredits, nameof(buttonCredits));
        Bind(buttonExitGame, ExitGame, nameof(buttonExitGame));


        
        Bind(buttonCloseConfigurations, CloseConfigurations, nameof(buttonCloseConfigurations));
        Bind(buttonOkConfigurations, CloseConfigurations, nameof(buttonOkConfigurations));

        Bind(buttonCloseCredits, CloseCredits, nameof(buttonCloseCredits));

        // fechar paineis
        SetPanel(panelConfigurations, false);
        SetPanel(panelCredits, false);


    }

    void PlayGame()
    {
        MMSceneLoadingManager.LoadScene("ChaconGameScene", "LoadingScreenCitizenTech");
    }

    void PlayTutorial()
    {
        MMSceneLoadingManager.LoadScene("ChaconGameScene", "LoadingScreenCitizenTech");
    }

    void OpenConfigurations()
    {
        SetPanel(panelCredits, false);
        SetPanel(panelConfigurations, true);
    }

    void CloseConfigurations()
    {
        SetPanel(panelConfigurations, false);
    }

    void OpenCredits()
    {
        SetPanel(panelConfigurations, false);
        SetPanel(panelCredits, true);

        if (creditsScroll != null)
            creditsScroll.verticalNormalizedPosition = 1f;
    }

    void CloseCredits()
    {
        SetPanel(panelCredits, false);
    }


    void ExitGame()
    {
        Application.Quit();

       
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }





    void Bind(Button button, UnityAction action, string label)
    {
        if (button == null)
        {
            Debug.LogWarning($"[MainMenuManager] Botão não atribuído no Inspector: {label}", this);
            return;
        }

        button.onClick.AddListener(action);
    }

    void SetPanel(GameObject panel, bool active)
    {
        if (panel != null)
            panel.SetActive(active);
    }
}