open Calcetate
open System
open System.IO

let plugin = Extensibility.getOnLoadFromScript (Path.GetFullPath "./Plugin.fsx")

task {
    match plugin with
    | ValueSome plugin ->

        let! newContent =
            task {
                match plugin.load with
                | Some onLoad ->
                    try
                        let! result =
                            onLoad
                                { loader = "xml"
                                  source = Path.GetFullPath "./Calcetate.fsproj"
                                  url = Uri("/files/Calcetate.fsproj") }

                        return Some result
                    with
                    | ex ->
                        eprintfn "%s" ex.Message
                        return None
                | None -> return None
            }

        printfn $"%A{newContent}"
    | ValueNone -> printfn "Not Found"
}
|> Async.AwaitTask
|> Async.RunSynchronously
