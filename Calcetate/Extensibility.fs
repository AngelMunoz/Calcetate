namespace Calcetate

open System
open System.IO
open System.Threading.Tasks
open CalceTypes


module Extensibility =
    open FSharp.Compiler.Interactive.Shell

    let private getSession stdin stdout stderr =
        let defConfig = FsiEvaluationSession.GetDefaultConfiguration()

        let argv =
            [| "fsi.exe"
               "--noninteractive"
               "--nologo"
               "--gui-" |]

        FsiEvaluationSession.Create(defConfig, argv, stdin, stdout, stderr, true)

    let getOnLoadFromScript filepath : PluginInfo voption =
        use stdin = new StringReader("")
        use stdout = new StringWriter()
        use stderr = new StringWriter()
        use session = getSession stdin stdout stderr
        let content = File.ReadAllText filepath

        match session.EvalInteractionNonThrowing(content) with
        | Choice1Of2 (value), diagnostic ->
            printfn "%A" diagnostic
            ()
        | Choice2Of2 (ex), diagnostic ->
            eprintfn "%O" ex
            printfn "%A" diagnostic
            ()

        printfn "%A" (session.GetBoundValues())
        printfn "%s" (stdout.ToString())
        eprintfn "%s" (stderr.ToString())

        match session.TryFindBoundValue "Plugin" with
        | Some bound ->
            bound.Value.ReflectionValue :?> PluginInfo
            |> ValueSome
        | None -> ValueNone
