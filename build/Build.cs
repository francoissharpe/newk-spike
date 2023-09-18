using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Serilog;
using Services;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;

class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.End);

    [Parameter("Local path to git project")]
    readonly AbsolutePath repoPath = default!;

    // Analyzer Helpers

    private AnalyzerResult _analyzerResult = default!;

    private bool IsDotNetProject => _analyzerResult.TargetTypes.Contains(TargetType.DotNet);
    private bool IsPythonPipProject => _analyzerResult.TargetTypes.Contains(TargetType.PythonPip);
    private bool IsMkDocsProject => _analyzerResult.TargetTypes.Contains(TargetType.MkDocs);

    [LocalPath("pack")]
    readonly Tool Pack;

    [LocalPath("pip")]
    readonly Tool Pip;

    [LocalPath("python")]
    readonly Tool Python;

    [LocalPath("pyenv")]
    readonly Tool Pyenv;


    protected override void OnBuildInitialized()
    {
        base.OnBuildInitialized();

        Log.Information("Analysing project...");
        _analyzerResult ??= Analyzer.Analyze(repoPath);
    }


    Target Start => _ => _
        .Executes(() => _);

    Target End => _ => _
    .Executes(() => _);


    #region PythonPip

    Target RestorePip => _ => _
        .DependsOn(Start)
        .DependentFor(End)
        .OnlyWhenStatic(() => IsPythonPipProject)
        .Executes(() => Log.Information("pip install -r requirements.txt"));

    #endregion PythonPip


    #region MkDocs

    Target BuildMkDocs => _ => _
        .DependsOn(Start)
        .OnlyWhenStatic(() => IsMkDocsProject)
        .Executes(() => Log.Information("mkdocs build"));


    Target PublishMkDocs => _ => _
        .DependsOn(BuildMkDocs)
        .DependentFor(End)
        .OnlyWhenStatic(() => IsMkDocsProject)
        .Executes(() => Log.Information("mkdocs publish"));

    #endregion MkDocs


    #region DotNet
    
    Target BrandDotNet => _ => _
        .DependsOn(Start)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet-gitversion"));

    Target RestoreDotNet => _ => _
        .DependsOn(BrandDotNet)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() =>
            {
                Log.Information("dotnet restore");
                DotNetTasks.DotNetRestore();
            }
        );

    Target BuildDotNet => _ => _
        .DependsOn(RestoreDotNet)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet build --no-restore"));

    Target UnitTestDotNet => _ => _
        .DependsOn(BuildDotNet)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet test"));

    Target PublishDotNet => _ => _
        .DependsOn(UnitTestDotNet)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet publish"));

    Target PackageDotNet => _ => _
        .DependsOn(PublishDotNet)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet package"));

    Target DeliverDotNet => _ => _
        .DependsOn(PackageDotNet)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet deliver"));

    Target DeployDotNet => _ => _
        .DependsOn(DeliverDotNet)
        .DependentFor(End)
        .OnlyWhenStatic(() => IsDotNetProject)
        .Executes(() => Log.Information("dotnet deploy"));


    #endregion DotNet

}
