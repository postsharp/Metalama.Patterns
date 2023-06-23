// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using JetBrains.Annotations;

namespace Flashtrace;

/// <summary>
/// Extensions to the <see cref="LogRecordKind"/> enum.
/// </summary>
[PublicAPI]
public static class LogRecordKindExtensions
{
    /// <summary>
    /// Determines whether a given <see cref="LogRecordKind"/> represents the opening of a context.
    /// </summary>
    /// <param name="kind">A <see cref="LogRecordKind"/>.</param>
    /// <returns><c>true</c> if <paramref name="kind"/> represents the opening of a context, otherwise <c>false</c>.</returns>
    public static bool IsOpen( this LogRecordKind kind )
    {
        switch ( kind )
        {
            case LogRecordKind.MethodEntry:
            case LogRecordKind.AsyncMethodResume:
            case LogRecordKind.CustomActivityEntry:
            case LogRecordKind.IteratorMoveNext:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Determines whether a given <see cref="LogRecordKind"/> represents the closing of a context.
    /// </summary>
    /// <param name="kind">A <see cref="LogRecordKind"/>.</param>
    /// <returns><c>true</c> if <paramref name="kind"/> represents the closing of a context, otherwise <c>false</c>.</returns>
    public static bool IsClose( this LogRecordKind kind )
    {
        switch ( kind )
        {
            case LogRecordKind.MethodSuccess:
            case LogRecordKind.MethodOvertime:
            case LogRecordKind.MethodException:
            case LogRecordKind.AsyncMethodAwait:
            case LogRecordKind.CustomActivityException:
            case LogRecordKind.CustomActivitySuccess:
            case LogRecordKind.CustomActivityFailure:
            case LogRecordKind.IteratorYield:
            case LogRecordKind.CustomActivityExit:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Determines whether a given <see cref="LogRecordKind"/> represents the closing of a custom activity.
    /// </summary>
    /// <param name="kind">A <see cref="LogRecordKind"/>.</param>
    /// <returns><c>true</c> if <paramref name="kind"/> represents the closing of a custom activity, otherwise <c>false</c>.</returns>
    public static bool IsCloseCustomActivity( this LogRecordKind kind )
    {
        switch ( kind )
        {
            case LogRecordKind.CustomActivityException:
            case LogRecordKind.CustomActivitySuccess:
            case LogRecordKind.CustomActivityFailure:
            case LogRecordKind.CustomActivityExit:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Determines whether a given <see cref="LogRecordKind"/> represents a standalone record, i.e. a record that does
    /// not have a corresponding opening or closing. For instance, a <see cref="LogRecordKind.CustomRecord"/>
    /// is a standalone record.
    /// </summary>
    /// <param name="kind">A <see cref="LogRecordKind"/>.</param>
    /// <returns><c>true</c> if <paramref name="kind"/> is a standalone record, otherwise <c>false</c>.</returns>
    /// <remarks>
    /// <para><seealso cref="LogRecordKind.MethodException"/> may represent an closing or a standalone record, depending on
    /// the context. This method shall return <c>false</c> for <seealso cref="LogRecordKind.MethodException"/>.</para>
    /// </remarks>
    public static bool IsStandalone( this LogRecordKind kind )
    {
        switch ( kind )
        {
            case LogRecordKind.CustomRecord:
            case LogRecordKind.ExecutionPoint:
            case LogRecordKind.ValueChanged:
                return true;

            default:
                return false;
        }
    }

    /// <summary>
    /// Determines whether a give <see cref="LogRecordKind"/> represents a custom record, emitted by the <see cref="LogLevelSource"/> class.
    /// </summary>
    /// <param name="kind">A <see cref="LogRecordKind"/>.</param>
    /// <returns><c>true</c> if <paramref name="kind"/> is a custom kind of record emitted by the <see cref="LogLevelSource"/> class,
    /// otherwise <c>false</c>.</returns>
    public static bool IsCustom( this LogRecordKind kind )
    {
        switch ( kind )
        {
            case LogRecordKind.CustomRecord:
            case LogRecordKind.CustomActivityEntry:
            case LogRecordKind.CustomActivityException:
            case LogRecordKind.CustomActivitySuccess:
            case LogRecordKind.CustomActivityFailure:
            case LogRecordKind.CustomActivityExit:
            case LogRecordKind.ExecutionPoint:
                return true;

            default:
                return false;
        }
    }
}