using Nuke.Common.IO;
using Services;

namespace Newk.Tests;

public class AnalyzerTests
{
    [Fact]
    public void Analyzer_CorrectlyAnalyzes_PythonPipFolder()
    {
        var sourcePath = Path.GetFullPath("./_files/python-pip/");
        var analyzer = Analyzer.Analyze(sourcePath);
        Assert.Contains(TargetType.PythonPip, analyzer.TargetTypes);
        Assert.Equal(1, analyzer.TargetTypes.Count);
    }

    [Fact]
    public void Analyzer_CorrectlyAnalyzes_PythonPipMkDocs()
    {
        var sourcePath = Path.GetFullPath("./_files/python-pip-mkdocs/");
        var analyzer = Analyzer.Analyze(sourcePath);
        Assert.Contains(TargetType.PythonPip, analyzer.TargetTypes);
        Assert.Contains(TargetType.MkDocs, analyzer.TargetTypes);
        Assert.Equal(2, analyzer.TargetTypes.Count);
    }

    [Fact]
    public void Analyzer_CorrectlyAnalyzes_PythonPipProcfile()
    {
        var sourcePath = Path.GetFullPath("./_files/python-pip-procfile/");
        var analyzer = Analyzer.Analyze(sourcePath);
        Assert.Contains(TargetType.PythonPip, analyzer.TargetTypes);
        Assert.Contains(TargetType.Buildpacks, analyzer.TargetTypes);
        Assert.Equal(2, analyzer.TargetTypes.Count);
    }
}