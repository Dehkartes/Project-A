using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public GameObject[] ItemPrefabs;
    private Animator itemBoxAnim;
    private BoxCollider2D itemBoxCol;

    private bool isPlayerEnter = false;
    private Coroutine co = null;

    void Start()
    {
        itemBoxAnim = GetComponent<Animator>();
        itemBoxCol = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        ItemBoxGetInput();
    }

    private void ItemBoxGetInput()
    {
        //아이템 습득(G키)
        if (isPlayerEnter == true && ITEMMANAGER.Instance.isOpenitemBox == true && co == null)
        {
            co = StartCoroutine(SpawnItem());
        }
    }

    IEnumerator SpawnItem()
    {
        ITEMMANAGER.Instance.isOpenitemBox = false;
        int index = Random.Range(0, ItemPrefabs.Length);
        itemBoxCol.enabled = false;
        itemBoxAnim.SetTrigger("isOpen");
        yield return new WaitForSeconds(0.5f);
        GameObject SpawnItem = Instantiate(ItemPrefabs[index], new Vector2(transform.position.x, transform.position.y + 0.3f), Quaternion.Euler(0, 0, 0));
        SpawnItem.name = SpawnItem.name.Split("(")[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ITEMMANAGER.Instance.isPlayerOnItemBox = true;
            isPlayerEnter = true;
            ITEMMANAGER.Instance.ShowItemBoxUI(gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ITEMMANAGER.Instance.isPlayerOnItemBox = false;
            isPlayerEnter = false;
            ITEMMANAGER.Instance.CloseStateWindow();
        }
    }
}
