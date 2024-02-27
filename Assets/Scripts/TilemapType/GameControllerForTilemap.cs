// ---------------------------------------------------------
// GameController.cs
//
// 作成日:2023/10/25
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームの流れを制御する
/// </summary>
public class GameControllerForTilemap : MonoBehaviour
{
	#region 変数

	#region 状態(enum)
	/// <summary>
	/// ゲームの状態
	/// １　ゲーム開始
	/// ２　ポーズ
	/// ３　プレイ中
	/// ４　ステージ生成
	/// ５　ブロック生成
	/// ６　ブロック破壊
	/// ７　スコア加算
	/// ８　難易度変更
	/// ９　ゲーム終了
	/// </summary>
	private enum GameState
	{
		Start,
		Stop,
		Play,
		CreateStage,
		CreateBlock,
		DeleteBlock,
		Score,
		DifficultUp,
		End,
	}

	// 現在のゲームの状態
	[SerializeField]
	private GameState _gameState = GameState.Start;
	#endregion

	#region 入力
	// 入力がされたか
	[SerializeField]
	private bool _isInput = false;
	// 入力されてから時間
	private float _inputTime = 0f;
	// 入力をリセットするまでの間隔
	private const float INPUT_RESET_TIME = 0.2f;
	// 縦方向の入力
	private string _horizontal = "Horizontal";
	// 横方向の入力
	private string _vertical = "Vertical";
	// ポーズのための入力
	private string _poseInput = "Pose";
	// ブロックを引くための入力
	private string _pullBlockInput = "PullBlock";
	// タイトルへ移動するための入力
	private string _goTitleInput = "GoTitle";
	#endregion

	#region ブロック
	// 動かせるブロックを生成するまでの時間
	private float _createBlockTime = 0f;
	// 動かせるブロックを生成する間隔
	private float _createBlockInterval = 5f;
	#endregion

	#region ゲーム終了後
	// ゲームが終了してからの時間
	private float _afterGameTime = 0f;
	// ゲームが終了してからResultシーンへ移動する時間
	private const int MOVE_SCENE_TIME = 3;
	#endregion

	#region UI
	// ポーズ時に表示するパネル
	[SerializeField]
	private GameObject _posePanel = default;
	#endregion

	#region シーン名
	// タイトルシーン
	private string _title = "Title";
	// リザルトシーン
	private string _result = "Result";
	#endregion

	#region スコア
	// スコアの保存名称
	private string _score = "Score";
	#endregion

	#region クラス
	// 各クラスの定義
	private StageArrayDataForTilemap _stageArrayDataForTilemap = default;
	private MoveManagerForTilemap _moveManagerForTilemap = default;
	private BlockProcessForTilemap _blockProcessForTilemap = default;
	private ScoreManagerForTilemap _scoreManagerForTilemap = default;
	#endregion

	#endregion

	#region メソッド
	/// <summary>
	/// 各クラスの初期化処理
	/// </summary>
	private void Awake()
	{
		// 各クラスの初期化
		_stageArrayDataForTilemap = GetComponent<StageArrayDataForTilemap>();
		_moveManagerForTilemap = GetComponent<MoveManagerForTilemap>();
		_blockProcessForTilemap = GetComponent<BlockProcessForTilemap>();
		_scoreManagerForTilemap = GetComponent<ScoreManagerForTilemap>();
	}

