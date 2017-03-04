using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//Very similar to BallRigidBody, a few things modified for range scoring
//see BallRigidBody for more comments

public class RangeBallRigidBody : MonoBehaviour {
	public bool launched = false;
	private Rigidbody rb;

	//Follow Camera Control
	private Vector3 offset;
	Camera follow;
	Camera high;
	private Vector3 restPosition;
	private GameObject cupheight;
	private float degreeChange;

	private float oldFace;
	private float newFace;
	private float turnspeed;

	private Text yardage;
	private Text club;

	private string[] sticks = {
		"Driver 270 yds",
		"3 Wood 240 yds",
		"5 Wood 220 yds",
		"3 Hybrid 205 yds",
		"4 Iron 195 yds",
		"5 Iron 180 yds",
		"6 Iron 170 yds",
		"7 Iron 155 yds",
		"8 Iron 145 yds",
		"9 Iron 135 yds",
		"PW 122 yds",
		"SW 105 yds",
		"LW 85 yds",
		"Putter 50 yds"
	};
	private Vector3[] full = {
		new Vector3(0,21,54),	//D
		new Vector3(0,21,50),	//3
		new Vector3(0,21,47),	//5
		new Vector3(0,21,44),	//3h
		new Vector3(0,22,41),	//4
		new Vector3(0,22,38),	//5
		new Vector3(0,22,35),	//6
		new Vector3(0,23,33),	//7
		new Vector3(0,23,31),	//8
		new Vector3(0,23,28),	//9
		new Vector3(0,24,26),	//PW
		new Vector3(0,24,23),	//SW
		new Vector3(0,24,20),	//LW
		new Vector3(0,0,25)		//Putter
	};

	int clubSelection;

	//Power and Accuracy UI
	private RectTransform powerbar;
	private bool swinging = false;
	private bool powIncrease = true;
	private float power;
	private Text powerText;

	private RectTransform accuracybar;
	private bool aiming = false;
	private bool accIncrease = true;
	private float accuracy;
	private Text accuracyText;

	private bool shotTaken = false;

	public float MAXPOWER = 104;
	float toTopOfSwing;

	//GolfClub
	private GameObject golfclub;
	private Vector3 initialGolfClubRotation;
	private Quaternion resetGolfClubRotation;
	private Vector3 currentGolfClubWorldPosition;
	private Vector3 initialGolfClubLocalPosition;

	//Audio
	AudioSource wood;
	AudioSource wedge;
	AudioSource putt;

	//Time
	float totalTime;
	float timeOfShot;

	//Trail
	TrailRenderer ballTrail;

	//Collsion test
	public bool firstCollision = false;

	//Draw and Fade
	private float drawFade;
	private RectTransform drawBar;
	private RectTransform fadeBar;

	//Lies
	enum Lies {Tee, Fairway, Green, Rough, Sand};
	private Lies lie;
	private Text lieText;
	private float liePowerModifier;

	//Tee
	private GameObject tee;

	//Wind
	public Vector3 wind;
	Camera pinPreview;

	//Strokes and Hole Info
	private Text strokeText;
	private int strokeCount = 10;

	//Points
	public int rangePoints;
	private Text pointsText;

	//Announcement
	private Text announcementText;
	private bool announcementOn = false;
	private string displayText = "";
	private float displayDuration = 0;
	Color displayColor = Color.blue;

	//Main Menu
	private GameObject mainMenu;

