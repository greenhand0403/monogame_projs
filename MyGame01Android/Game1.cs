using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
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
        _slime.Scale = new Vector2(8.0f, 8.0f);
        _slime.CenterOrigin();
        // Create the bat animated sprite from the atlas.
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(8.0f, 8.0f);
        _bat.CenterOrigin();

        _gameLogic = new SnakeGame(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        // _gameLogic.EatDistance = (int)(_slime.Width + _bat.Width) / 2;

        _gameLogic.SlimeWidth = _slime.Width;
        _gameLogic.SlimeHeight = _slime.Height;
        _gameLogic.BatWidth = _bat.Width;
        _gameLogic.BatHeight = _bat.Height;

        _gameLogic.Reset();
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

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _slime.Draw(SpriteBatch, _gameLogic.SlimePosition);
        _bat.Draw(SpriteBatch, _gameLogic.BatPosition);

        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
