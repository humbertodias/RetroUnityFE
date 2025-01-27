using System.Diagnostics.CodeAnalysis;
using SK.Libretro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class LibretroWrapperManager : MonoBehaviour
{
    public string core = "snes9x";
    public string gameName = "Classic Kong Complete (U)";
    public string gameDirectory = "Assets/StreamingAssets/libretro~/roms";

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

    void Start()
    {
        Wrapper = new Wrapper();
        if (Wrapper.StartGame(core, gameDirectory, gameName))
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