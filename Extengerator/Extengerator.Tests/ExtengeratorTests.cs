using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extengerator.Tests.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using VerifyNUnit;

namespace Extengerator.Tests;

[TestFixture]
public class ExtengeratorTests 
{

    private Extengerator _generator = null!; //TODO AK: remove?
    private CSharpCompilation _compilation = null!; //TODO AK: remove?
    
    private IEnumerable<PortableExecutableReference> _references = null!;

    
    private const string SnapShotDirectory = "Snapshots";

    /*language=c#*/
    private const string TestInterface = 
        """
        namespace Test;
        public interface ITest {}
        """;
    
    /*language=c#*/
    private const string TestClass1 =
        """
        using namespace Test;
        public  class Test1 : ITest {};
        """;

    [SetUp]
    public void SetUp()
    {
        _generator = new Extengerator();
        _compilation = CSharpCompilation.Create(nameof(ExtengeratorTests));

        _references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
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
    public Task ItCreatesSource()
    {
        // Arrange
        var sources = new[]{
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration =
            """
            - interfaceType: Test.ITest
              template: |-
                namespace Test
                {{
                  {0}
                }}
              replacer:
                - '//{0};'
              fileName: TestItCreatesSource
            """;
        
        // Act
        var actual = Act(configuration, sources);
        
        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
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
    
    private GeneratorDriver Act(string configuration, params string[]? sources)
    {
        var syntaxTrees = sources?.Select(s => CSharpSyntaxTree.ParseText(s));
        
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: _references);

        var generators = new[] {_generator}.Select(GeneratorExtensions.AsSourceGenerator);
        var additionalTexts = new[]
        {
            new TestAdditionalFile("./Extengerator.settings.yaml", configuration)
        };
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators, additionalTexts);

        return driver.RunGenerators(compilation);
    }
}