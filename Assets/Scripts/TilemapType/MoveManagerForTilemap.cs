// ---------------------------------------------------------
// MoveManagerForTilemap.cs
//
// 作成日:2023/10/25
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

/// <summary>
/// プレイヤーとブロックの移動及びその判定を行う
/// </summary>
public class MoveManagerForTilemap : MonoBehaviour
{
	#region 変数
	// 移動チェック回数判定用定数
	private const int FIRST = 1;
	private const int SECOND = 2;

	// ブロックを引ける状態か
	[SerializeField] 
	private bool _isPullBlockMode = false;

	// ブロックの移動チェックの実行回数
	private int _moveBlockCheckCount = 0;

	#region ターゲット
	// ブロックがターゲットの上にある個数
	private int _targetCount = 0;
	// ターゲットの最大数
	private int _targetMaxCount = 3;
	#endregion

	// ブロックを引ける状態かを表示するテキスト
	private TMP_Text _isPullBlockModeText = default;
	private string _pullBlockModeTextTag = "PullBlockModeText";

    private string _pullModeOn = "On";
	private string _pullModeOff = "Off";

	// 各クラスの定義
	private StageArrayDataForTilemap _stageArrayDataForTilemap = default;
	private BlockProcessForTilemap _blockProcessForTilemap = default;

	#endregion

	#region メソッド
	/// <summary>
    /// 各クラス、変数の初期化処理
    /// </summary>
	private void Awake()
    {
		// 各クラスの初期化
		_stageArrayDataForTilemap = GetComponent<StageArrayDataForTilemap>();
		_blockProcessForTilemap = GetComponent<BlockProcessForTilemap>();

		// ブロックを引ける状態かを表示するテキストを取得する
		_isPullBlockModeText = GameObject.FindWithTag(_pullBlockModeTextTag).GetComponent<TMP_Text>();
	}

	/// <summary>
	/// Tilemapを上書きする
	/// データを上書きするので移動できるかどうか検査して
	/// 移動可能な場合処理を実行する
	/// </summary>
	/// <param name="preRow">移動前縦情報</param>
	/// <param name="preCol">移動前横情報</param>
	/// <param name="nextRow">移動後縦情報</param>
	/// <param name="nextCol">移動後横情報</param>
	private void MoveData(int preRow, int preCol, int nextRow, int nextCol)
    {
		// 動かしたいオブジェクトを取得する
		TileBase moveTile = _stageArrayDataForTilemap.GetStageObject(preRow, preCol);

		// オブジェクトが空じゃないか
		if (moveTile != null)
        {
			Vector3Int afterTilePos = new Vector3Int(nextCol, -nextRow);
			// オブジェクトを移動させる
			_stageArrayDataForTilemap.GettingTileMap.SetTile(afterTilePos, moveTile);
        }

		// ステージ配列の情報を書き換える
		_stageArrayDataForTilemap.StageArray[nextRow, nextCol] = _stageArrayDataForTilemap.StageArray[preRow, preCol];

		// 移動前の情報を削除する
		_stageArrayDataForTilemap.StageArray[preRow, preCol] = ConstantForGame.NO_BLOCK;
		Vector3Int beforeTilePos = new Vector3Int(preCol, -preRow);
		_stageArrayDataForTilemap.GettingTileMap.SetTile(beforeTilePos, null);

	}

    /// <summary>
    /// ブロックが移動可能か判定する
    /// 可能な場合true,不可能な場合falseを返す
    /// </summary>
    /// <param name="row">移動先Y座標</param>
    /// <param name="col">移動先X座標</param>
    /// <returns></returns>
    private bool BlockMoveCheck(int row, int col)
	{
		// ブロック移動チェック回数を加算する
		_moveBlockCheckCount++;

		// すでにターゲットにブロックがあったら、移動できないように判定する（後半部分）
		if (_stageArrayDataForTilemap.TargetData[row, col] == ConstantForGame.TARGET_AREA && 
			_stageArrayDataForTilemap.StageArray[row, col] != ConstantForGame.MOVE_BLOCK)
		{
			// ターゲットカウントを上げる
			_targetCount++;

			return true;
		}

		// ブロックの移動チェックが何回行われたか
        switch (_moveBlockCheckCount)
        {
			case FIRST:
				// 移動先が空か動かせるブロックだったらtrue
				return _stageArrayDataForTilemap.StageArray[row, col] == ConstantForGame.MOVE_BLOCK || 
						_stageArrayDataForTilemap.StageArray[row, col] == ConstantForGame.NO_BLOCK;

			case SECOND:
				// 移動先が空ならtrue
				return _stageArrayDataForTilemap.StageArray[row, col] == ConstantForGame.NO_BLOCK;

			default:
				break;
        }

        return false;
	}

