// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Definitions;
using Spectre.Console.Cli;
using MetalamaDependencies = PostSharp.Engineering.BuildTools.Dependencies.Definitions.MetalamaDependencies.V2023_3;

var product = new Product( MetalamaDependencies.MetalamaPatterns )
{
    Solutions = new Solution[] { new DotNetSolution( "Metalama.Patterns.sln" ) { CanFormatCode = true } },
  
    // PublicArtifacts = Pattern.Create( "Metalama.Patterns.$(PackageVersion).nupkg" ),
    Dependencies = new[] { DevelopmentDependencies.PostSharpEngineering, MetalamaDependencies.Metalama },
    MainVersionDependency = MetalamaDependencies.Metalama,
    Configurations = Product.DefaultConfigurations.WithValue( 
        BuildConfiguration.Public,
        Product.DefaultConfigurations.Public with
        {
            // TODO: We don't have any packages to publish yet.
            // To start publishing the packages, set the public artifacts above and remove
            // this assignment to Configurations property.
            ExportsToTeamCityDeploy = false,
            
            // This is the first version of Patters.
            // This line should be removed in the next version.
            RequiresUpstreamCheck = false
        } )
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );