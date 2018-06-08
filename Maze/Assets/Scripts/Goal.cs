using UnityEngine;
using System.Collections;


/*
 *	ゴールクラス
 *	Maruchu
 *
 *	Playerクラスを持った GameObject が接触するとステージクリアになる
 */
public		class		Goal				: MonoBehaviour {
	
	
	
	
	/*
	 *	Collider が何かにヒットしたら呼ばれる関数
	 *
	 *	自分の GameObject に Collider(IsTriggerをつける) と Rigidbody をつけると呼ばれるようになります
	 */
	private		void	OnTriggerEnter( Collider hitCollider) {
		
		//相手の GameObject を取得
		GameObject	hitObject	= hitCollider.gameObject;
		
		//Playerクラスを持った GameObject が接触するしたらゴールにする
		if( null==hitObject.GetComponent<Player>()) {
			//プレーヤーではなかったので無視
			return;
		}
		
		//ステージクリアにする
		Game.SetStageClear();
	}
	
	
	
	
}
