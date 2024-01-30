// ---------------------------------------------------------
// ConstantForGame.cs
//
// 作成日:2023/11/09
// 作成者:小林慎
// ---------------------------------------------------------
using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームに用いる定数を宣言している
/// </summary>
public class ConstantForGame
{
    #region プロパティ
    #region 配列
    //二次元配列に設定するための定数
    public const int NO_BLOCK = 0;
	public const int STATIC_BLOCK = 1;
	public const int MOVE_BLOCK = 2;
	public const int PLAYER = 3;
	public const int TARGET_AREA = 4;
    #endregion

    #region 達成度
    // ゲームレベルを示す定数
    public const int BEGINNER = 2;
	public const int INTERMEDIATE = 3;
	public const int EXPERT = 4;
	public const int MASTER = 5;
    #endregion

    #region 難易度
    // ブロックの生成間隔を示す定数
    public const int EASY = 4;
	public const int NORMAL = 3;
	public const int HARD = 2;
	public const int VERYHARD = 1;
    #endregion

    #endregion
}