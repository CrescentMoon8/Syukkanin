// ---------------------------------------------------------
// AudioController.cs
//
// 作成日:2023/11/04
// 改修開始日:2024/02/27
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// SEを鳴らすためのクラス
/// </summary>
public class AudioController : MonoBehaviour
{
	#region 変数
	//ブロックを破壊した時のSE
	[SerializeField] 
	private AudioClip _destroySe = default;
	//ゲームレベルが上がった時のSE
	[SerializeField] 
	private AudioClip _levelUpSe = default;

	private string _seObjectTag = "SE";

	private AudioSource _audioSource = default;
	#endregion

	#region プロパティ

	#endregion

	#region メソッド
	/// <summary>
	/// 各クラスの初期化処理
	/// </summary>
	private void Awake()
	{
		// 各クラスの初期化
		_audioSource = GameObject.FindWithTag(_seObjectTag).GetComponent<AudioSource>();
	}

	/// <summary>
	/// ブロックが壊れた時のSEを再生する
	/// </summary>
	public void DestroySe()
	{
		// ブロックが壊れた時のSEを再生する
		_audioSource.PlayOneShot(_destroySe);
	}

	/// <summary>
	/// ゲームレベルが上がった時のSEを再生する
	/// </summary>
	public void LevelUpSe()
	{
		// ゲームレベルが上がった時のSEを再生する
		_audioSource.PlayOneShot(_levelUpSe);
	}
	#endregion
}