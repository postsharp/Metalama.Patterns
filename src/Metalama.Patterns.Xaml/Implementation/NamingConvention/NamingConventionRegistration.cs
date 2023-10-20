// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Options;
using Metalama.Framework.Serialization;

namespace Metalama.Patterns.Xaml.Implementation.NamingConvention;
#if false
[CompileTime]
internal interface INamingConventionRegistration
{
    INamingConvention NamingConvention { get; }

    int? Priority { get; }
}
#endif

[CompileTime]
public sealed class NamingConventionRegistration<T> : IIncrementalKeyedCollectionItem<T>
    where T : class, INamingConvention, ICompileTimeSerializable, IEquatable<T>
{
    public NamingConventionRegistration( T namingConvention, int? priority = null )
    {
        this.NamingConvention = namingConvention ?? throw new ArgumentNullException( nameof( namingConvention ) );
        this.Priority = priority;
    }

    T IIncrementalKeyedCollectionItem<T>.Key => this.NamingConvention;

    object IIncrementalObject.ApplyChanges( object changes, in ApplyChangesContext context )
        => new NamingConventionRegistration<T>( this.NamingConvention, ((NamingConventionRegistration<T>) changes).Priority ?? this.Priority );    

    public T NamingConvention { get; }

    public int? Priority { get; }
}
