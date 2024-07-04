// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Caching.Implementation;
using System.Collections.Concurrent;

namespace Metalama.Patterns.Caching.TestHelpers;

public sealed class BackgroundTaskSchedulerObserver : IBackgroundTaskSchedulerObserver
{
    private readonly ConcurrentDictionary<int, int> _pendingBackgroundTasks = new();
    private volatile int _nextTaskId;

    public int OnTaskEnqueued()
    {
        var id = Interlocked.Increment( ref this._nextTaskId );
        this._pendingBackgroundTasks.TryAdd( id, id );

        return id;
    }

    public void OnTaskCompleted( int observedTaskId )
    {
        this._pendingBackgroundTasks.TryRemove( observedTaskId, out _ );
    }

    public int PendingTasks => this._pendingBackgroundTasks.Count;
}