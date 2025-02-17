using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using SK.Libretro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class LibretroWrapperManager : MonoBehaviour
{
    [SerializeField] public string coreName = "snes9x";
    [SerializeField] public string coreDirectory = "Assets/StreamingAssets/libretro~/cores";
    [SerializeField] public string gameName = "Classic Kong Complete (U)"; 
    [SerializeField] public string gameDirectory = "Assets/StreamingAssets/libretro~/roms";
    [SerializeField] string RootDirectory = Application.platform == RuntimePlatform.Android ? 
        Application.persistentDataPath : 
        Application.streamingAssetsPath;


    [Range(0.5f, 2f)] [SerializeField] private float _timeScale = 1.0f;

    [FormerlySerializedAs("Display")] public Renderer _rendererComponent;
    public Material _originalMaterial;

    private float _frameTimer = 60;

    private Wrapper Wrapper;
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX || UNITY_STANDALONE_LINUX
    private bool useUnityAudio = true;
#else
        private bool useUnityAudio = false;
#endif


    private bool useRunLoop = true;

    private bool
        activateGraphics =
            true; // When Update() is used we need to activate graphics there so unityGraphics.TextureUpdated is at least once true

    bool ParseCommandLineArgs(ref string coreDirectory, ref string coreName, ref string gameDirectory, ref string gameName)
    {
        string[] args = Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if ((args[i] == "-L" || args[i] == "--libretro") && i + 2 < args.Length)
            {
                string corePath = args[i + 1];
                string gamePath = args[i + 2];

                if (File.Exists(corePath) && File.Exists(gamePath))
                {
                    coreDirectory = Path.GetDirectoryName(corePath);
                    coreName = Path.GetFileName(corePath).Split("_libretro")[0];
                    
                    gameDirectory = Path.GetDirectoryName(gamePath);
                    gameName = Path.GetFileNameWithoutExtension(gamePath);
                    
                    Debug.Log("Parsed Core {coreName} | Core Directory {coreDirectory} | Game Directory {gameDirectory} | Game Name {gameName}");
                    
                    return true;
                }
                else
                {
                    Debug.LogError($"File does not exist: Core Path {corePath} or Game Path {gamePath}");
                }

                i += 2;
            }
        }
        return false;
    }
    
    bool ParseConfigFile(string configFilePath, ref string coreDirectory, ref string coreName, ref string gameDirectory, ref string gameName)
    {
        if (!File.Exists(configFilePath))
        {
            Debug.LogError($"Config file not found: {configFilePath}");
            return false;
        }

        try
        {
            string[] lines = File.ReadAllLines(configFilePath);
            string corePath = null;
            string gamePath = null;

            foreach (string line in lines)
            {
                string[] parts = line.Split('=');
                if (parts.Length != 2) continue;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (key.Equals("corePath", StringComparison.OrdinalIgnoreCase))
                {
                    corePath = value;
                }
                else if (key.Equals("gamePath", StringComparison.OrdinalIgnoreCase))
                {
                    gamePath = value;
                }
            }

            if (!string.IsNullOrEmpty(corePath) && !string.IsNullOrEmpty(gamePath) &&
                File.Exists(corePath) && File.Exists(gamePath))
            {
                coreDirectory = Path.GetDirectoryName(corePath);
                coreName = Path.GetFileName(corePath).Split("_libretro")[0];

                gameDirectory = Path.GetDirectoryName(gamePath);
                gameName = Path.GetFileNameWithoutExtension(gamePath);

                Debug.Log($"Parsed Core {coreName} | Core Directory {coreDirectory} | Game Directory {gameDirectory} | Game Name {gameName}");
                return true;
            }
            else
            {
                Debug.LogError($"Invalid or missing paths in config file: Core Path {corePath}, Game Path {gamePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error reading config file: {ex.Message}");
        }
        return false;
    }

    
    void Awake()
    {
        
        string rootDirectory = Application.platform == RuntimePlatform.Android ? Application.persistentDataPath : AppDomain.CurrentDomain.BaseDirectory;
        Debug.Log($"rootDirectory:{rootDirectory}");
        if (!ParseCommandLineArgs(ref coreDirectory, ref coreName, ref gameDirectory, ref gameName))
        {
            string configFilePath = Path.Combine(rootDirectory, "RetroUnityFE.ini");
            Debug.Log($"configFilePath:{configFilePath}");
            if (!File.Exists(configFilePath))
            {
                using (StreamWriter writer = new StreamWriter(configFilePath))
                {
                    writer.WriteLine($"corePath={coreDirectory}/{coreName}");
                    writer.WriteLine($"gamePath={gameDirectory}/{gameName}");
                }
                
            }
            ParseConfigFile(configFilePath, ref coreDirectory, ref coreName, ref gameDirectory, ref gameName);
        }
        Wrapper = new Wrapper(rootDirectory);
        if (Wrapper.StartGame(coreDirectory, coreName, gameDirectory, gameName))
        {
            ActivateGraphics();
            ActivateAudio();
            ActivateInput();
            _originalMaterial = _rendererComponent.material;
            var _newMaterial = new Material(_rendererComponent.material);
            _rendererComponent.material = _newMaterial;
            _rendererComponent.material.mainTextureScale = new Vector2(1f, -1f);
            _rendererComponent.material.color = Color.black;
            _rendererComponent.material.EnableKeyword("_EMISSION");
            _rendererComponent.material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            _rendererComponent.material.SetColor("_EmissionColor", Color.white);
            if (useRunLoop)
            {
                InvokeRepeating("LibretroRunLoop", 0f, 1f / (float) Wrapper.Game.SystemAVInfo.timing.fps);
            }
        }
        else
        {
            Wrapper.StopGame();
            Wrapper = null;
        }
    }


    private void ActivateGraphics()
    {
        Wrapper?.ActivateGraphics(new UnityGraphicsProcessor());
    }

    private void ActivateAudio()
    {
        //  var unityAudioProcessorComponent = gameObject.AddComponent<UnityAudioProcessorComponent>();
        UnityAudioProcessorComponent unityAudio = GetComponent<UnityAudioProcessorComponent>();
        if (unityAudio == null && useUnityAudio)
        {
            unityAudio = gameObject.AddComponent<UnityAudioProcessorComponent>();
        }

        if (unityAudio != null)
        {
            Wrapper?.ActivateAudio(unityAudio);
        }
        else
        {
            Wrapper?.ActivateAudio(new NAudioAudioProcessor());
        }
    }

    private void ActivateInput()
    {
        Wrapper?.ActivateInput(FindFirstObjectByType<PlayerInputManager>().GetComponent<IInputProcessor>());
    }


    private void Update()
    {
        if (useRunLoop)
        {
            return;
        }

        if (activateGraphics)
        {
            ActivateGraphics();
            activateGraphics = false;
        }

        if (Wrapper != null && Wrapper.Game.SystemAVInfo.timing.fps > 0.0)
        {
            _frameTimer += Time.deltaTime;
            float timePerFrame = 1f / (float) Wrapper.Game.SystemAVInfo.timing.fps / _timeScale;

            while (_frameTimer >= timePerFrame)
            {
                Wrapper.Update();
                _frameTimer -= timePerFrame;
            }

            if (Wrapper.GraphicsProcessor != null && Wrapper.GraphicsProcessor is UnityGraphicsProcessor unityGraphics)
            {
                if (unityGraphics.TextureUpdated)
                {
                    _rendererComponent.material.SetTexture("_EmissionMap", unityGraphics.Texture);
                }
            }
        }
    }

    [SuppressMessage("Code Quality", "IDE0051:Remove unused private members",
        Justification = "Called by InvokeRepeating")]
    private void LibretroRunLoop()
    {
        if (Wrapper != null)
        {
            Wrapper.Update();
            if (Wrapper.GraphicsProcessor != null && Wrapper.GraphicsProcessor is UnityGraphicsProcessor unityGraphics)
            {
                if (unityGraphics.TextureUpdated)
                {
                    _rendererComponent.material.SetTexture("_EmissionMap", unityGraphics.Texture);
                }
            }
        }
    }

}