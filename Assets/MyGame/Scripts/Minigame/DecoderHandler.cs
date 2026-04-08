using System;
using UnityEngine;

public class DecoderHandler : MonoBehaviour
{
    [SerializeField] GameObject[] keys;
    GameObject[,] keysSorted = new GameObject[4, 15];

    [SerializeField] Vector2[] locatorPositions;
    [SerializeField] int currentStage;
    [SerializeField] GameObject locator;


    GameObject[] reachableKeysSave;

    public GameObject stopper;

    public enum personalRecolour
    {
        restart,

        locator,
    }

    public enum locatorStates
    {
        saveReachables,

        restart,

        secondLetter,
        thirdLetter,
        fourthLetter,

        moveNormal
    }

    [Header("TempSaveFields")]
    public bool[] column1, column2, column3, column4;
    bool[,] safetyGrid = new bool[4, 15];
    bool[,] safetyGridUnedited = new bool[4, 15];



    private void Awake()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");
        SortMyArrays();

        Instantiate(locator, keysSorted[(int)locatorPositions[0].x, (int)locatorPositions[0].y].transform);

        ColorItIn(personalRecolour.restart);
    }

    private void Update()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                MoveLocator(locatorStates.moveNormal, null, key);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                
                if (GameObject.FindGameObjectsWithTag("Locator") != null)
                {

                    foreach (var Locator in GameObject.FindGameObjectsWithTag("Locator"))
                    {
                        Destroy(Locator);
                    }
                }
                Instantiate(stopper, transform);
            }
        }
    }
    private void SortMyArrays()
    {
        foreach (var key in keys)
        {
            keysSorted[key.GetComponent<KeyPers>().positionX, key.GetComponent<KeyPers>().positionY] = key;
            
        }

        for (int i = 0; i < 4; i++)
        {
            if (i == 0)
            {
                for (int o = 0; o < column1.Length; o++)
                {
                    safetyGrid[i, o] = column1[o];
                }
            }
            if (i == 1)
            {
                for (int o = 0; o < column2.Length; o++)
                {
                    safetyGrid[i, o] = column2[o];
                }
            }
            if (i == 2)
            {
                for (int o = 0; o < column3.Length; o++)
                {
                    safetyGrid[i, o] = column3[o];
                }
            }
            if (i == 3)
            {
                for (int o = 0; o < column4.Length; o++)
                {
                    safetyGrid[i, o] = column4[o];
                }
            }
        }
        safetyGridUnedited = safetyGrid;

    }

    public void CheckForSafety(int posX, int posY)
    {
        if (safetyGrid[posX, posY] == true) // Wenn unser Player auf einem Sicheren Feld ist
        {
            GameObject[] reachableKeysGet = new GameObject[4];

            if (posY > 0)
                reachableKeysGet[0] = keysSorted[posX, posY - 1];

            if (posY < keysSorted.GetLength(1) - 1)
                reachableKeysGet[1] = keysSorted[posX, posY + 1];

            if (posX < keysSorted.GetLength(0) - 1)
                reachableKeysGet[2] = keysSorted[posX + 1, posY];

            if (posX > 0)
                reachableKeysGet[3] = keysSorted[posX - 1, posY];

            MoveLocator(locatorStates.saveReachables, reachableKeysGet, KeyCode.None);
            Debug.Log("Der Locator ist bei " + posX + " " + posY + " Und ist SCHON sicher");
            
        }
        else if (safetyGrid[posX, posY] != true)
        {
            Debug.Log("Der Locator ist bei " + posX + " " + posY + " Und ist NICHT sicher");
            MoveLocator(locatorStates.restart, null, KeyCode.None);
        }
    }
    private void MoveLocator(locatorStates whatToDoWithLocator, GameObject[] reachableKeysRecieve, KeyCode pressedKey)
    {
        if (whatToDoWithLocator == locatorStates.saveReachables)
        {
            reachableKeysSave = reachableKeysRecieve;
        }

        if (whatToDoWithLocator == locatorStates.restart)
        {
            if (GameObject.FindGameObjectsWithTag("Locator") != null)
            {
                foreach (var Locator in GameObject.FindGameObjectsWithTag("Locator"))
                {
                    Destroy(Locator);
                }
            }
            Instantiate(locator, keysSorted[(int)locatorPositions[currentStage].x, (int)locatorPositions[currentStage].y].transform);
            ColorItIn(personalRecolour.restart);
        }

        if (whatToDoWithLocator == locatorStates.moveNormal)
        {
            foreach (var reachable in reachableKeysSave)
            {

                if (reachableKeysSave == null) return;

                if (reachable != null &&
                    pressedKey.ToString() == reachable.GetComponent<KeyPers>().idKeyCode)
                {
                    if (GameObject.FindGameObjectsWithTag("Locator") != null)
                    {
                        foreach (var Locator in GameObject.FindGameObjectsWithTag("Locator"))
                        {
                            Destroy(Locator);
                        }
                    }
                    Instantiate(locator, reachable.transform);
                    reachable.GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.locatorPresent);
                    break;
                }
                else if (reachable != null &&
                    pressedKey.ToString() != reachable.GetComponent<KeyPers>().idKeyCode)
                {
                    MoveLocator(locatorStates.restart, null, KeyCode.None);
                }

            }
        }
    }

    private void ColorItIn(personalRecolour state)
    {
        switch (state)
        {
            case personalRecolour.restart:
                for (int x = 0; x < safetyGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < safetyGrid.GetLength(1); y++)
                    {
                        if (keysSorted[x, y] != null)
                        {
                            if (safetyGrid[x, y])
                            {
                                keysSorted[x, y].GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.locatorPath);
                            }
                            else
                            {
                                keysSorted[x, y].GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.languageBase);
                            }
                        }

                    }
                }
            keysSorted[(int)locatorPositions[0].x, (int)locatorPositions[0].y].GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.animalPressed);
            break;

        }
        
    }

}
