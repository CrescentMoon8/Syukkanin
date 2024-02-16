// ---------------------------------------------------------
// BoldBrush.cs
//
// 作成日:
// 作成者:
// ---------------------------------------------------------
using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
namespace UnityEditor.Tilemaps
{

    [CustomGridBrush(true, false, false, "Bold Brush")]
    public class BoldBrush : GridBrush
    {
        // ペイントツールで呼び出される関数
        public override void Paint
        (
            GridLayout grid,
            GameObject brushTarget,
            Vector3Int position
        )
        {
            // 3 x 3 マスを塗る
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var pos = position;
                    pos.x += x;
                    pos.y += y;

                    // タイルを塗る時は base.Paint を呼び出す
                    base.Paint(grid, brushTarget, pos);
                }
            }
        }

        // 削除ツールで呼び出される関数
        public override void Erase
        (
            GridLayout grid,
            GameObject brushTarget,
            Vector3Int position
        )
        {
            // 3 x 3 マスを消す
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var pos = position;
                    pos.x += x;
                    pos.y += y;

                    // タイルを消す時は base.Erase を呼び出す
                    base.Erase(grid, brushTarget, pos);
                }
            }
        }
    }

    [CustomEditor(typeof(BoldBrush))]
    public class BoldBrushEditor : GridBrushEditor
    {
        // Scene ビューでブラシを描画する時に呼び出される関数
        public override void OnPaintSceneGUI
        (
            GridLayout grid,
            GameObject brushTarget,
            BoundsInt position,
            GridBrushBase.Tool tool,
            bool executing
        )
        {
            // タイルマップのプレビューをクリアする
            var tilemap = brushTarget.GetComponent<Tilemap>();
            if (tilemap != null)
            {
                tilemap.ClearAllEditorPreviewTiles();
            }

            // 3 x 3 マスでプレビューを描画する
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    var pos = new Vector3Int(position.x + x, position.y + y, position.z);

                    // タイルのプレビューを 1 マス分描画する時は
                    // PaintPreview を呼び出す
                    PaintPreview(grid, brushTarget, pos);
                }
            }

            // 3 x 3 マスのサイズで線を引く
            var min = new Vector3Int(position.x - 1, position.y - 1, position.z);
            var max = new Vector3Int(position.x + 2, position.y + 2, position.z);

            var p1 = new Vector3(min.x, min.y, min.z);
            var p2 = new Vector3(max.x, min.y, min.z);
            var p3 = new Vector3(max.x, max.y, min.z);
            var p4 = new Vector3(min.x, max.y, min.z);

            // 線を引く時は Handles.DrawLine を呼び出す
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p4);
            Handles.DrawLine(p4, p1);
        }
    }
}