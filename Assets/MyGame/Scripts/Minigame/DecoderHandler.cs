using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class DecoderHandler : MonoBehaviour
{
    [SerializeField] GameObject[] keys;
    GameObject[,] keysSorted = new GameObject[5, 15];

    [SerializeField] Vector2[] locatorPositions;
    [SerializeField] int currentStage;
    [SerializeField] Vector2[] letterEnds;
    [SerializeField] GameObject locator;

    GameObject reseter;

    GameObject[] reachableKeysSave;

    public GameObject stopper;

    // Blockiert Eingaben, während der falsche Key aufleuchtet
    private bool isProcessingError = false;

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
    public bool[] column1, column2, column3, column4, column5;
    bool[,] safetyGrid = new bool[5, 15];
    bool[,] safetyGridUnedited = new bool[5, 15];

    private void Awake()
    {
        keys = GameObject.FindGameObjectsWithTag("Key");
        SortMyArrays();

        Instantiate(locator, keysSorted[(int)locatorPositions[0].x, (int)locatorPositions[0].y].transform);

        ColorItIn(personalRecolour.restart, null);

        reseter = GameObject.FindGameObjectWithTag("KeyboardReseter");
    }

    private void Update()
    {
        // Wenn gerade ein roter Key angezeigt wird, ignoriere alle weiteren Eingaben
        if (isProcessingError) return;

        if (currentStage > 2)
        {
            if (GameObject.FindGameObjectsWithTag("Locator") != null)
            {
                foreach (var Locator in GameObject.FindGameObjectsWithTag("Locator"))
                {
                    Destroy(Locator);
                }
            }
            Instantiate(stopper, transform);

            reseter.GetComponent<KeyColourReset>().ResetColours(reseter.GetComponent<KeyColourReset>().baseGameColor);
        }

        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
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
        if (safetyGrid[posX, posY] == true)
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
            // Kurz warten vorm restart damit man den roten key sieht
            StartCoroutine(ShowErrorAndRestart(keysSorted[posX, posY]));
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
                // Den falschen Key in der Liste suchen, um ihn rot zu färben
                GameObject wrongKeyObj = null;
                foreach (var keyObj in keys)
                {
                    if (keyObj != null && keyObj.GetComponent<KeyPers>().idKeyCode == pressedKey.ToString())
                    {
                        wrongKeyObj = keyObj;
                        break;
                    }
                }

                // Coroutine für die Verzögerung starten
                StartCoroutine(ShowErrorAndRestart(wrongKeyObj));
                return;
            }
        }

        if (whatToDoWithLocator == locatorStates.toLetterStart)
        {
            currentStage++;
            if (currentStage < locatorPositions.Length)
            {
                Instantiate(locator, keysSorted[(int)locatorPositions[currentStage].x, (int)locatorPositions[currentStage].y].transform);
            }
        }
    }

    // Coroutine, dieVerzögerung und rotes Aufleuchten regelt wenn man einen falschen key drückt
    private IEnumerator ShowErrorAndRestart(GameObject wrongKeyObj)
    {
        isProcessingError = true; // Input sperren

        if (wrongKeyObj != null)
        {
            // Falsches Feld einfärben
            wrongKeyObj.GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.wrongKeyPressed);
        }

        // Kurze Verzögerung
        yield return new WaitForSeconds(1.0f);

        // Den Fortschritt zurücksetzen
        MoveLocator(locatorStates.restart, null, KeyCode.None);
        ColorItIn(personalRecolour.restart, null);

        isProcessingError = false; // Input wieder freigeben
    }


    private void ColorItIn(personalRecolour state, KeyPers keyToRecolour)
    {
        switch (state)
        {
            case personalRecolour.restart:

                // alle Tasten einfach schwarz färben
                foreach (var key in keys)
                {
                    if (key != null)
                    {
                        key.GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.languageBase);
                    }
                }

                //  5x15 Grid durchgehen und NUR den sicheren Pfad gelb einfärben
                for (int x = 0; x < safetyGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < safetyGrid.GetLength(1); y++)
                    {
                        // Nur wenn an der Koordinate ein Key existiert und das Grid dort sicher ist
                        if (keysSorted[x, y] != null && safetyGrid[x, y] == true)
                        {
                            keysSorted[x, y].GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.locatorPath);
                        }
                    }
                }

                // Den aktuellen Locator setzen
                keysSorted[(int)locatorPositions[currentStage].x, (int)locatorPositions[currentStage].y].GetComponent<KeyPers>().recolourEverything(KeyPers.RecolourState.locatorPresent);

                break;

            case personalRecolour.locator:
                keyToRecolour.recolourEverything(KeyPers.RecolourState.locatorPresent);
                break;
        }
    }
}