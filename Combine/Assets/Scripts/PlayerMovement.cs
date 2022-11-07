using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;
using Image = UnityEngine.UI.Image;

public class PlayerMovement : MonoBehaviour
{
    //�Ѿ� 
    public GameObject[] bulletPrefab;
    private int bulletIndex;
    //
    

    //Attackposition ���� ����
    public GameObject weaponAttackPosParent;
    public GameObject weaponAttackPos;

    //�ִϸ��̼� ������ ���� ����
    public GameObject weaponAnimPos;
    private WeaponAnimScript WA;

    //���� ���� ������ ���� ����
    public GameObject AttakcRangePos;

    //Flipüũ�� ���� ����
    public SpriteRenderer weaponAttackPosSR;
    public SpriteRenderer weaponAnimPosSR;
    private bool isFlip;

    //������ �� ����Ǵ� ��������Ʈ����
    private Sprite PlayeroriginalWeaponSprite;

    //���� ��Ÿ��
    private float curTime;

    //item ����
    public bool Enteritem = false;
    private SpriteRenderer itemRender;
    private GameObject itemObject;


    //���� ���� ���� Ȯ�ο� bool
    // true�� 1�� ����, false�� 2�� ����
    public bool checkWeaponSlot;

    private Rigidbody2D playerRb;
    private Animator myAnim;

    Camera mainCamera; //����ī�޶�
    Vector2 MousePosition; //���콺 ��ǥ

    //���콺 ��ǥ ġȯ
    private float MouseX;
    private float MouseY;

    private float weaponParentAngle;

    private bool isDodge;
    private bool isPlayerAlive;

    #region ����� ����
    // ���̽�ƽ
    private bl_Joystick joystick;
    // ���̽�ƽ ���� ġȯ
    private float Horizontal;
    private float Vertical;

    // ���� ���̽�ƽ
    private JoyStick1 joystick1;
    // ���� ���̽�ƽ ���� ġȯ
    private float Horizontal1;
    private float Vertical1;

    // ���̽�ƽ ��ġ ����
    public bool AimTouch;

    // �÷��̾� ���� ��Ŀ� ���� UI Ȱ��ȭ
    public GameObject Aim;
    public GameObject meleeButton;

    // �÷��̾ ������ ������ ���� ��ư �� �ؽ�Ʈ ����
    //public TextMeshProUGUI meleeButtonText;
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

