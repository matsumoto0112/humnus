using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Assertions;

namespace NoteEditor
{
    /// <summary>
    /// ノーツエディター上でのノーツのレイアウト
    /// </summary>
    [System.Serializable]
    public class NoteLayout
    {
        public NoteType type; //種類
        public string name; //名前
        public Color color; //色
        //public Texture2D texture; //TODO:テクスチャが入手出来次第追加する
    }

    /// <summary>
    /// ノーツエディターの見た目定義
    /// </summary>
    public class NoteEditorLayout : ScriptableObject
    {
        [SerializeField]
        private List<NoteLayout> noteLayouts = new List<NoteLayout>();

        public Color GetColor(NoteType type)
        {
            NoteLayout find = FindLayout(type);
            return find.color;
        }

        public string[] GetNames()
        {
            return noteLayouts.Select(n => n.name).ToArray();
        }

        private NoteLayout FindLayout(NoteType type)
        {
            NoteLayout find = noteLayouts.Find(n => n.type == type);
            Assert.IsNotNull(find, type + "が登録されていません");
            return find;
        }
    }
}

