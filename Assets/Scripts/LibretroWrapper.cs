/* This file reuses code from the following file: https://github.com/fr500/R.net/blob/34a9c867684e6a7891280de5b8c373482247fc93/R.net/libretro-sharpie.cs
 * See the original license below:  
 * 
 * R.net project
 *  Copyright (C) 2010-2015 - Andrés Suárez
 *  Copyright (C) 2010-2011 - Iván Fernandez
 *
 *  libretro.net is free software: you can redistribute it and/or modify it under the terms
 *  of the GNU General Public License as published by the Free Software Found-
 *  ation, either version 3 of the License, or (at your option) any later version.
 *
 *  libretro.net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
 *  without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *  PURPOSE.  See the GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License along with libretro.net.
 *  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using Utility;


public class LibretroWrapper : MonoBehaviour
{
    private static Speaker _leftSpeaker;
    private static Speaker _rightSpeaker;

    public static Texture2D tex;
    public static int pix;
    public static int w;
    public static int h;
    public static int p;

    public static byte[] Src;
    public static byte[] Dst;

    public enum PixelFormat
    {
        // 0RGB1555, native endian. 0 bit must be set to 0.
        // This pixel format is default for compatibility concerns only.
        // If a 15/16-bit pixel format is desired, consider using RGB565.
        RetroPixelFormat_0RGB1555 = 0,

        // XRGB8888, native endian. X bits are ignored.
        RetroPixelFormatXRGB8888 = 1,

        // RGB565, native endian. This pixel format is the recommended format to use if a 15/16-bit format is desired
        // as it is the pixel format that is typically available on a wide range of low-power devices.
        // It is also natively supported in APIs like OpenGL ES.
        RetroPixelFormatRGB565 = 2,

        // Ensure sizeof() == sizeof(int).
        RetroPixelFormatUnknown = int.MaxValue
    }

    private void Start()
    {
        _leftSpeaker = GameObject.Find("LeftSpeaker").GetComponent<Speaker>();
        _rightSpeaker = GameObject.Find("RightSpeaker").GetComponent<Speaker>();
    }

    //Shouldn't be part of the wrapper, will remove later
    [StructLayout(LayoutKind.Sequential)]
    public class Pixel
    {
        public float Alpha;
        public float Red;
        public float Green;
        public float Blue;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SystemAVInfo
    {
        public Geometry geometry;
        public Timing timing;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct GameInfo
    {
        public char* path;
        public void* data;
        public uint size;
        public char* meta;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Geometry
    {
        public uint base_width;
        public uint base_height;
        public uint max_width;
        public uint max_height;
        public float aspect_ratio;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Timing
    {
        public double fps;
        public double sample_rate;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SystemInfo
    {
        public char* library_name;
        public char* library_version;
        public char* valid_extensions;

        [MarshalAs(UnmanagedType.U1)] public bool need_fullpath;

        [MarshalAs(UnmanagedType.U1)] public bool block_extract;
    }

    public class Environment
    {
        public const uint RetroEnvironmentSetRotation = 1;
        public const uint RetroEnvironmentGetOverscan = 2;
        public const uint RetroEnvironmentGetCanDupe = 3;
        public const uint RetroEnvironmentGetVariable = 4;
        public const uint RetroEnvironmentSetVariables = 5;
        public const uint RetroEnvironmentSetMessage = 6;
        public const uint RetroEnvironmentShutdown = 7;
        public const uint RetroEnvironmentSetPerformanceLevel = 8;
        public const uint RetroEnvironmentGetSystemDirectory = 9;
        public const uint RetroEnvironmentSetPixelFormat = 10;
        public const uint RetroEnvironmentSetInputDescriptors = 11;
        public const uint RetroEnvironmentSetKeyboardCallback = 12;
    }

    public class Wrapper
    {
        public const int AudioBatchSize = 2048;
        public static float[] AudioBatch = new float[AudioBatchSize];
        public static int BatchPosition;
        private PixelFormat _pixelFormat;
        private bool _requiresFullPath;
        private SystemAVInfo _av;
        private Pixel[] _frameBuffer;
        public static Texture2D Tex { get; set; }
        public static int Pix = 0;
        public static int w = 0;
        public static int h = 0;
        public static int p = 0;
        public static uint Button;
        public static uint Keep;

        //Prevent GC on delegates as long as the wrapper is running
        private Libretro.RetroEnvironmentDelegate _environment;
        private Libretro.RetroVideoRefreshDelegate _videoRefresh;
        private Libretro.RetroAudioSampleDelegate _audioSample;
        private Libretro.RetroAudioSampleBatchDelegate _audioSampleBatch;
        private Libretro.RetroInputPollDelegate _inputPoll;
        private Libretro.RetroInputStateDelegate _inputState;

        public Wrapper(string coreToLoad)
        {
            Libretro.InitializeLibrary(coreToLoad);
        }

        public unsafe void Init()
        {
            int apiVersion = Libretro.RetroApiVersion();
            SystemInfo info = new SystemInfo();
            Libretro.RetroGetSystemInfo(ref info);

            string coreName = Marshal.PtrToStringAnsi((IntPtr) info.library_name);
            string coreVersion = Marshal.PtrToStringAnsi((IntPtr) info.library_version);
            string validExtensions = Marshal.PtrToStringAnsi((IntPtr) info.valid_extensions);
            _requiresFullPath = info.need_fullpath;
            bool blockExtract = info.block_extract;

            Debug.Log("Core information:");
            Debug.Log("API Version: " + apiVersion);
            Debug.Log("Core Name: " + coreName);
            Debug.Log("Core Version: " + coreVersion);
            Debug.Log("Valid Extensions: " + validExtensions);
            Debug.Log("Block Extraction: " + blockExtract);
            Debug.Log("Requires Full Path: " + _requiresFullPath);

            _environment = RetroEnvironment;
            _videoRefresh = RetroVideoRefresh;
            _audioSample = RetroAudioSample;
            _audioSampleBatch = RetroAudioSampleBatch;
            _inputPoll = RetroInputPoll;
            _inputState = RetroInputState;

            Debug.Log("Setting up environment:");

            Libretro.RetroSetEnvironment(_environment);
            Libretro.RetroSetVideoRefresh(_videoRefresh);
            Libretro.RetroSetAudioSample(_audioSample);
            Libretro.RetroSetAudioSampleBatch(_audioSampleBatch);
            Libretro.RetroSetInputPoll(_inputPoll);
            Libretro.RetroSetInputState(_inputState);

            Libretro.RetroInit();
        }

        public bool Update()
        {
            Libretro.RetroRun();
            return true;
        }

        public SystemAVInfo GetAVInfo()
        {
            return _av;
        }

        public Pixel[] GetFramebuffer()
        {
            return _frameBuffer;
        }

        private unsafe void RetroVideoRefresh(void* data, uint width, uint height, uint pitch)
        {
            // Process Pixels one by one for now...this is not the best way to do it 
            // should be using memory streams or something

            //Declare the pixel buffer to pass on to the renderer
            _frameBuffer = new Pixel[width * height];

            //Get the array from unmanaged memory as a pointer
            var pixels = (IntPtr) data;
            //Gets The pointer to the row start to use with the pitch
            //IntPtr rowStart = pixels;

            //Get the size to move the pointer
            //int size = 0;

            uint i;
            uint j;

            switch (_pixelFormat)
            {
                case PixelFormat.RetroPixelFormat_0RGB1555:

                    LibretroWrapper.w = Convert.ToInt32(width);
                    LibretroWrapper.h = Convert.ToInt32(height);
                    if (tex == null)
                    {
                        tex = new Texture2D(LibretroWrapper.w, LibretroWrapper.h, TextureFormat.RGB565, false);
                    }

                    LibretroWrapper.p = Convert.ToInt32(pitch);

                    //size = Marshal.SizeOf(typeof(short));
                    for (i = 0; i < height; i++)
                    {
                        for (j = 0; j < width; j++)
                        {
                            short packed = Marshal.ReadInt16(pixels);
                            _frameBuffer[i * width + j] = new Pixel
                            {
                                Alpha = 1,
                                Red = ((packed >> 10) & 0x001F) / 31.0f,
                                Green = ((packed >> 5) & 0x001F) / 31.0f,
                                Blue = (packed & 0x001F) / 31.0f
                            };
                            var color = new Color(((packed >> 10) & 0x001F) / 31.0f, ((packed >> 5) & 0x001F) / 31.0f,
                                (packed & 0x001F) / 31.0f, 1.0f);
                            tex.SetPixel((int)i, (int)j, color);
                            //pixels = (IntPtr)((int)pixels + size);
                        }

                        tex.filterMode = FilterMode.Trilinear;
                        tex.Apply();
                        //pixels = (IntPtr)((int)rowStart + pitch);
                        //rowStart = pixels;
                    }

                    break;
                case PixelFormat.RetroPixelFormatXRGB8888:

                        LibretroWrapper.w = Convert.ToInt32(width);
                        LibretroWrapper.h = Convert.ToInt32(height);
                        if (tex == null) {
                            tex = new Texture2D(LibretroWrapper.w, LibretroWrapper.h, TextureFormat.RGB565, false);
                        }
                        LibretroWrapper.p = Convert.ToInt32(pitch);

                        //size = Marshal.SizeOf(typeof(int));
                        for (i = 0; i < height; i++) {
                            for (j = 0; j < width; j++) {
                                int packed = Marshal.ReadInt32(pixels);
                                _frameBuffer[i * width + j] = new Pixel {
                                    Alpha = 1,
                                    Red = ((packed >> 16) & 0x00FF) / 255.0f,
                                    Green = ((packed >> 8) & 0x00FF) / 255.0f,
                                    Blue = (packed & 0x00FF) / 255.0f
                                };
                                var color = new Color(((packed >> 16) & 0x00FF) / 255.0f, ((packed >> 8) & 0x00FF) / 255.0f, (packed & 0x00FF) / 255.0f, 1.0f);
                                tex.SetPixel((int)i, (int)j, color);
                                //pixels = (IntPtr)((int)pixels + size);
                            }
                            //pixels = (IntPtr)((int)rowStart + pitch);
                            //rowStart = pixels;

                        }

                        tex.filterMode = FilterMode.Trilinear;
                        tex.Apply();
                        break;

                case PixelFormat.RetroPixelFormatRGB565:

                    var imagedata565 = new IntPtr(data);
                    LibretroWrapper.w = Convert.ToInt32(width);
                    LibretroWrapper.h = Convert.ToInt32(height);
                    if (tex == null)
                    {
                        tex = new Texture2D(LibretroWrapper.w, LibretroWrapper.h, TextureFormat.RGB565, false);
                    }

                    LibretroWrapper.p = Convert.ToInt32(pitch);
                    int srcsize565 = 2 * (LibretroWrapper.p * LibretroWrapper.h);
                    int dstsize565 = 2 * (LibretroWrapper.w * LibretroWrapper.h);
                    Src = new byte[srcsize565];
                    Dst = new byte[dstsize565];
                    Marshal.Copy(imagedata565, Src, 0, srcsize565);
                    int m565 = 0;
                    for (int y = 0; y < LibretroWrapper.h; y++)
                    {
                        for (int k = 0 * 2 + y * LibretroWrapper.p;
                            k < LibretroWrapper.w * 2 + y * LibretroWrapper.p;
                            k++)
                        {
                            Dst[m565] = Src[k];
                            m565++;
                        }
                    }

                    tex.LoadRawTextureData(Dst);
                    tex.filterMode = FilterMode.Trilinear;
                    tex.Apply();
                    break;
                case PixelFormat.RetroPixelFormatUnknown:
                    _frameBuffer = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RetroAudioSample(short left, short right)
        {
            // Unused.
        }

        private unsafe void RetroAudioSampleBatch(short* data, uint frames)
        {
            for (int i = 0; i < (int) frames; i++)
            {
                short chunk = Marshal.ReadInt16((IntPtr) data);
                data += sizeof(short); // Set pointer to next chunk.
                float value = chunk / 32768f; // Divide by Int16 max to get correct float value.
                value = Mathf.Clamp(value, -1.0f, 1.0f); // Unity's audio only takes values between -1 and 1.

                AudioBatch[BatchPosition] = value;
                BatchPosition++;

                // When the batch is filled send it to the speakers.
                if (BatchPosition >= AudioBatchSize - 1)
                {
                    _leftSpeaker.UpdateAudio(AudioBatch);
                    _rightSpeaker.UpdateAudio(AudioBatch);
                    BatchPosition = 0;
                }
            }
        }

        private void RetroInputPoll()
        {
        }

        public static short RetroInputState(uint port, uint device, uint index, uint id)
        {
            switch (id)
            {
                case 0:
                    return Input.GetKey(KeyCode.Z) || Input.GetButton("B") ? (short) 1 : (short) 0; // B
                case 1:
                    return Input.GetKey(KeyCode.A) || Input.GetButton("Y") ? (short) 1 : (short) 0; // Y
                case 2:
                    return Input.GetKey(KeyCode.Space) || Input.GetButton("SELECT") ? (short) 1 : (short) 0; // SELECT
                case 3:
                    return Input.GetKey(KeyCode.Return) || Input.GetButton("START") ? (short) 1 : (short) 0; // START
                case 4:
                    return Input.GetKey(KeyCode.UpArrow) || Input.GetAxisRaw("DpadX") >= 1.0f
                        ? (short) 1
                        : (short) 0; // UP
                case 5:
                    return Input.GetKey(KeyCode.DownArrow) || Input.GetAxisRaw("DpadX") <= -1.0f
                        ? (short) 1
                        : (short) 0; // DOWN
                case 6:
                    return Input.GetKey(KeyCode.LeftArrow) || Input.GetAxisRaw("DpadY") <= -1.0f
                        ? (short) 1
                        : (short) 0; // LEFT
                case 7:
                    return Input.GetKey(KeyCode.RightArrow) || Input.GetAxisRaw("DpadY") >= 1.0f
                        ? (short) 1
                        : (short) 0; // RIGHT
                case 8:
                    return Input.GetKey(KeyCode.X) || Input.GetButton("A") ? (short) 1 : (short) 0; // A
                case 9:
                    return Input.GetKey(KeyCode.S) || Input.GetButton("X") ? (short) 1 : (short) 0; // X
                case 10:
                    return Input.GetKey(KeyCode.Q) || Input.GetButton("L") ? (short) 1 : (short) 0; // L
                case 11:
                    return Input.GetKey(KeyCode.W) || Input.GetButton("R") ? (short) 1 : (short) 0; // R
                case 12:
                    return Input.GetKey(KeyCode.E) ? (short) 1 : (short) 0;
                case 13:
                    return Input.GetKey(KeyCode.R) ? (short) 1 : (short) 0;
                case 14:
                    return Input.GetKey(KeyCode.T) ? (short) 1 : (short) 0;
                case 15:
                    return Input.GetKey(KeyCode.Y) ? (short) 1 : (short) 0;
                default:
                    return 0;
            }
        }

        private unsafe bool RetroEnvironment(uint cmd, void* data)
        {
            switch (cmd)
            {
                case Environment.RetroEnvironmentGetOverscan:
                    break;
                case Environment.RetroEnvironmentGetVariable:
                    break;
                case Environment.RetroEnvironmentSetVariables:
                    break;
                case Environment.RetroEnvironmentSetMessage:
                    break;
                case Environment.RetroEnvironmentSetRotation:
                    break;
                case Environment.RetroEnvironmentShutdown:
                    break;
                case Environment.RetroEnvironmentSetPerformanceLevel:
                    break;
                case Environment.RetroEnvironmentGetSystemDirectory:
                    break;
                case Environment.RetroEnvironmentSetPixelFormat:
                    _pixelFormat = *(PixelFormat*) data;
                    switch (_pixelFormat)
                    {
                        case PixelFormat.RetroPixelFormat_0RGB1555:
                            break;
                        case PixelFormat.RetroPixelFormatRGB565:
                            break;
                        case PixelFormat.RetroPixelFormatXRGB8888:
                            break;
                        case PixelFormat.RetroPixelFormatUnknown:
                            break;
                    }

                    break;
                case Environment.RetroEnvironmentSetInputDescriptors:
                    break;
                case Environment.RetroEnvironmentSetKeyboardCallback:
                    break;
                default:
                    return false;
            }

            return true;
        }

        private static unsafe char* StringToChar(string s)
        {
            IntPtr p = Marshal.StringToHGlobalUni(s);
            return (char*) p.ToPointer();
        }

        private unsafe GameInfo LoadGameInfo(string file)
        {
            var gameInfo = new GameInfo();

            var stream = new FileStream(file, FileMode.Open);

            var data = new byte[stream.Length];
            stream.Read(data, 0, (int) stream.Length);
            IntPtr arrayPointer = Marshal.AllocHGlobal(data.Length * Marshal.SizeOf(typeof(byte)));
            Marshal.Copy(data, 0, arrayPointer, data.Length);


            gameInfo.path = StringToChar(file);
            gameInfo.size = (uint) data.Length;
            gameInfo.data = arrayPointer.ToPointer();

            stream.Close();

            return gameInfo;
        }

        public bool LoadGame(string gamePath)
        {
            GameInfo gameInfo = LoadGameInfo(gamePath);
            bool ret = Libretro.RetroLoadGame(ref gameInfo);

            Console.WriteLine("\nSystem information:");

            _av = new SystemAVInfo();
            Libretro.RetroGetSystemAVInfo(ref _av);

            Debug.Log("Geometry:");
            Debug.Log("Base width: " + _av.geometry.base_width);
            Debug.Log("Base height: " + _av.geometry.base_height);
            Debug.Log("Max width: " + _av.geometry.max_width);
            Debug.Log("Max height: " + _av.geometry.max_height);
            Debug.Log("Aspect ratio: " + _av.geometry.aspect_ratio);
            Debug.Log("Geometry:");
            Debug.Log("Target fps: " + _av.timing.fps);
            Debug.Log("Sample rate " + _av.timing.sample_rate);
            return ret;
        }
    }

    public unsafe class Libretro
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RetroApiVersionDelegate();

        public static RetroApiVersionDelegate RetroApiVersion;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroInitDelegate();

        public static RetroInitDelegate RetroInit;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroGetSystemInfoDelegate(ref SystemInfo info);

        public static RetroGetSystemInfoDelegate RetroGetSystemInfo;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroGetSystemAVInfoDelegate(ref SystemAVInfo info);

        public static RetroGetSystemAVInfoDelegate RetroGetSystemAVInfo;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool RetroLoadGameDelegate(ref GameInfo game);

        public static RetroLoadGameDelegate RetroLoadGame;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroSetVideoRefreshDelegate(RetroVideoRefreshDelegate r);

        public static RetroSetVideoRefreshDelegate RetroSetVideoRefresh;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroSetAudioSampleDelegate(RetroAudioSampleDelegate r);

        public static RetroSetAudioSampleDelegate RetroSetAudioSample;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroSetAudioSampleBatchDelegate(RetroAudioSampleBatchDelegate r);

        public static RetroSetAudioSampleBatchDelegate RetroSetAudioSampleBatch;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroSetInputPollDelegate(RetroInputPollDelegate r);

        public static RetroSetInputPollDelegate RetroSetInputPoll;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroSetInputStateDelegate(RetroInputStateDelegate r);

        public static RetroSetInputStateDelegate RetroSetInputState;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool RetroSetEnvironmentDelegate(RetroEnvironmentDelegate r);

        public static RetroSetEnvironmentDelegate RetroSetEnvironment;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroRunDelegate();

        public static RetroRunDelegate RetroRun;

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate void RetroDeInitDelegate();

        public static RetroDeInitDelegate RetroDeInit;

        //typedef void (*retro_video_refresh_t)(const void *data, unsigned width, unsigned height, size_t pitch);
        public delegate void RetroVideoRefreshDelegate(void* data, uint width, uint height, uint pitch);

        //typedef void (*retro_audio_sample_t)(int16_t left, int16_t right);
        public delegate void RetroAudioSampleDelegate(short left, short right);

        //typedef size_t (*retro_audio_sample_batch_t)(const int16_t *data, size_t frames);
        public delegate void RetroAudioSampleBatchDelegate(short* data, uint frames);

        //typedef void (*retro_input_poll_t)(void);
        public delegate void RetroInputPollDelegate();

        //typedef int16_t (*retro_input_state_t)(unsigned port, unsigned device, unsigned index, unsigned id);
        public delegate short RetroInputStateDelegate(uint port, uint device, uint index, uint id);

        //typedef bool (*retro_environment_t)(unsigned cmd, void *data);
        public delegate bool RetroEnvironmentDelegate(uint cmd, void* data);

        public static void InitializeLibrary(string dllName)
        {
            IDLLHandler dllHandler = null;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    dllHandler = WindowsDLLHandler.Instance;
#elif UNITY_ANDROID
	dllHandler = AndroidDLLHandler.Instance;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	dllHandler = OSXDLLHandler.Instance;
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
            dllHandler = LinuxDLLHandler.Instance;
#endif
            if (dllHandler == null) return;
            dllHandler.LoadCore(dllName);

            RetroApiVersion = dllHandler.GetMethod<RetroApiVersionDelegate>("retro_api_version");
            RetroInit = dllHandler.GetMethod<RetroInitDelegate>("retro_init");
            RetroGetSystemInfo = dllHandler.GetMethod<RetroGetSystemInfoDelegate>("retro_get_system_info");
            RetroGetSystemAVInfo = dllHandler.GetMethod<RetroGetSystemAVInfoDelegate>("retro_get_system_av_info");
            RetroLoadGame = dllHandler.GetMethod<RetroLoadGameDelegate>("retro_load_game");
            RetroSetVideoRefresh = dllHandler.GetMethod<RetroSetVideoRefreshDelegate>("retro_set_video_refresh");
            RetroSetAudioSample = dllHandler.GetMethod<RetroSetAudioSampleDelegate>("retro_set_audio_sample");
            RetroSetAudioSampleBatch =
                dllHandler.GetMethod<RetroSetAudioSampleBatchDelegate>("retro_set_audio_sample_batch");
            RetroSetInputPoll = dllHandler.GetMethod<RetroSetInputPollDelegate>("retro_set_input_poll");
            RetroSetInputState = dllHandler.GetMethod<RetroSetInputStateDelegate>("retro_set_input_state");
            RetroSetEnvironment = dllHandler.GetMethod<RetroSetEnvironmentDelegate>("retro_set_environment");
            RetroRun = dllHandler.GetMethod<RetroRunDelegate>("retro_run");
            RetroDeInit = dllHandler.GetMethod<RetroDeInitDelegate>("retro_deinit");
        }
    }
}
