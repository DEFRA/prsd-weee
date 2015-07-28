& $env:TF_BUILD_SOURCESDIRECTORY\tools\NuGet\nuget.exe restore $env:TF_BUILD_SOURCESDIRECTORY\src\EA.Weee.sln

& C:\SonarCube\SonarQube.MSBuild.Runner-0.9\SonarQube.MSBuild.Runner.exe /key:EA.Weee /name:EA.Weee /version:version1.0