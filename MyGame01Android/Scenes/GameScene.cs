using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MyGameLib01;
using MyGameLib01.Graphics;
using MyGameLib01.Input;
using MyGameLib01.Scenes;
using SnakeGameLib;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using Gum.DataTypes;
using Gum.Wireframe;
using Gum.Managers;
using MyGame01Android.UI;

namespace MyGame01Android.Scenes;

public class GameScene : Scene
{// Defines the slime animated sprite.
    private AnimatedSprite _slime;

    // Defines the bat animated sprite.
    private AnimatedSprite _bat;
    // Defines the tilemap to draw.
    public Tilemap _tilemap;
    // Defines the bounds of the room that the slime and bat are contained within.
    public Rectangle _roomBounds;
    private SoundEffect _bounceSoundEffect;
    private SoundEffect _collectSoundEffect;

    // The SpriteFont Description used to draw text.
    private SpriteFont _font;
    // Defines the position to draw the score text at.
    private Vector2 _scoreTextPosition;

    // Defines the origin used when drawing the score text.
    private Vector2 _scoreTextOrigin;
    private SnakeGame _gameLogic = null!;
    private MoveCommand _currentMoveCommand = MoveCommand.None;

    // private Rectangle _backButtonBounds;
    // private const string BACK_TEXT = "Back";
    // private Vector2 _backTextPos;
    private AnimatedButton _pauseButton;
    private Panel _pausePanel;
    // A reference to the resume button UI element so we can focus it
    // when the game is paused.
    private AnimatedButton _resumeButton;
    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;
    private SoundEffect _uiSoundEffect;
    public override void Initialize()
    {
        base.Initialize();

        InitializeUI();
    }
    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePauseButton();
        CreatePausePanel();
    }
    public override void LoadContent()
    {
        base.LoadContent();

        // Create the texture atlas from the XML configuration file
        _atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        // Create the slime animated sprite from the atlas.
        _slime = _atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);
        _slime.CenterOrigin();
        // Create the bat animated sprite from the atlas.
        _bat = _atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);
        _bat.CenterOrigin();

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        // Load the bounce sound effect
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the font for the standard text.
        _font = Content.Load<SpriteFont>("fonts/04B_30");

        _uiSoundEffect = Content.Load<SoundEffect>("audio/ui");

        // Vector2 size = _font.MeasureString(BACK_TEXT);
        // // 左上角原点，故需要减去高度的一半，才能居中显示
        // _backTextPos = new Vector2(_roomBounds.Right - size.X, _tilemap.TileHeight * 0.5f - size.Y * 0.5f);

        // _backButtonBounds = new Rectangle(
        //     (int)_backTextPos.X,
        //     (int)_backTextPos.Y,
        //     (int)size.X,
        //     (int)size.Y
        // );

        _gameLogic = new SnakeGame(Game1.DesignWidth, Game1.DesignHeight);

        _roomBounds = new Rectangle(
             (int)_tilemap.TileWidth,
             (int)_tilemap.TileHeight,
             Game1.DesignWidth - (int)_tilemap.TileWidth * 2,
             Game1.DesignHeight - (int)_tilemap.TileHeight * 2
         );

        // Set the position of the score text to align to the left edge of the
        // room bounds, and to vertically be at the center of the first tile.
        _scoreTextPosition = new Vector2(_roomBounds.Left, _tilemap.TileHeight * 0.5f);

        // Set the origin of the text so it is left-centered.
        float scoreTextYOrigin = _font.MeasureString("Score").Y * 0.5f;
        _scoreTextOrigin = new Vector2(0, scoreTextYOrigin);

        _gameLogic.RoomBounds = _roomBounds;
        _gameLogic.Tilemap = _tilemap;

        _gameLogic.SlimeWidth = _slime.Width;
        _gameLogic.SlimeHeight = _slime.Height;
        _gameLogic.BatWidth = _bat.Width;
        _gameLogic.BatHeight = _bat.Height;

        _currentMoveCommand = MoveCommand.None;

        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;

        Vector2 pos = new Vector2(
            centerColumn * _tilemap.TileWidth + _tilemap.TileWidth * 0.5f,
            centerRow * _tilemap.TileHeight + _tilemap.TileHeight * 0.5f
        );

        _gameLogic.Reset(pos);
    }
    public override void Update(GameTime gameTime)
    {
        GumService.Default.Update(gameTime);

        if (_pausePanel.IsVisible)
        {
            return;
        }

        // Update the slime animated sprite.
        _slime.Update(gameTime);

        // Update the bat animated sprite.
        _bat.Update(gameTime);

        MoveCommand newCommand = ReadTouchMoveCommand();
        if (newCommand != MoveCommand.None && !IsOpposite(_currentMoveCommand, newCommand))
        {
            _currentMoveCommand = newCommand;
        }

        _gameLogic.Update(gameTime, _currentMoveCommand);

        if (_gameLogic.DidCollectThisFrame)
        {
            Core.Audio.PlaySoundEffect(_collectSoundEffect);
        }

        if (_gameLogic.DidBounceThisFrame)
        {
            Core.Audio.PlaySoundEffect(_bounceSoundEffect);
        }
    }

    private MoveCommand ReadTouchMoveCommand()
    {
        switch (Core.Input.Touch.Swipe)
        {
            case SwipeDirection.Up:
                return MoveCommand.Up;

            case SwipeDirection.Down:
                return MoveCommand.Down;

            case SwipeDirection.Left:
                return MoveCommand.Left;

            case SwipeDirection.Right:
                return MoveCommand.Right;

            default:
                return MoveCommand.None;
        }
    }
    private static bool IsOpposite(MoveCommand a, MoveCommand b)
    {
        return (a == MoveCommand.Up && b == MoveCommand.Down) ||
               (a == MoveCommand.Down && b == MoveCommand.Up) ||
               (a == MoveCommand.Left && b == MoveCommand.Right) ||
               (a == MoveCommand.Right && b == MoveCommand.Left);
    }
    public override void Draw(GameTime gameTime)
    {
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: Core.ScaleMatrix);
        // Draw the tilemap.
        _tilemap.Draw(Core.SpriteBatch);

        _slime.Draw(Core.SpriteBatch, _gameLogic.SlimePosition);

        _bat.Draw(Core.SpriteBatch, _gameLogic.BatPosition);

        // Draw the score
        Core.SpriteBatch.DrawString(
            _font,              // spriteFont
            $"Score: {_gameLogic.Score}", // text
            _scoreTextPosition, // position
            Color.White,        // color
            0.0f,               // rotation
            _scoreTextOrigin,   // origin
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layerDepth
        );

        // Core.SpriteBatch.DrawString(
        //     _font,
        //     BACK_TEXT,
        //     _backTextPos,
        //     Color.White
        // );
        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();

        GumService.Default.Draw();
    }

    private void CreatePauseButton()
    {
        _pauseButton = new AnimatedButton(_atlas);
        _pauseButton.Text = "Pause";
        _pauseButton.Anchor(Anchor.TopRight);
        _pauseButton.X = -10;
        _pauseButton.Y = 10;
        _pauseButton.Width = 40;
        // 注意屏幕边缘的触摸安全区域是触摸不到的，所以最好偏移一点并且把按钮设置得大一点
        // _pauseButton.Height = 20;
        _pauseButton.Click += HandlePauseButtonClicked;
        _pauseButton.AddToRoot();
    }

    private void HandlePauseButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        PauseGame();
    }
    private void CreatePausePanel()
    {
        _pausePanel = new Panel();
        _pausePanel.Anchor(Anchor.Center);
        _pausePanel.WidthUnits = DimensionUnitType.Absolute;
        _pausePanel.HeightUnits = DimensionUnitType.Absolute;
        _pausePanel.Height = 70;
        _pausePanel.Width = 264;
        _pausePanel.IsVisible = false;
        _pausePanel.AddToRoot();

        TextureRegion backgroundRegion = _atlas.GetRegion("panel-background");

        NineSliceRuntime background = new NineSliceRuntime();
        background.Dock(Dock.Fill);
        background.Texture = backgroundRegion.Texture;
        background.TextureAddress = TextureAddress.Custom;
        background.TextureHeight = backgroundRegion.Height;
        background.TextureLeft = backgroundRegion.SourceRectangle.Left;
        background.TextureTop = backgroundRegion.SourceRectangle.Top;
        background.TextureWidth = backgroundRegion.Width;
        _pausePanel.AddChild(background);

        var textInstance = new TextRuntime();
        textInstance.Text = "PAUSED";
        textInstance.CustomFontFile = @"fonts/04b_30.fnt";
        textInstance.UseCustomFont = true;
        textInstance.FontScale = 0.5f;
        textInstance.X = 10f;
        textInstance.Y = 10f;
        _pausePanel.AddChild(textInstance);

        _resumeButton = new AnimatedButton(_atlas);
        _resumeButton.Text = "RESUME";
        _resumeButton.Anchor(Anchor.BottomLeft);
        _resumeButton.X = 9f;
        _resumeButton.Y = -9f;
        _resumeButton.Width = 80;
        _resumeButton.Click += HandleResumeButtonClicked;
        _pausePanel.AddChild(_resumeButton);

        AnimatedButton quitButton = new AnimatedButton(_atlas);
        quitButton.Text = "QUIT";
        quitButton.Anchor(Anchor.BottomRight);
        quitButton.X = -9f;
        quitButton.Y = -9f;
        quitButton.Width = 80;
        quitButton.Click += HandleQuitButtonClicked;
        _pausePanel.AddChild(quitButton);
    }
    private void PauseGame()
    {
        _pausePanel.IsVisible = true;
    }

    private void HandleResumeButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        _pausePanel.IsVisible = false;
    }

    private void HandleQuitButtonClicked(object sender, EventArgs e)
    {
        Core.Audio.PlaySoundEffect(_uiSoundEffect);
        Core.ChangeScene(new TitleScene());
    }
}
