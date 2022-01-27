# fsdocs-binding-bug-repro
A demonstration of the wrong value being included as evaluation result

there are 2 files:
- test1.fsx, which defines a heatmap chart and binds it to the name `dataChart`
- test2.fsx, which defines a simple point chart and binds it to the name `dataChart` as well

The problem: both rendered htmls contain the chart from test1.fsx.

To reproduce:

- download repo
- `dotnet tool restore`
- `dotnet fsdocs watch --eval`

The error is also reproducible via github actions, using .NET 5.0.404 on ubuntu: kmutagene.github.io/fsdocs-binding-bug-repro
