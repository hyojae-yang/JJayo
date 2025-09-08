using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

// 사운드 종류를 명확하게 관리하기 위한 열거형
public enum SFXType
{
    Button_Click,
    Cow_Moo,
    Cow_Die,
    Gun_Shot,
    Wolf_Appear,
    Wolf_Die,
    Item_Purchase,
    Trader_Appear,
    Trader_Yes,
    Trader_No,
    Monthly_Review,
    Item_Sell,
    Chicken,
    Game_Clear
}

// 사운드 클립과 타입을 연결하기 위한 직렬화 가능한 클래스
[Serializable]
public class SoundClip
{
    public SFXType sfxType;
    public AudioClip audioClip;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private ObjectPool sfxPool;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip gameClearBGM;
    [SerializeField] private List<SoundClip> sfxClips;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float bgmVolume = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 0.8f;

    private Dictionary<SFXType, AudioClip> sfxDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            InitializeSFXDictionary();
            // ★ 추가된 코드: 초기 BGM 설정
            PlayBGM("TitleScene");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSFXDictionary()
    {
        sfxDictionary = new Dictionary<SFXType, AudioClip>();
        foreach (SoundClip soundClip in sfxClips)
        {
            if (soundClip.audioClip != null)
            {
                sfxDictionary.Add(soundClip.sfxType, soundClip.audioClip);
            }
        }
    }

    // --- BGM 관련 메서드 ---
    public void PlayBGM(string sceneName)
    {
        if (bgmSource == null) return;

        AudioClip clipToPlay = null;
        switch (sceneName)
        {
            case "TitleScene":
                clipToPlay = titleBGM;
                break;
            case "MainScene":
                clipToPlay = mainBGM;
                break;
            default:
                break;
        }

        if (clipToPlay != null && bgmSource.clip != clipToPlay)
        {
            bgmSource.clip = clipToPlay;
            bgmSource.loop = true;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    public void PlayGameClearBGM()
    {
        if (bgmSource != null && gameClearBGM != null)
        {
            bgmSource.Stop();
            bgmSource.loop = false;
            bgmSource.clip = gameClearBGM;
            bgmSource.volume = bgmVolume;
            bgmSource.Play();
        }
    }

    // --- SFX 관련 메서드 ---
    public void PlaySFX(SFXType sfxType)
    {
        if (sfxPool == null || !sfxDictionary.ContainsKey(sfxType))
        {
            Debug.LogWarning($"SoundManager: {sfxType} 효과음을 재생할 수 없습니다. 풀이 없거나 클립이 할당되지 않았습니다.");
            return;
        }

        GameObject sfxObject = sfxPool.GetFromPool();
        AudioSource sfxSource = sfxObject.GetComponent<AudioSource>();

        if (sfxSource != null)
        {
            AudioClip clipToPlay = sfxDictionary[sfxType];
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(clipToPlay);
            StartCoroutine(ReturnToPoolAfterDelay(sfxObject, clipToPlay.length));
        }
        else
        {
            Debug.LogError("SoundManager: 풀에서 가져온 오브젝트에 AudioSource 컴포넌트가 없습니다.");
            sfxPool.ReturnToPool(sfxObject);
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxPool.ReturnToPool(obj);
    }

    // --- 볼륨 조절을 위한 외부 호출 메서드 ---
    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }
}