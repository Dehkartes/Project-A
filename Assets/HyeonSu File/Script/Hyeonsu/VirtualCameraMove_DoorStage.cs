using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class VirtualCameraMove_DoorStage : CinemachineExtension
{
    public CinemachineVirtualCamera Cv;

    public PolygonCollider2D DoorStage_Ply;

    public CinemachineConfiner2D Cc_2D;

    private GameObject Player;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");

        Cc_2D.m_BoundingShape2D = DoorStage_Ply;

        Player.transform.position = Vector3.zero;

        Cv.Follow = Player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
