using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TransformerCaller : MonoBehaviour
{
    public TcpClient client;
    StreamReader reader;
    Process backendProcess = new Process();
    private string currentUserName;

    private float reconnectTimer = 0f;
    private float reconnectInterval = 2.0f;

    // Der Inhalt von der Pythonmessage
    private string pythonMessageContent;

    private Image generatedImage;
    private Image waitImage;

    // Variable mit get/set, damit man immer, wenn sich die message ändert etwas machen kann 
    public string pythonMessage
    {
        get
        {
            //wenn ich die Python Message lesen will, will ich einfach den Inhalt bekommen
            return pythonMessageContent;
        }
        set
        {
            //wenn ich die message überschreiben will, soll sie halt den neuen Wert bekommen, aber auch die message verarbeiten (weiter unten)
            pythonMessageContent = value;

            UnityEngine.Debug.Log(value);

            if (!string.IsNullOrEmpty(pythonMessageContent))
            {
                ProcessPythonCommand(pythonMessageContent);
            }
        }
    }

    public void PressedRegularStartButton(bool createNoWindow)
    {
        DontDestroyOnLoad(this.gameObject);
        //Die install bzw. launch scripts für den stablediffusion Teil des games sind je nach Platform und Hardware auf dem das game gespielt wird unterschiedlioch, deswegen ist die Start function so lang
        currentUserName = Environment.UserName.ToString();

        string fileName = "";
        string arguments = "";

        // überprüfen ob das game auf einem mac läuft
        if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor)
        {
            UnityEngine.Debug.Log("Mac detected. Launching Mac setup script.");

            fileName = "/bin/bash";

            // Pfad in Anführungszeichen, weil die Ordnerstruktur von dem, der das Spiel spielt ein Leerzeichen drinnen haben kann und das sonst im Terminal alles kaputt macht
            arguments = "\"" + Application.streamingAssetsPath + "/start_mac.sh\"";
        }

        // überprüfen ob das game auf windows läuft
        else if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor)
        {
            string gpuVendor = SystemInfo.graphicsDeviceVendor.ToLower();
            UnityEngine.Debug.Log(gpuVendor);
            string batPath = "";

            // Tell Windows to use the command prompt
            fileName = "cmd.exe";

            if (gpuVendor.Contains("nvidia"))
            {
                batPath = Application.streamingAssetsPath + "/start_windows_nvidia.bat";
                if (!File.Exists(batPath))
                {
                    UnityEngine.Debug.LogError("NVIDIA installscript not found: " + batPath);
                }
                else
                {
                    UnityEngine.Debug.Log($"NVIDIA detected. Launching CUDA setup script at {batPath}");
                }
            }
            else
            {
                batPath = Application.streamingAssetsPath + "/start_windows_amd.bat";

                if (!File.Exists(batPath))
                {
                    UnityEngine.Debug.LogError("AMD/INTEL installscript not found: " + batPath);
                }
                else
                {
                    UnityEngine.Debug.Log("AMD/Intel detected. Launching DirectML setup script.");
                }
            }

            // Das "/c" bewirkt, dass das cmd.exe nach dem Ausführen von dem .bat file stoppt
            arguments = "/c \"" + batPath + "\"";
        }
        else
        {
            UnityEngine.Debug.LogError("Unsupported Operating System, game works on Windows and MacOS only");
            return; // Auf linux läuft das game halt nicht
        }

        // Hier wird der Prozess erstellt, mit all den Parametern die über das System gesammelt wurden
        backendProcess.StartInfo.FileName = fileName;
        backendProcess.StartInfo.Arguments = arguments;

        // Wenn man kein Terminal Fenster haben will, kann man das hier ändern, aber dann hat man halt keinen Ahnhaltspunkt, der einem sagt, wie weit die dependencies schon runtergeladen sind
        backendProcess.StartInfo.UseShellExecute = true;
        backendProcess.StartInfo.CreateNoWindow = createNoWindow;

        // Hier wird der Prozess dann endlich ausgeführt
        try
        {
            backendProcess.Start();
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Failed to launch AI backend: " + e.Message);
        }
    }

    public void TryConnect(int port)
    {
        try
        {
            TcpClient testClient = new TcpClient();
            testClient.Connect("127.0.0.1", port);

            client = testClient;
            reader = new StreamReader(client.GetStream());
            UnityEngine.Debug.Log("Connected on port " + port);

        }
        catch (Exception)
        {
            // Anderen Port versuchen, debug ist in anderem Codeblock
        }
    }

    void Update()
    {
        if (client != null && client.Connected && client.Available > 0)
        {
            string rawLine = reader.ReadLine();

            if (!string.IsNullOrEmpty(rawLine))
            {
                //Triggert den set block von oben
                pythonMessage = rawLine;
            }
        }
        else if (client == null || !client.Connected)
        {
            reconnectTimer += Time.deltaTime; // Add the time since the last frame

            if (reconnectTimer >= reconnectInterval)
            {
                UnityEngine.Debug.Log("Attempting to connect to Python...");
                TryConnect(5007);

                reconnectTimer = 0f; // Reset the timer
            }
        }
    }

    // Wird jedes mal gecallt, wenn man vom python script eine message bekommt
    void ProcessPythonCommand(string pythonMessage)
    {
        UnityEngine.Debug.Log("Message von Python: " + pythonMessage);

        if (pythonMessage.Contains("DONE"))
        {
            // When Python says it's done generating, swap the image
            GameFinished();
        }
    }

    public void SendGenerateCommand(int levelId)
    {
        // Nochmal checken, ob die TCP connection verbunden is
        if (client == null || !client.Connected)
        {
            UnityEngine.Debug.LogWarning("Cannot send command: Not connected to the Python backend.");
            return;
        }

        try
        {
            // den string in dem Format bauen, in dem ich ihn im python Programm abfrage
            string messageToSend = "GENERATE|" + levelId.ToString();

            // in UTF-8 kodieren
            byte[] data = Encoding.UTF8.GetBytes(messageToSend);

            // Die Bytes über TCP an das Programm schicken
            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length);

            // Direkt 
            stream.Flush();

            UnityEngine.Debug.Log("Sent command to Python: " + messageToSend);
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error sending command to Python: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        reader?.Close();
        client?.Close();

        KillProcessTree(backendProcess);
    }

    public void CloseConnection()
    {
        try
        {
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }

            if (client != null)
            {
                client.Close();
                client = null;
            }

            UnityEngine.Debug.Log("Socket connection closed manually.");

        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Error while closing connection: " + e.Message);
        }
    }

    void KillProcessTree(Process process)
    {
        if (process == null || process.HasExited)
        {
            UnityEngine.Debug.Log($"Already quit Process {process.ProcessName}");
            return;
        }
        else
        {
            UnityEngine.Debug.Log($"Trying to Quit the following Program: {process.ProcessName}");
            Process.Start(new ProcessStartInfo
            {
                FileName = "taskkill",
                Arguments = $"/PID {process.Id} /T /F",
                CreateNoWindow = true,
                UseShellExecute = false
            });

            process.Dispose();
        }
    }

    public void GameFinished()
    {
        // 1. Get the placeholder image
        waitImage = GameObject.FindGameObjectWithTag("DrawnImage").GetComponent<Image>();

        if (waitImage == null)
        {
            UnityEngine.Debug.LogError("Could not find an Image with the tag 'DrawnImage'!");
            return;
        }

        // 2. Establish the exact path to the output image
        string fileName = "output_level_1.png";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        // 3. Read the image and apply it
        if (File.Exists(filePath))
        {
            // Read bytes from the drive
            byte[] fileData = File.ReadAllBytes(filePath);

            // Create a temporary texture. (The 2x2 size is a placeholder; LoadImage automatically resizes it)
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            // Convert to a Sprite
            Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // Overwrite the placeholder!
            waitImage.sprite = newSprite;

            UnityEngine.Debug.Log("Successfully loaded the new AI generated image.");
        }
        else
        {
            UnityEngine.Debug.LogError($"File not found. Expected AI image at: {filePath}");
        }
    }
}