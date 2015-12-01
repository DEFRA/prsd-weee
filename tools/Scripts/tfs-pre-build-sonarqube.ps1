& $env:TF_BUILD_SOURCESDIRECTORY\tools\NuGet\nuget.exe restore $env:TF_BUILD_SOURCESDIRECTORY\src\EA.Weee.sln

& C:\SonarCube\MSBuild.SonarQube.Runner-1.0.1\MSBuild.SonarQube.Runner.exe /key:EA.Weee /name:EA.Weee /version:version1.0