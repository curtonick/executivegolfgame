// Lab 3 modified by: Nick Curto
// Cloth motion for flag
using UnityEngine;
using System.Collections;

public class cloth_motion: MonoBehaviour {

	float 		t;
	int[] 		edge_list;
	float		damping;
	float[] 	L0;
	Vector3[] 	velocities;
	GameObject pin;
	GameObject wind;
	Vector3 windGusts;




	// Use this for initialization
	void Start () 
	{
		t 			= 0.075f;
		damping 	= 0.99f;
		//Only find game objects once, not every frame
		pin = GameObject.Find("Pin");
		wind = GameObject.Find ("Wind");
		windGusts = wind.GetComponent<WindScript> ().windGusts;

		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		int[] 		triangles = mesh.triangles;
		Vector3[] 	vertices = mesh.vertices;
	    
		//Construct the original edge list
		int[] original_edge_list = new int[triangles.Length*2];
		for (int i=0; i<triangles.Length; i+=3) 
		{
			original_edge_list[i*2+0]=triangles[i+0];
			original_edge_list[i*2+1]=triangles[i+1];
			original_edge_list[i*2+2]=triangles[i+1];
			original_edge_list[i*2+3]=triangles[i+2];
			original_edge_list[i*2+4]=triangles[i+2];
			original_edge_list[i*2+5]=triangles[i+0];
		}
		//Reorder the original edge list
		for (int i=0; i<original_edge_list.Length; i+=2)
			if(original_edge_list[i] > original_edge_list[i + 1]) 
				Swap(ref original_edge_list[i], ref original_edge_list[i+1]);
		//Sort the original edge list using quicksort
		Quick_Sort (ref original_edge_list, 0, original_edge_list.Length/2-1);

		int count = 0;
		for (int i=0; i<original_edge_list.Length; i+=2)
			if (i == 0 || 
			    original_edge_list [i + 0] != original_edge_list [i - 2] ||
			    original_edge_list [i + 1] != original_edge_list [i - 1]) 
					count++;

		edge_list = new int[count * 2];
		int r_count = 0;
		for (int i=0; i<original_edge_list.Length; i+=2)
			if (i == 0 || 
			    original_edge_list [i + 0] != original_edge_list [i - 2] ||
				original_edge_list [i + 1] != original_edge_list [i - 1]) 
			{
				edge_list[r_count*2+0]=original_edge_list [i + 0];
				edge_list[r_count*2+1]=original_edge_list [i + 1];
				r_count++;
			}


		L0 = new float[edge_list.Length/2];
		for (int e=0; e<edge_list.Length/2; e++) 
		{
			int v0 = edge_list[e*2+0];
			int v1 = edge_list[e*2+1];
			L0[e]=(vertices[v0]-vertices[v1]).magnitude;
		}

		velocities = new Vector3[vertices.Length];
		for (int v=0; v<vertices.Length; v++)
			velocities [v] = new Vector3 (0, 0, 0);

		//for(int i=0; i<edge_list.Length/2; i++)
		//	Debug.Log ("number"+i+" is" + edge_list [i*2] + "and"+ edge_list [i*2+1]);
	}

	void Quick_Sort(ref int[] a, int l, int r)
	{
		int j;
		if(l<r)
		{
			j=Quick_Sort_Partition(ref a, l, r);
			Quick_Sort (ref a, l, j-1);
			Quick_Sort (ref a, j+1, r);
		}
	}

	int  Quick_Sort_Partition(ref int[] a, int l, int r)
	{
		int pivot_0, pivot_1, i, j;
		pivot_0 = a [l * 2 + 0];
		pivot_1 = a [l * 2 + 1];
		i = l;
		j = r + 1;
		while (true) 
		{
			do ++i; while( i<=r && (a[i*2]<pivot_0 || a[i*2]==pivot_0 && a[i*2+1]<=pivot_1));
			do --j; while(  a[j*2]>pivot_0 || a[j*2]==pivot_0 && a[j*2+1]> pivot_1);
			if(i>=j)	break;
			Swap(ref a[i*2], ref a[j*2]);
			Swap(ref a[i*2+1], ref a[j*2+1]);
		}
		Swap (ref a [l * 2 + 0], ref a [j * 2 + 0]);
		Swap (ref a [l * 2 + 1], ref a [j * 2 + 1]);
		return j;
	}

