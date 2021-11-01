using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Ao.Auto.Ui
{
    public record KeyProcesses(IntPtr Key, List<Process> Processes);
}
