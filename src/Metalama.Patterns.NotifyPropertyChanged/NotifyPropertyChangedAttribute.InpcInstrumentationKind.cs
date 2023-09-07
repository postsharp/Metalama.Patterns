using Metalama.Framework.Aspects;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
    [CompileTime]
    private enum InpcInstrumentationKind
    {
        None,
        Implicit,
        Explicit,
        /// <summary>
        /// Returned at design time for types other than the current type and its ancestors.
        /// </summary>
        Unknown
    }
}