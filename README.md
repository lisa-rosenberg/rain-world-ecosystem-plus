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
* BepInEx 5 for Rain World → Installed in your Rain World folder
  * You may need to change %USERPROFILE%\AppData\Local\Microsoft\WindowsApps;C:\Users\lisar\AppData\Local\Programs\Microsoft VS Code\bin;C:\Users\lisar\AppData\Local\Microsoft\WinGet\Packages\Git.MinGit_Microsoft.Winget.Source_8wekyb3d8bbwe\cmd;F:\Tools\SDK\EclipseAdoptium.Temurin.24.JDK\bin;%USERPROFILE%\.dotnet\tools;C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin;
* Some IDE (e.g. JetBrains Rider, VSCode, or Visual Studio)
* Visual Studio or JetBrains Rider (recommended for C#)

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

### Building
Once everything is ready, you can build the project with these commands:

```
msbuild /t:Clean EcosystemPlus.csproj
msbuild EcosystemPlus.csproj
```

Or use equivalent actions to build the solution (`.sln`). The DLL is automatically copied to the BepInEx plugin folder after build.

When Rain World launches, BepInEx will log plugin load messages in the console and `BepInEx/LogOutput.log`.

Check the BepInEx log at `RainWorld/BepInEx/LogOutput.log` to ensure your mod is being loaded.