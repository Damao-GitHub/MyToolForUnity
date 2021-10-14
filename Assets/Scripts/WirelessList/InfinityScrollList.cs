/****************************************************
    文件：InfinityScrollList.cs
	作者：大毛
    邮箱: 455267823@qq.com
    日期：2021/10/13 10:59:54
	功能：无限列表
****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfinityScrollList : MonoBehaviour
{
    private int m_headIndex = 0;                                // 顶部显示的数据索引
    private int m_tailIndex = 0;                                // 最底部显示数据索引
    private float m_itemHeight = 0;                             // item高度
    private float m_itemWidth = 0;                              // item宽度
    public float m_rowSpacing = 0;                              // 行间距
    public float m_columnSpacing = 0;                           // 行间距
    private int m_maxItemCount;                                  // 最大item数量（最好总高度刚好超过滚动框高度）
    [Tooltip("控制是否为上下滑动")]
    public bool m_isRow = true;                                 // 控制横向滑动还是纵向滑动
    [Tooltip("行或列展示数量")]
    public int m_showNum = 1;

    private List<Transform> m_itemList = new List<Transform>(); // item列表
    [SerializeField]
    private GameObject m_itemPrefab;
    [SerializeField]
    private ScrollRect m_scrollView;
    [SerializeField]
    private Transform m_content;
    private RectTransform m_contentRect;

    private int DataCount { get; set; } = 0;                    // 待填充数据数量
    private List<InfinityScollItemData> itemDatas;              // 所有Item数据

    /// <summary>
    /// 初始化必要数据
    /// </summary>
    /// <param name="maxData">数据数量</param>
    /// <param name="SetItemCallBack">修改item信息的回调</param>
    public void Init(List<InfinityScollItemData> itemData)
    {
        m_scrollView.onValueChanged.RemoveAllListeners();
        m_contentRect = m_content.GetComponent<RectTransform>();
        itemDatas = itemData;
        DataCount = itemDatas.Count;
        m_itemHeight = m_itemPrefab.GetComponent<RectTransform>().rect.height + m_rowSpacing;
        m_itemWidth = m_itemPrefab.GetComponent<RectTransform>().rect.width + m_columnSpacing;
        // 根据横向展示还是纵向展示区分
        if (m_isRow)
        {
            m_scrollView.onValueChanged.AddListener(this.OnValueChangedRow);
            m_contentRect.sizeDelta = new Vector2(m_itemWidth * m_showNum, m_itemHeight * (int)Math.Ceiling((double)DataCount / m_showNum));
            m_maxItemCount = ((int)Math.Ceiling((double)m_scrollView.GetComponent<RectTransform>().rect.height / m_itemHeight) + 2) * m_showNum;
        }
        else
        {
            m_scrollView.onValueChanged.AddListener(this.OnValueChangedcolumn);
            m_contentRect.sizeDelta = new Vector2(m_itemWidth * (int)Math.Ceiling((double)DataCount / m_showNum), m_itemHeight * m_showNum);
            m_maxItemCount = ((int)Math.Ceiling((double)m_scrollView.GetComponent<RectTransform>().rect.width / m_itemWidth) + 2) * m_showNum;
        }
        m_maxItemCount = m_maxItemCount > itemDatas.Count ? itemDatas.Count : m_maxItemCount;
        m_scrollView.horizontal = !m_isRow;
        m_scrollView.vertical = m_isRow;

        UpdateContent();
    }

    /// <summary>
    /// 数据发生变化时刷新item显示内容
    /// </summary>
    /// <param name="toTop"> 是否回到顶部</param>
    public void DoForceUpdate(bool toTop)
    {
        if (toTop)
        {
            m_headIndex = 0;
        }
        UpdateContent();
    }

    /// <summary>
    /// 根据首尾数据索引，刷新item内容
    /// </summary>
    private void UpdateContent()
    {
        for (int i = 0; i < m_itemList.Count; i++)
        {
            if (i >= DataCount)
                m_itemList[i].gameObject.SetActive(false);
        }

        int itemCount = Mathf.Clamp(DataCount, 0, m_maxItemCount);
        //修正从后向前移除节点时，尾部索引越界的bug
        if (m_headIndex + itemCount > DataCount)
        {
            m_headIndex = DataCount - itemCount;
        }

        Transform tran;
        for (Int32 i = 0; i < itemCount; i++)
        {
            if (i < m_itemList.Count)
            {
                tran = m_itemList[i];
            }
            else
            {
                var item = GameObject.Instantiate(m_itemPrefab);//m_itemPrefab.Instantiate();
                tran = item.transform;
                tran.SetParent(m_content, false);
                m_itemList.Add(tran);
            }
            int index = m_headIndex + i;
            tran.name = index.ToString();
            if(m_isRow)
                tran.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_itemWidth * (index % m_showNum), -m_itemHeight * (index / m_showNum));
            else
                tran.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_itemWidth * (index / m_showNum), -m_itemHeight * (index % m_showNum));

            // 更新item数据
            tran.GetComponent<InfinityScollItem>().RefreshUI(itemDatas[index]);
        }
        m_tailIndex = m_headIndex + (itemCount - 1);
    }

    private void OnValueChangedRow(Vector2 v2)
    {
        if (m_itemList.Count <= 0)
        {
            return;
        }

        //向上滚动
        while (m_contentRect.anchoredPosition.y > Math.Ceiling((double)m_headIndex / m_showNum) * m_itemHeight && m_tailIndex != DataCount - 1)
        {
            for (int i = 0; i < m_showNum; i++)
            {
                if (m_tailIndex == DataCount - 1) return;
                Transform _first = m_itemList[0];
                RectTransform _firstRect = _first.GetComponent<RectTransform>();

                //将顶部item移到底部
                m_itemList.RemoveAt(0);
                m_itemList.Add(_first);

                m_headIndex++;
                m_tailIndex++;

                _firstRect.anchoredPosition = new Vector2(m_itemWidth * (m_tailIndex % m_showNum), -m_itemHeight * (m_tailIndex / m_showNum));

                //修改显示
                _first.name = m_tailIndex.ToString();
                // 更新Item数据
                _first.GetComponent<InfinityScollItem>().RefreshUI(itemDatas[m_tailIndex]);
            }
        }

        //向下滚动
        while (m_contentRect.anchoredPosition.y < Math.Ceiling((double)m_headIndex / m_showNum) * m_itemHeight && m_headIndex != 0)
        {
            for (int i = 0; i < m_showNum; i++)
            {
                if (m_headIndex == 0) return;
                Transform _last = m_itemList[m_itemList.Count - 1];
                RectTransform _lastRect = _last.GetComponent<RectTransform>();

                m_itemList.RemoveAt(m_itemList.Count - 1);
                m_itemList.Insert(0, _last);

                m_headIndex--;
                m_tailIndex--;

                _lastRect.anchoredPosition = new Vector2(m_itemWidth * (m_headIndex % m_showNum), -m_itemHeight * (m_headIndex / m_showNum));
                _last.name = m_headIndex.ToString();
                // 更新Item数据
                _last.GetComponent<InfinityScollItem>().RefreshUI(itemDatas[m_headIndex]);
            }
        }
    }
    private void OnValueChangedcolumn(Vector2 v2)
    {
        if (m_itemList.Count <= 0)
        {
            return;
        }
        //向右滚动
        while (m_contentRect.anchoredPosition.x < -Math.Ceiling((double)m_headIndex / m_showNum) * m_itemWidth && m_tailIndex != DataCount - 1)
        {
            for (int i = 0; i < m_showNum; i++)
            {
                if (m_tailIndex == DataCount - 1) return;
                Transform _first = m_itemList[0];
                RectTransform _firstRect = _first.GetComponent<RectTransform>();

                //将顶部item移到底部
                m_itemList.RemoveAt(0);
                m_itemList.Add(_first);

                m_headIndex++;
                m_tailIndex++;

                _firstRect.anchoredPosition = new Vector2(m_itemWidth * (m_tailIndex / m_showNum), -m_itemHeight * (m_tailIndex % m_showNum));

                //修改显示
                _first.name = m_tailIndex.ToString();
                // 更新Item数据
                _first.GetComponent<InfinityScollItem>().RefreshUI(itemDatas[m_tailIndex]);
            }
        }

        //向左滚动
        while (m_contentRect.anchoredPosition.x > -Math.Ceiling((double)m_headIndex / m_showNum) * m_itemWidth && m_headIndex != 0)
        {
            for (int i = 0; i < m_showNum; i++)
            {
                if (m_headIndex == 0) return;
                Transform _last = m_itemList[m_itemList.Count - 1];
                RectTransform _lastRect = _last.GetComponent<RectTransform>();

                m_itemList.RemoveAt(m_itemList.Count - 1);
                m_itemList.Insert(0, _last);

                m_headIndex--;
                m_tailIndex--;

                _lastRect.anchoredPosition = new Vector2(m_itemWidth * (m_headIndex / m_showNum), -m_itemHeight * (m_headIndex % m_showNum));
                _last.name = m_headIndex.ToString();
                // 更新Item数据
                _last.GetComponent<InfinityScollItem>().RefreshUI(itemDatas[m_headIndex]);
            }
        }
    }
}
[System.Serializable]
public class InfinityScollItemData : MonoBehaviour
{
    // 作为基类  此类内容可删除  新的ItemData类必须继承此类使用
    public int index = 0;
}