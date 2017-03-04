using UnityEngine;
using System.Collections;

public class RangeSignTrigger : MonoBehaviour {

	//Points for collisions with signs

	// Use this for initialization
	void OnTriggerEnter (Collider other){
		string name = gameObject.name;
		if(name.Equals ("Sign100")){
			GameObject.Find("Ball").GetComponent<RangeBallRigidBody>().addPoints(100);
			GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().MakeAnnouncement ("100 points", Time.time + 2.5f, Color.red);
		} else if(name.Equals ("Sign150")){
			GameObject.Find("Ball").GetComponent<RangeBallRigidBody>().addPoints(150);
			GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().MakeAnnouncement ("150 points", Time.time + 2.5f, Color.white);
		} else if(name.Equals ("Sign200")){
			GameObject.Find("Ball").GetComponent<RangeBallRigidBody>().addPoints(200);
			GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().MakeAnnouncement ("200 points", Time.time + 2.5f, Color.blue);
		} else if(name.Equals ("Sign250")){
			GameObject.Find("Ball").GetComponent<RangeBallRigidBody>().addPoints(250);
			GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().MakeAnnouncement ("250 points", Time.time + 2.5f, Color.yellow);
		}

	}

	void OnTriggerStay (Collider other){

	}

	void OnTriggerExit (Collider other){


	}
		


}
