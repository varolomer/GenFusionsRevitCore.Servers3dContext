using System.Text;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Octokit;

sealed partial class Build
{
    Target PublishGitHub => _ => _
        .DependsOn(Compile)
        .Requires(() => GitHubToken)
        .Requires(() => GitRepository)
        //.OnlyWhenStatic(() => IsServerBuild && GitRepository.IsOnMainOrMasterBranch())
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(Solution.Name))
            {
                Credentials = new Credentials(GitHubToken)
            };

            var gitHubName = GitRepository.GetGitHubName();
            var gitHubOwner = GitRepository.GetGitHubOwner();

            //ValidateRelease();
            await CheckTagsAsync(gitHubOwner, gitHubName, GithubPublishVersion);
            var artifacts = GetDllFilesAsZipFile(GithubPublishVersion);
            var changelog = CreateGithubChangelog();

            Assert.NotEmpty(artifacts, "No artifacts were found to create the Release");

            var newRelease = new NewRelease(GithubPublishVersion)
            {
                Name = GithubPublishVersion,
                Body = changelog,
                TargetCommitish = GitRepository.Commit
            };

            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(gitHubOwner, gitHubName, newRelease);
            await UploadArtifactsAsync(release, artifacts);
        });

    List<string> GetDllFilesAsZipFile(string version)
    {
        string keyword = "Release";

        // Get all directories that contain the keyword "Release"

        var binFolder = Solution.GenFusionsRevitCore_Servers3dContext.Directory / "bin";
        var zipFolder = ArtifactsDirectory / $"Binariers";
        var zipName = ArtifactsDirectory / $"Binaries.zip";
        zipFolder.CreateOrCleanDirectory();
        string[] directories = Directory.GetDirectories(binFolder, "*Release*", SearchOption.AllDirectories);

        // Iterate through each directory and get all DLL files
        foreach (string directory in directories)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            DirectoryInfo newReleaseDir = Directory.CreateDirectory(Path.Combine(zipFolder, dirInfo.Name));

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(dirInfo.FullName, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(dirInfo.FullName, newReleaseDir.FullName), true);
            }
        }

        zipFolder.ZipTo(zipName);
        Assert.True(zipName.FileExists());

        return new List<string>(){ zipName };

    }

    void ValidateRelease()
    {
        var tags = GitTasks.Git("describe --tags --abbrev=0 --always", logInvocation: false, logOutput: false);
        var latestTag = tags.First().Text;
        if (latestTag == GitRepository.Commit) return;

        Assert.False(latestTag == GithubPublishVersion, $"A Release with the specified tag already exists in the repository: {GithubPublishVersion}");
        Log.Information("Version: {Version}", GithubPublishVersion);
    }

    static async Task CheckTagsAsync(string gitHubOwner, string gitHubName, string version)
    {
        var gitHubTags = await GitHubTasks.GitHubClient.Repository.GetAllTags(gitHubOwner, gitHubName);
        if (gitHubTags.Select(tag => tag.Name).Contains(version))
            throw new ArgumentException($"A Release with the specified tag already exists in the repository: {version}");
    }



    static async Task UploadArtifactsAsync(Release release, IEnumerable<string> artifacts)
    {
        foreach (var file in artifacts)
        {
            var releaseAssetUpload = new ReleaseAssetUpload
            {
                ContentType = "application/x-binary",
                FileName = Path.GetFileName(file),
                RawData = File.OpenRead(file)
            };

            await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(release, releaseAssetUpload);
            Log.Information("Artifact: {Path}", file);
        }
    }

    string CreateGithubChangelog()
    {
        if (!File.Exists(ChangeLogPath))
        {
            Log.Warning("Unable to locate the changelog file: {Log}", ChangeLogPath);
            return string.Empty;
        }

        Log.Information("Changelog: {Path}", ChangeLogPath);

        var changelog = BuildChangelog();
        if (changelog.Length == 0)
        {
            Log.Warning("No version entry exists in the changelog: {Version}", GithubPublishVersion);
            return string.Empty;
        }

        WriteCompareUrl(changelog);
        return changelog.ToString();
    }

    void WriteCompareUrl(StringBuilder changelog)
    {
        var tags = GitTasks.Git("describe --tags --abbrev=0 --always", logInvocation: false, logOutput: false);
        var latestTag = tags.First().Text;
        if (latestTag == GitRepository.Commit) return;

        if (changelog[^1] != '\r' || changelog[^1] != '\n') changelog.AppendLine(Environment.NewLine);
        changelog.Append("Full changelog: ");
        changelog.Append(GitRepository.GetGitHubCompareTagsUrl(GithubPublishVersion, latestTag));
    }

    StringBuilder BuildChangelog()
    {
        const string separator = "# ";

        var hasEntry = false;
        var changelog = new StringBuilder();
        foreach (var line in File.ReadLines(ChangeLogPath))
        {
            if (hasEntry)
            {
                if (line.StartsWith(separator)) break;

                changelog.AppendLine(line);
                continue;
            }

            if (line.StartsWith(separator) && line.Contains(GithubPublishVersion))
            {
                hasEntry = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    static void TrimEmptyLines(StringBuilder builder)
    {
        if (builder.Length == 0) return;

        while (builder[^1] == '\r' || builder[^1] == '\n')
        {
            builder.Remove(builder.Length - 1, 1);
        }

        while (builder[0] == '\r' || builder[0] == '\n')
        {
            builder.Remove(0, 1);
        }
    }
}