using System;
using AYellowpaper.SerializedCollections;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    
    public AudioSource MainTrack;
    public AudioSource SecondaryTrack;

    private string _currentTrack;

    [SerializedDictionary("Name", "Audio")]
    public SerializedDictionary<string, AudioClip> Tracks;

    [Range(0f, 1f)]
    public float InitVolume = 1f;
    
    private float _volume;
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = value;
            if (SecondaryTrack.volume == 0 || SecondaryTrack.clip == null)
                MainTrack.volume = _volume;
            else
                SecondaryTrack.volume = _volume;
        }
    }

    public float TransitionTime = 0.5f;

    private void Awake()
    {
        Instance = this;
        Volume = InitVolume;
    }

    public void PlayAudio(string trackName)
    {
        _currentTrack = trackName;
        
        AudioSource nowPlaying = MainTrack;
        AudioSource target = SecondaryTrack;

        if (!nowPlaying.isPlaying)
        {
            nowPlaying = SecondaryTrack;
            target = MainTrack;
            MainTrack.clip = Tracks[trackName];
        }
        else
        {
            SecondaryTrack.clip = Tracks[trackName];
        }

        StartCoroutine(MixSources(nowPlaying, target));
    }

    public string GetCurrent() => _currentTrack;

    private IEnumerator MixSources(AudioSource nowPlaying, AudioSource target)
    {
        float percentage = 0;

        while (nowPlaying.volume > 0)
        {
            nowPlaying.volume = Mathf.Lerp(Volume, 0, percentage);
            percentage += Time.deltaTime / TransitionTime;
            yield return null;
        }

        nowPlaying.Pause();
        if (!target.isPlaying)
        {
            target.Play();
        }
        target.UnPause();
        percentage = 0;

        while (target.volume < Volume)
        {
            target.volume = Mathf.Lerp(0, Volume, percentage);
            percentage += Time.deltaTime / TransitionTime;
            yield return null;
        }
    }
}
