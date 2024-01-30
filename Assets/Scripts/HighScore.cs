// ---------------------------------------------------------
// HighScore.cs
//
// 作成日:2023/11/08
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using TMPro;

/// <summary>
/// タイトルシーンにハイスコアを表示するクラス
/// </summary>
public class HighScore : MonoBehaviour
{
	#region 変数
	// ハイスコアを表示するためのテキスト
	[SerializeField] private TMP_Text _highScoreText;

	private string _highScore = "HighScore";
	#endregion

	#region メソッド
	/// <summary>
	/// テキスト設定処理
	/// </summary>
	void Start ()
	{
		// ハイスコアを表示する
		_highScoreText.SetText(PlayerPrefs.GetInt(_highScore, 0).ToString());
	}
	#endregion
}