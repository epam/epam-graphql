// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Diagnostics
{
    internal interface IRootConfigurationContext : IObjectConfigurationContext
    {
        void AddError(string message, params IConfigurationContext[] invalidItems);

        string GetError(string message, params IConfigurationContext[] invalidItems);

        string GetRuntimeError(string message, params IConfigurationContext[] invalidItems);

        void ThrowErrors();
    }
}
