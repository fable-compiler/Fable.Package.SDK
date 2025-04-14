[<AutoOpen>]
module Workspace

open EasyBuild.FileSystemProvider

type Workspace = RelativeFileSystem<".">

type VirtualWorkspace = VirtualFileSystem<".",
"""
temp/
fixtures/
    valid/
        library-with-files/
            bin/
                Release/
                    MyLibrary.1.0.0.nupkg
        library-with-files-multi-tfm/
            bin/
                Release/
                    MyLibraryMultiTFM.1.0.0.nupkg
""">
