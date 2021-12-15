// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    public class OptionsBuilder<TOptions>
        where TOptions : Options<TOptions>, new()
    {
        public OptionsBuilder()
            : this(new TOptions())
        {
        }

        public OptionsBuilder(TOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public TOptions Options { get; private set; }

        public void AddOrUpdateExtension<TExtension>(TExtension extension)
            where TExtension : class
        {
            if (extension == null)
            {
                throw new ArgumentNullException(nameof(extension));
            }

            Options = Options.WithExtension(extension);
        }
    }
}
