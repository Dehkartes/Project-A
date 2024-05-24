using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Data
{

    // 버튼에 이벤트 할당을 위해
    public static GameObject AimJoyStick;
    public static GameObject MeleeButton;

    // 플레이어
    public static GameObject Player;
    // 플레이어 클래스
    public static PlayerMovement Pm;

    // 에임 조이스틱
    public static Joystick joystick;

    public static Joystick Movejoystick;

    // 상호작용 버튼
    public static Button InteractionBtn;

    // 조이스틱 UI 캔버스 저장
    public static Canvas JoyStickUi;
}
