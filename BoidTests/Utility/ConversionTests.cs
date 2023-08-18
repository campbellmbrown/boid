
using Boid.Utility;
using Microsoft.Xna.Framework;
using NUnit.Framework;

namespace BoidTests.Utility;

public class ConversionTests
{
    [SetUp]
    public void Setup()
    {
    }

    static object[] ConvertPointToVector2TestCases =
    {
        new object[] { new Point(0, 0), new Vector2(0f, 0f) },
        new object[] { new Point(123, 456), new Vector2(123f, 456f) },
    };

    [TestCaseSource(nameof(ConvertPointToVector2TestCases))]
    public void ConvertPointToVector2(Point point, Vector2 expectedVector2)
    {
        Assert.That(Conversion.PointToVector2(point), Is.EqualTo(expectedVector2));
    }
}
