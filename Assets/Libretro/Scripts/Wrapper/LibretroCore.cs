/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using SK.Libretro.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using static SK.Libretro.Wrapper;
using static SK.Libretro.Utilities.StringUtils;

namespace SK.Libretro
{
    [Serializable]
    public class CoreOptions
    {
        public string CoreName = string.Empty;
        public List<string> Options = new List<string>();
    }

    [Serializable]
    public class CoreOptionsList
    {
        public List<CoreOptions> Cores = new List<CoreOptions>();
    }

    public class LibretroCore
    {
        #region Dynamically loaded function pointers
        public retro_set_environment_t retro_set_environment;
        public retro_set_video_refresh_t retro_set_video_refresh;
        public retro_set_audio_sample_t retro_set_audio_sample;
        public retro_set_audio_sample_batch_t retro_set_audio_sample_batch;
        public retro_set_input_poll_t retro_set_input_poll;
        public retro_set_input_state_t retro_set_input_state;
        public retro_init_t retro_init;
        public retro_deinit_t retro_deinit;
        public retro_api_version_t retro_api_version;
        public retro_get_system_info_t retro_get_system_info;
        public retro_get_system_av_info_t retro_get_system_av_info;
        public retro_set_controller_port_device_t retro_set_controller_port_device;
        public retro_reset_t retro_reset;
        public retro_run_t retro_run;
        public retro_serialize_size_t retro_serialize_size;
        public retro_serialize_t retro_serialize;
        public retro_unserialize_t retro_unserialize;
        public retro_cheat_reset_t retro_cheat_reset;
        public retro_cheat_set_t retro_cheat_set;
        public retro_load_game_t retro_load_game;
        public retro_load_game_special_t retro_load_game_special;
        public retro_unload_game_t retro_unload_game;
        public retro_get_region_t retro_get_region;
        public retro_get_memory_data_t retro_get_memory_data;
        public retro_get_memory_size_t retro_get_memory_size;
        #endregion

        public bool Initialized { get; private set; }

        public int ApiVersion { get; private set; }

        public string CoreName { get; private set; }
        public string CoreVersion { get; private set; }
        public string[] ValidExtensions { get; private set; }
        public bool NeedFullPath { get; private set; }
        public bool BlockExtract { get; private set; }

        public int Rotation;
        public int PerformanceLevel;
        public bool SupportNoGame;

        public string[,] ButtonDescriptions = new string[MAX_USERS, FIRST_META_KEY];
        public bool HasInputDescriptors;

        public CoreOptions CoreOptions;

        public retro_controller_info[] ControllerPorts;

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        private readonly DllModule _dll = new DllModuleWindows();
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        private readonly DllModule _dll = new DllModuleMacOS();
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
        private readonly DllModule _dll = new DllModuleLinux();
#else
#endif

        private retro_environment_t _environmentCallback;
        private retro_video_refresh_t _videoRefreshCallback;
        private retro_audio_sample_t _audioSampleCallback;
        private retro_audio_sample_batch_t _audioSampleBatchCallback;
        private retro_input_poll_t _inputPollCallback;
        private retro_input_state_t _inputStateCallback;
        private retro_log_printf_t _logPrintfCallback;
        private retro_perf_get_time_usec_t _perfGetTimeUsecCallback;
        private retro_perf_get_counter_t _perfGetCounterCallback;
        private retro_get_cpu_features_t _getCPUFeaturesCallback;
        private retro_perf_log_t _perfLogCallback;
        private retro_perf_register_t _perfRegisterCallback;
        private retro_perf_start_t _perfStartCallback;
        private retro_perf_stop_t _perfStopCallback;

        public unsafe bool Start(Wrapper wrapper, string coreName)
        {
            bool result = false;

            try
            {
                string extension;
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                extension = ".dll";
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        extension = ".dylib";
#elif UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX
        extension = ".so";
#else
#endif

        string corePath = FileSystem.GetAbsolutePath($"{CoresDirectory}/{coreName}_libretro{extension}");
                if (FileSystem.FileExists(corePath))
                {
                    string tempDirectory = FileSystem.GetAbsolutePath(TempDirectory);
                    if (!Directory.Exists(tempDirectory))
                    {
                        _ = Directory.CreateDirectory(tempDirectory);
                    }

                    string instancePath = Path.Combine(tempDirectory, $"{coreName}_{Guid.NewGuid()}{extension}");
                    File.Copy(corePath, instancePath);

                    _dll.Load(instancePath);

                    GetCoreFunctions();

                    ApiVersion = retro_api_version();

                    SetCallbacks(wrapper);

                    retro_system_info systemInfo = new retro_system_info();
                    retro_get_system_info(ref systemInfo);

                    CoreName = CharsToString(systemInfo.library_name);
                    CoreVersion = CharsToString(systemInfo.library_version);
                    if (systemInfo.valid_extensions != null)
                    {
                        ValidExtensions = CharsToString(systemInfo.valid_extensions).Split('|');
                    }
                    NeedFullPath = systemInfo.need_fullpath;
                    BlockExtract = systemInfo.block_extract;

                    retro_set_environment(_environmentCallback);
                    retro_init();
                    retro_set_video_refresh(_videoRefreshCallback);
                    retro_set_audio_sample(_audioSampleCallback);
                    retro_set_audio_sample_batch(_audioSampleBatchCallback);
                    retro_set_input_poll(_inputPollCallback);
                    retro_set_input_state(_inputStateCallback);

                    Initialized = true;
                    result = true;
                }
                else
                {
                    Log.Error($"Core '{coreName}' at path '{corePath}' not found.", "Libretro.LibretroCore.Start");
                }
            }
            catch (Exception e )
            {
                Log.Exception(e, "Libretro.LibretroCore.Start");
            }

            return result;
        }

