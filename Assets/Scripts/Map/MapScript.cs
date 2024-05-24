using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("EliteWeaponCheck") || col.CompareTag("EliteWeapon")
            || col.CompareTag("PlayerWeapon") || col.CompareTag("EnemyWeapon"))
        {
            Destroy(col.gameObject);
        }
    }    
}
