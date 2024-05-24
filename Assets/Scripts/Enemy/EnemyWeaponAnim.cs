using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponAnim : MonoBehaviour
{
    public bool isAttack;
    Animator enemyWeapon;

    private void Awake()
    {
        isAttack = false;

        enemyWeapon = GetComponent<Animator>();
    }

    public void WeaponAnimation()
    {
        enemyWeapon.SetTrigger("isAttack");
    }
}
