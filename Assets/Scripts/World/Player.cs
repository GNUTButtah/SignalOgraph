using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float horizontalInput;

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        Vector3 move = new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0f, 0f);

        GetComponent<Rigidbody2D>().MovePosition(transform.position + move);
    }
}