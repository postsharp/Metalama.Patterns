// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Options;

namespace Metalama.Patterns.Caching.Aspects.Configuration;

internal class ParameterFilterRegistration : IIncrementalKeyedCollectionItem<string>
{
    public object ApplyChanges( object changes, in ApplyChangesContext context ) => changes;

    public string Name { get; }

    public ICacheParameterClassifier Classifier { get; }

    public ParameterFilterRegistration( string name, ICacheParameterClassifier classifier )
    {
        this.Name = name;
        this.Classifier = classifier;
    }

    string IIncrementalKeyedCollectionItem<string>.Key => this.Name;
}