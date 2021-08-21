﻿using FluentAssertions;
using Xunit;

namespace CodeGenerator.Tests.ClassBuilderSpec
{
    public class ClassBuilderTests
    {
        [Fact]
        public void Fully_configured_classes_are_rendered_in_the_correct_format()
        {
            var sut = new ClassBuilder();

            var method1 = new Method(MethodAccessibilityLevel.Public, "Task<IActionResult>", "Create",
                    @"await _context.Pets.AddAsync(pet);
await _context.SaveChangesAsync();
return Ok();");
            method1.AddModifier("async");
            method1.AddAttribute("[HttpPost]");
            var method1Arg1 = new MethodArgument("Pet", "pet");
            method1Arg1.AddAttribute("[FromBody]");
            method1.AddArgument(method1Arg1);

            var method2 = new Method(MethodAccessibilityLevel.Public, "Task<IActionResult>", "Get",
                @"var pet = await _context.Pets.FindAsync(id);
if (pet == null)
{
    return NotFound();
}
return Ok(pet);");
            method2.AddModifier("async");
            method2.AddAttribute("[HttpGet(\"{id}\")]");
            var method2Arg1 = new MethodArgument("int", "id");
            method2Arg1.AddAttribute("[FromRoute]");
            method2.AddArgument(method2Arg1);

            var method3 = new Method(MethodAccessibilityLevel.Public, "Task<IActionResult>", "Feed");
            method3.AddModifier("async");
            method3.AddAttribute("[HttpPut(\"{id}/feed\")]");
            method3.AddAttribute("[HttpPut(\"feed/{id}\")]");
            var method3Arg1 = new MethodArgument("int", "id");
            method3Arg1.AddAttribute("[FromRoute]");
            method3.AddArgument(method3Arg1);
            var method3Arg2 = new MethodArgument("FeedCommand", "command");
            method3Arg2.AddAttribute("[FromBody]");
            method3.AddArgument(method3Arg2);

            var method4 = new Method(MethodAccessibilityLevel.Private, "void", "MyMethod");
            var method4Arg = new MethodArgument("string", "text", "in");
            method4.AddArgument(method4Arg);

            var result = sut
                .UsingNamespace("MyApplication.Infrastructure.Database")
                .UsingNamespace("Microsoft.EntityFrameworkCore")
                .UsingNamespace("Microsoft.AspNetCore.Mvc")
                .WithNamespace("MyApplication.Controllers")
                .WithAccessibilityLevel(ClassAccessibilityLevel.Public)
                .WithModifier("sealed")
                .WithName("PetsController")
                .WithBaseClass("MyBaseController")
                .ImplementingInterface("IEmptyInterface")
                .ImplementingInterface("IAnotherEmptyInterface")
                .WithAttribute("[Authorize]")
                .WithAttribute("[SomeExceptionFilter]")
                .WithField(new Field("PetsDbContext", "_context"))
                .WithField(new Field("int", "_whatever", " = 42;"))
                .WithProperty(new Property("string", "SomeProperty"))
                .WithProperty(new Property("PetsDbContext", "Context", " => _context;"))
                .WithConstructor(@"public PetsController(PetsDbContext context, ILogger<PetsController> logger) : base(logger) 
{
    _context = context;
}")
                .WithConstructor(@"public PetsController(ILogger<PetsController> logger) : base(logger) 
{
}")
                .WithMethod(method1)
                .WithMethod(method2)
                .WithMethod(method3)
                .WithMethod(method4)
                .Build();

            result.Should().Be(@"using MyApplication.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace MyApplication.Controllers
{
    [Authorize]
    [SomeExceptionFilter]
    public sealed class PetsController : MyBaseController, IEmptyInterface, IAnotherEmptyInterface
    {
        private PetsDbContext _context;
        private int _whatever = 42;

        public string SomeProperty { get; set; }
        public PetsDbContext Context  => _context;

        public PetsController(PetsDbContext context, ILogger<PetsController> logger) : base(logger) 
        {
            _context = context;
        }

        public PetsController(ILogger<PetsController> logger) : base(logger) 
        {
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet(""{id}"")]
        public async Task<IActionResult> Get([FromRoute]int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet == null)
            {
                return NotFound();
            }
            return Ok(pet);
        }

        [HttpPut(""{id}/feed"")]
        [HttpPut(""feed/{id}"")]
        public async Task<IActionResult> Feed([FromRoute]int id, [FromBody]FeedCommand command)
        {
        }

        private void MyMethod(in string text)
        {
        }
    }
}");
        }

        [Fact]
        public void Minimally_configured_classes_are_rendered_correctly_without_any_excess_whitespace()
        {
            var sut = new ClassBuilder();
            var result = sut
                .WithName("PetsController")
                .WithNamespace("MyApplication.Controllers")
                .WithAccessibilityLevel(ClassAccessibilityLevel.Public)
                .Build();

            result.Should().Be(@"namespace MyApplication.Controllers
{
    public class PetsController
    {
    }
}");
        }
    }
}