using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W_1 : MonoBehaviour, ISkill
{
    public int W_1_SkillDamage;
    public float W_1_SkillSpeed;

    private Rigidbody2D rb;

    //w6
    private bool isW6Done;
    List<GameObject> enemyCheck;
    private BoxCollider2D W6Col;
    //
    public int skillDamage { get; set; }
    private string skillName;
    private void Start()
    {
        skillDamage = W_1_SkillDamage;
        isW6Done = false;
        enemyCheck = new List<GameObject>();
        rb = GetComponent<Rigidbody2D>();
        skillName = gameObject.name.Split("(")[0];
        if (skillName == "W_6")
        {
            W6Col = gameObject.GetComponent<BoxCollider2D>();
            W6Col.enabled = false;
            StartCoroutine(UseSkill_6());
        }
        else
        {
            UseSkill_1_2();
        }

    }

    #region W_1, W_2 관리 필드
    //스킬 W_1, W_2 관리
    private void UseSkill_1_2()
    {
        rb.velocity = transform.right * W_1_SkillSpeed;
    }
    #endregion

    #region W_6 관리 필드

    IEnumerator UseSkill_6()
    {
        isW6Done = true;
        StartCoroutine(Set_UseSkill_6());
        //5초동안 돌아감
        yield return new WaitForSeconds(5f);
        isW6Done = false;
        enemyCheck.Clear();
    }
    IEnumerator Set_UseSkill_6()
    {
        yield return new WaitForSeconds(0.2f);
        W6Col.enabled = true;
        //콜라이더 껏다 키기
        while (isW6Done)
        {
            if (isW6Done == false)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
            W6Col.enabled = false;
            yield return new WaitForSeconds(0.5f);
            W6Col.enabled = true;
            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (skillName == "W_6")
        {
            bool isAddEnemy = false;
            if (col.CompareTag("Enemy"))
            {
                for (int index = 0; index < enemyCheck.Count; index++)
                {
                    if (col.gameObject == enemyCheck[index])
                    {
                        isAddEnemy = true;
                        break;
                    }
                }
                if (isAddEnemy == false)
                {
                    enemyCheck.Add(col.gameObject);
                    StartCoroutine(W_6_PullEnemy(col.gameObject));
                }
                else
                {
                    return;
                }
            }
        }
        else
        {
            if (col.CompareTag("Wall") || col.CompareTag("Boss1") || col.CompareTag("Boss2")
                    || col.CompareTag("Enemy") || col.CompareTag("EliteMonster") || col.CompareTag("Boss_Last"))
            {
                Destroy(gameObject);
            }
        }
    }
    IEnumerator W_6_PullEnemy(GameObject enemy)
    {
        //빨아들이기
        while (isW6Done)
        {
            if (isW6Done == false)
            {
                break;
            }
            if (enemy == null)
            {
                break;
            }
            else
            {
                enemy.transform.position = Vector2.Lerp(enemy.transform.position, transform.position, Time.deltaTime);
            }
            yield return null;
        }
    }
    #endregion
}
