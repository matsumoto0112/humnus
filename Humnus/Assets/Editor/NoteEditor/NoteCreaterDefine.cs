using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace NoteEditor
{

    public static class NoteCreaterDefine
    {
        /// <summary>
        /// ウィンドウの大きさ
        /// </summary>
        public static readonly Vector2 WINDOW_SIZE = new Vector2(800, 600);
        /// <summary>
        /// GUIのボタン幅
        /// </summary>
        public static readonly int BUTTON_WIDTH = 100;
        /// <summary>
        /// GUIボタンレイアウト 
        /// </summary>
        public static readonly GUILayoutOption LAYOUT_BUTTON_WIDTH = GUILayout.Width(BUTTON_WIDTH);

        public static readonly Vector2 MAIN_EDITOR_AREA_SIZE = new Vector2(WINDOW_SIZE.x, 100.0f);

        /// <summary>
        /// ノーツエディターのノーツ配置場所の各軸のオフセット値
        /// </summary>
        public static readonly Vector2 AXIS_OFFSET = new Vector2(10.0f, 10.0f);
        /// <summary>
        /// ノーツエディターのノーツ配置場所の各軸の幅
        /// </summary>
        public static readonly Vector2 AXIS_WIDTH = new Vector2(50.0f, MAIN_EDITOR_AREA_SIZE.y / 2 - AXIS_OFFSET.y);

        /// <summary>
        /// 60BPM時のX軸のGUI上の長さ
        /// 60BPMが一番直感的にわかりやすいと思われる
        /// </summary>
        public static readonly float WIDTH_PER_BEATS_60 = 480.0f;

        /// <summary>
        /// 軸の色
        /// </summary>
        public static readonly Color AXIS_NORMAL = Color.gray;

        /// <summary>
        /// 4軸に1軸色を変えるときの色
        /// </summary>
        public static readonly Color AXIS_PER_4_BEATS = new Color(0.2f, 0.2f, 0.2f, 1.0f);
    }
}
