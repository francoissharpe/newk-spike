using Nuke.Common.IO;
using System.Collections.Generic;

namespace Services;

public class AnalyzerResult
{
    public IReadOnlyList<TargetType> TargetTypes => _targetTypes;

    private List<TargetType> _targetTypes { get; } = new();

    public AbsolutePath SourcePath { get; private set; }

    public AnalyzerResult(AbsolutePath sourcePath)
    {
        SourcePath = sourcePath;
        Analyze();
    }

    private void Analyze()
    {
        AnalyzeTargetTypes();

    }

    private void AnalyzeTargetTypes()
    {
        if (IsPythonPipProject())
        {
            _targetTypes.Add(TargetType.PythonPip);
        }

        if (IsMkDocsProject())
        {
            _targetTypes.Add(TargetType.MkDocs);
        }

        if (IsNodeJsNpmProject())
        {
            _targetTypes.Add(TargetType.NodeJsNpm);
        }

        if (IsNodeJsYarnProject())
        {
            _targetTypes.Add(TargetType.NodeJsYarn);
        }

        if (IsBuildpacksProject())
        {
            _targetTypes.Add(TargetType.Buildpacks);
        }

        if (IsPythonPoetryProject())
        {
            _targetTypes.Add(TargetType.PythonPoetry);
        }

        if (IsDotNetProject())
        {
            _targetTypes.Add(TargetType.DotNet);
        }
    }

    private bool IsBuildpacksProject()
    {
        return SourcePath.ContainsFile("Procfile");
    }
    private bool IsDotNetProject() => SourcePath.ContainsFile("*.sln") || SourcePath.ContainsFile("*.csproj");
    private bool IsPythonPoetryProject() => SourcePath.ContainsFile("poetry.lock") || SourcePath.ContainsFile("pyproject.toml");
    private bool IsNodeJsYarnProject() => SourcePath.ContainsFile("yarn.lock");
    private bool IsNodeJsNpmProject() => SourcePath.ContainsFile("package-lock.json");
    private bool IsMkDocsProject()
    {
        return (
            (SourcePath.ContainsFile("mkdocs.yml") || SourcePath.ContainsFile("mkdocs.yaml"))
            && SourcePath.ContainsDirectory("docs")
        );
    }

    private bool IsPythonPipProject()
    {
        return SourcePath.ContainsFile("requirements.txt");
    }

}
