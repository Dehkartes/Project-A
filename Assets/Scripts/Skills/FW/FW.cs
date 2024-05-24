using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FW : MonoBehaviour, ISkill
{
    public int FW_SkillDamage;
    public float FW_SkillSpeed;

    public int skillDamage { get; set; }
    private string skillName;
    private Rigidbody2D rb;
    private BoxCollider2D FWCol;

    private Coroutine co;

    void Start()
    {
        skillDamage = FW_SkillDamage;
        rb = GetComponent<Rigidbody2D>();
        skillName = gameObject.name.Split("(")[0];
        if(skillName == "FW_1")
        {
            FWCol = gameObject.GetComponent<BoxCollider2D>();
            StartCoroutine(UseSkill_FW_1(0.3f));
        }
        else if(skillName == "FW_2")
        {
            FWCol = gameObject.GetComponent<BoxCollider2D>();
        }
        else
        {
            FWCol = gameObject.GetComponent<BoxCollider2D>();
            StartCoroutine(UseSkill_FW_3());            
        }
    }

    #region FW_1 필드
    IEnumerator UseSkill_FW_1(float time)
    {
        FWCol.enabled = true;
        //콜라이더 껏다 키기
        while (gameObject)
        {
            if (gameObject == null)
            {
                break;
            }
            yield return new WaitForSeconds(time);
            FWCol.enabled = false;
            yield return new WaitForSeconds(time);
            FWCol.enabled = true;
            yield return null;
        }
    }
    #endregion

    #region FW_2 필드
    #endregion

    #region FW_3 필드
    IEnumerator UseSkill_FW_3()
    {
        rb.velocity = transform.right * FW_SkillSpeed;
        transform.rotation = Quaternion.identity;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    IEnumerator UseSkill_FW_3_C(GameObject col)
    {
        while (gameObject)
        {
            if (gameObject == null || col == null)
            {
                rb.velocity = Vector3.zero;
                break;
            }
            transform.position = col.transform.position;
            yield return new WaitForSeconds(0.1f);
        }
    }
    #endregion

    #region 충돌 처리 필드
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (skillName == "FW_3")
        {
            if (col.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
            else if(col.CompareTag("Boss1") || col.CompareTag("Boss2")
                || col.CompareTag("Enemy") || col.CompareTag("EliteMonster"))
            {
                if(co == null)
                {
                    co = StartCoroutine(UseSkill_FW_3_C(col.gameObject));
                    StartCoroutine(UseSkill_FW_1(0.2f));
                }
            }
        }
    }
    #endregion
}