	/// <summary>
	/// ゲーム状態の更新処理
	/// </summary>
	private void Update()
	{
		// 現在のゲームの状態によって処理を変える
		switch (_gameState)
		{
			case GameState.Start:
				// 状態をステージ生成に移行する
				_gameState = GameState.CreateStage;
				break;

			case GameState.Stop:
				// Pキー、Xボタンが押されたか
				if (Input.GetButtonDown(_poseInput))
				{
					// ポーズを解除する
					Time.timeScale = 1;
					// ポーズ中のパネル、テキストを無効化する
					_posePanel.SetActive(false);
					// 状態をプレイ中に移行する
					_gameState = GameState.Play;
				}

				// Tキー、Bボタンが押されたか
				if (Input.GetButtonDown(_goTitleInput))
				{
					// ポーズを解除する
					Time.timeScale = 1;
					// タイトル画面へ戻る
					SceneManager.LoadScene(_title);
				}
				break;

			case GameState.Play:
				// 動かせるブロックを生成するまでの時間を加算する
				_createBlockTime += Time.deltaTime;

				// 一定時間でブロックを生成する
				if (_createBlockTime >= _createBlockInterval)
				{
					// ステージの空マスを抽出する
					_blockProcessForTilemap.SetCell();
					// 状態をブロック生成へ移行する
					_gameState = GameState.CreateBlock;
				}

				// Pキー、Xボタンが押されたか
				if (Input.GetButtonDown(_poseInput))
				{
					// ポーズする
					Time.timeScale = 0;
					// ポーズ中のパネル、テキストを有効化する
					_posePanel.SetActive(true);
					// 状態をポーズに移行する
					_gameState = GameState.Stop;
				}

				float horizontalInput = Input.GetAxisRaw(_horizontal);

				float verticalInput = Input.GetAxisRaw(_vertical);

				// ブロックを引くための入力があるか
				if (Input.GetButtonDown(_pullBlockInput))
				{
					// ブロックを引ける状態にする
					; _moveManagerForTilemap.SetTruePullMode();
				}

				// ブロックを引くための入力がないか
				if (Input.GetButtonUp(_pullBlockInput))
				{
					// ブロックを引ける状態を解除する
					_moveManagerForTilemap.SetFalsePullMode();
				}

				// 縦入力がなくて、全体の入力がない
				if (verticalInput == 0 && !_isInput)
				{
					// 横入力が０より大きい場合は右に移動
					if (horizontalInput > 0)
					{
						_moveManagerForTilemap.PlayerMove(_stageArrayDataForTilemap.PlayerPosition.x, _stageArrayDataForTilemap.PlayerPosition.y, 
															_stageArrayDataForTilemap.PlayerPosition.x,_stageArrayDataForTilemap.PlayerPosition.y + 1);

						_isInput = true;
					}
					// 横入力が０より小さい場合は左に移動
					else if (horizontalInput < 0)
					{
						_moveManagerForTilemap.PlayerMove(_stageArrayDataForTilemap.PlayerPosition.x, _stageArrayDataForTilemap.PlayerPosition.y, 
															_stageArrayDataForTilemap.PlayerPosition.x, _stageArrayDataForTilemap.PlayerPosition.y - 1);

						_isInput = true;
					}
				}

				// 横入力がなくて、全体の入力がない
				if (horizontalInput == 0 && !_isInput)
				{
					// 縦入力が０より大きい場合は上に移動
					if (verticalInput > 0)
					{
						_moveManagerForTilemap.PlayerMove(_stageArrayDataForTilemap.PlayerPosition.x, _stageArrayDataForTilemap.PlayerPosition.y, 
															_stageArrayDataForTilemap.PlayerPosition.x - 1, _stageArrayDataForTilemap.PlayerPosition.y);

						_isInput = true;
					}
					// 縦入力が０より小さい場合は下に移動
					else if (verticalInput < 0)
					{
						_moveManagerForTilemap.PlayerMove(_stageArrayDataForTilemap.PlayerPosition.x, _stageArrayDataForTilemap.PlayerPosition.y, 
															_stageArrayDataForTilemap.PlayerPosition.x + 1, _stageArrayDataForTilemap.PlayerPosition.y);

						_isInput = true;
					}
				}

				// 入力があるか
				if (_isInput)
				{
					// 入力されている時間を計測する
					_inputTime += Time.deltaTime;
				}

				// 入力状態が解除されるまで再入力できないようにする
				if ((horizontalInput + verticalInput) == 0 || _inputTime >= INPUT_RESET_TIME)
				{
					// 入力状態を解除する
					_isInput = false;
					// 入力されてからの時間をリセットする
					_inputTime = 0;
				}

				// ブロックがそろった判定
				if (_moveManagerForTilemap.OnBlockAllTargetCheck())
				{
					// 状態をブロック破壊に移行する
					_gameState = GameState.DeleteBlock;
				}
				break;

			case GameState.CreateStage:
				// 空マスの情報をリストに追加する
				_blockProcessForTilemap.SetCell();
				// 動かせないブロック及びM動かせるブロックをランダムに配置する
				_blockProcessForTilemap.CreateStage();
				// 状態をプレイ中に移行する
				_gameState = GameState.Play;
				break;

			case GameState.CreateBlock:
				// ランダムにブロックを生成する
				_blockProcessForTilemap.CreateMoveBlock();

				_createBlockTime = 0;

				if (_blockProcessForTilemap.IsGameOver)
				{
					_gameState = GameState.End;
				}
				else
				{
					_gameState = GameState.Play;
				}
				break;

			case GameState.DeleteBlock:
				// ブロックを削除する
				_blockProcessForTilemap.DeleteBlock();

				_gameState = GameState.Score;
				break;

			case GameState.Score:
				// ブロックを引ける状態を解除する
				_moveManagerForTilemap.SetFalsePullMode();
				// スコアとゲームレベルの更新を行う
				_scoreManagerForTilemap.ScoreUpdate();

				if (_scoreManagerForTilemap.LevelUpdate())
				{
					_gameState = GameState.DifficultUp;
				}

				_gameState = GameState.Play;
				break;

			case GameState.DifficultUp:
				// ゲームレベルが上がったらブロックの生成間隔を短くし、動かせないブロックを追加する
				switch (_scoreManagerForTilemap.GameLevel)
				{
					case ConstantForGame.BEGINNER:
						_createBlockInterval = ConstantForGame.EASY;
						break;
					case ConstantForGame.INTERMEDIATE:
						_createBlockInterval = ConstantForGame.NORMAL;
						_blockProcessForTilemap.CreateStage();
						break;
					case ConstantForGame.EXPERT:
						_createBlockInterval = ConstantForGame.HARD;
						break;
					case ConstantForGame.MASTER:
						_createBlockInterval = ConstantForGame.VERYHARD;
						_blockProcessForTilemap.CreateStage();
						break;
					default:
						break;
				}

				_gameState = GameState.Play;
				break;

			case GameState.End:
				// ゲームが終了してからの時間を計る
				_afterGameTime += Time.deltaTime;
				// ゲームが終了してから指定秒数経過したか
				if (_afterGameTime >= MOVE_SCENE_TIME)
				{
					// スコアを保存する
					PlayerPrefs.SetInt(_score, _scoreManagerForTilemap.Score);
					// リザルトへ移動する
					SceneManager.LoadScene(_result);
				}
				break;
		}
	}
	#endregion
}
