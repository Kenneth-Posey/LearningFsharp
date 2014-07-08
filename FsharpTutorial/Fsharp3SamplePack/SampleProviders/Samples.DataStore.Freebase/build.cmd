REM build for .NET 3.5
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=net35
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net35

REM build for .NET Portable Profile 47
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=portable47
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=portable47

REM build for .NET Portable Silverlight 5
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=sl5
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=sl5

REM build for .NET 4.0
msbuild Samples.DataStore.Freebase.fsproj  /p:TargetFramework=net40
msbuild Samples.DataStore.Freebase.DesignTime.fsproj   /p:TypeProviderRuntimeFramework=net40

REM build for .NET 4.5
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=net45
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net45

REM build for .NET 3.5
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=net35 /p:Configuration=Release
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net35 /p:Configuration=Release

REM build for .NET Portable Profile 47
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=portable47 /p:Configuration=Release
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=portable47 /p:Configuration=Release

REM build for .NET Portable Silverlight 5
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=sl5 /p:Configuration=Release
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=sl5 /p:Configuration=Release

REM build for .NET 4.0
msbuild Samples.DataStore.Freebase.fsproj  /p:TargetFramework=net40 /p:Configuration=Release
msbuild Samples.DataStore.Freebase.DesignTime.fsproj   /p:TypeProviderRuntimeFramework=net40 /p:Configuration=Release

REM build for .NET 4.5
msbuild Samples.DataStore.Freebase.fsproj /p:TargetFramework=net45 /p:Configuration=Release
msbuild Samples.DataStore.Freebase.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net45 /p:Configuration=Release

