using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class W3AttackScript : MonoBehaviour
{
    Animator anim;


    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void EndW3Attack()
    {
        Destroy(gameObject);
    } 

    private void Update()
    {
    }

    //private void W3AttackStart()
    //{
    //    if (anim.GetCurrentAnimatorStateInfo(0).IsName("W3EliteAttak"))
    //    {
    //        Destroy(gameObject);
    //    }        
    //}
}