	// Use this for initialization
	public void Start () {
		restPosition = transform.position;
		rb = gameObject.GetComponent<Rigidbody> ();
		follow = GameObject.Find ("FollowCamera").GetComponent<Camera> ();
		high = GameObject.Find ("HighCamera").GetComponent<Camera> ();
		cupheight = GameObject.Find ("CupHeight");
		offset = follow.transform.position - transform.position;
		degreeChange = 0.0f;
		oldFace = 0.0f;
		newFace = 0.0f;
		turnspeed = 0.2f;
		yardage = GameObject.Find ("Yardage").GetComponent<Text> ();
		club = GameObject.Find ("Club").GetComponent<Text> ();
		clubSelection = 0;
		power = 1.0f;
		powerbar = GameObject.Find ("PowerBar").GetComponent<RectTransform> ();
		accuracybar = GameObject.Find ("AccuracyBar").GetComponent<RectTransform> ();
		golfclub = GameObject.Find ("GolfClub");
		initialGolfClubRotation = golfclub.transform.eulerAngles;
		resetGolfClubRotation = golfclub.transform.localRotation;
		currentGolfClubWorldPosition = golfclub.transform.position;
		initialGolfClubLocalPosition = golfclub.transform.localPosition;
		ballTrail = gameObject.GetComponent<TrailRenderer> ();

		wood = GameObject.Find ("wood").GetComponent<AudioSource>();
		wedge = GameObject.Find ("wedge").GetComponent<AudioSource>();
		putt = GameObject.Find ("putt").GetComponent<AudioSource>();

		drawFade = 0;
		drawBar = GameObject.Find ("DrawBar").GetComponent<RectTransform> ();
		fadeBar = GameObject.Find ("FadeBar").GetComponent<RectTransform> ();

		lie = Lies.Tee;
		//Setup Draw and Fade UI
		GameObject.Find ("D").GetComponent<Text> ().text = "D";
		GameObject.Find ("F").GetComponent<Text> ().text = "F";

		//Lie UI
		lieText = GameObject.Find ("LieText").GetComponent<Text> ();
		lie = Lies.Tee;
		liePowerModifier = 1.0f;

		//Tee
		tee = GameObject.Find("Tee");

		//Wind
		wind = GameObject.Find("Wind").GetComponent<WindScript>().wind;
		pinPreview = GameObject.Find ("PinPreview").GetComponent<Camera> ();

		//Stroke
		strokeText = GameObject.Find ("StrokeCount").GetComponent<Text> ();
		strokeText.text = "Stroke: " + strokeCount;

		//Power and Accuracy Text
		powerText = GameObject.Find ("PowerDisplay").GetComponent<Text> ();
		accuracyText = GameObject.Find ("AccuracyDisplay").GetComponent<Text> ();

		powerText.text = "";
		accuracyText.text = "";

		rangePoints = 0;
		pointsText = GameObject.Find("ParText").GetComponent<Text>();
		pointsText.text = "Points: 0";

		announcementText = GameObject.Find ("Announcement").GetComponent<Text> ();

		mainMenu = GameObject.Find ("MainMenuCamera");

		Display_Yardage ();

		mainMenu.GetComponent<MainMenuScript> ().BeginMenu ();
	}


	//***Helper Methods***

	//Game Controls when Ball is not moving
	void Ball_Address() {

		//Quit the Range Activity
		if (Input.GetKeyDown (KeyCode.Escape) || strokeCount == 0) {
			MakeAnnouncement ("Final Score: " + rangePoints + "", Time.time + 1.99f, Color.white);
			follow.enabled = false;
			high.enabled = false;
			mainMenu.GetComponent<Camera> ().enabled = true;
			mainMenu.SetActive (true);
			mainMenu.GetComponent<MainMenuScript> ().Invoke ("RangeResults", 2.0f);
		}

		if (Input.GetKey (KeyCode.Tab)) {
			follow.enabled = false;
			high.enabled = true;
		} else {
			follow.enabled = true;
			high.enabled = false;
		}

		if (Input.GetKey (KeyCode.W)) {
			pinPreview.enabled = true;
		} else {
			pinPreview.enabled = false;
		}

		Set_DrawFade ();

		float x = gameObject.transform.eulerAngles.x;
		float y = gameObject.transform.eulerAngles.y;
		float z = gameObject.transform.eulerAngles.z;

		if (Input.GetKey (KeyCode.LeftShift)) {
			turnspeed = 0.6f;
		} else {
			turnspeed = 0.2f;
		}
			

		if(Input.GetKey(KeyCode.RightArrow)){
			gameObject.transform.eulerAngles = new Vector3 (x, y + turnspeed, z); //TODO: replace this with Rotate()
			follow.transform.RotateAround (transform.position, Vector3.up, turnspeed);
			degreeChange += turnspeed;
		}
		if(Input.GetKey(KeyCode.LeftArrow)){
			gameObject.transform.eulerAngles = new Vector3 (x, y - turnspeed, z);
			follow.transform.RotateAround (transform.position, Vector3.up, -turnspeed);
			degreeChange -= turnspeed;
		}
	}

	void Display_Yardage (){
		float yards = Mathf.Ceil(Vector3.Distance(gameObject.transform.position, cupheight.transform.position) * 1.0936f) ; //Convert meters to yards
		yardage.text = "Distance to Pin: " + yards + " yds";
	}


