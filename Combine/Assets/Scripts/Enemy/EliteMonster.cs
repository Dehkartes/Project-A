using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteMonster : MonoBehaviour
{
    public int enemyHp;
    public int nowHp;
    public int atkDmg;
    public float atkDelay;
    public float atkSpeed;
    public float moveSpeed;
    public float atkRange;
    public float fieldOfVision;

    public GameObject targetPosition; //�÷��̾�


    //�ǰ� ������ ���� ����
    Rigidbody2D rb;

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
        //test
        enemyHp = 100;

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        //EweaponAttackPos.transform.position = new Vector2(transform.position.x + 0.2f, transform.position.y);
    }

    public void AttackPosition()
    {
        //EweaponParentAngle = Mathf.Atan2(targetPosition.transform.position.y - transform.position.y
        //    , targetPosition.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        //EweaponAttackPosParent.transform.rotation = Quaternion.AngleAxis(EweaponParentAngle, Vector3.forward);
    }

    #region �浹 ó�� �ʵ�
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("PlayerWeapon"))
        {
            Destroy(col.gameObject);
            HitfromPlayer();
        }
    }
    #endregion

    #region Enemy Attacked(�ǰ�), Death �ʵ�
    public void HitfromPlayer()
    {
        enemyHp -= GameManager.Instance.HitDamage; 

        //Vector2 dir = targetPosition.transform.position - transform.position;
        //rb.AddForce(-dir * GameManager.Instance.knockback, ForceMode2D.Impulse);
        //enemyAI.attackedMove = true;
        //StartCoroutine(AttackedOut(dir));

        if (enemyHp <= 0)
        {
            Die();
        }
    }
    IEnumerator AttackedOut(Vector2 dir)
    {

        yield return new WaitForSeconds(0.2f);
        rb.velocity = new Vector2(0, 0);
        //enemyAI.attackedMove = false;

        //������ �ٽ� �׹������� �Ѿư��� �ڵ�
        //���� ����
        //yield return new WaitForSeconds(0.1f);
        //rb.AddForce(dir * GameManager.Instance.knockback, ForceMode2D.Impulse);
    }


    private void Die()
    {
        //enemyAnimator.SetTrigger("die");            // die �ִϸ��̼� ����

        GetComponent<Collider2D>().enabled = false; // �浹ü ��Ȱ��ȭ
        Destroy(gameObject, 1);
        GameManager.Instance.IncreaseAttribute();
    }
    #endregion
}
