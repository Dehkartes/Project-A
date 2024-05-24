using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Image = UnityEngine.UI.Image;

public class PlayerMovement : MonoBehaviour
{
    //총알 
    public GameObject[] bulletPrefab;
    private int bulletIndex;

    //Attackposition 설정 변수
    public GameObject weaponAttackPosParent;
    public GameObject weaponAttackPos;

    //애니메이션 설정을 위한 변수
    public GameObject weaponAnimPos;
    private WeaponAnimScript WA;

    //공격 범위 설정을 위한 변수
    public GameObject AttakcRangePos;

    //Flip체크를 위한 변수
    public SpriteRenderer weaponAttackPosSR;
    public SpriteRenderer weaponAnimPosSR;
    private bool isFlip;

    //구르기 시 저장되는 스프라이트변수
    private Sprite PlayeroriginalWeaponSprite;

    //무기 쿨타임
    private float curTime;

    //item 관리
    private bool Enteritem = false;
    private SpriteRenderer itemRender;
    private GameObject itemObject;
    

    //현재 무기 슬롯 확인용 bool
    // true면 1번 슬롯, false면 2번 슬롯
    public bool checkWeaponSlot;

    private Rigidbody2D playerRb;
    private Animator myAnim;

    Camera mainCamera; //메인카메라
    Vector2 MousePosition; //마우스 좌표

    //마우스 좌표 치환
    private float MouseX;
    private float MouseY;

    private float weaponParentAngle;

    public bool isDodge;
    private bool isPlayerAlive;

    [SerializeField] private GameObject AimJoyStick;
    [SerializeField] private GameObject MeleeButton;

    #region 모바일 관련
    // 조이스틱
    //private FixedJoystick joystick;
    // 조이스틱 벡터 치환
    private float Horizontal;
    private float Vertical;

    // 에임 조이스틱
    //private bl_Joystick_Aim joystick_Aim;
    // 에임 조이스틱 벡터 치환
    private float AimHorizontal;
    private float AimVertical;

    // 조이스틱 터치 관련
    public bool AimTouch;
    #endregion

    private void Awake()
    {
        bulletIndex = 0;
        mainCamera = Camera.main;
        PlayeroriginalWeaponSprite = null;
        isDodge = false;
        isPlayerAlive = true;
        playerRb = GetComponent<Rigidbody2D>();
        myAnim = GetComponent<Animator>();
        WA = FindObjectOfType<WeaponAnimScript>();

        //joystick = FindObjectOfType<bl_Joystick>();
        //joystick_Aim = FindObjectOfType<bl_Joystick_Aim>();

        Data.Player = this.gameObject;
        Data.Pm = this;
    }
    private void Start()
    {
        //추후 수정
        //weaponAttackPos.transform.position = new Vector2(transform.position.x+0.06f, transform.position.y - 0.02f);
        // -> 무기마다 위치 다름 -> 추후 변수로 관리
        //근접일때만 사용

        AimJoyStick = Data.AimJoyStick;
        MeleeButton = Data.MeleeButton;

        AttakcRangePos.transform.position = new Vector2(weaponAttackPos.transform.position.x + 0.35f, weaponAttackPos.transform.position.y);

        isFlip = false;

        //처음시작할때는 slot1 기준
        checkWeaponSlot = true;

        //쿨타임 초기화
        curTime = 0;

        Time.timeScale = 1f;

        ITEMMANAGER.Instance.isOpenitemBox = false;
        ITEMMANAGER.Instance.isOpenTotemSpawner = false;
        ITEMMANAGER.Instance.isOpenShopPoint = false;
        ITEMMANAGER.Instance.isOpenTutorialPoint = false;
        ITEMMANAGER.Instance.isOpenDoorPoint = false;
    }

    private void Update()
    {
        GetInput();

        //추후 수정(이벤트로)
        if (GameManager.Instance.HP <= 0)
        {
            PlayerDie();
        }
    }
    private void FixedUpdate()
    {
        //추후에 수정(과부하 문제)
        //인스턴스 줄이기 추후에 수정
#if UNITY_EDITOR
        //Move();
        MobileMove();
#elif UNITY_ANDROID || UNITY_IOS
        MobileMove();
#endif
    }

