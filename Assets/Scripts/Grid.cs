using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Grid
{
    private List<Cell> _cells;
    private Cell[] _grid;

    public List<Cell> Cells { get { return _cells; } }
    public Transform Container { get; set; }
    public float CellSize { get; private set; }
    int _width;
    int _height;
    public int Width { get { return _width; } }
    public int Height { get { return _height; } }
    public int Rotation { get; set; }

    public Grid(int w, int h, float cellSize)
    {
        _width = w;
        _height = h;
        CellSize = cellSize;
        _cells = new List<Cell>();
        Rotation = 0;
        _grid = new Cell[w * h];
    }

    public bool StateEquals(Grid other)
    {
        for (int i = 0; i < _height; ++i)
        {
            for (int j = 0; j < _width; ++j)
            {
                int x, y;
                changeRot(j, i, out x, out y);
                int pos = y * _width + x;
                int v1 = 0;
                if (_grid[pos] != null)
                    v1 = _grid[pos].Value;

                other.changeRot(j, i, out x, out y);
                pos = y * _width + x;
                int v2 = 0;
                if (other._grid[pos] != null)
                    v2 = other._grid[pos].Value;

                if (v1 != v2)
                    return false;
            }
        }

        return true;
    }

    private void changeRot(int x, int y, out int ox, out int oy)
    {
        switch (Rotation)
        {
            case 0:
                ox = x;
                oy = y;              
                break;
            case 1:
                ox = _width - 1 - y;
                oy = x;
                break;
            case 2:
                ox = _width - 1 - x;
                oy = _height - 1 - y;
                break;
            case 3:
                ox = y;
                oy = _width - 1 - x;
                break;
            default:
                ox = x;
                oy = y;   
                break;
        }
    }

    public void AddCell(Cell cell)
    {
        _cells.Add(cell);
    }

    public void GetRow(int row, ref List<Cell> cells)
    {
        if (row < 0 || row >= _height)
            return;

      /*  for (int i = 0; i < _width; ++i)
        {
            int pos = row * _width + i;
            if (_grid[pos] != null)
                cells.Add(_grid[pos]);
        }
        */

        switch (Rotation)
        {
            case 0:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = row * _width + i;
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
            case 1:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = i * _width + (_height - row - 1);
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
            case 2:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = (_height - row - 1) * _width + i;
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
            case 3:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = i * _width + row;
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
        }
    }

    public void GetColumn(int column, ref List<Cell> cells)
    {
        if (column < 0 || column >= _width)
            return;

        switch (Rotation)
        {
            case 0:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = i * _width + column;
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
            case 1:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = column * _width + i;
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
            case 2:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = i * _width + (_width - column - 1);
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
            case 3:
                for (int i = 0; i < _height; ++i)
                {
                    int pos = (_width - column - 1) * _width + i;
                    if (_grid[pos] != null)
                        cells.Add(_grid[pos]);
                }
                break;
        }

    }

    public void Update()
    {
        for (int i = 0; i < _width * _height; ++i)
            _grid[i] = null;

        foreach (Cell c in _cells)
            _grid[c.Y * _width + c.X] = c;
        /*switch (Rotation)
        {
            case 0:
                foreach (Cell c in _cells)
                    _grid[c.Y * _width + c.X] = c;
                break;
            case 1:
                foreach (Cell c in _cells)
                    _grid[(_width - c.X - 1) * _width + c.Y] = c;
                break;
            case 2:
                foreach (Cell c in _cells)
                    _grid[(_height - c.Y - 1) * _width + (_height - c.X - 1)] = c;
                break;
            case 3:
                foreach (Cell c in _cells)
                    _grid[(c.X) * _width + (_height - c.Y - 1)] = c;
                break;
        }*/



        foreach (Cell c in _cells)
        {
            int left = 0;
            int right = 0;
            int up = 0;
            int down = 0;

            //Left
            int x = c.X;
            int y = c.Y;
            while (--x >= 0)
            {
                if (_grid[y * _width + x] == null)
                    ++left;
            }

            //Right
            x = c.X;
            y = c.Y;
            while (++x < _width)
            {
                if (_grid[y * _width + x] == null)
                    ++right;
            }

            //Up
            x = c.X;
            y = c.Y;
            while (--y >= 0)
            {
                if (_grid[y * _width + x] == null)
                    ++up;
            }

            //Down
            x = c.X;
            y = c.Y;
            while (++y < _width)
            {
                if (_grid[y * _width + x] == null)
                    ++down;
            }

            c.MoveLeftDown = new Vector2(left * CellSize, down * CellSize);
            c.MoveRightUp = new Vector2(right * CellSize, up * CellSize);
            /*switch (Rotation)
            {
                case 0:
                    c.MoveLeftDown = new Vector2(left * CellSize, down * CellSize);
                    c.MoveRightUp = new Vector2(right * CellSize, up * CellSize);
                    break;
                case 1:
                    c.MoveLeftDown = new Vector2(left * CellSize, down * CellSize);
                    c.MoveRightUp = new Vector2(right * CellSize, up * CellSize);
                    break;
                case 2:
                    c.MoveLeftDown = new Vector2(right * CellSize, up * CellSize);
                    c.MoveRightUp = new Vector2(left * CellSize, down * CellSize);
                    break;
                case 3:
                    c.MoveLeftDown = new Vector2(left * CellSize, down * CellSize);
                    c.MoveRightUp = new Vector2(right * CellSize, up * CellSize);
                    break;
            }*/
        }

    }
}
