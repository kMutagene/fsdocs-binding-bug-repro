(***hide***)

(***condition:prepare***)
#r "nuget: Deedle, 2.5.0"
#r "nuget: FSharp.Stats, 0.4.3"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: FSharp.Data, 4.2.7"

(***condition:ipynb***)
#if IPYNB
#r "nuget: Deedle, 2.5.0"
#r "nuget: FSharp.Stats, 0.4.3"
#r "nuget: Newtonsoft.Json, 13.0.1"
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.16"
#r "nuget: FSharp.Data, 4.2.7"
#endif // IPYNB

(**

[![Binder]({{root}}images/badge-binder.svg)](https://mybinder.org/v2/gh/fslaborg/fslaborg.github.io/gh-pages?filepath=content/tutorials/{{fsdocs-source-basename}}.ipynb)&emsp;
[![Script]({{root}}images/badge-script.svg)]({{root}}content/tutorials/{{fsdocs-source-basename}}.fsx)&emsp;
[![Notebook]({{root}}images/badge-notebook.svg)]({{root}}content/tutorials/{{fsdocs-source-basename}}.ipynb)


# This script creates the chart

_Summary:_ This tutorial demonstrates k means clustering with FSharp.Stats and how to visualize the results with Plotly.NET.

## Introduction

Clustering methods can be used to group elements of a huge data set based on their similarity. Elements sharing similar properties cluster together and can be reported as coherent group.
k-means clustering is a frequently used technique, that segregates the given data into k clusters with similar elements grouped in each cluster, but high variation between the clusters.
The algorithm to cluster a n-dimensional dataset can be fully described in the following 4 steps:

  1. Initialize k n-dimensional centroids, that are randomly distributed over the data range.
  2. Calculate the distance of each point to all centroids and assign it to the nearest one.
  3. Reposition all centroids by calculating the average point of each cluster.
  4. Repeat step 2-3 until convergence.

### Centroid initiation

Since the random initiation of centroids may influences the result, a second initiation algorithm is proposed (_cvmax_), that extract a set of medians from the dimension with maximum variance to initialize the centroids. 

### Distance measure

While several distance metrics can be used (e.g. Manhattan distance or correlation measures) it is preferred to use Euclidean distance.
It is recommended to use a squared Euclidean distance. To not calculate the square root does not change the result but saves computation time.

<img style="max-width:75%" src="../../images/kMeans.png"></img>

<br>


For demonstration of k-means clustering, the classic iris data set is used, which consists of 150 records, each of which contains four measurements and a species identifier.

## Referencing packages

```fsharp
// Packages hosted by the Fslab community
#r "nuget: Deedle"
#r "nuget: FSharp.Stats"
// third party .net packages 
#r "nuget: Plotly.NET, 2.0.0-preview.16"
#r "nuget: Plotly.NET.Interactive, 2.0.0-preview.12"
#r "nuget: FSharp.Data"
```

*)

(**
## Loading data
*)
open FSharp.Data
open Deedle

// Retrieve data using the FSharp.Data package and read it as dataframe using the Deedle package
let rawData = Http.RequestString @"https://raw.githubusercontent.com/fslaborg/datasets/main/data/iris.csv"
let df = Frame.ReadCsvString(rawData)

df.Print()

(*** include-output ***)

(**

Let's take a first look at the data with heatmaps using Plotly.NET. Each of the 150 records consists of four measurements and a species identifier. 
Since the species identifier occur several times (_Iris-virginica_, _Iris-versicolor_, and _Iris-setosa_), we create unique labels by adding the rows index to the species identifier.

*)
open Plotly.NET

let colNames = ["sepal_length";"sepal_width";"petal_length";"petal_width"]

// isolate data as float [] []
let data = 
    Frame.dropCol "species" df
    |> Frame.toJaggedArray

//isolate labels as seq<string>
let labels = 
    Frame.getCol "species" df
    |> Series.values
    |> Seq.mapi (fun i s -> sprintf "%s_%i" s i)

let dataChart = 
    Chart.Heatmap(data,colNames=colNames,rowNames=labels)
    // required to fit the species identifier on the left side of the heatmap
    |> Chart.withMarginSize(Left=100.)
    |> Chart.withTitle "raw iris data"

// required to fit the species identifier on the left side of the heatmap

(*** condition: ipynb ***)
#if IPYNB
dataChart
#endif // IPYNB

(***hide***)
dataChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**soos
e
*)
