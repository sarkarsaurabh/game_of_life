using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class world : MonoBehaviour {

	public GameObject ant_lamp;
	public float[,] world_score;
	public float[,] cheese;
	public float[,] rocks;
	public GameObject[] gos;
	public GameObject rocks_obj;
	public GameObject heatmap_obj;
	public GameObject cheese_obj;
	public GameObject[] heatmap;
	public GameObject[] cheese_coll;
	public float home_stock;
	
	
	public int start_end = 0;
	// Use this for initialization
	void Start () {

		GameObject ground = GameObject.Find("floor");
        var moveAreaX = ground.GetComponent<Renderer>().bounds.size.x - 1;
        var moveAreaZ = ground.GetComponent<Renderer>().bounds.size.z - 1;


		world_score = new float[100,100];
		cheese = new float[100,100];
		rocks = new float[100,100];
		//print(world_score);
		//print("world created");

		for (int i=0;i <= 100; i++)
		{
			var x = (int) Random.Range(0,100);
			var z = (int) Random.Range(0,100);

			var scale_x = (float) Random.Range(1f,5f);
			var scale_y = (float) Random.Range(2f,7f);
			var scale_z = (float) Random.Range(1f,5f);

			rocks[x,z] += 1;
			Vector3 position = new Vector3(x*moveAreaX/100 - moveAreaX/2,0,z*moveAreaZ/100 - moveAreaZ/2);
       		//print(position);
       		//print(x);
       		//print(z);
			GameObject g = Instantiate(rocks_obj, position, Quaternion.identity) as GameObject;
			g.transform.localScale = new Vector3(scale_x, scale_y, scale_z);
			
		}

		

		var cheese_cnt = 5;

		cheese_coll = new GameObject[100*100];
		for (int ix=0;ix<100;ix++){
        	for (int iz=0;iz<100;iz++){

        		Vector3 position = new Vector3((ix - moveAreaX/2)*100/moveAreaX,0,(iz - moveAreaZ/2)*100/moveAreaZ );
       			cheese_coll[ix + iz*100]  = Instantiate(cheese_obj, position, Quaternion.identity) as GameObject;
				
				var rend = cheese_coll[ix + iz*100].GetComponent<Renderer>();
        		rend.enabled = false;
        	}
        }

		for (int i=0;i < cheese_cnt; i++)
		{
			var x = (int) Random.Range(5,90);
			var z = (int) Random.Range(5,90);

			for (int dx =-1; dx<=1; dx++){
			for (int dz = -1; dz <=1; dz ++){

					cheese[x+dx,z+dz] = 1;
					var rend = cheese_coll[x+dx + (z+dz)*100].GetComponent<Renderer>();
        			rend.enabled = true;


			}
		}
			
		}

		gos = new GameObject[25];
		for(int i = 0; i < gos.Length; i++)
         {
         	Vector3 position = new Vector3(Random.Range(-20.0f, 20.0f), 0, Random.Range(-5.0f, 5.0f));
       
			gos[i]= Instantiate(ant_lamp, position, Quaternion.identity) as GameObject;
			var bite = gos[i].transform.Find("bite").GetComponent<Renderer>() ;
        		bite.enabled = false;
        }

        heatmap = new GameObject[100*100];
        for (int ix=0;ix<100;ix++){
        	for (int iz=0;iz<100;iz++){

        		Vector3 position = new Vector3((ix - moveAreaX/2)*100/moveAreaX,0,(iz - moveAreaZ/2)*100/moveAreaZ );
       			heatmap[ix + iz*100]  = Instantiate(heatmap_obj, position, Quaternion.identity) as GameObject;
				heatmap[ix + iz*100].GetComponent<Renderer>().material.color = new Color (41/255f,137/255f,0f,1.0f);
				
				var rend = heatmap[ix + iz*100].GetComponent<Renderer>();
        		rend.enabled = false;
        		
        	}
        }

        // Mark home base:
        for (int ix=0;ix<10;ix++){
        	for (int iz=0;iz<10;iz++){

        		heatmap[ix + iz*100].GetComponent<Renderer>().material.color = new Color (101/255f,121/255f,200/255f,1.0f);
			
				var rend = heatmap[ix + iz*100].GetComponent<Renderer>();
        		rend.enabled = true;
        	}
        }



    start_end = 1;
		
	}
	
	// Update is called once per frame
	void Update () {

		if (start_end == 1){

		for (int ix=0;ix<100;ix++){
        	for (int iz=0;iz<100;iz++){

        		world_score[ix,iz] -= 0.00005f;
					if (world_score[ix,iz] < 0){
						world_score[ix,iz] = 0f;
					}
        		
        		if (world_score[ix,iz]>0) {
        			if (ix>=10 || iz >=10){
		        		var rend = heatmap[ix + iz*100].GetComponent<Renderer>();
		        		rend.enabled = true;
						heatmap[ix + iz*100].GetComponent<Renderer>().material.color = new Color (world_score[ix,iz],137/255f,0f,1.0f);
				}
				}
				else{
				// heatmap[ix + iz*100].GetComponent<Renderer>().material.color = new Color (0f,0f,0f,1.0f);
					if (ix>=10 || iz >=10){

					var rend = heatmap[ix + iz*100].GetComponent<Renderer>();
	        		rend.enabled = false;
					}

					
			}
        	}
        }
		
	}
}
}
