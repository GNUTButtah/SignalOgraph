using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public Player player;

    public bool playerIn;

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
           && Input.GetKeyDown(KeyCode.E))
        {
            SceneManager.LoadScene(1);
        }
    }
}
