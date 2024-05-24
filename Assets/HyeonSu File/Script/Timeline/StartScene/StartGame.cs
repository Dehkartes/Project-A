using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class StartGame : MonoBehaviour
{
    [SerializeField] private AudioSource BGM;
    [SerializeField] private TimelineAsset Ta;
    [SerializeField] private PlayableDirector Pd;

    private void Start()
    {
        BGM.Stop();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            BGM.Play();
            Pd.Stop();
            gameObject.SetActive(false);
        }
    }
    public void SetGame()
    {
        BGM.Play();
        gameObject.SetActive(false);
    }
}
