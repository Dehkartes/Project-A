using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage2BossAI : MonoBehaviour
{
    //스테이지 2 보스
    //[패턴]
    //1. 팔 휘두르면서 탄막 발사
    //2. 몸에서 럴커 가시 나가기 슈웃~
    //3. 맵 왼쪽 오른쪽에서 팔 나와서 휘두르기

    ///////////////P_1//////////////////////
    [SerializeField]
    private GameObject P_1_Leaf_1;
    [SerializeField]
    private GameObject P_1_Leaf_2;
    [SerializeField]
    private GameObject P_1_ParentPos;
    [SerializeField]
    private GameObject P_1_Pos_1;
    [SerializeField]
    private GameObject P_1_Pos_2;
    [SerializeField]
    private GameObject P_1_Pos_3;
    [SerializeField]
    private GameObject P_1_Pos_4;
    [SerializeField]
    private GameObject P_1_Pos_5;

    public GameObject P_1_Bullet_1;
    public GameObject P_1_Bullet_2;
    ////////////////////////////////////////


    ///////////////P_2//////////////////////
    [SerializeField]
    private GameObject P_2_Pos_1;
    [SerializeField]
    private GameObject P_2_Pos_2;
    [SerializeField]
    private GameObject P_2_Pos_3;
    [SerializeField]
    private GameObject P_2_Pos_4;

    public GameObject P_2_Check;
    public GameObject P_2_Thorn;
    public GameObject P_2_Warn;
    ////////////////////////////////////////

    ///////////////P_3//////////////////////
    [SerializeField]
    private GameObject P_3_Pos_1;
    [SerializeField]
    private GameObject P_3_Pos_2;
    [SerializeField]
    private GameObject P_3_Pos_3;
    [SerializeField]
    private GameObject P_3_Pos_4;
    [SerializeField]
    private GameObject P_3_Pos_5;
    [SerializeField]
    private GameObject P_3_Pos_6;

    public GameObject P_3_Thorn_L;
    public GameObject P_3_Thorn_R;
    public GameObject P_3_Warn_L;
    public GameObject P_3_Warn_R;
    ////////////////////////////////////////


    private Animator Boss2Anim;
    private GameObject Player;

    //랜덤 패턴을 위한 lsit
    List<int> originalList;
    private int nextPattern = 0;

    //보스 스탯
    public float Boss2HP;
    public float Boss2Pattern1Speed;
    public float Boss2Pattern1Damage;

    //패턴 1 실행도중 판별을 위한 bool
    private bool isP1play = false;

    private float localRotateZ = -90;

    private RoomManager Rm;


    private void Start()
    {
        GameManager.Instance.PlayerDieEvent += this.PlayerOnDie;
        Player = GameObject.FindWithTag("Player");

        Rm = FindObjectOfType<RoomManager>();

        Boss2HP = 100;
        Boss2Pattern1Speed = 2f;

        P_1_ParentPos.transform.localRotation = Quaternion.Euler(0, 0, localRotateZ);

        Boss2Anim = GetComponent<Animator>();
        originalList = new List<int>();
        StartCoroutine(Pattern1());
    }

    #region 보스 HP 관련
    private void Update()
    {
        Rm.BossHpImage_2.fillAmount = (float)Boss2HP / 100;
    }
    #endregion


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

    #region 플레이어를 향하는 공격 방향 설정 필드
    private void AttackEuler(GameObject targetAnglePosition)
    {
        float targetAngle = Mathf.Atan2(Player.transform.position.y - targetAnglePosition.transform.position.y
            , Player.transform.position.x - targetAnglePosition.transform.position.x) * Mathf.Rad2Deg;
        targetAnglePosition.transform.rotation = Quaternion.AngleAxis(targetAngle, Vector3.forward);
    }
    #endregion

    #region 패턴 1. 탄막 패턴
    IEnumerator Pattern1()
    {
        yield return new WaitForSeconds(3f);
        GetRandomNum(1);
        isP1play = true;
        Boss2Anim.SetTrigger("P1Start");
        yield return new WaitForSeconds(0.5f);
        Boss2Anim.SetBool("isP1", true);
        StartCoroutine(Pattern1_Rotate());
        StartCoroutine(Pattern1_SetShoot());
        StartCoroutine(LeafPattern1_SetShoot());
        yield return new WaitForSeconds(9.0f);
        Boss2Anim.SetTrigger("P1End");
        yield return new WaitForSeconds(0.5f);
        Boss2Anim.SetBool("isP1", false);
        isP1play = false;
        localRotateZ = -90f;
        P_1_ParentPos.transform.localRotation = Quaternion.Euler(0, 0, localRotateZ);
        yield return new WaitForSeconds(5.0f);
        nextPatternPlay();
    }

    private float rotateDir = 1;
    private float rotateSpeed = 10;
    IEnumerator Pattern1_Rotate()
    {
        while (isP1play)
        {
            localRotateZ += rotateSpeed * Time.deltaTime * rotateDir;
            P_1_ParentPos.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, localRotateZ));
            if (localRotateZ >= -60 || localRotateZ <= -120)
            {
                rotateDir *= -1;
            }
            if (!isP1play)
            {
                break;
            }
            yield return null;
        }
    }
    IEnumerator LeafPattern1_SetShoot()
    {
        while (isP1play)
        {
            AttackEuler(P_1_Leaf_1);
            AttackEuler(P_1_Leaf_2);
            StartCoroutine(Pattern1_Shoot(P_1_Leaf_1,1));
            StartCoroutine(Pattern1_Shoot(P_1_Leaf_2,1));
            if (!isP1play)
            {
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator Pattern1_SetShoot()
    {
        while (isP1play)
        {
            StartCoroutine(Pattern1_Shoot(P_1_Pos_1,2));
            StartCoroutine(Pattern1_Shoot(P_1_Pos_2, 2));
            StartCoroutine(Pattern1_Shoot(P_1_Pos_3, 2));
            StartCoroutine(Pattern1_Shoot(P_1_Pos_4, 2));
            StartCoroutine(Pattern1_Shoot(P_1_Pos_5, 2));
            if (!isP1play)
            {
                break;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Pattern1_Shoot(GameObject attackPos,int type)
    {
        if(type == 1)
        {
            GameObject boss2Bullet = Instantiate(P_1_Bullet_1, attackPos.transform.position, Quaternion.identity);
            boss2Bullet.gameObject.name = Boss2Pattern1Damage.ToString();
            Rigidbody2D rb = boss2Bullet.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = attackPos.transform.right * (Boss2Pattern1Speed * 2f);
        }
        else
        {
            GameObject boss2Bullet = Instantiate(P_1_Bullet_2, attackPos.transform.position, Quaternion.identity);
            boss2Bullet.gameObject.name = Boss2Pattern1Damage.ToString();
            Rigidbody2D rb = boss2Bullet.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = attackPos.transform.right * Boss2Pattern1Speed;
        }
        yield return null;
    }

    #endregion

    #region 패턴 2. 럴커 패턴
    IEnumerator Pattern2()
    {
        yield return new WaitForSeconds(3.0f);
        Boss2Anim.SetTrigger("P2Start");
        yield return new WaitForSeconds(0.5f);
        Boss2Anim.SetBool("isP2",true);
        yield return new WaitForSeconds(3.0f);
        GetRandomNum(2);
        for (int index = 0; index < 3; index++)
        {
            StartCoroutine(Pattern2_SetShoot(P_2_Pos_1,1));
            StartCoroutine(Pattern2_SetShoot(P_2_Pos_2,2));
            StartCoroutine(Pattern2_SetShoot(P_2_Pos_3,3));
            StartCoroutine(Pattern2_SetShoot(P_2_Pos_4,4));
            yield return new WaitForSeconds(5.0f);
        }
        
        Boss2Anim.SetTrigger("P2End");
        yield return new WaitForSeconds(0.2f);
        Boss2Anim.SetBool("isP2", false);
        yield return new WaitForSeconds(0.2f);
        //idle 로 돌아가기 (Anim)
        yield return new WaitForSeconds(5.0f);
        nextPatternPlay();
    }

    IEnumerator P2Destroy(GameObject thorn, int posnum, float time)
    {
        Animator thornAnim = thorn.GetComponent<Animator>();
        SpriteRenderer thornSR = thorn.GetComponent<SpriteRenderer>();
        if (posnum > 2)
        {
            thornSR.flipX = true;
        }
        else
        {
            thornSR.flipX = false;
        }        
        Destroy(thorn, time);
        yield return new WaitForSeconds(time - 0.4f);
        thornAnim.SetTrigger("isP2End");
    }

    IEnumerator Pattern2_SetShoot(GameObject P_2_pos,int posNum)
    {
        AttackEuler(P_2_pos);
        GameObject P_2AttackCheck = Instantiate(P_2_Check, P_2_pos.transform.position, Quaternion.Euler(0, 0, 0));
        P_2AttackCheck.transform.SetParent(this.transform);
        Rigidbody2D rb = P_2AttackCheck.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = P_2_pos.transform.right * 0.6f;
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if (P_2AttackCheck == null)
            {
                break;
            }
            GameObject P_2Warn = Instantiate(P_2_Warn, P_2AttackCheck.transform.position, Quaternion.Euler(0, 0, 0));
            Vector2 oriPos = P_2AttackCheck.transform.position;
            P_2Warn.transform.SetParent(this.transform);
            yield return new WaitForSeconds(1f);
            Destroy(P_2Warn);
            GameObject P_2thorn = Instantiate(P_2_Thorn, oriPos, Quaternion.Euler(0, 0, 0));
            P_2thorn.transform.SetParent(this.transform);
            StartCoroutine(P2Destroy(P_2thorn, posNum, 3f));
        }
        yield return null;
    }


    #endregion

    #region 패턴 3. 나무휘두루빠빠
    IEnumerator Pattern3()
    {
        yield return new WaitForSeconds(3f);
        GetRandomNum(3);
        int rand = Random.Range(0, 2);
        Boss2Anim.SetTrigger("P3Start");
        yield return new WaitForSeconds(0.3f);
        Boss2Anim.SetBool("isP3", true);
        if (rand == 0)
        {
            StartCoroutine(Pattern3_SetShoot(P_3_Pos_1,"R"));
            StartCoroutine(Pattern3_SetShoot(P_3_Pos_2,"L"));
            StartCoroutine(Pattern3_SetShoot(P_3_Pos_3,"R"));
        }
        else
        {
            StartCoroutine(Pattern3_SetShoot(P_3_Pos_4, "L"));
            StartCoroutine(Pattern3_SetShoot(P_3_Pos_5, "R"));
            StartCoroutine(Pattern3_SetShoot(P_3_Pos_6, "L"));
        }
        yield return new WaitForSeconds(12.0f);
        nextPatternPlay();
    }
    private void P3Destroy(GameObject thorn, float time)
    {
        Destroy(thorn, time);
    }
    IEnumerator Pattern3_SetShoot(GameObject P_3_pos,string vec)
    {
        if(vec == "R")
        {
            GameObject P3_Warn_r = Instantiate(P_3_Warn_R, P_3_pos.transform.position, Quaternion.Euler(0, 0, 0));
            P3_Warn_r.transform.SetParent(this.transform);
            yield return new WaitForSeconds(2.0f);
            Destroy(P3_Warn_r);
            GameObject P3_Thorn_r = Instantiate(P_3_Thorn_R, P_3_pos.transform.position, Quaternion.Euler(0, 0, 0));
            P3_Thorn_r.transform.SetParent(this.transform);
            Animator thornAnim_r = P3_Thorn_r.GetComponent<Animator>();
            thornAnim_r.SetTrigger("P3Start");
            P3Destroy(P3_Thorn_r, 10f);
            yield return new WaitForSeconds(9.0f);
            thornAnim_r.SetTrigger("P3End");
            yield return new WaitForSeconds(0.2f);
        }
        else
        {
            GameObject P3_Warn_l = Instantiate(P_3_Warn_L, P_3_pos.transform.position, Quaternion.Euler(0, 0, 0));
            P3_Warn_l.transform.SetParent(this.transform);
            yield return new WaitForSeconds(2.0f);
            Destroy(P3_Warn_l);
            GameObject P3_Thorn_l = Instantiate(P_3_Thorn_L, P_3_pos.transform.position, Quaternion.Euler(0, 0, 0));
            P3_Thorn_l.transform.SetParent(this.transform);
            Animator thornAnim_l = P3_Thorn_l.GetComponent<Animator>();
            thornAnim_l.SetTrigger("P3Start");
            P3Destroy(P3_Thorn_l, 10f);
            yield return new WaitForSeconds(9.0f);
            thornAnim_l.SetTrigger("P3End");
            yield return new WaitForSeconds(0.2f);
        }
        Boss2Anim.SetTrigger("P3End");
        yield return new WaitForSeconds(0.3f);
        Boss2Anim.SetBool("isP3", false);
    }
    #endregion

    #region 충돌 처리, 보스 사망 필드
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
        Debug.Log("공격들옴");
        Boss2HP -= GameManager.Instance.HitDamage;
        PlayerSkillManager.Instance.PassiveSkill(transform.position);
        if (Boss2HP <= 0)
        {
            Boss2Die();
        }
    }
    public void HitfromPlayerSkill(int skillDamage)
    {
        Boss2HP -= skillDamage;
        //피격추가
        if (Boss2HP <= 0)
        {
            Boss2Die();
        }
    }

    private void Boss2Die()
    {
        Boss2Anim.SetTrigger("isBoss2Die");
        StopAllCoroutines(); //-> 이 방식은 해당 스크립트내의 모든 코루틴 종료(다른 스크립트 X)
        GetComponent<CapsuleCollider2D>().enabled = false; // 충돌체 비활성화
        Destroy(gameObject, 2);
    }
    #endregion

    #region 플레이어 사망 이벤트
    private void PlayerOnDie()
    {
        GetComponent<Stage2BossAI>().enabled = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        GameManager.Instance.PlayerDieEvent -= this.PlayerOnDie;
    }
    #endregion
}
