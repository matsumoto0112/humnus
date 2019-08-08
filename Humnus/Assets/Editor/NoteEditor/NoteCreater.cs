using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Assertions;
using System.IO;

namespace NoteEditor
{
    public class NoteCreater : EditorWindow
    {
        private AudioPlayer audioPlayer;
        private List<Note> notes;
        private List<LongNote> longNotes;

        private ReactiveProperty<NoteType> currentEditMode;
        private LongNote lastLongNote;
        private NoteEditorLayout layout;
        private AudioWave wave;

        [MenuItem("Editor/NoteEditor")]
        private static void Create()
        {
            NoteCreater window = GetWindow<NoteCreater>("NoteEditor");
            window.minSize = window.maxSize = NoteCreaterDefine.WINDOW_SIZE;
        }

        private void Awake()
        {
            notes = new List<Note>();
            longNotes = new List<LongNote>();
            currentEditMode = new ReactiveProperty<NoteType>
            {
                Value = NoteType.Normal
            };
            currentEditMode.Subscribe(_ => LongNoteEditReset());
            lastLongNote = null;
            layout = LoadLayoutDefineObject();
            wave = new AudioWave();
            audioPlayer = new AudioPlayer();
        }

        /// <summary>
        /// レイアウトの定義をしているオブジェクトを読み込む
        /// </summary>
        /// <returns></returns>
        private NoteEditorLayout LoadLayoutDefineObject()
        {
            //まずこのファイルの保存されているパスを取得
            MonoScript mono = MonoScript.FromScriptableObject(this);
            string currentPath = AssetDatabase.GetAssetPath(mono);
            //最後に保存されたパスの情報が入っているデータを読み込む
            string filename = System.IO.Path.GetFileName(currentPath);
            currentPath = currentPath.Replace(filename, "");
            return AssetDatabase.LoadAssetAtPath<NoteEditorLayout>(currentPath + typeof(NoteEditorLayout).Name + ".asset");
        }

        private void OnGUI()
        {
            RecieveMusic();
            if (!CanEdit()) return;
            PrintInfo();
            ChangableInfo();
            TopButtons();
            PlaySlider();
            NoteEditorMain();
            DrawWave();
            BottomButtons();
        }

        private void RecieveMusic()
        {
            //楽曲入力
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Music Name");
                audioPlayer.AudioField();
            }
        }

        /// <summary>
        /// 楽曲情報の表示
        /// </summary>
        private void PrintInfo()
        {
            using (new GUILayout.HorizontalScope())
            {
                audioPlayer.PrintAudioName();
            }
        }

        /// <summary>
        /// 変更可能な情報の表示
        /// </summary>
        private void ChangableInfo()
        {
            using (new GUILayout.HorizontalScope())
            {
                audioPlayer.BPMField();
            }
        }

