// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace NpcExperiments.Exp9;

[Layers("1", "2")]
[Inheritable]
class TestAttribute : Attribute, IAspect<INamedType>
{
    [Introduce(Layer = "1")]
    void Foo1()
    {
        var target = meta.Target.Method.DeclaringType;
        Console.WriteLine( $"BaseType='{target.BaseType}', BelToCurProj={target.BaseType?.BelongsToCurrentProject}, " +
            $"HasAspect={(target.BaseType?.BelongsToCurrentProject == true ? target.BaseType?.Enhancements().HasAspect( typeof( TestAttribute ) ) : "n/a")}" );

        foreach ( var pred in meta.AspectInstance.Predecessors )
        {
            Console.WriteLine( $"BasePred = {pred.Instance.TargetDeclaration?.ToString()}" );
        }
    }

    [Introduce( Layer = "2" )]
    void Foo2()
    {
        var target = meta.Target.Method.DeclaringType;
        Console.WriteLine( $"BaseType='{target.BaseType}', BelToCurProj={target.BaseType?.BelongsToCurrentProject}, " +
            $"HasAspect={(target.BaseType?.BelongsToCurrentProject == true ? target.BaseType?.Enhancements().HasAspect( typeof( TestAttribute ) ) : "n/a")}" );

        foreach ( var pred in meta.AspectInstance.Predecessors )
        {
            Console.WriteLine( $"BasePred = {pred.Instance.TargetDeclaration?.ToString()}" );
        }
    }
}

[Test]
class A { }

class B : A { }

class C : B { }

class D : C { }