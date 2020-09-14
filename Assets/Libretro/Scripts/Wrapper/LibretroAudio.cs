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

using Unity.Mathematics;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public void RetroAudioSampleCallback(short left, short right)
        {
            if (AudioProcessor != null)
            {
                float[] floatBuffer = new float[]
                {
                    math.clamp(left * -0.000030517578125f, -1.0f, 1.0f),
                    math.clamp(right * -0.000030517578125f, -1.0f, 1.0f)
                };

                AudioProcessor.ProcessSamples(floatBuffer);
            }
        }

        public unsafe uint RetroAudioSampleBatchCallback(short* data, uint frames)
        {
            if (AudioProcessor != null)
            {
                float[] floatBuffer = new float[frames * 2];

                for (int i = 0; i < floatBuffer.Length; ++i)
                {
                    floatBuffer[i] = math.clamp(data[i] * 0.000030517578125f, -1.0f, 1.0f);
                }

                AudioProcessor.ProcessSamples(floatBuffer);
            }

            return frames;
        }
    }
}
