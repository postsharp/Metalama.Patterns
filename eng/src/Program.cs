// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using PostSharp.Engineering.BuildTools;
using PostSharp.Engineering.BuildTools.Build.Model;
using PostSharp.Engineering.BuildTools.Build.Solutions;
using PostSharp.Engineering.BuildTools.Dependencies.Model;
using Spectre.Console.Cli;

var product = new Product( Dependencies.MetalamaPatterns )
{
    Solutions = new Solution[]
    {
        new DotNetSolution( "src\\Metalama.Patterns.sln" )
    },
    PublicArtifacts = Pattern.Create( "Metalama.Patterns.$(PackageVersion).nupkg" ),
    Dependencies = new[] { Dependencies.PostSharpEngineering, Dependencies.Metalama }
};

var commandApp = new CommandApp();

commandApp.AddProductCommands( product );

return commandApp.Run( args );