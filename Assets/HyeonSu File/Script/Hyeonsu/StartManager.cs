using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public GameObject OptionPanel;

    // Update is called once per frame

    public void OnStart()
    {
        SceneManager.LoadScene(1);
    }

    public void OnOption()
    {
        OptionPanel.SetActive(true);
    }

    public void OnOptionClose()
    {
        OptionPanel.SetActive(false);
    }

    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
