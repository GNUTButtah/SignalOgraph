using UnityEngine;

public class locaterPers : MonoBehaviour
{
    DecoderHandler dHandler;
    GameObject parentKey;
    private void Awake()
    {
        parentKey = gameObject.transform.parent.gameObject;
        dHandler = GameObject.FindGameObjectWithTag("DHandler").GetComponent<DecoderHandler>();


        dHandler.CheckForSafety(parentKey.GetComponent<KeyPers>().positionX, parentKey.GetComponent<KeyPers>().positionY);


    }

}
