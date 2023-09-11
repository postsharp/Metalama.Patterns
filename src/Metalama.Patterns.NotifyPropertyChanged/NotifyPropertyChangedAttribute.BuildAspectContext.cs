using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.NotifyPropertyChanged.Implementation;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged;

public sealed partial class NotifyPropertyChangedAttribute
{
    [CompileTime]
    private sealed class BuildAspectContext
    {
        private readonly Dictionary<IType, InpcInstrumentationKind> _inpcInstrumentationKindLookup = new();

        public BuildAspectContext( IAspectBuilder<INamedType> builder )
        {
            this.Builder = builder;
            this.Type_INotifyPropertyChanged = (INamedType) TypeFactory.GetType( typeof( INotifyPropertyChanged ) );
            this.Event_INotifyPropertyChanged_PropertyChanged = this.Type_INotifyPropertyChanged.Events.First();
            this.Type_PropertyChangedEventHandler = (INamedType) TypeFactory.GetType( typeof( PropertyChangedEventHandler ) );
            this.Type_Nullable_PropertyChangedEventHandler = this.Type_PropertyChangedEventHandler.ToNullableType();
            this.Type_IgnoreAutoChangeNotificationAttribute = (INamedType) TypeFactory.GetType( typeof( IgnoreAutoChangeNotificationAttribute ) );
            this.Type_OnChangedAttribute= (INamedType) TypeFactory.GetType( typeof( OnChangedAttribute ) );
            this.Type_OnChildChangedAttribute = (INamedType) TypeFactory.GetType( typeof( OnChildChangedAttribute ) );
            this.Type_EqualityComparerOfT = (INamedType) TypeFactory.GetType( typeof( EqualityComparer<> ) );

            var target = builder.Target;

            this.BaseImplementsInpc =
                target.BaseType != null && (
                    target.BaseType.Is( this.Type_INotifyPropertyChanged )
                    || (target.BaseType is { BelongsToCurrentProject: true } && target.BaseType.Enhancements().HasAspect( typeof( NotifyPropertyChangedAttribute ) )));

            this.TargetImplementsInpc = this.BaseImplementsInpc || target.Is( this.Type_INotifyPropertyChanged );
        }

        public IAspectBuilder<INamedType> Builder { get; }

        public INamedType Target => this.Builder.Target;

        public INamedType Type_INotifyPropertyChanged { get; }

        public IEvent Event_INotifyPropertyChanged_PropertyChanged { get; }

        public INamedType Type_PropertyChangedEventHandler { get; }

        public INamedType Type_Nullable_PropertyChangedEventHandler { get; }

        public INamedType Type_IgnoreAutoChangeNotificationAttribute { get; }

        public INamedType Type_OnChangedAttribute { get; }

        public INamedType Type_OnChildChangedAttribute { get; }

        public INamedType Type_EqualityComparerOfT { get; }

        public bool TargetImplementsInpc { get; }

        public bool BaseImplementsInpc { get; }

        public IMethod OnPropertyChangedMethod { get; set; } = null!;

        public IProperty GetDefaultEqualityComparerForType( IType type )
            => this.Type_EqualityComparerOfT.WithTypeArguments( type ).Properties.Single( p => p.Name == "Default" );

        internal interface IBaseChangeMethods
        {
            /// <summary>
            /// Gets a method like <c>void OnA1B1Changed()</c>.
            /// </summary>
            IMethod OnChangedMethod { get; }

            /// <summary>
            /// Gets for property types that implement <see cref="INotifyPropertyChanged"/>, a method like <c>void OnA1B1ChildChanged( string propertyName )</c>;
            /// otherwise, <see langword="null"/>.
            /// </summary>
            IMethod? OnChildChangedMethod { get;  }
        }

        private class BaseChangeMethods : IBaseChangeMethods
        {
            public IMethod OnChangedMethod { get; set; } = null!;

            public IMethod? OnChildChangedMethod { get; set; }
        }

