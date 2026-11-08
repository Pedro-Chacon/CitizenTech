using UnityEngine;
using UnityEngine.UI;
public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        MASTER,

        MUSIC,

        SFX,

        RESTAURANTNOISE,

        UI
    }

    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;

    private void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                volumeSlider.value = AudioManager.instance.masterVolume;
                break;
            case VolumeType.MUSIC:
                volumeSlider.value = AudioManager.instance.musicVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.SFXVolume;
                break;
            case VolumeType.UI:
                volumeSlider.value = AudioManager.instance.UIVolume;
                break;
            default:
                Debug.LogWarning("Volume Type Not supported: " + volumeType);
                break;
        }
    }


    public void OnSliderValueChanged()
    {
        switch (volumeType)
        {
            case VolumeType.MASTER:
                AudioManager.instance.masterVolume = volumeSlider.value;
                PlayerPrefs.SetFloat("MasterVolume", volumeSlider.value);
                break;

            case VolumeType.MUSIC:
                AudioManager.instance.musicVolume = volumeSlider.value;
                PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
                break;

            case VolumeType.SFX:
                AudioManager.instance.SFXVolume = volumeSlider.value;
                PlayerPrefs.SetFloat("SFXVolume", volumeSlider.value);
                break;

            case VolumeType.UI:
                AudioManager.instance.UIVolume = volumeSlider.value;
                PlayerPrefs.SetFloat("UIVolume", volumeSlider.value);
                break;

            default:
                Debug.LogWarning("Volume Type Not supported: " + volumeType);
                break;
        }

        PlayerPrefs.Save();
    }

}
