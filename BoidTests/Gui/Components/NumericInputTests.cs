using System.Numerics;
using Boid;
using Boid.Gui.Components;
using Boid.Visual;
using Microsoft.Xna.Framework.Input;
using Moq;
using NUnit.Framework;

namespace BoidTests.Gui.Components;

public class NumericInputTests
{
    Mock<ITextDisplay> _textMock;
    NumericInput _numericInput;

    [SetUp]
    public void Setup()
    {
        _textMock = new Mock<ITextDisplay>();
        // Whenever the Text is set, it will return the new value on it's next get
        _textMock
            .SetupSet(text => text.Text = It.IsAny<string>())
            .Callback<string>(newText => _textMock.Setup(text => text.Text).Returns(newText));
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 123.456f);
    }

    [Test]
    public void NumericInputDimensions_UsesWidthAndTextHeight()
    {
        // Given:
        _textMock.Setup(text => text.Height).Returns(20);

        // When:
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 123.456f);

        // Then:
        Assert.Multiple(() =>
        {
            Assert.That(_numericInput.Width, Is.EqualTo(100));
            Assert.That(_numericInput.Height, Is.EqualTo(30));
        });
    }

    [Test]
    public void TextAndValue_MatchDefaultValue()
    {
        // Given object created in Setup()

        // Then:
        Assert.Multiple(() =>
        {
            Assert.That(_numericInput.Value, Is.EqualTo(123.456f));
            Assert.That(_textMock.Object.Text, Is.EqualTo("123.456"));
        });
    }

    [Test]
    public void CursorIndex_DefaultsToLastCharacterPosition()
    {
        // Given object created in Setup()

        // Then:
        Assert.That(_numericInput.CursorIndex, Is.EqualTo(_textMock.Object.Text.Length));
    }

    [Test]
    public void LeftClickAction_FocusesAndCursorVisible()
    {
        // Given:
        Assert.Multiple(() =>
        {
            Assert.That(_numericInput.Focused, Is.False);
            Assert.That(_numericInput.CursorVisible, Is.False);
        });

        // When:
        _numericInput.LeftClickAction();

        // Then:
        Assert.Multiple(() =>
        {
            Assert.That(_numericInput.Focused, Is.True);
            Assert.That(_numericInput.CursorVisible, Is.True);
        });
    }

    [Test]
    public void MoveLeft_CursorIndex_Changes()
    {
        // Given object created in Setup()

        // When:
        _numericInput.KeyPressed(Keys.Left);
        _numericInput.KeyPressed(Keys.Left);
        _numericInput.KeyPressed(Keys.Left);

        // Then:
        Assert.That(_numericInput.CursorIndex, Is.EqualTo(_textMock.Object.Text.Length - 3));
    }

    [Test]
    public void MoveRight_CursorIndex_Changes()
    {
        // Given object created in Setup() and:
        _numericInput.KeyPressed(Keys.Left);
        _numericInput.KeyPressed(Keys.Left);
        _numericInput.KeyPressed(Keys.Left);

        // When:
        _numericInput.KeyPressed(Keys.Right);
        _numericInput.KeyPressed(Keys.Right);

        // Then:
        Assert.That(_numericInput.CursorIndex, Is.EqualTo(_textMock.Object.Text.Length - 3 + 2));
    }

    [Test]
    public void MoveLeftTooFar_CursorIndexStaysAt0()
    {
        // Given object created in Setup()

        // When:
        for (int idx = 0; idx < _textMock.Object.Text.Length + 10; idx++)
        {
            _numericInput.KeyPressed(Keys.Left);
        }

        // Then:
        Assert.That(_numericInput.CursorIndex, Is.EqualTo(0));
    }

    [Test]
    public void MoveRightTooFar_CursorIndexStaysAtMax()
    {
        // Given object created in Setup()

        // When:
        for (int idx = 0; idx < 10; idx++)
        {
            _numericInput.KeyPressed(Keys.Right);
        }

        // Then:
        Assert.That(_numericInput.CursorIndex, Is.EqualTo(_textMock.Object.Text.Length));
    }

    [TestCase(Keys.D0, "123045")]
    [TestCase(Keys.D1, "123145")]
    [TestCase(Keys.D2, "123245")]
    [TestCase(Keys.D3, "123345")]
    [TestCase(Keys.D4, "123445")]
    [TestCase(Keys.D5, "123545")]
    [TestCase(Keys.D6, "123645")]
    [TestCase(Keys.D7, "123745")]
    [TestCase(Keys.D8, "123845")]
    [TestCase(Keys.D9, "123945")]
    [TestCase(Keys.Back, "1245")]
    [TestCase(Keys.Delete, "1235")]
    [TestCase(Keys.OemPeriod, "123.45")]
    public void Key_ModifiesText(Keys key, string expectedText)
    {
        // Given a default value:
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 12345);
        // and our cursor is after the '3' and before the '4'
        _numericInput.KeyPressed(Keys.Left);
        _numericInput.KeyPressed(Keys.Left);

        // When:
        _numericInput.KeyPressed(key);

        // Then:
        Assert.That(_textMock.Object.Text, Is.EqualTo(expectedText));
    }

    [Test]
    public void Key_OnlyOnePeriodAdded()
    {
        // Given a default value:
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 12345);
        // and our cursor is after the '3' and before the '4'
        _numericInput.KeyPressed(Keys.Left);
        _numericInput.KeyPressed(Keys.Left);

        // When:
        _numericInput.KeyPressed(Keys.OemPeriod);
        _numericInput.KeyPressed(Keys.OemPeriod);
        _numericInput.KeyPressed(Keys.OemPeriod);

        // Then:
        Assert.That(_textMock.Object.Text, Is.EqualTo("123.45"));
    }

    [TestCase(Keys.Escape)]
    [TestCase(Keys.Enter)]
    public void Key_Unfocus(Keys key)
    {
        // Given:
        _numericInput.Focused = true;

        // When:
        _numericInput.KeyPressed(key);

        // Then:
        Assert.That(_numericInput.Focused, Is.False);
    }

    [Test]
    public void Key_Enter_UpdatesValue()
    {
        // Given a default value:
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 12345);
        // and some additions:
        _numericInput.KeyPressed(Keys.D6);
        _numericInput.KeyPressed(Keys.D7);
        _numericInput.KeyPressed(Keys.D8);

        // When:
        _numericInput.KeyPressed(Keys.Enter);

        // Then:
        Assert.That(_numericInput.Value, Is.EqualTo(12345678));
    }

    [Test]
    public void Key_Escape_DoesNotUpdateValue()
    {
        // Given a default value:
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 12345);
        // and some additions:
        _numericInput.KeyPressed(Keys.D6);
        _numericInput.KeyPressed(Keys.D7);
        _numericInput.KeyPressed(Keys.D8);

        // When:
        _numericInput.KeyPressed(Keys.Escape);

        // Then:
        Assert.That(_numericInput.Value, Is.EqualTo(12345));
    }

    [Test]
    public void LoseFocus_UpdatesValue()
    {
        // Given a default value:
        _numericInput = new NumericInput(_textMock.Object, 100, 5, 12345);
        // and some additions:
        _numericInput.KeyPressed(Keys.D6);
        _numericInput.KeyPressed(Keys.D7);
        _numericInput.KeyPressed(Keys.D8);

        // When:
        _numericInput.Focused = false;

        // Then:
        Assert.That(_numericInput.Value, Is.EqualTo(12345678));
    }

    [Test]
    public void FrameTick_Focused_ChangesCursorVisibility()
    {
        // Given:
        _numericInput.FinalizeComponent(1000, 500);
        _numericInput.Focused = true;
        Mock<IFrameTickManager> frameTickManagerMock = new();
        frameTickManagerMock
            .SetupSequence(frameTickManager => frameTickManager.TimeDiffSec)
            .Returns(0.1f) // Not enough to toggle
            .Returns(0.45f) // Enough to toggle
            .Returns(0.1f) // Not enough to toggle
            .Returns(0.45f) // Enough to toggle
            .Returns(0.1f) // Not enough to toggle
            .Returns(0.45f); // Enough to toggle

        // When/Then:
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.True);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.True);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.True);
    }

    [Test]
    public void FrameTick_Unfocused_CursorAlwaysInvisible()
    {
        // Given:
        _numericInput.FinalizeComponent(1000, 500);
        _numericInput.Focused = false;
        Mock<IFrameTickManager> frameTickManagerMock = new();
        frameTickManagerMock
            .Setup(frameTickManager => frameTickManager.TimeDiffSec)
            .Returns(0.6f); // Enough to toggle

        // When/Then:
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
        _numericInput.FrameTick(frameTickManagerMock.Object);
        Assert.That(_numericInput.CursorVisible, Is.False);
    }

    [Test]
    public void UpdatePosition_UpdatesTextPosition()
    {
        // Given:
        Vector2 newPosition = new(123, 456);
        Vector2 expectedTextPosition = newPosition + new Vector2(5);

        // When:
        _numericInput.UpdatePosition(newPosition);

        // Then:
        _textMock.Verify(text => text.UpdatePosition(expectedTextPosition));
    }
}
