using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MyGameLib01;
using MyGameLib01.Graphics;
using SnakeGameLib;

namespace MyGame01Android;
// 需要创建安卓项目模板
// `dotnet new mgandroid -o MyGame01Android` 并引用游戏类库 Lib01
// 构建安卓程序命令行 SDK 
// `dotnet build -c Release` 
// 输出目录在 MyGame01Android\bin\Release\net9.0-android 注意此时并未生成程序安装包
// 安装安卓SDK https://learn.microsoft.com/en-us/dotnet/android/getting-started/installation/dependencies
// 在vs里面安装openjdk单个组件 https://learn.microsoft.com/en-us/previous-versions/xamarin/android/get-started/installation/openjdk
// 命令行安装依赖项
// dotnet build -t:InstallAndroidDependencies -f net9.0-android "-p:AndroidSdkDirectory=D:\zwc\app\android-sdk" "-p:AcceptAndroidSDKLicenses=true"
// 注意mgcb管理器要将xml文件设置成copy,图片是build
// 注意要先打开mgcb，清理缓存，然后再继续mgcb里面build一下，
// `dotnet build -c Release -f net9.0-android -p:AndroidBuildApplicationPackage=true`
// 此时才是生成安装包
public class Game1 : Core
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime;

    // Defines the bat animated sprite.
    private AnimatedSprite _bat;
    private SnakeGame _gameLogic = null!;

    private MoveCommand _currentMoveCommand = MoveCommand.Right;
    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 200f;
    public Game1() : base("MyGame01Android Android", 1280, 720, false)
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

        MoveCommand newCommand = ReadTouchMoveCommand();
        if (newCommand != MoveCommand.None && !IsOpposite(_currentMoveCommand, newCommand))
        {
            _currentMoveCommand = newCommand;
        }

        _gameLogic.Update(gameTime, _currentMoveCommand, MOVEMENT_SPEED);

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
