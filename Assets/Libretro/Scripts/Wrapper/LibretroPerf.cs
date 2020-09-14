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

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public long RetroPerfGetTimeUsec()
        {
            Log.Warning("RetroPerfGetTimeUsec");
            return 0;
        }

        public ulong RetroPerfGetCounter()
        {
            Log.Warning("RetroPerfGetCounter");
            return 0;
        }

        public ulong RetroGetCPUFeatures()
        {
            Log.Warning("RetroGetCPUFeatures");
            return 0;
        }

        public void RetroPerfLog()
        {
            Log.Warning("RetroPerfLog");
        }

        public void RetroPerfRegister(ref retro_perf_counter counter)
        {
            Log.Warning("RetroPerfRegister");
        }

        public void RetroPerfStart(ref retro_perf_counter counter)
        {
            Log.Warning("RetroPerfStart");
        }

        public void RetroPerfStop(ref retro_perf_counter counter)
        {
            Log.Warning("RetroPerfStop");
        }
    }
}
