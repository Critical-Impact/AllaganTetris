using System.Collections.Generic;
using System.Numerics;
using AllaganTetris.Tetris;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace AllaganTetris.Addon;

public sealed class ControlsNode : ResNode {
    private readonly TextNode headerNode;
    private readonly SimpleNineGridNode dividingLineNode;
    private PieceType currentPieceType = PieceType.None;
    private TextNode textNode;

    public ControlsNode(int width) {

        headerNode = new TextNode()
        {
            LineSpacing = 12,
            AlignmentType = AlignmentType.Left,
            FontSize = 12,
            TextFlags = TextFlags.Edge,
            FontType = FontType.Axis,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled |
                        NodeFlags.EmitsEvents,
            TextColor = new Vector4(0.933f, 0.882f, 0.773f, 1.0f),
            TextOutlineColor = new Vector4(0,0,0, 1.0f),
            BackgroundColor = Vector4.Zero,
            Size = new Vector2(46.0f, 20.0f),
            Position = new Vector2(0, 0),
            SeString = "Controls:"
        };

        headerNode.AttachNode(this);

        dividingLineNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/WindowA_Line.tex",
            TextureCoordinates = Vector2.Zero,
            TextureSize = new Vector2(32.0f, 4.0f),
            Size = new Vector2(width, 4.0f),
            IsVisible = true,
            LeftOffset = 12.0f,
            RightOffset = 12.0f,
            Position = new Vector2(0, 18),
        };

        dividingLineNode.AttachNode(this);

        var instructions = new List<string>(){"Left Arrow: Move Left", "Right Arrow: Move Right", "Down Arrow: Drop Block",
            "Up Arrow: Smash Block", "Z: Rotate Block", "X: Rotate Block"};

        var yPos = 24;

        textNode = new TextNode()
        {
            LineSpacing = 12,
            AlignmentType = AlignmentType.Left,
            FontSize = 12,
            TextFlags = TextFlags.MultiLine,
            FontType = FontType.Axis,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled |
                        NodeFlags.EmitsEvents,
            TextColor = new Vector4(0.933f, 0.882f, 0.773f, 1.0f),
            TextOutlineColor = new Vector4(0,0,0, 1.0f),
            BackgroundColor = Vector4.Zero,
            Size = new Vector2(46.0f, 80.0f),
            Position = new Vector2(0, yPos),
        };

        textNode.SeString = string.Join("\n", instructions);
        textNode.AttachNode(this);

        Height = headerNode.Height + dividingLineNode.Height;
    }
}
