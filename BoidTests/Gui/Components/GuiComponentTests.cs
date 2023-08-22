using System;
using Boid;
using Boid.Gui;
using Boid.Gui.Components;
using Boid.Visual;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BoidTests.Gui.Components;

/// <summary>
/// GuiItem is abstract so create a simple implementation of it so we can unit test it.
/// </summary>
class GuiComponentImplementation : GuiComponent
{
    public GuiComponentImplementation(HorizontalAlignment horizontalAlignment, int width, int height)
        : base(horizontalAlignment)
    {
        Width = width;
        Height = height;
    }

    public Vector2 GetOrigin() => Origin;
}

public class GuiItemTests
{
    Mock<ILayerView> _layerViewMock;
    GuiComponentImplementation _guiComponent;

    [SetUp]
    public void Setup()
    {
        _layerViewMock = new Mock<ILayerView>();
        _guiComponent = new(HorizontalAlignment.Left, 100, 40);
    }

    [Test]
    public void FinalizeComponent_AlreadyFinalized_ThrowsException()
    {
        // Given:
        _guiComponent.FinalizeComponent(100, 40);

        // When/then:
        Assert.Throws<InvalidOperationException>(() => _guiComponent.FinalizeComponent(100, 40));
    }

    [Test]
    public void FrameTick_NotFinalized_ThrowsException()
    {
        // Given not finalized:

        // When/then:
        Assert.Throws<InvalidOperationException>(
            () => _guiComponent.FrameTick(Mock.Of<IFrameTickManager>())
        );
    }

    [Test]
    public void FrameTick_Finalized_DoesNotThrowsException()
    {
        // Given:
        _guiComponent.FinalizeComponent(100, 40);

        // When/then:
        Assert.DoesNotThrow(
            () => _guiComponent.FrameTick(Mock.Of<IFrameTickManager>())
        );
    }

    [Test]
    public void Draw_NotFinalized_ThrowsException()
    {
        // Given not finalized:

        // When/then:
        Assert.Throws<InvalidOperationException>(
            () => _guiComponent.Draw(Mock.Of<ISpriteBatchWrapper>())
        );
    }

    [Test]
    public void Draw_Finalized_DoesNotThrowsException()
    {
        // Given:
        _guiComponent.FinalizeComponent(100, 40);

        // When/then:
        Assert.DoesNotThrow(
            () => _guiComponent.Draw(Mock.Of<ISpriteBatchWrapper>())
        );
    }

    [Test]
    public void UpdatePosition_UpdatesPosition()
    {
        // Given:
        Vector2 newPosition = new(123, 456);

        // When:
        _guiComponent.UpdatePosition(newPosition);

        // Then:
        Assert.That(_guiComponent.Position, Is.EqualTo(newPosition));
    }

    [TestCase(HorizontalAlignment.Left, 0, 0)]
    [TestCase(HorizontalAlignment.Center, 300, 0)]
    [TestCase(HorizontalAlignment.Right, 600, 0)]
    public void OffsetCalculatedBasedOnHorizontalAlignment(
        HorizontalAlignment horizontalAlignment,
        int expectedOriginX,
        int expectedOriginY)
    {
        // Given:
        GuiComponentImplementation guiComponent = new(horizontalAlignment, 400, 100);

        // When:
        guiComponent.FinalizeComponent(1000, 500);

        // Then:
        Assert.That(guiComponent.GetOrigin(), Is.EqualTo(new Vector2(expectedOriginX, expectedOriginY)));
    }

    [Test]
    public void InvalidGuiPlacement_ThrowsException()
    {
        // Given:
        GuiComponentImplementation guiComponent = new((HorizontalAlignment)99, 400, 110);

        // When/then:
        Assert.Throws<ArgumentException>(
            () => guiComponent.FinalizeComponent(1000, 500)
        );
    }
}
