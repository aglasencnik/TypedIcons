# TypedIcons

![TypedIcons logo](https://raw.githubusercontent.com/aglasencnik/TypedIcons/refs/heads/main/Icon.png)

**TypedIcons** is a tool for using Iconify icons in Blazor with full type safety and IntelliSense support.

Instead of relying on string-based icon names, TypedIcons uses a .NET CLI and source generator to generate strongly-typed access to icons from Iconify icon sets. This means you get compile-time validation, auto-completion, and a much better developer experience when working with icons.

Icons are added through the CLI and resolved at build time, so there are no runtime dependencies, no dynamic loading, and no unnecessary overhead. Everything is known at compile time, making icon usage predictable and safe.

The goal of TypedIcons is simple: make working with icons in Blazor feel like working with any other strongly-typed part of your codebase.

## Table of Contents
 
- [Installation](#installation)
  - [CLI Tool](#cli-tool)
  - [Source Generator](#source-generator)
- [Usage](#usage)
  - [Initializing a Project](#initializing-a-project)
  - [Adding Icons](#adding-icons)
  - [Using Icons in Components](#using-icons-in-components)
  - [Restoring Icons](#restoring-icons)
  - [CLI Reference](#cli-reference)
- [Manual Setup](#manual-setup)
- [How It Works](#how-it-works)
- [Support the Project](#support-the-project)
- [Contributing](#contributing)
- [License](#license)
- [Acknowledgment](#acknowledgment)

## Installation

TypedIcons consists of two components: the **CLI tool** and the **source generator**. The CLI is installed once globally (or locally), and the source generator is added to your Blazor project automatically by the CLI.

### CLI Tool
 
Install the TypedIcons CLI as a global .NET tool:
 
```bash
dotnet tool install --global TypedIcons
```
 
Or install it locally from a directory:
 
```bash
dotnet tool install --global --add-source <user_directory> TypedIcons
```

To update the tool to the latest version:

```bash
dotnet tool update --global TypedIcons
```

### Source Generator
 
The source generator (`TypedIcons.Generator`) is installed automatically into your Blazor project when you run `typedicons init` and confirm the prompt. If you prefer to add it manually, see the [Manual Setup](#manual-setup) section below.

## Usage

### Initializing a Project
 
Run the following command from your Blazor project directory (where your `.csproj` file is located):
 
```bash
typedicons init
```
 
The CLI will:
1. Detect your `.csproj` file
2. Create a `typedicons.json` configuration file
3. Create a local icon cache in the `obj` folder
4. Offer to install the `TypedIcons.Generator` source generator into your project
5. Offer to automatically add `@using TypedIcons` to `Components/_Imports.razor`

Example output:
 
```
Found project: MyBlazorApp.csproj
Created typedicons.json
Created cache file
Do you want to install the TypedIcons source generator? [y/n] (y): y
Installing package...
Source generator package installed successfully
Do you want to automatically add @using TypedIcons to Components/_Imports.razor? [y/n] (y): y
Added @using TypedIcons to Components/_Imports.razor
TypedIcons initialized successfully
```
 
To skip all confirmation prompts and accept defaults, use the `-y` flag:
 
```bash
typedicons init -y
```

### Adding Icons
 
Add any icon from Iconify using the `<set>:<icon>` format:
 
```bash
typedicons add boxicons:air-conditioner
typedicons add lucide:home
typedicons add mdi:account-circle
```
 
After adding an icon, the source generator will regenerate the strongly-typed classes. This is usually automatic and near-instant, but if the changes don't appear in your IDE immediately, try rebuilding the project or restarting your IDE.

### Using Icons in Components

Once icons are added, use them via the `<Icon>` component anywhere in your Blazor markup:

```razor
<Icon Value="Icons.Boxicons.AirConditioner" Size="5em" />
<Icon Value="Icons.Heroicons.Backspace" Size="5em" />
```

Icons are accessed through the generated `Icons.<Set>.<Name>` static classes, giving you full IntelliSense and compile-time validation.

#### Parameters

| Parameter | Type | Description |
|-----------|------|-------------|
| `Value` | `IconData` | The icon to render (required) |
| `Size` | `string?` | Sets both width and height (e.g. `"24px"`, `"1.5em"`) |
| `Width` | `string?` | Overrides width independently |
| `Height` | `string?` | Overrides height independently |

### Restoring Icons
 
The icon cache lives in the `obj` folder and is **not committed to source control** — only `typedicons.json` is. After cloning a repository or deleting the `obj` folder, restore the cache by running:
 
```bash
typedicons restore
```
 
It's a good practice to add this to your project's setup instructions or run it as part of your build process.

### CLI Reference
 
```
USAGE:
    typedicons [OPTIONS] <COMMAND>
 
OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information
 
COMMANDS:
    init             Initialize TypedIcons in the current project
    add <name>       Add an icon by name (<set>:<icon>)
    restore          Restore icons in the local cache
```

---
 
## Manual Setup
 
If you prefer not to use `typedicons init`, you can set up TypedIcons manually.
 
**1. Create `typedicons.json`** in your project root:
 
```json
{
  "icons": []
}
```
 
**2. Add the source generator** to your `.csproj`:
 
```xml
<ItemGroup>
  <PackageReference Include="TypedIcons.Generator" Version="X.Y.Z" />
</ItemGroup>
```
 
Replace `X.Y.Z` with the [latest version](https://www.nuget.org/packages/TypedIcons.Generator).
 
**3. Add the using directive** to `Components/_Imports.razor`:
 
```razor
@using TypedIcons
```
 
You can then use `typedicons add` as normal to add icons to your project.
 
---
 
## How It Works
 
TypedIcons bridges Iconify's vast icon library with .NET's type system:
 
- **`typedicons.json`** is the source of truth — it lists every icon your project uses and is committed to source control.
- **The icon cache** (in `obj/`) holds the downloaded SVG data locally. It is generated from `typedicons.json` and is excluded from source control.
- **The source generator** reads `typedicons.json` and the icon cache at build time and emits strongly-typed C# classes, giving you IntelliSense and compile-time safety in your Blazor components.
> **Note:** The source generator runs automatically in most cases. However, due to Roslyn and IDE quirks, you may occasionally need to rebuild your project or restart your IDE for changes to take effect.
 
---

## Support the Project

If you find this project useful, consider supporting it by [buying me a coffee](https://www.buymeacoffee.com/aglasencnik). Your support is greatly appreciated!

## Contributing

Contributions are welcome! If you have a feature to propose or a bug to fix, create a new pull request.

## License

This project is licensed under the [MIT License](https://github.com/aglasencnik/TypedIcons/blob/main/LICENSE).

## Acknowledgment

This project is built with the help of Iconify icon sets and API.

Project icon designed by [ThatSebastjan](https://github.com/ThatSebastjan).