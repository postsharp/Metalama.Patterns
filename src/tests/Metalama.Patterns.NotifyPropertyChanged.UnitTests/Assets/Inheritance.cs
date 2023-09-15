// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Core;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.Inheritance
{
    #region Basic Cases

    public partial class InheritsSimpleAddsNothing : Simple
    {
    }

    public partial class I1_InheritsSimpleAddsProperty : Simple
    {
        public int I1P1 { get; set; }
    }

    public partial class I2_InheritsI1AddsProperty : I1_InheritsSimpleAddsProperty
    {
        public int I2P1 { get; set; }
    }

    public partial class I3_InheritsI2AddsProperty : I2_InheritsI1AddsProperty
    {
        public int I3P1 { get; set; }
    }

    public partial class I4_InheritsI3AddsProperty : I3_InheritsI2AddsProperty
    {
        public int I4P1 { get; set; }
    }

    public partial class I10_InheritsSimpleAddsPropertyRefS1 : Simple
    {
        public int I10P1 => this.S1;
    }

    public partial class I11_InheritsI10AddsPropertyRefS1 : I10_InheritsSimpleAddsPropertyRefS1
    {
        public int I11P1 => this.S1;
    }

    #endregion

    #region Inherit from existing non-aspect base

    [NotifyPropertyChanged]
    public partial class I20_InheritsExistingInpcImplWithValidOPCMethod : ExistingInpcImplWithValidOPCMethod
    {
        public int I20P1 { get; set; }

        public int I20P2 => this.EX1 * 2;

        // Ref to EX2.S1 requires [NPC] to provide monitoring of EX2 within this class.
        public int I20P3 => this.EX2.S1;
    }

    #endregion

    // TODO: Move to AspectTests
#if false
    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithValidOPCMethodNamedNotifyOfPropertyChange : ExistingInpcImplWithValidOPCMethodNamedNotifyOfPropertyChange
    {
    }

    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithValidOPCMethodNamedRaisePropertyChanged : ExistingInpcImplWithValidOPCMethodNamedRaisePropertyChanged
    {
    }

    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithOPCMethodThatIsNotVirtual : ExistingInpcImplWithOPCMethodThatIsNotVirtual
    {
    }

    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithOPCMethodThatIsPrivate : ExistingInpcImplWithOPCMethodThatIsPrivate
    {
    }

    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithOPCMethodWithWrongParamCount : ExistingInpcImplWithOPCMethodWithWrongParamCount
    {
    }

    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithOPCMethodWithWrongParamType : ExistingInpcImplWithOPCMethodWithWrongParamType
    {
    }

    [NotifyPropertyChanged]
    public partial class InheritsExistingInpcImplWithoutNPCMethod : ExistingInpcImplWithoutNPCMethod
    {
    }
#endif
}