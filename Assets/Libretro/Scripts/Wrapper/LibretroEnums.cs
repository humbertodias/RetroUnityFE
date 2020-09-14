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

namespace SK.Libretro
{
    public partial class Wrapper
    {
        private enum retro_language
        {
            RETRO_LANGUAGE_ENGLISH             = 0,
            RETRO_LANGUAGE_JAPANESE            = 1,
            RETRO_LANGUAGE_FRENCH              = 2,
            RETRO_LANGUAGE_SPANISH             = 3,
            RETRO_LANGUAGE_GERMAN              = 4,
            RETRO_LANGUAGE_ITALIAN             = 5,
            RETRO_LANGUAGE_DUTCH               = 6,
            RETRO_LANGUAGE_PORTUGUESE_BRAZIL   = 7,
            RETRO_LANGUAGE_PORTUGUESE_PORTUGAL = 8,
            RETRO_LANGUAGE_RUSSIAN             = 9,
            RETRO_LANGUAGE_KOREAN              = 10,
            RETRO_LANGUAGE_CHINESE_TRADITIONAL = 11,
            RETRO_LANGUAGE_CHINESE_SIMPLIFIED  = 12,
            RETRO_LANGUAGE_ESPERANTO           = 13,
            RETRO_LANGUAGE_POLISH              = 14,
            RETRO_LANGUAGE_VIETNAMESE          = 15,
            RETRO_LANGUAGE_ARABIC              = 16,
            RETRO_LANGUAGE_GREEK               = 17,
            RETRO_LANGUAGE_TURKISH             = 18,
            RETRO_LANGUAGE_LAST
        };

        private enum retro_key
        {
            RETROK_UNKNOWN = 0,
            RETROK_FIRST = 0,
            RETROK_BACKSPACE = 8,
            RETROK_TAB = 9,
            RETROK_CLEAR = 12,
            RETROK_RETURN = 13,
            RETROK_PAUSE = 19,
            RETROK_ESCAPE = 27,
            RETROK_SPACE = 32,
            RETROK_EXCLAIM = 33,
            RETROK_QUOTEDBL = 34,
            RETROK_HASH = 35,
            RETROK_DOLLAR = 36,
            RETROK_AMPERSAND = 38,
            RETROK_QUOTE = 39,
            RETROK_LEFTPAREN = 40,
            RETROK_RIGHTPAREN = 41,
            RETROK_ASTERISK = 42,
            RETROK_PLUS = 43,
            RETROK_COMMA = 44,
            RETROK_MINUS = 45,
            RETROK_PERIOD = 46,
            RETROK_SLASH = 47,
            RETROK_0 = 48,
            RETROK_1 = 49,
            RETROK_2 = 50,
            RETROK_3 = 51,
            RETROK_4 = 52,
            RETROK_5 = 53,
            RETROK_6 = 54,
            RETROK_7 = 55,
            RETROK_8 = 56,
            RETROK_9 = 57,
            RETROK_COLON = 58,
            RETROK_SEMICOLON = 59,
            RETROK_LESS = 60,
            RETROK_EQUALS = 61,
            RETROK_GREATER = 62,
            RETROK_QUESTION = 63,
            RETROK_AT = 64,
            RETROK_LEFTBRACKET = 91,
            RETROK_BACKSLASH = 92,
            RETROK_RIGHTBRACKET = 93,
            RETROK_CARET = 94,
            RETROK_UNDERSCORE = 95,
            RETROK_BACKQUOTE = 96,
            RETROK_a = 97,
            RETROK_b = 98,
            RETROK_c = 99,
            RETROK_d = 100,
            RETROK_e = 101,
            RETROK_f = 102,
            RETROK_g = 103,
            RETROK_h = 104,
            RETROK_i = 105,
            RETROK_j = 106,
            RETROK_k = 107,
            RETROK_l = 108,
            RETROK_m = 109,
            RETROK_n = 110,
            RETROK_o = 111,
            RETROK_p = 112,
            RETROK_q = 113,
            RETROK_r = 114,
            RETROK_s = 115,
            RETROK_t = 116,
            RETROK_u = 117,
            RETROK_v = 118,
            RETROK_w = 119,
            RETROK_x = 120,
            RETROK_y = 121,
            RETROK_z = 122,
            RETROK_LEFTBRACE = 123,
            RETROK_BAR = 124,
            RETROK_RIGHTBRACE = 125,
            RETROK_TILDE = 126,
            RETROK_DELETE = 127,

            RETROK_KP0 = 256,
            RETROK_KP1 = 257,
            RETROK_KP2 = 258,
            RETROK_KP3 = 259,
            RETROK_KP4 = 260,
            RETROK_KP5 = 261,
            RETROK_KP6 = 262,
            RETROK_KP7 = 263,
            RETROK_KP8 = 264,
            RETROK_KP9 = 265,
            RETROK_KP_PERIOD = 266,
            RETROK_KP_DIVIDE = 267,
            RETROK_KP_MULTIPLY = 268,
            RETROK_KP_MINUS = 269,
            RETROK_KP_PLUS = 270,
            RETROK_KP_ENTER = 271,
            RETROK_KP_EQUALS = 272,

