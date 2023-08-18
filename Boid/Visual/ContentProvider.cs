using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.BitmapFonts;

namespace Boid.Visual;

public enum FontId
{
    Normal,
}

public interface IContentProvider
{
    BitmapFont GetFont(FontId id);
}

public class ContentProvider : IContentProvider
{
    readonly Dictionary<FontId, BitmapFont> _fonts;

    public ContentProvider(ContentManager content)
    {
        _fonts = new Dictionary<FontId, BitmapFont>()
        {
            { FontId.Normal, content.Load<BitmapFont>("fonts/normal_font") },
        };
        AssertAllFontsLoaded();
    }

    public BitmapFont GetFont(FontId id) => _fonts[id];

    void AssertAllFontsLoaded()
    {
        foreach (FontId id in Enum.GetValues(typeof(FontId)))
        {
            if (!_fonts.ContainsKey(id))
            {
                throw new InvalidOperationException($"The font is not loaded from the ContentManager for {id}");
            }
        }
    }
}
