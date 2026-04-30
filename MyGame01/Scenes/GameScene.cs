using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MyGameLib01;
using MyGameLib01.Input;
using MyGameLib01.Scenes;

namespace MyGame01.Scenes;

public class GameScene : Scene
{
    public override void Initialize()
    {
        base.Initialize();

        // During the game scene, we want to disable exit on escape. Instead,
        // the escape key will be used to return back to the title screen
        Core.ExitOnEscape = false;
        
        ((Game1)Core.Instance).StartNewGame();
    }
    public override void LoadContent()
    {
        base.LoadContent();

    }
    public override void Update(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Escape))
        {
            Core.ChangeScene(new TitleScene());
            return;
        }

        ((Game1)Core.Instance).UpdateGameWorld(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        ((Game1)Core.Instance).DrawGameWorld(gameTime);
    }
}