	void Swap(ref int a, ref int b)
	{
		int temp = a;
		a = b;
		b = temp;
	}


	// Note: I added a Vector3 parameter so I only assign vertices once to the mesh
	void Strain_Limiting(Vector3[] vertices)
	{
		Vector3[] temp_x = new Vector3[vertices.Length];
		int[] temp_n = new int[vertices.Length];

		for (int e=0; e<edge_list.Length/2; e++) 
		{
			int i = edge_list [e * 2 + 0];
			int j = edge_list [e * 2 + 1];
			Vector3 vi = vertices[i];
			Vector3 vj = vertices[j];
			Vector3 xie = 0.5f*(vi + vj + L0[e] * ((vi - vj)/Vector3.Distance(vi,vj)));
			Vector3 xje = 0.5f*(vi + vj + L0[e] * ((vj - vi)/Vector3.Distance(vj,vi)));
			temp_x [i] = temp_x[i] + xie; 
			temp_n [i]++;
			temp_x [j] = temp_x[j] + xje; 
			temp_n [j]++;
		}
		//weighted average and assign new positions and velocities
		for (int v = 0; v < 10; v++) {
			Vector3 temp = (0.2f * vertices [v] + temp_x [v])/(0.2f + temp_n[v]);
			velocities[v] = velocities[v] + (temp - vertices[v])/t;
			vertices [v] = temp;
		}
		for (int v = 11; v < vertices.Length-1; v++) {
			Vector3 temp = (0.2f * vertices [v] + temp_x [v])/(0.2f + temp_n[v]);
			velocities[v] = velocities[v] + (temp - vertices[v])/t;
			vertices [v] = temp;
		}

    
	}

	//Failed attempt at doing collision handling with the pin
	void Collision_Handling(Vector3[] vertices)
	{
		//Detect if all veritices except 10 and last are within the radius of the cylinder. Fix if so
		for (int v = 0; v < 10; v++) {
			if (vertices [v].x < pin.transform.position.x + 0.1f && vertices [v].x > -pin.transform.position.x - 0.1f) {
				//vertices [v].x += 0.05f;
			}
			if (vertices [v].z + 100 < pin.transform.position.z + 0.1f && vertices [v].z + 100 > -pin.transform.position.z - 0.1f) {
				//vertices [v].z += 0.05f;
			}

		}
		for (int v = 11; v < vertices.Length-1; v++) {
			if (vertices [v].x < pin.transform.position.x + 0.1f && vertices [v].x > -pin.transform.position.x - 0.1f) {
				print ("Hit");
				//vertices [v].x += 0.05f;
			}
			if (vertices [v].z + 100 < pin.transform.position.z + 0.1f && vertices [v].z + 100 > -pin.transform.position.z - 0.1f) {
				print ("Hit");
				//vertices [v].z += 0.05f;
			}
		}
	}

	// Update is called once per frame
	void Update () 
	{
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		Vector3[] vertices = mesh.vertices;
		//Create a gravity vector force in the y direction
		Vector3 g = new Vector3(0, 0, -(t * -9.8f * damping));
		windGusts = wind.GetComponent<WindScript> ().windGusts;

		// Apply gravity and wind to vertices (except 0 and 10)
		for (int v = 0; v < 10; v++) {
			velocities [v] = velocities [v] + g + windGusts; //add wind to vertices
			vertices [v] = vertices [v] + t * velocities [v];
		}
		for (int v = 11; v < vertices.Length-1; v++) {
			velocities [v] = velocities [v] + g + windGusts;
			vertices [v] = vertices [v] + t * velocities [v];
		}

		// Strain Limiting - modified to accept vertices as an argument
		// done 64 times as discussed in class
		for (int i = 0; i < 64; i++) {
			Strain_Limiting (vertices);
		}



		//Collision_Handling (vertices);

		//Assign new positions of vertices to mesh
		mesh.vertices = vertices;
		mesh.RecalculateNormals ();

	}
}
