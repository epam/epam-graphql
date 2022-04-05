// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Text;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics
{
    internal class ObjectConfigurationContext : ObjectConfigurationContextBase
    {
        public ObjectConfigurationContext(ConfigurationContext? parent)
            : base(parent)
        {
        }

        public override void Append(StringBuilder stringBuilder, IEnumerable<IConfigurationContext> choosenItems, int indent)
        {
        }
    }

    internal class ObjectConfigurationContext<TProjection> : ObjectConfigurationContextBase
    {
        public ObjectConfigurationContext()
            : base(null)
        {
        }

        public override void Append(StringBuilder stringBuilder, IEnumerable<IConfigurationContext> choosenItems, int indent)
        {
            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.AppendLine("public override void OnConfigure()");

            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.AppendLine("{");

            AppendBody(stringBuilder, choosenItems, indent + 1);

            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.Append('}');
        }

        protected override string DoGetError(string message)
        {
            return $"Error during {typeof(TProjection).HumanizedName()}.OnConfigure() call.\r\n{message}";
        }
    }
}
