using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class ITEMMANAGER : MonoBehaviour
{
    private static ITEMMANAGER instance = null;


    //���Ŀ� �̱������� ��ȯ
    //public GameObject Slots;
    public List<Dictionary<string, object>> data;    

    // ������ ����â
    [SerializeField] GameObject StateWindow;
    [SerializeField] GameObject WeaponCompareWindow;
    [SerializeField] GameObject CompareUI;

    // ������ �̸�, ����
    [SerializeField] Text ItemName;
    [SerializeField] Text ItemInfo;
    [SerializeField] Text ItemState;

    //���º�â ������ �̸�, ����(���� ���� ��)
    [SerializeField] Text CurrentItemName;
    [SerializeField] Text CurrentItemStat;
    [SerializeField] Image CurrentItemImage;

    // ���º�â ������ �̸�, ����(���� ������)
    [SerializeField] Text NewItemName;
    [SerializeField] Text NewItemStat;
    [SerializeField] Image NewItemImage;


    //������ �κ��丮 �̹���(��������Ʈ�� ��������)
    public Image ArmorItemImage;
    public Image Weapon1Image;
    public Image Weapon2Image;
    public Image TotemImage;
    public Image SkillImage;

    //������ �κ��丮 �ؽ�Ʈ ++ �߰��Ȱ���
    public TextMeshProUGUI Weapon1TabNameText;
    public TextMeshProUGUI Weapon2TabNameText;
    public TextMeshProUGUI ArmorTabNameText;

    //������ �κ��丮���� ������ �ɷ�ġ�� �����ִ� �ؽ�Ʈ ++ �߰��Ȱ���
    public TextMeshProUGUI Weapon1TabStatText;
    public TextMeshProUGUI Weapon2TabStatText;
    public TextMeshProUGUI ArmorTabStatText;

    //������������ ��ü�ϱ����� Queue
    public Queue<GameObject> ArmorOriginalItem;
    public Queue<GameObject> Weapon1originalItem;
    public Queue<GameObject> Weapon2originalItem;
    public Queue<GameObject> TotemOriginalItem;

    //�����ϰ� �ִ� ������ weapon Type(���Ÿ����� �ٰŸ�����)
    public int currentSlot1WeaponType;
    public int currentSlot2WeaponType;

    //�����ϰ� �ִ� ������ Range
    public Vector2 currentSlot1Range;
    public Vector2 currentSlot2Range;

    //�����ϰ� �ִ� ���Ÿ� ������ Bullet index
    public int currentSlot1BulletIndex;
    public int currentSlot2BulletIndex;

    //���� �����ϰ� �ִ� weaponslot �ѹ�
    public int checkCurrentSlot;

    //������ �������� csv Index
    int currentArmorItemIndex;
    int currentWeaponItemIndex;

    //������ ���� -> Hand_weapon ��������Ʈ�� ��ȯ �� �����ϱ����� Dictionary
    //public Dictionary<string, Sprite> HandWeapon;
    public List<Sprite> HandWeapon;
    public Sprite HandWeapon1Sprite;
    public Sprite HandWeapon2Sprite;

    //���������
    public GameObject[] DropItemPrefab;

    /////////////////////////////////////////////
    //��ų
    //UI�� ǥ�õǴ� ��ų Image
    public Sprite[] SkillUIPrefab;
    //UI�� ǥ�õǴ� ��ų Image�� �̸�
    public string currentSkillName;
    // �� �Ӽ����� �ƴ��� �Ǻ��ϱ� ���� ���(Player���� ���)
    public string currentSkillisM;
    /// <summary>
    /// ////////////////////////////////////////
    /// </summary>

    //�÷��̾�
    public GameObject Player;

    //������ â
    public GameObject ItemWindow;
    public Button ItemArrowButton;
    public Sprite[] ItemWindowArrow;

    #region �ν��Ͻ� �ʵ�
    public static ITEMMANAGER Instance
    {        
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
        }
    }
    #endregion

    #region ������ â ��ư Ŭ���ϸ� ������ ����
    public void ItemWindowAnim()
    {
        Animator anim = ItemWindow.GetComponent<Animator>();
        RectTransform rect = ItemWindow.GetComponent<RectTransform>();
        Canvas cv = rect.transform.GetComponentInParent<Canvas>();

        if(rect.anchoredPosition.x < 0)
        {         
            rect.anchoredPosition = Vector3.zero;
            cv.sortingOrder = 100;
            //ItemWindowArrow.text = "<";
            ItemArrowButton.image.sprite = ItemWindowArrow[0];
        }
        else
        {
            rect.anchoredPosition = new Vector3(-770, 0, 0);
            cv.sortingOrder = 0;
            //ItemWindowArrow.text = ">";
            ItemArrowButton.image.sprite = ItemWindowArrow[1];
        }
    }
    #endregion

    void Start()
    {
        //Player = GameObject.FindGameObjectWithTag("Player");
        ArmorOriginalItem = new Queue<GameObject>();
        Weapon1originalItem = new Queue<GameObject>();
        Weapon2originalItem = new Queue<GameObject>();
        TotemOriginalItem = new Queue<GameObject>();
        data = CSVReader.Read("ItemData");

        //HandWeapon = new Dictionary<string, Sprite>();       

        currentSlot1WeaponType = 1;
        currentSlot2WeaponType = 1;
    }
    // Update is called once per frame
    void Update()
    {

    }
    //////////////////////////////////////////////////////////////////////////////////////
    /////������
    #region �÷��̾ ������ ���� ������ ����â ���
    public void testShowStateWindow(GameObject obj,string sortname,bool checkWeaponSlot)
    {
        if (sortname == "Armor")
        {            
            if (ArmorItemImage.sprite == null)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    if (obj.name == (string)data[i]["Name"])
                    {
                        ItemName.text = (string)data[i]["Name"];
                        ItemInfo.text = (string)data[i]["Tooltip"];
                        ItemState.text = "���� + " + data[i]["Armor"];
                    }
                }
                StateWindow.SetActive(true);
                StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
            }
            else
            {
                CompareUI.SetActive(true);
                CompareUI.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);

                testCurrentItemState(sortname);
                NewItemState(obj);
            }
        }
        else if (sortname == "Weapon")
        {
            //checkWeaponSlot == true �� 1�� ���⽽������ ����
            if (checkWeaponSlot == true)
            {
                checkCurrentSlot = 1;
                if (Weapon1Image.sprite == null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (obj.name == (string)data[i]["Name"])
                        {
                            ItemName.text = (string)data[i]["Name"];
                            ItemInfo.text = (string)data[i]["Tooltip"];
                            ItemState.text = "���ݷ� + " + data[i]["Power"];
                        }
                    }
                    StateWindow.SetActive(true);
                    StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
                }
                else
                {
                    CompareUI.SetActive(true);
                    CompareUI.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);

                    testCurrentItemState(sortname);
                    NewItemState(obj);
                }
            }
            //2�� ���� ����
            else if(checkWeaponSlot == false)
            {
                checkCurrentSlot = 2;
                if (Weapon2Image.sprite == null)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        if (obj.name == (string)data[i]["Name"])
                        {
                            ItemName.text = (string)data[i]["Name"];
                            ItemInfo.text = (string)data[i]["Tooltip"];
                            ItemState.text = "���ݷ� + " + data[i]["Power"];
                        }
                    }
                    StateWindow.SetActive(true);
                    StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
                }
                else
                {
                    CompareUI.SetActive(true);
                    CompareUI.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);

                    testCurrentItemState(sortname);
                    NewItemState(obj);
                }
            }
        }
        else if (sortname == "Totem")
        {
            for (int i = 0; i < data.Count; i++)
            {
                if (obj.name == (string)data[i]["Name"])
                {
                    ItemName.text = (string)data[i]["Name"];
                    ItemInfo.text = (string)data[i]["Tooltip"];
                    ItemState.text = data[i]["Power"] + " �ܰ� �����̴�";
                }
            }
            StateWindow.SetActive(true);
            StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position);
        }
    }
    #endregion

    #region �÷��̾ �����ۿ��� �������� ����â �����
    public void CloseStateWindow()
    {
        StateWindow.SetActive(false);
        CompareUI.SetActive(false);
        WeaponCompareWindow.SetActive(false);
    }
    #endregion

    #region �� ���� �÷��̾�� ����
    public void ArmorStat(string itemName,Sprite itemImage)
    {        
        if(ArmorItemImage.sprite != null)
        {
            GameObject instantiate = Instantiate(ArmorOriginalItem.Dequeue());
            string[] originalName = instantiate.name.Split("(");
            instantiate.name = originalName[0];
            instantiate.transform.position = Player.transform.position;
            instantiate.SetActive(true);
            //���߿� ����ó�� ������
            //try
            //{
            //    originalItem.Dequeue();
            //}
            //catch(Exception ex)
            //{
            //    Debug.Log(ex);
            //}
        }
        ArmorItemImage.sprite = itemImage;
        for (int i = 0; i < data.Count; i++)
        {
            if (itemName == (string)data[i]["Name"])
            {
                GameManager.Instance.ArmorItem = (int)data[i]["Armor"];
                // �� �κ��丮 ���� �ؽ�Ʈ ����
                ArmorTabNameText.text = (string)data[i]["Name"];
                ArmorTabStatText.text = "���� + " + (int)data[i]["Armor"];
                currentArmorItemIndex = i;
            }
        }
        
    }
    #endregion

    #region ���� ���� �÷��̾�� ����
    public void WeaponStat(string itemName, Sprite itemImage,bool slotType)
    {
        //1������
        if(slotType == true)
        {
            if (Weapon1Image.sprite != null)
            {
                GameObject instantiate = Instantiate(Weapon1originalItem.Dequeue());
                string[] originalName = instantiate.name.Split("(");
                instantiate.name = originalName[0];
                instantiate.transform.position = Player.transform.position;
                instantiate.SetActive(true);
            }

            Weapon1Image.sprite = itemImage;
            // 1�� ���� ���� �κ��丮 �̸�
            Weapon1TabNameText.text = itemName;

            //�ӽ�, ���߿� ��� ã��(��ųʸ�)
            for(int j=0; j< HandWeapon.Count; j++)
            {
                if(itemImage.name == HandWeapon[j].name)
                {
                    HandWeapon1Sprite = HandWeapon[j];
                }
            }
            //���⼭ ������ Ÿ���� �������� -> Player�� ����
            for (int i = 0; i < data.Count; i++)
            {
                if (itemName == (string)data[i]["Name"])
                {
                    GameManager.Instance.slot1WeaponItemPower = (int)data[i]["Power"];
                    // 1�� ���� ���� �κ��丮 ����
                    Weapon1TabStatText.text = "���ݷ� + " + (int)data[i]["Power"];
                    currentSlot1WeaponType = (int)data[i]["Type"];
                    if(currentSlot1WeaponType == 2)
                    {
                        currentSlot1BulletIndex = (int)data[i]["BulletIndex"];
                    }
                    else
                    {
                        currentSlot1Range = new Vector2((float)data[i]["Range.x"], (float)data[i]["Range.y"]);
                    }
                    currentWeaponItemIndex = i;
                }
            }
        }
        //2������
        else if(slotType == false)
        {
            if (Weapon2Image.sprite != null)
            {
                GameObject instantiate = Instantiate(Weapon2originalItem.Dequeue());
                string[] originalName = instantiate.name.Split("(");
                instantiate.name = originalName[0];
                instantiate.transform.position = Player.transform.position;
                instantiate.SetActive(true);
            }
            Weapon2Image.sprite = itemImage;
            // 2�� ���� ���� �κ��丮 �̸�
            Weapon2TabNameText.text = itemName;
            //�ӽ�, ���߿� ��� ã��(��ųʸ�)
            for (int j = 0; j < HandWeapon.Count; j++)
            {
                if (itemImage.name == HandWeapon[j].name)
                {
                    HandWeapon2Sprite = HandWeapon[j];
                }
            }
            //���⼭ ������ Ÿ���� �������� -> Player�� ����
            for (int i = 0; i < data.Count; i++)
            {
                if (itemName == (string)data[i]["Name"])
                {
                    GameManager.Instance.slot2WeaponItemPower = (int)data[i]["Power"];
                    // 2�� ���� ���� �κ��丮 ����
                    Weapon2TabStatText.text = "���ݷ� + " + (int)data[i]["Power"];
                    currentSlot2WeaponType = (int)data[i]["Type"];
                    if (currentSlot2WeaponType == 2)
                    {
                        currentSlot2BulletIndex = (int)data[i]["BulletIndex"];
                    }
                    else
                    {
                        currentSlot2Range = new Vector2((float)data[i]["Range.x"], (float)data[i]["Range.y"]);
                    }
                    currentWeaponItemIndex = i;
                }
            }
        }
    }
    #endregion

    #region ���� ���� �÷��̾�� ����
    public void TotemStat(string itemName, Sprite itemImage)
    {
        if (TotemImage.sprite != null)
        {
            GameObject instantiate = Instantiate(TotemOriginalItem.Dequeue());
            string[] originalName = instantiate.name.Split("(");
            instantiate.name = originalName[0];
            instantiate.transform.position = Player.transform.position;
            instantiate.SetActive(true);
        }
        TotemImage.sprite = itemImage;
        for (int i = 0; i < data.Count; i++)
        {
            if (itemName == (string)data[i]["Name"])
            {
                GameManager.Instance.currentTotemLevel = (int)data[i]["Power"];
            }
        }

    }
    #endregion

    #region ���� �����ϰ� �ִ� ������ ����
    void testCurrentItemState(string sortitem)
    {
        if (sortitem == "Armor")
        {
            CurrentItemName.text = (string)data[currentArmorItemIndex]["Name"];
            CurrentItemStat.text = "���� + " + data[currentArmorItemIndex]["Armor"];
            CurrentItemImage.sprite = ArmorItemImage.sprite;
        }
        else if(sortitem == "Weapon")
        {
            CurrentItemName.text = (string)data[currentArmorItemIndex]["Name"];
            CurrentItemStat.text = "���ݷ� + " + data[currentWeaponItemIndex]["Power"];
            if(checkCurrentSlot == 1)
            {
                CurrentItemImage.sprite = Weapon1Image.sprite;
            }
            else
            {
                CurrentItemImage.sprite = Weapon2Image.sprite;
            }
        }
    }
    #endregion

    #region �񱳵Ǵ� ������ ����
    void NewItemState(GameObject obj)
    {
        SpriteRenderer render = obj.GetComponent<SpriteRenderer>();
        
        for (int i = 0; i < data.Count; i++)
        {
            if (obj.name == (string)data[i]["Name"])
            {
                NewItemName.text = (string)data[i]["Name"];
                NewItemImage.sprite = render.sprite;
                if((int)data[i]["Type"] > 0)
                {
                    NewItemStat.text = "���ݷ� + " + data[i]["Power"];
                }
                else
                {
                    NewItemStat.text = "���� + " + data[i]["Armor"];
                }                
            }
        }
    }
    #endregion

    #region ������ ���(��, ����, ���)
    // 0. ����  1.��  2.����
    public void ItemDrop(GameObject Enemy)
    {
        //Rigidbody2D rb = playerBullet.gameObject.GetComponent<Rigidbody2D>();
        //rb.velocity = weaponAttackPos.transform.right * GameManager.Instance.PlayerBulletSpeed;

        //30% Ȯ��
        bool Rand = Nexon(30);
        if (Rand)
        {
            ItemDropGo(0, Enemy);
            ItemDropGo(1, Enemy);
        }
        Rand = Nexon(50);
        if (Rand)
        {
            ItemDropGo(2, Enemy);
            ItemDropGo(2, Enemy);
        }
    }

    private void ItemDropGo(int idx, GameObject Enemy)
    {
        GameObject DropItem = Instantiate(DropItemPrefab[idx], Enemy.transform.position, transform.rotation);
        Rigidbody2D rb = DropItem.gameObject.GetComponent<Rigidbody2D>();
        int x = Random.Range(-1, 2);
        int y = Random.Range(-1, 2);
        rb.AddForce(new Vector2(0.05f * x, 0.05f * y), ForceMode2D.Impulse);
    }

    //Ȯ�� ������
    //���� 1% ���� ����
    public bool Nexon(float percentage)
    {
        if(percentage < 1f)
        {
            percentage = 1f;
        }
        bool Sucecess = false;
        //100 -> 1% ���� ����
        //1000 -> 0.1% ���� ����
        //10000 -> 0.01% ���� ����
        int RandAccuracy = 100;
        float RandHitRange = (percentage/100) * RandAccuracy;
        int Rand = Random.Range(1, RandAccuracy + 1);
        if(Rand <= RandHitRange)
        {
            Sucecess = true;
        }
        return Sucecess;
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////////////////////
    #region ��ų ���� �ʵ�
    public void GetSkillInfomation(int F,int W,int E)
    {
        //SkillImage.sprite = SkillUIPrefab[?]
        if( F == W && W == E)
        {
            SetSkillInfomation(F,"M");
        }
        else
        {
            if (F == W && F > 0)
            {
                SetSkillInfomation(F, "FW");
            }
            else if(W == E && W > 0)
            {
                SetSkillInfomation(W, "WE");
            }
            else if(F == E && F > 0)
            {
                SetSkillInfomation(F, "FE");
            }
            else
            {
                if (F > W)
                {
                    if(F > E)
                    {
                        SetSkillInfomation(F, "F");
                    }
                    else
                    {
                        SetSkillInfomation(E, "E");
                    }
                }
                else
                {
                    if(W > E)
                    {
                        SetSkillInfomation(W, "W");
                    }
                    else
                    {
                        SetSkillInfomation(E, "E");
                    }
                    
                }
            }
        }
    }

    private void SetSkillInfomation(int level,string skillType)
    {
        //1.F 2.W 3.E 4.FE 5.WE 6.FW 7.None

        currentSkillName = skillType +"_" + level.ToString();
        for (int i = 0; i < SkillUIPrefab.Length; i++)
        {
            //Debug.Log(spriteName[0]);
            if (currentSkillName == SkillUIPrefab[i].name.Split("(")[0])
            {
                currentSkillisM = currentSkillName.Split("_")[0];
                SkillImage.sprite = SkillUIPrefab[i];

            }
        }

    }
    #endregion
    //////////////////////////////////////////////////////////////////////////////////////
}
