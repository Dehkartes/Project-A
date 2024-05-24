using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class F_1 : MonoBehaviour, ISkill
{
    public int F_1_SkillDamage;
    public float F_1_SkillSpeed;



    private Rigidbody2D rb;

    private CapsuleCollider2D F5Col;
    private BoxCollider2D F6Col;
    public int skillDamage { get; set; }
    private string skillName;
    void Start()
    {
        skillDamage = F_1_SkillDamage;
        rb = GetComponent<Rigidbody2D>();
        skillName = gameObject.name.Split("(")[0];
        if (skillName == "F_5")
        {
            F5Col = gameObject.GetComponent<CapsuleCollider2D>();
            StartCoroutine(UseSkill_5());
        }
        else if (skillName == "F_6")
        {
            F6Col = gameObject.GetComponent<BoxCollider2D>();
            F6Col.enabled = false;
            StartCoroutine(UseSkill_6());
        }
        else
        {
            UseSkill();
        }
    }

    #region F_1, F_2 관리 필드
    private void UseSkill()
    {
        rb.velocity = transform.right * F_1_SkillSpeed;
    }
    #endregion

    #region F_5 관리 필드
    private bool isF5Done;
    IEnumerator UseSkill_5()
    {
        isF5Done = true;
        StartCoroutine(Set_UseSkill_5());
        //5초동안 돌아감
        yield return new WaitForSeconds(5f);
        isF5Done = false;
    }
    IEnumerator Set_UseSkill_5()
    {
        yield return new WaitForSeconds(0.2f);
        F5Col.enabled = true;
        //콜라이더 껏다 키기
        while (isF5Done)
        {
            if (gameObject)
            {
                if (isF5Done == false)
                {
                    break;
                }
                yield return new WaitForSeconds(0.5f);
                F5Col.enabled = false;
                yield return new WaitForSeconds(0.5f);
                F5Col.enabled = true;
                yield return null;
            }
            else
            {
                break;
            }
        }
    }
    #endregion

    #region F_6 관리 필드
    IEnumerator UseSkill_6()
    {
        yield return new WaitForSeconds(0.4f);
        F6Col.enabled = true;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (skillName == "F_1" || skillName == "F_2")
        {
            if (col.CompareTag("Wall") || col.CompareTag("Boss1") || col.CompareTag("Boss2")
                || col.CompareTag("Enemy") || col.CompareTag("EliteMonster") || col.CompareTag("Boss_Last"))
            {
                Destroy(gameObject);
            }
        }
    }
}
