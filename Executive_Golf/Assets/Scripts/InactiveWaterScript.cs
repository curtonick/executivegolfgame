using UnityEngine;
using System.Collections;

public class InactiveWaterScript : MonoBehaviour {
	//Moves the ripple water effect plane to spot of ball entry

	void OnTriggerEnter (Collider other){
		Rigidbody ballrb = other.attachedRigidbody;
		Vector3 entryBallPosition = ballrb.transform.position;
		GameObject.Find ("Water").GetComponent<Transform> ().position = new Vector3 (entryBallPosition.x, 0.7f, entryBallPosition.z);

	}
}
