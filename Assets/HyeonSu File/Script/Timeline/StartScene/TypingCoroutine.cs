using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TypingCoroutine : MonoBehaviour
{
    public TextMeshProUGUI m_TypingText;
    public string m_Message;
    public float m_Speed = 0.2f;

    [SerializeField] private string m_ExText;
    [SerializeField] private AudioSource TypingSound;
         

    // Start is called before the first frame update 
    void Start()
    {
        m_Message = m_ExText;

        StartCoroutine(Typing(m_TypingText, m_Message, m_Speed));
    }

    IEnumerator Typing(TextMeshProUGUI typingText, string message, float speed)
    {
        TypingSound.Play();

        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }

        yield return new WaitForSeconds(0.5f);
        TypingSound.Stop();
    }
}
