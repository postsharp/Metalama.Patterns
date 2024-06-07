// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Immutability.Configuration;

[CompileTime]
public static class ImmutabilityConfigurationExtensions
{
    public static void ConfigureImmutability( this IAspectReceiver<INamespace> receiver, ImmutabilityKind immutabilityKind )
        => receiver.SetOptions( new ImmutabilityOptions() { Kind = immutabilityKind } );

    public static void ConfigureImmutability( this IAspectReceiver<INamespace> receiver, IImmutabilityClassifier classifier )
        => receiver.SetOptions( new ImmutabilityOptions() { Classifier = classifier } );

    public static void ConfigureImmutability( this IAspectReceiver<INamedType> receiver, ImmutabilityKind immutabilityKind )
        => receiver.SetOptions( new ImmutabilityOptions() { Kind = immutabilityKind } );

    public static void ConfigureImmutability( this IAspectReceiver<INamedType> receiver, IImmutabilityClassifier classifier )
        => receiver.SetOptions( new ImmutabilityOptions() { Classifier = classifier } );
}

public interface IImmutabilityClassifier : ICompileTimeSerializable
{
    ImmutabilityKind GetImmutabilityKind( INamedType type );
}

internal class ImmutableCollectionClassifier : IImmutabilityClassifier
{
    public ImmutabilityKind GetImmutabilityKind( INamedType type )
    {
        foreach ( var typeArgument in type.TypeArguments )
        {
            if ( typeArgument.GetImmutabilityKind() != ImmutabilityKind.Deep )
            {
                return ImmutabilityKind.Shallow;
            }
        }

        return ImmutabilityKind.Deep;
    }
}