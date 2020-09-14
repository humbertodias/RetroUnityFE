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

using System;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public unsafe void RetroVideoRefreshCallback(void* data, uint width, uint height, uint pitch)
        {
            if (GraphicsProcessor != null)
            {
                int intWidth = (int)width;
                int intHeight = (int)height;
                int intPitch = (int)pitch;

                switch (Game.PixelFormat)
                {
                    case retro_pixel_format.RETRO_PIXEL_FORMAT_0RGB1555:
                    {
                        GraphicsProcessor.ProcessFrame0RGB1555((ushort*)data, intWidth, intHeight, intPitch / sizeof(ushort));
                    }
                    break;
                    case retro_pixel_format.RETRO_PIXEL_FORMAT_XRGB8888:
                    {

                        GraphicsProcessor.ProcessFrameARGB8888((uint*)data, intWidth, intHeight, intPitch / sizeof(uint));
                    }
                    break;
                    case retro_pixel_format.RETRO_PIXEL_FORMAT_RGB565:
                    {
                        GraphicsProcessor.ProcessFrameRGB565((ushort*)data, intWidth, intHeight, intPitch / sizeof(ushort));
                    }
                    break;
                    default:
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}
