using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)] public float masterVolume = 1;
    [Range(0, 1)] public float musicVolume = 1;
    [Range(0, 1)] public float ambienceVolume = 1;
    [Range(0, 1)] public float SFXVolume = 1;
    [Range(0, 1)] public float UIVolume = 1;

    private Bus masterBus;
    private Bus ambienceBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus uiBus;

    [Header("Music Config")]
    [SerializeField] private StudioEventEmitter gameplayMusic;
    [SerializeField] private StudioEventEmitter resultsMusic;

    [Header("Ambience Config")]
    [SerializeField] private StudioEventEmitter dayAmbience;
    [SerializeField] private StudioEventEmitter nightAmbience;

    [Header("Dia / Noite (mixagem)")]

    [Range(0, 1)][SerializeField] private float dayAmbienceLevel = 0.6f;
    [Range(0, 1)][SerializeField] private float nightAmbienceLevel = 1f;

    [Range(0, 1)][SerializeField] private float musicNightLevel = 0.25f;

    [SerializeField] private float ambienceFadeTime = 3f;

    [Header("UI Config")]
    [SerializeField] private StudioEventEmitter buttonUISound;
    [SerializeField] private StudioEventEmitter buttonConfirmUISound;
    [SerializeField] private StudioEventEmitter buttonUIHoverSound;

    [Header("SFX Config")]
    [SerializeField] private StudioEventEmitter objectPlacementSFX;
    [SerializeField] private StudioEventEmitter duckSFX;
    [SerializeField] private StudioEventEmitter rotateSFX;
    [SerializeField] private StudioEventEmitter purchaseSFX;
    [SerializeField] private StudioEventEmitter deniedSFX;
    [SerializeField] private StudioEventEmitter harvestSFX;
    [SerializeField] private StudioEventEmitter collectMoneySFX;
    [SerializeField] private StudioEventEmitter horseSFX;
    [SerializeField] private StudioEventEmitter carSFX;
    [SerializeField] private StudioEventEmitter bakeStartSFX;
    [SerializeField] private StudioEventEmitter bakeDoneSFX;
    [SerializeField] private StudioEventEmitter upgradeSFX;
    [SerializeField] private StudioEventEmitter plantSFX;
    [SerializeField] private StudioEventEmitter deleteSFX;

    [SerializeField] private StudioEventEmitter CutsceneAudio;
    public static AudioManager instance { get; private set; }

    // 0 = dia, 1 = noite. 
    private float nightBlend;
    private float nightTarget;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Tem mais de um AudioManager na Cena!");
        }
        instance = this;

        LoadVolumes();

        masterBus = RuntimeManager.GetBus("bus:/");
        ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        uiBus = RuntimeManager.GetBus("bus:/UI");
    }

    private void Start()
    {
        StartLoop(dayAmbience);
        StartLoop(nightAmbience);
        UpdateDayNightAudio();
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        ambienceBus.setVolume(ambienceVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(SFXVolume);
        uiBus.setVolume(UIVolume);

        UpdateDayNightAudio();
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        ambienceVolume = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        UIVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
    }

    public void SetNightAudio(bool night)
    {
        nightTarget = night ? 1f : 0f;
    }

    private void UpdateDayNightAudio()
    {
        nightBlend = Mathf.MoveTowards(
            nightBlend, nightTarget, Time.unscaledDeltaTime / Mathf.Max(0.01f, ambienceFadeTime));

        float dayGain = 1f - Smooth(0f, 0.7f, nightBlend);
        float nightGain = Smooth(0.3f, 1f, nightBlend);

        SetEmitterVolume(gameplayMusic, Mathf.Lerp(musicNightLevel, 1f, dayGain));
        SetEmitterVolume(dayAmbience, dayAmbienceLevel * dayGain);
        SetEmitterVolume(nightAmbience, nightAmbienceLevel * nightGain);
    }

    private static float Smooth(float from, float to, float value)
    {
        float t = Mathf.Clamp01((value - from) / (to - from));
        return t * t * (3f - 2f * t);
    }

    private static void SetEmitterVolume(StudioEventEmitter emitter, float volume)
    {
        if (emitter == null || !emitter.IsPlaying())
            return;

        emitter.EventInstance.setVolume(volume);
    }

    private static void StartLoop(StudioEventEmitter emitter)
    {
        if (emitter != null && !emitter.IsPlaying())
            emitter.Play();
    }

    private static void PlayEmitter(StudioEventEmitter emitter)
    {
        if (emitter != null)
            emitter.Play();
    }

    private static void StopEmitter(StudioEventEmitter emitter)
    {
        if (emitter != null)
            emitter.Stop();
    }

    #region AUDIOS FUNÇÕES
    public void ShowResultsAudio()
    {
        StopEmitter(gameplayMusic);
        StopEmitter(dayAmbience);
        StopEmitter(nightAmbience);
        PlayEmitter(resultsMusic);
    }

    public void StopMenuMusic() => StopEmitter(gameplayMusic);
    public void PlayUIButtonSound() => PlayEmitter(buttonUISound);
    public void PlayUIButtonConfirmSound() => PlayEmitter(buttonConfirmUISound);
    public void PlayUIButtonHoverSound() => PlayEmitter(buttonUIHoverSound);
    public void ObjectPlacementSFX() => PlayEmitter(objectPlacementSFX);
    public void PlayDuckSFX() => PlayEmitter(duckSFX);
    public void PlayRotateSFX() => PlayEmitter(rotateSFX);
    public void PlayPurchaseSFX() => PlayEmitter(purchaseSFX);
    public void PlayDeniedSFX() => PlayEmitter(deniedSFX);
    public void PlayHarvestSFX() => PlayEmitter(harvestSFX);
    public void PlayCollectMoneySFX() => PlayEmitter(collectMoneySFX);
    public void PlayHorseSFX() => PlayEmitter(horseSFX);
    public void PlayCarSFX() => PlayEmitter(carSFX);
    public void PlayBakeStartSFX() => PlayEmitter(bakeStartSFX);
    public void PlayBakeDoneSFX() => PlayEmitter(bakeDoneSFX);
    public void PlayUpgradeSFX() => PlayEmitter(upgradeSFX);
    public void PlayPlantSFX() => PlayEmitter(plantSFX);
    public void PlayDeleteSFX() => PlayEmitter(deleteSFX);
    public void PlayCutsceneAudio() => PlayEmitter(CutsceneAudio);
    #endregion

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }
}