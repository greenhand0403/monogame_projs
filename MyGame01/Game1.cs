using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using MyGameLib01;
using MyGameLib01.Graphics;
using MyGameLib01.Input;
using MyGameLib01.Audio;
using SnakeGameLib;
using Microsoft.Xna.Framework.Audio;

namespace MyGame01;
// MyGame01         -> Windows 可运行项目
public class Game1 : Core
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime;
    // Defines the bat animated sprite.
    private AnimatedSprite _bat;
    private SnakeGame _gameLogic = null!;
    private MoveCommand _currentMoveCommand = MoveCommand.None;
    public const int DesignWidth = 1280;
    public const int DesignHeight = 720;
    // Defines the tilemap to draw.
    private Tilemap _tilemap;
    
    // The background theme song
    private Song _themeSong;
    private SoundEffect _bounceSoundEffect;
    private SoundEffect _collectSoundEffect;
    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;
    public Game1() : base("MyGame01 windows", DesignWidth, DesignHeight, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void LoadContent()
    {
        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);
        _slime.CenterOrigin();

        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);
        _bat.CenterOrigin();

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        Rectangle screenBounds = GraphicsDevice.PresentationParameters.Bounds;

        _roomBounds = new Rectangle(
             (int)_tilemap.TileWidth,
             (int)_tilemap.TileHeight,
             screenBounds.Width - (int)_tilemap.TileWidth * 2,
             screenBounds.Height - (int)_tilemap.TileHeight * 2
         );

        // Load the bounce sound effect
        _bounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect
        _collectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the background theme music
        _themeSong = Content.Load<Song>("audio/theme");

        _gameLogic = new SnakeGame(DesignWidth, DesignHeight);
        // Start playing the background music.
        Audio.PlaySong(_themeSong);

        _gameLogic.RoomBounds = _roomBounds;
        _gameLogic.Tilemap = _tilemap;
        
        _gameLogic.SlimeWidth = _slime.Width;
        _gameLogic.SlimeHeight = _slime.Height;
        _gameLogic.BatWidth = _bat.Width;
        _gameLogic.BatHeight = _bat.Height;

        // Initial slime position will be the center tile of the tile map.
        int centerRow = _tilemap.Rows / 2;
        int centerColumn = _tilemap.Columns / 2;
        Vector2 pos = new Vector2(centerColumn * _tilemap.TileWidth + _tilemap.TileWidth * 0.5f, centerRow * _tilemap.TileHeight + _tilemap.TileHeight * 0.5f);

        _gameLogic.Reset(pos);
    }

    protected override void Update(GameTime gameTime)
    {
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
            Audio.PlaySoundEffect(_collectSoundEffect);
        }

        if (_gameLogic.DidBounceThisFrame)
        {
            Audio.PlaySoundEffect(_bounceSoundEffect);
        }
        base.Update(gameTime);
    }

    private MoveCommand ReadKeyboardMoveCommand()
    {
        GamePadInfo gamePadOne = Input.GamePads[(int)PlayerIndex.One];

        // If the M key is pressed, toggle mute state for audio.
        if (Input.Keyboard.WasKeyJustPressed(Keys.M))
        {
            Audio.ToggleMute();
        }

        if (Input.Keyboard.WasKeyJustPressed(Keys.OemPlus) ||
    Input.Keyboard.WasKeyJustPressed(Keys.Add))
        {
            Audio.SongVolume += 0.1f;
            Audio.SoundEffectVolume += 0.1f;
        }

        if (Input.Keyboard.WasKeyJustPressed(Keys.OemMinus) ||
            Input.Keyboard.WasKeyJustPressed(Keys.Subtract))
        {
            Audio.SongVolume -= 0.1f;
            Audio.SoundEffectVolume -= 0.1f;
        }

        if (Input.Keyboard.IsKeyDown(Keys.W) || Input.Keyboard.IsKeyDown(Keys.Up)|| gamePadOne.IsButtonDown(Buttons.DPadUp))
            return MoveCommand.Up;

        if (Input.Keyboard.IsKeyDown(Keys.S) || Input.Keyboard.IsKeyDown(Keys.Down)|| gamePadOne.IsButtonDown(Buttons.DPadDown))
            return MoveCommand.Down;

        if (Input.Keyboard.IsKeyDown(Keys.A) || Input.Keyboard.IsKeyDown(Keys.Left)|| gamePadOne.IsButtonDown(Buttons.DPadLeft))
            return MoveCommand.Left;

        if (Input.Keyboard.IsKeyDown(Keys.D) || Input.Keyboard.IsKeyDown(Keys.Right)|| gamePadOne.IsButtonDown(Buttons.DPadRight))
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
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        // Draw the tilemap.
        _tilemap.Draw(SpriteBatch);

        _slime.Draw(SpriteBatch, _gameLogic.SlimePosition);
        
        _bat.Draw(SpriteBatch, _gameLogic.BatPosition);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
    
}
