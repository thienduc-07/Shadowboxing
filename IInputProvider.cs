using UnityEngine;
using System.Collections.Generic;

// Tính Trừu tượng (Abstraction): Giao diện chung cho mọi loại input
public interface IInputProvider
{
    Direction? GetInput(List<Direction> lockedMoves);
}

// Tính Đa hình (Polymorphism): Người chơi 1 dùng WASD
public class WASDInput : IInputProvider
{
    public Direction? GetInput(List<Direction> lockedMoves)
    {
        if (Input.GetKeyDown(KeyCode.W) && !lockedMoves.Contains(Direction.Up)) return Direction.Up;
        if (Input.GetKeyDown(KeyCode.S) && !lockedMoves.Contains(Direction.Down)) return Direction.Down;
        if (Input.GetKeyDown(KeyCode.A) && !lockedMoves.Contains(Direction.Left)) return Direction.Left;
        if (Input.GetKeyDown(KeyCode.D) && !lockedMoves.Contains(Direction.Right)) return Direction.Right;
        return null;
    }
}

// Tính Đa hình: Người chơi 2 dùng phím mũi tên
public class ArrowKeyInput : IInputProvider
{
    public Direction? GetInput(List<Direction> lockedMoves)
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && !lockedMoves.Contains(Direction.Up)) return Direction.Up;
        if (Input.GetKeyDown(KeyCode.DownArrow) && !lockedMoves.Contains(Direction.Down)) return Direction.Down;
        if (Input.GetKeyDown(KeyCode.LeftArrow) && !lockedMoves.Contains(Direction.Left)) return Direction.Left;
        if (Input.GetKeyDown(KeyCode.RightArrow) && !lockedMoves.Contains(Direction.Right)) return Direction.Right;
        return null;
    }
}

// Tính Đa hình: Trí tuệ nhân tạo (BOT)
public class AIInput : IInputProvider
{
    public Direction? GetInput(List<Direction> lockedMoves)
    {
        List<Direction> validMoves = new List<Direction> { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        foreach (Direction lockedMove in lockedMoves)
        {
            validMoves.Remove(lockedMove);
        }
        int randomIndex = Random.Range(0, validMoves.Count);
        return validMoves[randomIndex];
    }
}