(***hide***)

(***condition:prepare***)
#r "nuget: FSharpAux, 1.1.0"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: FSharp.Stats, 0.4.3"
#r "nuget: FSharp.Data, 4.2.7"

(***condition:ipynb***)
#if IPYNB
#r "nuget: FSharpAux, 1.1.0"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.16"
#r "nuget: FSharp.Stats, 0.4.3"
#r "nuget: FSharp.Data, 4.2.7"
#endif // IPYNB

open Plotly.NET

//This chart is named the same as the one in the test1.fsx script. The evaluation result below however, is the on from the other script, not the simple point chart that gets defined here.
//renaming this leads to "no value returned by any evaluator"
let dataChart = Chart.Point([1,2])

(*** condition: ipynb ***)
#if IPYNB
dataChart
#endif // IPYNB

(***hide***)
dataChart |> GenericChart.toChartHTML
(***include-it-raw***)