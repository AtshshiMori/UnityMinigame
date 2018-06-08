using UnityEngine;
using System.Collections;




/*
 *	汎用のくるくる回るオブジェクト用スクリプト
 *	Maruchu
 */
public		class		Round				: MonoBehaviour {



	private							float		rotationNow				= 0f;						//回転量のログ
	
	public							float		rotationAdd				= 90f;						//1秒間に回転する量
	
	
	
	/*
	 *	毎フレーム呼び出される関数
	 */
	private		void	Update() {
		
		//回転量を加算
		rotationNow			+= (rotationAdd	*Time.deltaTime);			//移動量、回転量には Time.deltaTime をかけて実行環境(フレーム数の差)による違いが出ないようにします
		
		//オイラー角で入れる
		transform.rotation	= Quaternion.Euler( 0, rotationNow, 0);		//Y軸回転でオブジェクトの向きを横に動かせます
	}
	
	
	
	
}