	/// <summary>
	/// ブロックを移動する（ブロック移動チェックも実施）
	/// </summary>
	/// <param name="preRow">移動前縦情報</param>
	/// <param name="preCol">移動前横情報</param>
	/// <param name="nextRow">移動後縦情報</param>
	/// <param name="nextCol">移動後横情報</param>
	private bool BlockMove(int preRow, int preCol, int nextRow, int nextCol)
	{
		// 移動後のブロックの移動後情報
		int nextNextRow = nextRow + (nextRow - preRow);
		int nextNextCol = nextCol + (nextCol - preCol);

		// 境界線外エラー
		if (nextRow < 0 || nextCol < 0 || 
			nextRow > _stageArrayDataForTilemap.VerticalMaxSize || 
			nextCol > _stageArrayDataForTilemap.HorizontalMaxSize ||
			nextNextRow > _stageArrayDataForTilemap.VerticalMaxSize ||
			nextNextCol > _stageArrayDataForTilemap.HorizontalMaxSize)
		{
			return false;
		}

		// 手前のブロックの移動チェック
		bool moveFlag = BlockMoveCheck(nextRow, nextCol);

		// 奥のブロックのチェックフラグ
		bool moveBackFlag = false;

		// ブロックの移動先が動かせるブロックだったら
		if (_stageArrayDataForTilemap.StageArray[nextRow, nextCol] == ConstantForGame.MOVE_BLOCK)
        {
			// 奥のブロックの移動チェック
			moveBackFlag = BlockMoveCheck(nextNextRow, nextNextCol);

            if (!moveBackFlag)
            {
				moveFlag = false;
            }
		}

		// チェックした回数をリセットする
		_moveBlockCheckCount = 0;

		// 移動可能かチェックする
		if (moveFlag)
		{
			// 移動可能かチェックする
            if (moveBackFlag)
            {
				// オブジェクトを移動させる
				MoveData(nextRow, nextCol, nextNextRow, nextNextCol);
			}

			// オブジェクトを移動させる
			MoveData(preRow, preCol, nextRow, nextCol);
		}

		return moveFlag;
	}

	/// <summary>
	/// プレイヤーを移動することが可能かチェックする
	/// trueの場合移動可能　falseの場合移動不可能
	/// </summary>
	/// <param name="preRow">移動前縦情報</param>
	/// <param name="preCol">移動前横情報</param>
	/// <param name="nextRow">移動後縦情報</param>
	/// <param name="nextCol">移動後横情報</param>
	/// <returns>プレイヤー移動の可否</returns>
	private bool PlayerMoveCheck(int preRow, int preCol, int nextRow, int nextCol)
	{
		/*
		 * プレイヤーの移動先が動くブロックの時
		 * ブロックの移動する処理を実行する
		 */
		if (_stageArrayDataForTilemap.StageArray[nextRow, nextCol] == ConstantForGame.MOVE_BLOCK)
		{
			// ブロックを動かす
			bool blockMoveFlag = BlockMove(nextRow, nextCol,nextRow + (nextRow - preRow), nextCol + (nextCol - preCol));



			// ターゲットブロックかつ移動できる移動ブロックだったら
			if (_stageArrayDataForTilemap.TargetData[nextRow, nextCol] == ConstantForGame.TARGET_AREA && 
				blockMoveFlag)
			{
				// ターゲットクリアカウントを下げる
				_targetCount--;
			}
			return blockMoveFlag;
		}

		// プレイヤーの移動先が空またはターゲットの時移動する
		if (_stageArrayDataForTilemap.StageArray[nextRow, nextCol] == ConstantForGame.NO_BLOCK ||
			_stageArrayDataForTilemap.StageArray[nextRow, nextCol] == ConstantForGame.TARGET_AREA)
		{
			return true;
		}
		return false;
	}

	/// <summary>
	/// プレイヤーを移動する（プレイヤー移動チェックも実施）
	/// </summary>
	/// <param name="preRow">移動前縦情報</param>
	/// <param name="preCol">移動前横情報</param>
	/// <param name="nextRow">移動後縦情報</param>
	/// <param name="nextCol">移動後横情報</param>
	public void PlayerMove(int preRow, int preCol, int nextRow, int nextCol)
	{
		// 移動可能かチェックする
		if (PlayerMoveCheck(preRow, preCol, nextRow, nextCol))
		{
			// 移動が可能な場合移動する
			MoveData(preRow, preCol, nextRow, nextCol);

			// プレイヤーの位置を更新する、座標情報なので最初の引数はX
			_stageArrayDataForTilemap.PlayerPosition = new Vector2Int(nextRow, nextCol);

			// ブロックを引ける状態かつ
			// プレイヤーの移動方向と反対側に動かせるブロックがあったら
			if (_isPullBlockMode && _stageArrayDataForTilemap.StageArray[preRow + (preRow - nextRow), preCol + (preCol - nextCol)] == ConstantForGame.MOVE_BLOCK)
            {
				// ブロックを動かす
                bool blockMoveFlag = BlockMove(preRow + (preRow - nextRow),preCol + (preCol - nextCol), preRow, preCol);

                // ターゲットブロックかつ移動できるブロックだったら
                if (_stageArrayDataForTilemap.TargetData[preRow + (preRow - nextRow), preCol + (preCol - nextCol)] == ConstantForGame.TARGET_AREA && blockMoveFlag)
				{
					// ターゲットクリアカウントを下げる
					_targetCount--;
				}
			}
		}
	}

	/// <summary>
	/// ブロックがターゲットに乗っているかの判定を行う
	/// </summary>
	/// <returns>ブロックがそろっているかの有無</returns>
	public bool OnBlockAllTargetCheck()
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

	/// <summary>
	/// ブロックを引ける状態にする
	/// </summary>
	public void SetTruePullMode()
    {
		// ブロックを引ける状態にする
		_isPullBlockMode = true;
		_isPullBlockModeText.SetText(_pullModeOn);
    }

	/// <summary>
	/// ブロックを引ける状態を解除する
	/// </summary>
	public void SetFalsePullMode()
    {
		// ブロックを引けない状態にする
		_isPullBlockMode = false;
		_isPullBlockModeText.SetText(_pullModeOff);
	}
	#endregion
}