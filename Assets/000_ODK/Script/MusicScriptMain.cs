using Code.Core;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoSingleton<BGMManager>
{
    private AudioSource musicSource;

    // 곡별로 재생 위치(float)를 저장하는 사전
    private Dictionary<AudioClip, float> musicHistory = new Dictionary<AudioClip, float>();

    [Header("Settings")]
    [Range(0f, 1f)] public float masterVolume = 1f;

    protected override void Awake()
    {
        base.Awake();
        // 오디오 소스 초기화
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true; // 배경음은 기본적으로 루프
    }

    /// <summary>
    /// 음악 재생 (이전 위치부터 재생)
    /// </summary>
    /// <param name="clip">재생할 클립</param>
    /// <param name="resetTime">true일 경우 처음부터 재생</param>
    public void PlayMusic(AudioClip clip, bool resetTime = false)
    {
        if (clip == null) return;

        // 1. 현재 재생 중인 곡이 있다면 위치 저장
        SaveCurrentTime();

        // 2. 이미 같은 곡이 재생 중이면 중복 실행 방지 (리셋이 아닐 때)
        if (musicSource.clip == clip && !resetTime) return;

        // 3. 곡 설정
        musicSource.clip = clip;

        // 4. 재생 위치 결정
        if (resetTime)
        {
            musicSource.time = 0f;
            if (musicHistory.ContainsKey(clip)) musicHistory[clip] = 0f;
        }
        else if (musicHistory.TryGetValue(clip, out float savedTime))
        {
            // 저장된 시간이 클립 길이를 넘지 않도록 방어 코드
            musicSource.time = savedTime < clip.length ? savedTime : 0f;
        }
        else
        {
            musicSource.time = 0f;
        }

        musicSource.Play();
    }

    /// <summary>
    /// 음악 일시 정지 (현재 위치 저장)
    /// </summary>
    public void StopMusic()
    {
        SaveCurrentTime();
        musicSource.Stop();
    }

    /// <summary>
    /// 특정 곡의 저장된 시간을 0으로 초기화
    /// </summary>
    public void ResetMusicTime(AudioClip clip)
    {
        if (clip == null) return;
        if (musicHistory.ContainsKey(clip))
        {
            musicHistory[clip] = 0f;
        }

        // 현재 재생 중인 곡이라면 즉시 0초로 이동
        if (musicSource.clip == clip) musicSource.time = 0f;
    }

    /// <summary>
    /// 모든 상태를 초기화하고 정지
    /// </summary>
    public void StopAndResetAll()
    {
        musicSource.Stop();
        musicSource.clip = null;
        musicHistory.Clear();
    }

    private void SaveCurrentTime()
    {
        if (musicSource.clip != null)
        {
            musicHistory[musicSource.clip] = musicSource.time;
        }
    }

    // 볼륨 조절용
    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        musicSource.volume = masterVolume;
    }
}