        public void Stop()
        {
            try
            {
                if (Initialized)
                {
                    retro_deinit();
                    Initialized = false;
                }

                _dll.Free();

                string dllPath = FileSystem.GetAbsolutePath($"{TempDirectory}/{_dll.Name}");
                if (File.Exists(dllPath))
                {
                    File.Delete(dllPath);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e, "Libretro.LibretroCore.Stop");
            }
        }

        private void GetCoreFunctions()
        {
            try
            {
                retro_set_environment            = _dll.GetFunction<retro_set_environment_t>("retro_set_environment");
                retro_set_video_refresh          = _dll.GetFunction<retro_set_video_refresh_t>("retro_set_video_refresh");
                retro_set_audio_sample           = _dll.GetFunction<retro_set_audio_sample_t>("retro_set_audio_sample");
                retro_set_audio_sample_batch     = _dll.GetFunction<retro_set_audio_sample_batch_t>("retro_set_audio_sample_batch");
                retro_set_input_poll             = _dll.GetFunction<retro_set_input_poll_t>("retro_set_input_poll");
                retro_set_input_state            = _dll.GetFunction<retro_set_input_state_t>("retro_set_input_state");
                retro_init                       = _dll.GetFunction<retro_init_t>("retro_init");
                retro_deinit                     = _dll.GetFunction<retro_deinit_t>("retro_deinit");
                retro_api_version                = _dll.GetFunction<retro_api_version_t>("retro_api_version");
                retro_get_system_info            = _dll.GetFunction<retro_get_system_info_t>("retro_get_system_info");
                retro_get_system_av_info         = _dll.GetFunction<retro_get_system_av_info_t>("retro_get_system_av_info");
                retro_set_controller_port_device = _dll.GetFunction<retro_set_controller_port_device_t>("retro_set_controller_port_device");
                retro_reset                      = _dll.GetFunction<retro_reset_t>("retro_reset");
                retro_run                        = _dll.GetFunction<retro_run_t>("retro_run");
                retro_serialize_size             = _dll.GetFunction<retro_serialize_size_t>("retro_serialize_size");
                retro_serialize                  = _dll.GetFunction<retro_serialize_t>("retro_serialize");
                retro_unserialize                = _dll.GetFunction<retro_unserialize_t>("retro_unserialize");
                retro_cheat_reset                = _dll.GetFunction<retro_cheat_reset_t>("retro_cheat_reset");
                retro_cheat_set                  = _dll.GetFunction<retro_cheat_set_t>("retro_cheat_set");
                retro_load_game                  = _dll.GetFunction<retro_load_game_t>("retro_load_game");
                retro_load_game_special          = _dll.GetFunction<retro_load_game_special_t>("retro_load_game_special");
                retro_unload_game                = _dll.GetFunction<retro_unload_game_t>("retro_unload_game");
                retro_get_region                 = _dll.GetFunction<retro_get_region_t>("retro_get_region");
                retro_get_memory_data            = _dll.GetFunction<retro_get_memory_data_t>("retro_get_memory_data");
                retro_get_memory_size            = _dll.GetFunction<retro_get_memory_size_t>("retro_get_memory_size");
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private unsafe void SetCallbacks(Wrapper wrapper)
        {
            _environmentCallback      = wrapper.RetroEnvironmentCallback;
            _videoRefreshCallback     = wrapper.RetroVideoRefreshCallback;
            _audioSampleCallback      = wrapper.RetroAudioSampleCallback;
            _audioSampleBatchCallback = wrapper.RetroAudioSampleBatchCallback;
            _inputPollCallback        = wrapper.RetroInputPollCallback;
            _inputStateCallback       = wrapper.RetroInputStateCallback;
            _logPrintfCallback        = wrapper.RetroLogPrintf;
            _perfGetTimeUsecCallback  = wrapper.RetroPerfGetTimeUsec;
            _perfGetCounterCallback   = wrapper.RetroPerfGetCounter;
            _getCPUFeaturesCallback   = wrapper.RetroGetCPUFeatures;
            _perfLogCallback          = wrapper.RetroPerfLog;
            _perfRegisterCallback     = wrapper.RetroPerfRegister;
            _perfStartCallback        = wrapper.RetroPerfStart;
            _perfStopCallback         = wrapper.RetroPerfStop;
        }

        public IntPtr GetLogCallback() => Marshal.GetFunctionPointerForDelegate(_logPrintfCallback);

        public IntPtr GetPerfGetTimeUsecCallback() => Marshal.GetFunctionPointerForDelegate(_perfGetTimeUsecCallback);
        public IntPtr GetPerfGetCounterCallback()  => Marshal.GetFunctionPointerForDelegate(_perfGetCounterCallback);
        public IntPtr GetGetCPUFeaturesCallback()  => Marshal.GetFunctionPointerForDelegate(_getCPUFeaturesCallback);
        public IntPtr GetPerfLogCallback()         => Marshal.GetFunctionPointerForDelegate(_perfLogCallback);
        public IntPtr GetPerfRegisterCallback()    => Marshal.GetFunctionPointerForDelegate(_perfRegisterCallback);
        public IntPtr GetPerfStartCallback()       => Marshal.GetFunctionPointerForDelegate(_perfStartCallback);
        public IntPtr GetPerfStopCallback()        => Marshal.GetFunctionPointerForDelegate(_perfStopCallback);

        public void SetFrameTimeCallback(IntPtr callback, long reference)
        {
            _ = callback;
            _ = reference;
            Log.Warning("Not Implemented", "Libretro.LibretroCore.SetFrameTimeCallback");
        }
    }
}