    #region Input 관련 필드
    private void GetInput()
    {
        //아이템 습득(G키)
        if (Enteritem && Input.GetKeyDown(KeyCode.G))
        {
            ApplyItem();
        }

        //구르기(스페이스키)
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            Dodge();
        }

        //공격(마우스 좌클릭)
        if(curTime <= 0)
        {
            //if (Input.GetMouseButtonDown(0) && isDodge == false)
            if(isDodge == false)
            {
                if ((checkWeaponSlot == true && ITEMMANAGER.Instance.Weapon1Image.sprite != null) || (checkWeaponSlot == false && ITEMMANAGER.Instance.Weapon2Image.sprite != null))
                {
                    //if (myAnim.GetBool("weaponType") == true)
                    //{
                    //    PlayerMeleeAttack();
                    //}
                    if ((myAnim.GetBool("weaponType") == false) && AimTouch)
                    {
                        SoundManager.Instance.PlayAudioClip(ITEMMANAGER.Instance.SoundId);
                        PlayerDistanceAttack();
                    }
                }
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }

        //아이템 슬롯 1번(1번키)
        if (checkWeaponSlot == false && Input.GetKeyDown(KeyCode.Alpha1))
        {
            checkWeaponSlot = true;
            GameManager.Instance.checkSlot = true;
            WeaponTypeConverter(checkWeaponSlot);
        }
        //아이템 슬롯 2번(2번키)
        if (checkWeaponSlot == true && Input.GetKeyDown(KeyCode.Alpha2))
        {
            //2번무기슬롯
            checkWeaponSlot = false;
            GameManager.Instance.checkSlot = false;
            WeaponTypeConverter(checkWeaponSlot);
        }
    }
#endregion

    #region 모바일 전용 필드
        // 구르기
        public void MobileDodge()
        {
            Dodge();
        }
        // 상호작용(아이템, 토템, 상점, 튜토리얼 포인트, 문...)
        public void MobileApplyItem()
        {
            if (Enteritem)
            {
                ApplyItem();
            }
            if (ITEMMANAGER.Instance.isPlayerOnItemBox)
            {
                ITEMMANAGER.Instance.isOpenitemBox = true;
            }
            if (ITEMMANAGER.Instance.isPlayerOnTotemSpawner)
            {
                ITEMMANAGER.Instance.isOpenTotemSpawner = true;
            }
            if (ITEMMANAGER.Instance.isEnterShopPoint)
            {
                ITEMMANAGER.Instance.isOpenShopPoint = true;
            }
            if (ITEMMANAGER.Instance.isEnterTutorialPoint)
            {
                ITEMMANAGER.Instance.isOpenTutorialPoint = true;
            }
            if (ITEMMANAGER.Instance.isEnterDoorPoint)
            {
                ITEMMANAGER.Instance.isOpenDoorPoint = true;
            }
            if(GameManager.Instance.PlayerInScroll)
            {
                GameManager.Instance.DestroyScroll = true;
            }
        }
        // 모바일 근접 공격
        public void MobileMeleeAttack()
        {
            if (curTime <= 0)
            {
                if (isDodge == false)
                {
                    if ((checkWeaponSlot == true && ITEMMANAGER.Instance.Weapon1Image.sprite != null)
                        || (checkWeaponSlot == false && ITEMMANAGER.Instance.Weapon2Image.sprite != null))
                    {
                        if (myAnim.GetBool("weaponType") == true)
                        {
                            SoundManager.Instance.PlayAudioClip(ITEMMANAGER.Instance.SoundId);
                            PlayerMeleeAttack();
                        }
                    }
                }
                curTime = GameManager.Instance.PlayerCoolTime;
            }
        }
        // 무기 교체
        public void MobileChangeWeapon()
        {
            checkWeaponSlot = !checkWeaponSlot;

            if (!checkWeaponSlot)
            {
                GameManager.Instance.checkSlot = true;
                WeaponTypeConverter(checkWeaponSlot);
            }
            else if (checkWeaponSlot)
            {
                GameManager.Instance.checkSlot = false;
                WeaponTypeConverter(checkWeaponSlot);
            }
        }
        // 스킬 공격
        public void MobileSkillAttack()
        {
            if (ITEMMANAGER.Instance.SkillImage.sprite != null)
            {
                if (ITEMMANAGER.Instance.currentSkillisM != "M")
                {
                    if (PlayerSkillManager.Instance.isEnded)
                    {
                        PlayerSkillManager.Instance.SkillAttack();
                    }
                }
            }
        }
    #endregion

