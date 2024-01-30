// ---------------------------------------------------------
// BlockProcess.cs
//
// 作成日:2023/10/24
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ブロックの生成、削除及びその判定を行うクラス
/// </summary>
public class BlockProcess : MonoBehaviour
{
	#region 変数
	#region ブロック削除
	// ブロックを消す判定用の定数
	private const int DELETE_JUDGE_COUNT = 6;
	// 削除対象のブロックを格納するためのリスト
	private List<GameObject> _deleteBlockList = new List<GameObject>();
    #endregion

    #region オブジェクト
    // ステージの親オブジェクト
    [SerializeField] 
	private GameObject _rootObject = default;
	// 動かせるブロックのオブジェクト
	[SerializeField] 
	private GameObject _moveBlock = default;
	// 動かせないブロックのオブジェクト
	[SerializeField] 
	private GameObject _staticBlock = default;
    #endregion

    #region パーティクル
    // ブロックを壊したときに再生するパーティクル
    [SerializeField] 
	private ParticleSystem _particleObject1 = default;
	[SerializeField] 
	private ParticleSystem _particleObject2 = default;
	[SerializeField] 
	private ParticleSystem _particleObject3 = default;
    #endregion

    #region ランダム
    // ステージの空マスの横情報を格納するリスト
    private List<int> _stageColList = new List<int>();
	// ステージの空マスの縦情報を格納するリスト
	private List<int> _stageRowList = new List<int>();
	// 移動可能なステージの縦の最大サイズ
	private const int STAGE_MAX_ROW = 10;
	// 移動可能なステージの横の最大サイズ
	private const int STAGE_MAX_COL = 10;
	// 空マスのリストから取り出した横情報
	private int _randomCol = 0;
	// 空マスのリストから取り出した縦情報
	private int _randomRow = 0;
    #endregion

    #region ブロックの配置
    // 動かせないブロックを配置する数
    private const int SET_STATICBLOCK_COUNT = 7;
	// 動かせるブロックを配置する数
	private const int SET_MOVEBLOCK_COUNT = 6;
	// 動かせるブロックの初期化が終わったか
	private bool _isMoveBlockInitialization = false;
    #endregion

    #region ターゲット
    // ブロックがターゲットの上にある個数
    private int _targetCount = 0;
	// ターゲットの最大数
	private int _targetMaxCount = 3;
    #endregion

    #region ゲームオーバー
    // ゲームオーバーになったか
    private bool _isGameOver = false;
    #endregion

    #region クラス
	// 各クラスの定義
    private StageArrayData _stageArrayData = default;
	private AudioController _audioController = default;
	#endregion
	#endregion

	#region プロパティ
	// ブロックがターゲットの上にある個数
	public int TargetCount { get => _targetCount; set => _targetCount = value; }
	// ターゲットの最大数
	public int TargetMaxCount { get => _targetMaxCount; set => _targetMaxCount = value; }
	public bool IsGameOver { get => _isGameOver; set => _isGameOver = value; }
	#endregion

	#region メソッド
	/// <summary>
	/// 各クラスの初期化処理
	/// </summary>
	private void Start()
    {
		// 各クラスの初期化
        _stageArrayData = GetComponent<StageArrayData>();
		_audioController = GetComponent<AudioController>();
    }

	/// <summary>
	/// 現在の空マスの座標を抽出する
	/// </summary>
    public void SetCell()
    {
		// リストを初期化する
		_stageColList.Clear();
		_stageRowList.Clear();

		// 配列内を全て探索する
        for (int y = 1; y <= STAGE_MAX_ROW; y++)
        {
            for (int x = 1; x <= STAGE_MAX_COL; x++)
            {
				// その座標が空かつ、ターゲットではない場合
                if (_stageArrayData.StageArray[y, x] == ConstantForGame.NO_BLOCK &&
					_stageArrayData.TargetData[y, x] != ConstantForGame.TARGET_AREA)
                {
					// 空マスの座標をそれぞれのリストに格納する
                    _stageRowList.Add(y);
                    _stageColList.Add(x);
                }
            }
        }
    }

