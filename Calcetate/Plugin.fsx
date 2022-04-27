#i "nuget: /Users/tuna/repos/Calcetate/CalceTypes/nupkg"
#i "nuget: https://api.nuget.org/v3/index.json"
#r "nuget: Newtonsoft.Json"
#r "nuget: CalceTypes"

open System.IO
open System.Xml
open Newtonsoft.Json

open type System.Text.Encoding

open CalceTypes


let onLoad: OnLoadCallback =
    let onLoad =
        fun (args: OnLoadArgs) ->
            task {
                let doc = XmlDocument()
                let! content = File.ReadAllTextAsync(args.source)
                doc.LoadXml(content)

                let content = JsonConvert.SerializeXmlNode doc |> UTF8.GetBytes

                return
                    { content = content
                      mimeType = "application/json" }
            }

    onLoad



let Plugin =
    { name = "xml-to-json"
      build = None
      copy = None
      virtualize = None
      load = Some onLoad }
