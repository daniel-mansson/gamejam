using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

    public Transform[] _containers;
    public int _width;
    public int _height;
    public Texture2D _data;

    public Transform _backgroundCellPrefab;
    public Transform _redCellPrefab;
    public Transform _greenCellPrefab;
    public Transform _blueCellPrefab;

	void Start ()   
    {
        int n ;

        for (n = 0; n < _containers.Length; ++n)
        {
            GameObject container = _containers[n].gameObject;
            float size = container.transform.localScale.x;
            float cellSize = size / (float)_width;
            var behaviour = container.GetComponent<ContainerBehaviour>();
            Grid grid = new Grid(_width, _height, cellSize);

            for (int i = 0; i < _height; ++i)
            {
                for (int j = 0; j < _width; ++j)
                {
                    Color c = _data.GetPixel(j, i + n * _height);

                    if (aprxEqual(c, Color.red))
                    {
                        float x = (float)j * cellSize + cellSize * 0.5f + container.transform.position.x - size * 0.5f;
                        float y = (float)i * cellSize + cellSize * 0.5f + container.transform.position.y - size * 0.5f;

                        Transform cell = (Transform)Instantiate(_redCellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        cell.parent = container.transform;
                        cell.transform.localScale = new Vector3(cell.transform.localScale.x * cellSize, cell.transform.localScale.y * cellSize, 1f);
                        grid.AddCell(new Cell(j, _height - i - 1, 1, cell));
                    }
                    if (aprxEqual(c, Color.green))
                    {
                        float x = (float)j * cellSize + cellSize * 0.5f + container.transform.position.x - size * 0.5f;
                        float y = (float)i * cellSize + cellSize * 0.5f + container.transform.position.y - size * 0.5f;

                        Transform cell = (Transform)Instantiate(_greenCellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        cell.parent = container.transform;
                        cell.transform.localScale = new Vector3(cell.transform.localScale.x * cellSize, cell.transform.localScale.y * cellSize, 1f);
                        grid.AddCell(new Cell(j, _height - i - 1, 2, cell));
                    }
                    if (aprxEqual(c, Color.blue))
                    {
                        float x = (float)j * cellSize + cellSize * 0.5f + container.transform.position.x - size * 0.5f;
                        float y = (float)i * cellSize + cellSize * 0.5f + container.transform.position.y - size * 0.5f;

                        Transform cell = (Transform)Instantiate(_blueCellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        cell.parent = container.transform;
                        cell.transform.localScale = new Vector3(cell.transform.localScale.x * cellSize, cell.transform.localScale.y * cellSize, 1f);
                        grid.AddCell(new Cell(j, _height - i - 1, 3, cell));
                    }
                }
            }

            grid.Container = container.transform;
            grid.Update();
            behaviour.InitData(grid, _containers);
        }

        n = 0;
        foreach (Transform c in _containers)
        {
            CreateContainer(c.gameObject, n);
            ++n;
        }

	}

    bool aprxEqual(Color a, Color b)
    { 
        return Vector3.Distance(new Vector3(a.r, a.g, a.b), new Vector3(b.r, b.g, b.b)) < 0.1f;
    }
	
    void CreateContainer(GameObject container, int n)
    {
        Destroy(container.GetComponent<MeshFilter>());
        float size = container.transform.localScale.x;
        float cellSize = size / (float)_width;
        
        for (int i = 0; i < _height; ++i)
        {
            for (int j = 0; j < _width; ++j)
            {
                float x = (float)j * cellSize + cellSize * 0.5f + container.transform.position.x - size * 0.5f;
                float y = (float)i * cellSize + cellSize * 0.5f + container.transform.position.y - size * 0.5f;

                Transform cell = (Transform)Instantiate(_backgroundCellPrefab, new Vector3(x, y, 0), Quaternion.identity);
                cell.parent = container.transform;
                cell.transform.localScale = new Vector3(cell.transform.localScale.x * cellSize, cell.transform.localScale.y * cellSize, 1f);
            }
        }
    }
}
