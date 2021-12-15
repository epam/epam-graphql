// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Projection
{
    public partial interface IQueryFieldBuilder<TExecutionContext> :
        IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TExecutionContext>
    {
        IQueryPayloadFieldBuilder<TType, TExecutionContext> PayloadField<TType>(string name);

        IQueryPayloadFieldBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string name)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        IQueryPayloadFieldBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TEntity>(Type projectionType, string name)
            where TEntity : class;

        IQueryArgumentBuilder<TType, TExecutionContext> Argument<TType>(string name);

        IQueryArgumentBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string name)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        IQueryArgumentBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TEntity>(Type projectionType, string name)
            where TEntity : class;

        IFromIQueryableBuilder<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(Func<TExecutionContext, IQueryable<TReturnType>> query, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> configure = null)
            where TReturnType : class;
    }

    public interface IQueryFieldBuilder<out TThisType, out TArgType, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TThisType, TArgType, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType, TExecutionContext>
    {
    }

    public interface IQueryFieldBuilder<out TThisType, out TArgType1, out TArgType2, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TThisType, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>
    {
    }

    public interface IQueryFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TThisType, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
    }

    public interface IQueryFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TThisType, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
    }

    public interface IQueryFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TThisType, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
    }
}
