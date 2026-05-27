using UnityEngine;

public class KeyColourReset : MonoBehaviour
{
    GameObject[] keys;

    public Color baseGameColor;

    private void Awake()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");

        ResetColours(baseGameColor);

        DontDestroyOnLoad(gameObject);
    }

    public void ResetColours(Color color)
    {
        foreach (var key in keys)
        {
            key.GetComponent<KeyPers>().generalGameColor = color;
            key.GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.JustPlaying);
        }
    }

    private void OnApplicationQuit()
    {
        TurnEveryKeyOneState(KeyPers.RecolourState.notPlaying);
    }

    public void TurnEveryKeyOneState(KeyPers.RecolourState state)
    {
        foreach (var key in keys)
        {
            key.GetComponent<KeyPers>().recolourEverything(state);
        }
    }
}


