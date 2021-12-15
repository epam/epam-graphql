// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal static class GraphTypeDescriptorExtensions
    {
        public static IGraphTypeDescriptor<TExecutionContext> MakeListDescriptor<TExecutionContext>(this IGraphTypeDescriptor<TExecutionContext> graphType)
        {
            return new ListGraphTypeDescriptor<TExecutionContext>(graphType);
        }

        public static IGraphTypeDescriptor<TExecutionContext> UnwrapIfNonNullable<TExecutionContext>(this IGraphTypeDescriptor<TExecutionContext> graphTypeDescriptor)
        {
            var type = graphTypeDescriptor.Type;
            var graphType = graphTypeDescriptor.GraphType;

            if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(NonNullGraphType<>))
            {
                type = type.GenericTypeArguments[0];
            }

            if (graphType is NonNullGraphType nonNullGraphType)
            {
                graphType = nonNullGraphType.ResolvedType;
                if (nonNullGraphType.Type != null)
                {
                    type = nonNullGraphType.Type;
                }
            }

            return new GraphTypeDescriptor<TExecutionContext>(type, graphType, graphTypeDescriptor.Configurator);
        }
    }
}
