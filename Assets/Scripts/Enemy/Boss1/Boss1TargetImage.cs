using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1TargetImage : MonoBehaviour
{
    Transform Player;
    public float speed = 5f;
    void Start()
    {
        Player = GameObject.Find("Player").transform;
    }
    void Update()
    {
        Vector2 dir = Player.position - transform.position;
        transform.Translate(dir * speed * Time.deltaTime);
    }
}
