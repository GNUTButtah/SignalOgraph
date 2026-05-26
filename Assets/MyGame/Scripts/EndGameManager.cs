using UnityEngine;
using UnityEngine.SceneManagement;
public class EndGameManager : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
    public void QuitGame()
    { 
        Application.Quit();
    }
}
