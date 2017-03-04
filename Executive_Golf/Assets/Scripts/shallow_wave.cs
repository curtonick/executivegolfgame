using UnityEngine;
using System.Collections;
//Modified Lab4 code

public class shallow_wave : MonoBehaviour {
	int size;
	float[,] old_h;
	float[,] h;
	float[,] new_h;
	public bool active = false;
	private bool splash = false;
	float ballX;
	float ballZ;


	// Use this for initialization
	void Start () {
		size = 64;
		old_h = new float[size,size];
		h = new float[size,size];
		new_h = new float[size,size];
		active = false;


		//Resize the mesh into a size*size grid
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.Clear ();
		Vector3[] vertices=new Vector3[size*size];
		for (int i=0; i<size; i++)
		for (int j=0; j<size; j++) 
		{
			vertices[i*size+j].x=i*0.2f-size*0.1f;
			vertices[i*size+j].y=0;
			vertices[i*size+j].z=j*0.2f-size*0.1f;
		}
		int[] triangles = new int[(size - 1) * (size - 1) * 6];
		int index = 0;
		for (int i=0; i<size-1; i++)
		for (int j=0; j<size-1; j++)
		{
			triangles[index*6+0]=(i+0)*size+(j+0);
			triangles[index*6+1]=(i+0)*size+(j+1);
			triangles[index*6+2]=(i+1)*size+(j+1);
			triangles[index*6+3]=(i+0)*size+(j+0);
			triangles[index*6+4]=(i+1)*size+(j+1);
			triangles[index*6+5]=(i+1)*size+(j+0);
			index++;
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
	


	}

	void Shallow_Wave()
	{	
		float rate = 0.005f;
		float damping = 0.999f;
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {

				//Assign the neighboring points as if they were boundary cases
				float hiPlus1 = h [i, j];
				float hjPlus1 = h [i, j];
				float hiLess1 = h [i, j];
				float hjLess1 = h [i, j];

				//Correctly assign points if they are within the height field
				if(i+1 < size)
					hiPlus1 = h [i+1, j];
				if(j+1 < size)
					hjPlus1 = h [i, j+1];

				if(i-1 > -1)
					hiLess1 = h [i-1, j];
				if(j-1 > -1)
					hjLess1 = h [i, j-1];
				
				new_h[i,j] = h[i,j] + (h[i,j] - old_h[i,j]) * damping + (hiLess1 + hiPlus1 + hjLess1 + hjPlus1 - 4.0f*h[i,j]) * rate;
			}
		}

		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				old_h[i,j] = h[i,j];
				h[i,j] = new_h[i,j];
			}
		}
			
	}
		

	//To be called by WaterTrigger function when ball enters water
	public void generateSplash (float xEnter, float zEnter){
		ballX = xEnter;
		ballZ = zEnter;
		active = true;

	}

	//To be called by WaterTrigger function to reset
	public void resetSplash (){
		Start ();
		splash = false;
		active = false;

	}


	// Update is called once per frame
	void Update () 
	{
		if (active) {

			//Step 1: Copy vertices.y into h
			Mesh mesh = GetComponent<MeshFilter> ().mesh;
			Vector3[] vertices = mesh.vertices;
			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					h [i, j] = vertices [i * size + j].y;
				}
			}



			if (!splash) {
				//get x/z result
				Vector3 center = (Vector3)gameObject.transform.position;
				//Use scale, plane is 10x10 mesh. Plus 1's because mesh grows on run
				Vector3 botLeft = center - new Vector3 ((transform.localScale.x + 1) / 2.0f * 10, 0, (transform.localScale.z + 1) / 2.0f * 10);


				//Get points on scale of 0 to 40? (the size of the plane)
				float xPoint = ballX - botLeft.x;
				float zPoint = ballZ - botLeft.z;

				//print (xPoint+":"+ zPoint);

				//Map 0 to 40 to 0 to 64 for size of mesh
				int pointI = Mathf.RoundToInt (xPoint * 1.5f);
				int pointJ = Mathf.RoundToInt (zPoint * 1.5f);
				//print (pointI + ":" + pointJ);

				float m = Random.value / 20.0f + 0.05f;
				h [pointI, pointJ] += m;
				splash = true;
			}






	
			//Step 3: Run Shallow Wave
			for (int k = 0; k < 8; k++) {
				//Note to grader: Can switch between bonus implementation and regular implementation here.
				Shallow_Wave ();
			}

			//Step 4: Copy h back into mesh
			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					vertices [i * size + j].y = h [i, j];
				}
			}

			mesh.vertices = vertices;
			mesh.RecalculateNormals ();
		}
	}
}
