using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContainerBehaviour : MonoBehaviour
{
    public float _inputMin;
    public float _inputMax;
    public Transform _highlightPrefab;

    enum InputState
    {
        Rotate,
        Move,
        Nothing
    };
    enum MoveState
    {
        Waiting,
        Horizontal,
        Vertical
    };

    List<Cell> _moveCells;
    MoveState _moveState;
    InputState _inputState;
    Vector2 _inputStart;
    float _inputStartAngle;
    float t = 0f;
    float _deltaRot;
    Grid _grid;
    bool _gameOver = false;
    public Grid Grid { get { return _grid; } }
    List<ContainerBehaviour> _otherContainers;

    bool _remoteMove = false;
    Vector2 _remoteOffset = Vector2.zero;

    bool _stateChangeLastMouseUp;
    public bool StateChangeOnLastMouseUp { get { return _stateChangeLastMouseUp; } }

    public void InitData(Grid grid, Transform[] allContainers)
    {
        _otherContainers = new List<ContainerBehaviour>();
        _grid = grid;
        foreach (Transform t in allContainers)
        {
            if (t != this.transform)
                _otherContainers.Add(t.GetComponent<ContainerBehaviour>());
        }
    }

    private List<Transform> _highlights;

    void Start()
    {
        _highlights = new List<Transform>();
        _deltaRot = 0f;
        _moveCells = new List<Cell>();
        _inputState = InputState.Nothing;
        _moveState = MoveState.Waiting;
        _stateChangeLastMouseUp = false;
    }

    void StartRemoteMove(Vector2 inputStart, Vector2 center, MoveState state, int rowCol)
    {
        _remoteMove = true;
        _moveState = state;
        _inputState = InputState.Move;
        _remoteOffset = Vec.xy(transform.position) - center;
        _inputStart = inputStart + _remoteOffset;

        _grid.Update();
        _moveCells.Clear();
        if (state == MoveState.Horizontal)
        {
            _grid.GetRow(rowCol, ref _moveCells);
            CreateHighlightRow(rowCol);
        }
        else if (state == MoveState.Vertical)
        {
            _grid.GetColumn(rowCol, ref _moveCells);
            CreateHighlightColumn(rowCol);
        }

        foreach (Cell c in _moveCells)
            c.StartMove();
    }

    void StopRemoteMove()
    {
        _remoteMove = false;
    }

    void Update()
    {
        if (!_gameOver)
        {
            Vector2 worldPos;

            if (!_remoteMove)
                worldPos = Vec.xy(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            else
                worldPos = Vec.xy(Camera.main.ScreenToWorldPoint(Input.mousePosition)) + _remoteOffset;

            if (!_remoteMove && Input.GetMouseButtonDown(0))
            {
                if (worldPos.x < _inputMin || worldPos.x > _inputMax)
                    _inputState = InputState.Nothing;
                else if (IsInside(worldPos))
                {
                    _inputState = InputState.Move;
                    _grid.Update();
                    _moveState = MoveState.Waiting;
                }
                else
                {
                    _inputState = InputState.Rotate;
                    Vector2 toCur = worldPos - Vec.xy(transform.position);
                    _inputStartAngle = transform.rotation.eulerAngles.z - Mathf.Atan2(toCur.y, toCur.x) * (180.0f / Mathf.PI);
                }

                _inputStart = worldPos;
            }

            if (Input.GetMouseButtonUp(0))
            {
                _stateChangeLastMouseUp = false;

                if (_inputState == InputState.Rotate)
                {
                    float v = transform.rotation.eulerAngles.z / 90.0f;
                    _deltaRot = v - Mathf.Floor(v);
                    if (_deltaRot < 0)
                        _deltaRot += 1f;

                    if (_deltaRot < 0.5f)
                        _deltaRot = -_deltaRot * 90f;
                    else
                        _deltaRot = (1 - _deltaRot) * 90f;

                    int rot = (int)(0.5f + (transform.rotation.eulerAngles.z + _deltaRot) / 90.0f) % 4;
                    _grid.Rotation = rot;
                }
                else if (_inputState == InputState.Move)
                {
                    if (_moveState != MoveState.Waiting)
                    {
                        if (!_remoteMove)
                        {
                            foreach (var con in _otherContainers)
                                con.StopRemoteMove();
                        }

                        DestroyHighlights();

                        foreach (Cell c in _moveCells)
                        {
                            c.StopMove();

                            //Find closest cell
                            Vector2 topLeft = Vec.xy(transform.position);
                            topLeft.x -= transform.localScale.x * 0.5f;
                            topLeft.y += transform.localScale.y * 0.5f;

                            Vector2 cellPos = Vec.xy(c.Transform.position);
                            Vector2 delta = (cellPos - topLeft) / _grid.CellSize;

                            int x = Mathf.FloorToInt(delta.x);
                            int y = -Mathf.FloorToInt(delta.y) - 1;

                            Vector2 center = new Vector2(topLeft.x + _grid.CellSize * ((float)x + 0.5f), topLeft.y - _grid.CellSize * ((float)y + 0.5f));
                            c.ResetDelta = center - cellPos;

                            int oldX = c.X;
                            int oldY = c.Y;

                            switch (_grid.Rotation)
                            {
                                case 0:
                                    c.X = x;
                                    c.Y = y;
                                    break;
                                case 1:
                                    c.X = _grid.Width - 1 - y;
                                    c.Y = x;
                                    break;
                                case 2:
                                    c.X = _grid.Width - 1 - x;
                                    c.Y = _grid.Width - 1 - y;
                                    break;
                                case 3:
                                    c.X = y;
                                    c.Y = _grid.Width - 1 - x;
                                    break;
                            }

                            if (oldX != c.X || oldY != c.Y)
                                _stateChangeLastMouseUp = true;
                        }
                    }
                }
            }

            bool mouseDown = Input.GetMouseButton(0);
            if (mouseDown)
            {
                if (_inputState == InputState.Rotate)
                {
                    Vector2 pos = Vec.xy(transform.position);
                    Vector2 toCur = worldPos - pos;

                    transform.rotation = Quaternion.Euler(0, 0, ((180.0f / Mathf.PI) * (Mathf.Atan2(toCur.y, toCur.x))) + _inputStartAngle);
                }
                else if (_inputState == InputState.Move)
                {
                    switch (_moveState)
                    {
                        case MoveState.Waiting:
                            {
                                if (Mathf.Abs(_inputStart.x - worldPos.x) > _grid.CellSize * 0.3f)
                                {
                                    Vector2 topLeft = Vec.xy(transform.position);
                                    topLeft.x -= transform.localScale.x * 0.5f;
                                    topLeft.y += transform.localScale.y * 0.5f;

                                    int row = (int)((topLeft.y - _inputStart.y) / _grid.CellSize);

                                    _moveCells.Clear();
                                    _grid.GetRow(row, ref _moveCells);

                                    _inputStart = worldPos;
                                    _moveState = MoveState.Horizontal;

                                    foreach (Cell c in _moveCells)
                                    {
                                        c.StartMove();
                                    }

                                    foreach (var con in _otherContainers)
                                        con.StartRemoteMove(_inputStart, Vec.xy(transform.position), _moveState, row);

                                    CreateHighlightRow(row);

                                }
                                else if (Mathf.Abs(_inputStart.y - worldPos.y) > _grid.CellSize * 0.3f)
                                {
                                    Vector2 topLeft = Vec.xy(transform.position);
                                    topLeft.x -= transform.localScale.x * 0.5f;
                                    topLeft.y += transform.localScale.y * 0.5f;

                                    int column = (int)((_inputStart.x - topLeft.x) / _grid.CellSize);

                                    _moveCells.Clear();
                                    _grid.GetColumn(column, ref _moveCells);

                                    _inputStart = worldPos;
                                    _moveState = MoveState.Vertical;

                                    foreach (Cell c in _moveCells)
                                        c.StartMove();

                                    foreach (var con in _otherContainers)
                                        con.StartRemoteMove(_inputStart, Vec.xy(transform.position), _moveState, column);

                                    CreateHighlightColumn(column);
                                }
                            }
                            break;
                        case MoveState.Horizontal:
                            {
                                foreach (Cell c in _moveCells)
                                {
                                    switch (_grid.Rotation)
                                    {
                                        case 0:
                                            c.Move(new Vector2(worldPos.x - _inputStart.x, 0.0f));
                                            break;
                                        case 1:
                                            c.Move(new Vector2(0f, -(worldPos.x - _inputStart.x)));
                                            break;
                                        case 2:
                                            c.Move(new Vector2(-(worldPos.x - _inputStart.x), 0.0f));
                                            break;
                                        case 3:
                                            c.Move(new Vector2(0f, worldPos.x - _inputStart.x));
                                            break;
                                    }
                                }
                            }
                            break;
                        case MoveState.Vertical:
                            {
                                foreach (Cell c in _moveCells)
                                {
                                    switch (_grid.Rotation)
                                    {
                                        case 0:
                                            c.Move(new Vector2(0.0f, worldPos.y - _inputStart.y));
                                            break;
                                        case 1:
                                            c.Move(new Vector2(worldPos.y - _inputStart.y, 0f));
                                            break;
                                        case 2:
                                            c.Move(new Vector2(0f, -(worldPos.y - _inputStart.y)));
                                            break;
                                        case 3:
                                            c.Move(new Vector2(-(worldPos.y - _inputStart.y), 0f));
                                            break;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            else
                _inputState = InputState.Nothing;
        }
        else
        {
            _inputState = InputState.Nothing;
        }

        if (_inputState != InputState.Rotate)
        {
            float rot;
            if (_deltaRot < 0.1 && _deltaRot > -0.1)
                rot = _deltaRot;
            else
                rot = _deltaRot * Time.deltaTime * 20f;

            transform.rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rot);
            _deltaRot -= rot;
        }

        if (_inputState != InputState.Move)
        {
            foreach (Cell c in _grid.Cells)
            {
                c.Update();
            }
        }
    }

    void CreateHighlightRow(int row)
    { 
        Vector2 topLeft = Vec.xy(transform.position);
        topLeft.x -= transform.localScale.x * 0.5f;
        topLeft.y += transform.localScale.y * 0.5f;

        for(int i = 0; i < _grid.Width; ++i)
        {
            Vector2 center = new Vector2(topLeft.x + _grid.CellSize * ((float)i + 0.5f), topLeft.y - _grid.CellSize * ((float)row + 0.5f));
            Transform t = (Transform)Instantiate(_highlightPrefab, center, Quaternion.identity);
            t.parent = this.transform;
            _highlights.Add(t);
            if (_grid.Width == 5)
                t.localScale = 0.8f * t.localScale;
        }
    }

    void CreateHighlightColumn(int column)
    {
        Vector2 topLeft = Vec.xy(transform.position);
        topLeft.x -= transform.localScale.x * 0.5f;
        topLeft.y += transform.localScale.y * 0.5f;

        for (int i = 0; i < _grid.Width; ++i)
        {
            Vector2 center = new Vector2(topLeft.x + _grid.CellSize * ((float)column + 0.5f), topLeft.y - _grid.CellSize * ((float)i + 0.5f));
            Transform t = (Transform)Instantiate(_highlightPrefab, center, Quaternion.identity);
            t.parent = this.transform;
            _highlights.Add(t);
            if (_grid.Width == 5)
                t.localScale = 0.8f * t.localScale;
        }
    }

    public void GameOver()
    {
        _gameOver = true;
        _stateChangeLastMouseUp = false;
    }

    void DestroyHighlights()
    {
        foreach(var t in _highlights)
            Destroy(t.gameObject);

        _highlights.Clear();
    }

    bool IsInside(Vector2 pos)
    {
        float w = 0.5f * transform.localScale.x;
        Vector3 c = transform.position;

        if (pos.x < c.x - w)
            return false;
        if (pos.x > c.x + w)
            return false;
        if (pos.y < c.y - w)
            return false;
        if (pos.y > c.y + w)
            return false;

        return true;
    }
}
