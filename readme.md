# callcluster-dotnet
A C# callgraph extractor for callcluster.

# Callcluster json callgraph specification (Minimal json)
## Callgraph file
```
{
    "metadata":{
        "language": a string that expresses the source language or languages,
    }
    "calls":[
        {
            "from":index in the 'functions' array,
            "to":index in the 'functions' array,
        }
    ],
    "functions":[
        {
            "location":a string representing the filename and line of this function in the sources or somehow tracing to the source code,
            "name": name of this function,
        }
    ],
    "community":a community object
}
```

## Community object
```
{
    "name":the name of this community,
    "functions":A list of indices in the functions object,
    "communities":A list of community objects
}
```

# Publishing
```
dotnet tarball -r linux-x64
```
Other platforms: https://github.com/qmfrederik/dotnet-packaging

´´´
dotnet tarball -r linux-x64
´´´

´´´
dotnet zip -r win7-x64
´´´