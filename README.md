# RetroUnityFE

RetroUnityFE is a frontend for the libretro API built in Unity 6+

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

```shell
sudo apt install libretro-snes9x libretro-snes9x-next
retroarch --libretro /usr/lib/libretro/snes9x_libretro.so Classic\ Kong\ Complete\ \(U\) \V2-01.smc 
```
Tested [cores](http://buildbot.libretro.com/nightly/apple/osx/arm64/latest/)

* snes9x
* blastem
* nestopia
* mgba
* mame2003_plus
* vecx
* yabause

Ref

* [snes-sdk](https://github.com/optixx/snes-sdk)

* [classickong](https://github.com/nathancassano/classickong)

* [flatscreen](https://assetstore.unity.com/packages/3d/props/electronics/flatscreen-tv-9721)

* [furniture-set](https://assetstore.unity.com/packages/3d/props/furniture/free-furniture-set-26678)
