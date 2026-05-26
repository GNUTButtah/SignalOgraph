using UnityEngine;

public class KeyColourReset : MonoBehaviour
{
    GameObject[] keys;

    public Color baseGameColor;

    private void Awake()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        ResetColours(baseGameColor);
    }

    public void ResetColours(Color color)
    {
        foreach (var key in keys)
        {
            key.GetComponent<KeyPers>().GetComponent<KeyPers>().generalGameColor = color;
            key.GetComponent<KeyPers>().GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.JustPlaying);
        }
    }
}
