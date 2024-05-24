using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapCam : MonoBehaviour
{
	public Transform player;

    private void Awake()
    {
		player = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
	{
		Vector3 newPosition = player.position;
		newPosition.z = transform.position.z;
		transform.position = newPosition;
	}
}
