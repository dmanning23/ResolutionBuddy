nuget pack .\ResolutionBuddy.nuspec -IncludeReferencedProjects -Prop Configuration=Release
nuget push *.nupkg