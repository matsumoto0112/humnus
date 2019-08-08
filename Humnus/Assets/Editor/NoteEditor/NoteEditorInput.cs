using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoteEditor
{
    /// <summary>
    /// ノーツエディター用マウス入力取得
    /// </summary>
    public static class NoteEditorInput
    {
        /// <summary>
        /// 入力の種類
        /// </summary>
        public enum InputType
        {
            None, //規定値
            Click, //左クリック 
            RightClick, //右クリック 
        }

        /// <summary>
        /// 左クリックされたか
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private static bool IsLeftButtonClick(Event e)
        {
            return e.type == EventType.MouseDown && e.button == 0;
        }

        /// <summary>
        /// 左クリックされたか
        /// </summary>
        /// <returns></returns>
        public static bool IsClick()
        {
            Event e = Event.current;
            return IsLeftButtonClick(e);
        }

        /// <summary>
        /// 右クリックされたか
        /// </summary>
        /// <returns></returns>
        public static bool IsRightClick()
        {
            Event e = Event.current;
            return e.type == EventType.MouseDown && e.button == 1;
        }

        /// <summary>
        /// 現在の入力の種類を取得する
        /// </summary>
        /// <returns>入力があればその入力の種類を返す。
        /// なければ規定値を返す</returns>
        public static InputType GetCurrentInputType()
        {
            //先頭から順々にイベントがないか判定する
            if (IsRightClick())
            {
                return InputType.RightClick;
            }
            if (IsClick())
            {
                return InputType.Click;
            }
            //何もなければ規定値を返す
            return InputType.None;
        }

        /// <summary>
        /// マウスの座標を取得する
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMousePosition()
        {
            Event e = Event.current;
            return e.mousePosition;
        }
    }
}

