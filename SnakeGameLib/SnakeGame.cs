using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SnakeGameLib;

public class SnakeGame
{
    private readonly Random _random = new();

    public Vector2 SlimePosition { get; private set; }
    public Vector2 BatPosition { get; private set; }

    public int Score { get; private set; }

    public int WorldWidth { get; }
    public int WorldHeight { get; }

    // 史莱姆与蝙蝠的碰撞半径，可按贴图尺寸调整
    public int EatDistance { get; set; } = 20;
    // 边界控制
    public float SlimeHalfWidth { get; set; } = 10f;
    public float SlimeHalfHeight { get; set; } = 10f;
    public float BatHalfWidth { get; set; } = 10f;
    public float BatHalfHeight { get; set; } = 10f;
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
    }

    public void Update(GameTime gameTime, MoveCommand command, float moveSpeed)
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

        SlimePosition += move * moveSpeed * dt;

        // 限制在屏幕范围内
        // SlimePosition = new Vector2(
        //     MathHelper.Clamp(SlimePosition.X, 0, WorldWidth - SlimeHalfWidth),
        //     MathHelper.Clamp(SlimePosition.Y, 0, WorldHeight - SlimeHalfHeight)
        // );
        SlimePosition = new Vector2(
            MathHelper.Clamp(SlimePosition.X, SlimeHalfWidth, WorldWidth - SlimeHalfWidth),
            MathHelper.Clamp(SlimePosition.Y, SlimeHalfHeight, WorldHeight - SlimeHalfHeight)
        );
        // 吃到蝙蝠
        if (Vector2.Distance(SlimePosition, BatPosition) <= EatDistance)
        {
            Score++;
            RespawnBat();
        }
    }

    private void RespawnBat()
    {
        // BatPosition = new Vector2(
        //     _random.Next((int)BatHalfWidth, (int)(WorldWidth - BatHalfWidth)),
        //     _random.Next((int)BatHalfHeight, (int)(WorldHeight - BatHalfHeight))
        // );
        // 方便测试，让蝙蝠只在四个角落生成
        int rand = _random.Next(4);
        switch (rand)
        {
            case 0:
                BatPosition = new Vector2(BatHalfWidth, BatHalfHeight);
                break;
            case 1:
                BatPosition = new Vector2(WorldWidth - BatHalfWidth, BatHalfHeight);
                break;
            case 2:
                BatPosition = new Vector2(BatHalfWidth, WorldHeight - BatHalfHeight);
                break;
            case 3:
                BatPosition = new Vector2(WorldWidth - BatHalfWidth, WorldHeight - BatHalfHeight);
                break;
        }
    }
}
