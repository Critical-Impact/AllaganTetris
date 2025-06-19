using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Numerics;
using AllaganTetris.Tetris;
using Dalamud.Game.Addon.Events;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit;
using KamiToolKit.Addon;
using KamiToolKit.Nodes;

namespace AllaganTetris.Addon;

public class TetrisAddon : NativeAddon
{
    private readonly IFramework framework;
    private readonly IKeyState keyState;
    private readonly Game.Factory gameFactory;
    private NineGridNode nineGridNode;
    private TextButtonNode testButton;

    private const float UnitSize = 65.0f;
    private const float FramePadding = 8.0f;
    private const float UnitPadding = 10.0f;
    private readonly IDalamudTextureWrap texture;
    private readonly ISharedImmediateTexture sharedImmediateTexture;

    public Game? Game;
    private int timerCounter;
    private readonly int timerStep = 10;
    private int currentScore = -1;
    private DateTime? lastMoveTime;
    private bool focused;

    private TextNode scoreNode;
    private TextButtonNode startGameButton;
    private Game.GameStatus currentStatus = Game.GameStatus.ReadyToStart;
    private TextButtonNode restartGameButton;
    private ControlsNode controlsNode;
    private NextBlockNode nextBlockNode;
    private TetrisGridNode tetrisGrid;
    private DateTime? lastGameTime;
    private int gameSpeed;

    [SetsRequiredMembers]
    public TetrisAddon(ITextureProvider textureProvider, IDalamudPluginInterface pluginInterface, IFramework framework, NativeController nativeController, IKeyState keyState, Game.Factory gameFactory)
    {
        InternalName = "Allagan Tetris";
        Title = "Allagan Tetris";
        Size = new(520,740);
        NativeController = nativeController;
        this.framework = framework;
        this.keyState = keyState;
        this.gameFactory = gameFactory;
        gameSpeed = 800;
        var assemblyLocation = pluginInterface.AssemblyLocation.DirectoryName!;
        var imagePath = Path.Combine(assemblyLocation,"Images", "block.png");
        sharedImmediateTexture = textureProvider.GetFromFile(imagePath);
        texture = sharedImmediateTexture.RentAsync().Result;
        this.framework.Update += FrameworkOnUpdate;
    }

    private void FrameworkOnUpdate(IFramework framework1)
    {
        if (focused)
        {
            if (lastMoveTime == null || lastMoveTime.Value.AddMilliseconds(100) < DateTime.Now)
            {
                if (Game != null && Game.Status == Game.GameStatus.InProgress)
                {
                    if (isKeyPressed(new[] { VirtualKey.Z }))
                    {
                        Game.RotateLeft();
                        lastMoveTime = DateTime.Now;
                    }

                    if (isKeyPressed(new[] { VirtualKey.X }))
                    {
                        Game.RotateRight();
                        lastMoveTime = DateTime.Now;
                    }

                    if (isKeyPressed(new[] { VirtualKey.UP }))
                    {
                        Game.SmashDown();
                        lastMoveTime = DateTime.Now;
                    }

                    if (isKeyPressed(new[] { VirtualKey.DOWN }))
                    {
                        Game.MoveDown();
                        lastMoveTime = DateTime.Now;
                    }

                    if (isKeyPressed(new[] { VirtualKey.LEFT }))
                    {
                        Game.MoveLeft();
                        lastMoveTime = DateTime.Now;
                    }

                    if (isKeyPressed(new[] { VirtualKey.RIGHT }))
                    {
                        Game.MoveRight();
                        lastMoveTime = DateTime.Now;
                    }
                }

                keyState.ClearAll();
            }
            else
            {
                keyState.ClearAll();
            }
            if (lastGameTime == null || DateTime.Now >= lastGameTime.Value + TimeSpan.FromMilliseconds(gameSpeed))
            {
                lastGameTime = DateTime.Now;
                RunGame();
            }
        }
    }

    public void CreateGame(bool start = false)
    {
        Game = gameFactory.Invoke();
        gameSpeed = 800;
        timerCounter = 0;
        if (start)
        {
            Game.Start();
        }
    }

    private void RunGame()
    {
        if (Game == null)
        {
            return;
        }

        if (Game.Status != Game.GameStatus.Finished)
        {
            if (Game.Status != Game.GameStatus.Paused)
            {
                timerCounter += timerStep;
                Game.MoveDown();
                if (Game.Status != Game.GameStatus.Finished)
                {
                    if ( timerCounter >= ( 1000 - (Game.Lines * 10) ) )
                    {
                        gameSpeed -= 50;
                        timerCounter = 0;
                    }
                }
            }
        }
    }

    private unsafe bool IsAddonFocused(AtkUnitBase* addon)
    {
        var units = AtkStage.Instance()->RaptureAtkUnitManager->AtkUnitManager.FocusedUnitsList;
        var count = units.Count;
        foreach (var unit in units.Entries)
        {
            if (unit == addon)
            {
                return true;
            }
        }

        return false;
    }

