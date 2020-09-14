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
using System.IO;
using System.Runtime.InteropServices;
using static SK.Libretro.Wrapper;
using static SK.Libretro.Utilities.StringUtils;

namespace SK.Libretro
{
    public class LibretroGame
    {
        public string Name { get; private set; }

        public retro_system_av_info SystemAVInfo;
        public retro_pixel_format PixelFormat;

        public bool Running { get; private set; }

        private LibretroCore _core;
        private IntPtr _internalData;

        private string _extractedPath = null;

        public bool Start(LibretroCore core, string gameDirectory, string gameName)
        {
            bool result = false;

            _core = core;
            Name = gameName;

            if (!string.IsNullOrEmpty(gameName))
            {
                string directory = FileSystem.GetAbsolutePath(gameDirectory);

                string gamePath = GetGamePath(directory, gameName);
                if (gamePath == null)
                {
                    // Try Zip archive
                    string archivePath = FileSystem.GetAbsolutePath($"{directory}/{gameName}.zip");
                    if (File.Exists(archivePath))
                    {
                        Guid gameGuid = Guid.NewGuid();
                        System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, $"{ExtractDirectory}/{gameName}_{gameGuid}");
                        gamePath = GetGamePath($"{ExtractDirectory}/{gameName}_{gameGuid}", gameName);
                        _extractedPath = gamePath;
                    }
                }

                if (gamePath != null)
                {
                    retro_game_info gameInfo = GetGameInfo(gamePath);
                    if (_core.retro_load_game(ref gameInfo))
                    {
                        try
                        {
                            SystemAVInfo = new retro_system_av_info();
                            _core.retro_get_system_av_info(ref SystemAVInfo);

                            Running = true;
                            result = true;
                        }
                        catch (Exception e)
                        {
                            Log.Exception(e, "Libretro.LibretroGame.Start");
                        }
                    }
                }
                else
                {
                    Log.Error($"Game '{gameName}' not found in directory '{gameDirectory}'.", "Libretro.LibretroGame.Start");
                }
            }
            else if (core.SupportNoGame)
            {
                retro_game_info gameInfo = new retro_game_info();
                if (_core.retro_load_game(ref gameInfo))
                {
                    try
                    {
                        SystemAVInfo = new retro_system_av_info();
                        _core.retro_get_system_av_info(ref SystemAVInfo);

                        Running = true;
                        result = true;
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e, "Libretro.LibretroGame.Start");
                    }
                }
            }
            else
            {
                Log.Warning($"Game not set, running '{core.CoreName}' core only.", "Libretro.LibretroGame.Start");
            }

            return result;
        }

        public void Stop()
        {
            if (Running)
            {
                _core?.retro_unload_game();
                Running = false;
            }

            if (_internalData != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_internalData);
            }

            if (!string.IsNullOrEmpty(_extractedPath) && FileSystem.FileExists(_extractedPath))
            {
                Directory.Delete(Path.GetDirectoryName(_extractedPath), true);
            }
        }

        private string GetGamePath(string directory, string gameName)
        {
            foreach (string extension in _core.ValidExtensions)
            {
                string filePath = FileSystem.GetAbsolutePath($"{directory}/{gameName}.{extension}");
                if (File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return null;
        }

        private unsafe retro_game_info GetGameInfo(string gamePath)
        {
            using (FileStream stream = new FileStream(gamePath, FileMode.Open))
            {
                byte[] data = new byte[stream.Length];
                _ = stream.Read(data, 0, (int)stream.Length);
                _internalData = Marshal.AllocHGlobal(data.Length * Marshal.SizeOf<byte>());
                Marshal.Copy(data, 0, _internalData, data.Length);
                return new retro_game_info
                {
                    path = StringToChars(gamePath),
                    size = Convert.ToUInt32(data.Length),
                    data = _internalData.ToPointer()
                };
            }
        }
    }
}
