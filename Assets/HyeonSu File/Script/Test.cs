using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                break;
            case 1:
                if (collision.name.Equals("Player"))
                {
                    SceneManager.LoadScene(2);
                }
                break;
            case 2:
                if(collision.name.Equals("Player"))
                {
                    SceneManager.LoadScene(3);
                }
                break;
            case 3:
                break;
            case 4:
                if(collision.name.Equals("Player"))
                {
                    SceneManager.LoadScene(7);
                }
                break;
            case 5:
                if (collision.name.Equals("Player"))
                {
                    SceneManager.LoadScene(7);
                }
                break;
            case 6:
                if(collision.name.Equals("Player"))
                {
                    SceneManager.LoadScene(7);
                }
                break;
            default:
                break;
        }
    }
}
