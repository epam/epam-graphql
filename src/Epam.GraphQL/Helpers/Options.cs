// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epam.GraphQL.Helpers
{
    public class Options<TOptions>
        where TOptions : Options<TOptions>, new()
    {
        private IReadOnlyDictionary<Type, object> _extensions;

        public Options()
            : this(new Dictionary<Type, object>())
        {
        }

        public Options(
            IReadOnlyDictionary<Type, object> extensions)
        {
            _extensions = extensions ?? throw new ArgumentNullException(nameof(extensions));
        }

        /// <summary>
        ///     Gets the extensions that store the configured options.
        /// </summary>
        /// <value>
        ///     The extensions that store the configured options.
        /// </value>
        public virtual IEnumerable<object> Extensions => _extensions.Values;

        /// <summary>
        ///     Gets the extension of the specified type. Returns null if no extension of the specified type is configured.
        /// </summary>
        /// <typeparam name="TExtension"> The type of the extension to get. </typeparam>
        /// <returns> The extension, or null if none was found. </returns>
        public virtual TExtension? FindExtension<TExtension>()
            where TExtension : class
            => _extensions.TryGetValue(typeof(TExtension), out var extension) ? (TExtension)extension : null;

        public TOptions WithExtension<TExtension>(TExtension extension)
            where TExtension : class
        {
            var extensions = Extensions.ToDictionary(p => p.GetType(), p => p);
            extensions[typeof(TExtension)] = extension ?? throw new ArgumentNullException(nameof(extension));

            var options = new TOptions
            {
                _extensions = extensions,
            };

            return options;
        }
    }
}
