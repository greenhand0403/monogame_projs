using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MyGameLib01;
using MyGameLib01.Graphics;

namespace MyGame01Android;
// 需要创建安卓项目模板 `dotnet new mgandroid -o MyGame01Android` 并引用游戏类库Lib01
// 构建安卓程序命令行 SDK `dotnet build -c Release` 输出目录在MyGame01Android\bin\Release\net9.0-android,注意此时并未生成程序安装包
// 安装安卓SDK https://learn.microsoft.com/en-us/dotnet/android/getting-started/installation/dependencies
// 在vs里面安装openjdk单个组件 https://learn.microsoft.com/en-us/previous-versions/xamarin/android/get-started/installation/openjdk
// 命令行安装依赖项 dotnet build -t:InstallAndroidDependencies -f net9.0-android "-p:AndroidSdkDirectory=D:\zwc\app\android-sdk" "-p:AcceptAndroidSDKLicenses=true"
// 注意mgcb管理器要将xml文件设置成copy,图片是build
// 注意要先clean清理项目，再打开mgcb，清理缓存，然后再继续mgcb里面build一下，`dotnet build -c Release -f net9.0-android -p:AndroidBuildApplicationPackage=true`此时才是生成安装包
public class Game1 : Core
{
    // Defines the slime animated sprite.
    private AnimatedSprite _slime;

    // Defines the bat animated sprite.
    private AnimatedSprite _bat;
    // Tracks the position of the slime.
    private Vector2 _slimePosition;

    // Speed multiplier when moving.
    private const float MOVEMENT_SPEED = 5.0f;
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
        _slime.Scale = new Vector2(4.0f, 4.0f);

        // Create the bat animated sprite from the atlas.
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update the slime animated sprite.
        _slime.Update(gameTime);

        // Update the bat animated sprite.
        _bat.Update(gameTime);

        // Check for touch input and handle it.
        CheckTouchPanelInput();

        base.Update(gameTime);
    }

    private void CheckTouchPanelInput()
    {
        while (TouchPanel.IsGestureAvailable)
        {
            float speed = MOVEMENT_SPEED;

            GestureSample gesture = TouchPanel.ReadGesture();

            if (gesture.GestureType == GestureType.HorizontalDrag)
            {
                if (gesture.Delta.X > 0)
                {
                    // 向右滑
                    _slimePosition.X += speed;
                }
                else if (gesture.Delta.X < 0)
                {
                    // 向左滑
                    _slimePosition.X -= speed;
                }
            }
            else if (gesture.GestureType == GestureType.VerticalDrag)
            {
                if (gesture.Delta.Y > 0)
                {
                    // 向下滑
                    _slimePosition.Y += speed;
                }
                else if (gesture.Delta.Y < 0)
                {
                    // 向上滑
                    _slimePosition.Y -= speed;
                }
            }
        }
    }
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime sprite.
        _slime.Draw(SpriteBatch, _slimePosition);
        // Draw the bat sprite 10px to the right of the slime.
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));
        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }

}
