﻿using System.Threading.Tasks;
using GitHubSync;
using Xunit;
using Xunit.Abstractions;

public class RepoSyncTests : TestBase
{
    [Fact]
    public Task SyncPr()
    {
        var credentials = CredentialsHelper.Credentials;
        var repoSync = new RepoSync(credentials, "SimonCropp", "GitHubSync.TestRepository", "source", WriteLog);
        repoSync.AddBlob("sourceFile.txt");
        repoSync.RemoveBlob("IDoNotExist/MeNeither.txt");
        repoSync.RemoveBlob("a/b/c/file.txt");
        repoSync.RemoveBlob("a/b/file.txt");
        repoSync.AddTarget("SimonCropp", "GitHubSync.TestRepository", "target");

        return repoSync.Sync();
    }

    [Fact]
    public Task SyncPrMerge()
    {
        var credentials = CredentialsHelper.Credentials;
        var repoSync = new RepoSync(credentials, "SimonCropp", "GitHubSync.TestRepository", "source", WriteLog);
        repoSync.AddBlob("sourceFile.txt");
        repoSync.RemoveBlob("IDoNotExist/MeNeither.txt");
        repoSync.RemoveBlob("README.md");
        repoSync.AddTarget("SimonCropp", "GitHubSync.TestRepository", "targetForMerge");

        return repoSync.Sync(SyncOutput.MergePullRequest);
    }

    [Fact]
    public Task SyncCommit()
    {
        var repoSync = new RepoSync(CredentialsHelper.Credentials, "SimonCropp", "GitHubSync.TestRepository", "source", WriteLog);
        repoSync.AddBlob("sourceFile.txt");
        repoSync.RemoveBlob("IDoNotExist/MeNeither.txt");
        repoSync.RemoveBlob("README.md");
        repoSync.AddTarget("SimonCropp", "GitHubSync.TestRepository", "targetForCommit");

        return repoSync.Sync(SyncOutput.CreateCommit);
    }

    public RepoSyncTests(ITestOutputHelper output) : base(output)
    {
    }
}