        private DependencyGraph.Node<NodeData>? _dependencyGraph;

        private DependencyGraph.Node<NodeData> PrepareDependencyGraph()
        {
            var graph = Implementation.DependencyGraph.GetDependencyGraph<NodeData>( this.Target );
            foreach ( var node in graph.DecendantsDepthFirst() )
            {
                node.Data.Initialize( this, node );
            }
            return graph;
        }

        public DependencyGraph.Node<NodeData> DependencyGraph => this._dependencyGraph ??= this.PrepareDependencyGraph();

        private HashSet<string>? _existingMemberNames;

        /// <summary>
        /// Gets an unused member name for the target type by adding an numeric suffix until an unused name is found.
        /// </summary>
        /// <param name="desiredName"></param>
        /// <returns></returns>
        public string GetAndReserveUnusedMemberName( string desiredName )
        {
            this._existingMemberNames ??= new( ((IEnumerable<INamedDeclaration>) this.Target.AllMembers()).Concat( this.Target.NestedTypes ).Select( m => m.Name ) );

            if ( this._existingMemberNames.Add( desiredName ) )
            {
                return desiredName;
            }
            else
            {
                for ( var i = 2; true; i++ )
                {
                    var adjustedName = $"{desiredName}{i}";
                    
                    if ( this._existingMemberNames.Add( adjustedName ) )
                    {
                        return adjustedName;
                    }
                }
            }
        }

        public InpcInstrumentationKind GetInpcInstrumentationKind( IType type )
        {
            if ( this._inpcInstrumentationKindLookup.TryGetValue( type, out var result ) )
            {
                return result;
            }
            else
            {
                result = Check( type );
                this._inpcInstrumentationKindLookup.Add( type, result );
                return result;
            }

            InpcInstrumentationKind Check( IType type )
            {
                switch ( type )
                {
                    case INamedType namedType:
                        if ( namedType.SpecialType != SpecialType.None )
                        {
                            // None of the special types implement INPC.
                            return InpcInstrumentationKind.None;
                        }
                        else if ( namedType.Equals( this.Type_INotifyPropertyChanged ) )
                        {
                            return InpcInstrumentationKind.Implicit;
                        }
                        else if ( namedType.Is( this.Type_INotifyPropertyChanged ) )
                        {
                            if ( namedType.TryFindImplementationForInterfaceMember( this.Event_INotifyPropertyChanged_PropertyChanged, out var result ) )
                            {
                                return result.IsExplicitInterfaceImplementation ? InpcInstrumentationKind.Explicit : InpcInstrumentationKind.Implicit;
                            }

                            throw new InvalidOperationException( "Could not find implementation of interface member." );
                        }
                        else if ( !namedType.BelongsToCurrentProject )
                        {
                            return InpcInstrumentationKind.None;
                        }
                        else 
                        {
                            if ( this.Target.Compilation.IsPartial && !this.Target.Compilation.Types.Contains( type ) )
                            {
                                return InpcInstrumentationKind.Unknown;
                            }

                            return namedType.Enhancements().HasAspect<NotifyPropertyChangedAttribute>()
                                ? InpcInstrumentationKind.Implicit // For now, the aspect always introduces implicit implementation.
                                : InpcInstrumentationKind.None;
                        }

                    case ITypeParameter typeParameter:
                        var hasImplicit = false;

                        foreach ( var t in typeParameter.TypeConstraints )
                        {
                            var k = this.GetInpcInstrumentationKind( t );
                            switch ( k )
                            {
                                case InpcInstrumentationKind.Implicit:
                                    return InpcInstrumentationKind.Implicit;

                                case InpcInstrumentationKind.Explicit:
                                    hasImplicit = true;
                                    break;
                            }
                        }

                        return hasImplicit ? InpcInstrumentationKind.Implicit : InpcInstrumentationKind.None;

                    default:
                        return InpcInstrumentationKind.None;
                }
            }
        }
    }
}