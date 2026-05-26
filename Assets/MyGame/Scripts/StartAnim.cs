using UnityEngine;
using System.Collections;

public class StartAnim : MonoBehaviour
{
    [SerializeField] SpriteRenderer PlayerRenderer;
    [SerializeField] Player PlayerScript;
    [SerializeField] BoxCollider2D PlayerColl;
    [SerializeField] Rigidbody2D PlayerRb;
    [SerializeField] GameObject fakePlayer;
    void Start()
    {
        PlayerRenderer.enabled = false;
        PlayerScript.enabled = false;
        PlayerColl.enabled = false;
        PlayerRb.simulated = false;
        StartCoroutine(EnablePlayerAfterDelay(5f));
    }

    private IEnumerator EnablePlayerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        fakePlayer.SetActive(false);
        PlayerRenderer.enabled = true;
        PlayerScript.enabled = true;
        PlayerColl.enabled = true;
        PlayerRb.simulated = true;
    }
}
