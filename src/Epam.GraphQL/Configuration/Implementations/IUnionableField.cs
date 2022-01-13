// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IUnionableField<TEntity, TExecutionContext> :
        IResolvableField<TEntity, TExecutionContext>,
        IUnionableFieldBase<IUnionableField<TEntity, TExecutionContext>, TEntity, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IUnionableField<TEntity, TArgType, TExecutionContext> :
        IResolvableField<TEntity, TArgType, TExecutionContext>,
        IUnionableFieldBase<IUnionableField<TEntity, TArgType, TExecutionContext>, TEntity, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IUnionableField<TEntity, TArgType1, TArgType2, TExecutionContext> :
        IResolvableField<TEntity, TArgType1, TArgType2, TExecutionContext>,
        IUnionableFieldBase<IUnionableField<TEntity, TArgType1, TArgType2, TExecutionContext>, TEntity, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> :
        IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IUnionableFieldBase<IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>, TEntity, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IUnionableFieldBase<IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TEntity, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IUnionableFieldBase<IUnionableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TEntity, TExecutionContext>
        where TEntity : class
    {
    }
}
