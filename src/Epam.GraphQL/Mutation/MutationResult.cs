// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;

namespace Epam.GraphQL.Mutation
{
    public static class MutationResult
    {
        public static MutationResult<TData> Create<TData>(IEnumerable<object> payload, TData data)
        {
            return new MutationResult<TData>
            {
                Payload = payload,
                Data = data,
            };
        }
    }

    public class MutationResult<TData> : IMutationResult
    {
        public IEnumerable<object> Payload { get; set; }

        public TData Data { get; set; }

        object IMutationResult.Data { get => Data; set => Data = (TData)value; }
    }
}
