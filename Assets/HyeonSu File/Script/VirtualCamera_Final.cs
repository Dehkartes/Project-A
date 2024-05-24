using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VirtualCamera_Final : CinemachineExtension
{
    public CinemachineConfiner Cc;
    public CinemachineVirtualCamera Cv;

    protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cv.Follow = FinalStage.Instance.Player.transform;

        switch (FinalStage.Instance.Roomname)
        {
            case 0:
                Cc.m_BoundingShape2D = FinalStage.Instance.StartRoomCol;
                break;
            case 1:
                Cc.m_BoundingShape2D = FinalStage.Instance.BossRoomCol;
                Cv.m_Lens.OrthographicSize = 2.4f;
                break;
        }

    }
}
