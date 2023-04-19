using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMaster(float volume)
    {
        _audioMixer.SetFloat("Master", volume);
    }

    public void SetMusic(float volume)
    {
        _audioMixer.SetFloat("Music", volume);
    }

    public void SetSFX(float volume)
    {
        _audioMixer.SetFloat("SFX", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }
}