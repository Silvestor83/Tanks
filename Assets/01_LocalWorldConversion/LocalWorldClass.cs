using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalWorldClass : MonoBehaviour {

	public Vector3 destination;

	Vector3 point;
	Vector3 vector;
	Vector3 direction;

    Vector3 point2;
    Vector3 vector2;
    Vector3 direction2;

	// Use this for initialization
	void Start () {
		point = transform.TransformPoint (destination);
		vector = transform.TransformVector (destination);
		direction = transform.TransformDirection (destination);

        point2 = transform.InverseTransformPoint((destination));
        vector2 = transform.InverseTransformVector (destination);
        direction2 = transform.InverseTransformDirection (destination);

	}
	
	void OnDrawGizmos(){
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere (point, 0.2f);
		Gizmos.color = Color.magenta;
		Gizmos.DrawSphere (vector, 0.2f);
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere (direction, 0.2f);


        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(point2, Vector3.one * 0.2f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawCube (vector2, Vector3.one * 0.2f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube (direction2, Vector3.one * 0.2f);
	}
}
