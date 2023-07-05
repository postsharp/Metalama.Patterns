// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Metalama.Patterns.Caching.Tests
{
    public class AsyncBarrier
    {
        private readonly int m_participantCount;
        private int m_remainingParticipants;
        private ConcurrentStack<TaskCompletionSource<bool>> m_waiters;

        public AsyncBarrier( int participantCount )
        {
            if ( participantCount <= 0 )
            {
                throw new ArgumentOutOfRangeException( "participantCount" );
            }

            this.m_remainingParticipants = this.m_participantCount = participantCount;
            this.m_waiters = new ConcurrentStack<TaskCompletionSource<bool>>();
        }

        public Task SignalAndWait()
        {
            Console.WriteLine( "SignalAndWait" );
            var tcs = new TaskCompletionSource<bool>();
            this.m_waiters.Push( tcs );

            if ( Interlocked.Decrement( ref this.m_remainingParticipants ) == 0 )
            {
                this.m_remainingParticipants = this.m_participantCount;
                var waiters = this.m_waiters;
                this.m_waiters = new ConcurrentStack<TaskCompletionSource<bool>>();
                Parallel.ForEach( waiters, w => w.SetResult( true ) );
            }

            return tcs.Task;
        }

        public override string ToString()
        {
            return $"{this.m_remainingParticipants}/{this.m_participantCount}";
        }
    }
}