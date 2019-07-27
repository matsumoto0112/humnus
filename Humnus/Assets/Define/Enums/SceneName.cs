//自動生成スクリプト
using System.Collections.Generic;
using System.Linq;

public enum SceneName
{
    SampleScene,
}
public static class SceneNameManager
{
    public static Dictionary<SceneName, string> scenenames = new Dictionary<SceneName, string> 
    {
        {SceneName.SampleScene,"SampleScene"},
    };
    /// <summary>
    /// 文字列で取得する
    /// </summary>
    public static string GetString(this SceneName scenename)
    {
        return scenenames[scenename];
    }
    /// <summary>
    /// SceneNameで取得する
    /// </summary>
    public static SceneName GetSceneName(string name)
    {
        return scenenames.FirstOrDefault(pair => pair.Value == name).Key;
    }
}
