// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Configuration
{
    public interface IPayloadFieldedMutationField<TArgType, TExecutionContext> :
        IUnionableRootField<TArgType, TExecutionContext>,
        IResolvableMutationFieldMixin<TArgType, TExecutionContext>
    {
        IPayloadFieldedMutationField<TArgType, TArgType2, TExecutionContext> PayloadField<TArgType2>(string argName);

        IPayloadFieldedMutationField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    public interface IPayloadFieldedMutationField<TArgType1, TArgType2, TExecutionContext> :
        IUnionableRootField<TArgType1, TArgType2, TExecutionContext>,
        IResolvableMutationFieldMixin<TArgType1, TArgType2, TExecutionContext>
    {
        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> PayloadField<TArgType3>(string argName);

        IPayloadFieldedMutationField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    public interface IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IResolvableMutationFieldMixin<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> PayloadField<TArgType4>(string argName);

        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    public interface IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IResolvableMutationFieldMixin<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> PayloadField<TArgType5>(string argName);

        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    public interface IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IResolvableMutationFieldMixin<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
    }
}
