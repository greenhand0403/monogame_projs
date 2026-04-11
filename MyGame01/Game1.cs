using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGameLib01;
using MyGameLib01.Graphics;
using SnakeGameLib;

namespace MyGame01;
// 缓冲输入
public class Game1 : Core
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime;
    // Defines the bat animated sprite.
    private AnimatedSprite _bat;
    private SnakeGame _gameLogic = null!;
    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 200f;
    private MoveCommand _currentMoveCommand = MoveCommand.Right;
    public Game1() : base("MyGame01 hello", 1280, 720, false)
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

        _gameLogic = new SnakeGame(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        _gameLogic.EatDistance = (int)(_slime.Width + _bat.Width) / 2;

        _gameLogic.SlimeHalfWidth = _slime.Width * 0.5f;
        _gameLogic.SlimeHalfHeight = _slime.Height * 0.5f;
        _gameLogic.BatHalfWidth = _bat.Width * 0.5f;
        _gameLogic.BatHalfHeight = _bat.Height * 0.5f;

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

        MoveCommand newCommand = ReadKeyboardMoveCommand();
        if (newCommand != MoveCommand.None && !IsOpposite(_currentMoveCommand, newCommand))
        {
            _currentMoveCommand = newCommand;
        }

        _gameLogic.Update(gameTime, _currentMoveCommand, MOVEMENT_SPEED);

        base.Update(gameTime);
    }
    private MoveCommand ReadKeyboardMoveCommand()
    {
        KeyboardState keyboard = Keyboard.GetState();

        if (keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up))
            return MoveCommand.Up;

        if (keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down))
            return MoveCommand.Down;

        if (keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left))
            return MoveCommand.Left;

        if (keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right))
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

        _slime.Draw(SpriteBatch, _gameLogic.SlimePosition);
        _bat.Draw(SpriteBatch, _gameLogic.BatPosition);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
    
}
