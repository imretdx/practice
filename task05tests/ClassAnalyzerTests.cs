using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using task05;

namespace task05tests;

#pragma warning disable CS8618, CS0169
public class TestClass
{
    public int PublicField;
    private string _privateField;
    public int Property { get; set; }

    public void Method() { }
    public string ComplexMethod(int id, string name) => string.Empty;
}
#pragma warning restore CS8618, CS0169

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
        Assert.Contains("ComplexMethod", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_ChecksCorrectAttributes()
    {
        var analyzerWithAttr = new ClassAnalyzer(typeof(AttributedClass));
        var analyzerWithoutAttr = new ClassAnalyzer(typeof(TestClass));

        Assert.True(analyzerWithAttr.HasAttribute<SerializableAttribute>());
        Assert.False(analyzerWithoutAttr.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void GetMethodParams_ReturnsCorrectSignature()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var info = analyzer.GetMethodParams("ComplexMethod").ToList();

        Assert.Equal("Return: String", info[0]);
        Assert.Equal("Int32 id", info[1]);
        Assert.Equal("String name", info[2]);
    }
}