	/// <summary>
	/// ブロックを生成する
	/// 生成できなかった場合、生成できるまで処理を行う
	/// </summary>
	public void CreateMoveBlock()
    {
		// 縦情報と横情報をランダムに抽出する
		GetCell();

		// MoveBlock情報を配列に加える
		_stageArrayData.StageArray[_randomRow, _randomCol] = ConstantForGame.MOVE_BLOCK;
		// MoveBlockのオブジェクトを生成する
		Instantiate(_moveBlock, new Vector2(_randomCol, -_randomRow), Quaternion.identity, _rootObject.transform);

		// 乱数がなくなったら＝ブロックを生成できる場所がなくなったら
		if (_stageColList.Count <= 0 && _stageRowList.Count <= 0)
		{
			// ゲームオーバーフラグをtrueにする
			IsGameOver = true;
			return;
		}
	}

	/// <summary>
	/// ゲーム開始時にステージを構成する
	/// ゲーム途中でブロックを追加する
	/// </summary>
	public void CreateStage()
    {
		// 動かせないブロックを指定数配置する
		for (int i = 0; i < SET_STATICBLOCK_COUNT; i++)
		{
			// 縦情報と横情報をランダムに抽出する
			GetCell();

			// StaticBlock情報を配列に加える
			_stageArrayData.StageArray[_randomRow, _randomCol] = ConstantForGame.STATIC_BLOCK;
			// StaticBlockのオブジェクトを生成する
			Instantiate(_staticBlock, new Vector2(_randomCol, -_randomRow), Quaternion.identity, _rootObject.transform);
		}
		
		// 動かせるブロックの初期配置が終わっているか
		if(_isMoveBlockInitialization)
        {
			return;
        }

		// 動かせるブロックを指定数配置する
		for (int i = 0; i < SET_MOVEBLOCK_COUNT; i++)
        {
			// 縦情報と横情報をランダムに抽出する
			GetCell();

			// MoveBlock情報を配列に加える
			_stageArrayData.StageArray[_randomRow, _randomCol] = ConstantForGame.MOVE_BLOCK;
			// MoveBlockのオブジェクトを生成する
			Instantiate(_moveBlock, new Vector2(_randomCol, -_randomRow), Quaternion.identity, _rootObject.transform);
		}

		// 動かせるブロックの初期化が終了
		_isMoveBlockInitialization = true;
	}

	/// <summary>
	/// 空マスをランダムに取得する
	/// </summary>
	private void GetCell()
    {
		// リストの大きさに合わせた範囲の乱数を取得する
		int index = UnityEngine.Random.Range(0, _stageColList.Count);

		// リストから値を取り出す
		_randomCol = _stageColList[index];
		_randomRow = _stageRowList[index];

		// リストから取り出した値を削除する
		_stageColList.RemoveAt(index);
		_stageRowList.RemoveAt(index);
    }

	/// <summary>
	/// ブロックを削除する
	/// </summary>
	public void DeleteBlock()
    {
		// 削除対象のブロックを格納するためのリストを初期化する
		_deleteBlockList.Clear();

		// 配列の中からターゲットの上に乗っている動かせるブロックを抽出する
		for (int y = 0; y < _stageArrayData.VerticalMaxSize; y++)
        {
			for(int x = 0; x < _stageArrayData.HorizontalMaxSize; x++)
            {
				if((_stageArrayData.StageArray[y, x] + _stageArrayData.TargetData[y, x]) == DELETE_JUDGE_COUNT)
                {
					// 壊すブロックのリストにオブジェクトを追加する
					_deleteBlockList.Add(_stageArrayData.GetStageObject(ConstantForGame.MOVE_BLOCK, y, x));
					// 配列の値を空にする
					_stageArrayData.StageArray[y, x] = ConstantForGame.NO_BLOCK;
                }
            }
        }

		// 壊すブロックのリスト内にあるオブジェくを破壊する
		for (int deleteBlockCount = 0; deleteBlockCount < _deleteBlockList.Count; deleteBlockCount++)
        {
			Destroy(_deleteBlockList[deleteBlockCount]);
		}

		// パーティクルを再生する
		_particleObject1.Play();
		_particleObject2.Play();
		_particleObject3.Play();

		// SEを再生する
		_audioController.DestroySe();
    }

	/// <summary>
	/// ブロックがターゲットに乗っているかの判定を行う
	/// </summary>
	/// <returns>ブロックがそろっているかの有無</returns>
	public bool OnBlockAllTarget()
	{
		// ターゲットクリア数とターゲットの最大数が一致したらブロックを破壊
		if (_targetCount == _targetMaxCount)
		{
			// ターゲットクリア数を初期化する
			_targetCount = 0;
			return true;
		}
		return false;
	}
	#endregion
}