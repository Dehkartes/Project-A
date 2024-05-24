using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using Random = UnityEngine.Random;

public class PlayerSkillManager : MonoBehaviour
{
    private static PlayerSkillManager instance = null;

    public GameObject[] skillPrefab;
    public Sprite[] FE1_UISprite;

    public GameObject playerWeaponAttackPosParent;
    public GameObject skillAttackPos;
    public GameObject skillAnimPos;
    public GameObject skillAnimSprite;
    private SpriteRenderer skillAnimSpriteSR;

    private GameObject Player; //플레이어
    private Animator skillAnimPosAnim;

    //스킬 쿨타임 사용 필드/////////////////////
    public Image SkillCoolImage;
    public TextMeshProUGUI SkillCoolText;

    private float time_cooltime = 0;
    private float time_current;
    private float time_start;
    public bool isEnded = true;
    ////////////////////////////////////////////

    #region 인스턴스 필드
    public static PlayerSkillManager Instance
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
        Player = GameObject.FindWithTag("Player");
        skillAnimPosAnim = skillAnimPos.GetComponent<Animator>();
        skillAnimSpriteSR = skillAnimSprite.GetComponent<SpriteRenderer>();

        SkillCoolImage.type = Image.Type.Filled;
        SkillCoolImage.fillMethod = Image.FillMethod.Radial360;
        SkillCoolImage.fillOrigin = (int)Image.Origin360.Top;
        SkillCoolImage.fillClockwise = false;
    }

    private void Update()
    {
        CoolTime_Update();
        if (GameManager.Instance.isSkillChange)
        {
            Debug.Log("스킬쿨탐초기화");
            End_CoolTime();
            GameManager.Instance.isSkillChange = false;
            skillAnimSprite.transform.localPosition = Vector2.zero;
            skillAnimSpriteSR.sprite = null;
        }
    }

    #region 스킬 사용 필드
    private float SetSkillCool()
    {
        if (ITEMMANAGER.Instance.currentSkillFirstName.Length == 1)
        {
            return 3f;
            //switch (ITEMMANAGER.Instance.currentSkillLevel)
            //{
            //    case 1:
            //        return 1f;
            //    case 2:
            //        break;
            //    case 3:
            //        break;
            //    case 4:
            //        break;
            //    case 5:
            //        break;
            //    case 6:
            //        break;
            //}
        }
        else
        {
            return 3f;
        }

    }
    public void SkillAttack()
    {
        float skillCool = SetSkillCool();
        ClassificationSkill(ITEMMANAGER.Instance.currentSkillFirstName, ITEMMANAGER.Instance.currentSkillLevel);
        time_cooltime = skillCool;
        Trigger_Skill();
    }
    #endregion

    #region 스킬 쿨타임 UI 관리 필드
    private void CoolTime_Update()
    {
        if (isEnded)
            return;
        Check_CoolTime();
    }
    private void Check_CoolTime()
    {
        time_current = Time.time - time_start;
        if (time_current < time_cooltime)
        {
            Set_FillAmount(time_cooltime - time_current);
        }
        else if (!isEnded)
        {
            End_CoolTime();
        }
    }

    private void End_CoolTime()
    {
        Set_FillAmount(0);
        isEnded = true;
        SkillCoolImage.gameObject.SetActive(false);
        SkillCoolText.gameObject.SetActive(false);
    }

    private void Trigger_Skill()
    {
        if (!isEnded)
        {
            return;
        }

        Reset_CoolTime();
    }

    private void Reset_CoolTime()
    {
        SkillCoolText.gameObject.SetActive(true);
        time_current = time_cooltime;
        time_start = Time.time;
        SkillCoolImage.gameObject.SetActive(true);
        Set_FillAmount(time_cooltime);
        isEnded = false;
    }
    private void Set_FillAmount(float _value)
    {
        SkillCoolImage.fillAmount = _value / time_cooltime;
        string txt = _value.ToString("0.0");
        SkillCoolText.text = txt;
    }
    #endregion

    #region 스킬 분류
    private void ClassificationSkill(string sFname, int sIndex)
    {
        if (sFname == "F")
        {
            switch (sIndex)
            {
                case 1:
                    skillF_1();
                    break;
                case 2:
                    skillF_2();
                    break;
                case 3:
                    skillF_3();
                    break;
                case 4:
                    skillF_4();
                    break;
                case 5:
                    StartCoroutine(skillF_5());
                    break;
                case 6:
                    StartCoroutine(skillF_6());
                    break;
            }
        }
        else if (sFname == "W")
        {
            switch (sIndex)
            {
                case 1:
                    skillW_1();
                    break;
                case 2:
                    skillW_2();
                    break;
                case 3:
                    skillW_3();
                    break;
                case 4:
                    skillW_4();
                    break;
                case 5:
                    skillW_5();
                    break;
                case 6:
                    skillW_6();
                    break;
            }
        }
        else if (sFname == "E")
        {
            switch (sIndex)
            {
                case 1:
                    skillE_1();
                    break;
                case 2:
                    StartCoroutine(skillE_2());
                    break;
                case 3:
                    skillE_3();
                    break;
                case 4:
                    skillE_4();
                    break;
                case 5:
                    skillE_5();
                    break;
                case 6:
                    StartCoroutine(skillE_6_C());
                    break;
            }
        }
        else if (sFname == "FW")
        {
            switch (sIndex)
            {
                case 1:
                    StartCoroutine(skillFW_1());
                    break;
                case 2:
                    StartCoroutine(skillFW_2());
                    break;
                case 3:
                    skillFW_3();
                    break;
            }
        }
        else if (sFname == "FE")
        {
            switch (sIndex)
            {
                case 1:
                    break;
                case 2:
                    skillFE_2();
                    break;
                case 3:
                    skillFE_3();
                    break;
            }
        }
        else
        {
            switch (sIndex)
            {
                case 1:
                    break;
                case 2:
                    skillWE_2();
                    break;
                case 3:
                    skillWE_3();
                    break;
            }
        }
    }
    #endregion

    #region Level1 Skill 필드
    #region Fire
    private void skillF_1()
    {
        Instantiate(skillPrefab[0],
            new Vector2(skillAttackPos.transform.position.x, skillAttackPos.transform.position.y)
            , skillAttackPos.transform.rotation);
    }
    #endregion

    #region Water
    private void skillW_1()
    {
        Instantiate(skillPrefab[4],
            new Vector2(skillAttackPos.transform.position.x, skillAttackPos.transform.position.y)
            , skillAttackPos.transform.rotation);
    }
    #endregion

    #region Earth
    private void skillE_1()
    {
        Instantiate(skillPrefab[8],
    new Vector2(skillAttackPos.transform.position.x, skillAttackPos.transform.position.y)
    , skillAttackPos.transform.rotation);
    }
    #endregion
    #endregion

    #region Level2 Skill 필드
    #region Fire
    private void skillF_2()
    {
        Instantiate(skillPrefab[1],
                new Vector2(skillAttackPos.transform.position.x, skillAttackPos.transform.position.y)
                 , skillAttackPos.transform.rotation);
        Instantiate(skillPrefab[1],
                new Vector2(skillAttackPos.transform.position.x - 0.1f, skillAttackPos.transform.position.y + 0.2f)
                 , skillAttackPos.transform.rotation);
        Instantiate(skillPrefab[1],
                new Vector2(skillAttackPos.transform.position.x - 0.1f, skillAttackPos.transform.position.y - 0.2f)
                 , skillAttackPos.transform.rotation);
    }
    #endregion

    #region Water
    private void skillW_2()
    {
        Instantiate(skillPrefab[5],
        new Vector2(skillAttackPos.transform.position.x, skillAttackPos.transform.position.y)
         , skillAttackPos.transform.rotation);
    }
    #endregion

    #region Earth
    IEnumerator skillE_2()
    {
        for (int i = 0; i < 6; i++)
        {
            Instantiate(skillPrefab[9],
                new Vector2(skillAttackPos.transform.position.x, skillAttackPos.transform.position.y)
                 , skillAttackPos.transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }

    }
    #endregion
    #endregion

    #region Level3 Skill 필드
    #region Fire
    private void skillF_3()
    {
        skillAnimPosAnim.SetTrigger("fStart");
        StartCoroutine(skillF_3_C());
    }
    IEnumerator skillF_3_C()
    {
        int playerOriDamage = GameManager.Instance.PlayerPower;
        skillAnimPosAnim.SetBool("isFUsing", true);
        GameManager.Instance.PlayerPower *= 2;
        yield return new WaitForSeconds(5f);
        GameManager.Instance.PlayerPower = playerOriDamage;
        skillAnimPosAnim.SetTrigger("fEnd");
        skillAnimPosAnim.SetBool("isFUsing", false);
    }
    #endregion

    #region Water
    private void skillW_3()
    {
        skillAnimPosAnim.SetTrigger("wStart");
        StartCoroutine(skillW_3_C());
    }
    IEnumerator skillW_3_C()
    {
        float playerOriSpeed = GameManager.Instance.PlayerMoveSpeed;
        float playerOriCool = GameManager.Instance.PlayerCoolTime;
        skillAnimPosAnim.SetBool("isWUsing", true);
        GameManager.Instance.PlayerMoveSpeed *= 1.5f;
        GameManager.Instance.PlayerCoolTime *= 0.5f;
        yield return new WaitForSeconds(5f);
        GameManager.Instance.PlayerMoveSpeed = playerOriSpeed;
        GameManager.Instance.PlayerCoolTime = playerOriCool;
        skillAnimPosAnim.SetTrigger("wEnd");
        skillAnimPosAnim.SetBool("isWUsing", false);
    }
    #endregion

    #region Earth
    private void skillE_3()
    {
        skillAnimPosAnim.SetTrigger("eStart");
        StartCoroutine(skillE_3_C());
    }
    IEnumerator skillE_3_C()
    {
        skillAnimPosAnim.SetBool("isEUsing", true);
        GameManager.Instance.HP += GameManager.Instance.HP / 2;
        GameManager.Instance.AP += 10;
        yield return new WaitForSeconds(5f);
        GameManager.Instance.AP -= 10;
        skillAnimPosAnim.SetTrigger("eEnd");
        skillAnimPosAnim.SetBool("isEUsing", false);
    }
    #endregion
    #endregion

    #region Level4 Skill 필드
    //추후 3단계와 조정 필요(레벨 디자인)
    #region Fire
    private void skillF_4()
    {
        skillAnimPosAnim.SetTrigger("fStart2");
        StartCoroutine(skillF_4_C());
    }
    IEnumerator skillF_4_C()
    {
        int playerOriDamage = GameManager.Instance.PlayerPower;
        skillAnimPosAnim.SetBool("isFUsing2", true);
        GameManager.Instance.PlayerPower *= 2;
        yield return new WaitForSeconds(5f);
        GameManager.Instance.PlayerPower = playerOriDamage;
        skillAnimPosAnim.SetTrigger("fEnd2");
        skillAnimPosAnim.SetBool("isFUsing2", false);
    }
    #endregion

    #region Water
    private void skillW_4()
    {
        skillAnimPosAnim.SetTrigger("wStart2");
        StartCoroutine(skillW_4_C());
    }
    IEnumerator skillW_4_C()
    {
        float playerOriSpeed = GameManager.Instance.PlayerMoveSpeed;
        float playerOriCool = GameManager.Instance.PlayerCoolTime;
        skillAnimPosAnim.SetBool("isWUsing2", true);
        GameManager.Instance.PlayerMoveSpeed *= 1.5f;
        GameManager.Instance.PlayerCoolTime *= 0.5f;
        yield return new WaitForSeconds(5f);
        GameManager.Instance.PlayerMoveSpeed = playerOriSpeed;
        GameManager.Instance.PlayerCoolTime = playerOriCool;
        skillAnimPosAnim.SetTrigger("wEnd2");
        skillAnimPosAnim.SetBool("isWUsing2", false);
    }
    #endregion

    #region Earth
    private void skillE_4()
    {
        skillAnimPosAnim.SetTrigger("eStart2");
        StartCoroutine(skillE_4_C());
    }
    IEnumerator skillE_4_C()
    {
        skillAnimPosAnim.SetBool("isEUsing2", true);
        GameManager.Instance.HP += GameManager.Instance.HP / 2;
        GameManager.Instance.AP += 20;
        yield return new WaitForSeconds(5f);
        GameManager.Instance.AP -= 20;
        skillAnimPosAnim.SetTrigger("eEnd2");
        skillAnimPosAnim.SetBool("isEUsing2", false);
    }
    #endregion
    #endregion

    #region Level5 Skill 필드
    #region Fire
    private bool isF5Skill;
    IEnumerator skillF_5()
    {
        isF5Skill = true;
        GameObject f5 = Instantiate(skillPrefab[2],
                new Vector2(Player.transform.position.x, Player.transform.position.y)
                 , Quaternion.identity);
        Animator f5Anim = f5.GetComponent<Animator>();
        f5Anim.SetTrigger("f5Start");
        StartCoroutine(skillF_5_C(f5));
        yield return new WaitForSeconds(3f);
        isF5Skill = false;
        Destroy(f5);
    }

    IEnumerator skillF_5_C(GameObject f5)
    {
        while (isF5Skill)
        {
            if (!isF5Skill)
            {
                break;
            }
            if (f5 == null)
            {
                break;
            }
            else
            {
                f5.transform.position = Player.transform.position;
            }
            yield return null;
        }
    }
    #endregion

    #region Water
    private void skillW_5()
    {
        skillAnimPosAnim.SetTrigger("w5Start");
        StartCoroutine(skillW_5_C());
    }

    private bool isw5Using;
    IEnumerator skillW_5_C()
    {
        isw5Using = false;
        skillAnimPosAnim.SetBool("isW5Using", true);
        isw5Using = true;
        yield return new WaitForSeconds(0.3f);
        StartCoroutine(skillW_5_Shoot());
        yield return new WaitForSeconds(7f);
        isw5Using = false;
        skillAnimPosAnim.SetTrigger("w5End");
        skillAnimPosAnim.SetBool("isW5Using", false);
    }

    IEnumerator skillW_5_Shoot()
    {
        while (isw5Using)
        {
            if (!isw5Using)
            {
                break;
            }
            //왼쪽
            Instantiate(skillPrefab[6],
            new Vector2(Player.transform.position.x - 0.15f, Player.transform.position.y + 0.08f)
            , playerWeaponAttackPosParent.transform.rotation);
            //오른쪽
            Instantiate(skillPrefab[6],
            new Vector2(Player.transform.position.x + 0.15f, Player.transform.position.y + 0.08f)
            , playerWeaponAttackPosParent.transform.rotation);
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region Earth
    private void skillE_5()
    {
        float eulerAngle = playerWeaponAttackPosParent.transform.localEulerAngles.z;
        Debug.Log(eulerAngle);
        if (eulerAngle <= 45f || eulerAngle >= 315f)
        {
            //오른쪽
            GameObject e5 = Instantiate(skillPrefab[11],
                new Vector2(Player.transform.position.x + 0.4f, Player.transform.position.y)
                 , Quaternion.identity);
            Animator e5_anim = e5.GetComponent<Animator>();
            StartCoroutine(skillE_5_C(e5, e5_anim));

        }
        else if (eulerAngle >= 45f && eulerAngle <= 135f)
        {
            //위
            GameObject e5 = Instantiate(skillPrefab[10],
                new Vector2(Player.transform.position.x, Player.transform.position.y + 0.4f)
                 , Quaternion.identity);
            Animator e5_anim = e5.GetComponent<Animator>();
            StartCoroutine(skillE_5_C(e5, e5_anim));
        }
        else if (eulerAngle >= 135f && eulerAngle <= 225f)
        {
            //왼쪽
            GameObject e5 = Instantiate(skillPrefab[11],
                new Vector2(Player.transform.position.x - 0.4f, Player.transform.position.y)
                 , Quaternion.identity);
            Animator e5_anim = e5.GetComponent<Animator>();
            StartCoroutine(skillE_5_C(e5, e5_anim));
        }
        else if (eulerAngle >= 225f && eulerAngle <= 315f)
        {
            //아래
            GameObject e5 = Instantiate(skillPrefab[10],
                new Vector2(Player.transform.position.x, Player.transform.position.y - 0.4f)
                 , Quaternion.identity);
            Animator e5_anim = e5.GetComponent<Animator>();
            StartCoroutine(skillE_5_C(e5, e5_anim));
        }
    }
    IEnumerator skillE_5_C(GameObject e5, Animator e5_anim)
    {
        yield return new WaitForSeconds(4f);
        e5_anim.SetTrigger("isDestroy");
        yield return new WaitForSeconds(0.33f);
        Destroy(e5);
    }
    #endregion
    #endregion

    #region Level6 Skill 필드
    #region Fire
    private IEnumerator skillF_6()
    {
        Vector3 OriginLocalPos = skillAttackPos.transform.localPosition;
        skillAttackPos.transform.localPosition = new Vector2(skillAttackPos.transform.localPosition.x + 0.4f,
                skillAttackPos.transform.localPosition.y);
        Vector2 f6_pos1 = skillAttackPos.transform.position;
        skillAttackPos.transform.localPosition = new Vector2(skillAttackPos.transform.localPosition.x + 0.8f,
                skillAttackPos.transform.localPosition.y);
        Vector2 f6_pos2 = skillAttackPos.transform.position;
        skillAttackPos.transform.localPosition = new Vector2(skillAttackPos.transform.localPosition.x + 0.8f,
                skillAttackPos.transform.localPosition.y);
        Vector2 f6_pos3 = skillAttackPos.transform.position;
        skillAttackPos.transform.localPosition = new Vector2(skillAttackPos.transform.localPosition.x + 1.2f,
        skillAttackPos.transform.localPosition.y);
        Vector2 f6_pos4 = skillAttackPos.transform.position;
        skillAttackPos.transform.localPosition = OriginLocalPos;


        GameObject f6_1 = Instantiate(skillPrefab[3], f6_pos1, Quaternion.identity);
        F6_Destroy(f6_1, 0.82f);
        yield return new WaitForSeconds(0.5f);
        GameObject f6_2 = Instantiate(skillPrefab[3], f6_pos2, Quaternion.identity);
        F6_Destroy(f6_2, 0.82f);
        yield return new WaitForSeconds(0.5f);
        GameObject f6_3 = Instantiate(skillPrefab[3], f6_pos3, Quaternion.identity);
        F6_Destroy(f6_3, 0.82f);
        StartCoroutine(skillF_6_C(f6_pos4));


        yield return new WaitForSeconds(1f);
    }

    private IEnumerator skillF_6_C(Vector2 pos)
    {
        for (int index = 0; index < 8; index++)
        {
            int rr = Random.Range(0, 9);
            switch (rr)
            {
                case 0:
                    GameObject f6 = Instantiate(skillPrefab[3], new Vector2(pos.x, pos.y), Quaternion.identity);
                    F6_Destroy(f6, 0.82f);
                    break;
                case 1:
                    GameObject f6_1 = Instantiate(skillPrefab[3], new Vector2(pos.x + 0.4f, pos.y + 0.4f), Quaternion.identity);
                    F6_Destroy(f6_1, 0.82f);
                    break;
                case 2:
                    GameObject f6_2 = Instantiate(skillPrefab[3], new Vector2(pos.x - 0.4f, pos.y + 0.4f), Quaternion.identity);
                    F6_Destroy(f6_2, 0.82f);
                    break;
                case 3:
                    GameObject f6_3 = Instantiate(skillPrefab[3], new Vector2(pos.x + 0.4f, pos.y - 0.4f), Quaternion.identity);
                    F6_Destroy(f6_3, 0.82f);
                    break;
                case 4:
                    GameObject f6_4 = Instantiate(skillPrefab[3], new Vector2(pos.x - 0.4f, pos.y - 0.4f), Quaternion.identity);
                    F6_Destroy(f6_4, 0.82f);
                    break;
                case 5:
                    GameObject f6_5 = Instantiate(skillPrefab[3], new Vector2(pos.x + 0.4f, pos.y), Quaternion.identity);
                    F6_Destroy(f6_5, 0.82f);
                    break;
                case 6:
                    GameObject f6_6 = Instantiate(skillPrefab[3], new Vector2(pos.x, pos.y + 0.4f), Quaternion.identity);
                    F6_Destroy(f6_6, 0.82f);
                    break;
                case 7:
                    GameObject f6_7 = Instantiate(skillPrefab[3], new Vector2(pos.x - 0.4f, pos.y), Quaternion.identity);
                    F6_Destroy(f6_7, 0.82f);
                    break;
                case 8:
                    GameObject f6_8 = Instantiate(skillPrefab[3], new Vector2(pos.x, pos.y - 0.4f), Quaternion.identity);
                    F6_Destroy(f6_8, 0.82f);
                    break;
                default:
                    break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void F6_Destroy(GameObject f6, float time)
    {
        Destroy(f6, time);
    }
    #endregion

    #region Water
    private void skillW_6()
    {
        Vector3 OriginLocalPos = skillAttackPos.transform.localPosition;
        skillAttackPos.transform.localPosition = new Vector2(skillAttackPos.transform.localPosition.x + 1.4f,
            skillAttackPos.transform.localPosition.y);
        GameObject w6 = Instantiate(skillPrefab[7], skillAttackPos.transform.position, Quaternion.identity);
        Animator w6_anim = w6.GetComponent<Animator>();
        w6_anim.SetTrigger("wStart");
        StartCoroutine(skillW_6_C(w6_anim, w6));
        skillAttackPos.transform.localPosition = OriginLocalPos;
    }

    IEnumerator skillW_6_C(Animator w6_anim, GameObject w6)
    {
        yield return new WaitForSeconds(0.333f);
        w6_anim.SetBool("iswUsing", true);

        yield return new WaitForSeconds(5f);

        w6_anim.SetTrigger("wEnd");
        w6_anim.SetBool("iswUsing", false);
        yield return new WaitForSeconds(0.333f);

        Destroy(w6_anim);
        Destroy(w6);
    }
    #endregion

    #region Earth
    private bool isE6Skill;
    IEnumerator skillE_6_C()
    {
        Player.gameObject.layer = 11;
        GameObject e6 = Instantiate(skillPrefab[12],
                new Vector2(Player.transform.position.x, Player.transform.position.y)
                 , Quaternion.identity);
        isE6Skill = true;
        StartCoroutine(skillE_6_C_Check(e6));
        yield return new WaitForSeconds(6f);
        isE6Skill = false;
        Destroy(e6);
        Player.gameObject.layer = 6;
    }

    IEnumerator skillE_6_C_Check(GameObject e6)
    {
        while (isE6Skill)
        {
            if (!isE6Skill)
            {
                break;
            }
            e6.transform.position = Player.transform.position;
            yield return null;
        }
    }
    #endregion
    #endregion

    #region Merge1 Skill 필드
    #region FW_1
    private bool isFW1Skill;
    IEnumerator skillFW_1()
    {
        GameObject fw = Instantiate(skillPrefab[13],
                new Vector2(Player.transform.position.x, Player.transform.position.y)
                 , Quaternion.identity);
        isFW1Skill = true;
        StartCoroutine(skillFW_C(fw));
        yield return new WaitForSeconds(3.5f);
        isFW1Skill = false;
        Destroy(fw);
    }

    IEnumerator skillFW_C(GameObject fw)
    {
        while (isFW1Skill)
        {
            if (!isFW1Skill)
            {
                break;
            }
            fw.transform.position = Player.transform.position;
            yield return null;
        }
    }
    #endregion

    #region FE_1
    private int FE1Count = 1;
    private void skillFE_1(Vector2 pos)
    {
        Instantiate(skillPrefab[16], pos, Quaternion.identity);
    }

    private void FE_1_UI()
    {
        skillAnimSprite.transform.localPosition = Vector2.zero;
        skillAnimSprite.transform.localPosition = new Vector2(skillAnimPos.transform.localPosition.x,
            skillAnimPos.transform.localPosition.y + 0.17f);
        skillAnimSpriteSR.sprite = FE1_UISprite[FE1Count - 1];
    }
    #endregion

    #region WE_1
    private void skillWE_1()
    {
        //0.333
        GameObject we = Instantiate(skillPrefab[19], Player.transform.position, Quaternion.identity);
        SpriteRenderer weSR = we.GetComponent<SpriteRenderer>();
        weSR.sortingOrder = 30;
        GameManager.Instance.HP += 10;
        we.transform.SetParent(Player.transform);
        Destroy(we, 0.333f);
    }

    #endregion
    #endregion  

    #region Merge2 Skill 필드
    #region FW_2
    private bool isFW2Done;
    IEnumerator skillFW_2()
    {
        Vector3 OriginLocalPos = skillAttackPos.transform.localPosition;
        skillAttackPos.transform.localPosition = new Vector2(skillAttackPos.transform.localPosition.x + 2.102f,
                skillAttackPos.transform.localPosition.y);
        isFW2Done = true;
        Player.gameObject.layer = 11;
        GameObject fw = Instantiate(skillPrefab[14], Player.transform.position, skillAttackPos.transform.rotation);
        StartCoroutine(skillFW_2_C(skillAttackPos.transform.position));
        skillAttackPos.transform.localPosition = OriginLocalPos;
        yield return new WaitForSeconds(0.4f);
        Player.gameObject.layer = 6;
        isFW2Done = false;
        yield return new WaitForSeconds(0.05f);
        Destroy(fw);
    }

    IEnumerator skillFW_2_C(Vector2 pos)
    {
        Vector2 targetPos;
        Vector2 direction = skillAttackPos.transform.position - Player.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(Player.transform.position, direction, 2.402f, LayerMask.GetMask("Wall"));
        if (hit)
        {
            targetPos = hit.point;
        }
        else
        {
            targetPos = pos;
        }
        while (isFW2Done)
        {
            if (isFW2Done == false)
            {
                break;
            }
            else
            {
                Player.transform.position = Vector2.Lerp(Player.transform.position, targetPos, 0.2f);
            }
            yield return null;
        }
    }
    #endregion

    #region FE_2
    private void skillFE_2()
    {
        Instantiate(skillPrefab[17], skillAttackPos.transform.position, skillAttackPos.transform.rotation);
    }

    public void Seperate_FE2(Vector2 pos, float speed)
    {
        GameObject fe1 = Instantiate(skillPrefab[17], pos, Quaternion.identity);
        //Rigidbody2D fe1_rb = fe1.GetComponent<Rigidbody2D>();
        //GameObject fe2 = Instantiate(skillPrefab[17], pos, Quaternion.identity);
        //Rigidbody2D fe2_rb = fe1.GetComponent<Rigidbody2D>();
        //GameObject fe3 = Instantiate(skillPrefab[17], pos, Quaternion.identity);
        //Rigidbody2D fe3_rb = fe1.GetComponent<Rigidbody2D>();
        //GameObject fe4 = Instantiate(skillPrefab[17], pos, Quaternion.identity);
        //Rigidbody2D fe4_rb = fe1.GetComponent<Rigidbody2D>();

        //fe1_rb.velocity = Vector2.zero;
        //fe2_rb.velocity = Vector2.zero;
        //fe3_rb.velocity = Vector2.zero;
        //fe4_rb.velocity = Vector2.zero;

        //fe1_rb.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        //fe2_rb.AddForce(Vector2.left * speed, ForceMode2D.Impulse);
        //fe3_rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        //fe4_rb.AddForce(Vector2.down * speed, ForceMode2D.Impulse);
        //yield return new WaitForSeconds(0.4f);
    }
    #endregion

    #region WE_2
    private void skillWE_2()
    {
        Instantiate(skillPrefab[20], Player.transform.position, Quaternion.identity);
    }
    #endregion
    #endregion

    #region Merge3 Skill 필드
    #region FW_3
    private void skillFW_3()
    {
        Instantiate(skillPrefab[15], skillAttackPos.transform.position, skillAttackPos.transform.rotation);
    }
    #endregion

    #region FE_3
    private void skillFE_3()
    {
        Instantiate(skillPrefab[18], Player.transform.position, Quaternion.identity);
    }
    #endregion

    #region WE_3
    private void skillWE_3()
    {
        Instantiate(skillPrefab[21], new Vector2(Player.transform.position.x, Player.transform.position.y + 0.6f), Quaternion.identity);
    }
    #endregion
    #endregion

    #region 패시브 스킬 전용( WE_1 , FE_1 ) 
    public void PassiveSkill(Vector2 pos)
    {
        if (ITEMMANAGER.Instance.currentSkillFirstName == "WE" && ITEMMANAGER.Instance.currentSkillLevel == 1)
        {
            skillWE_1();
        }
        else if (ITEMMANAGER.Instance.currentSkillFirstName == "FE" && ITEMMANAGER.Instance.currentSkillLevel == 1)
        {
            FE1Count++;
            if (FE1Count % 4 == 0)
            {
                FE1Count = 1;
                skillFE_1(pos);
            }
            FE_1_UI();
        }
    }
    #endregion
}
