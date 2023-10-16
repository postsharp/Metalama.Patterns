namespace Metalama.Patterns.Contracts.AspectTests.Invariants;
public class BaseClass
{
  [Invariant]
  private void TheInvariant()
  {
    if (this.A + this.B != 0)
    {
      throw new InvariantViolationException();
    }
  }
  public void SomePublicMethod()
  {
    try
    {
      return;
    }
    finally
    {
      this.VerifyInvariants();
    }
  }
  protected void SomeProtectedMethod()
  {
  }
  protected internal void SomeProtectedInternalMethod()
  {
    try
    {
      return;
    }
    finally
    {
      this.VerifyInvariants();
    }
  }
  internal void SomeInternalMethod()
  {
    try
    {
      return;
    }
    finally
    {
      this.VerifyInvariants();
    }
  }
  private void SomePrivateMethod()
  {
  }
  private void SomeReadOnlyMethod()
  {
  }
  private int _a;
  public int A
  {
    get
    {
      return this._a;
    }
    set
    {
      try
      {
        this._a = value;
        return;
      }
      finally
      {
        this.VerifyInvariants();
      }
    }
  }
  private int _b;
  public int B
  {
    get
    {
      return this._b;
    }
    set
    {
      try
      {
        this._b = value;
        return;
      }
      finally
      {
        this.VerifyInvariants();
      }
    }
  }
  protected virtual void VerifyInvariants()
  {
    this.TheInvariant();
  }
}
public class DerivedClass : BaseClass
{
  [Invariant]
  private void OtherInvariant()
  {
    if (this.A < this.C)
    {
      throw new InvariantViolationException();
    }
  }
  [DoNotCheckInvariants]
  public void NoInvariant()
  {
  }
  private int _c;
  public int C
  {
    get
    {
      return this._c;
    }
    set
    {
      try
      {
        this._c = value;
        return;
      }
      finally
      {
        this.VerifyInvariants();
      }
    }
  }
  protected override void VerifyInvariants()
  {
    base.VerifyInvariants();
    this.OtherInvariant();
  }
}