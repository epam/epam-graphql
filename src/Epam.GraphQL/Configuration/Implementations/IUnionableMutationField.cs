// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IUnionableMutationField<TExecutionContext> :
        IResolvableField<object, TExecutionContext>,
        IUnionableFieldBase<IUnionableMutationField<TExecutionContext>, TExecutionContext>
    {
    }

    internal interface IUnionableMutationField<TArgType, TExecutionContext> :
        IResolvableField<object, TArgType, TExecutionContext>,
        IUnionableFieldBase<IUnionableMutationField<TArgType, TExecutionContext>, TExecutionContext>
    {
    }

    internal interface IUnionableMutationField<TArgType1, TArgType2, TExecutionContext> :
        IResolvableField<object, TArgType1, TArgType2, TExecutionContext>,
        IUnionableFieldBase<IUnionableMutationField<TArgType1, TArgType2, TExecutionContext>, TExecutionContext>
    {
    }

    internal interface IUnionableMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        IResolvableField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IUnionableFieldBase<IUnionableMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>
    {
    }

    internal interface IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        IResolvableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IUnionableFieldBase<IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>
    {
    }

    internal interface IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        IResolvableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IUnionableFieldBase<IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>
    {
    }
}
