using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using System;

[System.Serializable]
public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    public Dictionary<int, AudioClip> _Sound = new Dictionary<int, AudioClip>();

    [SerializeField] private AudioClip SwordSound;
    [SerializeField] private AudioClip SwordSound_2;

    [SerializeField] private GameObject _MainCam;

    [SerializeField] private AudioSource SourceManager;
    [SerializeField] private AudioSource BGMSource;

    public AudioMixer audioMixer;

    public AudioMixerGroup BGMMixerGroup;
    public AudioMixerGroup EffectMixerGroup;

    public Slider BGMSlider;
    public Slider EffectSlider;

    #region Instance 설정 필드
    public static SoundManager Instance
    {
        get
        {
            if(null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(null == instance)
        {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);

        SourceManager.volume = 0.3f;
    }
    #endregion

    /// <summary>
    /// 씬 로드 이벤트
    /// </summary>
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoad;
    }

    private void SceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        _MainCam = Camera.main.gameObject;
        BGMSource = _MainCam.GetComponent<AudioSource>();

        SourceManager.outputAudioMixerGroup = EffectMixerGroup;
        BGMSource.outputAudioMixerGroup = BGMMixerGroup;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoad;
    }

    #region 오디오 클립 딕셔너리에 등록
    private void Start()
    {
        Regist(100, SwordSound);
        Regist(101, SwordSound_2);
        Regist(102 ,SwordSound);
        Regist(103, SwordSound_2);
        Regist(104, SwordSound);
        Regist(200, SwordSound_2);
        Regist(201, SwordSound);
        Regist(202, SwordSound_2);
        Regist(203, SwordSound);
        Regist(204, SwordSound_2);
    }
    #endregion

    // 오디오 클립, id 딕셔너리에 등록
    public void Regist(int id, AudioClip audioclip)
    {
        Debug.Assert(audioclip != null, "이미 존재하는 오디오클립입니다! AudioKey = " +
            id.ToString());

        if (_Sound.ContainsKey(id) == true)
        {
            Debug.Log("이미 등록된 오디오 클립입니다. AudioKey = " +
                id.ToString());
            return;
        }

        _Sound.Add(id, audioclip);
        //Debug.Log("등록 성공! AudioKey = " + id.ToString() + " AudioClip = " + audioclip.name.ToString());
    }

    // 등록된 딕셔너리에서 key를 찾아 전달된 audiosource에서 플레이
    public void PlayAudioClip(int id)
    {
        if(_Sound.ContainsKey(id) == false)
        {
            Debug.Log("등록되지 않은 오디오 클립입니다. AudioKey = " +
            id.ToString());
        }

        SourceManager.clip = _Sound[id];
        SourceManager.PlayOneShot(_Sound[id]);
    }

    //음량 조절(BGM)
    public void BGMAudioControl()
    {
        float BGMsound = BGMSlider.value;

        if (BGMsound == -40f) audioMixer.SetFloat("BGMVolume", -80);
        else
        {
            audioMixer.SetFloat("BGMVolume", BGMsound);
        }
    }

    // 음량 조절(효과음)
    public void EffectAudioControl()
    {
        float EffectSound = EffectSlider.value;

        if (EffectSound == -40f) audioMixer.SetFloat("EffectVolume", -80);
        else
        {
            audioMixer.SetFloat("EffectVolume", EffectSound);
        }
    }
}
