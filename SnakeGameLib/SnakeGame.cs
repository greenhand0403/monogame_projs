using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGameLib01;

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

    // 史莱姆与蝙蝠的碰撞半径，可按贴图尺寸调整
    // public int EatDistance { get; set; } = 20;

    // 边界控制
    public float SlimeWidth { get; set; } = 20f;
    public float SlimeHeight { get; set; } = 20f;
    public float BatWidth { get; set; } = 20f;
    public float BatHeight { get; set; } = 20f;
    public SnakeGame(int worldWidth, int worldHeight)
    {
        WorldWidth = worldWidth;
        WorldHeight = worldHeight;

        Reset();
    }

    public void Reset()
    {
        SlimePosition = new Vector2(WorldWidth / 2f, WorldHeight / 2f);
        Score = 0;

        RespawnBat();

        // Assign the initial random velocity to the bat.
        AssignRandomBatDirection();
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

        // 史莱姆碰撞检测，将位置限制在屏幕范围内
        // Create a bounding rectangle for the screen.
        Rectangle screenBounds = new Rectangle(
            0,
            0,
            WorldWidth,
            WorldHeight
        );

        // Creating a bounding circle for the slime
        Circle slimeBounds = new Circle(
            (int)newSlimePosition.X,
            (int)newSlimePosition.Y,
            (int)(SlimeWidth * 0.3f)
        );
        // 史莱姆触碰边界停下来
        if (slimeBounds.Left < screenBounds.Left)
        {
            newSlimePosition.X = SlimePosition.X;
        }
        else if (slimeBounds.Right > screenBounds.Right)
        {
            newSlimePosition.X = SlimePosition.X;
        }

        if (slimeBounds.Top < screenBounds.Top)
        {
            newSlimePosition.Y = SlimePosition.Y;
        }
        else if (slimeBounds.Bottom > screenBounds.Bottom)
        {
            newSlimePosition.Y = SlimePosition.Y;
        }

        SlimePosition = newSlimePosition;

        // Calculate the new position of the bat based on the velocity.
        Vector2 newBatPosition = BatPosition + _batDirection * dt * BAT_MOVEMENT_SPEED;

        // Create a bounding circle for the bat.
        Circle batBounds = new Circle(
            (int)newBatPosition.X,
            (int)newBatPosition.Y,
            (int)(BatWidth * 0.3f)
        );

        // 蝙蝠触碰屏幕边界反射
        Vector2 normal = Vector2.Zero;

        // Use distance based checks to determine if the bat is within the
        // bounds of the game screen, and if it is outside that screen edge,
        // reflect it about the screen edge normal.
        if (batBounds.Left < screenBounds.Left)
        {
            normal.X = Vector2.UnitX.X;
            newBatPosition.X = BatPosition.X;
        }
        else if (batBounds.Right > screenBounds.Right)
        {
            normal.X = -Vector2.UnitX.X;
            newBatPosition.X = BatPosition.X;
        }

        if (batBounds.Top < screenBounds.Top)
        {
            normal.Y = Vector2.UnitY.Y;
            newBatPosition.Y = BatPosition.Y;
        }
        else if (batBounds.Bottom > screenBounds.Bottom)
        {
            normal.Y = -Vector2.UnitY.Y;
            newBatPosition.Y = BatPosition.Y;
        }

        // If the normal is anything but Vector2.Zero, this means the bat had
        // moved outside the screen edge so we should reflect it about the
        // normal.
        if (normal != Vector2.Zero)
        {
            normal.Normalize();
            _batDirection = Vector2.Reflect(_batDirection, normal);
        }

        BatPosition = newBatPosition;

        // 史莱姆吃到蝙蝠
        if (slimeBounds.Intersects(batBounds))
        {
            // System.Diagnostics.Debugger.Break();
            Score++;
            RespawnBat();
            // Assign a new random velocity to the bat
            AssignRandomBatDirection();
        }
        
        // if (Vector2.Distance(SlimePosition, BatPosition) <= EatDistance)
        // {
        //     Score++;
        //     RespawnBat();
        // }
    }

    private void RespawnBat()
    {
        BatPosition = new Vector2(
            _random.Next((int)(BatWidth * 0.5f), (int)(WorldWidth - BatWidth * 0.5f)),
            _random.Next((int)(BatHeight * 0.5f), (int)(WorldHeight - BatHeight * 0.5f))
        );
        // 方便测试，让蝙蝠只在四个角落生成
        // int rand = _random.Next(4);
        // switch (rand)
        // {
        //     case 0:
        //         BatPosition = new Vector2(BatHalfWidth, BatHalfHeight);
        //         break;
        //     case 1:
        //         BatPosition = new Vector2(WorldWidth - BatHalfWidth, BatHalfHeight);
        //         break;
        //     case 2:
        //         BatPosition = new Vector2(BatHalfWidth, WorldHeight - BatHalfHeight);
        //         break;
        //     case 3:
        //         BatPosition = new Vector2(WorldWidth - BatHalfWidth, WorldHeight - BatHalfHeight);
        //         break;
        // }

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
