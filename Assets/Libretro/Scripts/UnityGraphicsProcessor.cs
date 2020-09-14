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

using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;

namespace SK.Libretro
{
    public class UnityGraphicsProcessor : IGraphicsProcessor
    {
        public Texture2D Texture { get; private set; } = new Texture2D(256, 256, TextureFormat.BGRA32, false) { filterMode = FilterMode.Trilinear };
        public bool TextureUpdated = false;

        public unsafe void ProcessFrame0RGB1555(ushort* data, int width, int height, int pitchInPixels)
        {
            if (Texture.format != TextureFormat.BGRA32 || Texture.width != width || Texture.height != height)
            {
                Texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
                TextureUpdated = true;
            }
            else
            {
                TextureUpdated = false;
            }

            new ARGB1555Job
            {
                SourceData  = data,
                Width       = width,
                Height      = height,
                PitchPixels = pitchInPixels,
                TextureData = Texture.GetRawTextureData<uint>()
            }.Schedule().Complete();

            Texture.Apply();
        }

        public unsafe void ProcessFrameARGB8888(uint* data, int width, int height, int pitchInPixels)
        {
            if (Texture.format != TextureFormat.BGRA32 || Texture.width != width || Texture.height != height)
            {
                Texture = new Texture2D(width, height, TextureFormat.BGRA32, false);
                TextureUpdated = true;
            }
            else
            {
                TextureUpdated = false;
            }

            new ARGB8888Job
            {
                SourceData  = data,
                Width       = width,
                Height      = height,
                PitchPixels = pitchInPixels,
                TextureData = Texture.GetRawTextureData<uint>()
            }.Schedule().Complete();

            Texture.Apply();
        }

        public unsafe void ProcessFrameRGB565(ushort* data, int width, int height, int pitchInPixels)
        {
            if (Texture.format != TextureFormat.RGB565 || Texture.width != width || Texture.height != height)
            {
                Texture = new Texture2D(width, height, TextureFormat.RGB565, false);
                TextureUpdated = true;
            }
            else
            {
                TextureUpdated = false;
            }

            new RGB565Job
            {
                SourceData  = data,
                Width       = width,
                Height      = height,
                PitchPixels = pitchInPixels,
                TextureData = Texture.GetRawTextureData<ushort>()
            }.Schedule().Complete();

            Texture.Apply();
        }

        [BurstCompile]
        private unsafe struct ARGB1555Job : IJob
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public ushort* SourceData;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [ReadOnly] public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                ushort* line = SourceData;
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = ARGB1555toBGRA32(line[x]);
                    }
                    line += PitchPixels;
                }
            }
        }

        [BurstCompile]
        private unsafe struct ARGB8888Job : IJob
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public uint* SourceData;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [ReadOnly] public int PitchPixels;
            [WriteOnly] public NativeArray<uint> TextureData;

            public void Execute()
            {
                uint* line = SourceData;
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = line[x];
                    }
                    line += PitchPixels;
                }
            }
        }

        [BurstCompile]
        private unsafe struct RGB565Job : IJob
        {
            [ReadOnly] [NativeDisableUnsafePtrRestriction] public ushort* SourceData;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [ReadOnly] public int PitchPixels;
            [WriteOnly] public NativeArray<ushort> TextureData;

            public void Execute()
            {
                ushort* line = SourceData;
                for (int y = 0; y < Height; ++y)
                {
                    for (int x = 0; x < Width; ++x)
                    {
                        TextureData[y * Width + x] = line[x];
                    }
                    line += PitchPixels;
                }
            }
        }

        private static uint ARGB1555toBGRA32(ushort packed)
        {
            uint a = (uint)(packed & 0x8000);
            uint r = (uint)(packed & 0x7C00);
            uint g = (uint)(packed & 0x03E0);
            uint b = (uint)(packed & 0x1F);
            uint rgb = (r << 9) | (g << 6) | (b << 3);
            return (a * 0x1FE00) | rgb | ((rgb >> 5) & 0x070707);
        }
    }
}
