using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CutScene_Fade : MonoBehaviour
{
    private Image self;

    private bool StartFade = false;
    private bool EndFade = false;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Image>();  
    }

    // Update is called once per frame
    void Update()
    {
        if (StartFade)
        {
            StartCoroutine(FadeIn());
        }

        if(EndFade)
        {
            StartCoroutine(FadeOut());
        }

        //if(self.color.a < 0.0f)
        //{
        //    Color color = self.color;
        //    color.a = 0;
        //    self.color = color;

        //    StopAllCoroutines();

        //    StartFade = false;
        //    EndFade = false;
        //}
    }

    IEnumerator FadeIn()
    {
        Color color = self.color;
        while(color.a < 1.0f)
        {
            color.a += Time.deltaTime;
            self.color = color;
            yield return null;
        }

        EndFade = false;
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1f);

        Color color = self.color;
        while(color.a > 0.0f)
        {
            color.a -= Time.deltaTime;
            self.color = color;
            yield return null;
        }
    }

    public void StartFading()
    {
        StartFade = true;
    }

    public void EndFading()
    {
        EndFade = true;
    }

    public void AllFadingEnd()
    {
        StartFade = false;
        EndFade = false;
        StopAllCoroutines();
    }
}
