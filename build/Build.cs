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

    // Static

    private Analyzer _analyzer = default!;

    private Analyzer Analyzer()
    {
        if (_analyzer is null)
        {
            _analyzer = new Analyzer(repoPath);
        }
        return _analyzer;
    }

    // Dynamic Steps

    Target Begin => _ => _
        .Executes(() => _);

    Target RestorePip => _ => _
        .DependsOn(Begin)
        .OnlyWhenStatic(() => Analyzer().TargetTypes.Contains(TargetType.PythonPip))
        .Executes(() => Console.WriteLine("pip install -r requirements.txt"));

    Target BuildMkDocs => _ => _
        .DependsOn(Begin)
        .OnlyWhenStatic(() => Analyzer().TargetTypes.Contains(TargetType.MkDocs))
        .Executes(() => Console.WriteLine("mkdocs build"));

    Target PublishMkDocs => _ => _
        .DependsOn(BuildMkDocs)
        .OnlyWhenStatic(() => Analyzer().TargetTypes.Contains(TargetType.MkDocs))
        .Executes(() => Console.WriteLine("mkdocs publish"));

    Target End => _ => _
        .DependsOn(PublishMkDocs,RestorePip)
        .Executes(() => _);

}
