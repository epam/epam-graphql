// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;

#nullable enable

namespace Epam.GraphQL.Mutation
{
    public interface IMutationResult
    {
        IEnumerable<object>? Payload { get; set; }

        object? Data { get; set; }
    }
}
