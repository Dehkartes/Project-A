using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBoss_Lightning : MonoBehaviour
{
    private GameObject CloudBoss;
    private Stage3CloudBossAI CloudBossScript;
    private Animator anim;
    public float rotateSpeed;
    private bool isRotate;

    private void Start()
    {
        CloudBoss = GameObject.FindWithTag("Boss3_Cloud");
        CloudBossScript = CloudBoss.GetComponent<Stage3CloudBossAI>();
        anim = this.GetComponent<Animator>();
        isRotate = false;
        StartCoroutine(Lightning_Start());
    }

    private void Update()
    {
        if (isRotate)
        {
            transform.Rotate(new Vector3(0,0, rotateSpeed * Time.deltaTime));
        }
    }

    IEnumerator Lightning_Start()
    {
        yield return new WaitForSeconds(0.333f);
        anim.SetBool("Lightning", true);
        isRotate = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.PlayerHit((int)CloudBossScript.CloudBossPattern1Damage);
        }
    }
}
