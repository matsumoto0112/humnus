using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoteEditor
{
    /// <summary>
    /// 音源情報
    /// </summary>
    public struct Music
    {
        public int BPM; //BPM
        public string name; //曲名
        public float position; //再生地点

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="BPM">BPM</param>
        /// <param name="name">曲名</param>
        public Music(int BPM, string name = "")
        {
            this.BPM = BPM;
            this.name = name;
            this.position = 0.0f;
        }
    }
}
