// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: [Porting] Temporary atrributes - resolve usages and remove.

namespace Metalama.Patterns.Caching
{
    public sealed class PSerializableAttribute : Attribute { }

    public sealed class PNonSerializedAttribute : Attribute { }

    public sealed class ExplicitCrossPackageInternalAttribute : Attribute { }

    public sealed class ImportSerializerAttribute : Attribute
    {
        public ImportSerializerAttribute( Type objectType, Type serializerType ) { }
    }
}