using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoteEditor
{
    /// <summary>
    /// ノーツ情報とJsonファイルを相互変換
    /// </summary>
    static class JsonConverter
    {
        /// <summary>
        /// ノーツデータをJson形式のテキストに変換する
        /// </summary>
        /// <param name="notes">すべてのノーツのリスト</param>
        /// <returns></returns>
        public static string ToJson(List<Note> notes)
        {
            return JsonUtility.ToJson(new ListSerialization<Note>(notes));
        }

        /// <summary>
        /// Jsonテキストからノーツデータに変換する
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns>読み込んだノーツリスト</returns>
        public static List<Note> FromJson(string jsonText)
        {
            return JsonUtility.FromJson<ListSerialization<Note>>(jsonText).ToList();
        }
    }
}

