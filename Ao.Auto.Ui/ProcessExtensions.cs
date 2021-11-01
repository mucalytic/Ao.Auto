using static LanguageExt.Prelude;
using System.Diagnostics;
using System;

namespace Ao.Auto.Ui
{
    public static class ProcessExtensions
    {
        public static string Info(this Process process) =>
            $"{DateTime.Now:T}:{process.Id}:{process.CharacterName()}";

        public static string CharacterName(this Process process) =>
            map(process.MainWindowTitle, title => title.Length <= 17 ? title : title.Remove(0, 17));
    }
}
