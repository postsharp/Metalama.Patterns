using Metalama.Framework.Fabrics;
namespace Metalama.Patterns.Contracts.AspectTests.Invariants_Suspend;
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
        if (!this.AreInvariantsSuspended())
        {
          this.VerifyInvariants();
        }
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
        if (!this.AreInvariantsSuspended())
        {
          this.VerifyInvariants();
        }
      }
    }
  }
  [SuspendInvariants]
  public void ExecuteWithoutInvariants()
  {
    using (this.SuspendInvariants())
    {
      try
      {
        this.A = -5;
        this.B = 5;
      }
      finally
      {
        if (!this.AreInvariantsSuspended())
        {
          this.VerifyInvariants();
        }
      }
      return;
    }
  }
  private readonly InvariantSuspensionCounter _invariantSuspensionCounter = new();
  protected bool AreInvariantsSuspended()
  {
    return _invariantSuspensionCounter.AreInvariantsSuspended;
  }
  protected SuspendInvariantsCookie SuspendInvariants()
  {
    this._invariantSuspensionCounter.Increment();
    return new SuspendInvariantsCookie(_invariantSuspensionCounter);
  }
  protected virtual void VerifyInvariants()
  {
    this.TheInvariant();
  }
}
#pragma warning disable CS0067, CS8618, CS0162, CS0169, CS0414, CA1822, CA1823, IDE0051, IDE0052
public class Fabric : ProjectFabric
{
  public override void AmendProject(IProjectAmender amender) => throw new System.NotSupportedException("Compile-time-only code cannot be called at run-time.");
}