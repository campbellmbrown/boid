using Boid.Gui.Components;
using Boid.Visual;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BoidTests.Gui.Components;

public class LabelTests
{
    Mock<ITextDisplay> _textMock;
    Label _label;

    [SetUp]
    public void Setup()
    {
        _textMock = new Mock<ITextDisplay>();
        _label = new(_textMock.Object);
        _label.FinalizeComponent(1000, 500);
    }

    [Test]
    public void LabelDimensions_UsesTextDimensions()
    {
        // Given:
        _textMock.Setup(text => text.Width).Returns(100);
        _textMock.Setup(text => text.Height).Returns(20);

        // When:
        Label label = new(_textMock.Object);

        // Then:
        Assert.Multiple(() =>
        {
            Assert.That(label.Width, Is.EqualTo(100));
            Assert.That(label.Height, Is.EqualTo(20));
        });
    }

    [Test]
    public void UpdatePosition_UpdatesTextPosition()
    {
        // Given:
        Vector2 position = new(123, 456);

        // When:
        _label.UpdatePosition(position);

        // Then:
        _textMock.Verify(text => text.UpdatePosition(position), Times.Once());
    }

    [Test]
    public void Draw_DrawsText()
    {
        // Given:
        Mock<ISpriteBatchWrapper> spriteBatchWrapperMock = new();

        // When:
        _label.Draw(spriteBatchWrapperMock.Object);

        // Then:
        _textMock.Verify(text => text.Draw(spriteBatchWrapperMock.Object), Times.Once());
    }
}
