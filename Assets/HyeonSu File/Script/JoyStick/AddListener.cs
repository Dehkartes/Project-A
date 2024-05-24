using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AddListener : MonoBehaviour
{
    // 격파, 죽음 메뉴 버튼(메인메뉴로, 재시작)
    [SerializeField] private Button NextBtn;
    [SerializeField] private Button MainBtn;
    [SerializeField] private Button DeathMainBtn;
    [SerializeField] private Button ReStartBtn;

    // 사운드 옵션 버튼
    [SerializeField] private Button SoundOptionBtn;
    [SerializeField] private Button SoundOptionCloseBtn;
    // 사운드 옵션 창
    [SerializeField] private GameObject SoundOptionPanel;

    [SerializeField] private GameObject AimJoyStick;
    [SerializeField] private GameObject Meleebutton;

    // 상호작용 버튼(씬 데이터에 전달)
    [SerializeField] private Button InteractionBtn;

    private void Awake()
    {

    }

    private void Start()
    {
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoad;
    }

    private void SceneLoad(Scene scene, LoadSceneMode mode)
    {
        RoomManager Rm = FindObjectOfType<RoomManager>();

        if(SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (gameObject.name == "Canvas")
            {
                SoundOptionBtn.onClick.AddListener(SoundOption);
                SoundOptionCloseBtn.onClick.AddListener(CloseSoundOption);
                if (SceneManager.GetActiveScene().buildIndex != 3)
                {                 
                    NextBtn.onClick.AddListener(Rm.NextBtn);
                    MainBtn.onClick.AddListener(Rm.MainBtn);
                    ReStartBtn.onClick.AddListener(Rm.ReStartBtn);
                    DeathMainBtn.onClick.AddListener(Rm.MainBtn);
                }
            }
            else
            {
                Data.AimJoyStick = AimJoyStick;
                Data.MeleeButton = Meleebutton;
                Data.InteractionBtn = InteractionBtn;
                Data.JoyStickUi = GetComponent<Canvas>();
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoad;
    }

    private void SoundOption()
    {
        SoundOptionPanel.SetActive(true);
    }

    private void CloseSoundOption()
    {
        SoundOptionPanel.SetActive(false);
    }
}
