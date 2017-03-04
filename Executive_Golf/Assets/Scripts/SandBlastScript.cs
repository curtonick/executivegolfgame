using UnityEngine;
using System.Collections;

public class SandBlastScript : MonoBehaviour {

	ParticleSystem.Particle[] array;
	Vector3 wind;
	ParticleSystem partSys;

	// Use this for initialization
	void Start () {
		partSys = gameObject.GetComponent<ParticleSystem> ();
		partSys.GetComponent<Renderer> ().enabled = false;
		array = new ParticleSystem.Particle[250];
		//Get wind force for the hole, decrease amount for more realistic result?
		wind = GameObject.Find ("Wind").GetComponent<WindScript> ().wind/16.0f;
	
	}

	// Update is called once per frame
	void Update () {
		if (partSys.particleCount < 250) {
			//this is to avoid no particle errors. Once some particles disappear, make them all turn off
			partSys.GetComponent<Renderer> ().enabled = false;
			gameObject.SetActive (false);
		} else {
			//apply wind force to all particles
			partSys.GetParticles(array);
			for (int i = 0; i < array.Length; i++) {
				array [i].velocity = array [i].velocity + wind;
			}
			partSys.SetParticles (array, array.Length);

		}

	} 

	//Plays a random blast of sand in front of the ball. emits 250 partices 
	// with random velocity and size
	public void playBlast(float pow){
		int upperVeloBound = (int) (pow / 10) + 2;
		gameObject.SetActive (true);
		partSys.GetComponent<Renderer> ().enabled = true;
		for (int i = 0; i < 250; i++) {
			ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams ();
			emitOverride.velocity = new Vector3 (0, 0, Random.value + Random.Range(0,upperVeloBound) + 0.4f);
			emitOverride.startSize = Random.value / 2.0f;
			emitOverride.startLifetime = Random.value + 1.8f;
			partSys.Emit (emitOverride, 1);
		}

	}

}
