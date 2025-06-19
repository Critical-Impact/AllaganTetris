using System.Collections.Generic;
using System.Numerics;
using AllaganTetris.Tetris;
using Dalamud.Interface.Textures.TextureWraps;
using KamiToolKit;
using KamiToolKit.Nodes;

namespace AllaganTetris.Addon;

public sealed class TetrisGridNode : ResNode {
    private readonly int blockSize;
    private readonly int boardWidth;
    private readonly int boardHeight;
    private readonly NineGridNode backgroundNode;
    private TetrisBlockNode[,] board;
    private PieceType currentPieceType = PieceType.None;
    private const float UnitPadding = 10.0f;

    public Dictionary<PieceType, Vector4> BlockColors = new()
    {
        {PieceType.I, new Vector4(-255f, 71f, 75f,1)}, //Straight Line
        {PieceType.O, new Vector4(77f, 73f, -213f,1)},   //Block
        {PieceType.L, new Vector4(73f, -16f, -228f,1)},    //L
        {PieceType.J, new Vector4(-255f, -255f, 69,1)},     //Reverse L
        {PieceType.S, new Vector4(-75f, 75f, -222f, 1)},  //S block
        {PieceType.Z, new Vector4(159f, -147f, -211f,1)},   //Z block
        {PieceType.T, new Vector4(-77f, -211f, 75f,1)},   //T block
        {PieceType.None, new Vector4(1,1,1,0f)},          //Empty
        {PieceType.Shadow, new Vector4(1,1,1,0.1f)},          //Shadow Piece
    };

    private readonly IconImageNode imageNode;

    public TetrisGridNode(NativeController nativeController, IDalamudTextureWrap blockTexture, int blockSize = 32, int boardWidth = 10, int boardHeight = 20) {
        this.blockSize = blockSize;
        this.boardWidth = boardWidth;
        this.boardHeight = boardHeight;

        board = new TetrisBlockNode[10, 20];

        backgroundNode = new InsetBackgroundNode() {
            Position = new Vector2(0, 0),
            Size = new Vector2((blockSize*10) + (UnitPadding * 2), (blockSize*20) + (UnitPadding * 2)),
            IsVisible = true,
        };

        nativeController.AttachNode(backgroundNode, this);

        imageNode = new IconImageNode()
        {
            IconId = 72659,
            Position = new Vector2(0, 0),
            IsVisible = true,
        };
        imageNode.Height = imageNode.TextureHeight;
        nativeController.AttachNode(imageNode, this);

        var xPos = UnitPadding / 2.0f;
        var yPos = UnitPadding;
        var startY = yPos;


        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                board[x,y] = new TetrisBlockNode() {
                    Position = new Vector2(xPos , yPos),
                    Size = new Vector2(blockSize, blockSize),
                    IsVisible = true,
                };

                board[x,y].LoadTexture(blockTexture);
                board[x, y].Alpha = 0f;

                yPos += blockSize;
                nativeController.AttachNode(board[x,y], this);
            }
            yPos = startY;
            xPos += blockSize;
        }

        this.Width = (UnitPadding * 2) + (blockSize * boardWidth);
        this.Height = (UnitPadding * 2) + (blockSize * boardHeight);
    }

    public void Draw(Game game)
    {
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 20; y++)
            {
                var boardPiece = game.ActualBoard[y, x];
                var pieceType = (PieceType)boardPiece;
                if (board[x, y].PieceType == pieceType)
                {
                    continue;
                }
                if (BlockColors.TryGetValue(pieceType, out var color))
                {
                    board[x, y].Alpha = color.W;
                    board[x, y].AddColor = new Vector3(color.X / 255f, color.Y / 255f, color.Z / 255f);
                }
                else
                {
                    board[x, y].Alpha = 0f;
                }
                board[x, y].PieceType = pieceType;
            }
        }
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            backgroundNode.Dispose();
            imageNode.Dispose();
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    board[x,y].Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
