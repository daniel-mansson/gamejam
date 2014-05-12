using UnityEngine;
using System.Collections;

public class Interface : MonoBehaviour {

    public GUIStyle style;
    public GUIStyle right;
    public GUIStyle left;
    public GUIStyle labelStyle;
    public GUIStyle levelStyle;
    public Texture restart;

    public int star1Value;
    public int star2Value;
    public int star3Value;

    public Transform star1;
    public Transform star2;
    public Transform star3;

    public Color _failColor;

    private int _count = 0;

    ContainerBehaviour[] _containers;
	// Use this for initialization
	void Start () {
        _containers = FindObjectsOfType<ContainerBehaviour>();
        
	}
	
	// Update is called once per frame
    void OnGUI()
    {
        float size = Screen.height / 12f;
        Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(0, -4.7f, 0));
        if (GUI.Button(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), "", style))
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        pos = Camera.main.WorldToScreenPoint(new Vector3(0.0f, 3.9f, 0));
        if (GUI.Button(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), "", right))
        {
            int next = (Application.loadedLevel + 1) % Application.levelCount;
            Application.LoadLevel(next);
        }

        pos = Camera.main.WorldToScreenPoint(new Vector3(0.0f, 4.7f, 0));
        if (GUI.Button(new Rect(pos.x - size*0.5f, pos.y - size*0.5f, size, size), "", left))
        {
            int prev = (Application.loadedLevel + Application.levelCount - 1) % Application.levelCount;
            Application.LoadLevel(prev);
        }

        pos = Camera.main.WorldToScreenPoint(new Vector3(0f, -3.8f, 0));
        GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), _count.ToString(), labelStyle);

        pos = Camera.main.WorldToScreenPoint(star1.transform.position);
        GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), star3Value.ToString(), labelStyle);

        pos = Camera.main.WorldToScreenPoint(star2.transform.position);
        GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), star2Value.ToString(), labelStyle);

        pos = Camera.main.WorldToScreenPoint(star3.transform.position);
        GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), star1Value.ToString(), labelStyle);

        pos = Camera.main.WorldToScreenPoint(new Vector3(0f, 3.2f, 0));
        GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), (Application.loadedLevel + 1) + "/" + Application.levelCount, levelStyle);

        if (Application.loadedLevel == 0)
        {
            pos = Camera.main.WorldToScreenPoint(new Vector3(-4f, -4.2f, 0));
            GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), "Try to  make both\nsides equal!", labelStyle);

            pos = Camera.main.WorldToScreenPoint(new Vector3(4f, 4.2f, 0));
            GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), "Changes one one side\nalso affects the other!", labelStyle);

            pos = Camera.main.WorldToScreenPoint(new Vector3(-4f, 4.2f, 0));
            GUI.Label(new Rect(pos.x - size * 0.5f, pos.y - size * 0.5f, size, size), "(Rotations does not count as moves)", levelStyle);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            foreach (var c in _containers)
            {
                if (c.StateChangeOnLastMouseUp)
                {
                    _count++;

                    if (_count > star1Value)
                        star1.renderer.material.color = _failColor;
                    if (_count > star2Value)
                        star2.renderer.material.color = _failColor;
                    if (_count > star3Value)
                        star3.renderer.material.color = _failColor;
                    break;
                }
            }

            if (isGameOver())
            {
                foreach (var c in _containers)
                    c.GameOver();
            }
        }
    }

    bool isGameOver()
    {
        foreach (var c in _containers)
            c.Grid.Update();

        for (int i = 1; i < _containers.Length; ++i)
        {
            if (!_containers[i - 1].Grid.StateEquals(_containers[i].Grid))
                return false;
        }

        return true;
    }
}
