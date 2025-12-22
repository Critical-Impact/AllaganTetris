using System.Collections.Generic;
using System.Numerics;
using AllaganTetris.Tetris;
using Dalamud.Interface.Textures.TextureWraps;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace AllaganTetris.Addon;

public sealed class NextBlockNode : ResNode {
    private readonly NineGridNode backgroundNode;
    private readonly TextNode headerNode;
    private readonly SimpleNineGridNode dividingLineNode;
    private ImGuiImageNode[,] pieceNodes;
    private PieceType currentPieceType = PieceType.None;

    public Dictionary<PieceType, Vector4> BlockColors = new()
    {
        {PieceType.I, new Vector4(-255f, 71f, 75f,1)}, //Straight Line
        {PieceType.O, new Vector4(77f, 73f, -213f,1)},   //Block
        {PieceType.L, new Vector4(73f, -16f, -228f,1)},    //L
        {PieceType.J, new Vector4(-255f, -255f, 69,1)},     //Reverse L
        {PieceType.S, new Vector4(-75f, 75f, -222f, 1)},  //S block
        {PieceType.Z, new Vector4(159f, -147f, -211f,1)},   //Z block
        {PieceType.T, new Vector4(-77f, -211f, 75f,1)},   //T block
        {PieceType.None, new Vector4(1,1,1,0f)},                 //Shadow Piece
    };

    public NextBlockNode(IDalamudTextureWrap blockTexture, int width, int height) {

        pieceNodes = new ImGuiImageNode[5, 5];

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
            SeString = "Next Block:"
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

        backgroundNode = new InsetBackgroundNode() {
            Position = new Vector2(0, 24),
            Size = new Vector2(width, width),
            IsVisible = true,
        };

        backgroundNode.AttachNode(this);

        var padding = 10.0f;
        var blockSize = (width - (padding * 2)) / 5.0f;
        var xPos = padding;
        var yPos = 24 + padding;

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                pieceNodes[x,y] = new ImGuiImageNode {
                    Position = new Vector2(xPos , yPos),
                    Size = new Vector2(blockSize, blockSize),
                    IsVisible = true,
                };

                pieceNodes[x,y].LoadTexture(blockTexture);
                pieceNodes[x, y].Alpha = 0f;

                yPos += blockSize;
                pieceNodes[x,y].AttachNode(this);
            }
            yPos = 24 + padding;
            xPos += blockSize;
        }

        Height = headerNode.Height + dividingLineNode.Height + backgroundNode.Height;
    }

    public void SetActiveBlockType(PieceType type)
    {
        if (type != currentPieceType)
        {
            currentPieceType = type;
            var nextPiece = PieceFactory.GetPiece(type);
            if (nextPiece != null)
            {
                if (BlockColors.TryGetValue(type, out var color))
                {
                    for (var x = 0; x < 5; x++)
                    {
                        for (int y = 0; y < 5; y++)
                        {
                            pieceNodes[x, y].Alpha = 0f;
                            pieceNodes[x, y].AddColor = new Vector3(0, 0, 0);
                        }
                    }

                    var xOffset = 0;
                    var yOffset = 0;

                    if (nextPiece.RealHeight == -1 || nextPiece.RealHeight <= 2)
                    {
                        yOffset = 1;
                    }

                    if (nextPiece.RealWidth <= 2)
                    {
                        xOffset = 1;
                    }

                    for (var x = 0; x < nextPiece.Width ; x++)
                    {
                        for (var y = 0; y < nextPiece.Height; y++)
                        {
                            if (nextPiece.RealHeight == -1)
                            {
                                y = 0;
                            }
                            if (nextPiece[y, x] == (int)type)
                            {
                                pieceNodes[x + xOffset, y + yOffset].Alpha = 1.0f;
                                pieceNodes[x + xOffset, y + yOffset].AddColor = new Vector3(color.X / 255f, color.Y / 255f, color.Z / 255f);
                            }
                        }
                    }
                }
            }
        }
    }
}
