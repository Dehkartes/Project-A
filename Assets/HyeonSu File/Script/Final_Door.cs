using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Final_Door : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            FinalStage.Instance.Player.transform.position = new Vector3(0, -1, 0);
        }
    }
}