            RETROK_UP = 273,
            RETROK_DOWN = 274,
            RETROK_RIGHT = 275,
            RETROK_LEFT = 276,
            RETROK_INSERT = 277,
            RETROK_HOME = 278,
            RETROK_END = 279,
            RETROK_PAGEUP = 280,
            RETROK_PAGEDOWN = 281,

            RETROK_F1 = 282,
            RETROK_F2 = 283,
            RETROK_F3 = 284,
            RETROK_F4 = 285,
            RETROK_F5 = 286,
            RETROK_F6 = 287,
            RETROK_F7 = 288,
            RETROK_F8 = 289,
            RETROK_F9 = 290,
            RETROK_F10 = 291,
            RETROK_F11 = 292,
            RETROK_F12 = 293,
            RETROK_F13 = 294,
            RETROK_F14 = 295,
            RETROK_F15 = 296,

            RETROK_NUMLOCK = 300,
            RETROK_CAPSLOCK = 301,
            RETROK_SCROLLOCK = 302,
            RETROK_RSHIFT = 303,
            RETROK_LSHIFT = 304,
            RETROK_RCTRL = 305,
            RETROK_LCTRL = 306,
            RETROK_RALT = 307,
            RETROK_LALT = 308,
            RETROK_RMETA = 309,
            RETROK_LMETA = 310,
            RETROK_LSUPER = 311,
            RETROK_RSUPER = 312,
            RETROK_MODE = 313,
            RETROK_COMPOSE = 314,

            RETROK_HELP = 315,
            RETROK_PRINT = 316,
            RETROK_SYSREQ = 317,
            RETROK_BREAK = 318,
            RETROK_MENU = 319,
            RETROK_POWER = 320,
            RETROK_EURO = 321,
            RETROK_UNDO = 322,
            RETROK_OEM_102 = 323,

            RETROK_LAST
        }

        private enum retro_mod
        {
            RETROKMOD_NONE = 0x0000,

            RETROKMOD_SHIFT = 0x01,
            RETROKMOD_CTRL  = 0x02,
            RETROKMOD_ALT   = 0x04,
            RETROKMOD_META  = 0x08,

            RETROKMOD_NUMLOCK   = 0x10,
            RETROKMOD_CAPSLOCK  = 0x20,
            RETROKMOD_SCROLLOCK = 0x40
        }

        private enum retro_hw_render_interface_type
        {
            RETRO_HW_RENDER_INTERFACE_VULKAN    = 0,
            RETRO_HW_RENDER_INTERFACE_D3D9      = 1,
            RETRO_HW_RENDER_INTERFACE_D3D10     = 2,
            RETRO_HW_RENDER_INTERFACE_D3D11     = 3,
            RETRO_HW_RENDER_INTERFACE_D3D12     = 4,
            RETRO_HW_RENDER_INTERFACE_GSKIT_PS2 = 5
        }

        private enum retro_hw_render_context_negotiation_interface_type
        {
            RETRO_HW_RENDER_CONTEXT_NEGOTIATION_INTERFACE_VULKAN = 0
        }

        public enum retro_log_level
        {
            RETRO_LOG_DEBUG = 0,
            RETRO_LOG_INFO  = 1,
            RETRO_LOG_WARN  = 2,
            RETRO_LOG_ERROR = 3
        }

        private enum retro_sensor_action
        {
            RETRO_SENSOR_ACCELEROMETER_ENABLE = 0,
            RETRO_SENSOR_ACCELEROMETER_DISABLE,
            RETRO_SENSOR_GYROSCOPE_ENABLE,
            RETRO_SENSOR_GYROSCOPE_DISABLE,
            RETRO_SENSOR_ILLUMINANCE_ENABLE,
            RETRO_SENSOR_ILLUMINANCE_DISABLE
        }

        private enum retro_camera_buffer
        {
            RETRO_CAMERA_BUFFER_OPENGL_TEXTURE = 0,
            RETRO_CAMERA_BUFFER_RAW_FRAMEBUFFER
        }

        private enum retro_rumble_effect
        {
            RETRO_RUMBLE_STRONG = 0,
            RETRO_RUMBLE_WEAK   = 1
        }

        private enum retro_hw_context_type
        {
            RETRO_HW_CONTEXT_NONE             = 0,
            RETRO_HW_CONTEXT_OPENGL           = 1,
            RETRO_HW_CONTEXT_OPENGLES2        = 2,
            RETRO_HW_CONTEXT_OPENGL_CORE      = 3,
            RETRO_HW_CONTEXT_OPENGLES3        = 4,
            RETRO_HW_CONTEXT_OPENGLES_VERSION = 5,
            RETRO_HW_CONTEXT_VULKAN           = 6,
            RETRO_HW_CONTEXT_DIRECT3D         = 7
        }

        public enum retro_pixel_format
        {
            RETRO_PIXEL_FORMAT_0RGB1555 = 0,
            RETRO_PIXEL_FORMAT_XRGB8888 = 1,
            RETRO_PIXEL_FORMAT_RGB565   = 2
        }
    }
}
