// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using FluentAssertions;
using Metalama.Patterns.NotifyPropertyChanged.UnitTests.Assets.ChildPropertyAssets;
using Xunit;

namespace Metalama.Patterns.NotifyPropertyChanged.UnitTests;

public sealed class ChildPropertyTests : InpcTestsBase
{
    [Fact]
    public void FourDeep()
    {
        var a = new A();
        var b = a.A2;
        var c = b.B2;
        var d = c.C2;

        var subA = this.SubscribeTo( a );

        // Reminder:
        // public int A3 => this.A2?.B2?.C2?.D1 ?? -1;

        // 1. Change leaf value D1

        d.D1 = 1;

        this.Events.Should().Equal(
            (subA, "A3") );

        this.Events.Clear();

        // 2. Change leaf parent ref, but leaf value is the same. This is notified as a change because
        // there is no false positive detection (we don't store a copy of the value of D1)

        var d2 = new D() { D1 = d.D1 };

        a.A2.B2.C2 = d2;

        this.Events.Should().Equal(
            (subA, "A3") );

        this.Events.Clear();

        // 3. Change leaf parent-parent ref, but parent is the same object. This is not notified as a change
        // beacuse we have to store the last value of the parent object anyhow, and we detect that it is the
        // same ref.

        var c2 = new C() { C2 = a.A2.B2.C2 };

        a.A2.B2 = c2;

        this.Events.Should().BeEmpty();

        this.Events.Clear();

        // 4. Change leaf parent-parent ref, parent is a different object, but leaf value D1 is the same.
        // This is notified as a change beacuse we have to store the last value of the parent object anyhow,
        // and we detect that it is a different ref. But we don't have false positive detection so we
        // can't tell that leaf value D1 is actually the same value.

        var c3 = new C() { C2 = new D() { D1 = c.C2.D1 } };

        a.A2.B2 = c3;

        this.Events.Should().Equal(
            (subA, "A3") );

        this.Events.Clear();

        // 5. Change leaf parent-parent ref, parent is a different object, and leaf value D1 is a different value.
        // This is notified as a change beacuse we have to store the last value of the parent object anyhow, and
        // we detect that it is a different ref.

        var c4 = new C() { C2 = new D() };

        a.A2.B2 = c4;

        this.Events.Should().Equal(
            (subA, "A3") );

        this.Events.Clear();

        // 6. Change leaf parent-parent-parent ref. As per above comments, because we don't have false
        // positive detection, it does not matter if leaf value D1 is changed or not, a change is
        // notified. However, if the D ref in C2 is the same ref, even if the ancestor C and B refs are
        // different, then we can tell it's not a change.

        // 6.1 Change leaf parent-parent-parent ref and parent-parent ref, but keep same parent ref.
        // No change of A3 is notified, but A2 change is notified.

        var b2 = new B() { B2 = new C() { C2 = a.A2.B2.C2 } };

        a.A2 = b2;

        this.Events.Should().Equal(
            (subA, "A2") );

        this.Events.Clear();

        // 6.1 Change leaf parent-parent-parent ref, parent-parent ref, and parent ref.
        // Change of both A3 and A2 is notified. Order is expected to be leaf-to-root.

        var b3 = new B() { B2 = new C() { C2 = new D() } };

        a.A2 = b3;

        this.Events.Should().Equal(
            (subA, "A3"),
            (subA, "A2") );
    }

    [Fact]
    public void MultipleChildRefsAtDifferentLevels()
    {
        // NB: Remember, there is no false positive detection for leaf values.

        var e = new E();

        var sub = this.SubscribeTo( e );

        e.E2.B2.C2.D1 = 1;

        this.Events.Should().Equal(
            (sub, "LR" ) );

        this.Events.Clear();

        e.E2.B2.C2 = new D();

        this.Events.Should().Equal(
            (sub, "LR") );

        this.Events.Clear();

        e.E2.B2 = new C();

        this.Events.Should().Equal(
            (sub, "LR"),
            (sub, "LP1R") );

        this.Events.Clear();

        e.E2 = new B();

        this.Events.Should().Equal(
            (sub, "LR"),
            (sub, "LP1R"),
            (sub, "LP2R"),
            (sub, "E2") );

        this.Events.Clear();
    }
}