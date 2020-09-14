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

using UnityEngine;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public const int MAX_USERS = 16;

        private const int FIRST_CUSTOM_BIND = 16;
        private const int FIRST_LIGHTGUN_BIND = (int)CustomBinds.ANALOG_BIND_LIST_END;
        private const int FIRST_MISC_CUSTOM_BIND = (int)CustomBinds.LIGHTGUN_BIND_LIST_END;
        public const int FIRST_META_KEY = (int)CustomBinds.CUSTOM_BIND_LIST_END;

        private enum CustomBinds
        {
            // Analogs (RETRO_DEVICE_ANALOG)
            ANALOG_LEFT_X_PLUS = FIRST_CUSTOM_BIND,
            ANALOG_LEFT_X_MINUS,
            ANALOG_LEFT_Y_PLUS,
            ANALOG_LEFT_Y_MINUS,
            ANALOG_RIGHT_X_PLUS,
            ANALOG_RIGHT_X_MINUS,
            ANALOG_RIGHT_Y_PLUS,
            ANALOG_RIGHT_Y_MINUS,
            ANALOG_BIND_LIST_END,

            // Lightgun
            LIGHTGUN_TRIGGER = FIRST_LIGHTGUN_BIND,
            LIGHTGUN_RELOAD,
            LIGHTGUN_AUX_A,
            LIGHTGUN_AUX_B,
            LIGHTGUN_AUX_C,
            LIGHTGUN_START,
            LIGHTGUN_SELECT,
            LIGHTGUN_DPAD_UP,
            LIGHTGUN_DPAD_DOWN,
            LIGHTGUN_DPAD_LEFT,
            LIGHTGUN_DPAD_RIGHT,
            LIGHTGUN_BIND_LIST_END,

            // Turbo
            TURBO_ENABLE = FIRST_MISC_CUSTOM_BIND,

            CUSTOM_BIND_LIST_END,

            // Command binds. Not related to game input, only usable for port 0.
            FAST_FORWARD_KEY = FIRST_META_KEY,
            FAST_FORWARD_HOLD_KEY,
            SLOWMOTION_KEY,
            SLOWMOTION_HOLD_KEY,
            LOAD_STATE_KEY,
            SAVE_STATE_KEY,
            FULLSCREEN_TOGGLE_KEY,
            QUIT_KEY,
            STATE_SLOT_PLUS,
            STATE_SLOT_MINUS,
            REWIND,
            BSV_RECORD_TOGGLE,
            PAUSE_TOGGLE,
            FRAMEADVANCE,
            RESET,
            SHADER_NEXT,
            SHADER_PREV,
            CHEAT_INDEX_PLUS,
            CHEAT_INDEX_MINUS,
            CHEAT_TOGGLE,
            SCREENSHOT,
            MUTE,
            OSK,
            FPS_TOGGLE,
            SEND_DEBUG_INFO,
            NETPLAY_HOST_TOGGLE,
            NETPLAY_GAME_WATCH,
            ENABLE_HOTKEY,
            VOLUME_UP,
            VOLUME_DOWN,
            OVERLAY_NEXT,
            DISK_EJECT_TOGGLE,
            DISK_NEXT,
            DISK_PREV,
            GRAB_MOUSE_TOGGLE,
            GAME_FOCUS_TOGGLE,
            UI_COMPANION_TOGGLE,

            MENU_TOGGLE,

            RECORDING_TOGGLE,
            STREAMING_TOGGLE,

            AI_SERVICE,

            BIND_LIST_END,
            BIND_LIST_END_NULL
        };

        public void RetroInputPollCallback()
        {
        }

        public short RetroInputStateCallback(uint port, retro_device device, uint _/*index*/, uint id)
        {
            short result = 0;

            if (InputProcessor != null)
            {
                switch (device)
                {
                    case retro_device.RETRO_DEVICE_JOYPAD:
                    {
                        if (id < (int)retro_device_id_joypad.RETRO_DEVICE_ID_JOYPAD_END)
                        {
                            result = ProcessJoypadDeviceState((int)port, (int)id);
                        }
                        else
                        {
                            Debug.Log("OPS");
                        }
                    }
                    break;
                    case retro_device.RETRO_DEVICE_MOUSE:
                    {
                        if (id < (int)retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_END)
                        {
                            result = ProcessMouseDeviceState((int)port, (retro_device_id_mouse)id);
                        }
                    }
                    break;
                    case retro_device.RETRO_DEVICE_KEYBOARD:
                    {
                        result = BoolToShort(id < (int)retro_key.RETROK_OEM_102 ? Input.GetKey((KeyCode)id) : false);
                    }
                    break;
                    case retro_device.RETRO_DEVICE_LIGHTGUN:
                        break;
                    case retro_device.RETRO_DEVICE_ANALOG:
                        break;
                    case retro_device.RETRO_DEVICE_POINTER:
                        break;
                    default:
                        break;
                }
            }

            return result;
        }

        private short ProcessJoypadDeviceState(int port, int button)
        {
            return BoolToShort(InputProcessor.JoypadButton(port, button));
        }

        private short ProcessMouseDeviceState(int port, retro_device_id_mouse command)
        {
            short result = 0;

            switch (command)
            {
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_X:
                {
                    result = FloatToShort(InputProcessor.MouseDelta(port, 0));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_Y:
                {
                    result = FloatToShort(InputProcessor.MouseDelta(port, 1));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_LEFT:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 0));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_RIGHT:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 1));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELUP:
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_WHEELDOWN:
                {
                    result = FloatToShort(InputProcessor.MouseWheelDelta(port, 0));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_MIDDLE:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 2));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELUP:
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_HORIZ_WHEELDOWN:
                {
                    result = FloatToShort(InputProcessor.MouseWheelDelta(port, 1));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_4:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 3));
                }
                break;
                case retro_device_id_mouse.RETRO_DEVICE_ID_MOUSE_BUTTON_5:
                {
                    result = BoolToShort(InputProcessor.MouseButton(port, 4));
                }
                break;
                default:
                    break;
            }

            return result;
        }

        private static short BoolToShort(bool boolValue)
        {
            return (short)(boolValue ? 1 : 0);
        }

        private static short FloatToShort(float floatValue)
        {
            return (short)Mathf.Clamp(Mathf.Round(floatValue), short.MinValue, short.MaxValue);
        }
    }
}
