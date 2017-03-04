using UnityEngine;
using System.Collections;

public class RangeHoleTrigger : MonoBehaviour {

	//Add points if make into hole on range
	//mostly untested, if you hole out on the range and it breaks, congratulations! and whoops, sorry


	private float enterTime = 0;
	AudioSource holed;
	bool holedout = false;
	Rigidbody ballrb;
	bool done = false;
	bool soundplayed = false;

	void Start(){
		ballrb = GameObject.Find ("Ball").GetComponent<Rigidbody> ();

	}

	void Holed_Out(){
		
		//In the hole condition should go here
		if (!done) {
			Physics.IgnoreCollision (ballrb.GetComponent<Collider> (), GameObject.Find ("Green").GetComponent<Collider> ());
			if (!soundplayed) {
				GameObject.Find ("holed").GetComponent<AudioSource> ().Play();
				soundplayed = true;
			}
			if (Time.time > enterTime + 0.2f) {
				GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().Reset_From_Hazard ();
				GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().MakeAnnouncement ("HOLED OUT! 1000 points!", Time.time + 2.5f, Color.red);
			} else if (Time.time > enterTime + 0.4f) {
				GameObject.Find ("Ball").GetComponent<RangeBallRigidBody> ().addPoints (1000);
				done = true;
			}
		}



	}

	// Use this for initialization
	void OnTriggerEnter (Collider other){
		enterTime = Time.time;

	}

	void OnTriggerStay (Collider other){
		if (other.attachedRigidbody.velocity.magnitude < 6.0f || other.attachedRigidbody.velocity.y < -1.0) {
			holedout = true;

		}

	}

	void OnTriggerExit (Collider other){
		if (other.attachedRigidbody.velocity.magnitude > 6.0f) {
			other.transform.LookAt(GameObject.Find("CupHeight").transform);
			other.attachedRigidbody.AddRelativeForce (0,0, 5.0f, ForceMode.Impulse);
		}


	}

	void Update(){
		if (holedout)
			Holed_Out ();

	}

}
