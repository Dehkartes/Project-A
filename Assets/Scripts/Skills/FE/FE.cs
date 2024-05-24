using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FE : MonoBehaviour , ISkill
{
    public int FE_SkillDamage;
    public float FE_SkillSpeed;

    public int skillDamage { get; set; }
    private string skillName;
    BoxCollider2D FEcol;
    Rigidbody2D rb;
    Animator anim;

    void Start()
    {
        skillDamage = FE_SkillDamage;
        skillName = gameObject.name.Split("(")[0];
        if (skillName == "FE_2")
        {
            FEcol = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            //임시
            //StartCoroutine(FE2_Rotate(0));
            UseSkillFE_2();
        }
        else if (skillName == "FE_3")
        {
            FEcol = GetComponent<BoxCollider2D>();
            rb = GetComponent<Rigidbody2D>();
            FEcol.enabled = false;
            StartCoroutine(UseSkillFE_3());
        }
    }

    #region FE_1
    public void FE_1Destoy()
    {
        Destroy(gameObject);
    }
    #endregion

    #region FE_2
    private void UseSkillFE_2()
    {
        rb.velocity = transform.right * FE_SkillSpeed;
        int rindex = Random.Range(0, 2);
        StartCoroutine(FE2_Rotate(rindex));
    }

    IEnumerator FE2_Rotate(int index)
    {
        int rindex;
        if(index == 0)
        {
            rindex = 1;
        }
        else
        {
            rindex = -1;
        }
        while (gameObject)
        {
            if (gameObject == null)
            {
                break;
            }
            transform.Rotate(new Vector3(0, 0, 100 * rindex * Time.deltaTime));
            yield return null;
        }
    }
    #endregion

    #region FE_3
    IEnumerator UseSkillFE_3()
    {
        yield return new WaitForSeconds(0.4f);
        FEcol.enabled = true;
        yield return new WaitForSeconds(0.93f);
        Destroy(gameObject);
    }
    #endregion


    private void OnTriggerEnter2D(Collider2D col)
    {
        if (skillName == "FE_2")
        {
            if (col.CompareTag("Boss1") || col.CompareTag("Boss2")|| col.CompareTag("Enemy") || col.CompareTag("EliteMonster"))
            {
                //수정 필요 임시 보류
                //PlayerSkillManager.Instance.Seperate_FE2(col.transform.position, FE_SkillSpeed);
                Destroy(gameObject);
            }
            if(col.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}
