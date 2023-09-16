using Nuke.Common.IO;
using System;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services;

public static class Analyzer
{


    public static AnalyzerResult Analyze(AbsolutePath sourcePath)
    {
        return new AnalyzerResult(sourcePath);
    }

}
