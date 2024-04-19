﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable SA1649, SA1402

// ReSharper disable  CheckNamespace

using Metalama.Framework.Fabrics;

namespace Metalama.Patterns.Contracts.AspectTests.Fabric_Project
{
    internal class Fabric : ProjectFabric
    {
        public override void AmendProject( IProjectAmender amender )
        {
            amender.VerifyNotNullableDeclarations();
        }
    }

    public class PublicClass
    {
        public string PublicProperty { get; set; }

        internal string InternalProperty { get; set; }

        public string? PublicNullableProperty { get; set; }

        public string PublicField;
        internal string InternalField;

        public string PublicNullableField;

        public void PublicMethod( string nonNullableParam, string? nullableParam ) { }

        internal void InternalMethod( string nonNullableParam, string? nullableParam ) { }
    }
}