using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSourceSFXPrefab;
    [SerializeField] private AudioMixer _audioMixer;

    [HideInInspector] public Dictionary<string, AudioClip> LoadedSFXClips = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadNewClipsBundle(SoundClipsBundle soundsBundle)
    {
        for (int i = 0; i < soundsBundle.Clips.Count; i++)
        {
            LoadedSFXClips.Add(soundsBundle.Clips[i].name, soundsBundle.Clips[i]);
        }
    }

    public void ReleaseSoundClips()
    {
        LoadedSFXClips.Clear();
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
