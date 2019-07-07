# RetroUnity

RetroUnity is a frontend for the libretro API built in Unity 2019.3.0a8+.

[webm](https://gfycat.com/PresentUnconsciousAmberpenshell)

## Documentation
See [wiki](https://github.com/Scorr/RetroUnity/wiki).

## External assets
The following assets were used in this project:
* [Flatscreen TV](https://www.assetstore.unity3d.com/en/#!/content/9721) by Rutger Klunder
* [Free Furniture Set](https://www.assetstore.unity3d.com/en/#!/content/26678) by Lef
* [LibRetro for Linux](http://dimitry-i.blogspot.com/2013/01/mononet-how-to-dynamically-load-native.html)


Tested on Linux

![](libretro.png)

Shared library and roms inside [Assets/StreamingAssets](Assets/StreamingAssets) folder

```
sudo apt install libretro-snes9x libretro-snes9x-next
retroarch --libretro /usr/lib/libretro/snes9x_libretro.so Chrono\ Trigger\ \(USA\).sfc 
```