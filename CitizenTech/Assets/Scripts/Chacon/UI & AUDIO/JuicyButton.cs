using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MoreMountains.Feedbacks;

public class JuicyButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    public enum ClickSound { Normal, Confirm, None }

    [Header("Feedbacks")]
    [SerializeField] MMF_Player hoverIn;
    [SerializeField] MMF_Player hoverOut;
    [SerializeField] MMF_Player clickDown;
    [SerializeField] MMF_Player clickUp;
    [SerializeField] RectTransform visual;

    [Header("Audio")]
    [SerializeField] ClickSound clickSound = ClickSound.Normal;
    [SerializeField] bool playHoverSound = true;

    bool _hovered;
    Button _button;

    Vector3 _restScale = Vector3.one;

    void Awake()
    {
        if (visual != null) _restScale = visual.localScale;

        SetUnscaled(hoverIn);
        SetUnscaled(hoverOut);
        SetUnscaled(clickDown);
        SetUnscaled(clickUp);

        _button = GetComponent<Button>();
        if (_button != null)
            _button.onClick.AddListener(PlayClickSound);
    }
    void SetUnscaled(MMF_Player player)
    {
        if (player == null) return;
        player.ForceTimescaleMode = true;
        player.ForcedTimescaleMode = TimescaleModes.Unscaled;
    }
    void OnDestroy()
    {
        if (_button != null)
            _button.onClick.RemoveListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        if (AudioManager.instance == null) return;

        switch (clickSound)
        {
            case ClickSound.Normal:
                AudioManager.instance.PlayUIButtonSound();
                break;
            case ClickSound.Confirm:
                AudioManager.instance.PlayUIButtonConfirmSound();
                break;
        }
    }

    void StopAll()
    {
        if (hoverIn != null) hoverIn.StopFeedbacks();
        if (hoverOut != null) hoverOut.StopFeedbacks();
        if (clickDown != null) clickDown.StopFeedbacks();
        if (clickUp != null) clickUp.StopFeedbacks();
    }

    public void OnPointerEnter(PointerEventData e)
    {
        if (_hovered) return;
        _hovered = true;

        StopAll();
        if (hoverIn != null) hoverIn.PlayFeedbacks();

        if (playHoverSound && AudioManager.instance != null)
            AudioManager.instance.PlayUIButtonHoverSound();
    }

    public void OnPointerExit(PointerEventData e)
    {
        if (!_hovered) return;
        _hovered = false;

        StopAll();
        if (hoverOut != null) hoverOut.PlayFeedbacks();
    }

    public void OnPointerDown(PointerEventData e)
    {
        StopAll();
        if (clickDown != null) clickDown.PlayFeedbacks();
    }

    public void OnPointerUp(PointerEventData e)
    {
        StopAll();
        if (_hovered) { if (clickUp != null) clickUp.PlayFeedbacks(); }
        else { if (hoverOut != null) hoverOut.PlayFeedbacks(); }
    }

    void OnDisable()
    {
        _hovered = false;
        StopAll();
        if (visual != null) visual.localScale = _restScale;
    }
}