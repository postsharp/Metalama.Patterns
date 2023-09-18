// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Inheritance
{
    #region Basic Cases

    /// <summary>
    /// C1 : Simple
    /// </summary>
    public partial class C1 : Simple
    {
    }
    
    /// <summary>
    /// C2 : Simple
    /// </summary>
    public partial class C2 : Simple
    {
        public int C2P1 { get; set; }
    }

    /// <summary>
    /// C3 : C2 : Simple
    /// </summary>
    public partial class C3 : C2
    {
        public int C3P1 { get; set; }
    }

    /// <summary>
    /// C4 : C3 : C2 : C1 : Simple
    /// </summary>
    public partial class C4 : C3
    {
        public int C4P1 { get; set; }
    }

    /// <summary>
    /// C5: C4 : C3 : C2 : C1 : Simple
    /// </summary>
    public partial class C5 : C4
    {
        public int C5P1 { get; set; }
    }

    /// <summary>
    /// C6 : Simple, has ref to <see cref="Simple.S1"/>.
    /// </summary>
    public partial class C6 : Simple
    {
        public int C6P1 => this.S1;
    }

    /// <summary>
    /// C7 : C6 : Simple, has ref to <see cref="Simple.S1"/>.
    /// </summary>
    public partial class C7 : C6
    {
        public int C7P1 => this.S1;
    }

    #endregion

    #region Child properties (particularly in the context of inheritance)

    public partial class C16 : SimpleWithInpcProperties
    {
        /// <summary>
        /// References child prop S2 of <see cref="SimpleWithInpcProperties.R1"/>, where that
        /// child is not referenced by <see cref="SimpleWithInpcProperties"/>.
        /// </summary>
        public int C16P1 => this.R1!.S2;
    }

    public partial class C17 : SimpleWithInpcProperties
    {
        /// <summary>
        /// References child prop S1 of <see cref="SimpleWithInpcProperties.R1"/>, where that
        /// child is also referenced by <see cref="SimpleWithInpcProperties"/>.
        /// </summary>
        public int C17P1 => this.R1!.S1;
    }

    public partial class C18 : SimpleWithInpcProperties
    {
        /// <summary>
        /// References child prop S1 of <see cref="SimpleWithInpcProperties.R2"/>, where neither
        /// R2, or R2.S1, is referenced by the <see cref="SimpleWithInpcProperties"/>.
        /// </summary>
        public int C18P1 => this.R2!.S1;
    }

    #endregion

    #region Inherit from existing non-aspect base

    /// <summary>
    /// C8 : ExistingInpcImplWithValidOPCMethod
    /// </summary>
    [NotifyPropertyChanged]
    public partial class C8 : ExistingInpcImplWithValidOPCMethod
    {
        /// <summary>
        /// Auto
        /// </summary>
        public int C8P1 { get; set; }

        /// <summary>
        /// Ref to <see cref="ExistingInpcImplWithValidOPCMethod.EX1"/>.
        /// </summary>
        public int C8P2 => this.EX1 * 2;

        /// <summary>
        /// Ref to <see cref="ExistingInpcImplWithValidOPCMethod.EX2"/>/<see cref="Simple.S1"/>. EX2 is not monitored by its declaring class.
        /// </summary>
        public int C8P3 => this.EX2!.S1 * 3;
    }

    /// <summary>
    /// C9 : C8
    /// </summary>
    public partial class C9 : C8
    {
        /// <summary>
        /// Ref to <see cref="C8.C8P1"/>.
        /// </summary>
        public int C9P1 => this.C8P1;

        /// <summary>
        /// Ref to <see cref="ExistingInpcImplWithValidOPCMethod.EX2"/>/<see cref="Simple.S2"/>.
        /// Monitoring of EX2 is provided by C8.
        /// </summary>
        public int C9P2 => this.EX2!.S2;
    }

    #endregion

    #region Abstract base

    /// <summary>
    /// Abstract [NPC] base class.
    /// </summary>
    [NotifyPropertyChanged]
    public abstract partial class C10
    {
        /// <summary>
        /// Auto
        /// </summary>
        public int C10P1 { get; set; }
    }

    /// <summary>
    /// C11 : C10
    /// </summary>
    public partial class C11 : C10
    {
        /// <summary>
        /// Ref to <see cref="C10.C10P1"/>.
        /// </summary>
        public int C11P1 => this.C10P1;
    }

    /// <summary>
    /// I12 : ExistingAbstractInpcImplWithValidOPCMethod
    /// </summary>
    public partial class C12 : ExistingAbstractInpcImplWithValidOPCMethod
    {
        /// <summary>
        /// Ref to <see cref="ExistingAbstractInpcImplWithValidOPCMethod.EX1"/>.
        /// </summary>
        public int C12P1 => this.EX1;
    }

    #endregion

    #region UserImpl => [NPC] => UserImpl => [NPC]

    /// <summary>
    /// C13 : ExistingInpcImplWithValidOPCMethod
    /// </summary>
    public partial class C13 : ExistingInpcImplWithValidOPCMethod
    {
        /// <summary>
        /// Auto
        /// </summary>
        public int C13P1 { get; set; }
    }

    /// <summary>
    /// C14 : C13, NotifyPropertyChangedAttribute excluded, hand-coded.
    /// </summary>
    [ExcludeAspect( typeof( NotifyPropertyChangedAttribute ) )]
    public class C14 : C13
    {
        private int _c14P1;

        public int C14P1
        {
            get => this._c14P1;
            set
            {
                if ( value != this._c14P1 )
                {
                    this._c14P1 = value;
                    this.OnPropertyChanged( nameof( this.C14P1 ) );
                }
            }
        }
    }

    /// <summary>
    /// C15 : C14, has [NotifyPropertyChanged].
    /// </summary>
    [NotifyPropertyChanged]
    public class C15 : C14
    {
        /// <summary>
        /// Ref to <see cref="C13.C13P1"/>.
        /// </summary>
        public int C15P1 => this.C13P1;

        /// <summary>
        /// Ref to <see cref="C14.C14P1"/>.
        /// </summary>
        public int C15P2 => this.C14P1;
    }

    #endregion
}