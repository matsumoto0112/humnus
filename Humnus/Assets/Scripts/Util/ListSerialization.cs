using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// リストをシリアライズ化する
/// </summary>
/// <typeparam name="T">リストの要素</typeparam>
[System.Serializable]
public class ListSerialization<T>
{
    [SerializeField]
    private List<T> list; //リスト

    /// <summary>
    /// リストに変換する
    /// </summary>
    /// <returns></returns>
    public List<T> ToList() { return list; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="list">変換したいリスト</param>
    public ListSerialization(List<T> list)
    {
        this.list = list;
    }
}
