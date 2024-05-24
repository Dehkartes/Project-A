using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NpcType { Shop, Tutorial, Door, Bank }
public class NpcTalk : MonoBehaviour
{
    public NpcType tpye;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        switch (tpye)
        {
            case NpcType.Shop:
                GameManager.Instance.ShopNpcText.gameObject.transform.position
                = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0.4f));
                break;
            case NpcType.Tutorial:
                GameManager.Instance.TutoNpcText.gameObject.transform.position
                = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0.3f));
                break;
            case NpcType.Door:
                GameManager.Instance.DoorNpcText.gameObject.transform.position
                = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0.7f));
                break;
            case NpcType.Bank:
                GameManager.Instance.BankNpcText.gameObject.transform.position
                = Camera.main.WorldToScreenPoint(gameObject.transform.position + new Vector3(0, 0.5f));
                break;
            default:
                break;
        }

    }
}
