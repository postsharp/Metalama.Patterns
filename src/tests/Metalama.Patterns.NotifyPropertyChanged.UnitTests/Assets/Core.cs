// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System.ComponentModel;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core
{
    [NotifyPropertyChanged]
    public partial class Simple
    {
        public int S1 { get; set; }

        public int S2 { get; set; }

        public int S3 { get; set; }
    }

    public class ExistingInpcImplWithoutNPCMethod : INotifyPropertyChanged
    {
        private int _ex1;

        public int EX1
        {
            get => this._ex1;
            set
            {
                if ( value != this._ex1 )
                {
                    this._ex1 = value;
                    this.PropertyChanged?.Invoke( this, new( nameof( this.EX1 ) ) );
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }

    public class ExistingInpcImplWithValidOPCMethod : INotifyPropertyChanged
    {
        private int _ex1;

        public int EX1
        {
            get => this._ex1;
            set
            {
                if ( value != this._ex1 )
                {
                    this._ex1 = value;
                    this.PropertyChanged?.Invoke( this, new( nameof( this.EX1 ) ) );
                }
            }
        }

        private Simple _ex2 = new();

        public Simple EX2
        {
            get => this._ex2;

            set
            {
                if ( this._ex2 != value )
                {
                    this._ex2 = value;
                    this.OnPropertyChanged( nameof( this.EX2 ) );
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged( string propertyName ) 
        {
            this.PropertyChanged?.Invoke( this, new( propertyName ) );
        }
    }

    public class ExistingInpcImplWithValidOPCMethodNamedNotifyOfPropertyChange : INotifyPropertyChanged
    {
        private int _ex1;

        public int EX1
        {
            get => this._ex1;
            set
            {
                if ( value != this._ex1 )
                {
                    this._ex1 = value;
                    this.PropertyChanged?.Invoke( this, new( nameof( this.EX1 ) ) );
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void NotifyOfPropertyChange( string propertyName )
        {
            this.PropertyChanged?.Invoke( this, new( propertyName ) );
        }
    }

    public class ExistingInpcImplWithValidOPCMethodNamedRaisePropertyChanged : INotifyPropertyChanged
    {
        private int _ex1;

        public int EX1
        {
            get => this._ex1;
            set
            {
                if ( value != this._ex1 )
                {
                    this._ex1 = value;
                    this.PropertyChanged?.Invoke( this, new( nameof( this.EX1 ) ) );
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void RaisePropertyChanged( string propertyName )
        {
            this.PropertyChanged?.Invoke( this, new( propertyName ) );
        }
    }

    public class ExistingInpcImplWithOPCMethodThatIsPrivate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged( string propertyName ) { }
    }

    public class ExistingInpcImplWithOPCMethodThatIsNotVirtual : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged( string propertyName ) { }
    }

    public class ExistingInpcImplWithOPCMethodWithWrongParamType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged( int propertyIdx ) { }
    }

    public class ExistingInpcImplWithOPCMethodWithWrongParamCount : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged( string propertyName, int propertyIdx ) { }
    }


}