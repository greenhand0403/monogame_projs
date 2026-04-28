using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MyGameLib01;
using MyGameLib01.Graphics;
using SnakeGameLib;

namespace MyGame01Android;
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

    // Defines the bounds of the room that the slime and bat are contained within.
    private Rectangle _roomBounds;
    public Game1() : base("MyGame01Android Android", DesignWidth, DesignHeight, false)
    {

    }

    protected override void Initialize()
    {
        base.Initialize();

        TouchPanel.EnabledGestures =
        GestureType.HorizontalDrag |
        GestureType.VerticalDrag |
        GestureType.DragComplete;
    }

    protected override void LoadContent()
    {

        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        // Create the slime animated sprite from the atlas.
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);
        _slime.CenterOrigin();
        // Create the bat animated sprite from the atlas.
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);
        _bat.CenterOrigin();

        // Create the tilemap from the XML configuration file.
        _tilemap = Tilemap.FromFile(Content, "images/tilemap-definition.xml");
        _tilemap.Scale = new Vector2(4.0f, 4.0f);

        _roomBounds = new Rectangle(
             (int)_tilemap.TileWidth,
             (int)_tilemap.TileHeight,
             DesignWidth - (int)_tilemap.TileWidth * 2,
             DesignHeight - (int)_tilemap.TileHeight * 2
         );

        _gameLogic = new SnakeGame(DesignWidth, DesignHeight);
        
        // Load the bounce sound effect
        _gameLogic.BounceSoundEffect = Content.Load<SoundEffect>("audio/bounce");

        // Load the collect sound effect
        _gameLogic.CollectSoundEffect = Content.Load<SoundEffect>("audio/collect");

        // Load the background theme music
        _gameLogic.Theme = Content.Load<Song>("audio/theme");

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
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

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

        base.Update(gameTime);
    }

    private MoveCommand ReadTouchMoveCommand()
    {
        while (TouchPanel.IsGestureAvailable)
        {
            GestureSample gesture = TouchPanel.ReadGesture();

            if (gesture.GestureType == GestureType.HorizontalDrag)
            {
                if (gesture.Delta.X > 0)
                    return MoveCommand.Right;
                if (gesture.Delta.X < 0)
                    return MoveCommand.Left;
            }
            else if (gesture.GestureType == GestureType.VerticalDrag)
            {
                if (gesture.Delta.Y > 0)
                    return MoveCommand.Down;
                if (gesture.Delta.Y < 0)
                    return MoveCommand.Up;
            }
        }

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

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: GetScaleMatrix());
        // Draw the tilemap.
        _tilemap.Draw(SpriteBatch);

        _slime.Draw(SpriteBatch, _gameLogic.SlimePosition);

        _bat.Draw(SpriteBatch, _gameLogic.BatPosition);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
    private Matrix GetScaleMatrix()
    {
        float viewportWidth = GraphicsDevice.Viewport.Width;
        float viewportHeight = GraphicsDevice.Viewport.Height;

        float scaleX = viewportWidth / DesignWidth;
        float scaleY = viewportHeight / DesignHeight;

        // 等比缩放，保证完整显示 1280x720，不裁切
        float scale = MathHelper.Min(scaleX, scaleY);

        // 居中偏移，剩余区域就是黑边/背景边
        float offsetX = (viewportWidth - DesignWidth * scale) * 0.5f;
        float offsetY = (viewportHeight - DesignHeight * scale) * 0.5f;

        return Matrix.CreateScale(scale, scale, 1f)
             * Matrix.CreateTranslation(offsetX, offsetY, 0f);
    }
}
