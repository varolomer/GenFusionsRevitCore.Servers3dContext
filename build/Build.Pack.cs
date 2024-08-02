using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using System.IO;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build
{
    const string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Secret][Parameter] string NugetApiKey;

    Target Pack => _ => _
        //.DependsOn(Clean)
        .DependsOn(PublishGitHub)
        .Executes(() =>
        {
            Log.Information("=================================");
            foreach (var config in Solution.GenFusionsRevitCore_Servers3dContext.Configurations)
            {
                if (!config.Value.Contains("Release"))
                {
                    continue;
                }
                string revitVersion = GetRevitVersionFromConfig(config.Key);
                if (revitVersion == null)
                {
                    continue;
                }
                string configName = config.Value.Split('|')[0].Trim();
                string version =  Solution.GenFusionsRevitCore_Servers3dContext.GetProperty("Version");
                string assemblyVersion = $"{revitVersion}{version}";
                string description = "A standalone utility library which wraps the functionality of DirectContext3D and exposes convinient functions to easily draw shapes to the Revit canvas.";
                
                Log.Information($"AssemblyVersion: {assemblyVersion}");
                Log.Information($"configName: {configName}");

                DotNetPack(settings => settings
                    .SetProject(Solution.GenFusionsRevitCore_Servers3dContext)
                    .SetConfiguration(configName)
                    .SetVersion(assemblyVersion)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetVerbosity(DotNetVerbosity.minimal)
                    .SetDescription(description));
                
                Log.Information("-----");
            }

            //foreach (var configuration in GlobBuildConfigurations())

        });

    Target NuGetPush => _ => _
    .DependsOn(Pack)
    //.Requires(() => NugetApiKey)
    .Executes(() =>
    {
        foreach (var package in ArtifactsDirectory.GlobFiles("*.nupkg"))
        {
            DotNetNuGetPush(settings => settings
                .SetTargetPath(package)
                .SetSource(NugetApiUrl)
                .SetApiKey(NugetApiKey));
        }
    });

    public string GetRevitVersionFromConfig(string config)
    {
        if (config.Contains("R20")) return "2020";
        else if (config.Contains("R21")) return "2021";
        else if (config.Contains("R22")) return "2022";
        else if (config.Contains("R23")) return "2023";
        else if (config.Contains("R24")) return "2024";
        else if (config.Contains("R25")) return "2025";
        else return null;
    }

}