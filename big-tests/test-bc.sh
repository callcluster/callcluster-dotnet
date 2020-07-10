wget https://github.com/bcgit/bc-csharp/archive/master.zip
unzip master.zip
cd ..
dotnet run --project callcluster-dotnet/callcluster-dotnet.csproj -- big-tests/bc-csharp-master/csharp.sln

