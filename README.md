[![Continuous Delivery](https://github.com/humbertodias/RetroUnityFE/actions/workflows/cd.yml/badge.svg)](https://github.com/humbertodias/RetroUnityFE/actions/workflows/cd.yml)
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/humbertodias/RetroUnityFE)
![GitHub all releases](https://img.shields.io/github/downloads/humbertodias/RetroUnityFE/total)

# RetroUnityFE

RetroUnityFE is a frontend for the **libretro API** built in **Unity 6+**.

[RetroUnity-demo.webm](https://github.com/user-attachments/assets/6af507c6-a1e5-4475-b221-fb9dfb2d859a)

| Input        | Action       |
|:------------:|:------------:|
| 5            | Insert Coin  |
| WASD/Arrows  | Move         |
| Enter        | Start        |
| P            | Pause        |
| Ctrl         | Punch        |
| Alt          | Jump         |

## 📖 Documentation

For detailed setup and usage instructions, see the [Wiki](https://deepwiki.com/humbertodias/RetroUnityFE/).

## 🎮 Tested Platforms

* [x] Linux
* [x] MacOS
* [x] Windows
* [x] Android
* [ ] iOS

## 📋 Features

* [ ] Savestate
* [ ] Rewind 

## 📂 External Assets

The following assets are used in this project:

- **[Flatscreen TV](https://assetstore.unity.com/packages/3d/props/electronics/flatscreen-tv-9721)** by Rutger Klunder
- **[Free Furniture Set](https://assetstore.unity.com/packages/3d/props/furniture/free-furniture-set-26678)** by Lef
- **[LibRetro for Linux](http://dimitry-i.blogspot.com/2013/01/mononet-how-to-dynamically-load-native.html)**

## 🔧 Setup Instructions

### 📁 Shared Library & ROMs

Place shared libraries and ROMs inside the `Assets/StreamingAssets/` folder.

### 🛠️ Install Dependencies (Linux)

```sh
sudo apt install libretro-snes9x libretro-snes9x-next
```

### 🎮 Running a Game

```sh
retroarch --libretro /usr/lib/libretro/snes9x_libretro.so "Classic Kong Complete (U) V2-01.smc"
```

```sh
RetroUnityFE --libretro /usr/lib/libretro/snes9x_libretro.so "Classic Kong Complete (U) V2-01.smc"
```

## 🕹️ Tested Libretro Cores

The following cores have been tested and confirmed to work:

- **snes9x** (Super Nintendo)
- **blastem** (Sega Genesis)
- **genesis_plus_gx** (Sega Genesis)
- **nestopia** (Nintendo Entertainment System)
- **mgba** (Game Boy Advance)
- **mame2003_plus** (Arcade)
- **vecx** (Vectrex)
- **yabause** (Sega Saturn)
- **bennugd** ([BennuGD](https://github.com/humbertodias/BennuGD_libretro/releases))

[Download latest cores](http://buildbot.libretro.com/nightly/) for your operating system and processor

## 📚 References

- [SNES SDK](https://github.com/optixx/snes-sdk)
- [Classic Kong](https://github.com/nathancassano/classickong)
- [Flatscreen TV Asset](https://assetstore.unity.com/packages/3d/props/electronics/flatscreen-tv-9721)
- [Furniture Set Asset](https://assetstore.unity.com/packages/3d/props/furniture/free-furniture-set-26678)

---

🎮 **RetroUnityFE** is an open-source project that aims to bring a seamless frontend experience for **libretro** cores in Unity!

