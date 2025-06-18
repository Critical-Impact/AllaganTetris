using System.Numerics;
using KamiToolKit.Nodes;

namespace AllaganTetris.Addon;

public class InsetBackgroundNode : SimpleNineGridNode {

    public InsetBackgroundNode()
    {
        TexturePath = "ui/uld/BgParts.tex";
        TextureCoordinates = new Vector2(33,37);
        TextureSize = new Vector2(28,28);
        LeftOffset = 8;
        RightOffset = 8;
        TopOffset = 8;
        BottomOffset = 8;
        IsVisible = true;
    }
}
