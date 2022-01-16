// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Builders.Projection;
using Epam.GraphQL.Builders.RootProjection;

namespace Epam.GraphQL.Builders.Mutation
{
    public interface IMutationFieldBuilderBase<TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilderBase<TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TExecutionContext>
    {
    }

    public interface IMutationFieldBuilderBase<out TArgType, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>,
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
    }
}
