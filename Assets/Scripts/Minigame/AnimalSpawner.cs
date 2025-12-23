using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;

public class AnimalSpawner : MonoBehaviour
{
    public event EventHandler OnAnimalSpawned;

    [SerializeField] float spawnTime;
    [SerializeField] int toCatch;
    [SerializeField] GameObject animal;
    [SerializeField] GameObject[] keys;
    [SerializeField] GameObject stopper;

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
        }
    }

    private void Start()
    {
        gameObject.SetActive(false);
        keys = GameObject.FindGameObjectsWithTag("Key");
    }

    private void OnEnable()
    {
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
            if (key.transform.childCount == 0)
                freeKeys.Add(key);
        }
        GameObject selectedKey;
        selectedKey = freeKeys[Random.Range(0, freeKeys.Count)];

        Instantiate(animal, selectedKey.transform);
        OnAnimalSpawned?.Invoke(this, null);
    }

    
}