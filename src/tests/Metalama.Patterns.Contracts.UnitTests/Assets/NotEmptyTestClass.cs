// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.Collections.Immutable;

namespace Metalama.Patterns.Contracts.UnitTests.Assets;

#pragma warning disable CS8603

public class NotEmptyTestClass
{
    [NotEmpty]
    public string StringField;

    [NotEmpty]
    public List<int> ListField;

    [NotEmpty]
    public ICollection<int> GenericCollectionProperty { get; set; }

    public string StringMethod( [NotEmpty] string parameter ) => parameter;

    public List<int> ListMethod( [NotEmpty] List<int> parameter ) => parameter;

    public void StringMethodWithRef( string newVal, [NotEmpty] ref string parameter ) => parameter = newVal;

    public void StringMethodWithOut( string newVal, [NotEmpty] out string parameter ) => parameter = newVal;

    [return: NotEmpty]
    public string StringMethodWithRetVal( string retVal ) => retVal;

    public void IReadOnlyCollectionMethod<T>( [NotEmpty] IReadOnlyCollection<T> parameter ) { }

    public void Array( [NotEmpty] int[]? array ) { }

    public void ImmutableArray( [NotEmpty] ImmutableArray<int> array ) { }
}