        joystick = FindObjectOfType<bl_Joystick>();
        joystick1 = FindObjectOfType<JoyStick1>();
    }
    private void Start()
    {
        //���� ����
        //weaponAttackPos.transform.position = new Vector2(transform.position.x+0.06f, transform.position.y - 0.02f);
        // -> ���⸶�� ��ġ �ٸ� -> ���� ������ ����
        //�����϶��� ���
        AttakcRangePos.transform.position = new Vector2(weaponAttackPos.transform.position.x + 0.35f, weaponAttackPos.transform.position.y);

        isFlip = false;

        //ó�������Ҷ��� slot1 ����
        checkWeaponSlot = true;

        //��Ÿ�� �ʱ�ȭ
        curTime = 0;
    }

    private void Update()
    {
        GetInput();

        //���� ����(�̺�Ʈ��)
        if (GameManager.Instance.HP <= 0)
        {
            PlayerDie();
        }
    }
    private void FixedUpdate()
    {
        //���Ŀ� ����(������ ����)
        //�ν��Ͻ� ���̱� ���Ŀ� ����
        Move();
    }
    #region ���̽�ƽ ��ǥ�� ���� �� ��ȯ
    private void transHorizontalandVertical()
    {
        if (joystick.Vertical > 0.1f)
        {
            if (joystick.Horizontal > 0.1f)
            {
                Horizontal = 1f;
            }
            else if (-0.1f < joystick.Horizontal && joystick.Horizontal < 0.1f)
            {
                Horizontal = 0;
            }
            else
            {
                Horizontal = -1f;
            }
            Vertical = 1f;
        }
        else if (joystick.Vertical < -0.1f)
        {
            if (joystick.Horizontal > 0.1f)
            {
                Horizontal = 1f;
            }
            else if (-0.1f < joystick.Horizontal && joystick.Horizontal < 0.1f)
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
            if (joystick.Horizontal > 0.1f)
            {
                Horizontal = 1f;
            }
            else if (-0.1f < joystick.Horizontal && joystick.Horizontal < 0.1f)
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
    private void transJoyStickPos()
    {
        if (joystick1.Vertical > 0.1f)
        {
            if (joystick1.Horizontal > 0.1f)
            {
                Horizontal1 = 1f;
            }
            else if (-0.1f < joystick1.Horizontal && joystick1.Horizontal < 0.1f)
            {
                Horizontal1 = 0;
            }
            else
            {
                Horizontal1 = -1f;
            }
            Vertical1 = 1f;
        }
        else if (joystick1.Vertical < 0.1f)
        {
            if (joystick1.Horizontal > 0.1f)
            {
                Horizontal1 = 1f;
            }
            else if (-0.1f < joystick1.Horizontal && joystick1.Horizontal < 0.1f)
            {
                Horizontal1 = 0;
            }
            else
            {
                Horizontal1 = -1f;
            }
            Vertical1 = -1f;
        }
        else
        {
            if (joystick1.Horizontal > 0.1f)
            {
                Horizontal1 = 1f;
            }
            else if (-0.1f < joystick1.Horizontal && joystick1.Horizontal < 0.1f)
            {
                Horizontal1 = 0;
            }
            else
            {
                Horizontal1 = -1f;
            }
            Vertical1 = 0;
        }
    }
    #endregion

    #region ����� ���� �ʵ�
    public void MobileDodge()
    {
        Dodge();
    }

    public void MobileApplyItem()
    {
        if (Enteritem)
        {
            ApplyItem();
        }
    }

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
                        PlayerMeleeAttack();
                    }
                }
            }
            curTime = GameManager.Instance.PlayerCoolTime;
        }
    }

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
    #endregion

    #region Input ���� �ʵ�
    private void GetInput()
    {
        //������ ����(GŰ)
        //if (Enteritem && Input.GetKeyDown(KeyCode.G))
        //{
        //    ApplyItem();
        //}

        //������(�����̽�Ű)
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            Dodge();
        }

        //����(���콺 ��Ŭ��)
        if(curTime <= 0)
        {
            if (isDodge == false && AimTouch)
            {
                if (((checkWeaponSlot == true && ITEMMANAGER.Instance.Weapon1Image.sprite != null) ||
                (checkWeaponSlot == false && ITEMMANAGER.Instance.Weapon2Image.sprite != null)))
                {
                    // ���Ÿ� Ÿ���̰�, ���� ���̽�ƽ�� Ŭ���ϸ� ����
                    if ((myAnim.GetBool("weaponType") == false) && AimTouch)
                    {
                        PlayerDistanceAttack();
                    }
                }
            }
        }
        else
        {
            curTime -= Time.deltaTime;
        }

        //������ ���� 1��(1��Ű)
        if (checkWeaponSlot == false && Input.GetKeyDown(KeyCode.Alpha1))
        {
            checkWeaponSlot = true;
            GameManager.Instance.checkSlot = true;
            WeaponTypeConverter(checkWeaponSlot);
        }
        //������ ���� 2��(2��Ű)
        if (checkWeaponSlot == true && Input.GetKeyDown(KeyCode.Alpha2))
        {
            //2�����⽽��
            checkWeaponSlot = false;
            GameManager.Instance.checkSlot = false;
            WeaponTypeConverter(checkWeaponSlot);
        }
    }
    #endregion

    #region ���콺 ��ǥ�� ���� �� ��ȯ �ʵ�

    //ȭ��� ���콺 ��ǥ ���ϱ�
    private void GetMousePos()
    {
        MousePosition = Input.mousePosition;
        MousePosition = mainCamera.ScreenToWorldPoint(MousePosition) - mainCamera.transform.position;
        transMousePos();        
    }

    //���콺 ��ǥ�� MouseX,MouseY �μ��� �ٲٱ�
    // weaponAttackPos ���Ÿ� posotion���� �ٲٱ�
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

    #region �̵� ���� �ʵ�
    private void Move()
    {
        transHorizontalandVertical();
        playerRb.velocity = new Vector2(Mathf.Round(joystick.Horizontal), Mathf.Round(joystick.Vertical)) * GameManager.Instance.PlayerMoveSpeed * Time.deltaTime;
        myAnim.SetFloat("MoveX", playerRb.velocity.x);
        myAnim.SetFloat("MoveY", playerRb.velocity.y);

        //Debug.Log(playerRb.velocity.x);
        //Debug.Log(joystick.Vertical);

        if (myAnim.GetBool("weaponType") == true)
        {
            if (joystick.Horizontal > 0.9 || joystick.Horizontal < -0.9 || joystick.Vertical > 0.9 || joystick.Vertical < -0.9)
            {
                myAnim.SetFloat("LastMoveX", Horizontal);
                myAnim.SetFloat("LastMoveY", Vertical);
                meleeAttackPosition(myAnim.GetFloat("LastMoveX"), myAnim.GetFloat("LastMoveY"));
            }
        }
        else
        {
            // ���콺�� �����̵�
            transJoyStickPos();

            myAnim.SetFloat("MouseX", joystick1.Horizontal);
            myAnim.SetFloat("MouseY", joystick1.Vertical);

            if (Mathf.Abs(joystick1.Horizontal) > 0.9 || Mathf.Abs(joystick1.Vertical) > 0.9)
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

    //���ùڽ� position (����)
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

    //���ùڽ� position (���Ÿ�)
    private void distanceAttackPosition()
    {
        if (MousePosition.x < -0.1f)
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
        weaponParentAngle = Mathf.Atan2(joystick1.Vertical - 0
                 , joystick1.Horizontal - 0) * Mathf.Rad2Deg;
        weaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(weaponParentAngle, Vector3.forward);
    }

    private void Dodge()
    {
        if(isDodge == false)
        {
            myAnim.SetTrigger("doDodge");
            isDodge = true;
            PlayeroriginalWeaponSprite = weaponAttackPosSR.sprite;
            weaponAttackPosSR.sprite = null;
            gameObject.layer = 11;
            Invoke("DodgeOut", 0.6f);
        }
    }

    private void DodgeOut()
    {
        isDodge = false;
        weaponAttackPosSR.sprite = PlayeroriginalWeaponSprite;
        gameObject.layer = 6;
    }

    //�ø�
    private void Flip()
    {
        isFlip = !isFlip;

        weaponAttackPosSR.flipY = isFlip;
        weaponAnimPosSR.flipY = isFlip;
    }
    #endregion

    #region Attack ���� �ʵ�(����, ���Ÿ�)
    //���� ���� �ڽ�
    public Vector2 OverlapBoxSize;
    private void PlayerMeleeAttack()
    {
        //���� �ִϸ��̼�
        WA.StartPosition(weaponAttackPosSR.sprite.name);


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
    //�ݶ��̴��� Scene�� ���̱� ���� �Լ�
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttakcRangePos.transform.position, OverlapBoxSize);
    }

    private void PlayerDistanceAttack()
    {
        //���� �ִϸ��̼�
        WA.StartPosition(weaponAttackPosSR.sprite.name);

        GameObject playerBullet = Instantiate(bulletPrefab[bulletIndex], weaponAttackPos.transform.position, weaponAttackPos.transform.rotation);
        Rigidbody2D rb = playerBullet.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = weaponAttackPos.transform.right * GameManager.Instance.PlayerBulletSpeed;

        curTime = GameManager.Instance.PlayerCoolTime;
    }
    #endregion

    #region �浹 ó�� �ʵ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnDamagedfromBody(collision);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //item ShowWindow ó�� �ʵ�
        if (collision.gameObject.tag == "Item")
        {              
            itemRender = collision.GetComponent<SpriteRenderer>();
            itemObject = collision.gameObject;
            ITEMMANAGER.Instance.testShowStateWindow(itemObject, itemRender.sortingLayerName, checkWeaponSlot);
            Enteritem = true;
        }
        //TotemSpawnerm ó�� �ʵ�
        if (collision.gameObject.tag == "TotemSpawner")
        {
            Debug.Log("���Ծ��");
            //Fire, Water, Earth
            GameManager.Instance.totemAtribute = collision.gameObject.name;
        }
        //�������� ó�� �ʵ�
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
        //�÷��̾� �ǰ� ó�� �ʵ�
        if (isPlayerAlive == true &&  collision.gameObject.tag == "EnemyWeapon")
        {
            OnDamagedfromWeapon(collision.gameObject.name);
            Destroy(collision.gameObject);
        }
        //������
        if (collision.tag == "Door")
        {
            FadeInOut.Instance.setFade(true, 1.35f);

            GameObject nextRoom = collision.gameObject.transform.parent.GetComponent<Door>().nextRoom;
            Door nextDoor = collision.gameObject.transform.parent.GetComponent<Door>().SideDoor;

            // ���� ������ �ľ� �� ĳ���� ��ġ ����
            //Vector3 currPos = new Vector3(nextDoor.transform.position.x, nextDoor.transform.position.y, -0.5f);
            Vector3 currPos = new Vector3(nextDoor.transform.position.x, nextDoor.transform.position.y, -0.5f) + (nextDoor.transform.localRotation * (Vector3.up * 5));
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
            ITEMMANAGER.Instance.CloseStateWindow();
            Enteritem = false;
        }
    }
    #endregion

    #region Item ó�� �ʵ�
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

                meleeButton.SetActive(true);
                Aim.SetActive(false);
            }
            else if ((ITEMMANAGER.Instance.currentSlot1WeaponType == 2))
            {
                myAnim.SetBool("weaponType", false);
                bulletIndex = ITEMMANAGER.Instance.currentSlot1BulletIndex;

                meleeButton.SetActive(false);
                Aim.SetActive(true);
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

                meleeButton.SetActive(true);
                Aim.SetActive(false);
            }
            else if ((ITEMMANAGER.Instance.currentSlot2WeaponType == 2))
            {
                myAnim.SetBool("weaponType", false);
                bulletIndex = ITEMMANAGER.Instance.currentSlot2BulletIndex;

                meleeButton.SetActive(false);
                Aim.SetActive(true);
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

    #region �÷��̾� Attacked(�ǰ�) �ʵ�
    //���� '��'�� �ε�������
    private void OnDamagedfromBody(Collision2D collision)
    {
        //���߿� 1������ ���� Enemy1
        //2�������� ���� Enemy2 �� �ٲ㼭 ���ø��� �������������� ������ �ٸ��� �ϱ�
        if (collision.gameObject.tag == "Enemy")
        {
            GameManager.Instance.HP -= 1;
        }
        if(collision.gameObject.tag == "Boss1")
        {
            GameManager.Instance.HP -= 15;
        }
    }

    //���� '����'�� �ε�������
    private void OnDamagedfromWeapon(string enemyName)
    {
        string[] splitEnemy = enemyName.Split("_");
        GameManager.Instance.HP -= int.Parse(splitEnemy[3]);
    }

    private void PlayerDie()
    {
        isPlayerAlive = false;
        weaponAttackPosSR.sprite = null;
        myAnim.SetTrigger("doDeath");
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        GameManager.Instance.PlayerDie();
    }
    #endregion
}
