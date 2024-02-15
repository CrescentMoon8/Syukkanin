// ---------------------------------------------------------
// ScoreManager.cs
//
// 作成日:2023/10/26
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

/// <summary>
/// スコアの加算、ゲームレベルの変更及びその判定を行う
/// </summary>
public class ScoreManager : MonoBehaviour
{
    #region 変数
    #region スコア
    // スコア
    private int _score = 0;
    // スコアの基本上昇量
    private const int BASE_SCORE = 30;
    // 難易度の変更に用いる基本判定値
    private const int LEVELUP_SCORE = 150;
    #endregion

    #region UI
    // スコアを表示するためのテキスト
    private TMPro.TMP_Text _scoreText = default;
    // 難易度を表示するためのテキスト
    private TMPro.TMP_Text _levelText = default;
    #endregion

    #region ゲームレベル
    // 難易度
    private int _gameLevel = 1;
    // 難易度の最大値
    private const int MAX_GAME_LEVEL = 5;
    #endregion

    #region タグ
    // スコアを表示するためのテキストのタグ
    private string _scoreTextTag = "ScoreText";
    // 難易度を表示するためのテキスト
    private string _gameLevelTextTag = "LevelText";
    #endregion

    #region クラス
    // 各クラスの定義
    private BlockProcess _blockProcess = default;
    private AudioController _audioController = default;
    #endregion
    #endregion

    #region プロパティ
    // スコア
    public int Score { get { return _score; } }
    // 難易度
    public int GameLevel { get { return _gameLevel; } }
    #endregion

    #region メソッド
    /// <summary>
    /// 各クラスの初期化及びTextオブジェクトの取得
    /// </summary>
    private void Awake()
    {
        // スコアを表示するためのテキストを取得する
        _scoreText = GameObject.FindWithTag(_scoreTextTag).GetComponent<TMP_Text>();
        // 難易度を表示するためのテキストを取得する
        _levelText = GameObject.FindWithTag(_gameLevelTextTag).GetComponent<TMP_Text>();

        // 各クラスの初期化
        _blockProcess = GetComponent<BlockProcess>();
        _audioController = GetComponent<AudioController>();
    }

    /// <summary>
    /// スコアの加算を行う（ゲームレベルの上昇の判定も行う）
    /// </summary>
    public void ScoreUpdate()
    {
        // スコアを加算する
        _score += BASE_SCORE * GameLevel;
        // ゲームレベルの上昇及びその判定を行う
        LevelUpdate();
        // スコアテキストを変更する
        _scoreText.SetText(_score.ToString());
        // 難易度テキストを変更する
        _levelText.SetText(_gameLevel.ToString());
    }

    /// <summary>
    /// 難易度の上昇及びその判定を行う
    /// </summary>
    public void LevelUpdate()
    {
        // 難易度が最大なら処理を抜ける
        if (GameLevel >= MAX_GAME_LEVEL)
        {
            return;
        }
        else
        {
            // スコアが一定数を超えたら
            if(_score >= LEVELUP_SCORE * GameLevel * GameLevel)
            {
                // 難易度を上昇させる
                _gameLevel++;
                // レベルアップ時のSEを再生する
                _audioController.LevelUpSe();

                // 特定の難易度になったら
                switch(GameLevel)
                {
                    case ConstantForGame.INTERMEDIATE:
                        // StaticBlockを追加する
                        _blockProcess.CreateStage();
                        break;

                    case ConstantForGame.MASTER:
                        // StaticBlockを追加する
                        _blockProcess.CreateStage();
                        break;

                    default:
                        break;
                }
            }
        }
    }
	#endregion
}