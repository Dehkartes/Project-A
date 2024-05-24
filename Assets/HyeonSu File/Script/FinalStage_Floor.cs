using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoomType { Start, Boss }

public class FinalStage_Floor : MonoBehaviour
{
    public RoomType roomType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name.Equals("Player"))
        {
            switch (roomType)
            { 
                case RoomType.Start:
                    FinalStage.Instance.Roomname = (int)RoomType.Start;
                    break;
                case RoomType.Boss:
                    FinalStage.Instance.Roomname = (int)RoomType.Boss;
                    break;                     
            }

        }
    }
}
