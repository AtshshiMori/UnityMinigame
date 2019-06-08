using UnityEngine;
using System.Collections;




/*
 *	ゲームの情報を管理するクラス
 *	Maruchu
 */
public class Game				: MonoBehaviour {
	
	
	
	
	private	static		bool		m_stageClearFlag		= false;	//これが true ならステージクリアとする
	
	/*
	 *	ステージクリアしたら呼ばれる
	 */
	public	static		void		SetStageClear() {
		//ステージクリアにする
		m_stageClearFlag		= true;
	}
	
	/*
	 *	ステージクリアしたかどうか確認
	 */
	public	static		bool		IsStageCleared() {
		return	m_stageClearFlag;
	}
	
	
	
	
}
