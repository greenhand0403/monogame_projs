using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGameLib01;
using MyGameLib01.Graphics;
using MyGameLib01.Input;
using MyGameLib01.Scenes;
using SnakeGameLib;
using Gum.DataTypes;
using Gum.Wireframe;
using MonoGameGum;
using Gum.Forms.Controls;
using MonoGameGum.GueDeriving;
using Gum.Managers;
using MyGame01.UI;

namespace MyGame01.Scenes;

public class GameScene : Scene
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime;
    // Defines the bat animated sprite.
    private AnimatedSprite _bat;
    // Defines the tilemap to draw.
    private Tilemap _tilemap;
    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;
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
    // A reference to the pause panel UI element so we can set its visibility
    // when the game is paused.
    private Panel _pausePanel;

    // A reference to the resume button UI element so we can focus it
    // when the game is paused.
    private AnimatedButton _resumeButton;
    // Reference to the texture atlas that we can pass to UI elements when they
    // are created.
    private TextureAtlas _atlas;
    // The UI sound effect to play when a UI event is triggered.
    private SoundEffect _uiSoundEffect;

    public override void Initialize()
    {
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;

        InitializeUI();
    }
    private void InitializeUI()
    {
        GumService.Default.Root.Children.Clear();

        CreatePausePanel();
    }

    public override void LoadContent()
    {

        // Create the texture atlas from the XML configuration file
        _atlas = TextureAtlas.FromFile(Core.Content, "images/atlas-definition.xml");

        _slime = _atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);
        _slime.CenterOrigin();

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

        // Load the font
        _font = Content.Load<SpriteFont>("fonts/04B_30");

        // Load the sound effect to play when ui actions occur.
        _uiSoundEffect = Core.Content.Load<SoundEffect>("audio/ui");

        // 持有游戏逻辑对象
        _gameLogic = new SnakeGame(Game1.DesignWidth, Game1.DesignHeight);

        // 分数文本绘制
        // Rectangle screenBounds = Core.GraphicsDevice.PresentationParameters.Bounds;

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
        // Ensure the UI is always updated
        GumService.Default.Update(gameTime);

        // If the game is paused, do not continue
        if (_pausePanel.IsVisible)
        {
            return;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            PauseGame();
            return;
        }

        // Update the slime animated sprite.
        _slime.Update(gameTime);

        // Update the bat animated sprite.
        _bat.Update(gameTime);

        MoveCommand newCommand = ReadKeyboardMoveCommand();
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

    private MoveCommand ReadKeyboardMoveCommand()
    {
        GamePadInfo gamePadOne = Core.Input.GamePads[(int)PlayerIndex.One];

        // If the M key is pressed, toggle mute state for audio.
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.M))
        {
            Core.Audio.ToggleMute();
        }
        // If the start button is pressed, pause the game
        if (gamePadOne.WasButtonJustPressed(Buttons.Start))
        {
            PauseGame();
            return MoveCommand.None;
        }
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.OemPlus) ||
    Core.Input.Keyboard.WasKeyJustPressed(Keys.Add))
        {
            Core.Audio.SongVolume += 0.1f;
            Core.Audio.SoundEffectVolume += 0.1f;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.OemMinus) ||
            Core.Input.Keyboard.WasKeyJustPressed(Keys.Subtract))
        {
            Core.Audio.SongVolume -= 0.1f;
            Core.Audio.SoundEffectVolume -= 0.1f;
        }

        if (Core.Input.Keyboard.IsKeyDown(Keys.W) || Core.Input.Keyboard.IsKeyDown(Keys.Up) || gamePadOne.IsButtonDown(Buttons.DPadUp))
            return MoveCommand.Up;

        if (Core.Input.Keyboard.IsKeyDown(Keys.S) || Core.Input.Keyboard.IsKeyDown(Keys.Down) || gamePadOne.IsButtonDown(Buttons.DPadDown))
            return MoveCommand.Down;

        if (Core.Input.Keyboard.IsKeyDown(Keys.A) || Core.Input.Keyboard.IsKeyDown(Keys.Left) || gamePadOne.IsButtonDown(Buttons.DPadLeft))
            return MoveCommand.Left;

        if (Core.Input.Keyboard.IsKeyDown(Keys.D) || Core.Input.Keyboard.IsKeyDown(Keys.Right) || gamePadOne.IsButtonDown(Buttons.DPadRight))
            return MoveCommand.Right;

        return MoveCommand.None;
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
        // ((Game1)Core).DrawGameWorld(gameTime);
        Core.GraphicsDevice.Clear(Color.CornflowerBlue);

        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _tilemap.Draw(Core.SpriteBatch);
        _slime.Draw(Core.SpriteBatch, _gameLogic.SlimePosition);
        _bat.Draw(Core.SpriteBatch, _gameLogic.BatPosition);

        Core.SpriteBatch.DrawString(
            _font,
            $"Score: {_gameLogic.Score}",
            _scoreTextPosition,
            Color.White,
            0.0f,
            _scoreTextOrigin,
            1.0f,
            SpriteEffects.None,
            0.0f
        );

        Core.SpriteBatch.End();

        // Draw the Gum UI
        GumService.Default.Draw();
    }
    private void PauseGame()
    {
        // Make the pause panel UI element visible.
        _pausePanel.IsVisible = true;

        // Set the resume button to have focus
        _resumeButton.IsFocused = true;
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
        textInstance.CustomFontFile = "fonts/04b_30.fnt";
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
    private void HandleResumeButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Make the pause panel invisible to resume the game.
        _pausePanel.IsVisible = false;
    }
    private void HandleQuitButtonClicked(object sender, EventArgs e)
    {
        // A UI interaction occurred, play the sound effect
        Core.Audio.PlaySoundEffect(_uiSoundEffect);

        // Go back to the title scene.
        Core.ChangeScene(new TitleScene());
    }

}
