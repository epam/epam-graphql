// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Projection
{
    public interface IMutationFieldBuilder<TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TExecutionContext>, TExecutionContext>,
        IMutationFieldBuilderBase<TExecutionContext>
    {
        IMutationPayloadFieldBuilder<TType, TExecutionContext> PayloadField<TType>(string name);

        IMutationPayloadFieldBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string name)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        IMutationPayloadFieldBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TEntity>(Type projectionType, string name)
            where TEntity : class;

        IMutationArgumentBuilder<TType, TExecutionContext> Argument<TType>(string name);

        IMutationArgumentBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string name)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        IMutationArgumentBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TEntity>(Type projectionType, string name)
            where TEntity : class;
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TThisType, TArgType, TExecutionContext>, TExecutionContext>,
        IMutationFieldBuilderBase<TArgType, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TThisType, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>,
        IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TThisType, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>,
        IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TThisType, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>,
        IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TThisType, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>,
        IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
    }
}
