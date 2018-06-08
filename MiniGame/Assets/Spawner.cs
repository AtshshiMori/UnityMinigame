using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject wallPrefab;
	public GameObject wallThornUnderPrefab;
	public GameObject wallThornTopPrefab;
	public float interval;
	public float range = 3.0f;
	private int wallnum;

	// Use this for initialization
	IEnumerator Start () {
		GameObject[] walls = { wallPrefab, wallThornUnderPrefab, wallThornTopPrefab};
		while (true) {
			wallnum = Random.Range (0, walls.Length);

			transform.position = new Vector3 (transform.position.x, Random.Range (-range, range), transform.position.z);
			Instantiate (walls[wallnum], transform.position, transform.rotation);
			yield return new WaitForSeconds (interval);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
