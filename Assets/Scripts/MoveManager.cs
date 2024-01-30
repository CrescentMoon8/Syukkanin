// ---------------------------------------------------------
// MoveManager.cs
//
// 作成日:2023/10/25
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using TMPro;

/// <summary>
/// プレイヤーとブロックの移動及びその判定を行う
/// </summary>
public class MoveManager : MonoBehaviour
{
	#region 変数
	// 移動チェック回数判定用定数
	const int FIRST = 1;
	const int SECOND = 2;

	// ブロックを引ける状態か
	[SerializeField] 
	private bool _isPullBlockMode = false;

	// ブロックの移動チェックの実行回数
	private int _moveBlockCheckCount = 0;

	// ブロックを引ける状態かを表示するテキスト
	private TMP_Text _isPullBlockModeText = default;
	private string _pullBlockModeTextTag = "PullBlockModeText";


    private string _pullModeOn = "On";
	private string _pullModeOff = "Off";

	// 各クラスの定義
	private StageArrayData _stageArrayData = default;
	private BlockProcess _blockProcess = default;

	#endregion

	#region メソッド
	/// <summary>
    /// 各クラス、変数の初期化処理
    /// </summary>
	private void Awake()
    {
		// 各クラスの初期化
		_stageArrayData = GetComponent<StageArrayData>();
		_blockProcess = GetComponent<BlockProcess>();

		// ブロックを引ける状態かを表示するテキストを取得する
		_isPullBlockModeText = GameObject.FindWithTag(_pullBlockModeTextTag).GetComponent<TMP_Text>();
	}

	/// <summary>
	/// オブジェクトを移動する
	/// データを上書きするので移動できるかどうか検査して
	/// 移動可能な場合処理を執行してください
	/// </summary>
	/// <param name="preRow">移動前縦情報</param>
	/// <param name="preCol">移動前横情報</param>
	/// <param name="nextRow">移動後縦情報</param>
	/// <param name="nextCol">移動後横情報</param>
	public void MoveData(int preRow, int preCol, int nextRow, int nextCol)
    {
		// 動かしたいオブジェクトを取得する
		GameObject moveObject = _stageArrayData.GetStageObject(_stageArrayData.StageArray[preRow, preCol], preRow, preCol);

		// オブジェクトが空じゃないか
		if (moveObject != null)
        {
			// オブジェクトを移動させる
			moveObject.transform.position = new Vector2(nextCol, -nextRow);
        }

		// ステージ配列の情報を書き換える
		_stageArrayData.StageArray[nextRow, nextCol] = _stageArrayData.StageArray[preRow, preCol];

		// 移動前の情報を削除する
		_stageArrayData.StageArray[preRow, preCol] = ConstantForGame.NO_BLOCK;

	}

    /// <summary>
    /// ブロックが移動可能か判定する
    /// 可能な場合true,不可能な場合falseを返す
    /// </summary>
    /// <param name="y">移動先Y座標</param>
    /// <param name="x">移動先X座標</param>
    /// <returns></returns>
    public bool BlockMoveCheck(int y, int x)
	{
		// ブロック移動チェック回数を加算する
		_moveBlockCheckCount++;

		// すでにターゲットにブロックがあったら、移動できないように判定する（後半部分）
		if (_stageArrayData.TargetData[y, x] == ConstantForGame.TARGET_AREA && 
			_stageArrayData.StageArray[y, x] != ConstantForGame.MOVE_BLOCK)
		{
			// ターゲットカウントを上げる
			_blockProcess.TargetCount++;

			return true;
		}

		// ブロックの移動チェックが何回行われたか
        switch (_moveBlockCheckCount)
        {
			case FIRST:
				// 移動先が空か動かせるブロックだったらtrue
				return _stageArrayData.StageArray[y, x] == ConstantForGame.MOVE_BLOCK || 
						_stageArrayData.StageArray[y, x] == ConstantForGame.NO_BLOCK;

			case SECOND:
				// 移動先が空ならtrue
				return _stageArrayData.StageArray[y, x] == ConstantForGame.NO_BLOCK;

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
	public bool BlockMove(int preRow, int preCol, int nextRow, int nextCol)
	{
		// 移動後のブロックの移動後情報
		int nextNextRow = nextRow + (nextRow - preRow);
		int nextNextCol = nextCol + (nextCol - preCol);

		// 境界線外エラー
		if (nextRow < 0 || nextCol < 0 || 
			nextRow > _stageArrayData.VerticalMaxSize || 
			nextCol > _stageArrayData.HorizontalMaxSize ||
			nextNextRow > _stageArrayData.VerticalMaxSize ||
			nextNextCol > _stageArrayData.HorizontalMaxSize)
		{
			return false;
		}

		// 手前のブロックの移動チェック
		bool moveFlag = BlockMoveCheck(nextRow, nextCol);

		// 奥のブロックのチェックフラグ
		bool moveBackFlag = false;

		// ブロックの移動先が動かせるブロックだったら
		if (_stageArrayData.StageArray[nextRow, nextCol] == ConstantForGame.MOVE_BLOCK)
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
	public bool PlayerMoveCheck(int preRow, int preCol, int nextRow, int nextCol)
	{
		/*
		 * プレイヤーの移動先が動くブロックの時
		 * ブロックの移動する処理を実行する
		 */
		if (_stageArrayData.StageArray[nextRow, nextCol] == ConstantForGame.MOVE_BLOCK)
		{
			// ブロックを動かす
			bool blockMoveFlag = BlockMove(nextRow, nextCol,
											nextRow + (nextRow - preRow),
											nextCol + (nextCol - preCol));



			// ターゲットブロックかつ移動できる移動ブロックだったら
			if (_stageArrayData.TargetData[nextRow, nextCol] == ConstantForGame.TARGET_AREA && 
				blockMoveFlag)
			{
				// ターゲットクリアカウントを下げる
				_blockProcess.TargetCount--;
			}
			return blockMoveFlag;
		}

		// プレイヤーの移動先が空またはターゲットの時移動する
		if (_stageArrayData.StageArray[nextRow, nextCol] == ConstantForGame.NO_BLOCK ||
			_stageArrayData.StageArray[nextRow, nextCol] == ConstantForGame.TARGET_AREA)
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
			_stageArrayData.PlayerPosition = new Vector2(nextRow, nextCol);

			// ブロックを引ける状態だったら
			if (_isPullBlockMode)
			{
				// プレイヤーの移動方向と反対側に動かせるブロックがあったら
				if (_stageArrayData.StageArray[preRow + (preRow - nextRow), preCol + (preCol - nextCol)] == ConstantForGame.MOVE_BLOCK)
                {
					// ブロックを動かす
                    bool blockMoveFlag = BlockMove(preRow + (preRow - nextRow),
                                                   preCol + (preCol - nextCol), 
												   preRow, preCol);

                    // ターゲットブロックかつ移動できる移動ブロックだったら
                    if (_stageArrayData.TargetData[preRow + (preRow - nextRow), preCol + (preCol - nextCol)] == ConstantForGame.TARGET_AREA && 
						blockMoveFlag)
					{
						// ターゲットクリアカウントを下げる
						_blockProcess.TargetCount--;
					}
				}
			}
		}
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