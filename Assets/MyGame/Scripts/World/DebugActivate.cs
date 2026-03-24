using UnityEngine;

public class DebugActivate : MonoBehaviour
{
    [SerializeField] bool inside;
    [SerializeField] bool kbDebug = false;
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
            if (kbDebug == false)
            {
                keyboard.transform.position = new Vector3(keyboard.transform.position.x, 1000f, keyboard.transform.position.z);
                kbDebug = true;
            }
            else if (kbDebug == true)
            {
                keyboard.transform.position = new Vector3(keyboard.transform.position.x, -2000f, keyboard.transform.position.z);
            }
        }
    }

}
