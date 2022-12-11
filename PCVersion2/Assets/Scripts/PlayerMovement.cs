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
    //////////////////////////�������� 
    /////���� ���� �ʿ�
    //[SerializeField]
    //private GameObject Boss_1;
    [SerializeField]
    private GameObject Boss_2;
    ///////////////////////////////////

    //�Ѿ� 
    public GameObject[] bulletPrefab;
    private int bulletIndex;

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
    private bool Enteritem = false;
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

    #region Input ���� �ʵ�
    private void GetInput()
    {
        //������ ����(GŰ)
        if (Enteritem && Input.GetKeyDown(KeyCode.G))
        {
            ApplyItem();
        }

        //������(�����̽�Ű)
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            Dodge();
        }

        //����(���콺 ��Ŭ��)
        if(curTime <= 0)
        {
            if (Input.GetMouseButtonDown(0) && isDodge == false)
            {
                if ((checkWeaponSlot == true && ITEMMANAGER.Instance.Weapon1Image.sprite != null) || (checkWeaponSlot == false && ITEMMANAGER.Instance.Weapon2Image.sprite != null))
                {
                    if (myAnim.GetBool("weaponType") == true)
                    {
                        PlayerMeleeAttack();
                    }
                    else if ((myAnim.GetBool("weaponType") == false))
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
        weaponParentAngle = Mathf.Atan2(MousePosition.y - 0
            , MousePosition.x - 0) * Mathf.Rad2Deg;
        weaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(weaponParentAngle, Vector3.forward);
        //Debug.Log(weaponAttackPosParent.transform.localEulerAngles.z);
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
            if (collider.tag == "Boss1")
            {
                collider.GetComponent<Stage1BossAI>().HitfromPlayer();
            }
            if (collider.tag == "Boss2")
            {
                Boss_2.GetComponent<Stage2BossAI>().HitfromPlayer();                
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
            //Fire, Water, Earth
            GameManager.Instance.totemAtribute = collision.gameObject.name;
            if(GameManager.Instance.currentTotemLevel != 0 && collision.gameObject.name == "Fire")
            {
                ITEMMANAGER.Instance.ShowTotemPointUI(collision.gameObject, "Using Fire Totem Point\nPress 'G'");
            }
            else if (GameManager.Instance.currentTotemLevel != 0 && collision.gameObject.name == "Water")
            {
                ITEMMANAGER.Instance.ShowTotemPointUI(collision.gameObject, "Using Water Totem Point\nPress 'G'");
            }
            else if (GameManager.Instance.currentTotemLevel != 0 && collision.gameObject.name == "Earth")
            {
                ITEMMANAGER.Instance.ShowTotemPointUI(collision.gameObject, "Using Earth Totem Point\nPress 'G'");
            }
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
        if (isPlayerAlive == true && collision.gameObject.tag == "EliteWeapon")
        {
            OnDamagedfromEliteWeapon(collision.gameObject.name);
            Destroy(collision.gameObject);
        }
        if (isPlayerAlive == true && collision.gameObject.tag == "EliteWeaponCheck")
        {
            Destroy(collision.gameObject);
        }
        //������
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

            // ���� ������ �ľ� �� ĳ���� ��ġ ����
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
            ITEMMANAGER.Instance.CloseStateWindow();
            Enteritem = false;
        }
        if(collision.gameObject.tag == "TotemSpawner")
        {
            ITEMMANAGER.Instance.CloseStateWindow();
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
            }
            else if ((ITEMMANAGER.Instance.currentSlot1WeaponType == 2))
            {
                myAnim.SetBool("weaponType", false);
                bulletIndex = ITEMMANAGER.Instance.currentSlot1BulletIndex;
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
            }
            else if ((ITEMMANAGER.Instance.currentSlot2WeaponType == 2))
            {
                myAnim.SetBool("weaponType", false);
                bulletIndex = ITEMMANAGER.Instance.currentSlot2BulletIndex;
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
            GameManager.Instance.PlayerHit(1);
        }
        if(collision.gameObject.tag == "Boss1")
        {
            GameManager.Instance.PlayerHit(15);
        }
        if (collision.gameObject.tag == "Boss2")
        {
            GameManager.Instance.PlayerHit(2);
        }
        //F_3,W_3,E_3 ����
        if (collision.gameObject.tag == "EliteMonster")
        {
            GameManager.Instance.PlayerHit(1);
        }
    }

    //���� '����'�� �ε�������
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
        GameManager.Instance.PlayerDie();
    }
    #endregion
}
