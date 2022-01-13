// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal interface IArgumentedField<TEntity, TExecutionContext> : IUnionableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        IArgumentedField<TEntity, TArgType, TExecutionContext> Argument<TArgType>(string argName);

        IArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;

        IArgumentedField<TEntity, TArgType, TExecutionContext> PayloadField<TArgType>(string argName);

        IArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedField<TEntity, TArgType, TExecutionContext> : IUnionableField<TEntity, TArgType, TExecutionContext>
        where TEntity : class
    {
        IArgumentedField<TEntity, TArgType, TArgType2, TExecutionContext> ApplyArgument<TArgType2>(string argName);

        IArgumentedField<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext> : IUnionableField<TEntity, TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> ApplyArgument<TArgType3>(string argName);

        IArgumentedField<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> : IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> ApplyArgument<TArgType4>(string argName);

        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> : IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> ApplyArgument<TArgType5>(string argName);

        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> : IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
    }
}
