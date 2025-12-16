using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
