using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using MyGameLib01;
using MyGameLib01.Input;
using MyGameLib01.Scenes;

namespace MyGame01Android.Scenes;

public class GameScene : Scene
{
    private Rectangle _backButtonBounds;
    private const string BACK_TEXT = "Back";
    private Vector2 _backTextPos;
    // The font to use to render normal text.
    public SpriteFont _font;
    public override void Initialize()
    {
        base.Initialize();

        Vector2 size = _font.MeasureString(BACK_TEXT);
        // 左上角原点，故需要减去高度的一半，才能居中显示
        _backTextPos = new Vector2(((Game1)Core.Instance)._roomBounds.Right - size.X, ((Game1)Core.Instance)._tilemap.TileHeight * 0.5f - size.Y * 0.5f);

        _backButtonBounds = new Rectangle(
            (int)_backTextPos.X,
            (int)_backTextPos.Y,
            (int)size.X,
            (int)size.Y
        );

        ((Game1)Core.Instance).StartNewGame();
    }
    public override void LoadContent()
    {
        base.LoadContent();

        // Load the font for the standard text.
        _font = Core.Instance.Content.Load<SpriteFont>("fonts/04B_30");

    }
    public override void Update(GameTime gameTime)
    {
        TouchCollection touches = TouchPanel.GetState();

        foreach (TouchLocation touch in touches)
        {
            if (touch.State == TouchLocationState.Pressed)
            {
                Matrix scaleMatrix = ((Game1)Core.Instance).GetScaleMatrix();
                Matrix inverseMatrix = Matrix.Invert(scaleMatrix);

                Vector2 worldPos = Vector2.Transform(touch.Position, inverseMatrix);
                if (_backButtonBounds.Contains(worldPos))
                {
                    Core.ChangeScene(new TitleScene());
                    return;
                }
            }
        }

        ((Game1)Core.Instance).UpdateGameWorld(gameTime);
    }
    public override void Draw(GameTime gameTime)
    {
        ((Game1)Core.Instance).DrawGameWorld(gameTime);

        // Begin the sprite batch to prepare for rendering.
        Core.SpriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: ((Game1)Core.Instance).GetScaleMatrix());
        Core.SpriteBatch.DrawString(
            _font,
            BACK_TEXT,
            _backTextPos,
            Color.White
        );
        // Always end the sprite batch when finished.
        Core.SpriteBatch.End();
    }
}