    #region 조이스틱 좌표값 적용 및 변환
        private void transHorizontalandVertical()
        {
            if (Data.Movejoystick.Vertical > 0.1f)
            {
                if (Data.Movejoystick.Horizontal > 0.1f)
                {
                    Horizontal = 1f;
                }
                else if (-0.1f < Data.Movejoystick.Horizontal && Data.Movejoystick.Horizontal < 0.1f)
                {
                    Horizontal = 0;
                }
                else
                {
                    Horizontal = -1f;
                }
                Vertical = 1f;
            }
            else if (Data.Movejoystick.Vertical < -0.1f)
            {
                if (Data.Movejoystick.Horizontal > 0.1f)
                {
                    Horizontal = 1f;
                }
                else if (-0.1f < Data.Movejoystick.Horizontal && Data.Movejoystick.Horizontal < 0.1f)
                {
                    Horizontal = 0;
                }
                else
                {
                    Horizontal = -1f;
                }
                Vertical = -1f;
            }
            else
            {
                if (Data.Movejoystick.Horizontal > 0.1f)
                {
                    Horizontal = 1f;
                }
                else if (-0.1f < Data.Movejoystick.Horizontal && Data.Movejoystick.Horizontal < 0.1f)
                {
                    Horizontal = 0;
                }
                else
                {
                    Horizontal = -1f;
                }
                Vertical = 0;
            }
        }
    #endregion

    #region 마우스 좌표값 적용 및 변환 필드

        //화면상 마우스 좌표 구하기
        private void GetMousePos()
        {
            MousePosition = Input.mousePosition;
            MousePosition = mainCamera.ScreenToWorldPoint(MousePosition) - mainCamera.transform.position;
            transMousePos();        
        }

        //마우스 좌표값 MouseX,MouseY 인수로 바꾸기
        // weaponAttackPos 원거리 posotion으로 바꾸기
        private void transMousePos()
        {
            if(MousePosition.y > 0.1f)
            {
                if(MousePosition.x > 0.1f)
                {
                    MouseX = 1f;                
                }
                else if(-0.1f < MousePosition.x && MousePosition.x < 0.1f)
                {
                    MouseX = 0;
                }
                else
                {
                    MouseX = -1f;
                }
                MouseY = 1f;
            }
            else if(MousePosition.y < 0.1f)
            {
                if (MousePosition.x > 0.1f)
                {
                    MouseX = 1f;
                }
                else if (-0.1f < MousePosition.x && MousePosition.x < 0.1f)
                {
                    MouseX = 0;
                }
                else
                {
                    MouseX = -1f;
                }
                MouseY = -1f;
            }
            else
            {
                if (MousePosition.x > 0.1f)
                {
                    MouseX = 1f;
                }
                else if (-0.1f < MousePosition.x && MousePosition.x < 0.1f)
                {
                    MouseX = 0;
                }
                else
                {
                    MouseX = -1f;
                }
                MouseY = 0;
            }
        }
    #endregion

