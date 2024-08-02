using Nuke.Common.ProjectModel;

sealed partial class Build
{
    static string GithubPublishVersion = "N/A"; 
    readonly AbsolutePath ArtifactsDirectory = RootDirectory / "output";
    readonly AbsolutePath ChangeLogPath = RootDirectory / "Changelog.md";

    protected override void OnBuildInitialized()
    {
        GithubPublishVersion = $"{1}{Solution.GenFusionsRevitCore_Servers3dContext.GetProperty("Version")}"; // In Nuget packages there will be Revit version instead of 1 at the beginning
        Configurations =
        [
            "Release*",
        ];
    }
}