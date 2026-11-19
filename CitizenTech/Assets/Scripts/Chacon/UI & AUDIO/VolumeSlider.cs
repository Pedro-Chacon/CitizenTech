using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    // Atenção: a ordem não pode mudar, o Unity guarda o número de cada tipo nos sliders da cena.
    private enum VolumeType
    {
        MASTER,

        MUSIC,

        SFX,

        AMBIENCE,

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
            case VolumeType.AMBIENCE:
                volumeSlider.value = AudioManager.instance.ambienceVolume;
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

            case VolumeType.AMBIENCE:
                AudioManager.instance.ambienceVolume = volumeSlider.value;
                PlayerPrefs.SetFloat("AmbienceVolume", volumeSlider.value);
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