using System.Collections.Generic;
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
    private Extengerator _generator = null!;
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

    /*language=c#*/
    private const string TestClassWithoutInterface =
        """
        using namespace Test;
        public class TestWithoutInterface : {};
        """;

    /*language=c#*/
    private const string OtherTestInterface =
        """
        namespace Test;
        public interface IOtherTest {}
        """;

    /*language=c#*/
    private const string OtherTestClass =
        """
        using namespace Test;
        public  class OtherTest : IOtherTest {};
        """;

    /*language=yaml*/
    private const string SimpleConfiguration =
        """
        - interfaceType: Test.ITest
          template: |-
            namespace Test
            {{
              {0}
            }}
          replacer:
            - //{0};
          fileName: TestSource
        """;

    [SetUp]
    public void SetUp()
    {
        _generator = new Extengerator();
        _references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location)
        };
    }

    [Test]
    public Task ItProducesNoOutputIfAdditionalFileNotIsSet()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        IEnumerable<AdditionalText>? additionalTexts = null;

        // Act
        var actual = Act(additionalTexts, sources);

        // Assert
        // there's no verified snapshot for this since, since no output is expected
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesNoOutputIfAdditionalFileIsEmpty()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        const string configuration = "";

        // Act
        var actual = Act(configuration, sources);

        // Assert
        // there's no verified snapshot for this since, since no output is expected
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfAdditionalFileIsNotFound()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        IEnumerable<AdditionalText>? additionalTexts = new[]
        {
            new TestAdditionalFile("./Extengerator.settings.yaml", null)
        };

        // Act
        var actual = Act(additionalTexts, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesNoOutputIfTheProjectHasNoClassImplementingAnInterface()
    {
        // Arrange
        var sources = new[] {TestClassWithoutInterface};

        // Act
        var actual = Act(SimpleConfiguration, sources);

        // Assert
        // there's no verified snapshot for this since, since no output is expected
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesNoOutputIfNoClassImplementsTheInterface()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClassWithoutInterface,
        };

        // Act
        var actual = Act(SimpleConfiguration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItIgnoresAbstractClasses()
    {
        // Arrange
        /*language=c#*/
        const string abstractTestClass =
            """
            using namespace Test;
            public abstract class AbtractTest : ITest {};
            """;

        var sources = new[]
        {
            TestInterface,
            abstractTestClass,
            TestClass1,
        };

        // Act
        var actual = Act(SimpleConfiguration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItDoesNotProduceOutputForClassesWithABaseClassImplementingTheInterface()
    {
        // Arrange
        /*language=c#*/
        const string derivedFromTestClass1 =
            """
            using namespace Test;
            public  class TestDerived : Test1 {};
            """;

        var sources = new[]
        {
            TestInterface,
            TestClass1,
            derivedFromTestClass1
        };

        // Act
        var actual = Act(SimpleConfiguration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfIfConfigurationDeserializationFails()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                           {1}
                                         }}
                                       replacer:
                                         - // we have an unquoted colon here: {0}
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfInterfaceTypeIsMissingInConfiguration()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                       - template: |-
                                             namespace Test
                                             {{
                                               {0}
                                             }}
                                         replacer:
                                          - //{0};
                                         fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfTemplateIsMissingInConfiguration()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       replacer:
                                         - //{0};
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfReplacerIsMissingInConfiguration()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                         }}
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfReplacerIsEmptyInConfiguration()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                         }}
                                       replacer:
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfFileNameIsMissingInConfiguration()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                         }}
                                       replacer:
                                         - //{0};
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfTemplateFormatStringIsMalformed()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {
                                           {0}
                                         }
                                       replacer:
                                         - //{0};
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesAWarningIfReplacerFormatStringIsMalformed()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                         }}
                                       replacer:
                                         - // Action a = () => { {0}.SomeStaticMethod(); }
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItIgnoresFilesNotDerivedFromInterface()
    {
        // Arrange
        var sources = new[]
        {
            OtherTestInterface,
            OtherTestClass
        };

        // Act
        var actual = Act(SimpleConfiguration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItCreatesSource()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        // Act
        var actual = Act(SimpleConfiguration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesOneSourceFilePerConfigurationEntry()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
            OtherTestInterface,
            OtherTestClass
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                         }}
                                       replacer:
                                         - //{0};
                                       fileName: TestSource
                                     - interfaceType: Test.IOtherTest
                                       template: |-
                                         namespace OtherTest
                                         {{
                                          {0}
                                         }}
                                       replacer:
                                         - //{0};
                                       fileName: OtherTestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }

    [Test]
    public Task ItProducesSourceCodeWithMultipleReplacers()
    {
        // Arrange
        var sources = new[]
        {
            TestInterface,
            TestClass1,
        };

        /*language=yaml*/
        const string configuration = """
                                     - interfaceType: Test.ITest
                                       template: |-
                                         namespace Test
                                         {{
                                           {0}
                                           {1}
                                         }}
                                       replacer:
                                         - // first {0};
                                         - // second {0};
                                       fileName: TestSource
                                     """;

        // Act
        var actual = Act(configuration, sources);

        // Assert
        return Verifier
            .Verify(actual)
            .UseDirectory(SnapShotDirectory);
    }


    private GeneratorDriver Act(string configuration, params string[]? sources)
    {
        var additionalTexts = new[] {new TestAdditionalFile("./Extengerator.settings.yaml", configuration)};
        return Act(additionalTexts, sources);
    }

    private GeneratorDriver Act(IEnumerable<AdditionalText>? additionalTexts, params string[]? sources)
    {
        var syntaxTrees = sources?.Select(s => CSharpSyntaxTree.ParseText(s));

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: syntaxTrees,
            references: _references);

        var generators = new[] {_generator}.Select(GeneratorExtensions.AsSourceGenerator);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generators, additionalTexts);

        return driver.RunGenerators(compilation);
    }
}