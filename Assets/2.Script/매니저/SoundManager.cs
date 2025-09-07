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
    Game_Clear // GameManager에서 직접 호출하지만, 클립 관리를 위해 포함
}
//SoundManager.Instance.PlaySFX(SFXType.Button_Click);//다른 스크립트에서 사용 예시
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
    [SerializeField] private ObjectPool sfxPool; // 같은 오브젝트에 부착된 ObjectPool 스크립트

    [Header("Audio Clips")]
    [SerializeField] private AudioClip titleBGM;
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip gameClearBGM;
    [SerializeField] private List<SoundClip> sfxClips;

    private Dictionary<SFXType, AudioClip> sfxDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeSFXDictionary();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ★★★ 수정된 부분: "BGM" 태그를 가진 오브젝트를 찾습니다. ★★★
        GameObject bgmObject = GameObject.FindWithTag("BGM");
        if (bgmObject != null)
        {
            bgmSource = bgmObject.GetComponent<AudioSource>();
            if (bgmSource != null)
            {
                // BGM 루프 설정
                bgmSource.loop = true;
                PlayBGMByScene(scene.name);
            }
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
    public void PlayBGMByScene(string sceneName)
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

    // GameManager에서 호출하여 게임 클리어 BGM을 재생
    public void PlayGameClearBGM()
    {
        if (bgmSource != null && gameClearBGM != null)
        {
            bgmSource.Stop();
            bgmSource.loop = false;
            bgmSource.clip = gameClearBGM;
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
            sfxSource.PlayOneShot(clipToPlay);
            StartCoroutine(ReturnToPoolAfterDelay(sfxObject, clipToPlay.length));
        }
        else
        {
            Debug.LogError("SoundManager: 풀에서 가져온 오브젝트에 AudioSource 컴포넌트가 없습니다.");
            sfxPool.ReturnToPool(sfxObject); // 문제가 발생해도 풀로 반환
        }
    }

    private IEnumerator ReturnToPoolAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        sfxPool.ReturnToPool(obj);
    }

    public void SetVolume(float bgmVolume, float sfxVolume)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = bgmVolume;
        }
        if (sfxPool != null)
        {
            // 풀의 모든 AudioSource 볼륨 조절 (선택 사항)
        }
    }
}