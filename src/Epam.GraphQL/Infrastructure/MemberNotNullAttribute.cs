// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
    internal sealed class MemberNotNullAttribute : Attribute
    {
#pragma warning disable CA1019 // Define accessors for attribute arguments
        public MemberNotNullAttribute(string member)
#pragma warning restore CA1019 // Define accessors for attribute arguments
        {
            Members = new[] { member };
        }

        public string[] Members { get; }
    }
}
