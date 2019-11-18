using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    //タッチ状態の列挙型
    enum touchStatus
    {
        Began,
        Moved,
        Stationary,
        Ended,
        Canceled,
        None
    };
    //現在の状態
    touchStatus currentTouchStatus;

    [SerializeField, Header("フリック判定距離")]
    float flickDistance;

    Vector2 start;
    Vector2 end;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(isFlick(ref start, ref end));
    }

    /// <summary>
    /// タッチ状態の取得
    /// </summary>
    touchStatus GetTouchStatus()
    {
        if (!Application.isEditor)
        {
            if (Input.touchCount == 0)
                return touchStatus.None;

            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case (TouchPhase.Began):
                    return touchStatus.Began;

                case (TouchPhase.Moved):
                    return touchStatus.Moved;

                case (TouchPhase.Stationary):
                    return touchStatus.Stationary;

                case (TouchPhase.Ended):
                    return touchStatus.Ended;

                case (TouchPhase.Canceled):
                    return touchStatus.Canceled;
            }
        }

        if (Input.GetMouseButtonDown(0))
            return touchStatus.Began;
        if (Input.GetMouseButton(0))
        {
            end = Input.mousePosition;
            return touchStatus.Moved;
        }
        if (Input.GetMouseButtonUp(0))
        {
            start = Vector3.zero;
            end = Vector3.zero;
            return touchStatus.Ended;
        }

        return touchStatus.None;
    }

    /// <summary>
    /// タッチ座標の取得
    /// </summary>
    /// <param name="id"></param>
    /// <returns>座標</returns>
    Vector3 GetPosition(int id = 0)
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
                return Input.mousePosition;
        }
        else
        {
            if (Input.touchCount != 0)
            {
                Touch touch = Input.GetTouch(id);
                return touch.position;
            }
        }
        return Vector3.zero;
    }

    /// <summary>
    /// 複数座標取得
    /// </summary>
    /// <returns>タッチがないときはnullを返します</returns>
    Vector3[] GetPositions()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3[] pos = new Vector3[1];
                pos[0] = Input.mousePosition;
                return pos;
            }
        }
        else
        {
            if (Input.touchCount != 0)
            {
                Vector3[] pos = new Vector3[Input.touchCount];
                for (int i = 0; i < pos.Length; i++)
                {
                    pos[i] = Input.touches[i].position;
                }

                return pos;
            }
        }

        Vector3[] zero = new Vector3[1];
        zero[0] = Vector3.zero;
        return zero;
    }
    
    /// <summary>
    /// 現在のタッチ数を返す
    /// </summary>
    /// <returns>タッチ数</returns>
    int GetTouchCnt()
    {
        if (Application.isEditor)
        {
            if (Input.GetMouseButton(0))
                return 1;

            return 0;
        }

        else
            return Input.touchCount;
    }

    /// <summary>
    /// フリックされたかを取得する
    /// </summary>
    /// <param name="start">始点</param>
    /// <param name="end">終点</param>
    /// <returns>フリック判定</returns>
    bool isFlick(ref Vector2 start, ref Vector2 end)
    {
        if (GetTouchStatus() == touchStatus.Began)
        {
            if (Application.isEditor)
                start = Input.mousePosition;
            else
                start = Input.GetTouch(0).position;
        }

        // if (GetTouchStatus() == touchStatus.Ended)
        //end =nput.GetTouch(0).position;

        if (Vector2.Distance(start, end) >= flickDistance)
            return true;

        return false;
    }
}

public static class Wrapper
{

}
