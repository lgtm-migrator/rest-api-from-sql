﻿using FluentAssertions;
using System;
using Xunit;

namespace CodeGenerator.Tests.MethodDefinitionBuilderSpec
{
    public class WithNameTests
    {
        [Fact]
        public void Builder_sets_name_correctly()
        {
            const string returnType = "void";
            const string name = "MyMethod";
            var sut = new MethodDefinitionBuilder();
            var result = sut
                .WithReturnType(returnType)
                .WithName(name)
                .WithAccessibilityLevel(MethodAccessibilityLevel.Private)
                .Build();
            result.Should().Contain($"{returnType} {name}");
        }

        [Fact]
        public void Builder_throws_invalid_operation_exception_when_name_is_not_provided()
        {
            var sut = new MethodDefinitionBuilder();
            Assert.Throws<InvalidOperationException>(() => sut
                .WithReturnType("void")
                .WithAccessibilityLevel(MethodAccessibilityLevel.Private)
                .Build());
        }
    }
}