	void Select_Club (){
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			if (clubSelection < sticks.Length-1) {
				clubSelection++;
			}
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			if (clubSelection > 0) {
				clubSelection--;
			}
		}
		club.text = sticks[clubSelection];
	}

	void Take_Shot(){
		if (power < MAXPOWER && powIncrease) {
			power += 2.0f;
			powerbar.sizeDelta = new Vector2 (5.0f, power * 2 + 5.0f);
		} else if (power >= MAXPOWER) {
			powIncrease = false;
		}
			
		if (power > 0 && !powIncrease) {
			power -= 4.0f;
			powerbar.sizeDelta = new Vector2 (5.0f, power * 2 + 5.0f);
		} else if (power <= 0){
			powIncrease = true;
		}
	}

	void Take_Aim(){
		if (accuracy < 125 && accIncrease) {
			accuracy += 2.0f;
			accuracybar.sizeDelta = new Vector2 (5.0f, accuracy * 2 + 5.0f);
		} else if (accuracy >= 125) {
			accIncrease = false;
		}
			
	}


	void Reset_Power_Accuracy (){
		powerText.text = "";
		accuracyText.text = "";
		golfclub.transform.localPosition = initialGolfClubLocalPosition;
		golfclub.SetActive (true);
		toTopOfSwing = 0;
		power = 0;
		accuracy = 0;
		powIncrease = true;
		accIncrease = true;
		powerbar.sizeDelta = new Vector2 (5.0f, 5.0f);
		accuracybar.sizeDelta = new Vector2 (5.0f, 5.0f);
	}

	void Set_DrawFade(){
		if (Input.GetKey (KeyCode.D) && drawFade >= -100.0f) {
			drawFade -= 2.0f;
		} else if (Input.GetKey (KeyCode.F) && drawFade <= 100.0f) {
			drawFade += 2.0f;
		}
		if (drawFade < 0) {
			drawBar.sizeDelta = new Vector2 (Mathf.Abs (drawFade), 24.0f);
		} else {
			fadeBar.sizeDelta = new Vector2 (drawFade, 24.0f);
		}
	}

	void Reset_DrawFade (){
		drawFade = 0;
		drawBar.sizeDelta = new Vector2 (0.0f, 24.0f);
		fadeBar.sizeDelta = new Vector2 (0.0f, 24.0f);
	}


	void Backswing(){
		golfclub.transform.eulerAngles = initialGolfClubRotation;
		golfclub.transform.Rotate(0,0,-power*1.6f); //TODO: Bad constant here
	}

	void DownSwing(){
		if(-Mathf.Abs((accuracy - 100)) > -power && accuracy < 101){
			golfclub.transform.eulerAngles = initialGolfClubRotation;
			golfclub.transform.Rotate(0,0,-Mathf.Abs((accuracy - 100)*1.6f)); //TODO: Bad constant here
		}
	}

	void FollowThrough(){
		if (toTopOfSwing < power) {
			//Ensure golfclub does not move with ball
			golfclub.transform.position = currentGolfClubWorldPosition;

			toTopOfSwing += 2.0f;
			golfclub.transform.eulerAngles = initialGolfClubRotation;
			golfclub.transform.Rotate (0, 0, toTopOfSwing * 1.6f); //TODO: Bad constant here
		} else {
			golfclub.SetActive (false);
		}

	}

	int GetClubType(){
		if (clubSelection >= 13) {
			return 3; //Putter
		} else if (clubSelection >= 4) {
			return 2; //Iron
		} else {
			return 1; //Wood
		}
	}

	void SetLie () {
		if (lie == Lies.Fairway){
			lieText.text = "Fairway: 99%";
			liePowerModifier = 0.99f;
		}
		else if (lie == Lies.Green){
			clubSelection = 13;
			club.text = sticks[clubSelection];
			lieText.text = "Green: 100%";
			liePowerModifier = 1.0f;
		}
		else if (lie == Lies.Rough){
			lieText.text = "Rough: 90%";
			liePowerModifier = 0.9f;
		}
		else if (lie == Lies.Sand){
			lieText.text = "Sand: 60%";
			liePowerModifier = 0.6f;
		}
		else{
			lieText.text = "Tee: 100%";
			liePowerModifier = 1.0f;
		}
	}



	
	// Update is called once per frame
	void Update () {
		if (announcementOn)
			MakeAnnouncement (displayText, displayDuration, displayColor);

		ballTrail.enabled = false;
		//Club Selection and Camera Rotation
		if (!launched) {
			Ball_Address ();
			Select_Club ();
		}

		if (swinging)
			Backswing ();
		if (aiming)
			DownSwing ();
		if (launched)
			FollowThrough ();
		if (!launched) {
			//Kinda dumb solution, want to avoid if possible
			currentGolfClubWorldPosition = golfclub.transform.position;
		}

		//Power and Accuracy Setting
		if (Input.GetKeyDown (KeyCode.Space) && !swinging && !aiming && !shotTaken) {
			initialGolfClubRotation = golfclub.transform.eulerAngles;
			Vector3 oldPosition = golfclub.transform.localPosition;
			Vector3 newPosition = new Vector3 (oldPosition.x + 2.5f, oldPosition.y, oldPosition.z);
			golfclub.transform.localPosition = newPosition;
			swinging = true;

		} else if (Input.GetKeyDown (KeyCode.Space) && swinging && !aiming && !shotTaken) {
			powerText.text = Mathf.Ceil (power) + "%";
			swinging = false;
			aiming = true;
		} else if (Input.GetKeyDown (KeyCode.Space) && !swinging && aiming && !shotTaken) {
			accuracyText.text = Mathf.Ceil (accuracy) + "%";

			//Play hit sound
			int soundType = GetClubType ();
			if (soundType == 3) {
				putt.Play ();
			} else if (soundType == 2) {
				wedge.Play ();
			} else if (soundType == 1) {
				wood.Play ();
			}
			//Collsion bool variables
			rb.isKinematic = false;
			firstCollision = false; //adding this elsewhere, could delete
			rb.drag = 0.08f;
			shotTaken = true;
			aiming = false;
			timeOfShot = Time.time; //Time of shot set when final space is pressed
		}


		//Follow Camera Control
		if (launched) {
			if (Time.time > timeOfShot + (1.0f + Mathf.Abs(clubSelection - full.Length)/8.0f)  * power/104.0f) {
				follow.transform.position = transform.position + offset;
			} else {
				ballTrail.enabled = true;
			}
			Display_Yardage ();
		} else {
			offset = follow.transform.position - transform.position;
		}
		
	}

	void FixedUpdate () {
		//Update game time
		if(swinging) Take_Shot();
		if (aiming) Take_Aim ();


		//Take the shot
		if(!launched && shotTaken){
			tee.SetActive (false);
			float leftRightChange = (accuracy - 100.0f);
			power = power - Mathf.Abs (leftRightChange/2);

			//Display "NICE SHOT" if the shot was good.
			if (power >= 100.0f) {
				MakeAnnouncement ("NICE SHOT!", Time.time + 1.0f, Color.yellow);
			}

			//Change ball rotation based on accuracy
			float x = gameObject.transform.eulerAngles.x;
			float y = gameObject.transform.eulerAngles.y;
			float z = gameObject.transform.eulerAngles.z;
			gameObject.transform.eulerAngles = new Vector3 (x, y + leftRightChange, z);

			//Change Power Vector based on power and club selection

			//Set DrawFade Modifier. divde by 400.0 to get range of 0.0 to 0.1. Then multiply by -1, then add 1.
			//Range of 0.9 to 1
			//Shots that draw or fade should not have their full power in the forward direction
			float drawFadeModifier = Mathf.Abs(drawFade)/1000.0f * -1 + 1;

			Vector3 shot = full[clubSelection] * power/100.0f * liePowerModifier * drawFadeModifier;
			rb.AddRelativeForce (shot, ForceMode.Impulse);
			//rb.AddTorque (gameObject.transform.right * 1.2f);
			if (clubSelection == 13) {
				firstCollision = true;
			} else {
				firstCollision = false;
			}

			launched = true;

		}
		if (launched && shotTaken && !firstCollision) {
			//Draw or Fade
			Vector3 curve = new Vector3 (drawFade/10 * (Mathf.Abs(clubSelection - full.Length)/28.0f + 0.5f), 0, 0); //TODO: Bad draw/fade constant
			rb.AddRelativeForce (curve, ForceMode.Force);

			//Wind
			rb.AddForce (wind, ForceMode.Force);
		}


			
	}

	void LateUpdate(){

		if (launched && rb.velocity.magnitude < 0.75f && rb.angularVelocity.magnitude < 7.1f && Mathf.Abs(rb.velocity.y) < 0.05f) {
			int distance = Mathf.CeilToInt(Vector3.Distance (transform.position, restPosition) * 1.0936f);
			MakeAnnouncement ("Last shot: " + distance + " yds", Time.time + 2.0f, Color.white);
			GameObject.Find ("Wind").GetComponent<WindScript> ().Awake (); //re-roll wind
			wind = GameObject.Find("Wind").GetComponent<WindScript>().wind;
			Reset_From_Hazard ();

		}


	}


	void OnCollisionEnter (Collision col)
	{
		rb.velocity = rb.velocity * 0.90f;

		if (!firstCollision && launched && Time.time > timeOfShot + Time.deltaTime*2) {
			firstCollision = true;
		}

		if (Time.time > timeOfShot + Time.deltaTime * 2) {
			if (col.gameObject.name == "Fairway") {
				lie = Lies.Fairway;
				rb.drag = 0.30f;
			} else if (col.gameObject.name == "Green") {
				lie = Lies.Green;
				rb.drag = 0.40f;
			} else if (col.gameObject.name == "Rough") {
				lie = Lies.Rough;
				rb.drag = 0.50f;
			} else if (col.gameObject.name == "Sand") {
				lie = Lies.Sand;
				rb.drag = 0.65f;
			} else {
				lie = Lies.Tee;
				rb.drag = 0.30f;
			}
		}

		if (col.gameObject.name == "Sign100") {
			addPoints (100);
			MakeAnnouncement ("100 Points! x2", Time.time + 1.5f, Color.red);
		} else if (col.gameObject.name == "Sign150") {
			addPoints (150);
			MakeAnnouncement ("150 Points! x2", Time.time + 1.5f, Color.white);
		} else if (col.gameObject.name == "Sign200") {
			addPoints (200);
			MakeAnnouncement ("200 Points! x2", Time.time + 1.5f, Color.blue);
		} else if (col.gameObject.name == "Sign250") {
			addPoints (250);
			MakeAnnouncement ("250 Points! x2", Time.time + 1.5f, Color.yellow);
		}

	}



	//public methods
	public void Reset_From_Hazard(){

		strokeCount -= 1; //one less range ball
		strokeText.text = "Stroke: " + strokeCount;
		launched = false;

		//Reset Ball
		rb.velocity = Vector3.zero;
		rb.angularVelocity = Vector3.zero;
		//restPosition = transform.position;
		transform.position = restPosition;
		cupheight.transform.Translate (0, transform.position.y - cupheight.transform.position.y, 0);
		transform.LookAt (cupheight.transform);
		rb.drag = 0.08f;
		rb.isKinematic = true;

		//Find change in Rotation
		newFace = transform.eulerAngles.y;
		degreeChange -= newFace - oldFace;
		oldFace = newFace;

		//Reset Club
		Vector3 oldPosition = golfclub.transform.localPosition;
		Vector3 newPosition = new Vector3 (oldPosition.x - 2.5f, oldPosition.y, oldPosition.z);
		golfclub.transform.localPosition = newPosition;
		golfclub.transform.localRotation = resetGolfClubRotation;
		golfclub.SetActive(true);

		//Reset Camera
		follow.transform.position = transform.position + offset;
		follow.transform.RotateAround (transform.position, Vector3.up, -degreeChange);
		degreeChange = 0;
		shotTaken = false;

		//Reset UI
		Reset_Power_Accuracy ();
		Reset_DrawFade ();

		//Update UI Lie
		firstCollision = false;
		Display_Yardage ();
		//setLie() ommitted because do not want to update lie

	}

	public void addPoints(int points){
		rangePoints += points;
		pointsText.text = "Points: " + rangePoints;
	}

	public void MakeAnnouncement (string text, float t, Color color){
		displayText = text;
		displayDuration = t;
		displayColor = color;
		if (Time.time < displayDuration) {
			announcementOn = true;
			announcementText.text = displayText;
			announcementText.color = color;
		} else {
			displayText = "";
			displayDuration = 0;
			announcementText.text = displayText;
			announcementOn = false;
		}

	}

}
	