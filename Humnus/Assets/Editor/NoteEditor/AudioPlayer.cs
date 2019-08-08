using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace NoteEditor
{
    /// <summary>
    /// 曲の再生管理
    /// </summary>
    public class AudioPlayer
    {
        /// <summary>
        /// 現在の音の再生状況
        /// </summary>
        private enum AudioState
        {
            Playing,
            Pause,
        }

        private class AudioGameObject
        {
            private GameObject audioObject;
            public AudioSource audioSource { get; private set; }
            public AudioGameObject()
            {
                audioObject = new GameObject("AudioObject")
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                audioSource = audioObject.AddComponent<AudioSource>();
            }

            /// <summary>
            /// このオブジェクトが破棄されるタイミングでゲームオブジェクトも一緒に破棄したいため
            /// </summary>
            ~AudioGameObject()
            {
                if (audioObject) Object.DestroyImmediate(audioObject);
            }
        }

        private ReactiveProperty<AudioClip> audio;
        private AudioGameObject audioObject;
        private AudioState currentState;
        private Music _music;
        public Music music { get { return _music; } }

        public float audioLength { get; private set; }

        public AudioPlayer()
        {
            audio = new ReactiveProperty<AudioClip>();
            //初期化時に呼ばれるのを防ぐ
            audio.SkipLatestValueOnSubscribe().Subscribe(_ => UpdateAudioSource());
            audioObject = new AudioGameObject();
            currentState = AudioState.Pause;
            _music = new Music(60);
        }

        public void PrintAudioName()
        {
            GUILayout.Label(music.name);
        }

        public void AudioField()
        {
            audio.Value = EditorGUILayout.ObjectField(audio.Value, typeof(AudioClip), false) as AudioClip;
        }

        public void BPMField()
        {
            GUILayout.Label("BPM");
            _music.BPM = EditorGUILayout.IntSlider(music.BPM, 1, 320);
        }

        public void PlaySlider()
        {
            //時間表示
            _music.position = audioObject.audioSource.time;
            GUILayout.Label(NoteEditorUtil.ConvertTimeFromSecond((int)music.position) + "/" + NoteEditorUtil.ConvertTimeFromSecond((int)audioObject.audioSource.clip.length));
            _music.position = GUILayout.HorizontalSlider(music.position, 0.0f, audioObject.audioSource.clip.length - 0.01f);
            audioObject.audioSource.time = music.position >= audioLength ? 0.0f : music.position;
        }

        /// <summary>
        /// ユーザーインターフェース部表示
        /// </summary>
        public void UserInterface()
        {
            if (currentState == AudioState.Playing)
            {
                PauseButton();
            }
            else
            {
                PlayButton();
            }
        }

        private void PlayButton()
        {
            if (GUILayout.Button("Play", NoteCreaterDefine.LAYOUT_BUTTON_WIDTH))
            {
                PlayBGM();
            }
        }

        private void PauseButton()
        {
            if (GUILayout.Button("Pause", NoteCreaterDefine.LAYOUT_BUTTON_WIDTH))
            {
                PauseBGM();
            }
        }

        private void PlayBGM()
        {
            if (!audioObject.audioSource) return;
            if (currentState == AudioState.Playing) return;
            audioObject.audioSource.Play();
            currentState = AudioState.Playing;
        }

        private void PauseBGM()
        {
            if (!audioObject.audioSource) return;
            if (currentState == AudioState.Pause) return;
            audioObject.audioSource.Pause();
            currentState = AudioState.Pause;
        }

        /// <summary>
        /// 現在再生中か
        /// </summary>
        /// <returns></returns>
        public bool IsPlaying()
        {
            return currentState == AudioState.Playing;
        }

        /// <summary>
        /// 曲が設定されているか
        /// </summary>
        /// <returns></returns>
        public bool IsSettingAudio()
        {
            return audio.Value != null;
        }

        /// <summary>
        /// AudioSourceを最新の状態に更新する
        /// 新しくAudioが設定されたときに呼ぶ
        /// </summary>
        private void UpdateAudioSource()
        {
            if (audioObject.audioSource.clip == audio.Value) return;
            audioObject.audioSource.clip = audio.Value;

            //Audioの割り当てをなくす場合はこれ以降の処理をしない
            if (audio.Value == null)
            {
                _music.name = "";
                audioLength = 0.0f;
                return;
            }

            _music.name = audioObject.audioSource.clip.name;
            audioObject.audioSource.time = 0.0f;
            audioLength = audioObject.audioSource.clip.length;

            //再生してから一時停止すると再生前に再生場所を変更できるようになる
            //特定の場所から再生したいときにこの処理をしないとうまくいかない
            audioObject.audioSource.Play();
            audioObject.audioSource.Pause();
        }

    }
}
