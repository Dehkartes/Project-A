using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Random = UnityEngine.Random;

public class ITEMMANAGER : MonoBehaviour
{
    private static ITEMMANAGER instance = null;


    public List<Dictionary<string, object>> data;    

    // 아이템 상태창
    [SerializeField] GameObject StateWindow;
    [SerializeField] GameObject WeaponCompareWindow;
    [SerializeField] GameObject CompareUI;

    // 오브젝트 상호작용  UI

    public TextMeshProUGUI ItemBoxText;
    public TextMeshProUGUI TotemPointText;

    // 인벤토리 정보
    [SerializeField] GameObject ItemWindow;

    public TextMeshProUGUI Weapon1TabNameText;
    public TextMeshProUGUI Weapon2TabNameText;
    public TextMeshProUGUI ArmorTabNameText;

    public TextMeshProUGUI Weapon1TabStatText;
    public TextMeshProUGUI Weapon2TabStatText;
    public TextMeshProUGUI ArmorTabStatText;

    // 세부 인벤토리 창
    [SerializeField] GameObject ItemWindow_Detail;

    // 아이템 이름, 정보
    [SerializeField] private TextMeshProUGUI ItemName;
    [SerializeField] private TextMeshProUGUI ItemInfo;
    [SerializeField] private TextMeshProUGUI ItemState;

    //상태비교창 아이템 이름, 정보(현재 장착 중)
    [SerializeField] TextMeshProUGUI CurrentItemName;
    [SerializeField] TextMeshProUGUI CurrentItemStat;
    [SerializeField] Image CurrentItemImage;

    // 상태비교창 아이템 이름, 정보(먹을 아이템)
    [SerializeField] TextMeshProUGUI NewItemName;
    [SerializeField] TextMeshProUGUI NewItemStat;
    [SerializeField] Image NewItemImage;


    //아이템 인벤토리 이미지(스프라이트를 쓰기위한)
    public Image ArmorItemImage;
    public Image Weapon1Image;
    public Image Weapon2Image;
    public Image TotemImage;
    public Image SkillImage;

    // 아이템 주머니 열기
    private bool b = false;

    // 세부사항 주머니 열기
    private bool bb = false;

    //원래아이템을 교체하기위한 Queue
    public Queue<GameObject> ArmorOriginalItem;
    public Queue<GameObject> Weapon1originalItem;
    public Queue<GameObject> Weapon2originalItem;
    public Queue<GameObject> TotemOriginalItem;

    //장착하고 있는 무기의 weapon Type(원거리인지 근거리인지)
    public int currentSlot1WeaponType;
    public int currentSlot2WeaponType;

    //장착하고 있는 무기의 Range
    public Vector2 currentSlot1Range;
    public Vector2 currentSlot2Range;

    //장착하고 있는 원거리 무기의 Bullet index
    public int currentSlot1BulletIndex;
    public int currentSlot2BulletIndex;

    //현재 접근하고 있는 weaponslot 넘버
    public int checkCurrentSlot;

    //장착한 아이템의 csv Index
    int currentArmorItemIndex;
    int currentWeaponItemIndex;

    //아이템 습득 -> Hand_weapon 스프라이트로 전환 및 저장하기위한 Dictionary
    //public Dictionary<string, Sprite> HandWeapon;
    public List<Sprite> HandWeapon;
    public Sprite HandWeapon1Sprite;
    public Sprite HandWeapon2Sprite;

    //드랍아이템
    public GameObject[] DropItemPrefab;

    /////////////////////////////////////////////
    //스킬
    //UI에 표시되는 스킬 Image
    public Sprite[] SkillUIPrefab;
    //UI에 표시되는 스킬 Image의 이름
    public string currentSkillName;
    public string currentSkillFirstName;
    public int currentSkillLevel;
    // 금 속성인지 아닌지 판별하기 위한 멤버(Player에서 사용)
    public string currentSkillisM;
    /// <summary>
    /// ////////////////////////////////////////
    /// </summary>
    /// 
        //아이템 박스를 열기 위한 bool 값(모바일 전용)
    public bool isOpenitemBox;
    //아이템 박스에서 사용하는 플레이어 접근 값
    public bool isPlayerOnItemBox;

    //토템스포너를 열기 위한 bool 값(모바일 전용)
    public bool isOpenTotemSpawner;
    //토템스포너를 쓰기 위한 bool 값(모바일 전용)
    public bool isPlayerOnTotemSpawner;

    //상점 포인트를 체크하기 위한 bool 값(모바일 전용)
    public bool isEnterShopPoint;
    //상점을 열기 위한 bool 값(모바일 전용)
    public bool isOpenShopPoint;

    //튜토리얼 포인트를 체크하기 위한 bool 값(모바일 전용)
    public bool isEnterTutorialPoint;
    //튜토리얼 창을 열기 위한 bool 값(모바일 전용)
    public bool isOpenTutorialPoint;

    public bool isEnterDoorPoint;
    public bool isOpenDoorPoint;

    //플레이어
    public GameObject Player;

    public int SoundId;

    #region 인스턴스 필드
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