    #region 이동 관련 필드
    private void Move()
    {
        playerRb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;

        myAnim.SetFloat("MoveX", playerRb.velocity.x);
        myAnim.SetFloat("MoveY", playerRb.velocity.y);

        if (myAnim.GetBool("weaponType") == true)
        {
            if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
            {
                myAnim.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
                myAnim.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
                meleeAttackPosition(myAnim.GetFloat("LastMoveX"), myAnim.GetFloat("LastMoveY"));
            }
        }
        else
        {
            GetMousePos();

            myAnim.SetFloat("MouseX", MouseX);
            myAnim.SetFloat("MouseY", MouseY);

            if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 1 || Mathf.Abs(Input.GetAxisRaw("Vertical")) == 1)
            {
                myAnim.SetBool("isMouseMove", true);
            }
            else
            {
                myAnim.SetBool("isMouseMove", false);
            }
            distanceAttackPosition();

        }
    }
    private void MobileMove()
    {
        transHorizontalandVertical();

        Vector3 transVector = new Vector3(Data.Movejoystick.Horizontal, Data.Movejoystick.Vertical, 0);

        transVector.Normalize();

        playerRb.velocity = transVector * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;
        myAnim.SetFloat("MoveX", playerRb.velocity.x);
        myAnim.SetFloat("MoveY", playerRb.velocity.y);

        if (myAnim.GetBool("weaponType") == true)
        {
            //if(transVector.x > 0.9 || transVector.x < -0.9 || transVector.y > 0.9 || transVector.y < -0.9)
            if (Data.Movejoystick.Horizontal > 0.9 || Data.Movejoystick.Horizontal < -0.9 || Data.Movejoystick.Vertical > 0.9 || Data.Movejoystick.Vertical < -0.9)
            {
                myAnim.SetFloat("LastMoveX", Horizontal);
                myAnim.SetFloat("LastMoveY", Vertical);
                meleeAttackPosition(myAnim.GetFloat("LastMoveX"), myAnim.GetFloat("LastMoveY"));
            }
        }
        else
        {
            // 마우스로 에임이동
            Vector3 transAimVector = new Vector3(Data.Movejoystick.Horizontal, Data.Movejoystick.Vertical, 0);

            myAnim.SetFloat("MouseX", transAimVector.x);
            myAnim.SetFloat("MouseY", transAimVector.y);

            //if (Mathf.Abs(joystick1.Horizontal) > 0.9 || Mathf.Abs(joystick1.Vertical) > 0.9)
            if (Mathf.Abs(transAimVector.x) > 0.9 || Mathf.Abs(transAimVector.y) > 0.9)
            {
                if(Data.Movejoystick.Vertical > 0f)
                {
                    weaponAttackPosSR.sortingOrder = 10;
                }
                else
                {
                    weaponAttackPosSR.sortingOrder = 50;
                }
                myAnim.SetBool("isMouseMove", true);
                myAnim.SetFloat("LastMoveX", Data.Movejoystick.Horizontal);
                myAnim.SetFloat("LastMoveY", Data.Movejoystick.Vertical);
            }
            else
            {
                myAnim.SetBool("isMouseMove", false);
            }
            distanceAttackPosition();
        }
    }

    //어택박스 position (근접)
    private void meleeAttackPosition(float lastMX, float lastMY)
    {
        if (lastMX >= 0)
        {
            if(isFlip == true)
            {
                Flip();
            }
            if (lastMX == 0)
            {
                if (lastMY > 0)
                {
                    weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 90);
                }
                else if (lastMY < 0)
                {
                    weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, -90);
                }
            }
            else
            {
                if (lastMY > 0)
                {
                    weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 45);
                }
                else if (lastMY == 0)
                {
                    weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                else
                {
                    weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, -45);
                }
            }
        }
        else
        {
            if (isFlip == false)
            {
                Flip();
            }
            if (lastMY > 0)
            {
                weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 135);
            }
            else if (lastMY == 0)
            {
                weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 180);
            }
            else
            {
                weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, -135);
            }
        }
    }

    //어택박스 position (원거리)
    private void distanceAttackPosition()
    {
        //float a = joystick_Aim.Vertical;
        //float b = joystick_Aim.Horizontal;

        float a = Data.joystick.Vertical;
        float b = Data.joystick.Horizontal;

        float lastMX = myAnim.GetFloat("LastMoveX");
        float lastMY = myAnim.GetFloat("LastMoveY");
        //Debug.Log("x : " + lastMX);
        //Debug.Log("y : " + lastMY);

        //if (joystick_Aim.Horizontal < -0.1f)
        if (b < -0.1f)
        {
            if (isFlip == false)
            {
                Flip();
            }
        }
        else
        {
            if (isFlip == true)
            {
                Flip();
            }
        }


        //if (lastMX > 0.1f)
        //{
        //    if (lastMX >= -0.1f || lastMX <= 0.1f)
        //    {
        //        if (lastMY > 0.1f)
        //        {
        //            weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 90);
        //        }
        //        else if (lastMY < -0.1f)
        //        {
        //            weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, -90);
        //        }
        //    }
        //    else
        //    {
        //        if (lastMY > 0.1f)
        //        {
        //            weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 45);
        //        }
        //        else if (lastMY >= -0.1f || lastMY <= 0.1f)
        //        {
        //            weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 0);
        //        }
        //        else
        //        {
        //            weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, -45);
        //        }
        //    }
        //}
        //else
        //{
        //    if (lastMY > 0.1f)
        //    {
        //        weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 135);
        //    }
        //    else if (lastMY >= -0.1f || lastMY <= 0.1f)
        //    {
        //        weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, 180);
        //    }
        //    else
        //    {
        //        weaponAttackPosParent.transform.localEulerAngles = new Vector3(0, 0, -135);
        //    }
        //}

        if (a == 0 && b == 0)
        {
            if(Mathf.Abs(weaponParentAngle) > 90f)
            {
                Flip();
            }
            return;
        }
        else
        {
            //모바일버전(Aim JoyStick)
            //weaponParentAngle = Mathf.Atan2(joystick_Aim.Vertical - 0
            // , joystick_Aim.Horizontal - 0) * Mathf.Rad2Deg;
            //weaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(weaponParentAngle, Vector3.forward);

            weaponParentAngle = Mathf.Atan2(Data.joystick.Vertical - 0
            , Data.joystick.Horizontal - 0) * Mathf.Rad2Deg;
            weaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(weaponParentAngle, Vector3.forward);
        }
    }

    private void Dodge()
    {
        if(isDodge == false && weaponAnimPosSR.sprite == null)
        {
            myAnim.SetTrigger("doDodge");
            isDodge = true;
            PlayeroriginalWeaponSprite = weaponAttackPosSR.sprite;
            weaponAttackPosSR.sprite = null;
            gameObject.layer = 11;
            Invoke("DodgeOut", 0.72f);
        }
    }

    private void DodgeOut()
    {
        weaponAttackPosSR.sprite = PlayeroriginalWeaponSprite;
        gameObject.layer = 6;
        isDodge = false;
    }

    //플립
    private void Flip()
    {
        isFlip = !isFlip;

        weaponAttackPosSR.flipY = isFlip;
        weaponAnimPosSR.flipY = isFlip;
    }
