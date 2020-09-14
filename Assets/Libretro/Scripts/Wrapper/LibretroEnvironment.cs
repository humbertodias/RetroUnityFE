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
using System.Runtime.InteropServices;
using static SK.Libretro.Utilities.StringUtils;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        //private const uint SUBSYSTEM_MAX_SUBSYSTEMS = 20;
        //private const uint SUBSYSTEM_MAX_SUBSYSTEM_ROMS = 10;

        //private readonly RetroSubsystemInfo[] subsystem_data = new RetroSubsystemInfo[SUBSYSTEM_MAX_SUBSYSTEMS];
        //private readonly unsafe RetroSubsystemRomInfo*[] subsystem_data_roms = new RetroSubsystemRomInfo*[SUBSYSTEM_MAX_SUBSYSTEMS];
        //private uint subsystem_current_count;

        [return: MarshalAs(UnmanagedType.U1)]
        public unsafe bool RetroEnvironmentCallback(retro_environment cmd, void* data)
        {
            switch (cmd)
            {
                /************************************************************************************************
                / Data passed from the frontend to the core
                /***********************************************************************************************/
                case retro_environment.RETRO_ENVIRONMENT_GET_OVERSCAN:
                {
                    bool* outOverscan = (bool*)data;
                    *outOverscan = !OptionCropOverscan;
                    Log.Info($"-> Overscan: {!OptionCropOverscan}", "RETRO_ENVIRONMENT_GET_OVERSCAN");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_CAN_DUPE:
                {
                    bool* outCanDupe = (bool*)data;
                    *outCanDupe = true;
                    Log.Info($"-> CanDupe: {*outCanDupe}", "RETRO_ENVIRONMENT_GET_CAN_DUPE");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY:
                {
                    char** outSystemDirectory = (char**)data;
                    string systemDirectory = FileSystem.GetAbsolutePath(SystemDirectory);
                    *outSystemDirectory = StringToChars(systemDirectory);
                    Log.Info($"-> SystemDirectory: {systemDirectory}", "RETRO_ENVIRONMENT_GET_SYSTEM_DIRECTORY");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_VARIABLE:
                {
                    retro_variable* outVariable = (retro_variable*)data;

                    string key = CharsToString(outVariable->key);
                    if (Core.CoreOptions != null)
                    {
                        string coreOption = Core.CoreOptions.Options.Find(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
                        if (coreOption != null)
                        {
                            outVariable->value = StringToChars(coreOption.Split(';')[1]);
                        }
                        else
                        {
                            Log.Warning($"Core option {key} not found!");
                            return false;
                        }
                    }
                    else
                    {
                        Log.Warning($"Core didn't set its options for key '{key}'.");
                        return false;
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_VARIABLE_UPDATE:
                {
                    bool* outVariableUpdate = (bool*)data;
                    *outVariableUpdate = false;
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_LIBRETRO_PATH:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_LIBRETRO_PATH");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_RUMBLE_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_INPUT_DEVICE_CAPABILITIES");
                    //ulong* outBitmask = (ulong*)data;
                    //*outBitmask       = (1 << (int)RetroDevice.RETRO_DEVICE_JOYPAD) | (1 << (int)RetroDevice.RETRO_DEVICE_ANALOG) | (1 << (int)RetroDevice.RETRO_DEVICE_KEYBOARD);
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_SENSOR_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_CAMERA_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_LOG_INTERFACE:
                {
                    retro_log_callback* outLogInterface = (retro_log_callback*)data;
                    outLogInterface->log = Core.GetLogCallback();
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_PERF_INTERFACE:
                {
                    retro_perf_callback* outPerfInterface = (retro_perf_callback*)data;
                    outPerfInterface->get_time_usec    = Core.GetPerfGetTimeUsecCallback();
                    outPerfInterface->get_cpu_features = Core.GetGetCPUFeaturesCallback();
                    outPerfInterface->get_perf_counter = Core.GetPerfGetCounterCallback();
                    outPerfInterface->perf_register    = Core.GetPerfRegisterCallback();
                    outPerfInterface->perf_start       = Core.GetPerfStartCallback();
                    outPerfInterface->perf_stop        = Core.GetPerfStopCallback();
                    outPerfInterface->perf_log         = Core.GetPerfLogCallback();
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_LOCATION_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY:
                {
                    char** outCoreAssetsDirectory = (char**)data;
                    string coreAssetsDirectory = FileSystem.GetAbsolutePath(SystemDirectory);
                    *outCoreAssetsDirectory = StringToChars(coreAssetsDirectory);
                    Log.Info($"-> CoreAssetsDirectory: {coreAssetsDirectory}", "RETRO_ENVIRONMENT_GET_CORE_ASSETS_DIRECTORY");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY:
                {
                    char** outSaveDirectory = (char**)data;
                    string saveDirectory = FileSystem.GetAbsolutePath(SavesDirectory);
                    *outSaveDirectory = StringToChars(saveDirectory);
                    Log.Info($"-> SaveDirectory: {saveDirectory}", "RETRO_ENVIRONMENT_GET_SAVE_DIRECTORY");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_USERNAME:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_USERNAME");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_LANGUAGE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_LANGUAGE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_CURRENT_SOFTWARE_FRAMEBUFFER");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_HW_RENDER_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_VFS_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_VFS_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_LED_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_LED_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_AUDIO_VIDEO_ENABLE:
                {
                    int result = 0;
                    result |= 1; // if video enabled
                    result |= 2; // if audio enabled

                    int* outAudioVideoEnabled = (int*)data;
                    *outAudioVideoEnabled = result;
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_MIDI_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_MIDI_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_FASTFORWARDING:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_FASTFORWARDING");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_TARGET_REFRESH_RATE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_INPUT_BITMASKS:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_INPUT_BITMASKS");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION:
                {
                    uint* outVersion = (uint*)data;
                    *outVersion = RETRO_API_VERSION;
                    Log.Info($"-> Version: {RETRO_API_VERSION}", "RETRO_ENVIRONMENT_GET_CORE_OPTIONS_VERSION");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_GET_PREFERRED_HW_RENDER:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_PREFERRED_HW_RENDER");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_GET_DISK_CONTROL_INTERFACE_VERSION:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_GET_DISK_CONTROL_INTERFACE_VERSION");
                    return false;
                }

                /************************************************************************************************
                / Data passed from the core to the frontend
                /***********************************************************************************************/
                case retro_environment.RETRO_ENVIRONMENT_SET_ROTATION:
                {
                    // TODO: Rotate screen (counter-clockwise)
                    // Values: 0,  1,   2,   3
                    // Result: 0, 90, 180, 270 degrees
                    uint* inRotation = (uint*)data;
                    Core.Rotation = (int)*inRotation;
                    Log.Warning($"<- Rotation: {*inRotation}", "RETRO_ENVIRONMENT_SET_ROTATION");
                    return false;
                }
                //break;
                case retro_environment.RETRO_ENVIRONMENT_SET_MESSAGE:
                {
                    // TODO: Do I need something from this?
                    retro_message* inMessage = (retro_message*)data;
                    Log.Warning($"<- Message: {CharsToString(inMessage->msg)}", "RETRO_ENVIRONMENT_SET_MESSAGE");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL:
                {
                    int* inPerformanceLevel = (int*)data;
                    Core.PerformanceLevel = *inPerformanceLevel;
                    Log.Warning($"Environment not fully implemented!", "RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL");
                    Log.Warning($"<- PerformanceLevel: {Core.PerformanceLevel}", "RETRO_ENVIRONMENT_SET_PERFORMANCE_LEVEL");
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_PIXEL_FORMAT:
                {
                    retro_pixel_format* inPixelFormat = (retro_pixel_format*)data;
                    switch (*inPixelFormat)
                    {
                        case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                        case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                        case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                        {
                            Game.PixelFormat = *inPixelFormat;
                            Log.Info($"[IN] PixelFormat: {*inPixelFormat}", "RETRO_ENVIRONMENT_SET_PIXEL_FORMAT");
                        }
                        break;
                        default:
                            return false;
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_INPUT_DESCRIPTORS:
                {
                    retro_input_descriptor* inInputDescriptors = (retro_input_descriptor*)data;
                    uint id;
                    for (; inInputDescriptors->desc != null; ++inInputDescriptors)
                    {
                        uint port = inInputDescriptors->port;
                        if (port >= MAX_USERS)
                        {
                            continue;
                        }

                        retro_device device = (retro_device)inInputDescriptors->device;
                        if (device != retro_device.RETRO_DEVICE_JOYPAD && device != retro_device.RETRO_DEVICE_ANALOG)
                        {
                            continue;
                        }

                        id = inInputDescriptors->id;
                        if (id >= FIRST_CUSTOM_BIND)
                        {
                            continue;
                        }

                        string descText = CharsToString(inInputDescriptors->desc);
                        retro_device_index_analog index = (retro_device_index_analog)inInputDescriptors->index;
                        if (device == retro_device.RETRO_DEVICE_ANALOG)
                        {
                            retro_device_id_analog idAnalog = (retro_device_id_analog)id;
                            switch (idAnalog)
                            {
                                case retro_device_id_analog.RETRO_DEVICE_ID_ANALOG_X:
                                {
                                    switch (index)
                                    {
                                        case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_LEFT:
                                        {
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_X_PLUS] = descText;
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_X_MINUS] = descText;
                                        }
                                        break;
                                        case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_RIGHT:
                                        {
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_X_PLUS] = descText;
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_X_MINUS] = descText;
                                        }
                                        break;
                                    }
                                }
                                break;
                                case retro_device_id_analog.RETRO_DEVICE_ID_ANALOG_Y:
                                {
                                    switch (index)
                                    {
                                        case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_LEFT:
                                        {
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_Y_PLUS] = descText;
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_LEFT_Y_MINUS] = descText;
                                        }
                                        break;
                                        case retro_device_index_analog.RETRO_DEVICE_INDEX_ANALOG_RIGHT:
                                        {
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_Y_PLUS] = descText;
                                            Core.ButtonDescriptions[port, (int)CustomBinds.ANALOG_RIGHT_Y_MINUS] = descText;
                                        }
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        else
                        {
                            Core.ButtonDescriptions[port, id] = descText;
                        }
                    }

                    Core.HasInputDescriptors = true;
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_KEYBOARD_CALLBACK");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_DISK_CONTROL_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_HW_RENDER:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_HW_RENDER");
                    retro_hw_render_callback* inHwRenderCallback = (retro_hw_render_callback*)data;
                    Log.Error($"Context type: {inHwRenderCallback->context_type}", "RETRO_ENVIRONMENT_SET_HW_RENDER");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_VARIABLES:
                {
                    try
                    {
                        retro_variable* inVariable = (retro_variable*)data;

                        Core.CoreOptions = _coreOptionsList.Cores.Find(x => x.CoreName.Equals(Core.CoreName, StringComparison.OrdinalIgnoreCase));
                        if (Core.CoreOptions == null)
                        {
                            Core.CoreOptions = new CoreOptions { CoreName = Core.CoreName };
                            _coreOptionsList.Cores.Add(Core.CoreOptions);
                        }

                        while (inVariable->key != null)
                        {
                            string key = CharsToString(inVariable->key);
                            string coreOption = Core.CoreOptions.Options.Find(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
                            if (coreOption == null)
                            {
                                string inValue = CharsToString(inVariable->value);
                                string[] descriptionAndValues = inValue.Split(';');
                                string[] possibleValues = descriptionAndValues[1].Trim().Split('|');
                                string defaultValue = possibleValues[0];
                                string value = defaultValue;
                                coreOption = $"{key};{value};{string.Join("|", possibleValues)};";
                                Core.CoreOptions.Options.Add(coreOption);
                            }
                            ++inVariable;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e);
                    }

                    SaveCoreOptionsFile();
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_NO_GAME:
                {
                    bool* inSupportNoGame = (bool*)data;
                    Core.SupportNoGame = *inSupportNoGame;
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_FRAME_TIME_CALLBACK:
                {
                    retro_frame_time_callback* inFrameTimeCallback = (retro_frame_time_callback*)data;
                    Core.SetFrameTimeCallback(inFrameTimeCallback->callback, inFrameTimeCallback->reference);
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_AUDIO_CALLBACK");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_SYSTEM_AV_INFO:
                {
                    retro_system_av_info* inSystemAVnfo = (retro_system_av_info*)data;
                    Game.SystemAVInfo = *inSystemAVnfo;
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_PROC_ADDRESS_CALLBACK");
                    return false;
                }
                //case retro_environment.RETRO_ENVIRONMENT_SET_SUBSYSTEM_INFO:
                //{
                //    //RetroSubsystemInfo* subsytemInfo = (RetroSubsystemInfo*)data;
                //    ////Debug.Log("<color=yellow>Subsystem Info:</color>");
                //    ////Debug.Log($"<color=yellow>Description:</color> {Marshal.PtrToStringAnsisubsytemInfo->desc)}");
                //    ////Debug.Log($"<color=yellow>Ident:</color> {Marshal.PtrToStringAnsisubsytemInfo->ident)}");
                //    //_game_type = subsytemInfo->id;
                //    //_num_info = subsytemInfo->num_roms;
                //    //while (subsytemInfo->roms != null)
                //    //{
                //    //    RetroSubsystemRomInfo* romInfo = subsytemInfo->roms;
                //    //    //Debug.Log("<color=orange>Rom Info:</color>");
                //    //    //Debug.Log($"<color=orange>Description:</color> {Marshal.PtrToStringAnsiromInfo->desc)}");
                //    //    //Debug.Log($"<color=orange>Extensions:</color> {Marshal.PtrToStringAnsiromInfo->valid_extensions)}");
                //    //    subsytemInfo++;
                //    //}

                //    RetroSubsystemInfo* inSubsytemInfo = (RetroSubsystemInfo*)data;
                //    // settings_t* settings = configuration_settings;
                //    // unsigned log_level = settings->uints.frontend_log_level;

                //    subsystem_current_count = 0;

                //    uint size = 0;
                //    Log.Info("SET_SUBSYSTEM_INFO", "Environment");
                //    {
                //        uint i = 0;
                //        while (inSubsytemInfo[i].ident != null)
                //        {
                //            string subsystemDesc = Marshal.PtrToStringAnsiinSubsytemInfo[i].desc);
                //            string subsystemIdent = Marshal.PtrToStringAnsiinSubsytemInfo[i].ident);
                //            uint subsystemId = inSubsytemInfo[i].id;

                //            Log.Info($"Subsystem ID: {i}");
                //            Log.Info($"Special game type: {subsystemDesc}\n  Ident: {subsystemIdent}\n  ID: {subsystemId}\n  Content:");
                //            for (uint j = 0; j < inSubsytemInfo[i].num_roms; j++)
                //            {
                //                string romDesc = Marshal.PtrToStringAnsiinSubsytemInfo[i].roms[j].desc);
                //                string required = inSubsytemInfo[i].roms[j].required ? "required" : "optional";
                //                Log.Info($"    {romDesc} ({required})");
                //            }
                //            i++;
                //        }

                //        //if (log_level == RETRO_LOG_DEBUG)
                //        Log.Info($"Subsystems: {i}");
                //        size = i;
                //    }
                //    //if (log_level == RETRO_LOG_DEBUG)
                //    if (size > SUBSYSTEM_MAX_SUBSYSTEMS)
                //    {
                //        Log.Warning($"Subsystems exceed subsystem max, clamping to {SUBSYSTEM_MAX_SUBSYSTEMS}");
                //    }

                //    if (Core != null)
                //    {
                //        for (uint i = 0; i < size && i < SUBSYSTEM_MAX_SUBSYSTEMS; i++)
                //        {
                //            ref RetroSubsystemInfo subdata = ref subsystem_data[i];

                //            subdata.desc = inSubsytemInfo[i].desc;
                //            subdata.ident = inSubsytemInfo[i].ident;
                //            subdata.id = inSubsytemInfo[i].id;
                //            subdata.num_roms = inSubsytemInfo[i].num_roms;

                //            //if (log_level == RETRO_LOG_DEBUG)
                //            if (subdata.num_roms > SUBSYSTEM_MAX_SUBSYSTEM_ROMS)
                //            {
                //                Log.Warning($"Subsystems exceed subsystem max roms, clamping to {SUBSYSTEM_MAX_SUBSYSTEM_ROMS}");
                //            }

                //            for (uint j = 0; j < subdata.num_roms && j < SUBSYSTEM_MAX_SUBSYSTEM_ROMS; j++)
                //            {
                //                while (subdata.roms != null)
                //                {
                //                    RetroSubsystemRomInfo* romInfo = subdata.roms;
                //                    romInfo->desc = inSubsytemInfo[i].roms[j].desc;
                //                    romInfo->valid_extensions = inSubsytemInfo[i].roms[j].valid_extensions;
                //                    romInfo->required = inSubsytemInfo[i].roms[j].required;
                //                    romInfo->block_extract = inSubsytemInfo[i].roms[j].block_extract;
                //                    romInfo->need_fullpath = inSubsytemInfo[i].roms[j].need_fullpath;
                //                    subdata.roms++;
                //                }
                //            }

                //            subdata.roms = subsystem_data_roms[i];
                //        }

                //        subsystem_current_count = (size <= SUBSYSTEM_MAX_SUBSYSTEMS) ? size : SUBSYSTEM_MAX_SUBSYSTEMS;
                //    }
                //    return false; //TODO: Remove when implemented!
                //}
                //break;
                case retro_environment.RETRO_ENVIRONMENT_SET_CONTROLLER_INFO:
                {
                    retro_controller_info* inControllerInfo = (retro_controller_info*)data;

                    int numPorts;
                    for (numPorts = 0; inControllerInfo[numPorts].types != null; ++numPorts)
                    {
                        Log.Info($"# Controller port: {numPorts + 1}", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                        for (int j = 0; j < inControllerInfo[numPorts].num_types; ++j)
                        {
                            string desc = CharsToString(inControllerInfo[numPorts].types[j].desc);
                            uint id = inControllerInfo[numPorts].types[j].id;
                            Log.Info($"    {desc} (ID: {id})", "RETRO_ENVIRONMENT_SET_CONTROLLER_INFO");
                        }
                    }

                    Core.ControllerPorts = new retro_controller_info[numPorts];
                    for (int j = 0; j < numPorts; ++j)
                    {
                        Core.ControllerPorts[j] = inControllerInfo[j];
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_MEMORY_MAPS:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_MEMORY_MAPS");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_GEOMETRY:
                {
                    if (Game.Running)
                    {
                        retro_game_geometry* inGeometry = (retro_game_geometry*)data;
                        if (Game.SystemAVInfo.geometry.base_width != inGeometry->base_width
                            || Game.SystemAVInfo.geometry.base_height != inGeometry->base_height
                            || Game.SystemAVInfo.geometry.aspect_ratio != inGeometry->aspect_ratio)
                        {
                            Game.SystemAVInfo.geometry = *inGeometry;
                            // TODO: Set video aspect ratio
                        }
                    }
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_SUPPORT_ACHIEVEMENTS");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_SERIALIZATION_QUIRKS");
                    ulong* quirk = (ulong*)data;
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_HW_SHARED_CONTEXT");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_CORE_OPTIONS");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_INTL:
                {
                    retro_core_options_intl inOptionsIntl = Marshal.PtrToStructure<retro_core_options_intl>((IntPtr)data);

                    Core.CoreOptions = _coreOptionsList.Cores.Find(x => x.CoreName.Equals(Core.CoreName, StringComparison.OrdinalIgnoreCase));
                    if (Core.CoreOptions == null)
                    {
                        Core.CoreOptions = new CoreOptions { CoreName = Core.CoreName };
                        _coreOptionsList.Cores.Add(Core.CoreOptions);
                    }

                    for (int i = 0; i < RETRO_NUM_CORE_OPTION_VALUES_MAX; i++)
                    {
                        IntPtr ins = new IntPtr(inOptionsIntl.us.ToInt64() + i * Marshal.SizeOf<retro_core_option_definition>());
                        retro_core_option_definition defs = Marshal.PtrToStructure<retro_core_option_definition>(ins);
                        if (defs.key == null)
                        {
                            break;
                        }

                        string key = CharsToString(defs.key);

                        string coreOption = Core.CoreOptions.Options.Find(x => x.StartsWith(key, StringComparison.OrdinalIgnoreCase));
                        if (coreOption == null)
                        {
                            string defaultValue = CharsToString(defs.default_value);

                            List<string> possibleValues = new List<string>();
                            for (int j = 0; j < defs.values.Length; j++)
                            {
                                retro_core_option_value val = defs.values[j];
                                if (val.value != null)
                                {
                                    possibleValues.Add(CharsToString(val.value));
                                }
                            }

                            string value = string.Empty;
                            if (!string.IsNullOrEmpty(defaultValue))
                            {
                                value = defaultValue;
                            }
                            else if (possibleValues.Count > 0)
                            {
                                value = possibleValues[0];
                            }

                            coreOption = $"{key};{value};{string.Join("|", possibleValues)}";

                            Core.CoreOptions.Options.Add(coreOption);
                        }
                    }

                    SaveCoreOptionsFile();
                }
                break;
                case retro_environment.RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_CORE_OPTIONS_DISPLAY");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SET_DISK_CONTROL_EXT_INTERFACE:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SET_DISK_CONTROL_EXT_INTERFACE");
                    return false;
                }
                case retro_environment.RETRO_ENVIRONMENT_SHUTDOWN:
                {
                    Log.Error("Environment not implemented!", "RETRO_ENVIRONMENT_SHUTDOWN");
                    return false;
                }
                default:
                {
                    Log.Error($"Environment unknown: {cmd}", "RetroEnvironmentCallback");
                    return false;
                }
            }

            return true;
        }
    }
}
