// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Flashtrace
{
    /// <exclude/>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AspectDescription("Tracks suspend/resume events on the async method using custom async activities.")]
    [PSerializable]
    [ProvideAspectRole("AwaitInstrumentationAspect")]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Validation)]
    [AspectRoleDependency(AspectDependencyAction.Order, AspectDependencyPosition.After, StandardRoles.Tracing)]
    [SuppressMessage("Microsoft.Naming", "CA1710")]
    [LinesOfCodeAvoided(0)]
    public sealed class AwaitInstrumentationAspect : MethodLevelAspect
    {
        /// <exclude/>
        [OnMethodEntryAdvice(Master = nameof(OnSuspend), LinesOfCodeAvoided = 0)]
        public static void OnEntry([AsyncCallId] AsyncCallId task)
        {
            AwaitInstrumentationServices.OnAsyncMethodEntry(task);
        }

        /// <exclude/>
        [OnMethodYieldAdvice(LinesOfCodeAvoided = 0)]
        [SelfPointcut]
        public static void OnSuspend([AsyncCallId] AsyncCallId task)
        {
            AwaitInstrumentationServices.OnAsyncMethodSuspended(task);
        }

        /// <exclude/>
        [OnMethodResumeAdvice(Master = nameof(OnSuspend), LinesOfCodeAvoided = 0)]
        public static void OnResume([AsyncCallId] AsyncCallId task)
        {
            AwaitInstrumentationServices.OnAsyncMethodResumed(task);
        }

        /// <exclude/>
        [OnMethodExitAdvice(Master = nameof(OnSuspend), LinesOfCodeAvoided = 0)]
        public static void OnExit([AsyncCallId] AsyncCallId task)
        {
            AwaitInstrumentationServices.OnAsyncMethodExit(task);
        }
    }
}
