using UnityEngine;
using System.Collections;

public class WindScript : MonoBehaviour {

	//Wind
	public Vector3 wind;
	public Vector3 windGusts;
	private int xWind;
	private int zWind;  
	private float firstHalfTime;
	private float gustTime;
	private float secondHalfTime;
	private float waitTime;
	private bool gusting = false;


	// Use this for initialization
	public void Awake () {
		xWind =  Random.Range(-8,8);
		zWind = Random.Range(-8,8);
		wind = new Vector3 (xWind/4.0f, 0, zWind/4.0f); //generate wind in x/z plane, 8/4 is max value because it feels right (?)
		//note: not a realistic implementation
		//unsure why ordering is weird, something with cloth lab
		windGusts = new Vector3 (-xWind/4.0f, -zWind/4.0f, 0);
	}

	// FixedUpdate is called
	void FixedUpdate () {

		if (!gusting) {
			//gust on a certain time interval, could be changed to be somewhat random here
			gusting = true;
			firstHalfTime = Time.time + 1.5f;
			gustTime = firstHalfTime + 2.0f;
			secondHalfTime = gustTime + 1.5f;
			waitTime = secondHalfTime + 0.5f;
		}
		if(gusting){
			if (Time.time < firstHalfTime) {
				windGusts = new Vector3 (-xWind / 8.0f, -zWind / 8.0f, 0); //half speed
			}
			else if (Time.time < gustTime) {
				windGusts = new Vector3 (-xWind / 4.0f, -zWind / 4.0f, 0); //full speed
			} else if (Time.time < secondHalfTime) {
				windGusts = new Vector3 (-xWind / 8.0f, -zWind / 8.0f, 0); //half speed
			} else if (Time.time < waitTime) {
				windGusts = new Vector3 (-xWind / 16.0f, -zWind / 16.0f, 0); //low speed
			} else {
				gusting = false;
			}
		}
		
	}
}
