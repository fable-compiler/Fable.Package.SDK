module Fable.Package.SDK.Tests

// open Microsoft.VisualStudio.TestTools.UnitTesting
open SimpleExec
open NUnit.Framework
open System.IO
open System.IO.Compression

let private expectToFailWithMessage projectName (message: string) =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"pack %s{projectName}",
                handleExitCode = System.Func<int, bool>(fun exitCode -> exitCode = 1)
            )

        Assert.That(stdout, Contains.Substring(message))
    }

[<Test>]
let ``Empty FablePackageType property should report an error`` () =
    expectToFailWithMessage
        Workspace.fixtures.invalid.``EmptyFablePackageTypeProperty.fsproj``
        "The property FablePackageType must be set to 'library' or 'binding'."

[<Test>]
let ``Invalid FablePackageType property should report an error`` () =
    expectToFailWithMessage
        Workspace.fixtures.invalid.``InvalidFablePackageTypeProperty.fsproj``
        "The property FablePackageType must be set to 'library' or 'binding'."

[<Test>]
let ``Missing FablePackageType property should report an error`` () =
    expectToFailWithMessage
        Workspace.fixtures.invalid.``MissingFablePackageTypeProperty.fsproj``
        "The property FablePackageType must be set to 'library' or 'binding'."

[<Test>]
let ``Missing FableTarget property should report an error`` () =
    task {
        do!
            expectToFailWithMessage
                Workspace.fixtures.invalid.``MissingFableTarget.fsproj``
                "You need to set at least one of Fable target via the PackageTags property. Possible values are: fable-dart, fable-dotnet, fable-javascript, fable-python, fable-rust, fable-all."

        do!
            expectToFailWithMessage
                Workspace.fixtures.invalid.``MissingFableTargetMultiTFM.fsproj``
                "You need to set at least one of Fable target via the PackageTags property. Possible values are: fable-dart, fable-dotnet, fable-javascript, fable-python, fable-rust, fable-all."
    }

[<Test>]
let ``You cannot set both FablePackageType and FableTarget properties`` () =
    task {
        do!
            expectToFailWithMessage
                Workspace.fixtures.invalid.``LibraryWithFableBindingTag.fsproj``
                "You cannot set both 'fable-library' and 'fable-binding' tags. If you used `PackageTags` to set 'fable-library' or 'fable-binding' tags, you should remove them. It will be automatically added based on the FablePackageType property."

        do!
            expectToFailWithMessage
                Workspace.fixtures.invalid.``BindingWithFableLibraryTag.fsproj``
                "You cannot set both 'fable-library' and 'fable-binding' tags. If you used `PackageTags` to set 'fable-library' or 'fable-binding' tags, you should remove them. It will be automatically added based on the FablePackageType property."
    }

module ``fable tag`` =

    [<Test>]
    let ``fable tag should be automatically added`` () =
        task {
            let! stdout, _ =
                Command.ReadAsync(
                    "dotnet",
                    $"msbuild %s{Workspace.fixtures.valid.``Empty.fsproj``} --getProperty:PackageTags"
                )

            Assert.That(stdout.Trim(), Is.EqualTo(";fable"))
        }

    [<Test>]
    let ``fable tag should not be added if already present`` () =
        task {
            let! stdout, _ =
                Command.ReadAsync(
                    "dotnet",
                    $"msbuild %s{Workspace.fixtures.valid.``FablePackageTags.fsproj``} --getProperty:PackageTags"
                )

            Assert.That(stdout.Trim(), Is.EqualTo("fable"))
        }

module ``fable-library tag addition`` =

    [<Test>]
    let ``fable-library tag should be added if FablePackageType is set to library`` () =
        task {
            let! stdout, _ =
                Command.ReadAsync(
                    "dotnet",
                    $"msbuild %s{Workspace.fixtures.valid.``Library.fsproj``} --getProperty:PackageTags"
                )

            Assert.That(stdout.Trim(), Is.EqualTo(";fable;fable-library"))
        }

    [<Test>]
    let ``fable-library tag should not be added if already present`` () =
        task {
            let! stdout, _ =
                Command.ReadAsync(
                    "dotnet",
                    $"msbuild %s{Workspace.fixtures.valid.``LibraryWithTagsPresent.fsproj``} --getProperty:PackageTags"
                )

            Assert.That(stdout.Trim(), Is.EqualTo("fable-library;fable"))
        }

