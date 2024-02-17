using JetBrains.Annotations;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;

namespace Metalama.Patterns.Contracts
{
    /// <summary>
    /// Custom attribute that, when added to a field, property or parameter, throws an <see cref="ArgumentException"/>
    /// if the target is assigned a value that is not a valid password. If the target is a nullable type, null strings
    /// are accepted and do not throw an exception.
    /// </summary>
    [PublicAPI]
    public sealed class PasswordAttribute : RegularExpressionBaseAttribute
    {
        #region Protected Methods
        protected override IExpression GetRegex()
        {
            var builder = new ExpressionBuilder();
            builder.AppendTypeName(typeof(ContractHelpers));
            builder.AppendVerbatim(".");
            builder.AppendVerbatim(nameof(ContractHelpers.PasswordRegex));

            return builder.ToExpression();
        }

        protected override void OnContractViolated(dynamic? value, dynamic regex)
        { meta.Target.GetContractOptions().Templates!.OnPasswordContractViolated(value); }

        #endregion
    }
}
