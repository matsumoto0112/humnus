using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ButtonNameToString
{
    /// <summary>
    /// InputManagerに登録されたボタンの名前を取得
    /// </summary>
    /// <returns>重複を削除されたリストを返す</returns>
    public static string[] GetButtonString()
    {
        List<string> buttonStringList = new List<string>();
        //ProjectSettingにあるInputManagerをシリアライズオブジェクトとして開く
        SerializedObject buttonObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
        //ボタンの定義部分のプロパティを取得
        SerializedProperty buttonProperty = buttonObject.FindProperty("m_Axes");
        for (int i = 0; i < buttonProperty.arraySize; i++)
        {
            //一つずづ名前を取得
            SerializedProperty prop = buttonProperty.GetArrayElementAtIndex(i);
            buttonStringList.Add(GetChildProperty(prop, "m_Name").stringValue);
        }
        //重複を削除し返す
        return buttonStringList.Distinct().ToArray();
    }


    /// <summary>
    /// 子プロパティを取得
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
    {
        SerializedProperty child = parent.Copy();
        child.Next(true);
        do
        {
            if (child.name == name) return child;
        } while (child.Next(false));
        return null;
    }
}
