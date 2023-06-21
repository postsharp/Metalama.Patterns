// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

using System;
using PostSharp.Patterns.Utilities;

namespace PostSharp.Patterns.Diagnostics.Custom
{
    /// <summary>
    /// Creates instances of the <see cref="ILogger"/> interface. An instance of this interface must be registered into the <see cref="ServiceLocator"/>.
    /// </summary>
    [Obsolete( "Use ILoggerFactoryProvider3" )]
    public interface ILoggerFactory
    {
        /// <summary>
        /// Gets an instance of the <see cref="ILogger"/> interface for a given role and <see cref="Type"/>.
        /// </summary>
        /// <param name="role">The role for which the logger is requested.</param>
        /// <param name="type">The type of the source code that will emit the records.</param>
        /// <returns>An instance of the <see cref="ILogger"/> interface for <paramref name="role"/> and <paramref name="type"/>.</returns>
        ILogger GetLogger( string role, Type type );
    }

    /// <summary>
    /// Creates instances of <see cref="ILoggerFactory2"/>. An instance of this interface must be registered into the <see cref="ServiceLocator"/>.
    /// </summary>
    [Obsolete( "Use ILoggerFactoryProvider3" )]
    public interface ILoggerFactoryProvider
    {
        /// <summary>
        /// Gets an instance of the <see cref="ILoggerFactory2"/> interface.
        /// </summary>
        /// <param name="role">The role for which the logger is requested.</param>
        /// <returns></returns>
        ILoggerFactory2 GetLoggerFactory( string role );
    }

    /// <summary>
    /// A new version of <see cref="ILoggerFactoryProvider"/> that also provides a logger factory that can create loggers based on names rather than
    /// based on a type.
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
    public interface ILoggerFactoryProvider3 : ILoggerFactoryProvider
#pragma warning restore CS0618 // Type or member is obsolete
    {
        /// <summary>
        /// Gets the <see cref="ILoggerFactory3"/> for the given role.
        /// </summary>
        /// <param name="role">The role for which the logger is requested.</param>
        ILoggerFactory3 GetLoggerFactory3( string role );
    }

    /// <summary>
    /// Creates instances of the <see cref="ILogger2"/> interface.
    /// </summary>
    [Obsolete( "Use ILoggerFactory3" )]
    public interface ILoggerFactory2
    {
        /// <summary>
        /// Gets an instance of the <see cref="ILogger2"/> for a specific <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type of the source code that will emit the records.</param>
        /// <returns>An instance of the <see cref="ILogger2"/> interface for <paramref name="type"/>.</returns>
        [Obsolete("Use ILoggerFactory3.GetLogger")]
        ILogger2 GetLogger( Type type );
    }

    /// <summary>
    /// Creates instances of the <see cref="ILogger2"/> interface based on names rather than types, in addition to based on types.
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
    public interface ILoggerFactory3 : ILoggerFactory2
#pragma warning restore CS0618 // Type or member is obsolete
    {
        /// <summary>
        /// Gets an instance of the <see cref="ILogger3"/> for a specific <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type of the source code that will emit the records.</param>
        /// <returns>An instance of the <see cref="ILogger2"/> interface for <paramref name="type"/>.</returns>
        new ILogger3 GetLogger( Type type );

        /// <summary>
        ///Gets an instance of the <see cref="ILogger2"/> interface for a specific <paramref name="sourceName"/>. The name will
        /// usually, but not always, be a type name.
        /// </summary>
        /// <param name="sourceName">Name identifying the returned logger. The backend creates a logger based on this name.</param>
        ILogger3 GetLogger( string sourceName );
    }

}
