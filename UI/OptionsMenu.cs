using UnityEngine;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;

    public void SetMaster(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }

    public void SetMusic(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }

    public void SetSFX(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
    }
}