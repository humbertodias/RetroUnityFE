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
using System.IO;
using System.Runtime.InteropServices;

namespace SK.Libretro.Utilities
{
    public sealed class DllModuleWindows : DllModule
    {
        [DllImport("kernel32.dll", EntryPoint = "LoadLibrary", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr Win32LoadLibrary([MarshalAs(UnmanagedType.LPTStr)] string lpLibFileName);

        [DllImport("kernel32.dll", EntryPoint = "GetProcAddress", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr Win32GetProcAddress(IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

        [DllImport("kernel32.dll", EntryPoint = "FreeLibrary", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool Win32FreeLibrary(IntPtr hModule);

        public override void Load(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                IntPtr hModule = Win32LoadLibrary(path);
                if (hModule != IntPtr.Zero)
                {
                    Name          = Path.GetFileName(path);
                    _nativeHandle = hModule;
                }
                else
                {
                    throw new Exception($"Failed to load library at path '{path}' (ErrorCode: {Marshal.GetLastWin32Error()})");
                }
            }
            else
            {
                throw new Exception("Library path is null or empty.");
            }
        }

        public override T GetFunction<T>(string functionName)
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                IntPtr procAddress = Win32GetProcAddress(_nativeHandle, functionName);
                if (procAddress != IntPtr.Zero)
                {
                    return Marshal.GetDelegateForFunctionPointer<T>(procAddress);
                }
                else
                {
                    throw new Exception($"Function '{functionName}' not found in library '{Name}'.");
                }
            }
            else
            {
                throw new Exception($"Library not loaded, cannot get function '{functionName}'");
            }
        }

        public override void Free()
        {
            if (_nativeHandle != IntPtr.Zero)
            {
                _ = Win32FreeLibrary(_nativeHandle);
            }
        }
    }
}
