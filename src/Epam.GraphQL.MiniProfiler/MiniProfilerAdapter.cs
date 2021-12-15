// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using StackExchange.Profiling;

namespace Epam.GraphQL.MiniProfiler
{
    internal class MiniProfilerAdapter : IProfiler
    {
        public static IProfiler Instance { get; } = new MiniProfilerAdapter();

        public IDisposable CustomTiming(string section, string message)
        {
            if (StackExchange.Profiling.MiniProfiler.Current != null)
            {
                StackExchange.Profiling.MiniProfiler.Current.Name = section;
            }

            return StackExchange.Profiling.MiniProfiler.Current.CustomTiming(section, message);
        }

        public IDisposable Step(string name)
        {
            return StackExchange.Profiling.MiniProfiler.Current.Step(name);
        }
    }
}