        // 인벤토리 비활성화
        ItemWindow.SetActive(false);
    }

    #region 버튼 값 초기화
    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoad;
    }

    private void SceneLoad(Scene arg0, LoadSceneMode arg1)
    {
        isOpenitemBox = false;
        isOpenTotemSpawner = false;
        isOpenShopPoint = false;
        isOpenTutorialPoint = false;
        isOpenDoorPoint = false;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoad;
    }
    #endregion

    //////////////////////////////////////////////////////////////////////////////////////
    /////아이템
    #region 플레이어가 가까이 가면 아이템 상태창 출력
    public void testShowStateWindow(GameObject obj,string sortname,bool checkWeaponSlot)
    {
        obj.transform.localScale = Vector3.one * 1.5f;

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
                        ItemState.text = "방어력 + " + data[i]["Armor"];
                    }
                }
                StateWindow.SetActive(true);
                StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));
            }
            else
            {
                CompareUI.SetActive(true);
                CompareUI.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));

                testCurrentItemState(sortname);
                NewItemState(obj);
            }
        }
        else if (sortname == "Weapon")
        {
            //checkWeaponSlot == true 면 1번 무기슬롯임을 뜻함
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
                            ItemState.text = "공격력 + " + data[i]["Power"];
                        }
                    }
                    StateWindow.SetActive(true);
                    StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));
                }
                else
                {
                    CompareUI.SetActive(true);
                    CompareUI.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));

                    testCurrentItemState(sortname);
                    NewItemState(obj);
                }
            }
            //2번 무기 슬롯
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
                            ItemState.text = "공격력 + " + data[i]["Power"];
                        }
                    }
                    StateWindow.SetActive(true);
                    StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));
                }
                else
                {
                    CompareUI.SetActive(true);
                    CompareUI.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));

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
                    ItemState.text = data[i]["Power"] + " 단계 토템이다";
                }
            }
            StateWindow.SetActive(true);
            StateWindow.transform.position = Camera.main.WorldToScreenPoint(obj.transform.position + new Vector3(0, 0.5f, 0));
        }
    }

    // 아이템 상자 가까이 가면 UI 출력
    public void ShowItemBoxUI(GameObject obj)
    {
        ItemBoxText.gameObject.SetActive(true);
        ItemBoxText.gameObject.transform.position = Camera.main.WorldToScreenPoint(obj.transform.localPosition + new Vector3(0, 0.3f, 0));
    }

    public void ShowTotemPointUI(GameObject obj, string msg)
    {
        TotemPointText.gameObject.SetActive(true);
        TotemPointText.transform.position = Camera.main.WorldToScreenPoint(new Vector2(obj.transform.position.x, obj.transform.position.y + 0.5f));

        TotemPointText.GetComponent<TextMeshProUGUI>().text = msg;
    }

    #endregion

    #region 플레이어가 아이템에서 떨어지면 상태창 미출력
    public void CloseStateWindow()
    {
        StateWindow.SetActive(false);
        CompareUI.SetActive(false);
        WeaponCompareWindow.SetActive(false);
        ItemBoxText.gameObject.SetActive(false);
        TotemPointText.gameObject.SetActive(false);
    }
    #endregion

    #region 방어구 스탯 플레이어에게 적용
    public void ArmorStat(string itemName,Sprite itemImage)
    {        
        if(ArmorItemImage.sprite != null)
        {
            GameObject instantiate = Instantiate(ArmorOriginalItem.Dequeue());
            string[] originalName = instantiate.name.Split("(");
            instantiate.name = originalName[0];
            instantiate.transform.position = Player.transform.position;
            instantiate.SetActive(true);
            //나중에 예외처리 해주자
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
                currentArmorItemIndex = i;

                // 방어구 인벤토리 스텟 텍스트 적용
                ArmorTabNameText.text = (string)data[i]["Name"];
                ArmorTabStatText.text = "방어력 + " + (int)data[i]["Armor"];
            }
        }
        
    }
    #endregion

    #region 무기 스텟 플레이어에게 적용
    public void WeaponStat(string itemName, Sprite itemImage,bool slotType)
    {
        //1번슬롯
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

            // 1번 슬롯 무기 인벤토리 이름
            Weapon1TabNameText.text = itemName;
            //임시, 나중에 방법 찾자(딕셔너리)
            for(int j=0; j< HandWeapon.Count; j++)
            {
                if(itemImage.name == HandWeapon[j].name)
                {
                    HandWeapon1Sprite = HandWeapon[j];
                }
            }
            //여기서 무기의 타입을 가져오고 -> Player에 적용
            for (int i = 0; i < data.Count; i++)
            {
                if (itemName == (string)data[i]["Name"])
                {
                    //무기 사운드 키값 출력
                    Debug.Log("해당 무기의 SoundKey = " + (int)data[i]["SoundId"]);
                    SoundId = (int)data[i]["SoundId"];

                    GameManager.Instance.slot1WeaponItemPower = (int)data[i]["Power"];
                    // 1번 슬롯 무기 인벤토리 스텟
                    Weapon1TabStatText.text = "공격력 + " + (int)data[i]["Power"];
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
        //2번슬롯
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
            // 2번 슬롯 무기 인벤토리 이름
            Weapon2TabNameText.text = itemName;
            //임시, 나중에 방법 찾자(딕셔너리)
            for (int j = 0; j < HandWeapon.Count; j++)
            {
                if (itemImage.name == HandWeapon[j].name)
                {
                    HandWeapon2Sprite = HandWeapon[j];
                }
            }
            //여기서 무기의 타입을 가져오고 -> Player에 적용
            for (int i = 0; i < data.Count; i++)
            {
                if (itemName == (string)data[i]["Name"])
                {
                    //무기 사운드 키값 출력
                    Debug.Log("해당 무기의 SoundKey = " + (int)data[i]["SoundId"]);
                    SoundId = (int)data[i]["SoundId"];

                    GameManager.Instance.slot2WeaponItemPower = (int)data[i]["Power"];
                    // 2번 슬롯 무기 인벤토리 스텟
                    Weapon2TabStatText.text = "공격력 + " + (int)data[i]["Power"];
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

    #region 토템 스탯 플레이어에게 적용
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

    #region 현재 장착하고 있는 아이템 스텟
    void testCurrentItemState(string sortitem)
    {
        if (sortitem == "Armor")
        {
            CurrentItemName.text = (string)data[currentArmorItemIndex]["Name"];
            CurrentItemStat.text = "방어력 + " + data[currentArmorItemIndex]["Armor"];
            CurrentItemImage.sprite = ArmorItemImage.sprite;
        }
        else if(sortitem == "Weapon")
        {
            CurrentItemName.text = (string)data[currentArmorItemIndex]["Name"];
            CurrentItemStat.text = "공격력 + " + data[currentWeaponItemIndex]["Power"];
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

    #region 비교되는 아이템 스텟
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
                    NewItemStat.text = "공격력 + " + data[i]["Power"];
                }
                else
                {
                    NewItemStat.text = "방어력 + " + data[i]["Armor"];
                }                
            }
        }
    }
    #endregion

    #region 아이템 드랍(쑥, 마늘, 골드)
    // 0. 마늘  1.쑥  2.동전
    public void ItemDrop(GameObject Enemy)
    {
        //Rigidbody2D rb = playerBullet.gameObject.GetComponent<Rigidbody2D>();
        //rb.velocity = weaponAttackPos.transform.right * GameManager.Instance.PlayerBulletSpeed;

        //30% 확률
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
        DropItem.name = DropItem.name.Split("(")[0];
        Rigidbody2D rb = DropItem.gameObject.GetComponent<Rigidbody2D>();
        int x = Random.Range(-1, 2);
        int y = Random.Range(-1, 2);
        rb.AddForce(new Vector2(0.05f * x, 0.05f * y), ForceMode2D.Impulse);
    }

    //확률 생성기
    //현재 1% 까지 가능
    public bool Nexon(float percentage)
    {
        if(percentage < 1f)
        {
            percentage = 1f;
        }
        bool Sucecess = false;
        //100 -> 1% 까지 가능
        //1000 -> 0.1% 까지 가능
        //10000 -> 0.01% 까지 가능
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

    #region 인벤토리 출력 
    public void TabInput()
    {
        b = !b;
        ItemWindow.SetActive(b);
    }

    public void TabInput_Detail()
    {
        bb = !bb;
        ItemWindow_Detail.SetActive(bb);
    }
    #endregion
    //////////////////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////////////////////
    #region 스킬 관리 필드
    //오류 있음 수정 필요
    public void GetSkillInfomation(int F,int W,int E)
    {
        if (F == W && W == E)
        {
            SetSkillInfomation(F, "M");
        }
        else if (F == W || W == E || F == E)
        {
            if(F == W)
            {
                if (F > E)
                {
                    SetSkillInfomation(F, "FW");
                }
                else
                {
                    SetSkillInfomation(E, "E");
                }
            }
            else if(F == E)
            {
                if (F > W)
                {
                    SetSkillInfomation(F, "FE");
                }
                else
                {
                    SetSkillInfomation(W, "W");
                }
            }
            else if(W == E)
            {
                if (W > F)
                {
                    SetSkillInfomation(W, "WE");
                }
                else
                {
                    SetSkillInfomation(F, "F");
                }
            }
        }
        else
        {
            if (F > W)
            {
                if (F > E)
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
                if (W > E)
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

    private void SetSkillInfomation(int level,string skillType)
    {
        //1.F 2.W 3.E 4.FE 5.WE 6.FW 7.None
        currentSkillFirstName = skillType;
        currentSkillLevel = level;
        currentSkillName = skillType +"_" + level.ToString();
        for (int i = 0; i < SkillUIPrefab.Length; i++)
        {
            if (currentSkillName == SkillUIPrefab[i].name.Split("(")[0])
            {
                currentSkillisM = currentSkillName.Split("_")[0];
                SkillImage.sprite = SkillUIPrefab[i];
                break;
            }
        }
        GameManager.Instance.isSkillChange = true;
    }
    #endregion
    //////////////////////////////////////////////////////////////////////////////////////
}
