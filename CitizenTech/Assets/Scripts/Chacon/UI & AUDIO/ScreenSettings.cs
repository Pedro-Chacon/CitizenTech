using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenSettings : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    private const string WidthKey = "ScreenWidth";
    private const string HeightKey = "ScreenHeight";
    private const string FullscreenKey = "ScreenFullscreen";

    // Resoluções oferecidas (as maiores que o monitor são escondidas).
    private static readonly Vector2Int[] Presets =
    {
        new Vector2Int(1280, 720),
        new Vector2Int(1366, 768),
        new Vector2Int(1600, 900),
        new Vector2Int(1920, 1080),
        new Vector2Int(2560, 1440),
    };

    private readonly List<Vector2Int> available = new();

    // Aplica a escolha salva assim que o jogo abre, sem depender de nenhuma cena.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void ApplySaved()
    {
        if (!PlayerPrefs.HasKey(WidthKey))
            return;

        Apply(PlayerPrefs.GetInt(WidthKey),
              PlayerPrefs.GetInt(HeightKey),
              PlayerPrefs.GetInt(FullscreenKey, 1) == 1);
    }

    private static void Apply(int width, int height, bool fullscreen)
    {
        // Exclusive: em tela cheia a resolução escolhida vale de verdade (só no Windows;
        // nas outras plataformas vira tela cheia sem borda).
        Screen.SetResolution(width, height,
            fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed);
    }

    private void Awake()
    {
        BuildOptions();

        resolutionDropdown.onValueChanged.AddListener(_ => ApplyAndSave());
        fullscreenToggle.onValueChanged.AddListener(_ => ApplyAndSave());
    }

    // OnEnable roda toda vez que a janela de opções abre: mostra o valor atual.
    private void OnEnable()
    {
        resolutionDropdown.SetValueWithoutNotify(FindClosestIndex(Screen.width, Screen.height));
        fullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
    }

    private void BuildOptions()
    {
        int maxWidth = Display.main.systemWidth;
        int maxHeight = Display.main.systemHeight;

        available.Clear();
        foreach (Vector2Int preset in Presets)
        {
            if (preset.x <= maxWidth && preset.y <= maxHeight)
                available.Add(preset);
        }

        if (available.Count == 0)
            available.Add(Presets[0]);

        List<string> labels = new();
        foreach (Vector2Int r in available)
            labels.Add(Label(r));

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(labels);
    }

    private static string Label(Vector2Int r)
    {
        if (r.y == 720) return $"HD ({r.x}x{r.y})";
        if (r.y == 1080) return $"Full HD ({r.x}x{r.y})";
        if (r.y == 1440) return $"QHD ({r.x}x{r.y})";
        return $"{r.x}x{r.y}";
    }

    private int FindClosestIndex(int width, int height)
    {
        int best = 0;
        int bestDiff = int.MaxValue;

        for (int i = 0; i < available.Count; i++)
        {
            int diff = Mathf.Abs(available[i].x - width) + Mathf.Abs(available[i].y - height);
            if (diff < bestDiff)
            {
                bestDiff = diff;
                best = i;
            }
        }
        return best;
    }

    private void ApplyAndSave()
    {
        Vector2Int r = available[Mathf.Clamp(resolutionDropdown.value, 0, available.Count - 1)];
        bool fullscreen = fullscreenToggle.isOn;

        Apply(r.x, r.y, fullscreen);

        PlayerPrefs.SetInt(WidthKey, r.x);
        PlayerPrefs.SetInt(HeightKey, r.y);
        PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}