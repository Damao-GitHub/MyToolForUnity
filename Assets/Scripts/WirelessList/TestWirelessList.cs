/****************************************************
    文件：TestWirelessList.cs
	作者：大毛
    邮箱: 455267823@qq.com
    日期：2021/10/13 11:7:47
	功能：无限列表测试脚本
****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestWirelessList : InfinityScrollList
{
    List<InfinityScollItemData> m_dataList = new List<InfinityScollItemData>();    //准备好的数据

    public void Start()
    {
        for (int i = 0; i < 1000; i++)
        {
            InfinityScollItemData data = new InfinityScollItemData();
            data.index = i;
            m_dataList.Add(data);
        }
        Init(m_dataList);    //初始化
    }
}