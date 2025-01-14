using UnityEngine;
using System.Collections;


/*
 *	自動迷路生成プログラム
 *	Maruchu
 *
 *	MAZE_LINE_X、MAZE_LINE_Yで指定したサイズの迷路を自動的に作成するプログラム
 */
public class Field : MonoBehaviour {
	
	
	
	
	
	public  GameObject	m_blockObject			= null; 	//迷路を構成するブロックのオブジェクト
	
	public  GameObject	m_playerObject			= null; 	//操作するプレーヤーキャラ
	
	public  GameObject	m_goalObject			= null; 	//ゴールのオブジェクト
	public  GameObject	m_targetObject			= null; 	//破壊するターゲットのオブジェクト
	
	public  GameObject	m_stageClearObject		= null; 	//ステージクリア時に生成するオブジェクト
	
	
	public  bool		m_createAtOnce			= true; 	//true なら迷路を一気に作る、false なら生成途中を見せる
	
	
	
	//ステージの種類
	public	enum	StageClear {
		Goal		//ゴールを目指す
		,Target		//ターゲットを全部壊す
	}
	
	public  StageClear	m_stageClear			= StageClear.Goal;		//ステージクリアの種類
	
	
	
	
	
	
	//チェック方向
	private	enum	CheckDir {
		//← ↑ → ↓ の順番
		Left		//左
		,Up			//上
		,Right		//右
		,Down		//下
		,EnumMax	//最大数
		
		,None		= -1
	}
	//チェック情報
	private	enum	CheckData {
		X			//X軸
		,Y			//Y軸
		,EnumMax	//最大数
	}
	
	
	private		static readonly		int[][]		CHECK_DIR_LIST			= new int[ (int)CheckDir.EnumMax][] {		//チェック方向
		//  		 X		 Y
		new int[ (int)CheckData.EnumMax] {		-1,		 0		}
		,new int[ (int)CheckData.EnumMax] {		 0,		-1		}
		,new int[ (int)CheckData.EnumMax] {		 1,		 0		}
		,new int[ (int)CheckData.EnumMax] {		 0,		 1		}
	};
	private		static readonly		CheckDir[]	REVERSE_DIR_LIST		= new CheckDir[ (int)CheckDir.EnumMax] {	//チェック方向の反対側
		CheckDir.Right
		,CheckDir.Down
		,CheckDir.Left
		,CheckDir.Up
	};
	private		static readonly		CheckDir[]	CHECK_ORDER_LIST		= new CheckDir[ (int)CheckDir.EnumMax] {	//チェックする順番
		CheckDir.Up
		,CheckDir.Down
		,CheckDir.Left
		,CheckDir.Right
	};
	
	
	private		static readonly		int			MAZE_LINE_X = 8;  		//迷路のX通路数
	private		static readonly		int			MAZE_LINE_Y = 8;  		//迷路のY通路数
	
	private		static readonly		int			MAZE_GRID_X = ((MAZE_LINE_X	*2)		+1); //迷路のX配列数
	private		static readonly		int			MAZE_GRID_Y = ((MAZE_LINE_Y	*2)		+1); //迷路のY配列数
	
	private		static readonly		int			EXEC_MAZE_COUNT_MAX		= (MAZE_LINE_X *MAZE_LINE_Y /2);			//ブロックをじょじょに生成する時の試行回数
	
	private		static readonly		float		MAZE_BLOCK_SCALE		= 2.0f;  		//迷路のスケール(ブロック1つ分のサイズ)
	private		static readonly		float		MAZE_BLOCK_SCALE_HEIGHT		= 6.0f;		//迷路のスケール（ブロックの高さ）
	private		static readonly		int			TARGET_NUM = 5;  		//破壊するターゲットの数
	
	
	
	private  bool[][]	m_mazeGrid = null;  		//迷路の配列
	
	private  GameObject	m_blockParent			= null;  		//迷路のブロックを覚えておく親
	
	
	private  int			m_makeMazeCounter		= 0;  		//ブロックをじょじょに生成する時のカウンタ
	
	
	
	private  bool		m_stageClearedFlag		= false;  	//ステージクリアのオブジェクトを生成したら true
	
	
	
	
	
	
	
