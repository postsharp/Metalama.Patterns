// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

namespace Flashtrace
{
    [ExplicitCrossPackageInternal]
    internal static class AwaitInstrumentationServices
    {
        public static event Action<AsyncCallId> AsyncMethodSuspended;
        public static event Action<AsyncCallId> AsyncMethodResumed;

        [ThreadStatic]
        private static Stack<AsyncCallId> taskStack;

        private static Stack<AsyncCallId> TaskStack
        {
            get
            {
                if (taskStack == null)
                    taskStack = new Stack<AsyncCallId>();
                return taskStack;
            }
        }

        private static void Pop(AsyncCallId task)
        {
            AsyncCallId poppedTask = TaskStack.Pop();
            if (poppedTask != task)
                throw new AssertionFailedException();
        }

        private static void Push(AsyncCallId task)
        {
            TaskStack.Push(task);
        }

        internal static void OnAsyncMethodSuspended(AsyncCallId task)
        {
            Pop(task);

            AsyncMethodSuspended?.Invoke(task);
        }

        public static AsyncCallId CurrentTask
        {
            get
            {
                Stack<AsyncCallId> t = TaskStack;

                if (t.Count == 0)
                    return AsyncCallId.Null;
                else
                    return t.Peek();

            }
        }

   

        internal static void OnAsyncMethodResumed(AsyncCallId task)
        {
            Push(task);

            AsyncMethodResumed?.Invoke(task);
        }

        internal static void OnAsyncMethodExit(AsyncCallId task)
        {
            Pop(task);
        }

        internal static void OnAsyncMethodEntry(AsyncCallId task)
        {
            Push(task);
        }
    }
}
