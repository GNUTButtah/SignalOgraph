using System;
using UnityEngine;
using UnityEngine.UI;

public class DrawHandler : MonoBehaviour
{
    [SerializeField] SignalRGBManager sRGB;
    [SerializeField] GameObject stopDrawingButton;

    [SerializeField] GameObject stopper;
    [SerializeField] GameEnder gameEnder;
    [SerializeField] GameObject pixelTransformer;


    GameObject reseter;


    public Color[] cycleColors = new Color[6];

    // Pixel Image Settings
    private const int pixelImageWidth = 15;
    private const int pixelImageHeight = 5;

    private Texture2D pixelImageTexture;

    // Color Selection
    private int selectedColorIndex = 0;
    [SerializeField]
    private GameObject selectedColorImage;


    void Start()
    {
        pixelImageTexture = new Texture2D(
            pixelImageWidth,
            pixelImageHeight,
            TextureFormat.RGBA32,
            false
        );
        pixelImageTexture.filterMode = FilterMode.Point;
        ClearPixelImage();

        reseter = GameObject.FindGameObjectWithTag("KeyboardReseter");
        reseter.GetComponent<KeyColourReset>().TurnEveryKeyOneState(KeyPers.RecolourState.successBlack);
    }

    void Update()
    {
        foreach (KeyCode pressedKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(pressedKey))
            {
                Debug.Log("Gedrückter Key: " + pressedKey);
                HandleKeyPress(pressedKey);
            }
        }

