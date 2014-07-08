msbuild Proxy\HadoopProxy.fsproj /p:TargetFramework=net40 
msbuild Proxy\HadoopHiveProxyLib.fsproj /p:TargetFramework=net40 
msbuild Proxy\HadoopHdfsProxyLib.fsproj /p:TargetFramework=net40 
msbuild Samples.Hadoop.fsproj /p:TargetFramework=net40 
msbuild Samples.Hadoop.fsproj /p:TargetFramework=net45
msbuild Samples.Hadoop.fsproj /p:TargetFramework=sl5-compiler 

msbuild Proxy\HadoopProxy.fsproj /p:TargetFramework=net40 /p:Configuration=Release
msbuild Proxy\HadoopHiveProxyLib.fsproj /p:TargetFramework=net40  /p:Configuration=Release
msbuild Proxy\HadoopHdfsProxyLib.fsproj /p:TargetFramework=net40  /p:Configuration=Release
msbuild Samples.Hadoop.fsproj /p:TargetFramework=net40  /p:Configuration=Release
msbuild Samples.Hadoop.fsproj /p:TargetFramework=net45 /p:Configuration=Release
msbuild Samples.Hadoop.fsproj /p:TargetFramework=sl5-compiler  /p:Configuration=Release


exit /b 0
