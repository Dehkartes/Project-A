using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage3CloudBossAI : MonoBehaviour
{
    //풍백
    //[패턴]
    //1. 전깃줄 생성
    //2. 구름생성후 번개 탄막
    //3. 플레이어에게 다가가다 근접공격


    ///////////////P_1//////////////////////
    public GameObject P_1_ChainLightning_1;
    public GameObject P_1_ChainLightning_2;
    ////////////////////////////////////////

    ///////////////P_2//////////////////////
    public GameObject P_2_ThunderBullet;
    public GameObject P_2_ParentRot;
    ////////////////////////////////////////

    private Animator CloudBossAnim;
    private GameObject Player;
    private PlayerMovement PlayerScript;

    //랜덤 패턴을 위한 lsit
    private int nextPattern = 0;

    //보스 스탯
    public float CloudBossHP;
    public float CloudBossPattern1Damage;

    public float CloudBossPattern2Speed;
    public float CloudBossPattern2Damage;

    public float CloudBossPattern3Damage;

    private RoomManager Rm;


    void Start()
    {
        GameManager.Instance.PlayerDieEvent += this.PlayerOnDie;
        Player = GameObject.FindWithTag("Player");
        PlayerScript = Player.GetComponent<PlayerMovement>();
        CloudBossHP = 100;

        CloudBossAnim = GetComponent<Animator>();
        StartCoroutine(Pattern1());

        Rm = FindObjectOfType<RoomManager>();
    }

    private void Update()
    {
        Rm.CloudBoss_Hp.fillAmount = CloudBossHP / 100;
    }

    #region 패턴 인덱스 랜덤 필드
    private void nextPatternPlay()
    {
        nextPattern = Random.Range(1, 3);
        switch (nextPattern)
        {
            case 1:
                StartCoroutine(Pattern2());
                break;
            case 2:
                StartCoroutine(Pattern3());
                break;
            default:
                Debug.Log("뭔가 이상함");
                break;
        }
    }
    #endregion

    #region 패턴 1. 체인 라이트닝
    private List<GameObject> Lightnings = new List<GameObject>();
    IEnumerator Pattern1()
    {

        //현수 수정(운사 맵 가운데로)
        transform.position = new Vector2(60, -3.5f);
        yield return new WaitForSeconds(3f);

        //현수 수정(운사 맵 왼쪽 오른쪽) -> 필히 같이할 것!!!!!!!!!씨2발련아!!!!!!!
        GameObject L1 = Instantiate(P_1_ChainLightning_1,new Vector2(62.62f,-3.64f),Quaternion.identity);
        GameObject L2 = Instantiate(P_1_ChainLightning_2,new Vector2(57.89f,-3.64f),Quaternion.identity);


        Lightnings.Add(L1);
        Lightnings.Add(L2);
        yield return new WaitForSeconds(5f);

        nextPatternPlay();
    }
    #endregion

    #region 패턴 2. 번개 탄막
    private void AttackEuler(GameObject targetAnglePosition)
    {
        float targetAngle = Mathf.Atan2(Player.transform.position.y - targetAnglePosition.transform.position.y
            , Player.transform.position.x - targetAnglePosition.transform.position.x) * Mathf.Rad2Deg;
        targetAnglePosition.transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
    }
    private bool isP2play = false;
    IEnumerator Pattern2()
    {
        yield return new WaitForSeconds(3f);
        isP2play = true;
        CloudBossAnim.SetTrigger("P2_Cloud");
        yield return new WaitForSeconds(0.83f);
        CloudBossAnim.SetBool("P2_Clouding", true);
        StartCoroutine(Pattern2_SetShoot());
        StartCoroutine(Pattern2_SetShoot());
        StartCoroutine(Pattern2_SetShoot());
        StartCoroutine(Pattern2_SetShoot());

        yield return new WaitForSeconds(15f);
        CloudBossAnim.SetBool("P2_Clouding", false);
        isP2play = false;
        nextPatternPlay();
    }

    IEnumerator Pattern2_SetShoot()
    {
        int rindex = Random.Range(0, 3);
        int rsecond = Random.Range(1, 4);
        int symbol = Random.Range(0, 2);
        if (symbol == 0)
            symbol = 1;
        else
            symbol = -1;
        while (isP2play)
        {
            if (!isP2play)
            {
                break;
            }
            AttackEuler(P_2_ParentRot);
            if (rindex > 2)
                rindex = 0;
            P_2_ParentRot.transform.Rotate(new Vector3(0, 0, symbol * rindex * 10));
            rindex++;
            StartCoroutine(Pattern2_Shoot());
            yield return new WaitForSeconds(rsecond / 10f);
        }
    }

    IEnumerator Pattern2_Shoot()
    {
        GameObject ThunderBullet = Instantiate(P_2_ThunderBullet, P_2_ParentRot.transform.position, P_2_ParentRot.transform.rotation);
        ThunderBullet.gameObject.name = CloudBossPattern2Damage.ToString();
        Rigidbody2D rb = ThunderBullet.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = P_2_ParentRot.transform.right * CloudBossPattern2Speed;
        yield return null;
    }
    #endregion

    #region 패턴 3. 근접 공격
    private bool isP3Play;
    IEnumerator Pattern3()
    {
        yield return new WaitForSeconds(3.0f);
        //WindBossAnim.SetTrigger("P3_FlyStart");
        //WindBossAnim.SetBool("P3_Flying", true);
        isP3Play = true;
        StartCoroutine(Pattern3_Set());
        yield return new WaitForSeconds(15.0f);
        //WindBossAnim.SetBool("P3_Flying", false);
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
                if (distance < 0.8f)
                {
                    CloudBossAnim.SetTrigger("P3Start");
                    yield return new WaitForSeconds(0.45f);
                    if (!PlayerScript.isDodge)
                        GameManager.Instance.PlayerHit((int)CloudBossPattern3Damage);
                    yield return new WaitForSeconds(0.5f);
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
    }


    public void HitfromPlayer()
    {
        CloudBossHP -= GameManager.Instance.HitDamage;
        PlayerSkillManager.Instance.PassiveSkill(transform.position);
        if (CloudBossHP <= 0)
        {
            Boss3Die();
        }
    }
    public void HitfromPlayerSkill(int skillDamage)
    {
        CloudBossHP -= skillDamage;
        //피격추가
        if (CloudBossHP <= 0)
        {
            Boss3Die();
        }
    }

    private void Boss3Die()
    {
        CloudBossAnim.SetTrigger("doDeath");
        for (int index = 0; index < Lightnings.Count; index++)
        {
            Destroy(Lightnings[index]);
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
        GetComponent<Stage3CloudBossAI>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GameManager.Instance.PlayerDieEvent -= this.PlayerOnDie;
    }
    #endregion
}
