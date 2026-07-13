module EasyBuild.Main

open Spectre.Console.Cli
open EasyBuild.Commands.Test
open SimpleExec

[<EntryPoint>]
let main args =

    Command.Run("dotnet", "husky install")

    let app = CommandApp()

    app.Configure(fun config ->
        config.Settings.ApplicationName <- "./build.sh"

        config.AddCommand<TestCommand>("test").WithDescription("Run the tests")
        |> ignore
    )

    app.Run(args)
