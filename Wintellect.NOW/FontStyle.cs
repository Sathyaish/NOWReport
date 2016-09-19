using System;

namespace Wintellect.NOW
{
    [Flags]
    public enum FontStyle
    {
        Normal = 0,
        Bold = 1,
        Italic = 2,
        Oblique = Italic,
        Underline = 4,
        Strikethrough = 8,
        LineThrough = Strikethrough
    }
}