// Copyright (c) SharpCrafters s.r.o. This file is not open source. It is released under a commercial
// source-available license. Please see the LICENSE.md file in the repository root for details.

// @Include(_CacheConfigurationAttributeTest.Dependency.cs)

using System;
using System.Diagnostics;
using PostSharp.Patterns.Caching.TestHelpers;
using PostSharp.Patterns.Caching;

[assembly: CacheConfiguration(ProfileName =
    PostSharp.Patterns.Caching.BuildTests.CacheConfigurationAttribute.TestValues.cacheConfigurationAttributeProfileName1)]
