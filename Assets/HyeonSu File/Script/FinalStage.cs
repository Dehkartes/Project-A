using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalStage : MonoBehaviour
{
    private static FinalStage instance = null;

    public GameObject Player;

    public int Roomname;

    // 카메라 콜라이더 설정
    public PolygonCollider2D StartRoomCol;
    public PolygonCollider2D BossRoomCol;

    public TimelineAsset Ta;
    public PlayableDirector Pd;

    // 컷신 실행 함수 한번만
    private bool CheckCutScene_F = false;

    private bool EndCutScene_F = false;

    private bool CheckBoss_F = false;

    private bool CheckBossSpawn_F = false;

    private GameObject Canvas;
    private GameObject JoyStickCanvas;

    public GameObject FinalBossStatUI;

    public GameObject FinalBoss;

    public Image BossHpImage_F;

    public GameObject EndingObject;

    public AudioSource AudioSource_F;

    public AudioClip StartRoomClip;
    public AudioClip BossRoomClip;

    #region Instance
    public static FinalStage Instance
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
        if(instance == null)
            instance = this;
    }
    #endregion


    void Start()
    {
        Player = GameObject.Find("Player");

        Player.transform.position = new Vector3(-20, 0, 0);

        Canvas = GameObject.Find("Canvas");

        JoyStickCanvas = GameObject.Find("JoyStickUi");

        FinalBossStatUI.SetActive(false);

        AudioSource_F.clip = StartRoomClip;
    }

    void Update()
    {
        if(Roomname == 1)
        {
            if(CheckCutScene_F == false)
            {
                CheckCutScene_F = true;
                Pd.Play(Ta);

                Canvas.SetActive(false);
                JoyStickCanvas.SetActive(false);

                Player.transform.position = new Vector3(0, -0.5f, 0);
                SetLayers(Player.transform, 12);
            }

            if (EndCutScene_F)
            {
                if(CheckBoss_F == false)
                {
                    CheckBoss_F = true;

                    FinalBoss.SetActive(true);

                    FinalBossStatUI.SetActive(true);
                    Canvas.SetActive(true);
                    JoyStickCanvas.SetActive(true);

                    SetLayers(Player.transform, 6);

                    Player.transform.position = new Vector3(0, -0.5f, 0);
                }
            }

            if(GameObject.FindWithTag("Boss_Last") == null && CheckBossSpawn_F)
            {
                // 엔딩 타임라인 제작...
                CheckBossSpawn_F = false;

                FinalBossStatUI.SetActive(false);

                EndingObject.SetActive(true);
            }
        }
    }

    public void EndCutScene_Final()
    {
        EndCutScene_F = true;
        Debug.Log(EndCutScene_F);
        Debug.Log("체크 컷신_파이널");
        Invoke("CheckBossSpawn_Final", 0.5f);
    }

    private void CheckBossSpawn_Final()
    {
        Debug.Log("보스 스폰 체크_파이널");
        CheckBossSpawn_F = true;

        CancelInvoke();
    }

    // 자식 레이어까지 변경
    public void SetLayers(Transform trans, int layerindex)
    {
        trans.gameObject.layer = layerindex;
        foreach (Transform child in trans)
        {
            SetLayers(child, layerindex);
        }
    }

    public void EndingBtn()
    {
        EndingObject.SetActive(false);

        SceneManager.LoadScene(0);
    }
}
