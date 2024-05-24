using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_1 : MonoBehaviour, ISkill
{
    public int E_1_SkillDamage;
    public float E_1_SkillSpeed;

    private Rigidbody2D rb;
    public int skillDamage { get; set; }
    private string skillName;
    void Start()
    {
        skillDamage = E_1_SkillDamage;
        rb = GetComponent<Rigidbody2D>();
        skillName = gameObject.name.Split("(")[0];
        if (skillName == "E_1" || skillName == "E_2")
        {
            UseSkill();
        }
    }

    private void UseSkill()
    {
        rb.velocity = transform.right * E_1_SkillSpeed;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (skillName == "E_1" || skillName == "E_2")
        {
            if (col.CompareTag("Wall") || col.CompareTag("Boss1") || col.CompareTag("Boss2")
                || col.CompareTag("Enemy") || col.CompareTag("EliteMonster") || col.CompareTag("Boss_Last"))
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (col.CompareTag("EliteWeaponCheck") || col.CompareTag("EliteWeapon") || col.CompareTag("EnemyWeapon"))
            {
                Destroy(col.gameObject);
            }
        }
    }
}
