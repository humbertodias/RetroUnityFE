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
using System.Reflection;
using UnityEngine;

namespace SK.Libretro.Utilities
{
    public static class Log
    {
        public static void Info(string message, string caller = null)
        {
            LogInternal("<color=yellow>[INFO]</color>", message, caller);
        }

        public static void Success(string message, string caller = null)
        {
            LogInternal("<color=green>[SUCCESS]</color>", message, caller);
        }

        public static void Warning(string message, string caller = null)
        {
            LogInternal("<color=orange>[WARNING]</color>", message, caller);
        }

        public static void Error(string message, string caller = null)
        {
            LogInternal("<color=red>[ERROR]</color>", message, caller);
        }

        public static void Exception(Exception e, string caller = null)
        {
            LogInternal("<color=red>[EXCEPTION]</color>", e.Message, caller);
        }

        private static void LogInternal(string prefix, string message, string caller)
        {
            Debug.Log($"{prefix} {(string.IsNullOrEmpty(caller) ? "" : $"<color=lightblue>[{caller}]</color> ")}{message}");
        }
        public static void ClearConsole()
        {
#if UNITY_EDITOR
            Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            Type type = assembly.GetType("UnityEditor.LogEntries");
            MethodInfo method = type.GetMethod("Clear");
            _ = method.Invoke(new object(), null);
#endif
        }
    }
}
