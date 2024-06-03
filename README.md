# Fable.Package.SDK

[![NuGet](https://img.shields.io/nuget/v/EasyBuild.CommitLinter.svg)](https://www.nuget.org/packages/Fable.Package.SDK)

Fable.Package.SDK is a set of MSBuild targets and tasks that help you build and package Fable projects.

## Features 🚀

- Automatically add Fable specific tags to `PackageTags` based on the `FablePackageType` property
- Automatically include F# source files in the package if needed
- Check that at least one Fable target is defined in the `PackageTags` property
- Set `GenerateDocumentationFile` to `true`
- Set up your package for improving IDE experiences by setting:
    - `DebugType` to `embedded`
    - `EmbedUntrackedSources` to `true`

## Installation

```bash
dotnet add package Fable.Package.SDK
```

## Usage

### 1. Set the `FablePackageType` property

Set the `FablePackageType` property in your project file to one of the following values:

- `library`: If your package is a library that can be used by Fable.

    Examples of libraries could be [Fable.Promise](https://github.com/fable-compiler/fable-promise/), [Elmish](https://elmish.github.io/), [Thoth.Json](https://thoth-org.github.io//Thoth.Json/), [Feliz](https://zaid-ajaj.github.io/Feliz/)

    > This will include the source files in the package.

- `binding`: If your package consist of a set of API to make a native library available

    For example:

    - A package which makes an NPM package API available
    - A package which makes the Browser API available
    - A package which makes a cargo package API available
    
    <br/>

    > Only the DLL will be included in the package, allowing for a faster build and smaller package size.

### 2. Specify the targets

Choose one or more of the following tags:

- `fable-dart`: Dart is supported by the package
- `fable-dotnet`: .NET is supported by the package
- `fable-javascript`: JavaScript is supported by the package
- `fable-python`: Python is supported by the package
- `fable-rust`: Rust is supported by the package
- `fable-all`: Package is compatible with all Fable targets.

> [!WARNING]
> A package can be compatible with all targets if it depends only on packages that are also compatible with all targets.
>
> A package compatible with all targets cannot be a binding, as these are target-specific.

Example:

If your package supports only JavaScript you need to use `fable-javascript`

If your package supports both JavaScript and Python, you need to use `fable-javascript` and `fable-python`

### Example of use case

If your package is a binding which target JavaScript you need to write:

```xml
<PropertyGroup>
    <PackageTags>fable-javascript</PackageTags>
    <FablePackageType>binding</FablePackageType>
</PropertyGroup>
```

If your package is a library which targets JavaScript and Python you need to write:

```xml
<PropertyGroup>
    <PackageTags>fable-javascript;fable-python</PackageTags>
    <FablePackageType>library</FablePackageType>
</PropertyGroup>
```
