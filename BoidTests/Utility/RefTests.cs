using Boid.Utility;
using NUnit.Framework;

namespace BoidTests.Utility;

public class RefTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void FloatType()
    {
        // When:
        Ref<float> refInstance = new();

        // Then:
        Assert.That(refInstance.Value, Is.InstanceOf<float>());
    }

    [Test]
    public void IntType()
    {
        // When:
        Ref<int> refInstance = new();

        // Then:
        Assert.That(refInstance.Value, Is.InstanceOf<int>());
    }

    [Test]
    public void UintType()
    {
        // When:
        Ref<uint> refInstance = new();

        // Then:
        Assert.That(refInstance.Value, Is.InstanceOf<uint>());
    }

    [Test]
    public void DoubleType()
    {
        // When:
        Ref<double> refInstance = new();

        // Then:
        Assert.That(refInstance.Value, Is.InstanceOf<double>());
    }

    [Test]
    public void CompareMoreLessThan_RefOnLeft()
    {
        // Given:
        Ref<double> left = new(123);
        double right = 456;

        // When:
        bool leftMoreThanRight = left > right;
        bool leftLessThanRight = left < right;

        // Then:
        Assert.Multiple(() =>
        {
            Assert.That(leftMoreThanRight, Is.False);
            Assert.That(leftLessThanRight, Is.True);
        });
    }

    [Test]
    public void CompareMoreLessThan_RefOnRight()
    {
        // Given:
        double left = 123;
        Ref<double> right = new(456);

        // When:
        bool leftMoreThanRight = left > right;
        bool leftLessThanRight = left < right;

        // Then:
        Assert.Multiple(() =>
        {
            Assert.That(leftMoreThanRight, Is.False);
            Assert.That(leftLessThanRight, Is.True);
        });
    }
}
