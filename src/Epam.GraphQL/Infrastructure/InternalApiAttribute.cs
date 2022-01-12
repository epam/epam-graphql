// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Infrastructure
{
    /// <summary>
    ///     Marks an API as internal to Epam.GraphQL. It may be changed or removed without notice in any release.
    ///     You should only use such APIs directly in your code with extreme caution and understanding that doing
    ///     so can cause an application failures when updating to a new Epam.GraphQL release.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Enum
        | AttributeTargets.Class
        | AttributeTargets.Struct
        | AttributeTargets.Interface
        | AttributeTargets.Event
        | AttributeTargets.Field
        | AttributeTargets.Method
        | AttributeTargets.Delegate
        | AttributeTargets.Property
        | AttributeTargets.Constructor)]
    public sealed class InternalApiAttribute : Attribute
    {
    }
}
