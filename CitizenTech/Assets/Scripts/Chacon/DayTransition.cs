using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class DayTransition : MonoBehaviour
{
    [Header("Referências")]
    [Tooltip("Global Volume da noite (começa com Weight 0).")]
    [SerializeField] private Volume nightVolume;
    [SerializeField] private CanvasGroup textGroup;
    [SerializeField] private TMP_Text dayText;

    [Header("Tempos (segundos)")]
    [SerializeField] private float fadeToNightTime = 1.2f;
    [SerializeField] private float textFadeInTime = 0.5f;
    [SerializeField] private float holdTime = 1.2f;
    [SerializeField] private float textFadeOutTime = 0.5f;
    [SerializeField] private float fadeToDayTime = 1.2f;

    private void Awake()
    {
        if (nightVolume != null)
            nightVolume.weight = 0f;

        if (textGroup != null)
        {
            textGroup.alpha = 0f;
            textGroup.interactable = false;
            textGroup.blocksRaycasts = false;
        }
    }

    public IEnumerator PlayNight(int nextDay)
    {
        if (dayText != null)
            dayText.text = $"Dia {nextDay}";

        if (AudioManager.instance != null)
            AudioManager.instance.SetNightAudio(true);   // NOVO

        yield return Tween(0f, 1f, fadeToNightTime, v => SetNight(v));
        yield return Tween(0f, 1f, textFadeInTime, v => SetText(v));
        yield return new WaitForSecondsRealtime(holdTime);
    }

    public IEnumerator PlayDay()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetNightAudio(false);  // NOVO

        yield return Tween(1f, 0f, textFadeOutTime, v => SetText(v));
        yield return Tween(1f, 0f, fadeToDayTime, v => SetNight(v));
    }




    private void SetNight(float value)
    {
        if (nightVolume != null)
            nightVolume.weight = value;
    }

    private void SetText(float value)
    {
        if (textGroup != null)
            textGroup.alpha = value;
    }

    // Tempo não escalado: funciona mesmo se o timeScale mudar.
    private IEnumerator Tween(float from, float to, float duration, Action<float> apply)
    {
        if (duration <= 0f)
        {
            apply(to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            apply(Mathf.Lerp(from, to, Mathf.Clamp01(t / duration)));
            yield return null;
        }
        apply(to);
    }


}