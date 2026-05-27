using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimalSpawner : MonoBehaviour
{
    public event EventHandler OnAnimalSpawned;

    [SerializeField] float spawnTime;
    [SerializeField] int toCatch;
    [SerializeField] GameObject animal;
    [SerializeField] GameObject[] keys;
    [SerializeField] GameObject stopper;

    GameObject reseter;

    //[SerializeField] TextMeshProUGUI caughtTMP;
    //[SerializeField] TextMeshProUGUI missedTMP;
    public int caught;
    public int missed;

    public Color baseCol;
    public Color pressedCol;
    public Color animalCol;


    bool gameActive;

    private void Update()
    {
        if (caught >= toCatch)
        {
            Instantiate(stopper, transform);
            StopAllCoroutines();
            gameActive = false;

            reseter.GetComponent<KeyColourReset>().ResetColours(reseter.GetComponent<KeyColourReset>().baseGameColor);
        }
    }

    private void Start()
    {
        
        keys = GameObject.FindGameObjectsWithTag("Key");
        
        
    }

    private void OnEnable()
    {
        reseter = GameObject.FindGameObjectWithTag("KeyboardReseter");
        reseter.GetComponent<KeyColourReset>().TurnEveryKeyOneState(KeyPers.RecolourState.successBlack);
        gameActive = true;
        StartCoroutine(SpawnInterval());
    }


    private IEnumerator SpawnInterval()
    {
        while (gameActive)
        {
            yield return new WaitForSeconds(spawnTime);
            SpawnAnimal();
        }
    }

    private void SpawnAnimal()
    {
        List<GameObject> freeKeys = new List<GameObject>();
        foreach (var key in keys)
        {
            if (key.name == "K_LESS" || key.name == "K_FN" || key.name == "K_RAZER" || key.name == "K_LWINDOWS")
            {
                continue;
            }
            else if (key.transform.childCount == 0)
            {
                freeKeys.Add(key);
            }
        }
        GameObject selectedKey;
        selectedKey = freeKeys[Random.Range(0, freeKeys.Count)];

        Instantiate(animal, selectedKey.transform);
        OnAnimalSpawned?.Invoke(this, null);
    }

    
}