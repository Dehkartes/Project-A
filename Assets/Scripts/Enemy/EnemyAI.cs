using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum EnemyStage { Stage1 = 1, Stage2, Stage3 }
public enum EnemyType { Stage1_D_1, Stage1_D_2, Stage1_M_1, Stage1_M_2,
                        Stage2_Mush, Stage2_Fairy, Stage2_Rock, Stage2_Swallo, Stage2_Squirrel,
                        Stage3_D_1, Stage3_D_2, Stage3_M_1, Stage3_M_2, Stage3_M_3}

public class EnemyAI : MonoBehaviour
{
    public EnemyType type;
    public EnemyStage stage;

    //OnDrawgizmo 때문에 사용 나중에 제거
    public GameObject EAttackRangePos;

    //배열로 바꾸기
    public GameObject testprefabBullet;
    //Enemy 스크립트
    Enemy enemy;
 
    //Enemy 스크립트 애니메이터
    Animator enemyAnimator;
    //EnemyWeaponAnim 스크립트 애니메이터
    public Animator enemyWeaponAnimator;

    //melee 공격을 위한 공격 범위
    public Vector2 OverlapBoxSize;

    //공격 쿨타임
    float attackDelay;

    //추후 수정
    //float RayMaxDistance = 1f;

    //true면 근접 false면 원거리 
    private bool enemyType;
    //1Stage 적에게만 적용되는 불 값
    private bool is1Stage;
    //적 피격시 애니메이션 적용 중인지 아닌지
    public bool attackedMove;
    //원거리 적 공격시 멈추기위해 사용하는 변수
    private bool isDAttacking;

    //patrol을 위한 변수
    private List<int> originalList;
    private int patrolPattern;
    private int checkCoroutineIdx;
    //patrol 코루틴 내부 확인을 위한 변수
    private bool isPatrol;
    //patrol을 진행중인지 아닌지 확인을 위한 변수
    private bool isPatrolling;
    private bool checkCoroutine;

