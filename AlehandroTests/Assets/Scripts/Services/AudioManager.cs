using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceSFXPrefab;
    [SerializeField] private AudioMixer _audioMixer;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void PlaySFX(AudioClip audioClip, Transform spawnTransform)
    {
        AudioSource audioSource = Instantiate(_audioSourceSFXPrefab, spawnTransform.position, Quaternion.identity);
        audioSource.clip = audioClip;
        audioSource.volume = 1f;
        audioSource.Play();

        Destroy(audioSource.gameObject, audioSource.clip.length);
    }

    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(value) * 30f);
    }

    public void SetSFXVolume(float value)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(value) * 30f);
    }
}
