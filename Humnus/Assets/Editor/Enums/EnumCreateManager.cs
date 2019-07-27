using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EnumCreateManager : EditorWindow
{

    // 無効な文字を管理する配列
    private static readonly string[] INVALUD_CHARS =
    {
        " ", "!", "\"", "#", "$",
        "%", "&", "\'", "(", ")",
        "-", "=", "^",  "~", "\\",
        "|", "[", "{",  "@", "`",
        "]", "}", ":",  "*", ";",
        "+", "/", "?",  ".", ">",
        ",", "<",
    };

    private struct EnumCreateInfo
    {
        public delegate string[] GetNames();
        /// <summary>
        /// クラス名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// Enum名を取得するメソッド
        /// </summary>
        public GetNames getNamesMethod { get; set; }
        public EnumCreateInfo(string name, GetNames getNamesMethod)
        {
            this.name = name;
            this.getNamesMethod = getNamesMethod;
        }
    }

    /// <summary>
    /// 作成用データ
    /// </summary>
    private static Dictionary<CreateEnumType, EnumCreateInfo> infos = new Dictionary<CreateEnumType, EnumCreateInfo>
    {
        {CreateEnumType.Tag,  new EnumCreateInfo("TagName",()=> {return UnityEditorInternal.InternalEditorUtility.tags; }) },
        {CreateEnumType.Layer,new EnumCreateInfo("LayerName",()=> { return UnityEditorInternal.InternalEditorUtility.layers; }) },
        {CreateEnumType.SortingLayer,  new EnumCreateInfo("SortingLayerName",()=> {return SortingLayer.layers.Select(s => s.name).ToArray(); }) },
        {CreateEnumType.Button,new EnumCreateInfo("ButtonName",ButtonNameToString.GetButtonString) },
        {CreateEnumType.Scene,new EnumCreateInfo("SceneName",()=> {return EditorBuildSettings.scenes.Select(s =>System.IO.Path.GetFileNameWithoutExtension(s.path)).ToArray(); }) }
    };

    /// <summary>
    /// 無効な文字の削除
    /// </summary>
    /// <param name="str">削除する文字列</param>
    /// <returns>無効文字を削除された文字列</returns>
    private static string RemoveInvalidChars(string str)
    {
        //無効文字が含まれていたら削除する
        System.Array.ForEach(INVALUD_CHARS, c => str = str.Replace(c, string.Empty));
        return str;
    }

    /// <summary>
    /// 作成できるか
    /// コンパイル中などは作成できない
    /// </summary>
    /// <returns></returns>
    private static bool CanCreate()
    {
        //デバッグ実行中ならfalse
        if (EditorApplication.isPlaying) return false;
        //アプリケーションが実行中ならfalse
        if (Application.isPlaying) return false;
        //コンパイル中ならfalse
        if (EditorApplication.isCompiling) return false;
        return true;
    }

    private bool initialized = false;
    private EnumCreateScriptableObject savePathData;

    [MenuItem("Editor/EnumCreate")]
    private static void Create()
    {
        if (!CanCreate()) return;
        GetWindow<EnumCreateManager>("EnumCreater");
    }

    [MenuItem("Editor/EnumCreate", true)]
    private static bool CanSelectMenu()
    {
        return CanCreate();
    }

    private void OnGUI()
    {
        //初期化していなければ初期化する
        if (!initialized)
        {
            Init();
        }
        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Label("保存するパス", GUILayout.Width(100));
            savePathData.savePath = GUILayout.TextField(savePathData.savePath, GUILayout.Width(200));
        }

        foreach (var info in infos)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(info.Value.name, GUILayout.Width(100));
                if (GUILayout.Button("作成", GUILayout.Width(100)))
                {
                    CreateEnum(info.Key);
                    continue;
                }
            }
        }
    }

    private void Init()
    {
        initialized = true;
        //ScriptableObjectのファイルを検索する
        string[] findAssetNames = AssetDatabase.FindAssets("t:" + typeof(EnumCreateScriptableObject).Name);
        //ファイルがなければメッセージを出して終了
        if (findAssetNames.Length == 0)
        {
            Debug.LogError("保存するパスを保存しておくScriptableObjectがありません。\n作成後に再度実行してください。");
            return;
        }

        //まずこのファイルの保存されているパスを取得
        MonoScript mono = MonoScript.FromScriptableObject(this);
        string currentPath = AssetDatabase.GetAssetPath(mono);
        //最後に保存されたパスの情報が入っているデータを読み込む
        string filename = System.IO.Path.GetFileName(currentPath);
        currentPath = currentPath.Replace(filename, "");
        //Assetをロード
        savePathData = AssetDatabase.LoadAssetAtPath<EnumCreateScriptableObject>(currentPath + typeof(EnumCreateScriptableObject).Name + ".asset");
    }

    /// <summary>
    /// クラスファイル名の取得
    /// </summary>
    /// <param name="className"></param>
    /// <returns></returns>
    private string GetClassFileName(string className)
    {
        return className + ".cs";
    }

    /// <summary>
    /// 保存先のフルパスを取得
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetSaveFullPath(CreateEnumType type)
    {
        string fullPath = Application.dataPath + "/" + savePathData.savePath + GetClassFileName(infos[type].name);
        return fullPath;
    }

    /// <summary>
    /// Enumの作成
    /// </summary>
    /// <param name="type"></param>
    private void CreateEnum(CreateEnumType type)
    {
        string[] names = infos[type].getNamesMethod();
        if (names.Length == 0)
        {
            Debug.Log("保存するデータが一つもありません。");
            return;
        }

        string className = infos[type].name;
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("//自動生成スクリプト");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Linq;");
        builder.AppendLine();
        //クラス名をつける
        builder.AppendFormat("public enum {0}", className).AppendLine();
        builder.AppendLine("{");

        //各要素をカンマ区切りで格納
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = RemoveInvalidChars(names[i]);
            builder.AppendLine("    " + names[i] + ",");
        }
        //enumの終わり
        builder.AppendLine("}");

        //classのManagerを作成
        builder.AppendFormat("public static class {0}Manager", className).AppendLine();
        builder.AppendLine("{");
        string classNameLower = className.ToLower();
        string dictionaryName = className.ToLower() + "s";
        //enumとstringのペアのディクショナリを作成
        builder.AppendFormat("    public static Dictionary<{0}, string> {1}s = new Dictionary<{0}, string> ", className, classNameLower).AppendLine();
        builder.AppendLine("    {");

        foreach (var name in names)
        {
            builder.AppendLine("        {" + className + "." + name + "," + "\"" + name + "\"},");
        }

        builder.AppendLine("    };");

        builder.AppendLine("    /// <summary>");
        builder.AppendLine("    /// 文字列で取得する");
        builder.AppendLine("    /// </summary>");
        builder.AppendFormat("    public static string GetString(this {0} {1})", className, classNameLower).AppendLine();
        builder.AppendLine("    {");
        builder.AppendFormat("        return {0}[{1}];", dictionaryName, classNameLower).AppendLine();
        builder.AppendLine("    }");

        builder.AppendLine("    /// <summary>");
        builder.AppendFormat("    /// {0}で取得する", className).AppendLine();
        builder.AppendLine("    /// </summary>");
        builder.AppendFormat("    public static {0} Get{1}(string name)", className, className).AppendLine();
        builder.AppendLine("    {");
        builder.AppendFormat("        return {0}.FirstOrDefault(pair => pair.Value == name).Key;", dictionaryName).AppendLine();
        builder.AppendLine("    }");

        builder.AppendLine("}");


        string filepath = GetSaveFullPath(type);
        //ディレクトリのパスを取得し、そこにファイルがなければ新しく作成
        string directoryName = System.IO.Path.GetDirectoryName(filepath);
        if (!System.IO.Directory.Exists(directoryName))
        {
            System.IO.Directory.CreateDirectory(directoryName);
        }
        //書き込み
        System.IO.File.WriteAllText(filepath, builder.ToString(), System.Text.Encoding.UTF8);
        Debug.Log(className + "を作成しました。\n");
    }
}
