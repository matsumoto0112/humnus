using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace NoteEditor
{
    /// <summary>
    /// ノーツ用Vector2
    /// </summary>
    [System.Serializable]
    public struct NoteVec2
    {
        /// <summary>
        /// 何番目のノーツか
        /// 1小節4拍だとして先頭から何番目か
        /// </summary>
        public int X;

        /// <summary>
        /// 何レーン目のノーツか
        /// </summary>
        public int Y;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="X">何番目のノーツか</param>
        /// <param name="Y">何レーン目のノーツか</param>
        /// <remarks>レーン数は0か1である必要があります</remarks>
        public NoteVec2(int X, int Y)
        {
            Assert.IsTrue(0 <= Y && Y < 2, "Yの値が不正です");
            this.X = X;
            this.Y = Y;
        }

        public static bool operator ==(NoteVec2 v1, NoteVec2 v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(NoteVec2 v1, NoteVec2 v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType()) return false;
            NoteVec2 other = (NoteVec2)obj;
            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }
    }
}