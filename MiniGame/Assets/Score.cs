using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

	public Text scoreText;
	//public Text highscoreText;
	private int score;
	//private int highscore;

	private float timeElapsed;

	// Use this for initialization
	void Start () {
		score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= 3) {
			score += 10;
			timeElapsed = 0;
		}
		scoreText.text = score.ToString ();
	}
}
