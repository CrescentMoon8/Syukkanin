// ---------------------------------------------------------
// StageArrayData.cs
//
// 作成日:2023/10/24
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// ステージの初期化、オブジェクトの取得を行う
/// </summary>
public class StageArrayData : MonoBehaviour
{
    #region 変数

    #region タグリスト
    /// <summary>
    /// StageObjectのタグ一覧
    /// １　空
    /// ２　動かせないブロック
    /// ３　動かせるブロック
    /// ４　プレイヤー
    /// ５　ターゲット
    /// </summary>
    private string[] _stageObjectTagList =
	{
		"", "StaticBlock", "MoveBlock", "Player", "TargetPosition"
	};
    #endregion

    #region 親オブジェクト
    [Header("ステージの親オブジェクトを入れる")] [SerializeField] 
	private GameObject _rootObject = default;
    #endregion

    #region ステージサイズ
    [Header("ステージの横の最大サイズ")] [SerializeField] 
	private int _horizontalMaxSize = default;
	[Header("ステージの縦の最大サイズ")] [SerializeField] 
	private int _verticalMaxSize = default;
    #endregion
    #endregion

    #region プロパティ
	// プレイヤーの座標
    public Vector2 PlayerPosition { get; set; }
	// ステージの情報を入れるための配列
	public int[,] StageArray { get; set; }
	// ターゲットの情報を入れるための配列
	public int[,] TargetData { get; set; }
	// ステージの横の最大サイズ
	public int HorizontalMaxSize { get { return _horizontalMaxSize; } }
	// ステージの縦の最大サイズ
    public int VerticalMaxSize { get { return _verticalMaxSize; } }

	#endregion

	#region メソッド
	/// <summary>
	/// 配列内の初期化処理
	/// </summary>
	private void Awake()
	{
		// マップの最大サイズを設定する
		SetStageMaxSize();
		// ステージ、ターゲットの配列の大きさを設定する
		// 座標と配列の大きさのずれをなくすために＋１する
		StageArray = new int[VerticalMaxSize + 1, HorizontalMaxSize + 1];
		TargetData = new int[VerticalMaxSize + 1, HorizontalMaxSize + 1];
		// マップイメージを配列に格納する
		ImageToArray();
	}

	/// <summary>
	/// 配列内を確認するための出力処理
	/// </summary>
	private void Update ()
	{
		//テスト用
		if(Input.GetKeyDown(KeyCode.H))
        {
            //配列を出力する
            print("Field--------------------------------------------");
            for (int y = 0; y < VerticalMaxSize + 1; y++)
            {
                string outPutString = "";
                for (int x = 0; x < HorizontalMaxSize + 1; x++)
                {
                    outPutString += StageArray[y, x];
                }
                print(outPutString);
            }
            print("Field--------------------------------------------");
        }
	}

	/// <summary>
	/// マップの最大サイズを設定する
	/// </summary>
	private void SetStageMaxSize()
    {
		// rootObject内の全てのオブジェクトを検索する
		foreach(Transform stageObject in _rootObject.transform)
        {
			// 各ブロックの座標を取得する（y座標はマイナスのため、マイナスをつけて逆転させる）
			int positionX = Mathf.FloorToInt(stageObject.position.x);
			int positionY = Mathf.FloorToInt(-stageObject.position.y);

			// 座標と現在の横の最大サイズを比較する
			if(HorizontalMaxSize < positionX)
            {
				// 横の最大サイズを設定する
				_horizontalMaxSize = positionX;
            }

			// 座標と現在の縦の最大サイズを比較する
			if (VerticalMaxSize < positionY)
            {
				// 縦の最大サイズを設定する
				_verticalMaxSize = positionY;
            }
        }
    }

	/// <summary>
	/// マップイメージを配列に格納する
	/// </summary>
	private void ImageToArray()
    {
		// rootObject内の全てのオブジェクトを検索する
		foreach (Transform stageObject in _rootObject.transform)
        {
            // 各ブロックの座標を取得する（y座標はマイナスのため、マイナスをつけて逆転させる）
            int col = Mathf.FloorToInt(stageObject.position.x);
			int row = Mathf.FloorToInt(-stageObject.position.y);

			/*
			 * 各オブジェクトのタグを判定し、
			 * ステージの配列にそれぞれ値を代入する
			 */
			if(_stageObjectTagList[ConstantForGame.STATIC_BLOCK].Equals(stageObject.tag))
            {
				StageArray[row, col] = ConstantForGame.STATIC_BLOCK;
            }
			else if (_stageObjectTagList[ConstantForGame.MOVE_BLOCK].Equals(stageObject.tag))
			{
				StageArray[row, col] = ConstantForGame.MOVE_BLOCK;
			}
			else if (_stageObjectTagList[ConstantForGame.PLAYER].Equals(stageObject.tag))
			{
				StageArray[row, col] = ConstantForGame.PLAYER;

				// プレイヤーの座標を代入する
				PlayerPosition = new Vector2(row, col);
			}
			else if (_stageObjectTagList[ConstantForGame.TARGET_AREA].Equals(stageObject.tag))
			{
				StageArray[row, col] = ConstantForGame.TARGET_AREA;
			}
		}

		// ステージの配列情報をターゲット判定用の配列へコピーする
		TargetData = (int[,])StageArray.Clone();
    }

	/// <summary>
	/// ステージにあるオブジェクトを取得する
	/// </summary>
	/// <param name="tagId">取得するオブジェクトを指定</param>
	/// <param name="row"></param>
	/// <param name="col"></param>
	/// <returns></returns>
	public GameObject GetStageObject(int tagId, int row, int col)
    {
		// rootObject内の全てのオブジェクトを検索する
		foreach (Transform stageObject in _rootObject.transform)
        {
			// タグIDが-1以外かつオブジェクトのタグがタグリストになかったら
			if(tagId != -1 && stageObject.tag != _stageObjectTagList[tagId])
            {
				// 検索を終了する
				continue;
            }

			// オブジェクトの座標が渡された引数と同じだったら
			if(stageObject.position.x == col && stageObject.position.y == -row)
            {
				// オブジェクトを変えす
				return stageObject.gameObject;
            }
        }

		return null;
    }
	#endregion
}