module ``fable-binding tag addition`` =

    [<Test>]
    let ``fable-binding tag should be added if FablePackageType is set to binding`` () =
        task {
            let! stdout, _ =
                Command.ReadAsync(
                    "dotnet",
                    $"msbuild %s{Workspace.fixtures.valid.``Binding.fsproj``} --getProperty:PackageTags"
                )

            Assert.That(stdout.Trim(), Is.EqualTo(";fable;fable-binding"))
        }

    [<Test>]
    let ``fable-binding tag should not be added if already present`` () =
        task {
            let! stdout, _ =
                Command.ReadAsync(
                    "dotnet",
                    $"msbuild %s{Workspace.fixtures.valid.``BindingWithTagsPresent.fsproj``} --getProperty:PackageTags"
                )

            Assert.That(stdout.Trim(), Is.EqualTo("fable-binding;fable"))
        }

[<Test>]
let ``should support fable-dart target`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_Dart.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(stdout.Trim(), Is.EqualTo("fable-dart;fable;fable-library"))
    }

[<Test>]
let ``should support fable-dotnet target`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_Dotnet.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(stdout.Trim(), Is.EqualTo("fable-dotnet;fable;fable-library"))
    }

[<Test>]
let ``should support fable-javascript target`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_JavaScript.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(stdout.Trim(), Is.EqualTo("fable-javascript;fable;fable-library"))
    }

[<Test>]
let ``should support fable-python target`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_Python.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(stdout.Trim(), Is.EqualTo("fable-python;fable;fable-library"))
    }

[<Test>]
let ``should support fable-rust target`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_Rust.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(stdout.Trim(), Is.EqualTo("fable-rust;fable;fable-library"))
    }

[<Test>]
let ``should support fable-all target`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_All.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(stdout.Trim(), Is.EqualTo("fable-all;fable;fable-library"))
    }

[<Test>]
let ``should support multiple fable targets`` () =
    task {
        let! stdout, _ =
            Command.ReadAsync(
                "dotnet",
                $"msbuild %s{Workspace.fixtures.valid.``FableTarget_Rust_Python_JavaScript.fsproj``} --getProperty:PackageTags"
            )

        Assert.That(
            stdout.Trim(),
            Is.EqualTo("fable-rust;fable-javascript;fable-python;fable;fable-library")
        )
    }

[<Test>]
let ``should include the source file and the project file under 'fable' folder`` () =
    task {
        // Make sure we work with a fresh nupkg file
        let fileInfo =
            VirtualWorkspace.fixtures.valid.``library-with-files``.bin.Release.``MyLibrary.1.0.0.nupkg``
            |> FileInfo

        if fileInfo.Exists then
            fileInfo.Delete()

        Command.Run(
            "dotnet",
            $"pack %s{Workspace.fixtures.valid.``library-with-files``.``MyLibrary.fsproj``}"
        )

        let archive =
            ZipFile.OpenRead(
                VirtualWorkspace.fixtures.valid.``library-with-files``.bin.Release.``MyLibrary.1.0.0.nupkg``
            )

        let entries = archive.Entries |> Seq.map (fun entry -> entry.FullName) |> Seq.toList

        Assert.That(entries, Contains.Item("fable/Entry.fs"))

        Assert.That(entries, Contains.Item("fable/MyLibrary.fsproj"))
    }

[<Test>]
let ``should include the source file and the project file under 'fable' folder multi tfm`` () =
    task {
        // Make sure we work with a fresh nupkg file
        let fileInfo =
            VirtualWorkspace.fixtures.valid.``library-with-files``.bin.Release.``MyLibraryMultiTFM.1.0.0.nupkg``
            |> FileInfo

        if fileInfo.Exists then
            fileInfo.Delete()

        Command.Run(
            "dotnet",
            $"pack %s{Workspace.fixtures.valid.``library-with-files``.``MyLibraryMultiTFM.fsproj``}"
        )

        let archive =
            ZipFile.OpenRead(
                VirtualWorkspace.fixtures.valid.``library-with-files``.bin.Release.``MyLibraryMultiTFM.1.0.0.nupkg``
            )

        let entries = archive.Entries |> Seq.map (fun entry -> entry.FullName) |> Seq.toList

        Assert.That(entries, Contains.Item("fable/Entry.fs"))

        Assert.That(entries, Contains.Item("fable/MyLibraryMultiTFM.fsproj"))

        Assert.That(entries, Contains.Item("lib/netstandard2.0/MyLibraryMultiTFM.dll"))

        Assert.That(entries, Contains.Item("lib/net8.0/MyLibraryMultiTFM.dll"))
    }
