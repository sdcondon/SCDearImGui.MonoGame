using Microsoft.Xna.Framework.Content.Pipeline;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SCDearImGui.MonoGame.Demos.Content;

internal class TraceBuildLogger : ContentBuildLogger
{
    private readonly Stack<string> _relativePaths = new();
    private readonly Stopwatch _stopWatch;
    private int _indentCount;

    public TraceBuildLogger()
    {
        _stopWatch = new Stopwatch();
        _stopWatch.Start();
    }

    public override void Log(LogLevel level, string message)
    {
        if (level >= base.LoggerLogLevel)
        {
            string value = (_relativePaths.Count > 0) ? (string.Join(" > ", _relativePaths.Reverse()) + ": ") : "";
            string value2 = string.Empty.PadLeft(Math.Max(0, _indentCount * 2), ' ');
            string value3 = (base.LoggerLogLevel <= LogLevel.Debug) ? $"{_stopWatch.Elapsed:hh\\:mm\\:ss\\.fff} " : "";
            string[] array = message.Split(['\r', '\n'], StringSplitOptions.None);
            foreach (string value4 in array)
            {
                Trace.WriteLine($"{value3}[{level.ToString()[0]}] {value}{value2}{value4}");
            }
        }
    }

    public override void PushFile(string filename)
    {
        string fullPath = Path.GetFullPath(filename);
        _relativePaths.Push(Path.GetRelativePath(base.LoggerRootDirectory, fullPath));
    }

    public override void PopFile()
    {
        _relativePaths.Pop();
    }

    public override void Indent()
    {
        _indentCount++;
    }

    public override void Unindent()
    {
        _indentCount = Math.Max(0, _indentCount - 1);
    }
}
