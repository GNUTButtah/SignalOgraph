using UnityEngine;

public class Clouds : MonoBehaviour
{
    [SerializeField] GameObject CloudObject;

    [SerializeField] float startCloudXPos = -14.5f;
    [SerializeField] float endCloudXPos = 14.5f;

    [SerializeField] float cameraLeftPosLimit = -21.5f;
    [SerializeField] float cameraRightPosLimit = 13.5f;

    //-21,5 13.5
    // Update is called once per frame
    void Update()
    {
        float interpolatedCameraPos;
        interpolatedCameraPos = Mathf.InverseLerp(cameraLeftPosLimit, cameraRightPosLimit, (gameObject.transform.position.x));
        CloudObject.transform.position = new Vector3(Mathf.Lerp(startCloudXPos, endCloudXPos, interpolatedCameraPos), CloudObject.transform.position.y, CloudObject.transform.position.z);
        

        Debug.Log("The Interpolated Camera Value is: " + interpolatedCameraPos);
        Debug.Log("The Cloud's X Position is: " + CloudObject.transform.position.x);
    }
}
