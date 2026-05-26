using UnityEngine;

public class KeyColourReset : MonoBehaviour
{
    GameObject[] keys;

    public Color baseGameColor;

    private void Awake()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        ResetColours();
    }

    public void ResetColours()
    {
        foreach (var key in keys)
        {
            key.GetComponent<KeyPers>().GetComponent<KeyPers>().generalGameColor = baseGameColor;
            key.GetComponent<KeyPers>().GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.JustPlaying);
        }
    }
}
