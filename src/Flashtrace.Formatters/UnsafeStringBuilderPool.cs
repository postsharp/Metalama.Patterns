// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using System.Collections.Concurrent;

namespace Flashtrace.Formatters;

/// <summary>
/// A thread-safe pool of <see cref="UnsafeStringBuilder"/>.
/// </summary>
public sealed class UnsafeStringBuilderPool : IDisposable
{
    private readonly ConcurrentStack<UnsafeStringBuilder> instances = new ConcurrentStack<UnsafeStringBuilder>();

    /// <summary>
    /// Gets the maximum number of characters in instances of the <see cref="UnsafeStringBuilder"/> class managed by the current pool.
    /// </summary>
    public int StringBuilderCapacity { get; }

    private readonly bool throwOnOverflow;
    private readonly int maxInstances;

    /// <summary>
    /// Initializes a new <see cref="UnsafeStringBuilderPool"/>.
    /// </summary>
    /// <param name="stringBuilderCapacity">Maximal number of characters in the <see cref="UnsafeStringBuilder"/> in the pool.</param>
    /// <param name="throwOnOverflow"><c>true</c> if an <see cref="OverflowException"/> should be thrown when
    /// the buffer capacity is insufficient, <c>false</c> if the <c>Append</c> method should return <c>false</c> without exception.</param>
    /// <param name="maxInstances">Maximal number of instances in the pool. The default value is 32.</param>
    public UnsafeStringBuilderPool( int stringBuilderCapacity, bool throwOnOverflow, int maxInstances = 32 )
    {
        this.StringBuilderCapacity = stringBuilderCapacity;
        this.throwOnOverflow = throwOnOverflow;
        this.maxInstances = maxInstances;
    }

    /// <summary>
    /// Gets an instance from the pool.
    /// </summary>
    /// <returns>An <see cref="UnsafeStringBuilder"/>.</returns>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
    public UnsafeStringBuilder GetInstance()
    {
        UnsafeStringBuilder stringBuilder;
        if ( !this.instances.TryPop( out stringBuilder ) )
        {
            stringBuilder = new UnsafeStringBuilder( this.StringBuilderCapacity, this.throwOnOverflow );
        }

        return stringBuilder;
    }

    /// <summary>
    /// Returns an <see cref="UnsafeStringBuilder"/> to the pool.
    /// </summary>
    /// <param name="stringBuilder">An <see cref="UnsafeStringBuilder"/> that has compatible values of the <see cref="UnsafeStringBuilder.Capacity"/>
    /// and <see cref="UnsafeStringBuilder.ThrowOnOverflow"/>.</param>
    public void ReturnInstance( UnsafeStringBuilder stringBuilder )
    {
        if ( stringBuilder == null )
            throw new ArgumentNullException( nameof( stringBuilder ) );
        
        if ( stringBuilder.IsDisposed )
            throw new ObjectDisposedException(nameof(UnsafeStringBuilder));

        if ( stringBuilder.Capacity != this.StringBuilderCapacity || stringBuilder.ThrowOnOverflow != this.throwOnOverflow )
            throw new ArgumentOutOfRangeException(nameof(stringBuilder));

        if ( this.instances.Count >= this.maxInstances )
        {
            stringBuilder.Dispose();
        }
        else
        {
            stringBuilder.Clear();
            this.instances.Push( stringBuilder );
        }

    }

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
    public void Dispose()
    {
        UnsafeStringBuilder stringBuilder;
        while ( this.instances.TryPop( out stringBuilder ) )
        {
            stringBuilder.Dispose();
        }
    }
}
