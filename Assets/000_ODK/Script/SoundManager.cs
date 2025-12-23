using Code.Core;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private AudioSource sfxSource;

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    public AudioClip[] audioClips;

    protected override void Awake()
    {
        base.Awake();

        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false; // 효과음은 기본적으로 루프하지 않음
    }


    public void PlaySFX(int index)
    {
        sfxSource.PlayOneShot(audioClips[index], masterVolume);
    }

}
