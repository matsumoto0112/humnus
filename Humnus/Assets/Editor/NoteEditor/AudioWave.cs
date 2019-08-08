using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NoteEditor
{
    /// <summary>
    /// 曲の波形管理
    /// </summary>
    public class AudioWave
    {
        private static readonly int MAX_SAMPLE_COUNT = 2048; //波形のサンプル数
        private float[] samples;
        private Vector3[] linePositions;

        public AudioWave()
        {
            Init();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Init()
        {
            samples = new float[MAX_SAMPLE_COUNT];
            linePositions = new Vector3[MAX_SAMPLE_COUNT];
        }

        /// <summary>
        /// サンプリングする
        /// </summary>
        public void Sample()
        {
            AudioListener.GetSpectrumData(samples, 0, FFTWindow.BlackmanHarris);
        }

        /// <summary>
        /// 波形描画をする
        /// </summary>
        /// <param name="area">描画範囲</param>
        public void Draw(Rect area)
        {
            float x = area.xMin + NoteCreaterDefine.AXIS_OFFSET.x;
            for (int i = 0; i < linePositions.Length; i++)
            {
                linePositions[i] = new Vector3(x, samples[i] * 300 + area.center.y, 0.0f);
                x += 1.0f;
            }
            Handles.DrawAAPolyLine(1.0f, linePositions);
        }
    }
}
