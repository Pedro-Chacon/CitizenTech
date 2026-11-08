using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    [Header("Volume")]
    [Range(0, 1)]

    public float masterVolume = 1;
    [Range(0, 1)]

    public float musicVolume = 1;
    [Range(0, 1)]

    public float SFXVolume = 1;
    [Range(0, 1)]

    public float UIVolume = 1;
    [Range(0, 1)]

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus uiBus;

    [Header("Music Config")]
    [SerializeField] private StudioEventEmitter gameplayMusic;
    [SerializeField] private StudioEventEmitter resultsMusic;

    [Header("UI Config")]
    [SerializeField] private StudioEventEmitter buttonUISound;
    [SerializeField] private StudioEventEmitter buttonConfirmUISound;
    [SerializeField] private StudioEventEmitter buttonUIHoverSound;

    [Header("SFX Config")]
    [SerializeField] private StudioEventEmitter objectPlacementSFX;

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("tem mais de um AudioListener na Cena!");
        }
        instance = this;

        LoadVolumes();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        uiBus = RuntimeManager.GetBus("bus:/UI");

    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        musicBus.setVolume(musicVolume);
        sfxBus.setVolume(SFXVolume);
        uiBus.setVolume(UIVolume);
    }
    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        UIVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
    }

    #region AUDIOS FUNÇÕES
    public void ShowResultsAudio()
    {
        gameplayMusic.Stop();
        resultsMusic.Play();
    }
    public void PlayUIButtonSound()
    {
        buttonUISound.Play();
    }
    public void PlayUIButtonConfirmSound()
    {
        buttonConfirmUISound.Play();
    }
    public void PlayUIButtonHoverSound()
    {
        buttonUIHoverSound.Play();
    }

    public void ObjectPlacementSFX()
    {
        objectPlacementSFX.Play();
    }

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