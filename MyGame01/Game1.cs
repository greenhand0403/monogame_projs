using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGameLib01;
using MyGameLib01.Graphics;
using MyGameLib01.Input;
using SnakeGameLib;

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

        _gameLogic = new SnakeGame(DesignWidth, DesignHeight);
        // _gameLogic.EatDistance = (int)(_slime.Width + _bat.Width) / 2;

        _gameLogic.SlimeWidth = _slime.Width;
        _gameLogic.SlimeHeight = _slime.Height;
        _gameLogic.BatWidth = _bat.Width;
        _gameLogic.BatHeight = _bat.Height;

        _gameLogic.Reset();
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

        base.Update(gameTime);
    }

    private MoveCommand ReadKeyboardMoveCommand()
    {
        GamePadInfo gamePadOne = Input.GamePads[(int)PlayerIndex.One];
        
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

        _slime.Draw(SpriteBatch, _gameLogic.SlimePosition);
        
        _bat.Draw(SpriteBatch, _gameLogic.BatPosition);

        SpriteBatch.End();

        base.Draw(gameTime);
    }
    
}
