using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MyGameLib01;
using MyGameLib01.Graphics;

namespace MyGame01;
// use dotnet new mgdesktopgl to create a new game project
// check https://docs.monogame.net/articles/tutorials/building_2d_games/08_the_sprite_class/index.html
public class Game1 : Core
{
    private Texture2D _logo;
    // Defines the slime sprite.
    private Sprite _slime;

    // Defines the bat sprite.
    private Sprite _bat;
    public Game1() : base("MyGame01 hello", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _logo = Content.Load<Texture2D>("images/logo");

        // TODO: use this.Content to load your game content here
        // Load the atlas texture using the content manager
        Texture2D atlasTexture = Content.Load<Texture2D>("images/atlas");

        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, "images/atlas-definition.xml");

        // Create the slime sprite from the atlas.
        _slime = atlas.CreateSprite("slime");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        // Create the bat sprite from the atlas.
        _bat = atlas.CreateSprite("bat");
        _bat.Scale = new Vector2(4.0f, 4.0f);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // The bounds of the icon within the texture.
        Rectangle iconSourceRect = new Rectangle(0, 0, 128, 128);

        // The bounds of the word mark within the texture.
        Rectangle wordmarkSourceRect = new Rectangle(150, 34, 458, 58);

        // TODO: Add your drawing code here
        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack);

        // Draw the logo texture
        // SpriteBatch.Draw(
        //     _logo,
        //     new Vector2(Window.ClientBounds.Width - _logo.Width, Window.ClientBounds.Height - _logo.Height) * 0.5f,
        //     Color.White
        // );

        // SpriteBatch.Draw(
        //     _logo,              // texture
        //     new Vector2(        // position
        //         Window.ClientBounds.Width,
        //         Window.ClientBounds.Height
        //     ) * 0.5f,
        //     null,               // sourceRectangle
        //     Color.White * 0.5f,        // color
        //     MathHelper.ToRadians(0),   // rotation
        //     new Vector2(                // origin
        //         _logo.Width,
        //         _logo.Height
        //     ) * 0.5f,
        //     new Vector2(1.5f, 0.5f),    // scale
        //     SpriteEffects.FlipHorizontally |    // effects
        //     SpriteEffects.FlipVertically,
        //     0.0f                // layerDepth
        // );

        // Draw only the icon portion of the texture.
        SpriteBatch.Draw(
            _logo,              // texture
            new Vector2(        // position
                Window.ClientBounds.Width,
                Window.ClientBounds.Height) * 0.5f,
            iconSourceRect,     // sourceRectangle
            Color.White,        // color
            0.0f,               // rotation
            new Vector2(        // origin
                iconSourceRect.Width,
                iconSourceRect.Height) * 0.5f,
            1.0f,               // scale
            SpriteEffects.None, // effects
            1.0f                // layerDepth
        );

        // Draw only the word mark portion of the texture.
        SpriteBatch.Draw(
            _logo,              // texture
            new Vector2(        // position
              Window.ClientBounds.Width,
              Window.ClientBounds.Height) * 0.5f,
            wordmarkSourceRect, // sourceRectangle
            Color.White,        // color
            0.0f,               // rotation
            new Vector2(        // origin
              wordmarkSourceRect.Width,
              wordmarkSourceRect.Height) * 0.5f,
            1.0f,               // scale
            SpriteEffects.None, // effects
            0.0f                // layerDepth
        );

        // Always end the sprite batch when finished.
        SpriteBatch.End();

        // Begin the sprite batch to prepare for rendering.
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime sprite.
        _slime.Draw(SpriteBatch, Vector2.Zero);

        // Draw the bat sprite 10px to the right of the slime.
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));
        // Always end the sprite batch when finished.
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
