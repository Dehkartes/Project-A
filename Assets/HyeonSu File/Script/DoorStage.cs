using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum DoorType { Wind, Rain, Cloud }
public class DoorStage : MonoBehaviour
{
    public DoorType Dt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (Dt)
        {
            case DoorType.Wind:
                SceneManager.LoadScene("WindStage");
                break;
            case DoorType.Cloud:
                SceneManager.LoadScene("CloudStage");
                break;
            case DoorType.Rain:
                SceneManager.LoadScene("RainStage");
                break;
            default:
                break;
        }
    }
}

