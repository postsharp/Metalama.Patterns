# Requirements

## Phase 1

* Property chains of type `A.B.C....D` or `A?.B?.C....?D` (or a combination of `?.` or `.`) where all types implement `INotifyPropertyChanged<T>` and all nodes are properties (not fields) in the same project.
* Report warning on unsupported constructs.
* `[IgnoreUnobservableExpressions]` suppresses the warning.
* Derived types inheriting from a a base type which is enhanced by `[Observable]`.

## Phase 2

* Support for "legacy" `INotifyPropertyChanged`.

## Next

* Support named events e.g. XAML-style `ColorChanged`.
* Support for functions
* Cross-project
* API for human-generated hints