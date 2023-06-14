﻿// Copyright (c) SharpCrafters s.r.o. See the LICENSE.md file in the root directory of this repository root for details.

using Metalama.Patterns.Tests.Helpers;
using Xunit;

namespace Metalama.Patterns.Contracts.Tests;

public partial class CustomExceptionFactoryTests : IDisposable
{
    public CustomExceptionFactoryTests()
    {
        ResetExceptionFactory();
    }

    public void Dispose()
    {
        ResetExceptionFactory();
    }

    private static void ResetExceptionFactory()
    {
        ContractServices.ResetExceptionFactory();
    }

    [Fact]
    public void TestDefaultExceptionFactory_Success()
    {
        var testingObject = new ContractTesting();
        AssertEx.Throws<ArgumentNullException>( () => testingObject.Method( null! ) );
    }

    [Fact]
    public void TestCustomExceptionFactory_Fallback()
    {
        ContractServices.ExceptionFactory = new EmptyContractExceptionFactory( ContractServices.ExceptionFactory );
        var testingObject = new ContractTesting();
        AssertEx.Throws<ArgumentNullException>( () => testingObject.Method( null! ) );
    }

    [Fact]
    public void TestCustomExceptionFactory_UnknownExceptionType()
    {
        ContractServices.ExceptionFactory = new EmptyContractExceptionFactory( null! );
        var testingObject = new ContractTesting();
        var message = "The [Required] contract failed with ArgumentNullException, but the current ContractExceptionFactory is not configured to instantiate this exception type";
        AssertEx.Throws<InvalidOperationException>( message, () => testingObject.Method( null! ) );
    }

    [Fact]
    public void TestCustomExceptionFactory_Success()
    {
        ContractServices.ExceptionFactory = new CustomContractExceptionFactory( ContractServices.ExceptionFactory );
        var testingObject = new ContractTesting();
        AssertEx.Throws<TestException>( () => testingObject.Method( null! ) );
    }

#if false // TODO: Review - test appears PS-specific.
    [Fact]
    [SuppressWarning("AR0105")]
    public void TestPostconditionDetection()
    {
        LocationValidationContext context = LocationValidationContext.SuccessPostcondition;
        RequiredAttribute requiredAttribute = new RequiredAttribute();        
        Exception exception =
 requiredAttribute.ValidateValueObject( null, "location", LocationKind.Parameter, context );
        Assert.Equal(typeof(PostconditionFailedException), exception.GetType());
    }
#endif

    [Fact]
    public void TestCustomExceptionFactory_ArgumentPassing()
    {
        var check = new Action<Type>(
            ( exceptionType ) => Assert.Equal( typeof( ArgumentException ), exceptionType ) );
        ContractServices.ExceptionFactory = new CustomContractExceptionFactory( ContractServices.ExceptionFactory, check );
        var testingObject = new ContractTesting();
        AssertEx.Throws<ArgumentException>( () => testingObject.Method2( "123456" ) );
    }
}