using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.Burst.CompilerServices;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    // 플레이어 정보 출력
    //추후 수정(UI로 바꾸기)

    public Image HpAmount;

    public TextMeshProUGUI MugWortCountText;
    public TextMeshProUGUI GarlicCountText;
    public TextMeshProUGUI CoinCountText;

    public TextMeshProUGUI FireStatText;
    public TextMeshProUGUI WaterStatText;
    public TextMeshProUGUI EarthStatText;

    // 일시 정지 메뉴 출력
    //
    public GameObject PauseMenu;

    //Item Data
    public int HealthItemCount = 0;
    public int ArmorItemCount = 0;

    public Sprite HealthItemSprite;
    public Sprite ArmorItemSprite;

    public int HealAmount = 10;
    public int ArmorAmount = 5; 

    public int ItemType;
    //

    //플레이어 사망 이벤트
    public event Action PlayerDieEvent;

    //stage1 보스의 위치
    public GameObject Boss1Pos;

    //////////////////////////////
    //Player Data
    //플레이어 스탯
    [SerializeField]
    public GameObject Player;
    [SerializeField]
    private SpriteRenderer PlayerSR;
    [SerializeField]
    private Material flashMaterial;
    private Material originalMaterial;

    public float HP;
    public int AP;
    //플레이어의  기본 공격력
    public int PlayerPower;
    //플레이어의 이동 속도
    public float PlayerMoveSpeed;
    //플레이어의 공격 속도(bulletdelay)
    public float PlayerCoolTime;
    //플레이어의 공격 속도(bulletspeed)
    public float PlayerBulletSpeed;
    //플레이어의 스킬 쿨타임
    public float PlayerSkillCoolTime;
    //플레이어의 최대 체력
    public int MaxHP = 100;     
    //플레이어의 총 공격력
    public int HitDamage = 0;
    //쑥과 마늘의 코인의 인덱스
    public int MugwortCount = 0;
    public int GarlicCount = 0;
    public int CoinCount = 0;
    //플레이어 무기마다의 넉백 수치
    public float knockback = 1f;
    //////////////////////////////

    public int FireAttribute;
    public int WaterAttribute;
    public int EarthAttribute;


    //TotemSpawner의 속성 저장(Player에서 사용)
    public string totemAtribute;
    //TotemItem의 level 저장(ItemManager에서 사용)
    public int currentTotemLevel;

    //ITEMMANAGER 에서 쓰는 방어력 수치
    public int ArmorItem;

    //무기마다의 공격력을 적용하기위한 타입
    public bool checkSlot;
    public int slot1WeaponItemPower;
    public int slot2WeaponItemPower;
    //스킬이 바뀌었는지 아닌지 판변하기위한 변수(SkillManager에서 사용)
    //추가
    public bool isSkillChange;
    //

    //사망 UI
    public GameObject DeathUI;

    // 마지막 보스 스크롤 관련
    public bool PlayerInScroll = false;
    public bool DestroyScroll = false;

    [Header("마을 관련 변수")]
    #region 마을 관리
    public bool ShopPointIn;
    public bool TutorialPointIn;
    public bool DoorPointIn;

    public TextMeshProUGUI ShopNpcText;
    public TextMeshProUGUI TutoNpcText;
    public TextMeshProUGUI DoorNpcText;
    public TextMeshProUGUI BankNpcText;

    public GameObject ShopPanel;
    public GameObject TutorialPanel;
    #endregion

    #region 인스턴스 설정 필드
    public static GameManager Instance
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

        //test
        CoinCount = 50;
    }
    #endregion

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        originalMaterial = PlayerSR.material;
        HP = 100;
        AP = 1;
        checkSlot = true;
        PlayerMoveSpeed = 100f;
        PlayerCoolTime = 1f;
        PlayerBulletSpeed = 5f;
        PlayerSkillCoolTime = 2f;
        PlayerPower = 10;
        slot1WeaponItemPower = 0;
        slot2WeaponItemPower = 0;

        FireAttribute = 0;
        WaterAttribute = 0;
        EarthAttribute = 0;

        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        isSkillChange = false;

    }

    void Update()
    {
        MugWortCountText.text = MugwortCount.ToString() + "개";
        GarlicCountText.text = GarlicCount.ToString() + "개";
        CoinCountText.text = CoinCount.ToString();

        AP = ArmorItem;
        //슬롯 1번일때
        if(checkSlot == true)
        {
            HitDamage = PlayerPower + slot1WeaponItemPower;
        }
        //슬롯 2번일때
        else if(checkSlot == false)
        {
            HitDamage = PlayerPower + slot2WeaponItemPower;
        }
        //추후 수정
        PlayerState();
    }

    #region 일시정지 메뉴 출력 & 일시정지 메뉴 버튼 관리
    public void PauseClick()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeClick()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void MenuClick()
    {
        SceneManager.LoadScene("testStartScene");
    }
    #endregion

    #region 플레이어 스탯 및 UI 관리 필드
    private void PlayerState()
    {
        //PlayerHp.text = "HP : " + MaxHP.ToString() + "/"  + HP.ToString();
        if (HP <= 0)
        {
            HP = 0;
            //test 나중에 제거
            //SceneManager.LoadScene("TestEndScene");
        }
        if(HP >= MaxHP)
        {
            HP = MaxHP;
        }
        //PlayerAp.text = "AP : " + AP.ToString();
        if (AP < 0)
        {
            AP = 0;
        }
        if (HP < MaxHP)
        {
            HpAmount.fillAmount = HP / (float)MaxHP;
        }
        else
        {
            HP = MaxHP;
        }
    }
    #endregion

    #region 플레이어 피격 이벤트
    public void PlayerHit(int damage)
    {
        StartCoroutine(PlayerHitSuper());
        HP -= damage * (1 - (AP/100));
    }

    IEnumerator PlayerHitSuper()
    {
        int hitTime = 0;
        Player.gameObject.layer = 11;
        while(hitTime < 8)
        {
            if(hitTime%2 == 0)
            {
                PlayerSR.material = originalMaterial;
                PlayerSR.color = new Color32(255, 255, 255, 90);
            }
            else
            {
                PlayerSR.material = flashMaterial;
                PlayerSR.color = new Color32(255, 255, 255, 180);
            }
            yield return new WaitForSeconds(0.2f);
            hitTime++;
        }
        PlayerSR.material = originalMaterial;
        Player.gameObject.layer = 6;
        PlayerSR.color = new Color32(255, 255, 255, 255);

        yield return null;
    }
    #endregion

    #region 속성 관리 필드
    public void IncreaseAttribute(string elitName)
    {
        string orgName = elitName.Split("(")[0];
        string[] eliteRealName = orgName.Split("_");
        if (eliteRealName[0] == "F")
        {
            FireAttribute += int.Parse(eliteRealName[1]);
            // 텍스트에 표시
            FireStatText.text = FireAttribute.ToString();
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0,1);
            }
            else
            {
                ApplyAttribute(1, 0);
            }
        }
        else if(eliteRealName[0] == "W")
        {
            WaterAttribute += int.Parse(eliteRealName[1]);
            // 텍스트에 표시
            WaterStatText.text = WaterAttribute.ToString();
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0,2);
            }
            else
            {
                ApplyAttribute(2, 0);
            }
        }
        else if(eliteRealName[0] == "E")
        {
            EarthAttribute += int.Parse(eliteRealName[1]);
            // 텍스트에 표시
            EarthStatText.text = EarthAttribute.ToString();
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0,3);
            }
            else
            {
                ApplyAttribute(3,0);
            }
        }
    }

    //0.무 1.불  2.물  3.땅  
    public void ApplyAttribute(int allat, int plusat)
    { 
        if(allat == 0)
        {
            if(plusat == 1)
            {
                PlayerPower += FireAttribute * 5;
            }
            else if(plusat == 2)
            {
                PlayerMoveSpeed += WaterAttribute * 2;
                PlayerCoolTime -= 0.1f;
                PlayerBulletSpeed += 0.5f;
                PlayerSkillCoolTime -= 0.1f;
            }
            else if(plusat == 3)
            {
                MaxHP += EarthAttribute * 7;
                AP += 5;
            }
            PlayerPower += FireAttribute * 5;
            PlayerMoveSpeed += WaterAttribute * 2;
            PlayerCoolTime -= 0.1f;
            PlayerBulletSpeed += 0.5f;
            PlayerSkillCoolTime -= 0.1f;
            MaxHP += EarthAttribute * 7;
            AP += 5;
        }
        else if(allat == 1)
        {
            PlayerPower += FireAttribute * 5;
        }
        else if(allat == 2)
        {
            PlayerMoveSpeed += WaterAttribute * 4;
            PlayerCoolTime -= 0.1f;
            PlayerBulletSpeed += 0.5f;
        }
        else if(allat == 3)
        {
            MaxHP += EarthAttribute * 7;
            AP += 5;
        }
        currentTotemLevel = 0;
        ITEMMANAGER.Instance.GetSkillInfomation(FireAttribute, WaterAttribute, EarthAttribute);
    }
    #endregion

    #region 플레이어 사망 이벤트
    public void PlayerDie()
    {
        if(PlayerDieEvent != null)
        {
            PlayerDieEvent();
        }
    }
    #endregion

    #region 마을 관리 필드
    public void CloseShopPanel()
    {
        ShopPanel.SetActive(false);
        ITEMMANAGER.Instance.isOpenShopPoint = false;

        Data.JoyStickUi.gameObject.SetActive(true);
    }

    public void CloseTutoPanel()
    {
        TutorialPanel.SetActive(false);
        ITEMMANAGER.Instance.isOpenTutorialPoint = false;

        Data.JoyStickUi.gameObject.SetActive(true);

    }

    public void FireBuyButton()
    {
        if (CoinCount >= 5)
        {
            CoinCount -= 5;

            FireAttribute += 1;
            FireStatText.text = FireAttribute.ToString();
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0, 1);
            }
            else
            {
                ApplyAttribute(1, 0);
            }
        }
        else
        {
            Debug.Log("코인이 부족해요!");
        }
    }

    public void WaterBuyButton()
    {
        if (CoinCount >= 5)
        {
            CoinCount -= 5;

            WaterAttribute += 1;
            WaterStatText.text = WaterAttribute.ToString();
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0, 2);
            }
            else
            {
                ApplyAttribute(2, 0);
            }
        }
        else
        {
            Debug.Log("코인이 부족해요!");
        }
    }

    public void EarthBuyButton()
    {
        if (CoinCount >= 5)
        {
            CoinCount -= 5;

            EarthAttribute += 1;
            EarthStatText.text = EarthAttribute.ToString();
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0, 3);
            }
            else
            {
                ApplyAttribute(3, 0);
            }
        }
        else
        {
            Debug.Log("코인이 부족해요!");
        }
    }
    #endregion
}
