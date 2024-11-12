using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public static Vector2Int[,] rotateIminoClockWiseDelta = new Vector2Int[4, 4] {
        { new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) },
        { new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) },
        { new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) },
        { new Vector2Int(-2, 0), new Vector2Int(1, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) }};

    public static Vector2Int[,] rotateIminoAnticlockWiseDelta = new Vector2Int[4, 4] {
        { new Vector2Int(2, 0), new Vector2Int(-1, 0), new Vector2Int(2, 1), new Vector2Int(-1, -2) },
        { new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(1, -2), new Vector2Int(-2, 1) },
        { new Vector2Int(1, 0), new Vector2Int(-2, 0), new Vector2Int(-2, -1), new Vector2Int(1, 2) },
        { new Vector2Int(-1, 0), new Vector2Int(2, 0), new Vector2Int(-1, 2), new Vector2Int(2, -1) }};

    public static Vector2Int[,] rotatenonIminoClockWiseDelta = new Vector2Int[4, 4] {
        { new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },
        { new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) },
        { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },
        { new Vector2Int(-2, 0), new Vector2Int(-2, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }};

    public static Vector2Int[,] rotatenonIminoAnticlockWiseDelta = new Vector2Int[4, 4] {
        { new Vector2Int(1, 0), new Vector2Int(1, 1), new Vector2Int(0, -2), new Vector2Int(1, -2) },
        { new Vector2Int(1, 0), new Vector2Int(1, -1), new Vector2Int(0, 2), new Vector2Int(1, 2) },
        { new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -2), new Vector2Int(-1, -2) },
        { new Vector2Int(-1, 0), new Vector2Int(-1, -1), new Vector2Int(0, 2), new Vector2Int(-1, 2) }};
}
