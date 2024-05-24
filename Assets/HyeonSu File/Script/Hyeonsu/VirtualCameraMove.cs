using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class VirtualCameraMove : CinemachineExtension
{
    public CinemachineConfiner Cc;

    public RoomManager rm;
    public string Roomname;

    public PolygonCollider2D ply;
    public PolygonCollider2D Bossply;
    public PolygonCollider2D Bossply_2;
    public PolygonCollider2D VillagePly;
    public PolygonCollider2D VillagePly_2;

    public PolygonCollider2D Ply_Wind;
    public PolygonCollider2D Ply_Cloud;
    public PolygonCollider2D Ply_Rain;
    //public PolygonCollider2D BossPly;

    [SerializeField] private CinemachineVirtualCamera Cv;
    [SerializeField] private GameObject Player;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, 
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Cc.m_ConfineMode = CinemachineConfiner.Mode.Confine2D;

        rm = FindObjectOfType<RoomManager>();

        Cv = GetComponent<CinemachineVirtualCamera>();

        Player = GameObject.Find("Player");
    }
    // Update is called once per frame
    void Update()
    {
        Roomname = rm.Roomname;
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                break;
            case 1:
                Cc.m_BoundingShape2D = VillagePly;
                if (Roomname.Contains("Boss"))
                {
                    Invoke("CameraSize", 1f);
                    Cc.m_BoundingShape2D = Bossply;
                }
                else
                {
                    StartConfiner();
                }
                break;
            case 2:
                Cv.Follow = Player.transform;
                Cc.m_BoundingShape2D = VillagePly_2;
                if (Roomname.Contains("Boss"))
                {
                    Invoke("CameraSize_2", 1f);
                    Cc.m_BoundingShape2D = Bossply_2;
                }
                else
                {
                    StartConfiner();
                }
                break;
            case 4:
                Cv.Follow = Player.transform;
                if (Roomname.Contains("Boss"))
                {
                    Invoke("CameraSize_2", 1f);
                    Cc.m_BoundingShape2D = Ply_Wind;
                }
                else
                {
                    StartConfiner();
                }
                break;
            case 5:
                Cv.Follow = Player.transform;
                if (Roomname.Contains("Boss"))
                {
                    Invoke("CameraSize_2", 1f);
                    Cc.m_BoundingShape2D = Ply_Rain;
                }
                else
                {
                    StartConfiner();
                }
                break;
            case 6:
                Cv.Follow = Player.transform;
                if (Roomname.Contains("Boss"))
                {
                    Invoke("CameraSize_2", 1f);
                    Cc.m_BoundingShape2D = Ply_Cloud;
                }
                else
                {
                    StartConfiner();
                }
                break;
        }
    }

    public void StartConfiner()
    {
        if(Roomname != "")
        {
            ply = GameObject.Find(Roomname).GetComponentInChildren<PolygonCollider2D>();
            Cv.m_Lens.OrthographicSize = 2;
            Cc.m_BoundingShape2D = ply;
        }
        else
        {
            return;
        }
    }

    private void CameraSize()
    {
        Cv.m_Lens.OrthographicSize = 3;
    }

    private void CameraSize_2()
    {
        Cv.m_Lens.OrthographicSize = 2.4f;
    }
}
