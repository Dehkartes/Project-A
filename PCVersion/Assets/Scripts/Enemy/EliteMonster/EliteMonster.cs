using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EliteMonster : MonoBehaviour
{
    public int EliteEnemyHp;
    public int nowHp;
    public int atkDmg;
    public int meleeDmg;
    public float atkDelay;
    public float atkSpeed;
    public float moveSpeed;
    public float atkRange;
    public float fieldOfVision;

    //weaponParent ����
    private float EweaponParentAngle;
    //����Ʈ ���� ���ø� ������
    private string[] enemyData;
    //���� ��Ÿ��
    private float attackDelay;
    //3�ܰ� ������ �ϰ� �ִ��� �ƴ���
    private bool isThirdAttack;

    //����Ʈ ���� ���� ������
    [SerializeField]
    private GameObject[] EliteMonsterWeaponPrefab;

    public GameObject EweaponAttackPosParent;
    public GameObject EweaponAttackPos;
    public GameObject targetPosition; //�÷��̾�
    public SpriteRenderer EliteSR;
    private Rigidbody2D EliteRb;
    private Animator EliteAnimator;
    private Animator Attack3Animator;

    //[FireElite] ��� ��� �ʵ�
    //private bool isColWall;
    //////////////////////////

    //[WaterElite] ��� ��� �ʵ�
    //private GameObject[] bool isColWall;
    //////////////////////////

    private void ClassificationEliteEnemy()
    {
        enemyData = gameObject.name.Split("_");
        //��
        if (enemyData[0] == "F")
        {
            //enemyType = false;
            switch (int.Parse(enemyData[1]))
            {
                //1�ܰ�
                case 1:
                    //1.hp 2.���Ÿ������� 3.���� ������ 4.���ݵ����� 5.���ݼӵ�(�Ѿ�) 6.�̵��ӵ� 7.���ݹ��� 8.�νĹ���
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1.5f, 1.7f);
                    break;
                case 2:
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1f, 2f);
                    break;
                case 3:
                    Attack3Animator = EweaponAttackPos.GetComponent<Animator>();
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1f, 2f);
                    break;
            }
        }
        //��
        else if (enemyData[0] == "W")
        {
            switch (int.Parse(enemyData[1]))
            {
                //1�ܰ�
                case 1:
                    //1.hp 2.���Ÿ������� 3.���� ������ 4.���ݵ����� 5.���ݼӵ�(�Ѿ�) 6.�̵��ӵ� 7.���ݹ��� 8.�νĹ���
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1f, 1.3f);
                    break;
                case 2:
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1f, 2f);
                    break;
                case 3:
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1f, 2f);
                    break;
            }
        }
        //��
        else if (enemyData[0] == "E")
        {
            switch (int.Parse(enemyData[1]))
            {
                //1�ܰ�
                case 1:
                    //1.hp 2.���Ÿ������� 3.���� ������ 4.���ݵ����� 5.���ݼӵ�(�Ѿ�) 6.�̵��ӵ� 7.���ݹ��� 8.�νĹ���
                    SetEnemyStatus(10, 1, 0, 1f, 5f, 0.1f, 1f, 1.3f);
                    break;
                case 2:
                    SetEnemyStatus(100, 1, 0, 1f, 5f, 0.1f, 1f, 2f);
                    break;
                case 3:
                    SetEnemyStatus(100, 1, 0, 1f, 5f, 0.1f, 1f, 2f);
                    break;
            }
        }
    }
    public void SetEnemyStatus(int _Hp, int _atkDmg, int _meleeDmg,float _atkDelay, float _atkSpeed, float _moveSpeed, float _atkRange, float _fieldOfVision)
    {
        EliteEnemyHp = _Hp;
        atkDmg = _atkDmg;
        meleeDmg = _meleeDmg;
        atkDelay = _atkDelay;
        atkSpeed = _atkSpeed;
        moveSpeed = _moveSpeed;
        atkRange = _atkRange;
        fieldOfVision = _fieldOfVision;
    }

    void Start()
    {
        GameManager.Instance.PlayerDieEvent += this.PlayerOnDie;
        EliteRb = GetComponent<Rigidbody2D>();
        EliteAnimator = GetComponent<Animator>();
        isThirdAttack = false;

        ClassificationEliteEnemy();
    }

    private void Update()
    {
        if (!isThirdAttack)
        {
            EliteAI();
        }
    }
    public void AttackPosition()
    {
        EweaponParentAngle = Mathf.Atan2(targetPosition.transform.position.y - transform.position.y
            , targetPosition.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        EweaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(EweaponParentAngle, Vector3.forward);
    }

    private void EliteAI()
    {
        attackDelay -= Time.deltaTime;
        if (attackDelay < 0)
            attackDelay = 0;

        float distance = Vector3.Distance(transform.position, targetPosition.transform.position);

        if (distance <= fieldOfVision)
        {
            //if (isPatrolling)
            //{
            //    //Patrol �ڷ�ƾ�� �����Ű�� ���� Stop
            //    StopAllCoroutines();
            //    checkCoroutine = false;
            //    isPatrol = false;
            //    isPatrolling = false;
            //    enemy.EweaponAttakPosSR.flipX = false;
            //}
            AttackPosition();
            FaceTarget();
            if (distance <= atkRange)
            {
                AttackTarget(distance);
            }
            else
            {
                MoveToTarget();
            }
        }
    }

    #region  FaceTarget, flip ���� �ʵ�
    private void FaceTarget()
    {
        // Ÿ���� ���ʿ� ���� ��
        if (targetPosition.transform.position.x - transform.position.x < 0)
        {
            EliteSR.flipX = true;
        }
        // Ÿ���� �����ʿ� ���� ��
        else
        {
            EliteSR.flipX = false;
        }
    }
    #endregion

    #region Attack ���� �ʵ�
    private void AttackTarget(float distance)
    {
        if(attackDelay == 0)
        {
            if (enemyData[1] == "1")
            {
                FirstEliteAttack();
            }
            else if (enemyData[1] == "2")
            {
                SecondEliteAttack(distance);
            }
            else
            {
                ThirdEliteAttack(distance);
            }
            attackDelay = atkDelay;
        }
    }

    private void FirstEliteAttack()
    {
        //enemyAnimator.SetTrigger("isAttack");
        GameObject bullet = Instantiate(EliteMonsterWeaponPrefab[0], EweaponAttackPos.transform.position, EweaponAttackPos.transform.rotation);
        bullet.name = gameObject.name + "_" + atkDmg;
        Rigidbody2D rb = bullet.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = EweaponAttackPos.transform.right * atkSpeed;
    }

    private void SecondEliteAttack(float distance)
    {
        //0.5 ���̸� ��������
        if(distance <= 0.5f)
        {
            Debug.Log("2�ܰ� ��������");
            Debug.Log(distance);
            //enemyAnimator.SetTrigger("isAttack");
            //enemyWeaponAnimator.SetTrigger("isAttack");
            GameManager.Instance.HP -= atkDmg;
        }
        else
        {
            GameObject bullet = Instantiate(EliteMonsterWeaponPrefab[0], EweaponAttackPos.transform.position, EweaponAttackPos.transform.rotation);
            bullet.name = gameObject.name + "_" + atkDmg;
            Rigidbody2D rb = bullet.gameObject.GetComponent<Rigidbody2D>();
            rb.velocity = EweaponAttackPos.transform.right * atkSpeed;
        }
    }

    private void ThirdEliteAttack(float distance)
    {
        if (distance <= 0.7f)
        {
            Debug.Log("3�ܰ� ��������");
            Debug.Log(distance);
            //enemyAnimator.SetTrigger("isAttack");
            //enemyWeaponAnimator.SetTrigger("isAttack");
            GameManager.Instance.HP -= atkDmg;
        }
        else
        {
            if (enemyData[0] == "F")
            {
                Fire3Pattern();
            }
            else if(enemyData[0] == "W")
            {
                Water3Pattern();
            }
            else if(enemyData[0] == "E")
            {
                Debug.Log("��");
            }
        }
    }
    #endregion

    #region ����Ʈ ���� 3�ܰ� ���� �ʵ�

    #region Fire 3�ܰ� �ʵ�
    private void Fire3Pattern()
    {
        isThirdAttack = true;
        EliteSR.enabled = false;
        Attack3Animator.SetBool("isContact",false);     
        EliteRb.velocity = EweaponAttackPos.transform.right * 2f;
    }
    #endregion

    #region Water 3�ܰ� �ʵ�
    private void Water3Pattern()
    {
        isThirdAttack = true;
        //W_3 ��ų ���� �ִϸ��̼� �߰�
        GameObject W3Skill = Instantiate(EliteMonsterWeaponPrefab[0], targetPosition.transform.position, 
            Quaternion.Euler(0,0,0));
        Invoke("Water3Pattern", 1f);
    }
    #endregion

    private void PatternOut()
    {
        isThirdAttack = false;
    }
    #endregion

    #region Move ���� �ʵ�
    private void MoveToTarget()
    {
        Vector2 direction = targetPosition.transform.position - transform.position;
        transform.Translate(new Vector2(direction.x, direction.y) * moveSpeed * Time.deltaTime);
        //enemyAnimator.SetBool("isMove", true);
    }
    #endregion

    #region �浹 ó�� �ʵ�
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //F_3 ��� �ʵ�
        if (collision.gameObject.CompareTag("Wall"))
        {
            if(enemyData[0] == "F")
            {
                Attack3Animator.SetBool("isContact", true);
                EliteSR.enabled = true;
                Invoke("PatternOut", 2f);
                //EweaponAttackPos.transform.eulerAngles = EweaponAttackPos.transform.eulerAngles + Quaternion.Euler(0, 0, 180);
            }
        }
    }
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
            Destroy(col.gameObject);
        }
    }
    #endregion

    #region EliteEnemy Attacked(�ǰ�), Death �ʵ�
    public void HitfromPlayer()
    {
        EliteEnemyHp -= GameManager.Instance.HitDamage; 
        if (EliteEnemyHp <= 0)
        {
            Die();
        }
    }
    public void HitfromPlayerSkill(int skillDamage)
    {
        EliteEnemyHp -=  skillDamage;
        if (EliteEnemyHp <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        //enemyAnimator.SetTrigger("die");            // die �ִϸ��̼� ����
        GetComponent<EliteMonster>().enabled = false;
        GetComponent<Collider2D>().enabled = false; // �浹ü ��Ȱ��ȭ
        Destroy(gameObject, 1);
        GameManager.Instance.IncreaseAttribute();
    }

    //�÷��̾� �����
    private void PlayerOnDie()
    {
        GetComponent<EliteMonster>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GameManager.Instance.PlayerDieEvent -= this.PlayerOnDie;
    }
    #endregion

}
