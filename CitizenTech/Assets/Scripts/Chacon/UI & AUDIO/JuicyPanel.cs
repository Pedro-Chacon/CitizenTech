using UnityEngine;
using MoreMountains.Feedbacks;

public class JuicyPanel : MonoBehaviour
{
    [SerializeField] RectTransform content;
    [SerializeField] MMF_Player showFeedback;
    [SerializeField] MMF_Player hideFeedback;

    [Tooltip("Marque se o ShowFeedback tem um Scale que faz o painel crescer a partir de 0")]
    [SerializeField] bool startFromZeroScale = true;

    bool _hiding;
    Vector3 _restScale = Vector3.one;

    void Awake()
    {
        if (content != null) _restScale = content.localScale;

        SetUnscaled(showFeedback);
        SetUnscaled(hideFeedback);

        if (hideFeedback != null)
            hideFeedback.Events.OnComplete.AddListener(OnHideComplete);
    }
    void SetUnscaled(MMF_Player player)
    {
        if (player == null) return;
        player.ForceTimescaleMode = true;
        player.ForcedTimescaleMode = TimescaleModes.Unscaled;
    }

    void OnDestroy()
    {
        if (hideFeedback != null)
            hideFeedback.Events.OnComplete.RemoveListener(OnHideComplete);
    }

    void OnHideComplete()
    {
        if (!_hiding) return;
        _hiding = false;

        // devolve a escala original, para o próximo Show nunca começar quebrado
        if (content != null) content.localScale = _restScale;
        gameObject.SetActive(false);
    }

    public void Show()
    {
        _hiding = false;
        gameObject.SetActive(true);

        if (hideFeedback != null) hideFeedback.StopFeedbacks();

        if (content != null)
            content.localScale = startFromZeroScale ? Vector3.zero : _restScale;

        if (showFeedback != null) showFeedback.PlayFeedbacks();
    }

    public void Hide(bool instant = false)
    {
        if (!gameObject.activeSelf) return;

        bool hasHideAnimation = hideFeedback != null
            && hideFeedback.FeedbacksList != null
            && hideFeedback.FeedbacksList.Count > 0;

        if (instant || !hasHideAnimation)
        {
            _hiding = false;
            if (content != null) content.localScale = _restScale;
            gameObject.SetActive(false);
            return;
        }

        if (_hiding) return;
        _hiding = true;
        if (showFeedback != null) showFeedback.StopFeedbacks();
        hideFeedback.PlayFeedbacks();
    }
}