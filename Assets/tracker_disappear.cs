using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tracker_disappear : MonoBehaviour {

	// Use this for initialization
	public float timer;
	void Start () {
		timer = 10f;
		
	}
	
	// Update is called once per frame
	void Update () {
			
			timer -= 0.5f;
			if (timer < 0) {
				Destroy(this.gameObject);
			}

	}
}
