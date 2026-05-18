using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DecoderHandler : MonoBehaviour
{
    [SerializeField] GameObject[] keys;
    GameObject[,] keysSorted = new GameObject[4, 15];

    [SerializeField] Vector2[] locatorPositions;
    [SerializeField] int currentStage;
    [SerializeField] Vector2[] letterEnds;
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

        toLetterStart,

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

        ColorItIn(personalRecolour.restart, null);
    }

    private void Update()
    {
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

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                //Debug.Log(key);
                MoveLocator(locatorStates.moveNormal, null, key);
                return;
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

        Array.Copy(safetyGrid, safetyGridUnedited, safetyGrid.Length);

    }

    public void CheckForSafety(int posX, int posY)
    {
        if (safetyGrid[posX, posY] == true) // Wenn unser Player auf einem Sicheren Feld ist
        {
            if (posX == (int)letterEnds[currentStage].x && posY == (int)letterEnds[currentStage].y)
            {
                MoveLocator(locatorStates.toLetterStart, null, KeyCode.None);
            }


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
            
        }
        else if (safetyGrid[posX, posY] != true)
        {
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
            ColorItIn(personalRecolour.restart, null);
            Instantiate(locator, keysSorted[(int)locatorPositions[currentStage].x, (int)locatorPositions[currentStage].y].transform);
            
        }

        if (whatToDoWithLocator == locatorStates.moveNormal)
        {
            if (reachableKeysSave == null) return;

            bool matched = false;
            foreach (var reachable in reachableKeysSave)
            {
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
                    ColorItIn(personalRecolour.locator, reachable.GetComponent<KeyPers>());
                    Instantiate(locator, reachable.transform);
                    
                    matched = true;
                    break;
                }
            }
            if (!matched)
            {
                

                MoveLocator(locatorStates.restart, null, KeyCode.None);
                ColorItIn(personalRecolour.restart, null);

                return;
            }
        }

        if (whatToDoWithLocator == locatorStates.toLetterStart)
        {
            currentStage++;
            Instantiate(locator, keysSorted[(int)locatorPositions[currentStage].x, (int)locatorPositions[currentStage].y].transform);

        }

    }

    private void ColorItIn(personalRecolour state, KeyPers keyToRecolour)
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
                keysSorted[(int)locatorPositions[currentStage].x, (int)locatorPositions[currentStage].y].GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.locatorPresent);
                
                break;

                
            case personalRecolour.locator:
                 
                keyToRecolour.recolourEverything(KeyPers.RecolourState.locatorPresent);
                
                break;

        }
        
    }

}
