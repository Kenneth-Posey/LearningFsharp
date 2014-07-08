REM build for .NET 3.5
REM msbuild Samples.Management.TypeProvider.fsproj /p:TargetFramework=net35
REM msbuild Samples.Management.TypeProvider.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net35

REM build for .NET 4.0
msbuild Samples.Management.TypeProvider.fsproj  /p:TargetFramework=net40
msbuild Samples.Management.TypeProvider.DesignTime.fsproj   /p:TypeProviderRuntimeFramework=net40

REM build for .NET 4.5
msbuild Samples.Management.TypeProvider.fsproj /p:TargetFramework=net45
msbuild Samples.Management.TypeProvider.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net45

REM build for .NET 3.5
REM msbuild Samples.Management.TypeProvider.fsproj /p:TargetFramework=net35 /p:Configuration=Release
REM msbuild Samples.Management.TypeProvider.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net35 /p:Configuration=Release

REM build for .NET 4.0
msbuild Samples.Management.TypeProvider.fsproj  /p:TargetFramework=net40 /p:Configuration=Release
msbuild Samples.Management.TypeProvider.DesignTime.fsproj   /p:TypeProviderRuntimeFramework=net40 /p:Configuration=Release

REM build for .NET 4.5
msbuild Samples.Management.TypeProvider.fsproj /p:TargetFramework=net45 /p:Configuration=Release
msbuild Samples.Management.TypeProvider.DesignTime.fsproj /p:TypeProviderRuntimeFramework=net45 /p:Configuration=Release
