using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEngine;



public class DecoderHandler : MonoBehaviour
{
    GameObject[] keys;
    GameObject[,] keysSorted;


    private void Awake()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        foreach (var key in keys)
        {
            keysSorted[key.GetComponent<KeyPers>().positionX, key.GetComponent<KeyPers>().positionY] = key;
        }

    }
}
