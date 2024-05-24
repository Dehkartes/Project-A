using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testStartButton : MonoBehaviour
{
    public GameObject gb;
    public void Start()
    {
        Debug.Log("¤²¤²¤²");
    }
    public void StartButton()
    {
        //SceneManager.LoadScene("SampleScene");
        gb.SetActive(true);
    }
}
