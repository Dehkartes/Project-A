using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PointType { Shop, Tutorial, Door }
public class VillagePoint : MonoBehaviour
{
    public PointType type;
    public GameObject Player;

    public Sprite ItemInteractionSprite;
    public Sprite InteractionSprite;
    public Sprite DoorInteractionSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            switch (type)
            {
                case PointType.Shop:
                    GameManager.Instance.ShopPointIn = true;
                    ITEMMANAGER.Instance.isEnterShopPoint = true;

                    Data.InteractionBtn.image.sprite = InteractionSprite;
                    break;
                case PointType.Tutorial:
                    GameManager.Instance.TutorialPointIn = true;
                    ITEMMANAGER.Instance.isEnterTutorialPoint = true;

                    Data.InteractionBtn.image.sprite = InteractionSprite;
                    break;
                case PointType.Door:
                    GameManager.Instance.DoorPointIn = true;
                    ITEMMANAGER.Instance.isEnterDoorPoint = true;

                    Data.InteractionBtn.image.sprite = DoorInteractionSprite;
                    break;
                default:
                    break;
            }      
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Data.InteractionBtn.image.sprite = ItemInteractionSprite;

            switch (type)
            {
                case PointType.Shop:
                    GameManager.Instance.ShopPointIn = false;
                    ITEMMANAGER.Instance.isEnterShopPoint = false;
                    break;
                case PointType.Tutorial:
                    GameManager.Instance.TutorialPointIn = false;
                    ITEMMANAGER.Instance.isEnterTutorialPoint = false;
                    break;
                case PointType.Door:
                    GameManager.Instance.DoorPointIn = false;
                    ITEMMANAGER.Instance.isEnterDoorPoint = false;
                    break;
                default:
                    break;
            }
        }
    }

    private void Update()
    {
        switch (type)
        {
            case PointType.Shop:
                //if(GameManager.Instance.ShopPointIn && Input.GetKey(KeyCode.G))
                if (GameManager.Instance.ShopPointIn && ITEMMANAGER.Instance.isOpenShopPoint)
                {
                    GameManager.Instance.ShopPanel.SetActive(true);
                    Data.JoyStickUi.gameObject.SetActive(false);
                }
                break;
            case PointType.Tutorial:
                //if (GameManager.Instance.TutorialPointIn && Input.GetKey(KeyCode.G))
                if (GameManager.Instance.TutorialPointIn && ITEMMANAGER.Instance.isOpenTutorialPoint)
                {
                    GameManager.Instance.TutorialPanel.SetActive(true);
                    Data.JoyStickUi.gameObject.SetActive(false);
                }
                break;
            case PointType.Door:
                //if(GameManager.Instance.DoorPointIn && Input.GetKey(KeyCode.G))
                if (GameManager.Instance.DoorPointIn && ITEMMANAGER.Instance.isOpenDoorPoint)
                {
                    GameObject Player = GameObject.Find("Player");
                    Player.transform.position = new Vector3(0, 0.5f, 0);
                }
                break;
            default:
                break;
        }
    }
}