        /// <summary>
        /// ボタンの処理
        /// </summary>
        private void TopButtons()
        {
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                AudioPlayerUI();
                currentEditMode.Value = (NoteType)GUILayout.Toolbar((int)currentEditMode.Value, layout.GetNames());
                GUILayout.FlexibleSpace();
                ClearButton();
            }
        }

        private void BottomButtons()
        {
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                IOButton();
            }
        }

        /// <summary>
        /// 再生時間のスライダー
        /// </summary>
        private void PlaySlider()
        {
            using (new GUILayout.HorizontalScope(GUI.skin.box))
            {
                audioPlayer.PlaySlider();
            }
        }

        private bool IsAddNoteEvent(NoteEditorInput.InputType inputType)
        {
            return inputType == NoteEditorInput.InputType.Click;
        }

        private bool IsRemoveNoteEvent(NoteEditorInput.InputType inputType)
        {
            return inputType == NoteEditorInput.InputType.RightClick;
        }

        /// <summary>
        /// ノーツエディターメイン部分
        /// </summary>
        private void NoteEditorMain()
        {
            Rect labelArea = GUILayoutUtility.GetRect(NoteCreaterDefine.MAIN_EDITOR_AREA_SIZE.x, 15);
            Rect area = GUILayoutUtility.GetRect(NoteCreaterDefine.MAIN_EDITOR_AREA_SIZE.x, NoteCreaterDefine.MAIN_EDITOR_AREA_SIZE.y);
            DrawAxis(area, labelArea);
            DrawNotes(area);

            //入力を調べ、入力の種類に応じて処理を行う
            NoteEditorInput.InputType inputType = NoteEditorInput.GetCurrentInputType();
            Vector2 mousePosition = NoteEditorInput.GetMousePosition();
            if (!IsClickPositionInArea(mousePosition, area)) return;

            if (IsAddNoteEvent(inputType))
            {
                AddNoteIfNotContains(mousePosition, area);
            }
            else if (IsRemoveNoteEvent(inputType))
            {
                RemoveNoteIfExist(mousePosition, area);
            }
        }

        /// <summary>
        /// ノーツの描画処理
        /// </summary>
        /// <param name="area"></param>
        private void DrawNotes(Rect area)
        {
            notes.ForEach(note => DrawNote(note, area));
            longNotes.ForEach(note => DrawLongNote(note, area));
        }

        /// <summary>
        /// ノーツがすでに同じ場所に作られていなければ新しく追加する
        /// </summary>
        /// <param name="mousePosition">マウスの座標</param>
        /// <param name="area">ノーツエディターの範囲</param>
        private void AddNoteIfNotContains(Vector2 mousePosition, Rect area)
        {
            //長押しノーツの場合は特殊な処理が入るため別枠で処理
            if (currentEditMode.Value == NoteType.Long)
            {
                AddLongNote(mousePosition, area);
            }
            else
            {
                Note newNote = CreateNote(mousePosition, area, currentEditMode.Value);
                if (IsExistNote(newNote.position))
                {
                    return;
                }
                notes.Add(newNote);
            }
        }

        /// <summary>
        /// 長押しノーツの追加
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="area"></param>
        private void AddLongNote(Vector2 mousePosition, Rect area)
        {
            LongNote newNote = CreateLongNote(mousePosition, area);
            //長押しノーツは同じX軸上には存在できないのでその場合は追加しない
            if (lastLongNote != null && newNote.position.X == lastLongNote.position.X)
            {
                return;
            }

            if (IsExistNote(newNote.position))
            {
                return;
            }

            //始点ノーツの場合
            if (lastLongNote == null)
            {
                lastLongNote = newNote;
                longNotes.Add(newNote);
            }
            //終点ノーツの場合
            else
            {
                lastLongNote.SetOtherNote(newNote);
                lastLongNote = null;
                //始点ノーツに終点が含まれるためリストには追加しない
            }
        }

        /// <summary>
        /// ノーツがすでに同じ場所に存在するか
        /// </summary>
        /// <param name="position">調べたい座標</param>
        /// <returns>存在していたらtrueを返す</returns>
        private bool IsExistNote(NoteVec2 position)
        {
            //同じ座標にあるノーツを検索する
            Note find = notes.Find(n => n.position == position);
            if (find != null) return true;
            find = longNotes.Find(n => n.position == position);
            if (find != null) return true;
            return false;
        }

        /// <summary>
        /// クリックされた場所にノーツがあればそのノーツを削除する
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="area"></param>
        private void RemoveNoteIfExist(Vector2 mousePosition, Rect area)
        {
            NoteVec2 position = CalculateNotePosition(mousePosition, area);
            RemoveNoteFromNotes(position);
            RemoveNoteFromLongNotes(position);
        }

        /// <summary>
        /// ノーツリストから指定された座標にあるノーツを削除する
        /// </summary>
        /// <param name="position"></param>
        private void RemoveNoteFromNotes(NoteVec2 position)
        {
            int removeIndex = notes.FindIndex(note => note.position == position);
            if (removeIndex != -1)
            {
                notes.RemoveAt(removeIndex);
            }
        }

        /// <summary>
        /// 長押しノーツリストから指定された座標にあるノーツを削除する
        /// </summary>
        /// <param name="position"></param>
        private void RemoveNoteFromLongNotes(NoteVec2 position)
        {
            int removeIndex = longNotes.FindIndex(note => note.other.position == position);
            if (removeIndex != -1)
            {
                longNotes.RemoveAt(removeIndex);
            }

            removeIndex = longNotes.FindIndex(note => note.position == position);
            if (removeIndex != -1)
            {
                longNotes.RemoveAt(removeIndex);
            }
        }

        /// <summary>
        /// クリックした座標がエディターの範囲内か
        /// </summary>
        /// <param name="mousePosition">マウス座標</param>
        /// <param name="area">エディターの範囲</param>
        /// <returns></returns>
        private bool IsClickPositionInArea(Vector3 mousePosition, Rect area)
        {
            return area.Contains(mousePosition);
        }

        /// <summary>
        /// 軸の描画
        /// </summary>
        /// <param name="area">描画する範囲</param>
        private void DrawAxis(Rect area, Rect labelArea)
        {
            DrawXAxis(area, labelArea, NoteCreaterDefine.AXIS_WIDTH.x, NoteCreaterDefine.AXIS_OFFSET.x);
            DrawYAxis(area, NoteCreaterDefine.AXIS_WIDTH.y, NoteCreaterDefine.AXIS_OFFSET.y);
        }

        /// <summary>
        /// X軸描画
        /// </summary>
        /// <param name="area">描画する範囲</param>
        /// <param name="width">軸の幅</param>
        /// <param name="offset">軸のオフセット</param>
        private void DrawXAxis(Rect area, Rect labelArea, float width, float offset)
        {
            //現在の再生ノーツ地点に赤線描画
            Handles.color = Color.red;
            Handles.DrawLine(new Vector3(offset, area.yMin, 0.0f), new Vector3(offset, area.yMax, 0.0f));

            Handles.color = Color.gray;

            for (int i = 0; i < Mathf.CeilToInt(audioPlayer.audioLength) * 4; i++)
            {
                float x = NoteEditorUtil.CalculateCurrentAudioPositionXOnGUI(audioPlayer.music, i, offset);
                //4本に一本実線を描画
                if (i % 4 == 0)
                {
                    Handles.color = NoteCreaterDefine.AXIS_PER_4_BEATS;
                    //実線の上に数字の描画
                    Handles.Label(new Vector3(x, labelArea.yMin, 0.0f), "" + (i / 4));
                }
                else
                {
                    Handles.color = NoteCreaterDefine.AXIS_NORMAL;
                }
                Vector3 p1 = new Vector3(x, area.yMax, 0.0f);
                Vector3 p2 = new Vector3(x, area.yMin, 0.0f);
                Handles.DrawLine(p1, p2);
            }
        }

        /// <summary>
        /// Y軸描画
        /// </summary>
        /// <param name="area">描画する範囲</param>
        /// <param name="width">軸の幅</param>
        /// <param name="offset">軸のオフセット</param>
        private void DrawYAxis(Rect area, float width, float offset)
        {
            Handles.color = Color.gray;
            //描画範囲からoffset分開けた部分をwidth間隔で線描画
            for (float y = area.yMin + offset; y <= area.yMax - offset; y += width)
            {
                Handles.DrawLine(new Vector3(area.xMin, y, 0.0f), new Vector3(area.xMax, y, 0.0f));
            }
        }

        /// <summary>
        /// 再生ボタン
        /// </summary>
        private void AudioPlayerUI()
        {
            audioPlayer.UserInterface();
        }

        /// <summary>
        /// クリアボタンの表示
        /// </summary>
        private void ClearButton()
        {
            if (GUILayout.Button("Clear", NoteCreaterDefine.LAYOUT_BUTTON_WIDTH))
            {
                notes.Clear();
                longNotes.Clear();
            }
        }

        private void Update()
        {
            //こまめに画面の再描画を行う
            Repaint();
        }

        /// <summary>
        /// 波形描画
        /// </summary>
        private void DrawWave()
        {
            using (new GUILayout.HorizontalScope())
            {
                //現在再生している場所から波形データを取得
                if (audioPlayer.IsPlaying())
                {
                    wave.Sample();
                }
                Handles.color = Color.black;
                Rect area = GUILayoutUtility.GetRect(NoteCreaterDefine.WINDOW_SIZE.x, 100.0f);
                wave.Draw(area);
            }
        }

        /// <summary>
        /// 編集可能か?
        /// </summary>
        /// <returns></returns>
        private bool CanEdit()
        {
            return audioPlayer.IsSettingAudio();
        }

        /// <summary>
        /// Import/Exportボタンの配置
        /// </summary>
        private void IOButton()
        {
            if (GUILayout.Button("Import", NoteCreaterDefine.LAYOUT_BUTTON_WIDTH))
            {
                Import();
            }
            if (GUILayout.Button("Export", NoteCreaterDefine.LAYOUT_BUTTON_WIDTH))
            {
                Export();
            }
        }

        /// <summary>
        /// マウスの座標からクリック地点にノーツを生成する
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        private Note CreateNote(Vector2 mousePosition, Rect area, NoteType type)
        {
            return new Note(CalculateNotePosition(mousePosition, area), type);
        }

        /// <summary>
        /// マウス座標からノーツの配置予定座標を計算する
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        private NoteVec2 CalculateNotePosition(Vector2 mousePosition, Rect area)
        {
            //四捨五入だけで座標を割り出せるようにするためにオフセットを計算する
            mousePosition -= area.min;
            mousePosition -= NoteCreaterDefine.AXIS_OFFSET;
            mousePosition.x += audioPlayer.music.position * NoteEditorUtil.CalculateCurrentXAxisWidthPerBeats(audioPlayer.music) * 4;

            int X = (int)(Mathf.Round(mousePosition.x / NoteEditorUtil.CalculateCurrentXAxisWidthPerBeats(audioPlayer.music)));
            int laneNum = mousePosition.y < NoteCreaterDefine.AXIS_WIDTH.y ? 0 : 1;
            return new NoteVec2(X, laneNum);
        }

        /// <summary>
        /// 長押し用ノーツを作成する
        /// 長押しノーツは特殊な生成方法なため
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="area"></param>
        /// <returns>作成できなかった場合はnullを返す。
        private LongNote CreateLongNote(Vector2 mousePosition, Rect area)
        {
            return new LongNote(CalculateNotePosition(mousePosition, area));
        }

        /// <summary>
        /// ノーツの描画
        /// </summary>
        /// <param name="note"></param>
        /// <param name="area"></param>
        private void DrawNote(Note note, Rect area)
        {
            Handles.color = layout.GetColor(note.type);
            Handles.DrawSolidDisc(NoteEditorUtil.CalculateNotePositionOnMainEditor(audioPlayer.music, note, area), Vector3.forward, 10.0f);
        }

        /// <summary>
        /// 長押しノーツの描画
        /// </summary>
        /// <param name="note"></param>
        /// <param name="area"></param>
        private void DrawLongNote(LongNote note, Rect area)
        {
            Handles.color = layout.GetColor(note.type);
            DrawNote(note, area);
            if (note.other != null)
            {
                DrawNote(note.other, area);
                Handles.DrawLine(NoteEditorUtil.CalculateNotePositionOnMainEditor(audioPlayer.music, note, area),
                    NoteEditorUtil.CalculateNotePositionOnMainEditor(audioPlayer.music, note.other, area));
            }
        }

        /// <summary>
        /// 長押しノーツの編集状態をリセットする
        /// </summary>
        private void LongNoteEditReset()
        {
            lastLongNote = null;
            longNotes.RemoveAll(n => n.other == null);
        }

        /// <summary>
        /// ノーツ情報を取り込む
        /// </summary>
        private void Import()
        {
            string loadFilePath = EditorUtility.OpenFilePanel("Import", "Assets/Datas", "json");
            if (string.IsNullOrEmpty(loadFilePath)) return;
            using (StreamReader sr = new StreamReader(loadFilePath, false))
            {
                string json = sr.ReadToEnd();
                List<Note> loadedNotes = JsonConverter.FromJson(json);
                notes.Clear();

                //ロングノーツまでのノーツを格納する
                IEnumerable<Note> beforeLongNotesList = loadedNotes.TakeWhile(n => n.type != NoteEditor.NoteType.Long);
                notes.AddRange(beforeLongNotesList);

                //ロングノーツはそれ用にデータを変換する
                loadedNotes.RemoveRange(0, beforeLongNotesList.Count());
                InitLongNotes(loadedNotes);
            }
        }

        /// <summary>
        /// 長押しノーツの初期化処理
        /// </summary>
        /// <param name="list"></param>
        private void InitLongNotes(IReadOnlyCollection<Note> list)
        {
            longNotes.Clear();
            for (int i = 0; i < list.Count; i += 2)
            {
                LongNote begin = new LongNote(list.ElementAt(i).position);
                begin.SetOtherNote(new LongNote(list.ElementAt(i + 1).position));
                longNotes.Add(begin);
            }
        }

        /// <summary>
        /// ノーツ情報を書き出す
        /// </summary>
        private void Export()
        {
            string saveFilePath = EditorUtility.SaveFilePanel("Export", "Assets/Datas", "default", "json");
            if (string.IsNullOrEmpty(saveFilePath)) return;
            using (StreamWriter sw = new StreamWriter(saveFilePath, false))
            {
                //変換するデータを用意する
                List<Note> originalData = new List<Note>();
                notes.Sort(Note.CompareFunc);
                originalData.AddRange(notes);

                //ロングノーツ用の出力データを用意する
                longNotes.Sort(Note.CompareFunc);
                foreach (var ln in longNotes)
                {
                    if (ln.other == null) continue;
                    originalData.Add(ln);
                    originalData.Add(ln.other);
                }

                string json = JsonConverter.ToJson(originalData);
                sw.WriteLine(json);
                sw.Flush();
            }
        }
    }
}