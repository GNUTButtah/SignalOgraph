using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class DrawHandler : MonoBehaviour
{
    [SerializeField] SignalRGBManager sRGB;
    [SerializeField] GameObject stopDrawingButton;
    

    [SerializeField] GameObject[] keys;
    [SerializeField] GameObject brush;
    [SerializeField] GameObject stopper;

    public int colorCycler;

    public event EventHandler OnWhoToDraw;

    public Color[] drawingColors;
    public Color currentColor;


    private void Start()
    {
        currentColor = drawingColors[colorCycler];

        keys = GameObject.FindGameObjectsWithTag("Key");

        sRGB = GameObject.FindGameObjectWithTag("SRGB").GetComponent<SignalRGBManager>();
    }

    private void Update()
    {
        stopDrawingButton.SetActive(gameObject.activeInHierarchy);

        if (Input.anyKeyDown)
        {
            foreach (var key in keys)
            {
                if(Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), key.GetComponent<KeyPers>().idKeyCode)))
                {
                    Instantiate(brush,key.transform);
                    OnWhoToDraw?.Invoke(this, null);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (colorCycler >= drawingColors.Length - 1) 
            {
                colorCycler = 0;
            }
            else
                colorCycler++;

            currentColor = drawingColors[colorCycler];

            sRGB.SetKeyColor("LeftControl", drawingColors[colorCycler]);
            sRGB.Apply();
        }
    }

    public void DrawStopper()
    {
        stopDrawingButton.SetActive(false);
        Instantiate(stopper, transform);
    }
}
