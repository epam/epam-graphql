// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Reflection;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class QueryField<TExecutionContext> : Field<object, TExecutionContext>, IQueryField<TExecutionContext>
    {
        private static MethodInfo? _fromLoaderImplMethodInfo;

        public QueryField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
        }

        public QueryableField<object, TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? configure)
            where TReturnType : class
        {
            return Parent.FromIQueryableClass(this, query, null, configure);
        }

        public ILoaderField<TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>()
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var graphResultType = Parent.GetGraphQLTypeDescriptor<TChildLoader, TChildEntity>();
            return Parent.ReplaceField(this, new LoaderField<object, TChildLoader, TChildEntity, TExecutionContext>(
                Registry,
                Parent,
                Name,
                condition: null,
                graphResultType,
                arguments: null,
                searcher: null,
                naturalSorters: SortingHelpers.Empty));
        }

        private static MethodInfo FromLoader(Type loaderType, Type entityType)
        {
            _fromLoaderImplMethodInfo ??= typeof(QueryField<TExecutionContext>).GetNonPublicGenericMethod(
                method => method
                    .HasName(nameof(FromLoader))
                    .HasTwoGenericTypeParameters());

            return _fromLoaderImplMethodInfo.MakeGenericMethod(loaderType, entityType);
        }
    }
}
