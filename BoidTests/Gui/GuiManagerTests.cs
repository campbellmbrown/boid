using System;
using Boid;
using Boid.Gui;
using Boid.Gui.Items;
using Boid.Visual;
using Moq;
using NUnit.Framework;

namespace BoidTests.Gui;

public class GuiManagerTests
{
    GuiManager _guiManager;

    [SetUp]
    public void Setup()
    {
        _guiManager = new GuiManager();
    }

    [Test]
    public void FinalizeGui_FinalizedAllItems()
    {
        // Given:
        Mock<IGuiItem> guiItemMock1 = new();
        Mock<IGuiItem> guiItemMock2 = new();
        _guiManager.AddItem(guiItemMock1.Object);
        _guiManager.AddItem(guiItemMock2.Object);

        // When:
        _guiManager.FinalizeGui();

        // Then:
        guiItemMock1.Verify(item => item.FinalizeItem(), Times.Once());
        guiItemMock2.Verify(item => item.FinalizeItem(), Times.Once());
    }

    [Test]
    public void FinalizeGui_AlreadyFinalized_ThrowsException()
    {
        // Given:
        _guiManager.FinalizeGui();

        // When/then:
        Assert.Throws<InvalidOperationException>(() => _guiManager.FinalizeGui());
    }

    [Test]
    public void FrameTick_RunsAllGuiItemsFrameTicks()
    {
        // Given:
        Mock<IGuiItem> guiItemMock1 = new();
        Mock<IGuiItem> guiItemMock2 = new();
        _guiManager.AddItem(guiItemMock1.Object);
        _guiManager.AddItem(guiItemMock2.Object);
        Mock<IFrameTickManager> frameTickManagerMock = new();
        _guiManager.FinalizeGui();

        // When:
        _guiManager.FrameTick(frameTickManagerMock.Object);

        // Then:
        guiItemMock1.Verify(item => item.FrameTick(frameTickManagerMock.Object), Times.Once());
        guiItemMock2.Verify(item => item.FrameTick(frameTickManagerMock.Object), Times.Once());
    }

    [Test]
    public void FrameTick_NotFinalized_ThrowsException()
    {
        // Given not finalized:

        // When/then:
        Assert.Throws<InvalidOperationException>(
            () => _guiManager.FrameTick(Mock.Of<IFrameTickManager>())
        );
    }

    [Test]
    public void Draw_DrawsAllGuiItems()
    {
        // Given:
        Mock<IGuiItem> guiItemMock1 = new();
        Mock<IGuiItem> guiItemMock2 = new();
        _guiManager.AddItem(guiItemMock1.Object);
        _guiManager.AddItem(guiItemMock2.Object);
        Mock<ISpriteBatchWrapper> spriteBatchWrapperMock = new();
        _guiManager.FinalizeGui();

        // When:
        _guiManager.Draw(spriteBatchWrapperMock.Object);

        // Then:
        guiItemMock1.Verify(item => item.Draw(spriteBatchWrapperMock.Object), Times.Once());
        guiItemMock2.Verify(item => item.Draw(spriteBatchWrapperMock.Object), Times.Once());
    }

    [Test]
    public void Draw_NotFinalized_ThrowsException()
    {
        // Given not finalized:

        // When/then:
        Assert.Throws<InvalidOperationException>(
            () => _guiManager.Draw(Mock.Of<ISpriteBatchWrapper>())
        );
    }
}