#endregion

    #region Attack 관리 필드(근접, 원거리)
    //공격 범위 박스
    public Vector2 OverlapBoxSize;
    private void PlayerMeleeAttack()
    {
        //공격 애니메이션
        WA.StartPosition(weaponAttackPosSR.sprite.name);

        SoundManager.Instance.PlayAudioClip(100);

        Collider2D[] collider2Ds
            = Physics2D.OverlapBoxAll(AttakcRangePos.transform.position, OverlapBoxSize, weaponAttackPosParent.transform.rotation.eulerAngles.z);
        foreach(Collider2D collider in collider2Ds)
        {
            if(collider.tag == "Enemy")
            {
                collider.GetComponent<Enemy>().HitfromPlayer();
            }
            if(collider.tag == "EliteMonster")
            {
                collider.GetComponent<EliteMonster>().HitfromPlayer();
            }
            if (collider.tag == "Boss1")
            {
                collider.GetComponent<Stage1BossAI>().HitfromPlayer();
            }
            if (collider.tag == "Boss2")
            {
                //GameObject boss2 = GameObject.Find("Boss_2");
                collider.GetComponentInParent<Stage2BossAI>().HitfromPlayer();                
            }
            if (collider.tag == "Boss3_Wind")
            {
                collider.GetComponent<Stage3WindBossAI>().HitfromPlayer();
            }
            if (collider.tag == "Boss3_Cloud")
            {
                collider.GetComponent<Stage3CloudBossAI>().HitfromPlayer();
            }
            //if (collider.tag == "Boss3_Rain")
            //{
            //    collider.GetComponent<Stage3RainBossAI>().HitfromPlayer();
            //}
            if (collider.tag == "Boss_Last")
            {
                collider.GetComponent<LastStageBossAI>().HitfromPlayer();
            }
        }
        
        curTime = GameManager.Instance.PlayerCoolTime;
    }

    private void SetMeleeAttackRange(bool slotType)
    {
        if (slotType)
        {
            OverlapBoxSize = ITEMMANAGER.Instance.currentSlot1Range;
        }
        else
        {
            OverlapBoxSize = ITEMMANAGER.Instance.currentSlot2Range;
        }
    }
    //콜라이더가 Scene에 보이기 위한 함수
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttakcRangePos.transform.position, OverlapBoxSize);
    }

    private void PlayerDistanceAttack()
    {
        //공격 애니메이션
        WA.StartPosition(weaponAttackPosSR.sprite.name);

        GameObject playerBullet = Instantiate(bulletPrefab[bulletIndex], weaponAttackPos.transform.position, weaponAttackPos.transform.rotation);
        Rigidbody2D rb = playerBullet.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = weaponAttackPos.transform.right * GameManager.Instance.PlayerBulletSpeed;

        curTime = GameManager.Instance.PlayerCoolTime;
    }
    #endregion

    #region 충돌 처리 필드
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnDamagedfromBody(collision);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            //item ShowWindow 처리 필드
            if (collision.gameObject.tag == "Item")
            {              
                itemRender = collision.GetComponent<SpriteRenderer>();
                itemObject = collision.gameObject;
                ITEMMANAGER.Instance.testShowStateWindow(itemObject, itemRender.sortingLayerName, checkWeaponSlot);
                Enteritem = true;
            }
            //TotemSpawnerm 처리 필드
            if (collision.gameObject.tag == "TotemSpawner")
            {
                //Fire, Water, Earth
                GameManager.Instance.totemAtribute = collision.gameObject.name;
                if(GameManager.Instance.currentTotemLevel != 0 && collision.gameObject.name == "Fire")
                {
                    ITEMMANAGER.Instance.ShowTotemPointUI(collision.gameObject, "불 토템 포인트\n'상호작용 키'");
                }
                else if (GameManager.Instance.currentTotemLevel != 0 && collision.gameObject.name == "Water")
                {
                    ITEMMANAGER.Instance.ShowTotemPointUI(collision.gameObject, "물 토템 포인트\n'상호작용 키'");
                }
                else if (GameManager.Instance.currentTotemLevel != 0 && collision.gameObject.name == "Earth")
                {
                    ITEMMANAGER.Instance.ShowTotemPointUI(collision.gameObject, "땅 토템 포인트\n'상호작용 키'");
                }
            }
            //힐아이템 처리 필드
            if (collision.gameObject.tag == "DropItem")
            {
                if (collision.gameObject.name == "Mugwort")
                {
                    GameManager.Instance.MugwortCount += 1;
                    GameManager.Instance.HP += 10;
                }
                else if (collision.gameObject.name == "Garlic")
                {
                    GameManager.Instance.GarlicCount += 1;
                    GameManager.Instance.HP += 10;
                }
                else if(collision.gameObject.name == "Coin")
                {
                    GameManager.Instance.CoinCount += 1;
                }
                Destroy(collision.gameObject);
            }
            //플레이어 피격 처리 필드
            if (isPlayerAlive == true &&  collision.gameObject.tag == "EnemyWeapon")
            {
                OnDamagedfromWeapon(collision.gameObject.name);
                Destroy(collision.gameObject);
            }
            if (isPlayerAlive == true && collision.gameObject.tag == "EliteWeapon")
            {
                OnDamagedfromEliteWeapon(collision.gameObject.name);
                Destroy(collision.gameObject);
            }
            if (isPlayerAlive == true && collision.gameObject.tag == "EliteWeaponCheck")
            {
                Destroy(collision.gameObject);
            }
            //랜덤맵
            if (collision.tag == "Door")
            {                              
                FadeInOut.Instance.setFade(true, 1.35f);

                GameObject nextRoom = collision.gameObject.transform.parent.GetComponent<Door>().nextRoom;
                Tilemap thisfloor = collision.gameObject.transform.parent.parent.parent.Find("Minimap Icon").GetChild(0).GetChild(0).transform.GetComponent<Tilemap>();

                Color preColor = thisfloor.color;
                Color.RGBToHSV(preColor, out float hh, out float ss, out float vv);   
                preColor = Color.HSVToRGB(hh, ss, vv / 2);
                thisfloor.color = preColor;
                Door nextDoor = collision.gameObject.transform.parent.GetComponent<Door>().SideDoor;

                // 진행 방향을 파악 후 캐릭터 위치 지정
                //Vector3 currPos = new Vector3(nextDoor.transform.position.x, nextDoor.transform.position.y, -0.5f);
                Vector3 currPos = new Vector3(nextDoor.transform.position.x, nextDoor.transform.position.y, -0.5f) + (nextDoor.transform.localRotation * (Vector3.up * 2f));
                transform.position = currPos;

                for (int i = 0; i < RoomController.Instance.loadedRooms.Count; i++)
                {
                    if (nextRoom.GetComponent<Room>().parent_Position == RoomController.Instance.loadedRooms[i].parent_Position)
                    {
                        RoomController.Instance.loadedRooms[i].childRooms.gameObject.SetActive(true);
                    }
                    else
                    {
                        RoomController.Instance.loadedRooms[i].childRooms.gameObject.SetActive(false);
                    }
                }

                FadeInOut.Instance.setFade(false, 0.15f);
            }
        }


        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Item")
            {
                collision.gameObject.transform.localScale = Vector3.one;
                ITEMMANAGER.Instance.CloseStateWindow();
                Enteritem = false;
            }
            if(collision.gameObject.tag == "TotemSpawner")
            {
                ITEMMANAGER.Instance.CloseStateWindow();
            }
        }
    #endregion

    #region Item 처리 필드
        private void ApplyItem()
        {
            if(itemRender.sortingLayerName == "Weapon")
            {
                if(checkWeaponSlot == true)
                {
                    ITEMMANAGER.Instance.Weapon1originalItem.Enqueue(itemObject);
                    itemObject.SetActive(false);
                    ITEMMANAGER.Instance.WeaponStat(itemObject.name, itemRender.sprite, checkWeaponSlot);
                    WeaponTypeConverter(checkWeaponSlot);
                }
                else
                {
                    ITEMMANAGER.Instance.Weapon2originalItem.Enqueue(itemObject);
                    itemObject.SetActive(false);
                    ITEMMANAGER.Instance.WeaponStat(itemObject.name, itemRender.sprite, checkWeaponSlot);
                    WeaponTypeConverter(checkWeaponSlot);
                }
            }
            else if(itemRender.sortingLayerName == "Armor")
            {
                ITEMMANAGER.Instance.ArmorOriginalItem.Enqueue(itemObject);
                itemObject.SetActive(false);
                ITEMMANAGER.Instance.ArmorStat(itemObject.name, itemRender.sprite);
            }
            else if (itemRender.sortingLayerName == "Totem")
            {
                ITEMMANAGER.Instance.TotemOriginalItem.Enqueue(itemObject);
                itemObject.SetActive(false);
                ITEMMANAGER.Instance.TotemStat(itemObject.name, itemRender.sprite);
            }

            Enteritem = false;
        }

        private void WeaponTypeConverter(bool slotType)
        {
            if(slotType == true)
            {
                weaponAttackPosSR.sprite = ITEMMANAGER.Instance.HandWeapon1Sprite;
                isPlayerHaveWeaponCheck();
                weaponAttackPosParent.transform.rotation = Quaternion.identity;
                //weaponAttackPos.transform.position = new Vector2(transform.position.x + 0.06f, transform.position.y - 0.02f);
                weaponAttackPos.transform.position = new Vector2(transform.position.x, transform.position.y);
                if (ITEMMANAGER.Instance.currentSlot1WeaponType == 1)
                {
                    SetMeleeAttackRange(slotType);
                    myAnim.SetBool("weaponType", true);

                    MeleeButton.SetActive(true);
                    AimJoyStick.SetActive(false);
                }
                else if ((ITEMMANAGER.Instance.currentSlot1WeaponType == 2))
                {
                    myAnim.SetBool("weaponType", false);
                    bulletIndex = ITEMMANAGER.Instance.currentSlot1BulletIndex;

                    MeleeButton.SetActive(false);
                    AimJoyStick.SetActive(true);
                }
            }
            else
            {
                weaponAttackPosSR.sprite = ITEMMANAGER.Instance.HandWeapon2Sprite;
                isPlayerHaveWeaponCheck();
                weaponAttackPosParent.transform.rotation = Quaternion.identity;
                //weaponAttackPos.transform.position = new Vector2(transform.position.x + 0.06f, transform.position.y - 0.02f);
                weaponAttackPos.transform.position = new Vector2(transform.position.x, transform.position.y);
                if (ITEMMANAGER.Instance.currentSlot2WeaponType == 1)
                {
                    SetMeleeAttackRange(slotType);
                    myAnim.SetBool("weaponType", true);

                    MeleeButton.SetActive(true);
                    AimJoyStick.SetActive(false);
                }
                else if ((ITEMMANAGER.Instance.currentSlot2WeaponType == 2))
                {
                    myAnim.SetBool("weaponType", false);
                    bulletIndex = ITEMMANAGER.Instance.currentSlot2BulletIndex;
                    MeleeButton.SetActive(false);
                    AimJoyStick.SetActive(true);
                }
            }

        }

        private void isPlayerHaveWeaponCheck()
        {
            if(weaponAttackPosSR.sprite == null)
            {
                myAnim.SetBool("isPlayerHaveWeapon", false);
            }
            else
            {
                myAnim.SetBool("isPlayerHaveWeapon", true);
            }
        }
    #endregion

    #region 플레이어 Attacked(피격) 필드
    //적의 '몸'에 부딪혔을때
    private void OnDamagedfromBody(Collision2D collision)
    {
        //나중에 1스테이 적은 Enemy1
        //2스테이지 적은 Enemy2 로 바꿔서 스플릿해 스테이지마다의 데미지 다르게 하기
        if (collision.gameObject.tag == "Enemy")
        {
            GameManager.Instance.PlayerHit(2);
        }
        if (collision.gameObject.tag == "EliteMonster")
        {
            GameManager.Instance.PlayerHit(5);
        }
        if (collision.gameObject.tag == "Boss1")
        {
            GameManager.Instance.PlayerHit(10);
        }
        if (collision.gameObject.tag == "Boss2")
        {
            GameManager.Instance.PlayerHit(20);
        }
        if (collision.gameObject.tag == "Boss3_Wind")
        {
            GameManager.Instance.PlayerHit(10);
        }
        if (collision.gameObject.tag == "Boss3_Cloud")
        {
            GameManager.Instance.PlayerHit(10);
        }
        //if (collision.gameObject.tag == "Boss3_Rain")
        //{
        //    GameManager.Instance.PlayerHit(10);
        //}
        if (collision.gameObject.tag == "Boss_Last")
        {
            GameManager.Instance.PlayerHit(15);
        }
    }

    //적의 '공격'에 부딪혔을때
    private void OnDamagedfromWeapon(string enemyName)
    {
        GameManager.Instance.PlayerHit(int.Parse(enemyName));
    }
    private void OnDamagedfromEliteWeapon(string enemyName)
    {
        GameManager.Instance.PlayerHit(int.Parse(enemyName));
    }

    private void PlayerDie()
    {
        playerRb.velocity = Vector3.zero;
        isPlayerAlive = false;
        weaponAttackPosSR.sprite = null;
        myAnim.SetTrigger("doDeath");
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        //GameManager.Instance.PlayerDie();

        GameManager.Instance.DeathUI.SetActive(true);
        GameManager.Instance.DeathUI.GetComponentInParent<Canvas>().sortingOrder = 1000;
        Time.timeScale = 0f;
    }
    #endregion
}
