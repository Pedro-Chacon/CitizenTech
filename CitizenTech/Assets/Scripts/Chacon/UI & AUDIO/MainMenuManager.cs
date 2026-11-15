using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using MoreMountains.Tools;
using System.Collections;

public class MainMenuManager : MonoBehaviour
{
    [Header("Main Buttons")]
    [SerializeField] Button buttonPlay;
    [SerializeField] Button buttonConfigurations;
    [SerializeField] Button buttonCredits;
    [SerializeField] Button buttonExitGame;

    [Header("Configurations Panel")]
    [SerializeField] JuicyPanel panelConfigurations;
    [SerializeField] Button buttonOkConfigurations;

    [Header("Credits Panel")]
    [SerializeField] JuicyPanel panelCredits;
    [SerializeField] Button buttonCloseCredits;
    [SerializeField] ScrollRect creditsScroll;
    Coroutine _scrollRoutine;
    float _creditsRestX;

    private void Start()
    {
        Bind(buttonPlay, PlayGame, nameof(buttonPlay));
        Bind(buttonConfigurations, OpenConfigurations, nameof(buttonConfigurations));
        Bind(buttonCredits, OpenCredits, nameof(buttonCredits));
        Bind(buttonExitGame, ExitGame, nameof(buttonExitGame));

        Bind(buttonOkConfigurations, CloseConfigurations, nameof(buttonOkConfigurations));
        Bind(buttonCloseCredits, CloseCredits, nameof(buttonCloseCredits));

        // guarda a posição horizontal correta do conteúdo dos créditos
        if (creditsScroll != null && creditsScroll.content != null)
            _creditsRestX = creditsScroll.content.anchoredPosition.x;

        //esconder paineis
        if (panelConfigurations != null) panelConfigurations.Hide(true);
        if (panelCredits != null) panelCredits.Hide(true);
    }

    void PlayGame()
    {
        MMSceneLoadingManager.LoadScene("ChaconGameScene", "LoadingScreenCitizenTech");
    }

    void OpenConfigurations()
    {
        if (panelCredits != null) panelCredits.Hide();
        if (panelConfigurations != null) panelConfigurations.Show();
    }

    void CloseConfigurations()
    {
        if (panelConfigurations != null) panelConfigurations.Hide();
    }

    void OpenCredits()
    {
        if (panelConfigurations != null) panelConfigurations.Hide();
        if (panelCredits != null) panelCredits.Show();

        if (creditsScroll != null)
        {
            if (_scrollRoutine != null) StopCoroutine(_scrollRoutine);
            _scrollRoutine = StartCoroutine(KeepCreditsAtTop());
        }
    }

    // isso foi ia:
    // segura o scroll no topo e centralizado enquanto o painel faz a animação de abrir
    IEnumerator KeepCreditsAtTop()
    {
        float t = 0f;
        while (t < 0.6f)
        {
            creditsScroll.StopMovement();
            creditsScroll.verticalNormalizedPosition = 1f;

            Vector2 pos = creditsScroll.content.anchoredPosition;
            pos.x = _creditsRestX;
            creditsScroll.content.anchoredPosition = pos;

            t += Time.unscaledDeltaTime;
            yield return null;
        }
        _scrollRoutine = null;
    }

    void CloseCredits()
    {
        if (panelCredits != null) panelCredits.Hide();
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
}