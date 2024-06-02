module EasyBuild.Main

open Spectre.Console.Cli
open EasyBuild.Commands.Release
open EasyBuild.Commands.Test
open SimpleExec

[<EntryPoint>]
let main args =

    Command.Run("dotnet", "husky install")

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"

        config
            .AddCommand<TestCommand>("test")
            .WithDescription("Run the tests")
        |> ignore

        config
            .AddCommand<ReleaseCommand>("release")
            .WithDescription(
                "Package a new version of the library and publish it to NuGet. This also updates the demo."
            )
        |> ignore

    )

    app.Run(args)
