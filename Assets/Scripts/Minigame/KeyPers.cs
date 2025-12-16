using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;



public class KeyPers : MonoBehaviour
{
    public string idKeyCode;
    public Color baseColor;
    public Color pressedColor;
    public Color animalColor;

    public int gameID;
    public float timerTime;

    private AnimalSpawner aSpawn;


    bool animalPresent;

    private void Start()
    {
        aSpawn = GameObject.FindGameObjectWithTag("AnimalSpawner").GetComponent<AnimalSpawner>();
        aSpawn.OnAnimalSpawned += AnimalAppeared;

        if (gameID == 0)
        {
            baseColor = Color.white;
            pressedColor = Color.red;
            animalColor = Color.yellow;

            timerTime = 3f;
        }

    }
    public void AnimalAppeared(object sender, EventArgs e)
    {
        if (transform.childCount > 0 &&
            transform.GetChild(0).CompareTag("Animal"))
        {
            animalPresent = true;
            recolourEverything(2);
            StartCoroutine(Deleter(true));
        }
    }

    private void Update()
    {
        if (animalPresent &&
            Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), idKeyCode)))
        {
            recolourEverything(1);
            StartCoroutine(Deleter(false));
        }
    }

    private IEnumerator Deleter(bool despawned)
    {
        if (despawned)
        {
            yield return new WaitForSeconds(timerTime);
            if (transform.childCount > 0)
            {
                Destroy(transform.GetChild(0).gameObject);
                aSpawn.missed++;
            }
            
        }
        else
        {
            Destroy(transform.GetChild(0).gameObject);
            aSpawn.caught++;
        }
        recolourEverything(3);
        animalPresent = false;
        StopAllCoroutines();
    }

    private void recolourEverything(int whatStep)
    {
        if (whatStep == 1) // Nachdem Geklickt wurde und das Tier fotografiert wird.
        {
            gameObject.GetComponent<SpriteRenderer>().color = pressedColor;

            //Der Code vom RGB zeug :P
        }
        if (whatStep == 2) // Wenn das Tier präsent ist
        {
            gameObject.GetComponent<SpriteRenderer>().color = animalColor;
        }
        if (whatStep == 3) // Wenn das Tier wieder weg ist
        {
            gameObject.GetComponent<SpriteRenderer>().color = baseColor;
        }
    }
}