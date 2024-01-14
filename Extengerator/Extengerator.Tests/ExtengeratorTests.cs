using System.IO;
using System.Linq;
using Extengerator.Tests.Utils;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Extengerator.Tests;

[TestFixture]
public class ExtengeratorTests 
{
    private Extengerator _generator = null!;
    private CSharpCompilation _compilation = null!;

    [SetUp]
    public void SetUp()
    {
        _generator = new Extengerator();
        _compilation = CSharpCompilation.Create(nameof(ExtengeratorTests));
    }

    [Test]
    public void ItHandlesNoAdditionalFileSet()
    {
        // Arrange
        var driver = CSharpGeneratorDriver.Create(_generator);
        _compilation = CSharpCompilation.Create(nameof(ExtengeratorTests));

        // Act
        driver.RunGeneratorsAndUpdateCompilation(_compilation,
            out var newCompilation,
            out var diagnostics);
        
        var generatedFiles = newCompilation.SyntaxTrees
            .Select(t => Path.GetFileName(t.FilePath))
            .ToArray();
        
        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(diagnostics, Is.Empty);
            Assert.That(generatedFiles, Is.Empty);
        });

    }
    
    
    [Test]
    public void ItHandlesAdditionalFileNotFound()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesEmptyAdditionalFile()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesNoInterfacesFound()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesEmptyConfigurationFile()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItIgnoresAbstractClasses()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesClassesThatHasAGrandParentDerivingFromTheInterface()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesConfigurationDeserializationError()
    {
        //TODO AK:here we could have multiple errors (i.e. overall structure or a single element)
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesMissingValuesInConfiguration()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesWrong_StringFormat_OfTemplate()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItIgnoresFilesNotDerivedFromInterface()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesWrong_AppendFormat_OfReplacer()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesMultipleConfigurationEntries()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesMultipleReplacers()
    {
        // Arrange

        // Act

        // Assert
        Assert.Fail();
    }
}