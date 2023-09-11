// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

#pragma warning disable

using Metalama.Patterns.NotifyPropertyChanged;
using Metalama.Patterns.NotifyPropertyChanged.Metadata;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NpcExperiments.Exp7
{
    public sealed class RefsAttribute : Attribute
    {
        public RefsAttribute( params string[] refs ) { }
    }

    [NotifyPropertyChanged]
    class A
    {
        public int A1 { get; set; }
    }

    class B : A
    {
        int B1 => A1;
    }

    namespace DesiredOpt1
    {
        /* In this model, all properties have OnXXChanged, whether they are writable properties/fields
         * or calculated properties. In non-sealed classes, all OnXXChanged methods must be created
         * because a derived class might be interested.
         * 
         * Pros:
         * - If a base class is in a different assy, updates (eg via package update) will not require derived
         *   types to rebuild even if refs change, because there is no CT analysis of refs.
         *   
         * - Arguably closer to idomatic hand-written code.
         *   
         * Cons:
         * - More OnXXChanged methods are required.
         */
        class A : INotifyPropertyChanged
        {
            private int _a1;
            public int A1
            {
                get
                {
                    return this._a1;
                }
                set
                {
                    if ( this._a1 != value )
                    {
                        this._a1 = value;
                        this.OnA1Changed();
                    }
                }
            }

            [OnChanged( "A1" )]
            protected virtual void OnA1Changed()
            {
                this.OnPropertyChanged( "A1" );
            }

            protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
            {
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        class B : A
        {
            public int B1 => A1;

            protected override void OnA1Changed()
            {
                this.OnB1Changed();
                base.OnA1Changed();
            }

            [OnChanged( "B1" )]
            protected virtual void OnB1Changed()
            {
                this.OnPropertyChanged( "B1" );
            }
        }

        class C : B
        {
            int C1 => B1;

            int C2 => A1;

            protected override void OnA1Changed()
            {
                this.OnC2Changed();
                base.OnA1Changed();
            }

            protected override void OnB1Changed()
            {
                this.OnC1Changed();
                base.OnB1Changed();
            }

            [OnChanged( "C1" )]
            protected virtual void OnC1Changed()
            {
                this.OnPropertyChanged( "C1" );
            }

            [OnChanged( "C2" )]
            protected virtual void OnC2Changed()
            {
                this.OnPropertyChanged( "C2" );
            }
        }
    }

    namespace DesiredOpt2
    {
        /* In this model, OnXXChanged methods are only introduced for writeable properties/fields.
         * Caclulated fields express their dependencies through metadata which is analysed at CT to
         * determine which OnXXChanged methods must be overriden.
         * 
         * Pros:
         * 
         * Fewer OnXXChanged methods.
         * 
         * Cons:
         * 
         * If the base is in a different assy, [Refs] might change via a package update, which would
         * require a recompilation of derived types.
         * 
         * Arguably less like idiomatic hand-written code, as the developer would need to examine
         * metadata on base types.
         */

        class A : INotifyPropertyChanged
        {
            private int _a1;
            public int A1
            {
                get
                {
                    return this._a1;
                }
                set
                {
                    if ( this._a1 != value )
                    {
                        this._a1 = value;
                        this.OnA1Changed();
                    }
                }
            }

            [OnChanged( "A1" )]
            protected virtual void OnA1Changed()
            {
                this.OnPropertyChanged( "A1" );
            }

            protected virtual void OnPropertyChanged( [CallerMemberName] string? propertyName = null )
            {
                this.PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
            }

            public event PropertyChangedEventHandler? PropertyChanged;
        }

        class B : A
        {
            [Refs( "A1" )]
            public int B1 => A1;

            protected override void OnA1Changed()
            {
                this.OnPropertyChanged( "B1" );
                base.OnA1Changed();
            }
        }

        class C : B
        {
            [Refs( "A1" )]
            public int C1 => A1;

            // To decide which methods to override to allow notification of changes to C2,
            // we must analyse [Refs] on B1 because there is no OnB1Changed().
            [Refs( "B1" )]
            public int C2 => B1;

            // C2 depends on B1, which depends on A1.
            // C1 depends on A1.
            protected override void OnA1Changed()
            {
                this.OnPropertyChanged( "C1" );
                this.OnPropertyChanged( "C2" );
                base.OnA1Changed();
            }
        }
    }
}