        SelectNextColor();
    }

    private void OnEnable()
    {
        selectedColorImage.SetActive(true);
        stopDrawingButton.SetActive(true);
    }

    private void OnDisable()
    {
        if (selectedColorImage != null)
        {
            selectedColorImage.SetActive(false);
        }
    }



    // Keys handlen und an SignalRGB manager weitergeben
    private void HandleKeyPress(KeyCode pressedKey)
    {
        string signalRgbKeyName = pressedKey.ToString();
        Color selectedDrawColor = cycleColors[selectedColorIndex];

        // SignalRGB
        if (!string.IsNullOrEmpty(signalRgbKeyName))
        {
            if (signalRgbKeyName == "LeftShift")
            {
                sRGB.SetKeyColor("Less", selectedDrawColor);
            }

            if (signalRgbKeyName == "RightAlt")
            {
                sRGB.SetKeyColor("Fn", selectedDrawColor);
            }

            if (signalRgbKeyName == "Space")
            {
                sRGB.SetKeyColor("RazerLogo", selectedDrawColor);
            }
            sRGB.SetKeyColor(signalRgbKeyName, selectedDrawColor);
            sRGB.Apply();
        }

        // Pixel Image
        ApplyPixelColorForKey(pressedKey, selectedDrawColor);
    }



    // Keys auf die Pixel am 5  x 15 Bild mappen (einzeln, weil viele keys z.B. return und Leertaste mehr Pixel belegen müssen oder shift das auch <> belegen muss weil Unity es nicht erfassen kann)
    private void ApplyPixelColorForKey(KeyCode pressedKey, Color pixelColor)
    {
        Debug.Log($"{pressedKey} wird auf Farbe {pixelColor} gesetzt");
        switch (pressedKey)
        {
            case KeyCode.Escape: SetPixelColor(0, 0, pixelColor); break;
            case KeyCode.Alpha1: SetPixelColor(1, 0, pixelColor); break;
            case KeyCode.Alpha2: SetPixelColor(2, 0, pixelColor); break;
            case KeyCode.Alpha3: SetPixelColor(3, 0, pixelColor); break;
            case KeyCode.Alpha4: SetPixelColor(4, 0, pixelColor); break;
            case KeyCode.Alpha5: SetPixelColor(5, 0, pixelColor); break;
            case KeyCode.Alpha6: SetPixelColor(6, 0, pixelColor); break;
            case KeyCode.Alpha7: SetPixelColor(7, 0, pixelColor); break;
            case KeyCode.Alpha8: SetPixelColor(8, 0, pixelColor); break;
            case KeyCode.Alpha9: SetPixelColor(9, 0, pixelColor); break;
            case KeyCode.Alpha0: SetPixelColor(10, 0, pixelColor); break;
            case KeyCode.Minus: SetPixelColor(11, 0, pixelColor); break;
            case KeyCode.Equals: SetPixelColor(12, 0, pixelColor); break;
            case KeyCode.Backspace: SetPixelColor(13, 0, pixelColor); break;
            case KeyCode.Delete: SetPixelColor(14, 0, pixelColor); break;
            case KeyCode.Tab: SetPixelColor(0, 1, pixelColor); break;
            case KeyCode.Q: SetPixelColor(1, 1, pixelColor); break;
            case KeyCode.W: SetPixelColor(2, 1, pixelColor); break;
            case KeyCode.E: SetPixelColor(3, 1, pixelColor); break;
            case KeyCode.R: SetPixelColor(4, 1, pixelColor); break;
            case KeyCode.T: SetPixelColor(5, 1, pixelColor); break;
            case KeyCode.Y: SetPixelColor(6, 1, pixelColor); break;
            case KeyCode.U: SetPixelColor(7, 1, pixelColor); break;
            case KeyCode.I: SetPixelColor(8, 1, pixelColor); break;
            case KeyCode.O: SetPixelColor(9, 1, pixelColor); break;
            case KeyCode.P: SetPixelColor(10, 1, pixelColor); break;
            case KeyCode.LeftBracket: SetPixelColor(11, 1, pixelColor); break;
            case KeyCode.RightBracket: SetPixelColor(12, 1, pixelColor); break;
            case KeyCode.Hash: SetPixelColor(13, 1, pixelColor); break;
            case KeyCode.PageUp: SetPixelColor(14, 1, pixelColor); break;
            case KeyCode.CapsLock: SetPixelColor(0, 2, pixelColor); break;
            case KeyCode.A: SetPixelColor(1, 2, pixelColor); break;
            case KeyCode.S: SetPixelColor(2, 2, pixelColor); break;
            case KeyCode.D: SetPixelColor(3, 2, pixelColor); break;
            case KeyCode.F: SetPixelColor(4, 2, pixelColor); break;
            case KeyCode.G: SetPixelColor(5, 2, pixelColor); break;
            case KeyCode.H: SetPixelColor(6, 2, pixelColor); break;
            case KeyCode.J: SetPixelColor(7, 2, pixelColor); break;
            case KeyCode.K: SetPixelColor(8, 2, pixelColor); break;
            case KeyCode.L: SetPixelColor(9, 2, pixelColor); break;
            case KeyCode.Semicolon: SetPixelColor(10, 2, pixelColor); break;
            case KeyCode.Quote: SetPixelColor(11, 2, pixelColor); break;
            case KeyCode.Backslash: SetPixelColor(12, 2, pixelColor); break;
            case KeyCode.Return:
                SetPixelColor(13, 1, pixelColor);
                SetPixelColor(13, 2, pixelColor);
                break;
            case KeyCode.PageDown: SetPixelColor(14, 2, pixelColor); break;
            case KeyCode.LeftShift:
                SetPixelColor(0, 3, pixelColor);
                SetPixelColor(1, 3, pixelColor);
                break;
            case KeyCode.Z: SetPixelColor(2, 3, pixelColor); break;
            case KeyCode.X: SetPixelColor(3, 3, pixelColor); break;
            case KeyCode.C: SetPixelColor(4, 3, pixelColor); break;
            case KeyCode.V: SetPixelColor(5, 3, pixelColor); break;
            case KeyCode.B: SetPixelColor(6, 3, pixelColor); break;
            case KeyCode.N: SetPixelColor(7, 3, pixelColor); break;
            case KeyCode.M: SetPixelColor(8, 3, pixelColor); break;
            case KeyCode.Comma: SetPixelColor(9, 3, pixelColor); break;
            case KeyCode.Period: SetPixelColor(10, 3, pixelColor); break;
            case KeyCode.Slash: SetPixelColor(11, 3, pixelColor); break;
            case KeyCode.RightShift: SetPixelColor(12, 3, pixelColor); break;
            case KeyCode.UpArrow: SetPixelColor(13, 3, pixelColor); break;
            case KeyCode.Insert: SetPixelColor(14, 3, pixelColor); break;
            case KeyCode.LeftControl: SetPixelColor(0, 4, pixelColor); break;
            case KeyCode.LeftWindows: SetPixelColor(1, 4, pixelColor); break;
            case KeyCode.LeftAlt: SetPixelColor(2, 4, pixelColor); break;
            case KeyCode.Space:
                SetPixelColor(3, 4, pixelColor);
                SetPixelColor(4, 4, pixelColor);
                SetPixelColor(5, 4, pixelColor);
                SetPixelColor(6, 4, pixelColor);
                SetPixelColor(7, 4, pixelColor);
                SetPixelColor(8, 4, pixelColor);
                break;
            case KeyCode.RightAlt:
                SetPixelColor(9, 4, pixelColor);
                SetPixelColor(10, 4, pixelColor);
                break;
            case KeyCode.RightControl: SetPixelColor(11, 4, pixelColor); break;
            case KeyCode.LeftArrow: SetPixelColor(12, 4, pixelColor); break;
            case KeyCode.DownArrow: SetPixelColor(13, 4, pixelColor); break;
            case KeyCode.RightArrow: SetPixelColor(14, 4, pixelColor); break;
        }


        pixelImageTexture.Apply();
    }

    private void SetPixelColor(int pixelXPosition, int pixelYPosition, Color pixelColor)
    {
        if (pixelXPosition < 0 || pixelXPosition >= pixelImageWidth)
            return;

        if (pixelYPosition < 0 || pixelYPosition >= pixelImageHeight)
            return;

        int pixelFlippedYPosition = (pixelImageHeight - 1) - pixelYPosition;
        pixelImageTexture.SetPixel(pixelXPosition, pixelFlippedYPosition, pixelColor);
        Debug.Log($"Pixel {pixelXPosition} , {pixelYPosition} auf {cycleColors[selectedColorIndex]} gesetzt.");
    }

    private void ClearPixelImage()
    {
        for (int xIndex = 0; xIndex < pixelImageWidth; xIndex++)
        {
            for (int yIndex = 0; yIndex < pixelImageHeight; yIndex++)
            {
                pixelImageTexture.SetPixel(xIndex, yIndex, Color.black);
            }
        }

        pixelImageTexture.Apply();
    }

    //Farbe wechseln
    private Color SelectNextColor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedColorIndex++;

            if (selectedColorIndex >= cycleColors.Length)
            {
                selectedColorIndex = 0;
            }
            selectedColorImage.GetComponent<Image>().color = cycleColors[selectedColorIndex];
            Debug.Log($"Aktive Zeichenfarbe: {cycleColors[selectedColorIndex]}");
        }

        return cycleColors[selectedColorIndex];
    }

    public void SavePixelImageAsPNG()
    {
        byte[] pngByteData = pixelImageTexture.EncodeToPNG();

        string saveDirectoryPath = Application.dataPath + "/StreamingAssets";
        string saveFilePath = saveDirectoryPath + "/newPixelInput.png";

        if (!System.IO.Directory.Exists(saveDirectoryPath))
        {
            System.IO.Directory.CreateDirectory(saveDirectoryPath);
        }

        System.IO.File.WriteAllBytes(saveFilePath, pngByteData);

        Debug.Log("Pixel-Image gespeichert unter: " + saveFilePath);


    }

    public void DrawStopper()
    {
        Debug.Log("We got to the stopper");
        pixelTransformer = GameObject.FindGameObjectWithTag("PixelTransformer");
        Debug.Log("Pixeltransformer: " + pixelTransformer.name);
        if (pixelTransformer != null)
        {
            pixelTransformer.GetComponent<TransformerCaller>().SendGenerateCommand(1);
        }
        stopDrawingButton.SetActive(false);
        Instantiate(stopper, transform);

        reseter.GetComponent<KeyColourReset>().ResetColours(Color.black);
        gameEnder.LetTheGameEnd();
    }
}
