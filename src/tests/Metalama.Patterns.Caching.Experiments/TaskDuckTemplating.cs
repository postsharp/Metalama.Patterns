// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: [Porting] Temporary, initial development only. Remove or adapt to proper tests.
// ReSharper disable all
#pragma warning disable

// TODO: Work around #33441 : Some method calls in scope via `using static` are not transformed.
using Metalama.Framework.Advising;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Patterns.Caching.Implementation;
using System;
using System.Runtime.CompilerServices;
using static Flashtrace.FormattedMessageBuilder;
using static Metalama.Patterns.Caching.Experiments.InfoWriter;

namespace Metalama.Patterns.Caching.Experiments.TaskDuckTemplating;

#if false
[Template]
void xxx( [CompileTime] IEnumerable<IMethod> v ) // v = [1,2,3]
{
    // new[] { `v[0].ToMethodInfo().ThrowIfMissing( v[0].ToDisplayString() )` , ... }
    var array = meta.RunTime( v.Select( m => m.ToMethodInfo().ThrowIfMissing( m.ToDisplayString() ) ).ToArray() );
}
#endif

public sealed class DuckAttribute : MethodAspect
{
    public override void BuildAspect( IAspectBuilder<IMethod> builder )
    {
        var asyncInfo = builder.Target.GetAsyncInfo();

        var templates = new MethodTemplateSelector(
            nameof( OverrideMethod ),
            asyncInfo.ResultType.Is( SpecialType.Void ) ? nameof( OverrideMethodAsyncTask ) : nameof( OverrideMethodAsyncTaskOfT ),
            useAsyncTemplateForAnyAwaitable: true );


        builder.Advice.Override( builder.Target, templates,
            args: new
            {
                TResult = asyncInfo.ResultType,
                TTask = builder.Target.ReturnType,
                resultType = asyncInfo.ResultType
            } );
        ;
    }

    [Template]
    public dynamic? OverrideMethod<[CompileTime] TResult, [CompileTime] TTask>( IType resultType )
        where TTask : Task<TResult>
    {
        var result = resultType.DefaultValue();

        result = meta.Proceed();

        Thread.Yield();

        return result;
    }

    [Template]
    public async Task<dynamic> OverrideMethodAsyncTaskOfT<[CompileTime] TResult, [CompileTime] TTask>( IType resultType )
        where TTask : Task<TResult>
    {
        var result = resultType.DefaultValue();

        var task = meta.Proceed();

        if ( task.IsCompleted )
        {
            result = task.GetAwaiter().GetResult();
        }
        else
        {
            // activity.Suspend();
            // try {
            result = await task;
            // } finally { activity.Resume(); }
        }

        return result;
    }

    [Template]
    public async Task OverrideMethodAsyncTask<[CompileTime] TResult, [CompileTime] TTask>( IType resultType )
        where TTask : Task<TResult>
    {
        var task = meta.Proceed();

        if ( task.IsCompleted )
        {
            task.GetAwaiter().GetResult();
        }
        else
        {
            // activity.Suspend();
            // try {
            await task;
            // } finally { activity.Resume(); }
        }

        return;
    }

}

public class Test
{
    [Duck]
    void VoidMethod()
    {
        Thread.Yield();
    }

    [Duck]
    int IntMethod()
    {
        return 42;
    }

    [Duck]
    Task TaskMethod()
    {
        return Task.CompletedTask;
    }

    [Duck]
    Task<int> TaskOfIntMethod()
    {
        return Task<int>.FromResult( 42 );
    }

    [Duck]
    async Task TaskMethodAsync()
    {
        await Task.Delay( 1 );
    }

    [Duck]
    async Task<int> TaskOfIntMethodAsync()
    {
        await Task.Delay( 1 );
        return 42;
    }


    [Duck]
    ValueTask ValueTaskMethod()
    {
        return ValueTask.CompletedTask;
    }

    [Duck]
    ValueTask<int> ValueTaskOfIntMethod()
    {
        return new ValueTask<int>( 42 );
    }

    [Duck]
    async ValueTask ValueTaskMethodAsync()
    {
        await Task.Delay( 1 );
    }

    [Duck]
    async ValueTask<int> ValueTaskOfIntMethodAsync()
    {
        await Task.Delay( 1 );
        return 42;
    }
}
