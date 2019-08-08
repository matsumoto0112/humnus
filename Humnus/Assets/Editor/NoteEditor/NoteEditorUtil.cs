using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace NoteEditor
{
    public static class NoteEditorUtil
    {
        /// <summary>
        /// 時刻表記に変換する
        /// </summary>
        /// <param name="second">時間(秒)</param>
        /// <returns></returns>
        public static string ConvertTimeFromSecond(int second)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:00}", second / 60);
            sb.Append(":");
            sb.AppendFormat("{0:00}", second % 60);
            return sb.ToString();
        }

        /// <summary>
        /// メインのエディター上でのノーツの位置を取得する
        /// </summary>
        /// <param name="note"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public static Vector2 CalculateNotePositionOnMainEditor(Music music, Note note, Rect area)
        {
            float x = CalculateCurrentAudioPositionXOnGUI(music, note.position.X, NoteCreaterDefine.AXIS_OFFSET.x);
            float y = note.position.Y * NoteCreaterDefine.AXIS_WIDTH.y + NoteCreaterDefine.AXIS_WIDTH.y / 2 + NoteCreaterDefine.AXIS_OFFSET.y;
            return new Vector2(x, y) + area.min;
        }

        /// <summary>
        /// オーディオの再生地点に対するGUI上のX座標を計算する
        /// </summary>
        /// <returns></returns>
        public static float CalculateCurrentAudioPositionXOnGUI(Music music, float X, float offset)
        {
            float unit = CalculateCurrentXAxisWidthPerBeats(music);
            return unit * (X - 4 * music.position) + offset;
        }

        /// <summary>
        /// 現在の1泊ごとのX軸の横幅を計算する
        /// </summary>
        /// <returns></returns>
        public static float CalculateCurrentXAxisWidthPerBeats(Music music)
        {
            return NoteCreaterDefine.WIDTH_PER_BEATS_60 * 60.0f / music.BPM / 4;
        }
    }
}
