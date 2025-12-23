using UnityEngine;

public class DebugActivate : MonoBehaviour
{
    [SerializeField] bool inside;
    [SerializeField] GameObject keyboard;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inside = true;
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inside = false;
    }

    private void Update()
    {
        if (inside && Input.GetKeyDown(KeyCode.E))
        {
            if (keyboard.activeInHierarchy == false)
            {
                keyboard.SetActive(true);
            }
            else if (keyboard.activeInHierarchy == true)
            {
                keyboard.SetActive(false);
            }
        }
    }
}
