// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using Spectre.Console.Cli;
using MetalamaDependencies = PostSharp.Engineering.BuildTools.Dependencies.Definitions.MetalamaDependencies.V2023_4;

var product = new Product( MetalamaDependencies.MetalamaPatterns )
{
    Solutions = new Solution[] { new DotNetSolution( "Metalama.Patterns.sln" ) { CanFormatCode = true } },
    PublicArtifacts = Pattern.Create(
        "Metalama.Patterns.Caching.$(PackageVersion).nupkg",
        "Metalama.Patterns.Caching.Aspects.$(PackageVersion).nupkg",
        "Metalama.Patterns.Caching.Backend.$(PackageVersion).nupkg",
        "Metalama.Patterns.Caching.Backends.Azure.$(PackageVersion).nupkg",
        "Metalama.Patterns.Caching.Backends.Redis.$(PackageVersion).nupkg",
        "Metalama.Patterns.Contracts.$(PackageVersion).nupkg",
        "Metalama.Patterns.Memoization.$(PackageVersion).nupkg",
        "Metalama.Patterns.NotifyPropertyChanged.$(PackagePreviewVersion).nupkg",
        "Flashtrace.$(PackageVersion).nupkg",
        "Flashtrace.Formatters.$(PackageVersion).nupkg" ),
    Dependencies = new[] { DevelopmentDependencies.PostSharpEngineering, MetalamaDependencies.MetalamaExtensions },
    MainVersionDependency = MetalamaDependencies.Metalama,
    Configurations = Product.DefaultConfigurations.WithValue(
        BuildConfiguration.Public,
        Product.DefaultConfigurations.Public with
        {
            // This is the first version of Patters.
            // This line should be removed in the next version.
            RequiresUpstreamCheck = false
        } )
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );