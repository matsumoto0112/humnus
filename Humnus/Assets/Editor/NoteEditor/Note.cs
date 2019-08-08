using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NoteEditor
{

    /// <summary>
    /// ノーツの種類定義
    /// </summary>
    public enum NoteType
    {
        Normal, //通常ノーツ
        Hard, //ハード
        Double, //同時押しノーツ
        Dummy, //お邪魔
        Skill, //スキル
        Flick, //フリック
        Long, //長押し
    }

    /// <summary>
    /// ノーツ
    /// </summary>
    [System.Serializable]
    public class Note
    {
        public NoteVec2 position; //ノーツ座標
        public NoteType type; //ノーツの種類

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">座標</param>
        /// <param name="type">ノーツの種類</param>
        public Note(NoteVec2 position, NoteType type)
        {
            this.position = position;
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != this.GetType())
            {
                return false;
            }
            Note other = (Note)obj;
            return this.position == other.position && this.type == other.type;
        }

        public override int GetHashCode()
        {
            //ハッシュ値は各変数のハッシュ値のXORとする
            return this.position.GetHashCode() ^ this.type.GetHashCode();
        }

        /// <summary>
        /// ソート時の比較関数定義
        /// ノーツの種類で昇順。種類が同じときはXで昇順。
        /// </summary>
        public static readonly System.Comparison<Note> CompareFunc = (n1, n2) =>
             {
                 if (n1.type == n2.type)
                 {
                     return n1.position.X == n2.position.X ? n1.position.Y - n2.position.Y : n1.position.X - n2.position.X;
                 }
                 else
                 {
                     return n1.type - n2.type;
                 }
             };

    }

    /// <summary>
    /// 長押しノーツ
    /// </summary>
    [System.Serializable]
    public class LongNote : Note
    {
        public LongNote other; //接続相手

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">座標</param>
        public LongNote(NoteVec2 position)
            : base(position, NoteType.Long)
        {
            other = null;
        }

        /// <summary>
        /// 接続相手を設定する
        /// </summary>
        /// <param name="other">接続する相手</param>
        public void SetOtherNote(LongNote other)
        {
            this.other = other;
        }
    }
}
