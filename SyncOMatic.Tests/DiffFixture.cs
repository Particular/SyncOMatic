﻿using System;
using System.Linq;
using NUnit.Framework;
using SyncOMatic;

[TestFixture]
public class DiffFixture
{
    Syncer BuildSUT()
    {
        return new Syncer(Helper.Credentials, Helper.Proxy, ConsoleLogger);
    }

    internal static void ConsoleLogger(LogEntry obj)
    {
        Console.WriteLine("{0}\t{1}", obj.At.ToString("o"), obj.What);
    }

    [Test]
    public void NothingToUpdateWhenSourceBlobAndDestinationBlobHaveTheSameSha()
    {
        var blob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "blessed-source", "file.txt");

        var map = new Mapper()
            .Add(blob, blob);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(0, diff.Count());
    }

    [Test]
    public void CanDetectBlobUpdation()
    {
        var sourceBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "blessed-source", "file.txt");
        var destBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "consumer-one", "file.txt");

        var map = new Mapper()
            .Add(sourceBlob, destBlob);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(1, diff.Count());
        Assert.NotNull(diff.Single().Key.Sha);
        Assert.AreEqual(1, diff.Single().Value.Count());
        Assert.NotNull(diff.Single().Value.Single().Sha);
    }

    [Test]
    public void CanDetectBlobCreation()
    {
        var sourceBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "blessed-source", "new-file.txt");
        var destBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "consumer-one", "new-file.txt");

        var map = new Mapper()
            .Add(sourceBlob, destBlob);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(1, diff.Count());
        Assert.NotNull(diff.Single().Key.Sha);
        Assert.AreEqual(1, diff.Single().Value.Count());
        Assert.Null(diff.Single().Value.Single().Sha);
    }

    [Test]
    public void ThrowsWhenSourceBlobDoesNotExist()
    {
        var sourceBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "blessed-source", "IDoNotExist.txt");
        var destBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "consumer-one", "file.txt");

        var map = new Mapper()
            .Add(sourceBlob, destBlob);

        using (var som = BuildSUT())
        {
            Assert.Throws<MissingSourceException>(() => som.Diff(map));
        }
    }

    [Test]
    public void NothingToUpdateWhenSourceTreeAndDestinationTreeHaveTheSameSha()
    {
        var tree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "blessed-source", "folder");

        var map = new Mapper()
            .Add(tree, tree);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(0, diff.Count());
    }

    [Test]
    public void CanDetectTreeUpdation()
    {
        var sourceTree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "blessed-source", "folder");
        var destTree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "consumer-one", "folder");

        var map = new Mapper()
            .Add(sourceTree, destTree);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(1, diff.Count());
        Assert.NotNull(diff.Single().Key.Sha);
        Assert.AreEqual(1, diff.Single().Value.Count());
        Assert.NotNull(diff.Single().Value.Single().Sha);
    }

    [Test]
    public void CanDetectTreeCreation()
    {
        var sourceTree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "blessed-source", "folder/sub2");
        var destTree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "consumer-one", "folder/sub2");

        var map = new Mapper()
            .Add(sourceTree, destTree);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(1, diff.Count());
        Assert.NotNull(diff.Single().Key.Sha);
        Assert.AreEqual(1, diff.Single().Value.Count());
        Assert.Null(diff.Single().Value.Single().Sha);
    }

    [Test]
    public void ThrowsWhenSourceTreeDoesNotExist()
    {
        var sourceTree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "blessed-source", "IDoNotExist/folder/sub2");
        var destTree = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Tree, "consumer-one", "folder/sub2");

        var map = new Mapper()
            .Add(sourceTree, destTree);

        using (var som = BuildSUT())
        {
            Assert.Throws<MissingSourceException>(() => som.Diff(map));
        }
    }

    [Test]
    public void CanDetectBlobCreationWhenTargetTreeFolderDoesNotExist()
    {
        var sourceBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "blessed-source", "new-file.txt");
        var destBlob = new Parts("Particular/SyncOMatic.TestRepository", TreeEntryTargetType.Blob, "consumer-one", "IDoNotExist/MeNeither/new-file.txt");

        var map = new Mapper()
            .Add(sourceBlob, destBlob);

        Diff diff;
        using (var som = BuildSUT())
        {
            diff = som.Diff(map);
        }

        Assert.AreEqual(1, diff.Count());
        Assert.NotNull(diff.Single().Key.Sha);
        Assert.AreEqual(1, diff.Single().Value.Count());
        Assert.Null(diff.Single().Value.Single().Sha);
    }
}