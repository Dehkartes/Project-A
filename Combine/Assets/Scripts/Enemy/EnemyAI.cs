using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //OnDrawgizmo ������ ��� ���߿� ����
    public GameObject EAttackRangePos;

    //�迭�� �ٲٱ�
    public GameObject testprefabBullet;
    //Enemy ��ũ��Ʈ
    Enemy enemy;
 
    //Enemy ��ũ��Ʈ �ִϸ�����
    Animator enemyAnimator;
    //EnemyWeaponAnim ��ũ��Ʈ �ִϸ�����
    public Animator enemyWeaponAnimator;

    //melee ������ ���� ���� ����
    public Vector2 OverlapBoxSize;

    //���� ��Ÿ��
    float attackDelay;

    float RayMaxDistance = 1f;

    //true�� ���� false�� ���Ÿ� 
    private bool enemyType;
    //�� �ǰݽ� �ִϸ��̼� ���� ������ �ƴ���
    public bool attackedMove;
    //���Ÿ� �� ���ݽ� ���߱����� ����ϴ� ����
    private bool isDAttacking;

    //patrol�� ���� ����
    private List<int> originalList;
    private int patrolPattern;
    private int checkCoroutineIdx;
    //patrol �ڷ�ƾ ���� Ȯ���� ���� ����
    private bool isPatrol;
    //patrol�� ���������� �ƴ��� Ȯ���� ���� ����
    private bool isPatrolling;
    private bool checkCoroutine;

    private void ClassificationEnemy()
    {
        string[] enemyData = gameObject.name.Split("_");
        enemy.EweaponAttackPos.transform.localPosition = new Vector2(0, 0);
        //1��������
        if (int.Parse(enemyData[0]) == 1)
        {
            //���Ÿ�
            if(enemyData[1] == "D")
            {
                //false �� ���Ÿ� ��
                enemyType = false;
                switch (int.Parse(enemyData[3]))
                {
                    case 1:
                        //1.hp 2.������ 3.���ݵ����� 4.���ݼӵ�(�Ѿ�) 5.�̵��ӵ� 6.���ݹ��� 7.�νĹ���
                        enemy.SetEnemyStatus(10, 7, 1f, 5f, 1.2f, 1f, 1.3f);
                        //enemy.SetEnemyStatus(10, 7, 1f, 5f, 3f, 3f, 5f);
                        break;
                    case 2:
                        enemy.SetEnemyStatus(100, 7, 1f, 5f, 1.2f, 1f, 2f);
                        //enemy.SetEnemyStatus(10, 7, 1f, 5f, 3f, 3f, 5f);
                        break;
                }
            }
            //�ٰŸ�
            else
            {
                enemyType = true;
                switch (int.Parse(enemyData[3]))
                {
                    case 1:
                        enemy.SetEnemyStatus(10, 7, 1f, 5f, 0.1f, 0, 1f);
                        //�����϶��� �ΰ� ����
                        OverlapBoxSize = new Vector2(0.2f, 0.2f);
                        enemy.EAttackRangePos.transform.localPosition = new Vector2(0.4f, 0f);
                        break;
                    case 2:
                        enemy.SetEnemyStatus(10, 7, 1f, 5f, 0f, 1f, 1.5f);
                        OverlapBoxSize = new Vector2(0.25f, 0.4f);
                        enemy.EAttackRangePos.transform.localPosition = new Vector2(0.46f, 0f);
                        break;
                }
            }
        }
        //2��������
        else if(int.Parse(enemyData[0]) == 2)
        {

        }
        //3��������
        else if (int.Parse(enemyData[0]) == 3)
        {

        }
    }
    void Start()
    {
        OverlapBoxSize = new Vector2(0, 0);

        checkCoroutine = false;
        isPatrol = false;
        isPatrolling = false;
        isDAttacking = false;

        //patrolPattern = Random.Range(1,5);
        patrolPattern = 0;

        originalList = new List<int>();
        attackedMove = false;
        enemy = GetComponent<Enemy>();
        enemyAnimator = enemy.enemyAnimator;

        ClassificationEnemy();
    }

    void Update()
    {
        AI();
    }

    #region �� AI
    private void AI()
    {
        attackDelay -= Time.deltaTime;
        if (attackDelay < 0)
            attackDelay = 0;

        float distance = Vector3.Distance(transform.position, enemy.targetPosition.transform.position);

        if (attackedMove == false)
        {
            if (distance <= enemy.fieldOfVision)
            {
                if (isPatrolling)
                {
                    //Patrol �ڷ�ƾ�� �����Ű�� ���� Stop
                    StopAllCoroutines();
                    checkCoroutine = false;
                    isPatrol = false;
                    isPatrolling = false;
                    enemy.EweaponAttakPosSR.flipX = false;
                }
                enemy.AttackPosition();
                FaceTarget();
                if (attackDelay == 0)
                {
                    AttackTarget(distance);
                }
                else
                {
                    if (!isDAttacking)
                    {
                        MoveToTarget();
                    }
                }
            }
            else
            {
                NotFoundTarget();
            }
        }
    }

    #region Patrol(����) AI
    private void NotFoundTarget()
    {
        //�̿ϼ�
        //����ĳ��Ʈ �߰�
        if(isPatrol == false)
        {
            if(checkCoroutine == false)
            {
                checkCoroutine = true;
                StartCoroutine(CheckPatrol());
            }
            if(isPatrolling == false)
            {
                isPatrolling = true;
                enemy.EweaponAttackPosParent.transform.rotation = Quaternion.identity;
                enemy.EweaponAttakPosSR.flipY = false;
                if (enemy.EnemySR.flipX == false)
                {
                    enemy.EweaponAttakPosSR.flipX = true;  
                }
                else
                {
                    enemy.EweaponAttakPosSR.flipX = false;
                }
                patrolPattern = 0;
                checkCoroutineIdx = Random.Range(1, 5);
            }
            switch (patrolPattern)
            {
                //�ٽ� ������ �����Ҷ� ������ 0����
                case 0:
                    if (enemyAnimator.GetBool("isMove"))
                    {
                        enemyAnimator.SetBool("isMove", false);
                    }
                    break;
                case 1:
                    if (enemy.EnemySR.flipX == false)
                    {
                        enemy.EnemySR.flipX = true;
                        enemy.EweaponAttakPosSR.flipX = false;
                    }
                    transform.Translate(Vector3.right * 0.1f * Time.deltaTime);
                    checkCoroutineIdx = 1;
                    break;
                case 2:
                    if(enemy.EnemySR.flipX == true)
                    {
                        enemy.EnemySR.flipX = false;
                        enemy.EweaponAttakPosSR.flipX = true;
                    }
                    transform.Translate(Vector3.left * 0.1f * Time.deltaTime);
                    checkCoroutineIdx = 2;
                    break;
                case 3:
                    transform.Translate(Vector3.up * 0.1f * Time.deltaTime);
                    checkCoroutineIdx = 3;
                    break;
                case 4:
                    transform.Translate(Vector3.down * 0.1f * Time.deltaTime);
                    checkCoroutineIdx = 4;
                    break;
            }
        }
    }
    private void GetRandomNum(int patternNum)
    {
        originalList.Add(1);
        originalList.Add(2);
        originalList.Add(3);
        originalList.Add(4);
        originalList.RemoveAt(patternNum - 1);
        int idx = Random.Range(0, 3);
        patrolPattern = originalList[idx];
        originalList.Clear();
    }
    IEnumerator CheckPatrol()
    {
        yield return new WaitForSeconds(2f);
        isPatrol = true;
        enemyAnimator.SetBool("isMove", false);
        yield return new WaitForSeconds(2f);
        GetRandomNum(checkCoroutineIdx);
        isPatrol = false;
        enemyAnimator.SetBool("isMove", true);
        yield return StartCoroutine(CheckPatrol());
    }
    #endregion

    #region �̵�(�÷��̾��) AI
    private void MoveToTarget()
    {
        Vector2 direction = enemy.targetPosition.transform.position - transform.position;
        //////////////////////////////////////////////�ϼ� �ϱ� (�̿ϼ�) 
        //RaycastHit2D hit = Physics2D.Raycast(transform.position,
        //    direction, RayMaxDistance, LayerMask.GetMask("Obstacle"));
        //if (hit)
        //{
        //    RayMaxDistance = 2f;
        //}
        //else
        //{
        //    RayMaxDistance = 1f;
        //}
        //////////////////////////////////////////////
        transform.Translate(new Vector2(direction.x, direction.y) * enemy.moveSpeed * Time.deltaTime);
        enemyAnimator.SetBool("isMove", true);
    }

    //Flip
    private void FaceTarget()
    {
        if (enemy.targetPosition.transform.position.x - transform.position.x < 0) // Ÿ���� ���ʿ� ���� ��
        {
            enemy.EnemySR.flipX = false;
            enemy.EweaponAttakPosSR.flipY = true;
        }
        else // Ÿ���� �����ʿ� ���� ��
        {
            enemy.EnemySR.flipX = true;
            enemy.EweaponAttakPosSR.flipY = false;
        }
    }
    #endregion

    #region ����(�÷��̾��) AI
    private void AttackTarget(float distance)
    {
        if (enemyType == true)
        {
            MeleeAttack();
        }
        else
        {
            if (distance <= enemy.atkRange)
            {
                isDAttacking = true;
                DistanceAttack();
            }
            else
            {
                isDAttacking = false;
            }
        }
        attackDelay = enemy.atkDelay;
    }
    #endregion
    #endregion

    #region Enemy Attacking(�� ����) �ʵ�
    //���� ����
    private void MeleeAttack()
    {
        Collider2D hit = Physics2D.OverlapBox(enemy.EAttackRangePos.transform.position, OverlapBoxSize, enemy.EweaponAttackPosParent.transform.rotation.eulerAngles.z, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            enemyAnimator.SetTrigger("isAttack");
            enemyWeaponAnimator.SetTrigger("isAttack");
            GameManager.Instance.HP -= enemy.atkDmg;
        }
    }    

    //���Ÿ� ����
    private void DistanceAttack()
    {
        enemyAnimator.SetTrigger("isAttack");
        enemyWeaponAnimator.SetTrigger("isAttack");
        GameObject bullet = Instantiate(testprefabBullet, enemy.EweaponAttackPos.transform.position, enemy.EweaponAttackPos.transform.rotation);
        bullet.name = gameObject.name + "_" + enemy.atkDmg;
        Rigidbody2D rb = bullet.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = enemy.EweaponAttackPos.transform.right * enemy.atkSpeed;
    }

    //�ݶ��̴��� Scene�� ���̱� ���� �Լ�
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(EAttackRangePos.transform.position, OverlapBoxSize);
    }
    #endregion

}