	/*
	 *	起動時に呼び出される関数
	 */
	private		void	Awake() {
		
		//迷路の初期化
		InitializeMaze();
		
		//一気に迷路を作る？
		if( m_createAtOnce) {
			//上下左右の端から中心に向かって枝を伸ばして迷路を生成する
			int		i;
			for( i=0; i<EXEC_MAZE_COUNT_MAX; i++) {		//上下左右からチェックするので半分までで良い
 ExecMaze();
			}
			
			//迷路を生成
			CreateMaze();
		}
		
		//プレーヤーを生成
		CreatePlayer();
		
		//ゲームクリアの種類は何か？
		switch( m_stageClear) {
		case StageClear.Goal:
			//ゴールを生成
			CreateGoal();
			break;
		case StageClear.Target:
			//ターゲットを作成
			CreateTarget();
			break;
		}
	}
	
	/*
	 *	毎フレーム呼び出される関数
	 */
	private		void	Update() {
		
		//一気に迷路を作らない？
		if( false==m_createAtOnce) {
			
			//スペースを押すと1つ進む
			if( Input.GetKeyDown( KeyCode.Space)) {
 //生成
 ExecMaze();
 
 //迷路を更新
 CreateMaze();
			}
		}
		
		//ステージクリアの確認
		if( false==m_stageClearedFlag) {
			
			//ステージクリアしていたらフラグを立てる
			if( Game.IsStageCleared()) {
 //ステージクリアの表示を生成
 CreateStageClear();
 
 //フラグを立てる
 m_stageClearedFlag	= true;
			}
		}
	}
	
	
	
	
	
	
	
	
	
	
	
	/*
	 *	迷路の初期化
	 *	配列変数を初期化して外壁と柱を作る
	 */
	private		void	InitializeMaze() {
		
		//最初に bool の配列を作ります(これが true ならブロックを配置する)
		
		//C#の2次元配列はちょっと面倒ですが、このように宣言します
		m_mazeGrid			= new bool[		MAZE_GRID_X][];		//最初に左側の配列を宣言します
		
		//次にループで右側の配列を宣言します
		int		gridX;
		int		gridY;
		for( gridX=0; gridX<MAZE_GRID_X; gridX++) {
			m_mazeGrid[ gridX]	= new bool[ MAZE_GRID_Y];
		}
		/*
			これで以下のようなアクセスができるようになります。

			Debug.Log( "GRID["+	gridX	+"]["+	gridY	+"] = "+	m_mazeGrid[ gridX][ gridY]);
		*/
		
		//最初からブロックと決まっている場所を埋めておく
		bool	blockFlag;
		for( gridX=0; gridX<MAZE_GRID_X; gridX++) {
			for( gridY=0; gridY<MAZE_GRID_Y; gridY++) {
 //true なら、この場所はブロックにして良い
 blockFlag	= false;
 
 //	左端		上端			右端  下端
 if( (0==gridX)		||(0==gridY)		||((MAZE_GRID_X -1)==gridX)		||((MAZE_GRID_Y -1)==gridY)) {
 	//上下左右の一番 端っこは壁
 	blockFlag	= true;
 } else
 if( (0==(gridX %2))	&&(0==(gridY %2))) { //「%」は「剰余」の演算子、割った時の余りを求めてくれます(例：13 % 10 = 3)
 	//X,Yが両方とも偶数のときは柱
 	blockFlag	= true;
 }
 
 //値を代入
 m_mazeGrid[ gridX][ gridY]		= blockFlag;
			}
		}
	}
	
	
	
