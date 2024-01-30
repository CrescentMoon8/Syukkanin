// ---------------------------------------------------------
// MoveMain.cs
//
// 作成日:2023/11/06
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 遊び方のシーンからゲームシーンに移動するためのクラス
/// </summary>
public class MoveMain : MonoBehaviour
{
	#region 変数
	#region 入力
	// ゲームシーンへ移動するための入力
	private string _startGameInput = "PullBlock";
	#endregion

	#region シーン名
	// ゲームシーン
	private string _game = "Main";
	#endregion
	#endregion

	#region メソッド

	/// <summary>
	/// シーン移動処理
	/// </summary>
	private void Update()
	{
		//スペースキー、Aボタンが押されたら
		if (Input.GetButtonDown(_startGameInput))
		{
			//ゲーム画面へ移動する
			SceneManager.LoadScene(_game);
		}
	}
	#endregion
}