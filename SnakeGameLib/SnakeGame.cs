using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MyGameLib01;
using MyGameLib01.Graphics;

namespace SnakeGameLib;
// SnakeGameLib     -> 类贪吃蛇玩法逻辑
public class SnakeGame
{
    private readonly Random _random = new();

    public Vector2 SlimePosition { get; private set; }
    public Vector2 BatPosition { get; private set; }
    // Tracks the velocity of the bat.
    private Vector2 _batDirection;
    // Speed multiplier when moving.
    public const float SLIME_MOVEMENT_SPEED = 200f;
    public const float BAT_MOVEMENT_SPEED = 200f;
    public int Score { get; private set; }

    public int WorldWidth { get; }
    public int WorldHeight { get; }

    public Rectangle RoomBounds { get; set; }
    public Tilemap Tilemap { get; set; }
    public Song Theme { get; set; }
    public SoundEffect BounceSoundEffect { get; set; }
    public SoundEffect CollectSoundEffect { get; set; }
    // 边界控制
    public float SlimeWidth { get; set; } = 20f;
    public float SlimeHeight { get; set; } = 20f;
    public float BatWidth { get; set; } = 20f;
    public float BatHeight { get; set; } = 20f;

    public SnakeGame(int worldWidth, int worldHeight)
    {
        WorldWidth = worldWidth;
        WorldHeight = worldHeight;
    }

    public void Reset(Vector2 pos)
    {
        SlimePosition = pos;

        Score = 0;

        RespawnBat();

        // Ensure media player is not already playing on device, if so, stop it
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Stop();
        }

        // Play the background theme music.
        MediaPlayer.Play(Theme);

        // Set the theme music to repeat.
        MediaPlayer.IsRepeating = true;
    }

    public void Update(GameTime gameTime, MoveCommand command)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 move = Vector2.Zero;

        switch (command)
        {
            case MoveCommand.Up:
                move.Y = -1f;
                break;
            case MoveCommand.Down:
                move.Y = 1f;
                break;
            case MoveCommand.Left:
                move.X = -1f;
                break;
            case MoveCommand.Right:
                move.X = 1f;
                break;
        }

        // 史莱姆移动
        Vector2 newSlimePosition = SlimePosition + move * SLIME_MOVEMENT_SPEED * dt;

        // Creating a bounding circle for the slime
        Circle slimeBounds = new Circle(
            (int)newSlimePosition.X,
            (int)newSlimePosition.Y,
            (int)(SlimeWidth * 0.5f)
        );
        // 史莱姆触碰边界停下来
        if (slimeBounds.Left < RoomBounds.Left)
        {
            newSlimePosition.X = RoomBounds.Left + slimeBounds.Radius;
        }
        else if (slimeBounds.Right > RoomBounds.Right)
        {
            newSlimePosition.X = RoomBounds.Right - slimeBounds.Radius;
        }

        if (slimeBounds.Top < RoomBounds.Top)
        {
            newSlimePosition.Y = RoomBounds.Top + slimeBounds.Radius;
        }
        else if (slimeBounds.Bottom > RoomBounds.Bottom)
        {
            newSlimePosition.Y = RoomBounds.Bottom - slimeBounds.Radius;
        }

        SlimePosition = newSlimePosition;

        // Calculate the new position of the bat based on the velocity.
        Vector2 newBatPosition = BatPosition + _batDirection * dt * BAT_MOVEMENT_SPEED;

        // Create a bounding circle for the bat.
        Circle batBounds = new Circle(
            (int)newBatPosition.X,
            (int)newBatPosition.Y,
            (int)(BatWidth * 0.5f)
        );

        // 蝙蝠触碰屏幕边界反射
        Vector2 normal = Vector2.Zero;

        // Use distance based checks to determine if the bat is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // reflect it about the screen edge normal.
        if (batBounds.Left < RoomBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = RoomBounds.Left + batBounds.Radius;
        }
        else if (batBounds.Right > RoomBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = RoomBounds.Right - batBounds.Radius;
        }

        if (batBounds.Top < RoomBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = RoomBounds.Top + batBounds.Radius;
        }
        else if (batBounds.Bottom > RoomBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = RoomBounds.Bottom - batBounds.Radius;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _batDirection = Vector2.Reflect(_batDirection, normal);

            // Play the bounce sound effect
            BounceSoundEffect.Play();
        }

        BatPosition = newBatPosition;

        // 史莱姆吃到蝙蝠
        if (slimeBounds.Intersects(batBounds))
        {
            // Play the collect sound effect
            CollectSoundEffect.Play();
            
            // System.Diagnostics.Debugger.Break();
            Score++;
            RespawnBat();
        }
    }

    private void RespawnBat()
    {
        // Choose a random row and column based on the total number of each
        int column = Random.Shared.Next(1, Tilemap.Columns - 1);
        int row = Random.Shared.Next(1, Tilemap.Rows - 1);

        // Change the bat position by setting the x and y values equal to
        // the column and row multiplied by the width and height.
        BatPosition = new Vector2(column * BatWidth + Tilemap.TileWidth * 0.5f, row * BatHeight + Tilemap.TileHeight * 0.5f);

        // Assign a new random velocity to the bat
        AssignRandomBatDirection();
    }

    private void AssignRandomBatDirection()
    {
        // Generate a random angle.
        float angle = (float)(Random.Shared.NextDouble() * Math.PI * 2);

        // Convert angle to a direction vector.
        float x = (float)Math.Cos(angle);
        float y = (float)Math.Sin(angle);
        Vector2 direction = new Vector2(x, y);

        // Multiply the direction vector by the movement speed.
        _batDirection = direction;
    }
}