    private void ClassificationEnemy()
    {
        //1스테이지
        if ((int)stage == 1)
        {
            switch (type)
            {
                case EnemyType.Stage1_D_1:
                    enemyType = false;
                    is1Stage = true;
                    //1.hp 2.데미지 3.공격딜레이 4.공격속도(총알) 5.이동속도 6.공격범위 7.인식범위
                    enemy.SetEnemyStatus(30, 7, 2f, 2.5f, 0.5f, 3f, 4f);
                    break;
                case EnemyType.Stage1_D_2:
                    enemyType = false;
                    is1Stage = true;
                    enemy.SetEnemyStatus(50, 10, 2f, 2.5f, 0.5f, 3f, 4f);
                    break;
                case EnemyType.Stage1_M_1:
                    enemyType = true;
                    is1Stage = true;
                    enemy.SetEnemyStatus(50, 5, 1f, 0, 2.3f, 0.6f, 1f);
                    //근접일때만 두개 붙음
                    OverlapBoxSize = new Vector2(0.8f, 0.8f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.4f, 0f);
                    break;
                case EnemyType.Stage1_M_2:
                    enemyType = true;
                    is1Stage = true;
                    enemy.SetEnemyStatus(50, 8, 1f, 0, 2.3f, 0.6f, 1f);
                    OverlapBoxSize = new Vector2(0.25f, 0.4f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.46f, 0f);
                    break;
                default:
                    Debug.Log("일반적 생성 오류");
                    break;
            }
        }
        //2스테이지
        else if ((int)stage == 2)
        {
            switch (type)
            {
                case EnemyType.Stage2_Mush:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(100, 10, 2, 0f, 0.5f, 0.7f, 2f);
                    OverlapBoxSize = new Vector2(0.25f, 0.4f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.46f, 0f);
                    break;
                case EnemyType.Stage2_Fairy:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(80, 10, 1f, 0f, 1.6f, 0.5f, 2f);
                    OverlapBoxSize = new Vector2(0.25f, 0.4f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.46f, 0f);
                    break;
                case EnemyType.Stage2_Rock:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(100, 12, 2f, 0f, 0.5f, 0.5f, 2f);
                    OverlapBoxSize = new Vector2(0.25f, 0.4f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.46f, 0f);
                    break;
                case EnemyType.Stage2_Swallo:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(150, 15, 1.5f, 0f, 1.5f, 0.8f, 2f);
                    OverlapBoxSize = new Vector2(0.25f, 0.4f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.46f, 0f);
                    break;
                case EnemyType.Stage2_Squirrel:
                    is1Stage = false;
                    enemyType = false;
                    enemy.SetEnemyStatus(70, 8, 1f, 2.5f, 0.5f, 3f, 4f);
                    break;
                default:
                    Debug.Log("일반적 생성 오류");
                    break;
            }
        }
        //3스테이지
        else if ((int)stage == 3)
        {
            switch (type)
            {
                case EnemyType.Stage3_D_1:
                    is1Stage = false;
                    enemyType = false;
                    enemy.SetEnemyStatus(100, 18, 1f, 2.5f, 1f, 3f, 4f);
                    break;
                case EnemyType.Stage3_D_2:
                    is1Stage = false;
                    enemyType = false;
                    enemy.SetEnemyStatus(100, 20, 1f, 2.5f, 1f, 3f, 4f);
                    break;
                case EnemyType.Stage3_M_1:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(150, 20, 1f, 0f, 1f, 1f, 2f);
                    OverlapBoxSize = new Vector2(0.2f, 0.2f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.4f, 0f);
                    break;
                case EnemyType.Stage3_M_2:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(150, 22, 1.5f, 0f, 1.3f, 0.7f, 2f);
                    OverlapBoxSize = new Vector2(0.2f, 0.2f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.4f, 0f);
                    break;
                case EnemyType.Stage3_M_3:
                    is1Stage = false;
                    enemyType = true;
                    enemy.SetEnemyStatus(200, 24, 2f, 0f, 0.6f, 0.8f, 2f);
                    OverlapBoxSize = new Vector2(0.2f, 0.2f);
                    enemy.EAttackRangePos.transform.localPosition = new Vector2(0.4f, 0f);
                    break;
                default:
                    Debug.Log("일반적 생성 오류");
                    break;
            }
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

        is1Stage = false;

        ClassificationEnemy();
    }

    void Update()
    {
        AI();
    }

    #region 적 AI
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
                    //Patrol 코루틴을 종료시키기 위한 Stop
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

    #region Patrol(순찰) AI
    private void NotFoundTarget()
    {
        //미완성
        //레이캐스트 추가
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
                //다시 순찰을 시작할때 무조건 0부터
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

    #region 이동(플레이어에게) AI
    private void MoveToTarget()
    {
        Vector2 direction = enemy.targetPosition.transform.position - transform.position;
        //////////////////////////////////////////////완성 하기 (미완성) 
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
        if (enemy.targetPosition.transform.position.x - transform.position.x < 0) // 타겟이 왼쪽에 있을 때
        {
            enemy.EnemySR.flipX = false;
            enemy.EweaponAttakPosSR.flipY = true;
        }
        else // 타겟이 오른쪽에 있을 때
        {
            enemy.EnemySR.flipX = true;
            enemy.EweaponAttakPosSR.flipY = false;
        }
    }
    #endregion

    #region 공격(플레이어에게) AI
    private void AttackTarget(float distance)
    {
        if (enemyType == true)
        {
            if (distance <= enemy.atkRange)
            {
                isDAttacking = true;
                MeleeAttack();
            }
            else
            {
                isDAttacking = false;
            }
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

    #region Enemy Attacking(적 공격) 필드
    //근접 공격
    private void MeleeAttack()
    {
        enemy.rb.velocity = Vector3.zero;

        //Collider2D hit = Physics2D.OverlapBox(transform.position, OverlapBoxSize, 0f, LayerMask.GetMask("Player"));
        //transform.position, OverlapBoxSize, enemy.EweaponAttackPosParent.transform.rotation.eulerAngles.z, LayerMask.GetMask("Player"));
        //if (hit != null)
        //{
        //    isDAttacking = true;
        //    if (is1Stage)
        //    {
        //        enemyAnimator.SetTrigger("isAttack");
        //        enemyWeaponAnimator.SetTrigger("isAttack");
        //        GameManager.Instance.PlayerHit(enemy.atkDmg);
        //    }
        //    else
        //    {
        //        enemyAnimator.SetTrigger("isAttack");
        //        GameManager.Instance.PlayerHit(enemy.atkDmg);
        //    }

        //}
        //else
        //{
        //    isDAttacking = false;
        //}
        if (is1Stage)
        {
            enemyAnimator.SetTrigger("isAttack");
            enemyWeaponAnimator.SetTrigger("isAttack");
            GameManager.Instance.PlayerHit(enemy.atkDmg);
        }
        else
        {
            enemyAnimator.SetTrigger("isAttack");
            GameManager.Instance.PlayerHit(enemy.atkDmg);
        }
    }    

    //원거리 공격
    private void DistanceAttack()
    {
        enemyAnimator.SetTrigger("isAttack");
        if (is1Stage)
        {
            enemyWeaponAnimator.SetTrigger("isAttack");
        }
        GameObject bullet = Instantiate(testprefabBullet, enemy.EweaponAttackPos.transform.position, enemy.EweaponAttackPos.transform.rotation);
        bullet.name = enemy.atkDmg.ToString();
        Rigidbody2D rb = bullet.gameObject.GetComponent<Rigidbody2D>();
        rb.velocity = enemy.EweaponAttackPos.transform.right * enemy.atkSpeed;
    }

    //콜라이더가 Scene에 보이기 위한 함수
    private void OnDrawGizmos()
    {
        Gizmos.color = UnityEngine.Color.red;
        Gizmos.DrawWireCube(transform.position, OverlapBoxSize);
    }
    #endregion

}
