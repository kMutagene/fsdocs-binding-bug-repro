(**
[![Binder](/fsdocs-binding-bug-repro/images/badge-binder.svg)](https://mybinder.org/v2/gh/fslaborg/fslaborg.github.io/gh-pages?filepath=content/tutorials/test1.ipynb)&emsp;
[![Script](/fsdocs-binding-bug-repro/images/badge-script.svg)](/fsdocs-binding-bug-repro/content/tutorials/test1.fsx)&emsp;
[![Notebook](/fsdocs-binding-bug-repro/images/badge-notebook.svg)](/fsdocs-binding-bug-repro/content/tutorials/test1.ipynb)

# This script creates the chart

**Summary:** This tutorial demonstrates k means clustering with FSharp.Stats and how to visualize the results with Plotly.NET.

## Introduction

Clustering methods can be used to group elements of a huge data set based on their similarity. Elements sharing similar properties cluster together and can be reported as coherent group.
k-means clustering is a frequently used technique, that segregates the given data into k clusters with similar elements grouped in each cluster, but high variation between the clusters.
The algorithm to cluster a n-dimensional dataset can be fully described in the following 4 steps:

0 Initialize k n-dimensional centroids, that are randomly distributed over the data range.

1 Calculate the distance of each point to all centroids and assign it to the nearest one.

2 Reposition all centroids by calculating the average point of each cluster.

3 Repeat step 2-3 until convergence.

### Centroid initiation

Since the random initiation of centroids may influences the result, a second initiation algorithm is proposed (**cvmax**), that extract a set of medians from the dimension with maximum variance to initialize the centroids.

### Distance measure

While several distance metrics can be used (e.g. Manhattan distance or correlation measures) it is preferred to use Euclidean distance.
It is recommended to use a squared Euclidean distance. To not calculate the square root does not change the result but saves computation time.

*)
<img style="max-width:75%" src="../../images/kMeans.png"></img>
<br>
(**
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


## Loading data

*)
open FSharp.Data
open Deedle

// Retrieve data using the FSharp.Data package and read it as dataframe using the Deedle package
let rawData = Http.RequestString @"https://raw.githubusercontent.com/fslaborg/datasets/main/data/iris.csv"
let df = Frame.ReadCsvString(rawData)

df.Print()(* output: 
sepal_length sepal_width petal_length petal_width species    
0   -> 5.5          2.4         3.8          1.1         versicolor 
1   -> 4.9          3.1         1.5          0.1         setosa     
2   -> 7.6          3           6.6          2.1         virginica  
3   -> 5.6          2.8         4.9          2           virginica  
4   -> 6.1          3           4.9          1.8         virginica  
5   -> 6.3          3.4         5.6          2.4         virginica  
6   -> 6.2          2.8         4.8          1.8         virginica  
7   -> 7.2          3.2         6            1.8         virginica  
8   -> 6.9          3.2         5.7          2.3         virginica  
9   -> 4.9          3           1.4          0.2         setosa     
10  -> 5.4          3.9         1.7          0.4         setosa     
11  -> 7            3.2         4.7          1.4         versicolor 
12  -> 6.1          3           4.6          1.4         versicolor 
13  -> 5.4          3.7         1.5          0.2         setosa     
14  -> 7.2          3.6         6.1          2.5         virginica  
:      ...          ...         ...          ...         ...        
135 -> 7.7          3.8         6.7          2.2         virginica  
136 -> 5.7          3           4.2          1.2         versicolor 
137 -> 6.4          2.7         5.3          1.9         virginica  
138 -> 5.8          2.7         3.9          1.2         versicolor 
139 -> 5            2.3         3.3          1           versicolor 
140 -> 6.7          3.1         4.4          1.4         versicolor 
141 -> 6.3          3.3         6            2.5         virginica  
142 -> 4.9          2.4         3.3          1           versicolor 
143 -> 5.8          2.8         5.1          2.4         virginica  
144 -> 5            3.3         1.4          0.2         setosa     
145 -> 7.7          2.6         6.9          2.3         virginica  
146 -> 5.7          2.6         3.5          1           versicolor 
147 -> 5.9          3           5.1          1.8         virginica  
148 -> 6.8          3.2         5.9          2.3         virginica  
149 -> 5            3.6         1.4          0.2         setosa*)
(**
Let's take a first look at the data with heatmaps using Plotly.NET. Each of the 150 records consists of four measurements and a species identifier.
Since the species identifier occur several times (**Iris-virginica**, **Iris-versicolor**, and **Iris-setosa**), we create unique labels by adding the rows index to the species identifier.

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

// required to fit the species identifier on the left side of the heatmap(* output: 
<div id="5b376fd4-ee10-434d-adc7-2fac3b2efdfe"><!-- Plotly chart will be drawn inside this DIV --></div>
<script type="text/javascript">

            var renderPlotly_5b376fd4ee10434dadc72fac3b2efdfe = function() {
            var fsharpPlotlyRequire = requirejs.config({context:'fsharp-plotly',paths:{plotly:'https://cdn.plot.ly/plotly-2.6.3.min'}}) || require;
            fsharpPlotlyRequire(['plotly'], function(Plotly) {

            var data = [{"type":"heatmap","x":["sepal_length","sepal_width","petal_length","petal_width"],"y":["versicolor_0","setosa_1","virginica_2","virginica_3","virginica_4","virginica_5","virginica_6","virginica_7","virginica_8","setosa_9","setosa_10","versicolor_11","versicolor_12","setosa_13","virginica_14","virginica_15","setosa_16","setosa_17","setosa_18","virginica_19","versicolor_20","setosa_21","virginica_22","setosa_23","virginica_24","virginica_25","versicolor_26","versicolor_27","setosa_28","virginica_29","virginica_30","virginica_31","setosa_32","setosa_33","versicolor_34","setosa_35","setosa_36","versicolor_37","versicolor_38","versicolor_39","versicolor_40","versicolor_41","setosa_42","virginica_43","versicolor_44","versicolor_45","virginica_46","setosa_47","virginica_48","setosa_49","virginica_50","setosa_51","versicolor_52","virginica_53","virginica_54","setosa_55","versicolor_56","setosa_57","setosa_58","virginica_59","setosa_60","setosa_61","virginica_62","setosa_63","virginica_64","setosa_65","versicolor_66","versicolor_67","versicolor_68","versicolor_69","virginica_70","setosa_71","virginica_72","virginica_73","setosa_74","setosa_75","versicolor_76","versicolor_77","versicolor_78","versicolor_79","versicolor_80","virginica_81","versicolor_82","setosa_83","setosa_84","setosa_85","versicolor_86","setosa_87","versicolor_88","versicolor_89","versicolor_90","versicolor_91","versicolor_92","versicolor_93","setosa_94","setosa_95","virginica_96","setosa_97","versicolor_98","setosa_99","versicolor_100","virginica_101","setosa_102","setosa_103","virginica_104","versicolor_105","setosa_106","versicolor_107","virginica_108","versicolor_109","versicolor_110","virginica_111","virginica_112","virginica_113","versicolor_114","virginica_115","virginica_116","setosa_117","versicolor_118","setosa_119","versicolor_120","setosa_121","setosa_122","virginica_123","setosa_124","setosa_125","setosa_126","virginica_127","versicolor_128","virginica_129","virginica_130","setosa_131","virginica_132","versicolor_133","setosa_134","virginica_135","versicolor_136","virginica_137","versicolor_138","versicolor_139","versicolor_140","virginica_141","versicolor_142","virginica_143","setosa_144","virginica_145","versicolor_146","virginica_147","virginica_148","setosa_149"],"z":[[5.5,2.4,3.8,1.1],[4.9,3.1,1.5,0.1],[7.6,3.0,6.6,2.1],[5.6,2.8,4.9,2.0],[6.1,3.0,4.9,1.8],[6.3,3.4,5.6,2.4],[6.2,2.8,4.8,1.8],[7.2,3.2,6.0,1.8],[6.9,3.2,5.7,2.3],[4.9,3.0,1.4,0.2],[5.4,3.9,1.7,0.4],[7.0,3.2,4.7,1.4],[6.1,3.0,4.6,1.4],[5.4,3.7,1.5,0.2],[7.2,3.6,6.1,2.5],[6.3,2.8,5.1,1.5],[5.4,3.4,1.7,0.2],[5.8,4.0,1.2,0.2],[5.0,3.2,1.2,0.2],[6.5,3.0,5.8,2.2],[5.6,3.0,4.1,1.3],[4.3,3.0,1.1,0.1],[6.7,3.3,5.7,2.5],[4.5,2.3,1.3,0.3],[5.7,2.5,5.0,2.0],[6.3,2.7,4.9,1.8],[5.6,3.0,4.5,1.5],[5.6,2.5,3.9,1.1],[4.9,3.1,1.5,0.1],[7.7,3.0,6.1,2.3],[6.3,2.5,5.0,1.9],[6.4,3.1,5.5,1.8],[4.7,3.2,1.6,0.2],[5.5,4.2,1.4,0.2],[5.9,3.2,4.8,1.8],[5.0,3.4,1.6,0.4],[5.4,3.4,1.5,0.4],[6.0,2.7,5.1,1.6],[5.6,2.9,3.6,1.3],[6.3,3.3,4.7,1.6],[5.1,2.5,3.0,1.1],[6.4,3.2,4.5,1.5],[5.1,3.7,1.5,0.4],[6.7,3.0,5.2,2.3],[6.2,2.9,4.3,1.3],[6.8,2.8,4.8,1.4],[7.7,2.8,6.7,2.0],[4.8,3.0,1.4,0.1],[6.2,3.4,5.4,2.3],[4.8,3.4,1.9,0.2],[6.7,2.5,5.8,1.8],[5.1,3.5,1.4,0.2],[6.7,3.0,5.0,1.7],[6.9,3.1,5.1,2.3],[5.8,2.7,5.1,1.9],[4.8,3.1,1.6,0.2],[5.5,2.6,4.4,1.2],[5.1,3.8,1.6,0.2],[5.7,4.4,1.5,0.4],[6.7,3.3,5.7,2.1],[4.4,3.0,1.3,0.2],[5.0,3.5,1.3,0.3],[6.9,3.1,5.4,2.1],[5.0,3.5,1.6,0.6],[6.5,3.2,5.1,2.0],[4.6,3.6,1.0,0.2],[6.1,2.8,4.7,1.2],[5.0,2.0,3.5,1.0],[5.8,2.7,4.1,1.0],[6.0,3.4,4.5,1.6],[4.9,2.5,4.5,1.7],[5.7,3.8,1.7,0.3],[7.9,3.8,6.4,2.0],[7.2,3.0,5.8,1.6],[5.5,3.5,1.3,0.2],[4.6,3.2,1.4,0.2],[6.4,2.9,4.3,1.3],[6.9,3.1,4.9,1.5],[6.0,2.9,4.5,1.5],[5.8,2.6,4.0,1.2],[5.2,2.7,3.9,1.4],[7.4,2.8,6.1,1.9],[5.5,2.4,3.7,1.0],[5.1,3.4,1.5,0.2],[4.8,3.4,1.6,0.2],[5.0,3.0,1.6,0.2],[5.7,2.8,4.1,1.3],[4.6,3.1,1.5,0.2],[5.9,3.0,4.2,1.5],[6.1,2.8,4.0,1.3],[5.7,2.8,4.5,1.3],[6.0,2.2,4.0,1.0],[5.4,3.0,4.5,1.5],[6.1,2.9,4.7,1.4],[5.4,3.9,1.3,0.4],[5.2,4.1,1.5,0.1],[7.3,2.9,6.3,1.8],[4.7,3.2,1.3,0.2],[6.3,2.5,4.9,1.5],[5.3,3.7,1.5,0.2],[6.6,2.9,4.6,1.3],[6.3,2.9,5.6,1.8],[4.4,3.2,1.3,0.2],[4.4,2.9,1.4,0.2],[6.5,3.0,5.5,1.8],[6.3,2.3,4.4,1.3],[4.9,3.1,1.5,0.1],[6.6,3.0,4.4,1.4],[6.4,2.8,5.6,2.1],[6.5,2.8,4.6,1.5],[5.5,2.3,4.0,1.3],[6.7,3.1,5.6,2.4],[6.4,3.2,5.3,2.3],[6.8,3.0,5.5,2.1],[5.7,2.9,4.2,1.3],[5.8,2.7,5.1,1.9],[6.0,3.0,4.8,1.8],[5.2,3.4,1.4,0.2],[6.7,3.1,4.7,1.5],[5.1,3.5,1.4,0.3],[5.5,2.5,4.0,1.3],[4.8,3.0,1.4,0.3],[5.1,3.8,1.5,0.3],[6.4,2.8,5.6,2.2],[5.0,3.4,1.5,0.2],[5.1,3.8,1.9,0.4],[5.1,3.3,1.7,0.5],[6.5,3.0,5.2,2.0],[5.6,2.7,4.2,1.3],[6.0,2.2,5.0,1.5],[7.1,3.0,5.9,2.1],[4.6,3.4,1.4,0.3],[6.1,2.6,5.6,1.4],[6.2,2.2,4.5,1.5],[5.2,3.5,1.5,0.2],[7.7,3.8,6.7,2.2],[5.7,3.0,4.2,1.2],[6.4,2.7,5.3,1.9],[5.8,2.7,3.9,1.2],[5.0,2.3,3.3,1.0],[6.7,3.1,4.4,1.4],[6.3,3.3,6.0,2.5],[4.9,2.4,3.3,1.0],[5.8,2.8,5.1,2.4],[5.0,3.3,1.4,0.2],[7.7,2.6,6.9,2.3],[5.7,2.6,3.5,1.0],[5.9,3.0,5.1,1.8],[6.8,3.2,5.9,2.3],[5.0,3.6,1.4,0.2]]}];
            var layout = {"width":600,"height":600,"template":{"layout":{"title":{"x":0.05},"font":{"color":"rgba(42, 63, 95, 1.0)"},"paper_bgcolor":"rgba(255, 255, 255, 1.0)","plot_bgcolor":"rgba(229, 236, 246, 1.0)","autotypenumbers":"strict","colorscale":{"diverging":[[0.0,"#8e0152"],[0.1,"#c51b7d"],[0.2,"#de77ae"],[0.3,"#f1b6da"],[0.4,"#fde0ef"],[0.5,"#f7f7f7"],[0.6,"#e6f5d0"],[0.7,"#b8e186"],[0.8,"#7fbc41"],[0.9,"#4d9221"],[1.0,"#276419"]],"sequential":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]],"sequentialminus":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]},"hovermode":"closest","hoverlabel":{"align":"left"},"coloraxis":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"geo":{"showland":true,"landcolor":"rgba(229, 236, 246, 1.0)","showlakes":true,"lakecolor":"rgba(255, 255, 255, 1.0)","subunitcolor":"rgba(255, 255, 255, 1.0)","bgcolor":"rgba(255, 255, 255, 1.0)"},"mapbox":{"style":"light"},"polar":{"bgcolor":"rgba(229, 236, 246, 1.0)","radialaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""},"angularaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","ticks":""}},"scene":{"xaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"yaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true},"zaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","gridwidth":2.0,"zerolinecolor":"rgba(255, 255, 255, 1.0)","backgroundcolor":"rgba(229, 236, 246, 1.0)","showbackground":true}},"ternary":{"aaxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"baxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"caxis":{"ticks":"","linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)"},"bgcolor":"rgba(229, 236, 246, 1.0)"},"xaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"yaxis":{"title":{"standoff":15},"ticks":"","automargin":true,"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","zerolinecolor":"rgba(255, 255, 255, 1.0)","zerolinewidth":2.0},"annotationdefaults":{"arrowcolor":"#2a3f5f","arrowhead":0,"arrowwidth":1},"shapedefaults":{"line":{"color":"rgba(42, 63, 95, 1.0)"}},"colorway":["rgba(99, 110, 250, 1.0)","rgba(239, 85, 59, 1.0)","rgba(0, 204, 150, 1.0)","rgba(171, 99, 250, 1.0)","rgba(255, 161, 90, 1.0)","rgba(25, 211, 243, 1.0)","rgba(255, 102, 146, 1.0)","rgba(182, 232, 128, 1.0)","rgba(255, 151, 255, 1.0)","rgba(254, 203, 82, 1.0)"]},"data":{"bar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}},"errorx":{"color":"rgba(42, 63, 95, 1.0)"},"errory":{"color":"rgba(42, 63, 95, 1.0)"}}],"barpolar":[{"marker":{"line":{"color":"rgba(229, 236, 246, 1.0)","width":0.5},"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"carpet":[{"aaxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"},"baxis":{"linecolor":"rgba(255, 255, 255, 1.0)","gridcolor":"rgba(255, 255, 255, 1.0)","endlinecolor":"rgba(42, 63, 95, 1.0)","minorgridcolor":"rgba(255, 255, 255, 1.0)","startlinecolor":"rgba(42, 63, 95, 1.0)"}}],"choropleth":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"contourcarpet":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"heatmap":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"heatmapgl":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram":[{"marker":{"pattern":{"fillmode":"overlay","size":10,"solidity":0.2}}}],"histogram2d":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"histogram2dcontour":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"mesh3d":[{"colorbar":{"outlinewidth":0.0,"ticks":""}}],"parcoords":[{"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"pie":[{"automargin":true}],"scatter":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatter3d":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}},"line":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattercarpet":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergeo":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattergl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scattermapbox":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolar":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterpolargl":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"scatterternary":[{"marker":{"colorbar":{"outlinewidth":0.0,"ticks":""}}}],"surface":[{"colorbar":{"outlinewidth":0.0,"ticks":""},"colorscale":[[0.0,"#0d0887"],[0.1111111111111111,"#46039f"],[0.2222222222222222,"#7201a8"],[0.3333333333333333,"#9c179e"],[0.4444444444444444,"#bd3786"],[0.5555555555555556,"#d8576b"],[0.6666666666666666,"#ed7953"],[0.7777777777777778,"#fb9f3a"],[0.8888888888888888,"#fdca26"],[1.0,"#f0f921"]]}],"table":[{"cells":{"fill":{"color":"rgba(235, 240, 248, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}},"header":{"fill":{"color":"rgba(200, 212, 227, 1.0)"},"line":{"color":"rgba(255, 255, 255, 1.0)"}}}]}},"margin":{"l":100.0},"title":{"text":"raw iris data"}};
            var config = {"responsive":true};
            Plotly.newPlot('5b376fd4-ee10-434d-adc7-2fac3b2efdfe', data, layout, config);
});
            };
            if ((typeof(requirejs) !==  typeof(Function)) || (typeof(requirejs.config) !== typeof(Function))) {
                var script = document.createElement("script");
                script.setAttribute("src", "https://cdnjs.cloudflare.com/ajax/libs/require.js/2.3.6/require.min.js");
                script.onload = function(){
                    renderPlotly_5b376fd4ee10434dadc72fac3b2efdfe();
                };
                document.getElementsByTagName("head")[0].appendChild(script);
            }
            else {
                renderPlotly_5b376fd4ee10434dadc72fac3b2efdfe();
            }
</script>
*)
(**
soos
e

*)

