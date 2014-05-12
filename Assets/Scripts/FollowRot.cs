using UnityEngine;
using System.Collections;

public class FollowRot : MonoBehaviour {
    public Transform target;

	
	void Start () 
    {
	
	}
	
	
	void Update () 
    {
        transform.rotation = target.transform.rotation;
	}
}