	/*
	 *	迷路をじょじょに生成する
	 */
	private		void	ExecMaze() {
		
		//迷路の作成は完了している
		if( m_makeMazeCounter >= EXEC_MAZE_COUNT_MAX) {
			return;
		}
		
		//今回生成するのはこの番号のブロックからチェック開始
		int			 counter	= m_makeMazeCounter;
		//カウント+1
		m_makeMazeCounter++;
		
		
		//汎用変数
		int			lineMax;			//XとYのライン数のうち大きい方を入れる
		int			start1, start2;		//チェックするときの開始位置
		
		int			gridX_A		= 0;
		int			gridY_A		= 0;
		int			gridX_B;
		int			gridY_B;
		int			gridX_C;
		int			gridY_C;
		
		CheckDir	checkDirNow;			//チェックする方向
		CheckDir	checkDirNG; //一つ前のチェック方向
		
		
		//ラインの最大を取得
		lineMax		= Mathf.Max( MAZE_LINE_X, MAZE_LINE_Y);
		
		//チェックするときの開始位置(ブロックは一個飛ばしでチェックするので ×2 する)
		start1		= ((counter		/lineMax)		*2);
		start2		= ((counter		%lineMax)		*2);
		
		
		//上下左右の端から一本ずつ枝を伸ばして壁を生成していく
		int		i;
		for( i=0; i<(int)CheckDir.EnumMax; i++){
			
			//今チェックするのはこの方向
			checkDirNow		= CHECK_ORDER_LIST[ i];
			//どの端からどの方向へ枝を伸ばす？
			switch( checkDirNow) {
			case CheckDir.Left:
 //左に枝を伸ばす(右端スタート)
 gridX_A	= ((MAZE_GRID_X -1) -start1);		//横軸は1をXに入れる
 gridY_A	= ((MAZE_GRID_Y -1) -start2);		//2はY軸
 break;
			case CheckDir.Up:
 //上に枝を伸ばす(下端スタート)
 gridX_A	= ((MAZE_GRID_X -1) -start2);		//縦軸は2をXに入れる
 gridY_A	= ((MAZE_GRID_Y -1) -start1);		//1はY軸
 break;
			case CheckDir.Right:
 //右に枝を伸ばす(左端スタート)
 gridX_A	= (                     start1);
 gridY_A	= (                     start2);
 break;
			case CheckDir.Down:
 //下に枝を伸ばす(上端スタート)
 gridX_A	= (                     start2);
 gridY_A	= (                     start1);
 break;
			default:
 //default に警告を入れておくと早期にバグが検出できて便利です
 Debug.LogError( "存在しない方向("+ checkDirNow +")");
 //適当な値
 gridX_A	= -1;
 gridY_A	= -1;
 break;
			}
			//場外チェック
			if(	(gridX_A < 0)		||(gridX_A >= MAZE_GRID_X)		||(gridY_A < 0)		||(gridY_A >= MAZE_GRID_Y)	) {
 //ここには調べるブロックがない
 continue;
			}
			
			
			//壁がある柱にぶつかるまで無限ループ
			for(;;) {
 //チェックする柱の場所(開始位置から2つ隣のブロック)
 gridX_B	= gridX_A	+(CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.X]	*2);
 gridY_B	= gridY_A	+(CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.Y]	*2);
 
 //任意のブロックの周囲を調べ、他のブロックと繋がっていないかチェック
 if( IsConnectedBlock( gridX_B, gridY_B)) {
 	
 	//すでに何かとつながっていたので、処理を中断
 	break;
 }
 
 
 //開始位置とチェック位置の間の位置にブロックを置く
 gridX_C	= gridX_A	+CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.X];
 gridY_C	= gridY_A	+CHECK_DIR_LIST[ (int)checkDirNow][ (int)CheckData.Y];
 
 //ブロックを配置
 SetBlock( gridX_C, gridY_C, true);
 
 
 //次は繋いだ柱から検索を始める
 gridX_A	= gridX_B;
 gridY_A	= gridY_B;
 
 
 //次はこっちに来てはいけない
 checkDirNG		= REVERSE_DIR_LIST[ (int)checkDirNow];
 
 //次に調べる柱をランダムに選択
 checkDirNow		= CHECK_ORDER_LIST[ Random.Range( 0, (int)CheckDir.EnumMax)];
 
