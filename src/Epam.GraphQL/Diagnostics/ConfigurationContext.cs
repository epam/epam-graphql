// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Diagnostics
{
    internal class ConfigurationContext<TProjection, TEntity, TExecutionContext> : ConfigurationContextBase
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        public override string GetError(string message)
        {
            return $"Error during {typeof(TProjection).HumanizedName()}.OnConfigure() call.\r\n{message}";
        }
    }
}
