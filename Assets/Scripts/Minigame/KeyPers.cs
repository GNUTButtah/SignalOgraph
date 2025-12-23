using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static KeyPers;



public class KeyPers : MonoBehaviour
{
    public string idKeyCode;
    public Color baseColor;
    public Color pressedColor;
    public Color animalColor;

    public int gameID;
    public float timerTime;

    [SerializeField] AnimalSpawner aSpawn;
    [SerializeField]  DrawHandler dHandler;
    [SerializeField] SignalRGBManager sRGB;

    public enum RecolourState
    {
        simpleBase,

        animalPressed,
        animalPresent,
        animalReset,

        draw,

    }

    bool animalPresent;

    private void Awake()
    {
        aSpawn = GameObject.FindGameObjectWithTag("AnimalSpawner").GetComponent<AnimalSpawner>();
        aSpawn.OnAnimalSpawned += AnimalAppeared;

        dHandler = GameObject.FindGameObjectWithTag("DrawHandler").GetComponent<DrawHandler>();
        dHandler.OnWhoToDraw += DrawResponse;

        sRGB = GameObject.FindGameObjectWithTag("SRGB").GetComponent<SignalRGBManager>();
        


        if (gameID == 0)
        {
            baseColor = aSpawn.baseCol;
            pressedColor = aSpawn.pressedCol;
            animalColor = aSpawn.animalCol;

            sRGB.SetKeyColor(idKeyCode, baseColor);
            sRGB.Apply();

            timerTime = 3f;
        }

    }
    public void AnimalAppeared(object sender, EventArgs e)
    {
        if (transform.childCount > 0 &&
            transform.GetChild(0).CompareTag("Animal"))
        {
            animalPresent = true;
            recolourEverything(RecolourState.animalPresent);
            StartCoroutine(Deleter(true));
        }
    }

    private void Update()
    {
        if (animalPresent &&
            Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), idKeyCode)))
        {
            recolourEverything(RecolourState.animalPresent);
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
        recolourEverything(RecolourState.animalPressed);
        animalPresent = false;
        StopAllCoroutines();
    }

    private void DrawResponse(object sender, EventArgs e)
    {
        if (transform.childCount > 0)
        { 

            recolourEverything(RecolourState.draw);
            
            Destroy(transform.GetChild(0).gameObject);
        }
    }


    private void recolourEverything(RecolourState state)
    {
        switch (state)
        {
            case RecolourState.animalPressed:
                GetComponent<SpriteRenderer>().color = pressedColor;
                sRGB.SetKeyColor(idKeyCode, pressedColor);
                break;

            case RecolourState.animalPresent:
                GetComponent<SpriteRenderer>().color = animalColor;
                sRGB.SetKeyColor(idKeyCode, animalColor);
                break;

            case RecolourState.animalReset:
                GetComponent<SpriteRenderer>().color = baseColor;
                sRGB.SetKeyColor(idKeyCode, baseColor);
                break;

            case RecolourState.draw:
                GetComponent<SpriteRenderer>().color = dHandler.currentColor;
                sRGB.SetKeyColor(idKeyCode, dHandler.currentColor);
                break;
            case RecolourState.simpleBase:
                GetComponent<SpriteRenderer>().color = Color.white;
                sRGB.SetKeyColor(idKeyCode, Color.white);
                break;
        }

        sRGB.Apply();
    }

}