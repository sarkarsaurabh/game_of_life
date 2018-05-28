using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ant : MonoBehaviour {
	public Vector3 target;
    public float speed;
    
    public float moveAreaX;
    public float moveAreaZ;
    public float internal_clock;

    public int carry_cheese;
    public float [,] path_matrix;
    public float time_resetl;
    public float crowding;

    public Material normal_mat;
    public Material angry_mat;
    public int crowded_flg;
    public int crowded_path_flg;
    public GameObject marker;
    public float explorer_seed = 0;
    public GameObject cheese_drop;



	// Use this for initialization
	void Start () {
		
		speed = 3.0f + Random.Range(-0.2f, 0.2f);
		
		GameObject ground = GameObject.Find("floor");
        moveAreaX = ground.GetComponent<Renderer>().bounds.size.x;
        moveAreaZ = ground.GetComponent<Renderer>().bounds.size.z;
        internal_clock = 0.0f;

        carry_cheese = 0;
        path_matrix = new float[3,3];
        time_resetl = Random.Range(5.0f,10.0f);
        crowding = 0f;
        crowded_flg = 0;
        crowded_path_flg = 0;
        explorer_seed = Random.Range(0f,1f);
        
	}
	
	 GameObject GetClosestEnemy (GameObject[] enemies, float DistT)
    {
        GameObject bestTarget = null;

        
			 	
        
        float closestDistanceSqr = DistT;
        Vector3 currentPosition = transform.position;
        foreach(GameObject potentialTarget in enemies)
        {
            var rend = potentialTarget.GetComponent<Renderer>();
        	if (rend.enabled) {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }
        }
     
        return bestTarget;
    }

    Vector3 GetClosestEnemy_pos (List<Vector3> enemies, float DistT)
    {
        Vector3 bestTarget = new Vector3(-1,0,-1);;

        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach(Vector3 potentialTarget in enemies)
        {
            
            Vector3 directionToTarget = potentialTarget - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if(dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        
        }
     
        return bestTarget;
    }


	// Update is called once per frame
	void Update () {

		// var navpath = this.GetComponent<UnityEngine.AI.NavMeshAgent>().path;
		// if (navpath.status == UnityEngine.AI.NavMeshPathStatus.PathInvalid || navpath.status == UnityEngine.AI.NavMeshPathStatus.PathPartial)
		// {
		// 	print("Path unreachable");
		// }
   // Target is unreachable


		crowding -= 0.01f;

		var changeSlider =  GameObject.Find ("changeSlider").GetComponent <Slider> ().value;

		if (explorer_seed < changeSlider){
			if (crowding > 0.8){
				
				// this.GetComponent<Renderer>().material = angry_mat;
				crowded_flg = 1;

				
			}
			else{
				this.GetComponent<Renderer>().material = normal_mat;
				if (crowding < 0.5){
					crowded_flg = 0;
					crowded_path_flg = 0;
				}
				
			}
		}

		var random_path = 0;

		if (internal_clock < 0) {
		random_path = 1;
		internal_clock = time_resetl;
		}
		else {
			internal_clock = internal_clock - Time.deltaTime;
		}


		// Find current position
		GameObject Cam = GameObject.Find("Main Camera");
		world obj = (Cam.GetComponent("world") as world);
		int worldx =  (int) (((transform.position.x + moveAreaX/2 )/ moveAreaX) * 100);
		int worldz =  (int) (((transform.position.z + moveAreaZ/2) / moveAreaZ) * 100);

		// print("world");
		// print(worldx);
		// print(worldz);
		
		// Check food cell
		if (carry_cheese == 0){

			// Check for cheese block
			if (obj.cheese[worldx,worldz] > 0){
				carry_cheese = 1;
				var bite = transform.Find("bite").GetComponent<Renderer>() ;
        		bite.enabled = true;

				// Lower cheese reserve
				obj.cheese[worldx,worldz] -= 0.5f;
				// print(obj.cheese[worldx,worldz]);
				//Check if cheese reserve finished. remove asset.
				if (obj.cheese[worldx,worldz] <= 0){

						obj.cheese[worldx,worldz] = 0;
						var rend = obj.cheese_coll[worldx + worldz*100].GetComponent<Renderer>();
	        			rend.enabled = false;

				}
			}
			// Check for best option
			
			path_matrix[1,1] = -1 ;//center
			var max_val = 0f;
			var max_x = 0;
			var max_z = 0;

			// Find the nearest cheese
			var DistT = (float) 10*moveAreaZ/100;
			GameObject cheese_target1 = GetClosestEnemy(obj.cheese_coll, DistT);

			if (cheese_target1 != null && crowded_flg == 0){
				UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
          		agent.destination = cheese_target1.transform.position;
			}
			else if(crowded_flg == 0) {
				// Look for pheromons
				var scan_t = 11;
				//Vector3[] cand_heatmap = new Vector3[scan_t*scan_t];
				List<Vector3> cand_heatmap = new List<Vector3>();


				var heatmap_T = (int) Mathf.Floor(scan_t / 2); // Always odd
				float high_heat = -1f;
				int max_x1 = 0;
				int max_z1 = 0;
				int cand_heat = 0;

				for (int dx = 0; dx <= heatmap_T; dx++){
					for (int dz = 0; dz <= heatmap_T; dz ++){
						if (dx != 0 && dz != 0){
						// var rend = obj.heatmap[worldx + dx + (worldz + dz) * 100].GetComponent<Renderer>();
        				var c_z = worldz + dz;
        				var c_x = worldx + dx;

        				if (c_z > 99){
        					c_z = 99;
        				}
        				if (c_x > 99){
        					c_x = 99;
        				}

        				if (c_z < 0){
        					c_z = 0;
        				}
        				if (c_x < 0){
        					c_x = 0;
        				}

        				if (obj.world_score[c_x , c_z] > 0) {	
        								//cand_heatmap[cnt] = obj.heatmap[worldx + dx + (worldz + dz) * 100].transform.position;
        								//cand_heatmap.Add(obj.heatmap[worldx + dx + (worldz + dz) * 100].transform.position);
        						cand_heat += 1;
        						if (obj.world_score[c_x , c_z] > high_heat){
        							high_heat = obj.world_score[c_x , c_z];
        							max_x1 = c_x;
        							max_z1 = c_z;

        						}
        					}

						
        				}

					}
				}
				 if (high_heat > 0.0 )
				{
					// print("heatmap");
				var targetX1 = max_x1*moveAreaX/100 - moveAreaX/2;
				var targetZ1 = max_z1*moveAreaZ/100 - moveAreaZ/2;

				Vector3 target1 = new Vector3(targetX1,0,targetZ1);
				UnityEngine.AI.NavMeshAgent agent1 = GetComponent<UnityEngine.AI.NavMeshAgent>();
				agent1.destination = target1;
				// this.GetComponent<Renderer>().material = angry_mat;
				//GameObject g = Instantiate(marker, target1, Quaternion.identity) as GameObject;

				}
					


				
				// var DistT1 = (float) 10*moveAreaZ/100;

				// Vector3 target_heatmap = GetClosestEnemy_pos(cand_heatmap, DistT1);
				
				// if (target_heatmap != new Vector3(-1,0,-1))
				// {
				// UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    //       		agent.destination = target_heatmap;
    //       		//print("Nearest heatmap");
    //       		}
          		
          			else if (random_path == 1){
		          		// Random move
		          		var targetZ = (((Random.Range(0,100))-moveAreaZ/2)*100/moveAreaZ);
					 	var targetX = (((Random.Range(0,100))-moveAreaX/2)*100/moveAreaX);
					 	Vector3 target = new Vector3(targetX,0,targetZ);
		       		
			        	// transform.position = Vector3.MoveTowards(transform.position, target, step);
			        	UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			          	agent.destination = target;
			          }
          		

			}
			else if (crowded_path_flg == 0){

				// wander off when crowded
				var targetZ = (((Random.Range(0,100))-moveAreaZ/2)*100/moveAreaZ);
					 	var targetX = (((Random.Range(0,100))-moveAreaX/2)*100/moveAreaX);
					 	Vector3 target = new Vector3(targetX,0,targetZ);
		       		
			        	// transform.position = Vector3.MoveTowards(transform.position, target, step);
			        	UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			          	agent.destination = target;
			     crowded_path_flg = 1;


			}


        

		}

		else{ // Carrying cheese

			path_matrix[1,1] = -1 ;//center
			var max_val = 0f;
			var max_x = 0;
			var max_z = 0; 

			// Check for home block
			if (worldx<10 && worldz<10){
				carry_cheese = 0;
				obj.home_stock += 0.1f;
				var bite = transform.Find("bite").GetComponent<Renderer>() ;
        		bite.enabled = false;

        		Vector3 position = new Vector3(-moveAreaX/2 + Random.Range(0,10),2,-moveAreaZ/2 + Random.Range(0,10)) ;
       		
				GameObject c = Instantiate(cheese_drop, position, Quaternion.identity) as GameObject;
				Rigidbody rb = c.GetComponent<Rigidbody>();
				rb.AddRelativeForce(Vector3.forward * 1f);

			}

			obj.world_score[worldx,worldz] += 0.01f;
			if (obj.world_score[worldx,worldz] > 1.0f) {
				obj.world_score[worldx,worldz] = 1.0f;
			}


			var targetZ = ((Random.Range(1,9)-moveAreaZ/2)*100/moveAreaZ);
			var targetX = ((Random.Range(1,9)-moveAreaX/2)*100/moveAreaX);

			Vector3 target = new Vector3(targetX,0,targetZ);
			UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			agent.destination = target;			


		}

		// look at surrunding block to find next step



  
	}

	private void OnCollisionEnter(Collision col)
{
    if (col.gameObject.tag == "ant"  )
    {
            this.transform.parent = col.gameObject.transform;
            
            // GameObject.Find("Controller").GetComponent<FixedJoint>().connectedBody = null;      
    }
    if (col.gameObject.tag == "wall"  )
    {		
    		target = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            
            transform.position = Vector3.MoveTowards(transform.position, target , speed * Time.deltaTime);
             
    }

}

	void OnTriggerStay(Collider col)
	{
		if (col.gameObject.tag == "ant"  )
    	{
            
            crowding += 0.01f;
            if (crowding > 1.0f){
            	crowding = 1.0f;
            	// print("maxed");
            }
            // print("Crowded");
            // GameObject.Find("Controller").GetComponent<FixedJoint>().connectedBody = null;      
    	}
	}

}
