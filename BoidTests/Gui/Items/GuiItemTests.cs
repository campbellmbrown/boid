using System;
using Boid;
using Boid.Gui;
using Boid.Gui.Items;
using Boid.Visual;
using Microsoft.Xna.Framework;
using Moq;
using NUnit.Framework;

namespace BoidTests.Gui.Items;

/// <summary>
/// GuiItem is abstract so create a simple implementation of it so we can unit test it.
/// </summary>
class GuiItemImplementation : GuiItem
{
    public GuiItemImplementation(ILayerView layerView, GuiPlacement placement, int width, int height)
        : base(layerView, placement)
    {
        Width = width;
        Height = height;
    }

    public Vector2 GetPosition() => Position;
}

public class GuiItemTests
{
    Mock<ILayerView> _layerViewMock;
    GuiItemImplementation _guiItem;

    [SetUp]
    public void Setup()
    {
        _layerViewMock = new Mock<ILayerView>();
        _guiItem = new(_layerViewMock.Object, GuiPlacement.TopLeft, 100, 40);
    }

    [Test]
    public void FinalizeItem_AlreadyFinalized_ThrowsException()
    {
        // Given:
        _guiItem.FinalizeItem();

        // When/then:
        Assert.Throws<InvalidOperationException>(() => _guiItem.FinalizeItem());
    }

    [Test]
    public void FrameTick_NotFinalized_ThrowsException()
    {
        // Given not finalized:

        // When/then:
        Assert.Throws<InvalidOperationException>(
            () => _guiItem.FrameTick(Mock.Of<IFrameTickManager>())
        );
    }

    [Test]
    public void FrameTick_Finalized_DoesNotThrowsException()
    {
        // Given:
        _guiItem.FinalizeItem();

        // When/then:
        Assert.DoesNotThrow(
            () => _guiItem.FrameTick(Mock.Of<IFrameTickManager>())
        );
    }

    [Test]
    public void Draw_NotFinalized_ThrowsException()
    {
        // Given not finalized:

        // When/then:
        Assert.Throws<InvalidOperationException>(
            () => _guiItem.Draw(Mock.Of<ISpriteBatchWrapper>())
        );
    }

    [Test]
    public void Draw_Finalized_DoesNotThrowsException()
    {
        // Given:
        _guiItem.FinalizeItem();

        // When/then:
        Assert.DoesNotThrow(
            () => _guiItem.Draw(Mock.Of<ISpriteBatchWrapper>())
        );
    }

    [TestCase(GuiPlacement.TopLeft, -200, -100)]
    [TestCase(GuiPlacement.TopMiddle, -20, -100)]
    [TestCase(GuiPlacement.TopRight, 160, -100)]
    [TestCase(GuiPlacement.CenterLeft, -200, -5)]
    [TestCase(GuiPlacement.Center, -20, -5)]
    [TestCase(GuiPlacement.CenterRight, 160, -5)]
    [TestCase(GuiPlacement.BottomLeft, -200, 90)]
    [TestCase(GuiPlacement.BottomMiddle, -20, 90)]
    [TestCase(GuiPlacement.BottomRight, 160, 90)]
    public void PositionCalculatedBasedOnGuiPlacement(
        GuiPlacement guiPlacement,
        int expectedPosX,
        int expectedPosY)
    {
        // Given:
        _layerViewMock.Setup(layerView => layerView.Origin).Returns(new Vector2(-200, -100));
        _layerViewMock.Setup(layerView => layerView.Size).Returns(new Vector2(400, 200));
        GuiItemImplementation guiItem = new(_layerViewMock.Object, guiPlacement, 40, 10);

        // When:
        var position = guiItem.GetPosition();

        // Then:
        Assert.That(position, Is.EqualTo(new Vector2(expectedPosX, expectedPosY)));
    }

    public void InvalidGuiPlacement_ThrowsException()
    {
        // Given:
        GuiItemImplementation guiItem = new(_layerViewMock.Object, (GuiPlacement)99, 40, 10);

        // When/then:
        Assert.Throws<ArgumentException>(
            () => guiItem.GetPosition()
        );
    }
}
