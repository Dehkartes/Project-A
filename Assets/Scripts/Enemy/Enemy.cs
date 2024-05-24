using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public int enemyHp;
    public int nowHp;
    public int atkDmg;
    public float atkDelay;
    public float atkSpeed;
    public float moveSpeed;
    public float atkRange;
    public float fieldOfVision;

    public GameObject EweaponAttackPosParent;
    public GameObject EweaponAttackPos;
    public GameObject EAttackRangePos;
    public SpriteRenderer EnemySR;
    public SpriteRenderer EweaponAttakPosSR;
    public GameObject targetPosition; //플레이어

    private float EweaponParentAngle;

    //피격 판정을 위한 변수
    public Rigidbody2D rb;
    EnemyAI enemyAI;

    //애니메이션
    public Animator enemyAnimator;

    public void SetEnemyStatus(int _Hp, int _atkDmg, float _atkDelay, float _atkSpeed, float _moveSpeed, float _atkRange, float _fieldOfVision)
    {
        enemyHp = _Hp;
        atkDmg = _atkDmg;
        atkDelay = _atkDelay;
        atkSpeed = _atkSpeed;
        moveSpeed = _moveSpeed;
        atkRange = _atkRange;
        fieldOfVision = _fieldOfVision;
    }

    void Awake()
    {
        //EweaponAttackPos.transform.position = new Vector2(transform.position.x, transform.position.y);
        enemyAnimator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyAI = GetComponent<EnemyAI>();
        EnemySR.flipX = true;
    }

    private void Start()
    {
        GameManager.Instance.PlayerDieEvent += this.PlayerOnDie;

        targetPosition = GameObject.FindWithTag("Player");
    }

    //EweaponAttackPosParent의 각도 제어
    public void AttackPosition()
    {
        EweaponParentAngle = Mathf.Atan2(targetPosition.transform.position.y - transform.position.y
            , targetPosition.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        EweaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(EweaponParentAngle, Vector3.forward);
    }

    #region 충돌 처리 필드
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
    #endregion

    #region Enemy Attacked(피격), Death 필드
    public void HitfromPlayerSkill(int skillDamage)
    {
        enemyHp -= skillDamage;
        if (enemyHp <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(AttackedOut());
        }
    }

    public void HitfromPlayer()
    {
        enemyHp -= GameManager.Instance.HitDamage;
        PlayerSkillManager.Instance.PassiveSkill(transform.position);
        if (enemyHp <= 0)
        { 
            StartCoroutine(Die());
        }
        else
        {
            StartCoroutine(AttackedOut());
        }
    }
    IEnumerator AttackedOut()
    {
        enemyAnimator.SetTrigger("isHit");
        Vector2 dir = targetPosition.transform.position - transform.position;
        rb.AddForce(-dir * GameManager.Instance.knockback, ForceMode2D.Impulse);
        enemyAI.attackedMove = true;
        yield return new WaitForSeconds(0.2f);
        rb.velocity = new Vector2(0, 0);
        enemyAI.attackedMove = false;

        //맞으면 다시 그방향으로 쫓아가는 코드
        //추후 수정
        //yield return new WaitForSeconds(0.1f);
        //rb.AddForce(dir * GameManager.Instance.knockback, ForceMode2D.Impulse);
    }

    IEnumerator Die()
    {
        this.enemyAnimator.SetTrigger("isDie");            // die 애니메이션 실행
        enemyAI.attackedMove = true;
        Vector2 dir = targetPosition.transform.position - transform.position;
        rb.AddForce(-dir * GameManager.Instance.knockback, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        rb.velocity = new Vector2(0, 0);

        EweaponAttakPosSR.enabled = false; 

        GetComponent<EnemyAI>().enabled = false;
        GetComponent<Collider2D>().enabled = false; // 충돌체 비활성화
        ITEMMANAGER.Instance.ItemDrop(gameObject);
        Destroy(gameObject, 2);            
        StopAllCoroutines();
    }
    #endregion


    void SetAttackSpeed(float speed)
    {
        //enemyAnimator.SetFloat("attackSpeed", speed);
    }

    #region 플레이어 사망 이벤트
    private void PlayerOnDie()
    {
        GetComponent<EnemyAI>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.PlayerDieEvent -= this.PlayerOnDie;
    }
    #endregion
}

