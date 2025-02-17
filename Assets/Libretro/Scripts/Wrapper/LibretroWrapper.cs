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
using System.Linq;

namespace SK.Libretro
{
    public partial class Wrapper
    {
        public readonly string CoreOptionsFile;
        public readonly string WrapperDirectory;
        public readonly string CoresDirectory;
        public readonly string SystemDirectory;
        public readonly string SavesDirectory;
        public readonly string TempDirectory;
        public readonly string ExtractDirectory;

        public bool OptionCropOverscan = true;

        public IGraphicsProcessor GraphicsProcessor;
        public IAudioProcessor AudioProcessor;
        public IInputProcessor InputProcessor;

        public LibretroCore Core { get; private set; } = new LibretroCore();
        public LibretroGame Game { get; private set; } = new LibretroGame();

        private string _rootDirectory;

        private CoreOptionsList _coreOptionsList;


        public Wrapper(string rootDirectory)
        {
            _rootDirectory = rootDirectory;
            CoreOptionsFile = $"{_rootDirectory}/core_options.json";
            WrapperDirectory = $"{_rootDirectory}/libretro~";
            CoresDirectory   = $"{WrapperDirectory}/cores";
            SystemDirectory  = $"{WrapperDirectory}/system";
            SavesDirectory   = $"{WrapperDirectory}/saves";
            TempDirectory    = $"{WrapperDirectory}/temp";
            ExtractDirectory = $"{TempDirectory}/extracted";
        }

        public bool StartGame(string coreDirectory, string coreName, string gameDirectory, string gameName)
        {
            bool result = false;

            LoadCoreOptionsFile();

            if (Core.Start(this, coreDirectory, coreName))
            {
                if (Game.Start(this, Core, gameDirectory, gameName))
                {
                    result = true;
                }
            }

            return result;
        }

        public void StopGame()
        {
            AudioProcessor?.DeInit();

            Game.Stop();
            Core.Stop();
        }

        public void Update()
        {
            if (!Game.Running || !Core.Initialized)
            {
                return;
            }

            Core.retro_run();
        }

        public void ActivateGraphics(IGraphicsProcessor graphicsProcessor)
        {
            GraphicsProcessor = graphicsProcessor;
        }

        public void DeactivateGraphics()
        {
            GraphicsProcessor = null;
        }

        public void ActivateAudio(IAudioProcessor audioProcessor)
        {
            AudioProcessor = audioProcessor;
            AudioProcessor.Init((int)Game.SystemAVInfo.timing.sample_rate);
        }

        public void DeactivateAudio()
        {
            AudioProcessor?.DeInit();
            AudioProcessor = null;
        }

        public void ActivateInput(IInputProcessor inputProcessor)
        {
            InputProcessor = inputProcessor;
        }

        public void DeactivateInput()
        {
            InputProcessor = null;
        }

        private void LoadCoreOptionsFile()
        {
            _coreOptionsList = FileSystem.DeserializeFromJson<CoreOptionsList>(CoreOptionsFile);
            if (_coreOptionsList == null)
            {
                _coreOptionsList = new CoreOptionsList();
            }
        }

        private void SaveCoreOptionsFile()
        {
            _coreOptionsList.Cores = _coreOptionsList.Cores.OrderBy(x => x.CoreName).ToList();
            for (int i = 0; i < _coreOptionsList.Cores.Count; i++)
            {
                _coreOptionsList.Cores[i].Options.Sort();
            }
            _ = FileSystem.SerializeToJson(_coreOptionsList, CoreOptionsFile);
        }
    }
}
