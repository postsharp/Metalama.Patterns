using Metalama.Framework.Fabrics;
namespace Metalama.Patterns.Contracts.AspectTests.Fabric_Project
{
#pragma warning disable CS0067, CS8618, CS0162, CS0169, CS0414, CA1822, CA1823, IDE0051, IDE0052
  internal class Fabric : ProjectFabric
  {
    public override void AmendProject(IProjectAmender amender) => throw new System.NotSupportedException("Compile-time-only code cannot be called at run-time.");
  }
#pragma warning restore CS0067, CS8618, CS0162, CS0169, CS0414, CA1822, CA1823, IDE0051, IDE0052
  public class PublicClass
  {
    private string _publicProperty = default !;
    public string PublicProperty
    {
      get
      {
        return this._publicProperty;
      }
      set
      {
        if (value == null !)
        {
          throw new global::System.ArgumentNullException("value", "The 'PublicProperty' property must not be null.");
        }
        this._publicProperty = value;
      }
    }
    internal string InternalProperty { get; set; }
    public string? PublicNullableProperty { get; set; }
    private global::System.String _publicField = default !;
    public global::System.String PublicField
    {
      get
      {
        return this._publicField;
      }
      set
      {
        if (value == null !)
        {
          throw new global::System.ArgumentNullException("value", "The 'PublicField' property must not be null.");
        }
        this._publicField = value;
      }
    }
    internal string InternalField;
    private global::System.String _publicNullableField = default !;
    public global::System.String PublicNullableField
    {
      get
      {
        return this._publicNullableField;
      }
      set
      {
        if (value == null !)
        {
          throw new global::System.ArgumentNullException("value", "The 'PublicNullableField' property must not be null.");
        }
        this._publicNullableField = value;
      }
    }
    public void PublicMethod(string nonNullableParam, string? nullableParam)
    {
      if (nonNullableParam == null !)
      {
        throw new global::System.ArgumentNullException("nonNullableParam", "The 'nonNullableParam' parameter must not be null.");
      }
    }
    internal void InternalMethod(string nonNullableParam, string? nullableParam)
    {
    }
  }
}