using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WE : MonoBehaviour, ISkill
{
    public int WE_SkillDamage;
    public float WE_SkillSpeed;

    public int skillDamage { get; set; }
    private string skillName;
    BoxCollider2D WEcol;
    Animator anim;

    //WE_2 필드
    private List<GameObject> enemyCheck;
    public GameObject WE_2_root;

    void Start()
    {
        enemyCheck = new List<GameObject>();
        skillDamage = WE_SkillDamage;
        skillName = gameObject.name.Split("(")[0];
        if (skillName == "WE_3")
        {
            anim = GetComponent<Animator>();
            WEcol = GetComponent<BoxCollider2D>();
            WEcol.enabled = false;
            StartCoroutine(UseSkill_WE_3());
        }
        else if (skillName == "WE_2")
        {
            anim = GetComponent<Animator>();
            WEcol = GetComponent<BoxCollider2D>();
            WEcol.enabled = false;
            StartCoroutine(UseSkill_WE_2());
        }
    }
    #region WE_1
    public void WE_1Destoy()
    {
        Destroy(gameObject);
    }
    #endregion

    #region WE_2
    IEnumerator UseSkill_WE_2()
    {
        SpriteRenderer we2SR = GetComponent<SpriteRenderer>(); 
        yield return new WaitForSeconds(0.1f);
        WEcol.enabled = true;
        yield return new WaitForSeconds(0.4f);
        WEcol.enabled = false;
        Destroy(gameObject, 6f);
        yield return new WaitForSeconds(0.33f);
        Destroy(anim);
        we2SR.sprite = null;
    }

    IEnumerator WE_2_Root(GameObject enemy)
    {
        GameObject we2_root = Instantiate(WE_2_root, enemy.transform.position, Quaternion.identity);
        SpriteRenderer we2SR = we2_root.GetComponent<SpriteRenderer>();
        we2SR.sortingOrder = 2;
        Destroy(we2_root, 3f);
        while (we2_root)
        {
            if (we2_root == null)
            {
                break;
            }
            if (enemy == null)
            {
                break;
            }
            else
            {
                enemy.transform.position = we2_root.transform.position;
            }
            yield return null;
        }
    }
    #endregion

    #region WE_3
    IEnumerator UseSkill_WE_3()
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(UseSkill_WE_3_C(0.3f));
        yield return new WaitForSeconds(10f);
        anim.SetTrigger("End");
        yield return new WaitForSeconds(0.4f);
        Destroy(gameObject);
    }

    IEnumerator UseSkill_WE_3_C(float time)
    {
        WEcol.enabled = true;
        //콜라이더 껏다 키기
        while (gameObject)
        {
            if (gameObject == null)
            {
                break;
            }
            yield return new WaitForSeconds(time);
            WEcol.enabled = false;
            yield return new WaitForSeconds(time);
            WEcol.enabled = true;
            yield return null;
        }
    }
    #endregion

    #region 충돌 처리
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (skillName == "WE_3")
        {
            if (col.CompareTag("Player"))
            {
                GameManager.Instance.HP += 10;
            }
        }
        else if (skillName == "WE_2")
        {
            bool isAddEnemy = false;
            if (col.CompareTag("Enemy") || col.CompareTag("EliteMonster"))
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
                    StartCoroutine(WE_2_Root(col.gameObject));
                }
                else
                {
                    return;
                }
            }
        }  
    }
    #endregion
}
