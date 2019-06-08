using UnityEngine;
using System.Collections;


/*
 *	ターゲットクラス
 *	Maruchu
 *
 *	Bulletクラスを持った GameObject が接触すると破壊される
 *	すべてのターゲットが破壊されるとステージクリアとする
 */
public class Target				: MonoBehaviour {
	
	
	
	
	public							GameObject	hitEffectPrefab			= null;						//ヒットエフェクトのプレハブ
	
	private	static					int			m_allTargetNum			= 0;						//ステージに配置されているターゲットの数
	
	
	
	/*
	 *	起動時に呼び出される関数
	 */
	private		void	Awake() {
		//ターゲットの総数を覚える
		m_allTargetNum++;
	}
	
	
	
	
	/*
	 *	Collider が何かにヒットしたら呼ばれる関数
	 *
	 *	自分の GameObject に Collider(IsTriggerをつける) と Rigidbody をつけると呼ばれるようになります
	 */
	private		void	OnTriggerEnter( Collider hitCollider) {
		
		//相手の GameObject を取得
		GameObject	hitObject	= hitCollider.gameObject;
		
		//相手は弾？
		if( null==hitObject.GetComponent<Bullet>()) {
			//弾ではなかったので無視
			return;
		}
		
		
		//破壊されたオブジェクトの処理
		
		//ヒットエフェクトがあるか？
		if( null!=hitEffectPrefab) {
			//自分と同じ位置でヒットエフェクトを出す
			Instantiate( hitEffectPrefab, transform.position, transform.rotation);
		}
		
		//ステージクリアのチェック
		{
			//ターゲットの総数から自分の分を削除
			m_allTargetNum--;
			
			//もしターゲットの数が 0 になったらステージクリア
			if( m_allTargetNum <= 0) {
				
				//ステージクリアにする
				Game.SetStageClear();
			}
		}
		
		//この GameObject をヒエラルキーから削除
		Destroy( gameObject);
	}
	
	
	
	
}
