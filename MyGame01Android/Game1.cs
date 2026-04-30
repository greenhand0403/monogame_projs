using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using MyGameLib01;
using MyGameLib01.Graphics;
using SnakeGameLib;
using MyGame01Android.Scenes;
using MyGameLib01.Input;
using MyGameLib01.Scenes;

namespace MyGame01Android;
public class Game1 : Core
{
    // The background theme song
    private Song _themeSong;

    private Scene _currentScene;
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

        ScaleMatrix = GetScaleMatrix();
        InverseScaleMatrix = Matrix.Invert(ScaleMatrix);

        _currentScene = new TitleScene();
        ChangeScene(_currentScene);
    }

    protected override void LoadContent()
    {
        // Load the background theme music
        _themeSong = Content.Load<Song>("audio/theme");
        // Start playing the background music.
        Audio.PlaySong(_themeSong);
    }
    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
    public Matrix GetScaleMatrix()
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
