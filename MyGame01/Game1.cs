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
using MyGame01.Scenes;
using MyGameLib01.Scenes;

namespace MyGame01;
// MyGame01         -> Windows 可运行项目
public class Game1 : Core
{
    // The background theme song
    private Song _themeSong;
    private Scene _currentScene;
    public const int DesignWidth = 1280;
    public const int DesignHeight = 720;
    public Game1() : base("MyGame01 windows", DesignWidth, DesignHeight, false)
    {

    }
    protected override void Initialize()
    {
        base.Initialize();
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
}
