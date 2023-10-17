// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Framework.Aspects;
using System.ComponentModel;
using System.Windows.Input;

namespace Metalama.Patterns.Xaml.Implementation;

internal sealed partial class CommandAspectBuilder
{
    private sealed class Templates : ITemplateProvider
    {
        private Templates() { }

        public static TemplateProvider Provider { get; } = TemplateProvider.FromInstance( new Templates() );
    }

    [Template]
    private static void InitializeCommand()
    {
        // property = new DelegateCommand()
    }
}