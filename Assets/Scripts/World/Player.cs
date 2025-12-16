using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float horizontalInput;

    void Update()
    {
        // Eingabe holen (A/D oder Pfeiltasten)
        horizontalInput = Input.GetAxis("Horizontal");

        // Bewegung pro Frame berechnen
        Vector3 move = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0f, 0f);

        // Position verändern
        transform.position += move;
    }
}