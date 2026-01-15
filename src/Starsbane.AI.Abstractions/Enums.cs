using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starsbane.AI
{
    public enum Platform
    {
        Others,
        Aliyun,
        Azure,
        AWS,
        GoogleCloud
    }

    /// <summary>
    /// Controls how the breakpoint threshold is calculated.
    /// </summary>
    public enum BreakpointThresholdType
    {
        Percentile,
        StandardDeviation,
        InterQuartile,
        Gradient
    }
}
