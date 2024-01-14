using NUnit.Framework;

namespace Extengerator.Tests;

[TestFixture]
public class ExtengeratorTests 
{
    [Test]
    public void ItHandlesNoAdditionalFileSet()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesAdditionalFileNotFound()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesEmptyAdditionalFile()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesNoInterfacesFound()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesEmptyConfigurationFile()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItIgnoresAbstractClasses()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesClassesThatHasAGrandParentDerivingFromTheInterface()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesConfigurationDeserializationError()
    {
        //TODO AK:here we could have multiple errors (i.e. overall structure or a single element)
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesMissingValuesInConfiguration()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesWrong_StringFormat_OfTemplate()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItIgnoresFilesNotDerivedFromInterface()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesWrong_AppendFormat_OfReplacer()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesMultipleConfigurationEntries()
    {
        Assert.Fail();
    }
    
    [Test]
    public void ItHandlesMultipleReplacers()
    {
        Assert.Fail();
    }
}