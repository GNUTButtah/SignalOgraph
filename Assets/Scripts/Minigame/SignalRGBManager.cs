using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class SignalRGBManager : MonoBehaviour
{
    [Header("Path to HTML effect file (SignalRGB)")]
    public string htmlFilePath;

    // Key color dictionary
    private Dictionary<string, Color> keyColors = new Dictionary<string, Color>();

    // Set color for a single key
    public void SetKeyColor(string keyName, Color color)
    {
        if (keyColors.ContainsKey(keyName))
            keyColors[keyName] = color;
        else
            keyColors.Add(keyName, color);
    }

    // Apply changes → nur den colors Scriptblock ersetzen
    public void Apply()
    {
        // 1. Farben als JSON-String erzeugen
        string jsonColors = "{\n";
        foreach (var kvp in keyColors)
        {
            string hex = ColorUtility.ToHtmlStringRGB(kvp.Value);
            jsonColors += $"  \"{kvp.Key}\": \"#{hex}\",\n";
        }
        if (jsonColors.EndsWith(",\n"))
            jsonColors = jsonColors.Substring(0, jsonColors.Length - 2) + "\n";
        jsonColors += "}";

        // 2. HTML-Datei laden
        string html = File.ReadAllText(htmlFilePath);

        // 3. Neuer colors-Scriptblock
        string newColorsScript = $"<script>\nconst colors = {jsonColors};\n</script>";

        // 4. Alten colors-Block mit Regex ersetzen
        string pattern = @"<script>\s*const colors = \{[\s\S]*?\};\s*</script>";
        string newHtml = Regex.Replace(html, pattern, newColorsScript);

        // 5. Datei speichern
        File.WriteAllText(htmlFilePath, newHtml);

        Debug.Log("SignalRGB HTML colors block updated: " + htmlFilePath);
    }
}
