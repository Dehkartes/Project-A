using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3WindBossAI : MonoBehaviour
{
    //풍백
    //[패턴]
    //1. 바운스 볼 뿌리기
    //2. 맵 4방향 으로 이동후 플레이어 빨아들이기
    //3. 플레이어 쫓아가다 근접 공격


    ///////////////P_1//////////////////////
    public GameObject P_1_Bullet;
    ////////////////////////////////////////

    ///////////////P_2//////////////////////
    public CapsuleCollider2D P_2_LeftCol;
    public CapsuleCollider2D P_2_RightCol;
    public CapsuleCollider2D P_2_UpCol;
    public CapsuleCollider2D P_2_DownCol;
    ////////////////////////////////////////


    private Animator WindBossAnim;
    private GameObject Player;
    private PlayerMovement PlayerScript;

    //랜덤 패턴을 위한 lsit
    List<int> originalList;
    private int nextPattern = 0;

    //보스 스탯
    public float WindBossHP;
    public float WindBossPattern1Damage;
    public float WindBossPattern1Speed;
    public float WindBossPattern2Damage;
    public float WindBossPattern3Damage;

    private RoomManager Rm;

    void Start()
    {
        GameManager.Instance.PlayerDieEvent += this.PlayerOnDie;
        Player = GameObject.FindWithTag("Player");
        PlayerScript = Player.GetComponent<PlayerMovement>();
        WindBossHP = 100;
        P_2_LeftCol.enabled = false;
        P_2_RightCol.enabled = false;
        P_2_UpCol.enabled = false;
        P_2_DownCol.enabled = false;

        WindBossAnim = GetComponent<Animator>();
        originalList = new List<int>();
        StartCoroutine(Pattern1());

        Rm = FindObjectOfType<RoomManager>();
    }

    private void Update()
    {
        Rm.WindBoss_Hp.fillAmount = WindBossHP / 100;
    }
    #region 패턴 인덱스 랜덤 필드
    private void GetRandomNum(int patternNum)
    {
        originalList.Add(1);
        originalList.Add(2);
        originalList.Add(3);
        originalList.RemoveAt(patternNum - 1);
        int idx = Random.Range(0, 2);
        nextPattern = originalList[idx];
        originalList.Clear();
    }

    private void nextPatternPlay()
    {
        switch (nextPattern)
        {
            case 1:
                StartCoroutine(Pattern1());
                break;
            case 2:
                StartCoroutine(Pattern2());
                break;
            case 3:
                StartCoroutine(Pattern3());
                break;
            default:
                Debug.Log("뭔가 이상함");
                break;
        }
    }
    #endregion

    #region 패턴 1. 바운스 볼 패턴
    private List<int> p1List = new List<int>();
    private List<GameObject> bounceBalls = new List<GameObject>();
    IEnumerator Pattern1()
    {
        GetRandomNum(1);

        //현수 수정(풍백 맵 가운데로)
        transform.position = new Vector2(60.41f, -3.84f);
        p1List.Add(1);
        p1List.Add(-1);
        yield return new WaitForSeconds(3f);
        WindBossAnim.SetTrigger("P1Start");
        StartCoroutine(Pattern1_Set());
        yield return new WaitForSeconds(0.5f);
        WindBossAnim.SetTrigger("P1Start");
        StartCoroutine(Pattern1_Set());
        yield return new WaitForSeconds(4.5f);
        p1List.Clear();
        nextPatternPlay();
    }

    IEnumerator Pattern1_Set()
    {
        int x, y, a, b;
        x = Random.Range(0, 2);
        y = Random.Range(0, 2);
        a = Random.Range(1, 3);
        b = Random.Range(1, 3);

        GameObject p1_Bullet = Instantiate(P_1_Bullet, transform.position, Quaternion.identity);
        bounceBalls.Add(p1_Bullet);
        Rigidbody2D p1_rb = p1_Bullet.GetComponent<Rigidbody2D>();
        p1_rb.velocity = new Vector2(a * p1List[x], b * p1List[y]) * WindBossPattern1Speed;
        yield return null;
    }
    #endregion

    #region 패턴 2. 빨아들이기 패턴
    private bool isP2Play;
    private bool isP2Play2;
    IEnumerator Pattern2()
    {
        GetRandomNum(2);
        yield return new WaitForSeconds(3f);
        isP2Play = true;
        int rindex = Random.Range(0, 4);
        string pos = null;
        //왼쪽
        if(rindex == 0)
        {
            //현수 수정(풍백 맵 왼쪽)
            transform.position = new Vector2(56.78f, -3.84f);
            pos = "P2_Right";
            WindBossAnim.SetBool(pos, true);
        }
        //오른쪽
        else if(rindex == 1)
        {
            //현수 수정(풍백 맵 오른쪽)
            transform.position = new Vector2(63.74f, -3.84f);
            pos = "P2_Left";
            WindBossAnim.SetBool(pos, true);
        }
        //위
        else if(rindex == 2)
        {
            //현수 수정(풍백 맵 위쪽)
            transform.position = new Vector2(60.41f, -0.67f);
            pos = "P2_Down";
            WindBossAnim.SetBool(pos, true);
        }
        //아래
        else
        {
            //현수 수정(풍백 맵 아래)
            transform.position = new Vector2(60.41f, -7.75f);
            pos = "P2_Up";
            WindBossAnim.SetBool(pos, true);
        }
        StartCoroutine(P2_PullPlayer());

        yield return new WaitForSeconds(10.0f);

        isP2Play = false;
        isP2Play2 = true;
        WindBossAnim.SetBool(pos, false);
        yield return new WaitForSeconds(0.1f);
        pos = pos + "Attack";
        WindBossAnim.SetTrigger(pos);
        yield return new WaitForSeconds(0.08f);
        switch (rindex)
        {
            case 0:
                P_2_RightCol.enabled = true;
                break;
            case 1:
                P_2_LeftCol.enabled = true;
                break;
            case 2:
                P_2_DownCol.enabled = true;
                break;
            case 3:
                P_2_UpCol.enabled = true;
                break;
        }
        yield return new WaitForSeconds(1f);
        isP2Play2 = false;
        P_2_LeftCol.enabled = false;
        P_2_RightCol.enabled = false;
        P_2_UpCol.enabled = false;
        P_2_DownCol.enabled = false;
        yield return new WaitForSeconds(3.0f);

        nextPatternPlay();
    }

    // 플레이어 빨아들이기
    IEnumerator P2_PullPlayer()
    {
        while (isP2Play)
        {
            if (isP2Play == false)
            {
                break;
            }
            else
            {
                Player.transform.position = Vector2.Lerp(Player.transform.position, transform.position, Time.deltaTime * 0.4f);
            }
            yield return null;
        }
    }
    #endregion

    #region 패턴 3. 근접 공격
    private bool isP3Play;
    IEnumerator Pattern3()
    {
        GetRandomNum(3);

        WindBossAnim.SetTrigger("P3_FlyStart");
        WindBossAnim.SetBool("P3_Flying",true);
        isP3Play = true;
        StartCoroutine(Pattern3_Set());
        yield return new WaitForSeconds(15.0f);
        WindBossAnim.SetBool("P3_Flying", false);
        isP3Play = false;
        nextPatternPlay();
    }

    IEnumerator Pattern3_Set()
    {
        while (isP3Play)
        {
            if (isP3Play == false)
            {
                break;
            }
            else
            {
                transform.position = Vector2.Lerp(transform.position, Player.transform.position, Time.deltaTime * 0.8f);
                float distance = Vector3.Distance(Player.transform.position, transform.position);
                if(distance < 0.8f)
                {
                    WindBossAnim.SetTrigger("P3Start");
                    yield return new WaitForSeconds(0.2f);
                    if (!PlayerScript.isDodge)
                        GameManager.Instance.PlayerHit((int)WindBossPattern3Damage);
                    yield return new WaitForSeconds(1f);
                }
            }
            yield return null;
        }
    }
    #endregion

    #region 충돌 처리 , 보스 사망 필드
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerWeapon"))
        {
            Destroy(col.gameObject);
            HitfromPlayer();
        }
        if (col.CompareTag("PlayerSkill"))
        {
            ISkill playerSkill = col.GetComponent<ISkill>();
            HitfromPlayerSkill(playerSkill.skillDamage);
        }
        if (col.CompareTag("Player"))
        {
            if (isP2Play2)
            {
                GameManager.Instance.PlayerHit((int)WindBossPattern2Damage);
            }
        }
    }


    public void HitfromPlayer()
    {
        WindBossHP -= GameManager.Instance.HitDamage;
        PlayerSkillManager.Instance.PassiveSkill(transform.position);
        if (WindBossHP <= 0)
        {
            Boss3Die();
        }
    }
    public void HitfromPlayerSkill(int skillDamage)
    {
        WindBossHP -= skillDamage;
        //피격추가
        if (WindBossHP <= 0)
        {
            Boss3Die();
        }
    }

    private void Boss3Die()
    {
        WindBossAnim.SetTrigger("doDeath");
        for (int index = 0; index < bounceBalls.Count; index++)
        {
            Destroy(bounceBalls[index]);
        }
        StopAllCoroutines(); //-> 이 방식은 해당 스크립트내의 모든 코루틴 종료(다른 스크립트 X)
        GetComponent<CapsuleCollider2D>().enabled = false; // 충돌체 비활성화
        Destroy(gameObject, 2);
    }


    #endregion

    #region 플레이어 사망 이벤트
    private void PlayerOnDie()
    {
        StopAllCoroutines();
        GetComponent<Stage3WindBossAI>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GameManager.Instance.PlayerDieEvent -= this.PlayerOnDie;
    }
    #endregion
}
