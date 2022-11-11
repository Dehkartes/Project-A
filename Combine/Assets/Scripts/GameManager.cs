using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;


    // �÷��̾� ���� ���
    //���� ����(UI�� �ٲٱ�)

    public Image HpImage;
    //

    //Item Data
    public int HealthItemCount = 0;
    public int ArmorItemCount = 0;

    public Sprite HealthItemSprite;
    public Sprite ArmorItemSprite;

    public int HealAmount = 10;
    public int ArmorAmount = 5; 

    public int ItemType;
    //

    //�÷��̾� ��� �̺�Ʈ
    public event Action PlayerDieEvent;

    //stage1 ������ ��ġ
    public GameObject Boss1Pos;

    //////////////////////////////
    //Player Data
    //�÷��̾� ����
    public int HP;
    public int AP = 0;
    //�÷��̾���  �⺻ ���ݷ�
    public int PlayerPower;
    //�÷��̾��� �̵� �ӵ�
    public float PlayerMoveSpeed;
    //�÷��̾��� ���� �ӵ�(bulletdelay)
    public float PlayerCoolTime;
    //�÷��̾��� ���� �ӵ�(bulletspeed)
    public float PlayerBulletSpeed;
    //�÷��̾��� �ִ� ü��
    public int MaxHP = 100;     
    //�÷��̾��� �� ���ݷ�
    public int HitDamage = 0;
    //���� ������ ������ �ε���
    public int MugwortCount = 0;
    public int GarlicCount = 0;
    public int CoinCount = 0;
    //�÷��̾� ���⸶���� �˹� ��ġ
    public float knockback = 1f;
    //////////////////////////////

    public int FireAttribute;
    public int WaterAttribute;
    public int EarthAttribute;


    //TotemSpawner�� �Ӽ� ����(Player���� ���)
    public string totemAtribute;
    //TotemItem�� level ����(ItemManager���� ���)
    public int currentTotemLevel;

    //ITEMMANAGER ���� ���� ���� ��ġ
    public int ArmorItem;

    //���⸶���� ���ݷ��� �����ϱ����� Ÿ��
    public bool checkSlot;
    public int slot1WeaponItemPower;
    public int slot2WeaponItemPower;

    //

    #region �ν��Ͻ� ���� �ʵ�
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
    }
    #endregion

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        HP = MaxHP;
        checkSlot = true;
        //�ӵ� �ٲ�
        PlayerMoveSpeed = 500f;
        PlayerCoolTime = 1f;
        PlayerBulletSpeed = 5f;
        PlayerPower = 10;
        slot1WeaponItemPower = 0;
        slot2WeaponItemPower = 0;

        FireAttribute = 0;
        WaterAttribute = 0;
        EarthAttribute = 0;
    }

    void Update()
    {
        AP = ArmorItem;
        //���� 1���϶�
        if(checkSlot == true)
        {
            HitDamage = PlayerPower + slot1WeaponItemPower;
        }
        //���� 2���϶�
        else if(checkSlot == false)
        {
            HitDamage = PlayerPower + slot2WeaponItemPower;
        }
        //���� ����
        PlayerState();
    }

    #region �÷��̾� ���� �� UI ���� �ʵ�
    private void PlayerState()
    {
        //PlayerHp.text = "HP : " + MaxHP.ToString() + "/"  + HP.ToString();
        if (HP <= 0)
        {
            HP = 0;
            //test ���߿� ����
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
        //PlayerPw.text = "Power : " + HitDamage.ToString();

        if (HP < MaxHP)
        {
            HpImage.fillAmount = (float)HP / (float)MaxHP;
        }
        else
        {
            HP = MaxHP;
        }
    }
    #endregion

    #region �Ӽ� ���� �ʵ�
    public void IncreaseAttribute()
    {
        if(totemAtribute == "Fire")
        {
            FireAttribute += currentTotemLevel;
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0,1);
            }
            else
            {
                ApplyAttribute(1, 0);
            }
        }
        else if(totemAtribute == "Water")
        {
            WaterAttribute += currentTotemLevel;
            if (FireAttribute == WaterAttribute && WaterAttribute == EarthAttribute)
            {
                ApplyAttribute(0,2);
            }
            else
            {
                ApplyAttribute(2, 0);
            }
        }
        else if(totemAtribute == "Earth")
        {
            EarthAttribute += currentTotemLevel;
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

    //0.�� 1.��  2.��  3.��  
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
            MaxHP += EarthAttribute * 7;
            AP += 5;
        }
        else if(allat == 1)
        {
            PlayerPower += FireAttribute * 5;
        }
        else if(allat == 2)
        {
            PlayerMoveSpeed += WaterAttribute * 2;
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

    #region �÷��̾� ��� �̺�Ʈ
    public void PlayerDie()
    {
        if(PlayerDieEvent != null)
        {
            PlayerDieEvent();
        }
    }
    #endregion
}
