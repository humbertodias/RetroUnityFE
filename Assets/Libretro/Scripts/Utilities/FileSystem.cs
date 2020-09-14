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

namespace SK.Libretro.Utilities
{
    public static class FileSystem
    {
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public static bool CreateFile(string path)
        {
            bool result = false;

            try
            {
                using (_ = File.Create(GetAbsolutePath(path)))
                {
                    result = true;
                }
            }
            catch (Exception e)
            {
                Log.Exception(e, "FileSystem.CreateFile");
            }

            return result;
        }

        public static bool DeleteFile(string path)
        {
            bool result = false;

            try
            {
                File.Delete(GetAbsolutePath(path));
#if UNITY_EDITOR
                File.Delete(GetAbsolutePath($"{path}.meta"));
#endif
                result = true;
            }
            catch (Exception e)
            {
                Log.Exception(e, "FileSystem.DeleteFile");
            }

            return result;
        }

        public static string GetAbsolutePath(string path)
        {
            string result = null;

            try
            {
                if (path.StartsWith("@", StringComparison.OrdinalIgnoreCase))
                {
                    result = Path.GetFullPath(Path.Combine(UnityEngine.Application.streamingAssetsPath, path.Remove(0, 1)));
                }
                else
                {
                    result = Path.GetFullPath(path);
                }
            }
            catch (Exception e)
            {
                Log.Exception(e, "FileSystem.GetAbsolutePath");
            }

            return result;
        }

        public static string GetRelativePath(string path)
        {
            string result = path;

            try
            {
                string fullPath = GetAbsolutePath(path);
                string formattedStreamingAssetsPath = UnityEngine.Application.streamingAssetsPath.Replace('/', Path.DirectorySeparatorChar);
                if (fullPath.Contains(formattedStreamingAssetsPath))
                {
                    result = $"@{fullPath.Replace($"{formattedStreamingAssetsPath}", string.Empty).Remove(0, 1)}";
                }
            }
            catch (Exception e)
            {
                Log.Exception(e, "FileSystem.GetRelativePath");
            }

            return result;
        }

        public static string[] GetFilesInDirectory(string path, string searchPattern, bool includeSubFolders = false)
        {
            string[] result = null;

            try
            {
                result = Directory.GetFiles(GetAbsolutePath(path), searchPattern, includeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            catch (Exception e)
            {
                Log.Exception(e, "FileSystem.GetFilesInDirectory");
            }

            return result;
        }

        public static bool SerializeToJson<T>(T sourceObject, string targetPath)
        {
            bool result = false;

            try
            {
                string jsonString = UnityEngine.JsonUtility.ToJson(sourceObject, true);
                File.WriteAllText(GetAbsolutePath(targetPath), jsonString);
                result = true;
            }
            catch (Exception e)
            {
                Log.Exception(e, $"FileSystem.SerializeToJson<{typeof(T)}>");
            }

            return result;
        }

        public static T DeserializeFromJson<T>(string sourcePath) where T : class
        {
            T result = null;

            try
            {
                string jsonString = File.ReadAllText(GetAbsolutePath(sourcePath));
                result = UnityEngine.JsonUtility.FromJson<T>(jsonString);
            }
            catch (Exception e)
            {
                Log.Exception(e, $"FileSystem.DeserializeFromJson<{typeof(T)}>");
            }

            return result;
        }
    }
}
