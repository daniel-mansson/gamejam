using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Cell
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Value { get; set; }
    public Vector2 MoveRightUp { get; set; }
    public Vector2 MoveLeftDown { get; set; }
    public Transform Transform { get; set; }
    public Vector2 ResetDelta { get; set; }


    private Vector2 _startPos;

    public Cell(int x, int y, int value, Transform transform)
    {
        X = x;
        Y = y;
        Value = value;
        Transform = transform;
        ResetDelta = Vector2.zero;
    }

    public void StartMove()
    { 
        _startPos = Vec.xy(Transform.position);
    }

    public void Move(Vector2 dist)
    {
        Vector2 pos = Vec.xy(Transform.position);
        dist.x = Mathf.Clamp(dist.x, -MoveLeftDown.x, MoveRightUp.x);
        dist.y = Mathf.Clamp(dist.y, -MoveLeftDown.y, MoveRightUp.y);
        
        Transform.position = new Vector3(_startPos.x, _startPos.y, 0f) + Transform.TransformDirection(dist);
    }

    public void StopMove()
    { 
    
    }

    public void Update()
    { 
        Vector3 move = 20f * Time.deltaTime * ResetDelta;
        Transform.position = Transform.position + move;
        ResetDelta -= Vec.xy(move);
    }

    public bool StateEquals(Cell other)
    {
        return other.X == X && other.Y == Y && other.Value == Value;
    }
}

