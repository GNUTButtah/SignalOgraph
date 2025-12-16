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
    [SerializeField] GameObject animal;
    [SerializeField] GameObject[] keys;

    [SerializeField] TextMeshProUGUI caughtTMP;
    [SerializeField] TextMeshProUGUI missedTMP;
    public int caught;
    public int missed;

    bool gameActive;

    private void Update()
    {
        caughtTMP.text = "Caught: " + caught + " :)";
        missedTMP.text = "Missed: " + missed + " :(";
    }

    private void Start()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");
    }

    public void StartTheGame()
    {
        gameActive = true;
        StartCoroutine(SpawnInterval());
    }
    public void EndTheGame()
    {
        gameActive = false;
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