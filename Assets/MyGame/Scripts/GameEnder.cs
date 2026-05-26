using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class GameEnder : MonoBehaviour
{
    [SerializeField] Animator fadeToBlack;


    public void LetTheGameEnd()
    {
        fadeToBlack.SetTrigger("EndGame");
        StartCoroutine(EndGameAfterDelay(7f));
    }

    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(2);
    }
}

