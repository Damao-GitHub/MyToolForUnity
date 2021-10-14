/****************************************************
    文件：InfinityScollItem.cs
	作者：大毛
    邮箱: 455267823@qq.com
    日期：2021/10/13 11:46:1
	功能：无限列表Item类
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityScollItem : MonoBehaviour
{
    [HideInInspector]
    public InfinityScollItemData data;

    // 此处为实例   建议正式使用的时候继承此类
    public virtual void RefreshUI(InfinityScollItemData data)
    {
        this.data = data;
        // 正式使用时删除此处
        transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = data.index.ToString();
    }
}
