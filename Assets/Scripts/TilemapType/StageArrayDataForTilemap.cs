// ---------------------------------------------------------
// StageArrayDataForTilemap.cs
//
// 作成日:2024/02/16
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class StageArrayDataForTilemap : MonoBehaviour
{
	#region 変数
	private Tilemap _tilemap = default;
	[Header("動かせないブロック")][SerializeField]
	private TileBase _staticBlockTile = default;
	[Header("動かせるブロック")][SerializeField]
	private TileBase _moveBlockTile = default;
	[Header("プレイヤー")][SerializeField]
	private TileBase _playerTile = default;
	[Header("ターゲットエリア")][SerializeField]
	private TileBase _targetAreaTile = default;

	[Header("ステージの横の最大サイズ")]
	[SerializeField]
	private int _horizontalMaxSize = default;
	[Header("ステージの縦の最大サイズ")]
	[SerializeField]
	private int _verticalMaxSize = default;
	#endregion

	#region プロパティ
	public Tilemap GettingTileMap { get { return _tilemap; } }
	// プレイヤーの座標
	public Vector2Int PlayerPosition { get; set; }
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
		_tilemap = GameObject.FindWithTag("StageMap").GetComponent<Tilemap>();
		// マップの最大サイズを設定する
		SetStageMaxSize();
		// ステージ、ターゲットの配列の大きさを設定する
		StageArray = new int[_verticalMaxSize, _horizontalMaxSize];
		TargetData = new int[_verticalMaxSize, _horizontalMaxSize];
		// マップイメージを配列に格納する
		ImageToArray();
	}

	/// <summary>
	/// 配列内を確認するための出力処理
	/// </summary>
	private void Update()
	{
		//テスト用
		if (Input.GetKeyDown(KeyCode.H))
		{
			//配列を出力する
			print("Field--------------------------------------------");
			for (int y = 0; y < _verticalMaxSize; y++)
			{
				string outPutString = "";
				for (int x = 0; x < _horizontalMaxSize; x++)
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
		_horizontalMaxSize = _tilemap.cellBounds.max.x;
		_verticalMaxSize = -_tilemap.cellBounds.min.y;
		Debug.Log(_tilemap.GetTile(new Vector3Int(0, -1)));
	}

	private void ImageToArray()
	{
        for (int i = 0; i < _verticalMaxSize; i++)
        {
            for (int j = 0; j < _horizontalMaxSize; j++)
            {
				// ワールド座標とタイルマップ座標のずれをなくすため＋１する
				// 座標と配列番号を合わせるためにマイナスをつける
				Vector3Int searchPos = new Vector3Int(j, -i);

				// 指定した座標にタイルがなければ処理をスキップする
				if (!_tilemap.HasTile(searchPos))
                {
					continue;
                }

				// 指定した座標のタイルによって配列情報をセットする
				if(_tilemap.GetTile(searchPos).Equals(_staticBlockTile))
                {
					StageArray[i, j] = ConstantForGame.STATIC_BLOCK;
				}
				else if (_tilemap.GetTile(searchPos).Equals(_moveBlockTile))
				{
					StageArray[i, j] = ConstantForGame.MOVE_BLOCK;
				}
				else if (_tilemap.GetTile(searchPos).Equals(_playerTile))
				{
					StageArray[i, j] = ConstantForGame.PLAYER;

					// プレイヤーの座標を代入する
					PlayerPosition = new Vector2Int(i, j);
				}
				else if (_tilemap.GetTile(searchPos).Equals(_targetAreaTile))
				{
					StageArray[i, j] = ConstantForGame.TARGET_AREA;
				}
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
	public TileBase GetStageObject(int row, int col)
	{
        // rootObject内の全てのオブジェクトを検索する
        foreach (Vector3Int pos in _tilemap.cellBounds.allPositionsWithin)
        {
			/*Debug.LogError(_tilemap.GetTile(pos));
			Vector3Int tilePos = pos + Vector3Int.up;

            // オブジェクトの座標が渡された引数と同じだったら
            if (tilePos.x == col && tilePos.y == -row)
            {
                return _tilemap.GetTile(tilePos);
            }*/

			if(pos.x == col && pos.y == -row)
            {
				return _tilemap.GetTile(pos);
            }
        }

        return null;
    }
	#endregion
}