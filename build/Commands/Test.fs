module EasyBuild.Commands.Test

open Spectre.Console.Cli
open SimpleExec
open LibGit2Sharp
open EasyBuild.Workspace
open System.Linq
open System.Text.RegularExpressions
open System
open System.IO
open BlackFox.CommandLine
open EasyBuild.Utils.Dotnet

type TestSettings() =
    inherit CommandSettings()

    [<CommandOption("-w|--watch")>]
    member val IsWatch = false with get, set

type TestCommand() =
    inherit Command<TestSettings>()
    interface ICommandLimiter<TestSettings>

    override __.Execute(context, settings) =
        printfn "CWD: %s" (Environment.CurrentDirectory)

        Command.Run(
            "dotnet",
            CmdLine.empty
            |> CmdLine.appendIf settings.IsWatch "watch"
            |> CmdLine.appendRaw "test"
            |> CmdLine.appendRaw "-v n"
            |> CmdLine.toString,
            workingDirectory = Workspace.tests.``.``
        )

        0
