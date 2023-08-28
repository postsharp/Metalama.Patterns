# Requirements

## Phase 1

* Property chains of type `A.B.C....D` or `A?.B?.C....?D` (or a combination of `?.` or `.`) where all types implement `INotifyPropertyChanged<T>` and all nodes are properties (not fields).
* Report warning on unsupported constructs.
* `[SafeForDependencyAnalysis]` suppresses the warning.

## Phase 2

* Support for "legacy" `INotifyPropertyChanged`.

## Next

* Support named events e.g. XAML-style `ColorChanged`.*
