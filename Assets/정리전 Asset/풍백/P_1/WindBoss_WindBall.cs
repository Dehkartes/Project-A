using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBoss_WindBall : MonoBehaviour
{
    private GameObject WindBoss;
    private Stage3WindBossAI WindBossScript;

    private void Start()
    {
        WindBoss = GameObject.FindWithTag("Boss3_Wind");
        WindBossScript = WindBoss.GetComponent<Stage3WindBossAI>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayerHit((int)WindBossScript.WindBossPattern1Damage);
        }
    }
}
