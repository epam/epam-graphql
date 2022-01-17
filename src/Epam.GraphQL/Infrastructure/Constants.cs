// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Microsoft.Extensions.Logging;

namespace Epam.GraphQL.Infrastructure
{
    internal static class Constants
    {
        public static class Logging
        {
            public const string Category = "Epam.GraphQL";

            public static class BeforeQuery
            {
                public const LogLevel Level = LogLevel.Trace;
                public const int EventId = 1;
            }
        }
    }
}
