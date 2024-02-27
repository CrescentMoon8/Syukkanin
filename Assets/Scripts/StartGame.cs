// ---------------------------------------------------------
// StartGame.cs
//
// 作成日:2023/10/31
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// ゲームの開始、終了を行う
/// </summary>
public class StartGame : MonoBehaviour
{
    #region 変数
    #region 入力
    // 遊び方へ移動するための入力
    private string _playGuidInput = "PullBlock";
	// ゲームを終了するための入力
	private string _quitInput = "Quit";
    #endregion

    #region シーン名
    // 遊び方のシーン名
    private string _playGuid = "PlayGuid";
    #endregion
    #endregion

    #region メソッド

    /// <summary>
    /// 入力処理
    /// </summary>
    private void Update ()
	{
		// スペースキー、Aボタンが押されたら
		if(Input.GetButtonDown(_playGuidInput))
        {
			// 遊び方を表示する画面へ移動する
			SceneManager.LoadScene(_playGuid);
        }

		// Eキー、Bボタンが押されたら
		if(Input.GetButtonDown(_quitInput))
        {
			// ゲームを終了する
			Application.Quit();
        }
	}
	#endregion
}