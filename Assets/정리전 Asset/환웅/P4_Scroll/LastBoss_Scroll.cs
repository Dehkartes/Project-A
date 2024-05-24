using Microsoft.Cci;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LastBoss_Scroll : MonoBehaviour
{
    private Animator anim;
    private GameObject parent;
    private BoxCollider2D bx;
    private int hitCount;
    private bool PlayerEnter = false;
    private string oriName; 


    void Start()
    {
        anim = this.GetComponent<Animator>();
        bx = this.GetComponent<BoxCollider2D>();
        hitCount = 0;
        oriName = gameObject.name.Split("(")[0];
        bx.enabled = false;
        StartCoroutine(SetScroll());
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayerEnter && GameManager.Instance.DestroyScroll)
        {
            parent.GetComponent<LastStageBossAI>().NotifyChildtoParent(oriName);
            StartCoroutine(DestroyScroll());
        }
    }

    IEnumerator SetScroll()
    {
        yield return new WaitForSeconds(0.5f);
        bx.enabled = true;
        parent = transform.parent.gameObject;
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Loop", true);
    }

    IEnumerator DestroyScroll()
    {
        anim.SetTrigger("Destroy");
        GameManager.Instance.DestroyScroll = false;
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerEnter = true;
            GameManager.Instance.PlayerInScroll = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            PlayerEnter = false;
            GameManager.Instance.PlayerInScroll = false;
        }
    }
}
