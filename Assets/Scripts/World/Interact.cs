using Unity.VisualScripting;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public GameObject minigame;
    public Player player;

    public bool playerIn;

    private void Start()
    {
        minigame.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") 
        {
            playerIn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            playerIn = false;
        }
    }
    private void Update()
    {
        if (playerIn
           && Input.GetKeyDown(KeyCode.E)
           && !minigame.activeInHierarchy)
        {
            minigame.SetActive(true);
            player.enabled = false;
        }
        else if (minigame.transform.childCount >= 1)
        {
            minigame.SetActive(false);
            Destroy(minigame.transform.GetChild(0).gameObject);
            player.enabled = true;


            MinigameComplete();

        }

    }

    private void MinigameComplete()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
    }

}