 //一回前の場所に戻らないように進行方向をチェック
 if( checkDirNow==checkDirNG) {
 	//戻ろうとしたら反対側を向かせる
 	checkDirNow		= REVERSE_DIR_LIST[ (int)checkDirNow];
 }
			}
			
		}
	}
	
	
	
	/*
	 *	指定した場所にブロックが存在するか
	 */
	private		void	SetBlock( int gridX, int gridY, bool blockFlag) {
		m_mazeGrid[ gridX][ gridY]	= blockFlag;
	}
	
	/*
	 *	指定した場所にブロックが存在するか
	 *	ブロックが存在すれば true が返る
	 */
	private		bool	IsBlock( int gridX, int gridY) {
		return	m_mazeGrid[ gridX][ gridY];
	}
	
	/*
	 *	指定したブロックの上下左右にブロックがあるか調べる
	 *	何かに連結していた場合は true が返る
	 */
	private		bool	IsConnectedBlock( int gridX, int gridY) {
		
		bool	connectedFlag	= false;	//何かに連結していたら true
		
		int		checkX; //チェックするX位置
		int		checkY; //チェックするY位置
		
		//周囲をぐるっとチェックする
		int		i;
		for( i=0; i<(int)CheckDir.EnumMax; i++){
			//調べるブロックの位置
			checkX		= (gridX	+CHECK_DIR_LIST[ i][ (int)CheckData.X]);
			checkY		= (gridY	+CHECK_DIR_LIST[ i][ (int)CheckData.Y]);
			
			//場外チェック
			if(	(checkX < 0)		||(checkX >= MAZE_GRID_X)		||(checkY < 0)		||(checkY >= MAZE_GRID_Y)	) {
 //ここには調べるブロックがない
 continue;
			}
			
			//既にブロックが立ってる？
			if( IsBlock( checkX, checkY)) {
 //ブロックがあった
 connectedFlag	= true;
 //即終了
 break;
			}
		}
		
		return	connectedFlag;
	}
	
	
	
	
	
	
	
	
	
	/*
	 *	迷路をヒエラルキーに生成
	 */
	private		void	CreateMaze() {
		
		//前にブロックの親がいたら削除
		if( m_blockParent) {
			//削除
			Destroy( m_blockParent);
			//null 入れておく
			m_blockParent			= null;
		}
		
		
		//ブロックの親を作る
		m_blockParent 	= new GameObject();
		m_blockParent.name = "BlockParent";
		m_blockParent.transform.parent	= transform;
		
		
		//ブロックを作る
		GameObject	blockObject;		//ブロックをとりあえず入れておく変数
		Vector3		position;			//ブロックの生成位置
		
		int	gridX;
		int	gridY;
		for( gridX=0; gridX<MAZE_GRID_X; gridX++) {
			
			for( gridY=0; gridY<MAZE_GRID_Y; gridY++) {
 
 //ブロックある？
 if( IsBlock( gridX, gridY)) {
 	
 	//ブロックの生成位置
 	position			= new Vector3( gridX, 0, gridY)	*MAZE_BLOCK_SCALE; //UnityではXZ平面が地平線(この場合、左下から右上に進む)
 	
 	//ブロック生成  複製する対象		生成位置		回転(今回は使わない)
 	blockObject			= Instantiate(		m_blockObject,		position,		Quaternion.identity)		as GameObject;
 	//名前を変更
 	blockObject.name	= "Block("+ gridX +","+ gridY +")";		//グリッドの位置を書いておく
 	
 	//ローカルスケール(大きさ)を変更
					blockObject.transform.localScale	= new Vector3( MAZE_BLOCK_SCALE, MAZE_BLOCK_SCALE_HEIGHT, MAZE_BLOCK_SCALE);		//Vector3.one は new Vector3( 1f, 1f, 1f) と同じ
 	
 	//前述の親の下につける
 	blockObject.transform.parent		= m_blockParent.transform;
 }
			}
		}
	}
	
	/*
	 *	プレーヤーをヒエラルキーに生成
	 */
	private		void	CreatePlayer() {
		//プレーヤーを作る
		Instantiate(		m_playerObject,		new Vector3( 1, 0, 1) *MAZE_BLOCK_SCALE,		Quaternion.identity);
	}
	
	/*
	 *	ゴールをヒエラルキーに生成
	 */
	private		void	CreateGoal() {
		//ゴールはプレーヤーと逆の角に配置する
		Vector3	position	= new Vector3( (MAZE_GRID_X -2), 0, (MAZE_GRID_Y -2))	*MAZE_BLOCK_SCALE;
		//ゴールを作る
		Instantiate(		m_goalObject,		position,		Quaternion.identity);
	}
	
	/*
	 *	ターゲットをヒエラルキーに生成
	 */
	private		void	CreateTarget() {
		Vector3	position;
		int	i;
		for( i=0; i<TARGET_NUM; i++) {
			//ターゲットはランダムな場所に作る
			position	= new Vector3( (Random.Range( 0, MAZE_LINE_X)	*2) +1, 0, (Random.Range( 0, MAZE_LINE_Y)	*2) +1)		*MAZE_BLOCK_SCALE;
			//ターゲットを作る
			Instantiate(		m_targetObject,		position,		Quaternion.identity);
		}
	}
	
	
	/*
	 *	ステージクリア表示をヒエラルキーに生成
	 */
	private		void	CreateStageClear() {
		//ステージクリアのオブジェクトを生成
		Instantiate(		m_stageClearObject,		Vector3.zero,		Quaternion.identity);
	}
	
	
	
	
	
}
