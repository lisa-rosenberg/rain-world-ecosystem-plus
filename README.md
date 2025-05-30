# RainWorld - EcosystemPlus

## Description

### Goal
Adds dynamic food-seeking behavior, grazing, and hunger to non-player creatures. Enhances ecosystem realism with food competition, AI hibernation, and inter-species predation.

### Current State
As of now, this project is WIP and not working!

## For Developers
To build and extend this mod, make sure the following tools and steps are set up correctly:

### Requirements
* Rain World (Steam) – Latest Version
* .NET Framework Developer Pack 4.7.2 → Install via Visual Studio Installer (check .NET desktop development workload)
  * You may need to add msbuild to your environment variables `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin` in order to use the command in the CLI
  * You only need the `.NET desktop build tools` from it
  * Looking into "Individual components", make sure these two are included:
    * .NET Framework 4.7.2 targeting pack
    * .NET Framework 4.7.2 SDK
* BepInEx 5 for Rain World
  * Download the latest version from here: https://github.com/BepInEx/BepInEx/releases
  * Extract the files and copy them into your Rain World folder, e.g.: `S:\Games\SteamLibrary\steamapps\common\Rain World`
* NuGet: https://www.nuget.org/downloads/ - you"ll only need the nuget.exe file, nothing else. You may add the folder into your environment variables.
* HookGen: See section "HookGen" for more information

* Some IDE (e.g. JetBrains Rider, VSCode, or Visual Studio)

### Folder Structure
Clone or develop in your own folder (e.g. `F:\Development\RainWorld\EcosystemPlus`) and do not develop directly inside the Rain World directory.

Compiled binaries are automatically copied into `Rain World/BepInEx/plugins/EcosystemPlus/` using the PostBuild step.

### Required DLL References
Your `.csproj` must reference these DLLs from the Rain World installation:

```
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\Assembly-CSharp.dll</HintPath>
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\UnityEngine.dll</HintPath>
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\RWCustom.dll</HintPath>
<HintPath>[YourRainWorldDir]\BepInEx\core\BepInEx.dll</HintPath>
<HintPath>[YourRainWorldDir]\BepInEx\core\MonoMod.RuntimeDetour.dll</HintPath>
```

### HookGen
One way is to get the HookGen binaries via NuGet with this command: `nuget install MonoMod.RuntimeDetour.HookGen -Version 22.7.31.1 -OutputDirectory F:\Tools\NuGet\HookGenTemp`.

After this, you need to collect the following `.dll` files from the subfolders, as `.exe` files don't automatically load nearby dependencies unless they are in the same folder. If they are not present in these subfolders, use the `.dll` files from nearby subfolders as stated below.

```
F:\Tools\NuGet\HookGenTemp\MonoMod.22.7.31.1\lib\net452\MonoMod.dll (or F:\Tools\NuGet\HookGenTemp\MonoMod.22.7.31.1\lib\netstandard2.0\MonoMod.dll)
F:\Tools\NuGet\HookGenTemp\MonoMod.RuntimeDetour.22.7.31.1\lib\net452\MonoMod.RuntimeDetour.dll
F:\Tools\NuGet\HookGenTemp\MonoMod.Utils.22.7.31.1\lib\net452\MonoMod.Utils.dll
F:\Tools\NuGet\HookGenTemp\Mono.Cecil.0.11.4\lib\net452\Mono.Cecil.dll (or F:\Tools\NuGet\HookGenTemp\Mono.Cecil.0.11.4\lib\net40\Mono.Cecil.dll)
```

Put all of these files into `F:\Tools\NuGet\HookGenTemp\MonoMod.RuntimeDetour.HookGen.22.7.31.1\lib\net452`. Your folder should look like this:

```
PS F:\Tools\NuGet\HookGenTemp\MonoMod.RuntimeDetour.HookGen.22.7.31.1\lib\net452> ls

    Directory: F:\Tools\NuGet\HookGenTemp\MonoMod.RuntimeDetour.HookGen.22.7.31.1\lib\net452

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---            7/2/2021 12:17 AM         358400 Mono.Cecil.dll
-a---           7/31/2022  6:33 PM          68096 MonoMod.dll
-a---           7/31/2022  6:33 PM         112128 MonoMod.RuntimeDetour.dll
-a---           7/31/2022  6:33 PM          24576 MonoMod.RuntimeDetour.HookGen.exe
-a---           7/31/2022  6:33 PM            154 MonoMod.RuntimeDetour.HookGen.xml
-a---           7/31/2022  6:33 PM         197632 MonoMod.Utils.dll

```

HookGen is needed to create hook files.

### Building

#### Hooks
The following command (execute it inside `F:\Tools\NuGet\HookGenTemp\MonoMod.RuntimeDetour.HookGen.22.7.31.1\lib\net452`) will create the needed hook for the `Assembly-CSharp.dll` file.

```
PS F:\Tools\NuGet\HookGenTemp\MonoMod.RuntimeDetour.HookGen.22.7.31.1\lib\net452> & '.\MonoMod.RuntimeDetour.HookGen.exe' 'S:\Games\SteamLibrary\steamapps\common\Rain World\RainWorld_Data\Managed\Assembly-CSharp.dll'
MonoMod.RuntimeDetour.HookGen 22.7.31.1
using MonoMod 22.7.31.1
using MonoMod.RuntimeDetour 22.7.31.1
[MonoMod] Reading input file into module.
[MonoMod] [HookGen] Starting HookGenerator
[MonoMod] [HookGen] Done.
```

#### Target
Make sure to check and update the target folder inside the `.csproj` file. The target needs to point to your mod project inside BepInEx.

#### Build the Project
Once everything is ready, you can build the project with these commands:

```
msbuild /t:Clean EcosystemPlus.csproj
msbuild EcosystemPlus.csproj
```

Or use equivalent actions to build the solution (`.sln`). The DLL is automatically copied to the BepInEx plugin folder after build.

When Rain World launches, BepInEx will log plugin load messages in the console and `BepInEx/LogOutput.log`.

Check the BepInEx log at `RainWorld/BepInEx/LogOutput.log` to ensure your mod is being loaded.