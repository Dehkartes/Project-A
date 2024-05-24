using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClickSound : MonoBehaviour
{
    [SerializeField] private AudioSource ButtonClickSound;

    public void PlaySound()
    {
        ButtonClickSound.Play();
    }

    public void ClickEffect()
    {
        TextMeshProUGUI ChildText = GetComponentInChildren<TextMeshProUGUI>();

        ChildText.fontSize = 96;
    }
    
    public void ExitClick()
    {
        TextMeshProUGUI ChildText = GetComponentInChildren<TextMeshProUGUI>();

        ChildText.fontSize = 64;
    }
}
