using System.Collections.Generic;
using System.Linq;
using AJP.MediatrEndpoints.Swagger;
using FluentAssertions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Xunit;

namespace AJP.MediatrEndpoints.Tests
{
    public class SwaggerTests
    {
        [Fact]
        public void NewDictionary_should_return_a_new_dictionary()
        {
            var result = ParameterDictionaryBuilder.NewDictionary();

            result.Should().NotBeNull();
            result.Should().BeOfType<Dictionary<string, OpenApiParameter>>();
        }
        
        [Fact]
        public void AddBoolParam_should_add_bool_param()
        {
            var dict = ParameterDictionaryBuilder.NewDictionary();
            dict.AddBoolParam("boolParam", ParameterDictionaryBuilder.In.Header, true, "boolParamDescription");

            dict.Count.Should().Be(1);
            dict.First().Value.Name.Should().Be("boolParam");
            dict.First().Value.Schema.Type.Should().Be("boolean");
            dict.First().Value.In.Should().Be(ParameterLocation.Header);
            dict.First().Value.Required.Should().BeTrue();
            dict.First().Value.Deprecated.Should().BeFalse();
            dict.First().Value.Description.Should().Be("boolParamDescription");
        }
        
        [Fact]
        public void AddStringParam_should_add_string_param()
        {
            var dict = ParameterDictionaryBuilder.NewDictionary();
            dict.AddStringParam("stringParam", ParameterDictionaryBuilder.In.Query, true, "stringParamDescription");

            dict.Count.Should().Be(1);
            dict.First().Value.Name.Should().Be("stringParam");
            dict.First().Value.Schema.Type.Should().Be("string");
            dict.First().Value.In.Should().Be(ParameterLocation.Query);
            dict.First().Value.Required.Should().BeTrue();
            dict.First().Value.Deprecated.Should().BeFalse();
            dict.First().Value.Description.Should().Be("stringParamDescription");
        }
        
        [Fact]
        public void AddIntegerParam_should_add_integer_param()
        {
            var dict = ParameterDictionaryBuilder.NewDictionary();
            dict.AddIntegerParam("integerParam", ParameterDictionaryBuilder.In.Route, true, "integerParamDescription", "<100");

            dict.Count.Should().Be(1);
            dict.First().Value.Name.Should().Be("integerParam");
            dict.First().Value.Schema.Type.Should().Be("integer");
            dict.First().Value.In.Should().Be(ParameterLocation.Path);
            dict.First().Value.Required.Should().BeTrue();
            dict.First().Value.Deprecated.Should().BeFalse();
            dict.First().Value.Description.Should().Be("integerParamDescription");
            ((OpenApiString)dict.First().Value.Example).Value.Should().Be("<100");
        }
        
        [Fact]
        public void AddEnumParam_should_add_enum_param()
        {
            var dict = ParameterDictionaryBuilder.NewDictionary();
            dict.AddEnumParam("enumParam", typeof(TestEnum), ParameterDictionaryBuilder.In.Route, false, "enumParamDescription", "", true);

            dict.Count.Should().Be(1);
            dict.First().Value.Name.Should().Be("enumParam");
            dict.First().Value.Schema.Type.Should().Be("string");
            dict.First().Value.Schema.Enum.Count.Should().Be(3);
            ((OpenApiString)dict.First().Value.Schema.Enum.First()).Value.Should().Be("Red");
            dict.First().Value.In.Should().Be(ParameterLocation.Path);
            dict.First().Value.Required.Should().BeFalse();
            dict.First().Value.Deprecated.Should().BeTrue();
            dict.First().Value.Description.Should().Be("enumParamDescription");
        }
    }
}