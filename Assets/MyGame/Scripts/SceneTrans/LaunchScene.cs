using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class LaunchScene : MonoBehaviour
{
    public static LaunchScene lScene;
    public TMP_InputField inputField;
    [SerializeField]
    private TMP_InputField welcomeMessage;

    private string currentUserName;
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

        currentUserName = Environment.UserName.ToString();
        
        srgbPath = "C:\\Users\\" + currentUserName + "\\Documents\\WhirlwindFX\\Effects\\Signalograph_Effect.html";
        welcomeMessage.text = "Hi " + currentUserName + "! \n" + "Please make sure SignalRGB is launched and the Signalograph Effect is active.\\nThis folder should contain the html file (select, copy into your explorer and check!)";
        inputField.text = "C:\\Users\\" + currentUserName + "\\Documents\\WhirlwindFX\\Effects\\";
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
