using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchScene : MonoBehaviour
{
    public static LaunchScene lScene;
    public TMP_InputField inputField;

    public string srgbPath;

    private void Awake()
    {
        if (lScene == null)
        {
            lScene = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSRGBPath()
    {
        srgbPath = inputField.text;
        SceneManager.LoadScene(1);
    }
}
