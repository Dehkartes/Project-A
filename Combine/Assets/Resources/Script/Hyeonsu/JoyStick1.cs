using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick1 : MonoBehaviour
{
    // Start is called before the first frame update

    // ����
    public Transform Player;
    public Transform Stick;         // ���̽�ƽ.


    // �����
    private Vector3 StickFirstPos;  // ���̽�ƽ�� ó�� ��ġ.
    private Vector3 JoyVec;         // ���̽�ƽ�� ����(����)
    private float Radius;           // ���̽�ƽ ����� �� ����.
    private PlayerMovement player;

    public float Horizontal { get { return JoyVec.x; } set { } }
    public float Vertical { get { return JoyVec.y; } }
    void Start()
    {
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        StickFirstPos = Stick.transform.position;

        // ĵ���� ũ�⿡���� ������ ����.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;

        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        //if (MoveFlag)
            //Player.transform.Translate(Vector3.forward * Time.deltaTime * 5f);
    }

    // �巡��
    public void Drag(BaseEventData _Data)
    {
        PointerEventData Data = _Data as PointerEventData;
        Vector3 Pos = Data.position;

        // ���̽�ƽ�� �̵���ų ������ ����.(������,����,��,�Ʒ�)
        JoyVec = (Pos - StickFirstPos).normalized;

        // ���̽�ƽ�� ó�� ��ġ�� ���� ���� ��ġ�ϰ��ִ� ��ġ�� �Ÿ��� ���Ѵ�.
        float Dis = Vector3.Distance(Pos, StickFirstPos);

        // �Ÿ��� ���������� ������ ���̽�ƽ�� ���� ��ġ�ϰ� �ִ°����� �̵�. 
        if (Dis < Radius)
        {
            Stick.position = StickFirstPos + JoyVec * Dis;
        }

        // �Ÿ��� ���������� Ŀ���� ���̽�ƽ�� �������� ũ�⸸ŭ�� �̵�.
        else
            Stick.position = StickFirstPos + JoyVec * Radius;

        
    }

    // �巡�� ��.
    public void DragEnd()
    {
        Stick.position = StickFirstPos; // ��ƽ�� ������ ��ġ��.
        JoyVec = Vector3.zero;          // ������ 0����.
    }

    // Ŭ�� ��...
    public void PointerDown()
    {
        Player.GetComponent<PlayerMovement>().AimTouch = true;      
    }

    // Ŭ�� ������...
    public void PointerUp()
    {
        Player.GetComponent<PlayerMovement>().AimTouch = false;
    }

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    OnDrag(eventData);
    //}

    //public void OnDrag(PointerEventData eventData)
    //{
    //    MoveFlag = true;
    //    //PointerEventData Data = _Data as PointerEventData;
    //    Vector3 Pos = eventData.position;

    //    // ���̽�ƽ�� �̵���ų ������ ����.(������,����,��,�Ʒ�)
    //    JoyVec = (Pos - StickFirstPos).normalized;

    //    // ���̽�ƽ�� ó�� ��ġ�� ���� ���� ��ġ�ϰ��ִ� ��ġ�� �Ÿ��� ���Ѵ�.
    //    float Dis = Vector3.Distance(Pos, StickFirstPos);

    //    // �Ÿ��� ���������� ������ ���̽�ƽ�� ���� ��ġ�ϰ� �ִ°����� �̵�. 
    //    if (Dis < Radius)
    //    {
    //        Stick.position = StickFirstPos + JoyVec * Dis;
    //    }

    //    // �Ÿ��� ���������� Ŀ���� ���̽�ƽ�� �������� ũ�⸸ŭ�� �̵�.
    //    else
    //        Stick.position = StickFirstPos + JoyVec * Radius;

    //    //Player.eulerAngles = new Vector3(0 , Mathf.Atan2(JoyVec.x, JoyVec.y) * Mathf.Rad2Deg, 0);
    //}

    //public void OnPointerUp(PointerEventData eventData)
    //{
    //    Stick.position = StickFirstPos; // ��ƽ�� ������ ��ġ��.
    //    JoyVec = Vector3.zero;          // ������ 0����.
    //    MoveFlag = false;
    //}
}
