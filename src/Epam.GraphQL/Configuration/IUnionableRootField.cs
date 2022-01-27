// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Configuration
{
    public interface IUnionableRootField<TExecutionContext> :
        IResolvableRootField<TExecutionContext>,
        IUnionableFieldBase<IUnionableRootField<TExecutionContext>, TExecutionContext>
    {
    }

    public interface IUnionableRootField<TArgType, TExecutionContext> :
        IResolvableRootField<TArgType, TExecutionContext>,
        IUnionableFieldBase<IUnionableRootField<TArgType, TExecutionContext>, TExecutionContext>
    {
    }

    public interface IUnionableRootField<TArgType1, TArgType2, TExecutionContext> :
        IResolvableRootField<TArgType1, TArgType2, TExecutionContext>,
        IUnionableFieldBase<IUnionableRootField<TArgType1, TArgType2, TExecutionContext>, TExecutionContext>
    {
    }

    public interface IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        IResolvableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IUnionableFieldBase<IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>
    {
    }

    public interface IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        IResolvableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IUnionableFieldBase<IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>
    {
    }

    public interface IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        IResolvableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IUnionableFieldBase<IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>
    {
    }
}
