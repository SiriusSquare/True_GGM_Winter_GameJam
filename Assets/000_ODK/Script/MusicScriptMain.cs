using UnityEngine;
using System.Collections.Generic;
public class MusicScriptMain : MonoBehaviour
{
    public static MusicScriptMain Instance { get; private set; }

    private AudioSource audioSource;

    private List<AudioClip> musicClips = new List<AudioClip>();
    private List<float> musicDuration = new List<float>();
    public float AudioAbsSound { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip cur, bool iszerotime = false)
    {
        if (cur == null) return;

        if (audioSource.clip != null)
        {
            for (int i = 0; i < musicClips.Count; i++)
            {
                if (musicClips[i] == audioSource.clip)
                {
                    musicDuration[i] = audioSource.time;
                    break;
                }
            }
        }

        if (cur == audioSource.clip) return;

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        for (int i = 0; i < musicClips.Count; i++)
        {
            if (musicClips[i] == cur)
            {
                audioSource.clip = cur;
                audioSource.time = musicDuration[i];
                if (iszerotime)
                {
                    musicDuration[i] = 0f;
                    audioSource.time = 0f;
                }

                audioSource.Play();
                return;
            }
        }

        musicClips.Add(cur);
        musicDuration.Add(0f);
        audioSource.clip = cur;

        audioSource.Play();

    }

    public void StopMusic(AudioClip cur)
    {
        if (cur == null) return;

        for (int i = 0; i < musicClips.Count; i++)
        {
            if (musicClips[i] == audioSource.clip)
            {
                musicDuration[i] = audioSource.time;
                audioSource.Stop();
                return;
            }
        }

    }
    public void MusicTimeBack(AudioClip cur)
    {
        if (cur == null) return;
        for (int i = 0; i < musicClips.Count; i++)
        {
            if (musicClips[i] == cur)
            {
                audioSource.time = 0;
                musicDuration[i] = audioSource.time;
                return;
            }
        }
    }
    public void StopAndReset()
    {

        if (audioSource.isPlaying)
        {
            audioSource.time = 0f;
            audioSource.Stop();

        }

    }
}
