# RainWorld - EcosystemPlus

## Description

### Goal
EcosystemPlus aims to enhance the realism of Rain World's ecosystem by adding dynamic AI behaviors such as grazing, food-seeking, hunger-driven actions, and inter-species predation. It introduces resource competition, optional AI hibernation mechanics, and an overall more immersive food chain.

### Current State
⚠️ Work in Progress – This mod is not yet fully functional.

## For Developers
**Note:** The following guide was written with myself in mind while learning how to set everything up - mainly as a future reference if I ever have to go through this madness again. 😄

To build and extend this mod, ensure the following tools and steps are completed.

### Requirements

- **Rain World (Steam)** – Latest Version
- **.NET Framework Developer Pack 4.7.2**
  - Install via Visual Studio Installer with the `.NET desktop development` workload
  - Add `C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin` to your environment variables if needed
  - Required components (can be selected under "Individual components"):
    - .NET Framework 4.7.2 targeting pack
    - .NET Framework 4.7.2 SDK
- **BepInEx 5 for Rain World**
  - Download: https://github.com/BepInEx/BepInEx/releases
  - Extract into your Rain World directory, e.g., `S:\Games\SteamLibrary\steamapps\common\Rain World`
- **NuGet CLI**
  - Download `nuget.exe` from https://www.nuget.org/downloads/
  - Optional: add the containing folder to your environment variables
- **HookGen** – see section below
- A C#-compatible IDE (e.g., JetBrains Rider, Visual Studio, or VSCode)

### Folder Structure

Clone the project into a separate directory, e.g., `F:\Development\RainWorld\EcosystemPlus`. Do **not** modify files directly in the Rain World folder.

Compiled binaries are copied into `Rain World/BepInEx/plugins/EcosystemPlus/` automatically via a post-build step.

### Required DLL References

Your `.csproj` file must reference these DLLs from the Rain World install:

```xml
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\Assembly-CSharp.dll</HintPath>
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\UnityEngine.dll</HintPath>
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\UnityEngine.CoreModule.dll</HintPath>
<HintPath>[YourRainWorldDir]\RainWorld_Data\Managed\RWCustom.dll</HintPath>
<HintPath>[YourRainWorldDir]\BepInEx\core\BepInEx.dll</HintPath>
<HintPath>[YourRainWorldDir]\BepInEx\core\MonoMod.RuntimeDetour.dll</HintPath>
```

### HookGen
To hook into Rain World's internal classes and methods (e.g. AI behaviors), you need to generate hookable delegates using HookGen.

Run this command to install HookGen via NuGet:

```powershell
nuget install MonoMod.RuntimeDetour.HookGen -Version 22.7.31.1 -OutputDirectory F:\Tools\NuGet\HookGenTemp
```

HookGen requires some additional libraries next to the `.exe` to function. Copy the following files into the same folder as `MonoMod.RuntimeDetour.HookGen.exe`:

```text
Mono.Cecil.dll
MonoMod.dll
MonoMod.RuntimeDetour.dll
MonoMod.Utils.dll
```

Use versions compatible with `net452` if available. If not, fallback to `netstandard2.0` or `net40` as needed.

Your folder should contain:

```text
Mono.Cecil.dll
MonoMod.dll
MonoMod.RuntimeDetour.dll
MonoMod.RuntimeDetour.HookGen.exe
MonoMod.RuntimeDetour.HookGen.xml
MonoMod.Utils.dll
```

### Generating Hookable Delegates

Navigate to the directory containing HookGen and run:

```powershell
& '.\MonoMod.RuntimeDetour.HookGen.exe' 'S:\Games\SteamLibrary\steamapps\common\Rain World\RainWorld_Data\Managed\Assembly-CSharp.dll'
```

This generates the `MMHOOK_Assembly-CSharp.dll` which contains hookable delegate definitions (e.g., `On.NeedleWormAI`). These are essential for reflection-based hooking and proper mod development, making this step absolutely mandatory. When set up correctly, you can even jump into the generated source (e.g., with `Ctrl + B` in JetBrains Rider), which is super helpful.

### Building

Ensure your `.csproj` file's output path points to your BepInEx plugin directory.

To build the project:

```powershell
msbuild /t:Clean EcosystemPlus.csproj
msbuild EcosystemPlus.csproj
```

Or use the build action from your IDE.

On launch, Rain World will load your plugin and output logs to:

```text
RainWorld\BepInEx\LogOutput.log
```

Check the logs for loading confirmation and error diagnostics.