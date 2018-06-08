using UnityEngine;
using System.Collections;

public class DispMsg : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public static int		lengthMsg;			//表示する文字列の長さ
	public static bool		flgDisp = false;	//表示フラグ
	public static float		waitTime = 0;
	
	float nextTime = 0;
	
	// Update is called once per frame
	void Update () {
		
		//メセージ表示状態時のみ処理を行う
		if(flgDisp == true){
			
			if(Time.time > nextTime) {			//次の文字の表示時間を越えたら
			
				if(lengthMsg < dispMsg.Length){	//文字の長さが最大でなければ
				
					lengthMsg ++;					//次の文字へ
				}			
			
				nextTime = Time.time + 0.01f;	//次の文字の表示間隔
			}
			
			if(lengthMsg >= dispMsg.Length){		//メッセージを全部表示していたら
			
				waitTime += Time.deltaTime;
					
				if(waitTime > dispMsg.Length / 4) {	//しばらく待つ　※メッセージの長さによって可変
				
					flgDisp = false;		//非表示
				}
			}
		}
	
	}
	
	public static string	dispMsg;
	
	public static void dispMessage (string msg) {
		
		dispMsg = msg;		//メッセージを代入
		lengthMsg = 0;		//０文字目にリセット
		flgDisp = true;		//表示
		waitTime = 0;
	}
	
	public GUIStyle msgWnd;
	
	void OnGUI () {
	
		//基準となる画面の幅
		const float screenWidth = 1136;
		
		//基準サイズに対するウインドウサイズと座標
		const float msgwWidth	= 800;
		const float msgwHeight	= 200;
		const float msgwPosX	= (screenWidth - msgwWidth) / 2;
		const float msgwPosY	= 390;
		
		//画面の幅から１ピクセルを算出
		float factorSize = Screen.width / screenWidth;
		
		float msgwX;
		float msgwY;
		float msgwW = msgwWidth * factorSize;
		float msgwH = msgwHeight * factorSize;
		
		//フォントのスタイル
		GUIStyle myStyle = new GUIStyle();
		myStyle.fontSize = (int)(30 * factorSize);
		
		//メセージ表示
		if(flgDisp == true){
			
			//ウィンドウ
			msgwX = msgwPosX * factorSize;
			msgwY = msgwPosY * factorSize;
			GUI.Box(new Rect(msgwX,msgwY,msgwW,msgwH),"ウインドウ",msgWnd);
			
			//メッセージ影用
			myStyle.normal.textColor = Color.black;
			
			msgwX = (msgwPosX + 22) * factorSize;
			msgwY = (msgwPosY + 22) * factorSize;
			GUI.Label(new Rect(msgwX,msgwY,msgwW,msgwH),dispMsg.Substring(0, lengthMsg),myStyle);
			
			//メッセージ
			myStyle.normal.textColor = Color.white;
			
			msgwX = (msgwPosX + 20) * factorSize;
			msgwY = (msgwPosY + 20) * factorSize;
			GUI.Label(new Rect(msgwX,msgwY,msgwW,msgwH),dispMsg.Substring(0, lengthMsg),myStyle);
		}
		
	}
}
