[<AutoOpen>]
module Workspace

open EasyBuild.FileSystemProvider

type Workspace = RelativeFileSystem<".">
