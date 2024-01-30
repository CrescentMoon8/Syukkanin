// ---------------------------------------------------------
// ResultScore.cs
//
// 作成日:2023/11/02
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// リザルト画面でスコアを表示する
/// </summary>
public class ResultScore : MonoBehaviour
{
    #region 変数
    #region 入力
    // ゲームシーンへ移動するための入力
    private string _RetryInput = "PullBlock";
	// タイトルへ移動するための入力
	private string _goTitleInput = "GoTitle";
    #endregion

    #region UI
    //スコアを表示するためのテキスト
    [SerializeField] 
	private TMPro.TMP_Text _scoreText = default;
    #endregion

    #region タグ
    //スコアを表示するためのテキストのタグ
    private string _scoreTextTag = "ScoreText";
    #endregion

    #region スコア
    // スコアの保存名称
    private string _score = "Score";
	// ハイスコアの保存名称
	private string _highScore = "HighScore";
    #endregion

    #region シーン名
    // タイトルシーン
    private string _title = "Title";
	// ゲームシーン
	private string _game = "Main";
	#endregion

	#endregion

	#region メソッド
	/// <summary>
	/// テキストの初期化処理
	/// </summary>
	private void Awake()
	{
		//スコアを表示するためのテキストを取得する
		_scoreText = GameObject.FindWithTag(_scoreTextTag).GetComponent<TMP_Text>();
	}

	/// <summary>
	/// テキストの更新処理、入力判定
	/// </summary>
	private void Update ()
	{
		//今回のスコアとハイスコアを比較し、今回のスコアが大きかったら
		if(PlayerPrefs.GetInt(_score) > PlayerPrefs.GetInt(_highScore, 0))
		{
			//ハイスコアに今回のスコアを保存する
			PlayerPrefs.SetInt(_highScore, PlayerPrefs.GetInt(_score));
		}
		//テキストにスコアを代入する
		_scoreText.SetText(PlayerPrefs.GetInt(_score).ToString());

		//Tキー、Bボタンが押されたら
		if(Input.GetButtonDown(_goTitleInput))
        {
			//タイトル画面へ移動する
			SceneManager.LoadScene(_title);
        }

		//Rキー、Aボタンが押されたら
		if (Input.GetButtonDown(_RetryInput))
		{
			//ゲーム画面へ移動する
			SceneManager.LoadScene(_game);
		}
	}
	#endregion
}