// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

// TODO: Review - these tests mess with static state and cause intermittent failures in other tests under default test execution conditions.

#if false
using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;

public class TestException : Exception
{
    public TestException(string message) : base(message)
    {
    }
}


public class CustomExceptionFactoryTests : IDisposable
{
    class ContractTesting
    {
        public bool Method( [Required] string parameter )
        {
            return true;
        }

        public void Method2( [StringLength(5)] string parameter)
        {
        }
    }

    private sealed class EmptyContractExceptionFactory : ContractExceptionFactory
    {
        public EmptyContractExceptionFactory( ContractExceptionFactory next ) : base( next )
        {
        }
    }

    private sealed class CustomContractExceptionFactory : ContractExceptionFactory
    {
        private readonly Action<Type> callback;

        public CustomContractExceptionFactory( ContractExceptionFactory next, Action<Type> callback = null) : base( next )
        {
            this.callback = callback;
        }

        public override Exception CreateException( ContractExceptionInfo exceptionInfo )
        {
            this.callback?.Invoke( exceptionInfo.ExceptionType );
            if ( exceptionInfo.ExceptionType == typeof(ArgumentNullException) )
            {
                return new TestException( "test" );
            }

            return base.CreateException( exceptionInfo );
        }
    }

    public CustomExceptionFactoryTests()
    {
        this.ResetExceptionFactory();
    }

    public void Dispose()
    {
        this.ResetExceptionFactory();
    }

    private void ResetExceptionFactory()
    {
        ContractServices.ResetExceptionFactory();
    }

    [Fact]
    public void TestDefaultExceptionFactory_Success()
    {
        ContractTesting testingObject = new ContractTesting();
        AssertEx.Throws<ArgumentNullException>( () => testingObject.Method(null) );
    }

    [Fact]
    public void TestCustomExceptionFactory_Fallback()
    {
        ContractServices.ExceptionFactory = new EmptyContractExceptionFactory( ContractServices.ExceptionFactory );
        ContractTesting testingObject = new ContractTesting();
        AssertEx.Throws<ArgumentNullException>( () => testingObject.Method(null) );
    }

    [Fact]
    public void TestCustomExceptionFactory_UnknownExceptionType()
    {
        ContractServices.ExceptionFactory = new EmptyContractExceptionFactory( null ); 
        ContractTesting testingObject = new ContractTesting();
        string message = "The [Required] contract failed with ArgumentNullException, but the current ContractExceptionFactory is not configured to instantiate this exception type";
        AssertEx.Throws<InvalidOperationException>( message, () => testingObject.Method(null) );
    }

    [Fact]
    public void TestCustomExceptionFactory_Success()
    {
        ContractServices.ExceptionFactory = new CustomContractExceptionFactory( ContractServices.ExceptionFactory ); 
        ContractTesting testingObject = new ContractTesting();
        AssertEx.Throws<TestException>( () => testingObject.Method(null) );
    }

#if false // TODO: Review - test appears PS-specific.
    [Fact]
    [SuppressWarning("AR0105")]
    public void TestPostconditionDetection()
    {
        LocationValidationContext context = LocationValidationContext.SuccessPostcondition;
        RequiredAttribute requiredAttribute = new RequiredAttribute();        
        Exception exception = requiredAttribute.ValidateValueObject( null, "location", LocationKind.Parameter, context );
        Assert.Equal(typeof(PostconditionFailedException), exception.GetType());
    }
#endif

    [Fact]
    public void TestCustomExceptionFactory_ArgumentPassing()
    {
        var check = new Action<Type>(
            ( exceptionType ) =>
            {
                Assert.Equal( typeof(ArgumentException), exceptionType );
            } );
        ContractServices.ExceptionFactory = new CustomContractExceptionFactory( ContractServices.ExceptionFactory, check );
        ContractTesting testingObject = new ContractTesting();
        AssertEx.Throws<ArgumentException>( () => testingObject.Method2( "123456" ) );
    }
}
#endif