    private bool isKeyPressed(VirtualKey[] keys) {
        foreach (var vk in keyState.GetValidVirtualKeys()) {
            if (keys.Contains(vk)) {
                if (!keyState[vk]) return false;
            } else {
                if (keyState[vk]) return false;
            }
        }
        return true;
    }

    protected override unsafe void OnFinalize(AtkUnitBase* addon)
    {
        focused = false;
    }

    protected override unsafe void OnUpdate(AtkUnitBase* addon)
    {
        var focusedNode = AtkStage.Instance()->GetFocus();

        if (IsAddonFocused(addon))
        {
            focused = true;
        }
        else
        {
            focused = false;
        }
    }

    protected override unsafe void OnDraw(AtkUnitBase* addon)
    {
        if (Game == null)
        {
            return;
        }

        tetrisGrid.Draw(Game);

        if (Game.Score != currentScore)
        {
            currentScore = Game.Score;
            scoreNode.Text = "Current Score: " + currentScore;
        }

        if (Game.Status != currentStatus)
        {
            currentStatus = Game.Status;
            switch (currentStatus)
            {
                case Game.GameStatus.ReadyToStart:
                     startGameButton.Label = "Start";
                     break;
                case Game.GameStatus.InProgress:
                    startGameButton.Label = "Stop";
                    break;
                case Game.GameStatus.Paused:
                    startGameButton.Label = "Resume";
                    break;
                case Game.GameStatus.Finished:
                    startGameButton.Label = "Play Again";
                    break;
            }
        }

        if (Game.NextPiece != null)
        {
            nextBlockNode.SetActiveBlockType(Game.NextPiece.PieceType);
        }
    }

    protected override unsafe void OnSetup(AtkUnitBase* addon)
    {
        var xPos = FramePadding;
        var yPos = UnitSize - FramePadding;

        tetrisGrid = new TetrisGridNode(NativeController, texture)
        {
            Position = new Vector2(xPos, yPos),
            IsVisible = true,
        };

        NativeController.AttachNode(tetrisGrid, this);

        yPos = UnitSize - FramePadding;
        xPos += tetrisGrid.Width + FramePadding;

        startGameButton = new TextButtonNode()
        {
            Position = new Vector2(xPos, yPos),
            Size = new Vector2(80, 32),
            IsVisible = true,
            Label = "Start",
        };

        startGameButton.AddEvent(AddonEventType.ButtonClick, StartButtonClicked);

        yPos += startGameButton.Height + UnitPadding;

        NativeController.AttachNode(startGameButton, this);

        restartGameButton = new TextButtonNode()
        {
            Position = new Vector2(xPos, yPos),
            Size = new Vector2(80, 32),
            IsVisible = true,
            Label = "Restart",
        };

        restartGameButton.AddEvent(AddonEventType.ButtonClick, RestartButtonClicked);

        NativeController.AttachNode(restartGameButton, this);

        yPos += restartGameButton.Height + UnitPadding;

        scoreNode = new TextNode()
        {
            NodeId = 4,
            LineSpacing = 12,
            AlignmentType = AlignmentType.Left,
            FontSize = 14,
            TextFlags = TextFlags.Edge,
            FontType = FontType.Axis,
            NodeFlags = NodeFlags.AnchorTop | NodeFlags.AnchorLeft | NodeFlags.Visible | NodeFlags.Enabled |
                        NodeFlags.EmitsEvents,
            TextColor = Vector4.One,
            TextOutlineColor = new Vector4(0.941f,0.557f,0.216f,1.0f),
            BackgroundColor = Vector4.Zero,
            Size = new Vector2(46.0f, 20.0f),
            Position = new Vector2(xPos, yPos),
        };

        NativeController.AttachNode(scoreNode, this);

        yPos += scoreNode.Height + UnitPadding;

        nextBlockNode = new NextBlockNode(NativeController, texture, 130, 130)
        {
            Position = new Vector2(xPos, yPos),
            IsVisible = true,
        };

        NativeController.AttachNode(nextBlockNode, this);

        yPos += nextBlockNode.Height + UnitPadding;

        controlsNode = new ControlsNode(NativeController, 130)
        {
            Position = new Vector2(xPos, yPos),
            IsVisible = true
        };

        NativeController.AttachNode(controlsNode, this);

        CreateGame();
    }

    private void StartButtonClicked(AddonEventData addonEventData)
    {
        if (Game != null)
        {
            switch (Game.Status)
            {
                case Game.GameStatus.ReadyToStart:
                    Game.Start();
                    break;
                case Game.GameStatus.InProgress:
                case Game.GameStatus.Paused:
                    Game.Pause();
                    break;
                case Game.GameStatus.Finished:
                    CreateGame(true);
                    break;
            }
        }
    }

    private void RestartButtonClicked(AddonEventData addonEventData)
    {
        if (Game != null)
        {
            CreateGame();
        }
    }

    private bool isDisposed;
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!isDisposed)
        {
            this.framework.Update -= FrameworkOnUpdate;
            texture.Dispose();
            isDisposed = true;
        }
    }
}
