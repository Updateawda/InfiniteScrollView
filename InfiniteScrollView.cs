using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

/// <summary>
/// 无限滑动列表
/// </summary>
public class InfiniteScrollView<T> where T : GridPartItem, new()
{
    /// <summary>
    /// 滑动框组件
    /// </summary>
    private ScrollRect scrollRect;
    /// <summary>
    /// 滑动框的Content
    /// </summary>
    private RectTransform content;
    /// <summary>
    /// 布局组件
    /// </summary>
    private GridLayoutGroup layout;//布局组件

    /// <summary>
    /// 滑动类型
    /// </summary>
    [Header("滑动类型")]
    public ScrollType scrollType;
    /// <summary>
    /// 生成初始化生成item数量
    /// </summary>
    private int fixedCount;
    [Header("Item的预制体")]
    public GameObject itemPrefab;
    /// <summary>
    /// 总的item数据数量
    /// </summary>
    private int totalCount;
    /// <summary>
    /// 数据实体类 RectTransform
    /// </summary>
    private List<RectTransform> dataList = new List<RectTransform>();
    private List<T> itemScriptsList = new List<T>();
    private List<object> objectList = new List<object>();
    /// <summary>
    /// 头下标
    /// </summary>
    private int headIndex;
    /// <summary>
    /// 尾下标
    /// </summary>
    private int tailIndex;
    /// <summary>
    /// 第一个Item的锚点坐标
    /// </summary>
    private Vector2 firstItemAnchoredPos;
    /// <summary>
    /// 获取mask初始宽高
    /// </summary>
    private Vector2 maskTranV2;
    /// <summary>
    /// 在mask初始宽高下,一行能放item最多数量,一列放item最多数量
    /// </summary>
    private int maxItemNumWide, maxItemNumHigh;
    #region Init

    /// <summary>
    /// 实例化Item
    /// </summary>
    private void InitItem()
    {
        int num = fixedCount <= totalCount ? fixedCount : totalCount;
        for (int i = 0; i < num; i++)
        {
            GameObject tempItem = GameObject.Instantiate(itemPrefab, content);
            dataList.Add(tempItem.GetComponent<RectTransform>());
            T itemScript = new T();
            itemScript.OnBindItemElements(tempItem);
            itemScriptsList.Add(itemScript);
            SetShow(itemScript, i);
        }
    }

    /// <summary>
    /// 设置Content大小
    /// </summary>
    private void SetContentSize()
    {
        if (scrollType == ScrollType.Horizontal)
        {
            content.sizeDelta = new Vector2
             (
                 layout.padding.left + layout.padding.right + (int)Math.Ceiling((float)totalCount / maxItemNumHigh) * (layoutCellSizeXAndSpacingX) - layout.spacing.x,
                 content.rect.height
             ); ;
        }
        else if (scrollType == ScrollType.Vertical)
        {
            content.sizeDelta = new Vector2
               (
                   content.rect.width,
                   layout.padding.top + layout.padding.bottom + (int)Math.Ceiling((float)totalCount / maxItemNumWide) * layoutCellSizeYAndSpacingY
               ); ;
        }
    }

    /// <summary>
    /// 设置布局
    /// </summary>
    private void SetLayout()
    {
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.constraint = Constraint.Flexible;
        if (scrollType == ScrollType.Horizontal)
        {
            layout.startAxis = GridLayoutGroup.Axis.Vertical;
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            fixedCount = (maxItemNumWide + 3) * maxItemNumHigh;
        }
        else if (scrollType == ScrollType.Vertical)
        {
            layout.startAxis = GridLayoutGroup.Axis.Horizontal;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            fixedCount = maxItemNumWide * (maxItemNumHigh + 3);
        }
    }

    /// <summary>
    /// 得到第一个数据的锚点位置
    /// </summary>
    private void GetFirstItemAnchoredPos()
    {
        firstItemAnchoredPos = new Vector2
            (
                layout.padding.left + layout.cellSize.x / 2,
                -layout.padding.top - layout.cellSize.y / 2
            );
        Debug.Log("第一个数据锚点:" + firstItemAnchoredPos);
    }

    #endregion

    #region Main

    /// <summary>
    /// 滑动中
    /// </summary>
    private void OnScroll(Vector2 v)
    {
        if (dataList.Count == 0)
        {
            Debug.LogWarning("先调用SetTotalCount方法设置数据总数量再调用Init方法进行初始化");
            return;
        }
        if (scrollType == ScrollType.Vertical)
        {
            //向上滑
            while (content.anchoredPosition.y >= (layout.padding.top + (headIndex / maxItemNumWide + 1) * layoutCellSizeYAndSpacingY)
            && tailIndex != totalCount - 1)
            {
                //将数据列表中的第一个元素移动到最后一个
                RectTransform item = dataList[0];
                dataList.Remove(item);
                dataList.Add(item);
                //设置位置
                SetPos(item, tailIndex + 1);

                T itemScript = itemScriptsList[0];
                itemScriptsList.Remove(itemScript);
                itemScriptsList.Add(itemScript);

                SetShow(itemScript, tailIndex + 1);
                headIndex++;
                tailIndex++;
            }
            //向下滑
            while (content.anchoredPosition.y <= (layout.padding.top + (headIndex / maxItemNumWide + 1) * layoutCellSizeYAndSpacingY)
                && headIndex != 0)
            {

                //将数据列表中的最后一个元素移动到第一个
                RectTransform item = dataList.Last();
                dataList.Remove(item);
                dataList.Insert(0, item);

                //设置位置
                SetPos(item, headIndex - 1);

                T itemScript = itemScriptsList.Last();
                itemScriptsList.Remove(itemScript);
                itemScriptsList.Insert(0, itemScript);
                SetShow(itemScript, headIndex - 1);

                headIndex--;
                tailIndex--;
            }
        }
        else if (scrollType == ScrollType.Horizontal)
        {
            //向左滑
            while (content.anchoredPosition.x <= layout.padding.left - (headIndex / maxItemNumHigh + 1) * layoutCellSizeXAndSpacingX
        && tailIndex != totalCount - 1)
            {
                //将数据列表中的第一个元素移动到最后一个
                RectTransform item = dataList[0];
                dataList.Remove(item);
                dataList.Add(item);

                //设置位置
                SetPos(item, tailIndex + 1);

                T itemScript = itemScriptsList[0];
                itemScriptsList.Remove(itemScript);
                itemScriptsList.Add(itemScript);
                SetShow(itemScript, tailIndex + 1);

                headIndex++;
                tailIndex++;
            }

            //向右滑
            while (content.anchoredPosition.x >= -layout.padding.left - (headIndex / maxItemNumHigh + 1) * layoutCellSizeXAndSpacingX
            && headIndex != 0)
            {
                // break;
                //将数据列表中的最后一个元素移动到第一个
                RectTransform item = dataList.Last();
                dataList.Remove(item);
                dataList.Insert(0, item);

                //设置位置
                SetPos(item, headIndex - 1);

                T itemScript = itemScriptsList.Last();
                itemScriptsList.Remove(itemScript);
                itemScriptsList.Insert(0, itemScript);
                SetShow(itemScript, headIndex - 1);

                headIndex--;
                tailIndex--;
            }
        }
    }

    #endregion

    #region Tool
    /// <summary>
    /// 设置位置
    /// </summary>
    private void SetPos(RectTransform trans, int index)
    {
        if (scrollType == ScrollType.Horizontal)
        {
            int rowNum = (int)Math.Ceiling((double)index / maxItemNumHigh);
            rowNum = index % maxItemNumHigh == 0 ? rowNum + 1 : rowNum;
            //列
            int lineNum = index % maxItemNumHigh + 1;
            trans.anchoredPosition = new Vector2(
                rowNum == 1 ? firstItemAnchoredPos.x : (rowNum - 1) * layoutCellSizeXAndSpacingX + layout.padding.left + layout.cellSize.x / 2,
                 lineNum == 1 ? firstItemAnchoredPos.y : -((lineNum - 1) * layoutCellSizeYAndSpacingY + layout.padding.top + layout.cellSize.y / 2)
                );
        }
        else if (scrollType == ScrollType.Vertical)
        {

            int rowNum = (int)Math.Ceiling((double)index / maxItemNumWide);
            rowNum = index % maxItemNumWide == 0 ? rowNum + 1 : rowNum;
            //列
            int lineNum = index % maxItemNumWide + 1;
            //trans.anchoredPosition = new Vector2(
            //lineNum == 1 ? firstItemAnchoredPos.x : (lineNum - 1) * (layout.cellSize.x + layout.spacing.x) + layout.padding.left + layout.cellSize.x / 2 ,
            // rowNum == 1 ? firstItemAnchoredPos.y : -((rowNum - 1) * (layout.cellSize.y + layout.spacing.y) + layout.padding.top + layout.cellSize.y / 2)
            //);
            trans.anchoredPosition = new Vector2(
                lineNum == 1 ? firstItemAnchoredPos.x : (lineNum - 1) * layoutCellSizeXAndSpacingX + layout.padding.left + layout.cellSize.x / 2,
                 rowNum == 1 ? firstItemAnchoredPos.y : -((rowNum - 1) * layoutCellSizeYAndSpacingY + layout.padding.top + layout.cellSize.y / 2)
                );
        }
    }
    #endregion

    #region 外部调用

    public InfiniteScrollView(GameObject ga, GameObject itemPrefab)
    {
        this.itemPrefab = itemPrefab;
        Init(ga);
    }
    private int layoutCellSizeYAndSpacingY, layoutCellSizeXAndSpacingX;
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(GameObject ga)
    {
        //显示item的界面宽高
        maskTranV2 = ga.GetComponent<RectTransform>().sizeDelta;
        scrollRect = ga.GetComponent<ScrollRect>();
        content = scrollRect.content;
        layout = content.GetComponent<GridLayoutGroup>();
        scrollRect.onValueChanged.AddListener((Vector2 v) => OnScroll(v));
        layoutCellSizeYAndSpacingY = (int)layout.cellSize.y + (int)layout.spacing.y;
        layoutCellSizeXAndSpacingX = (int)layout.cellSize.x + (int)layout.spacing.x;
        maxItemNumWide = (int)((maskTranV2.x - layout.padding.left - layout.padding.top) / layoutCellSizeXAndSpacingX);
        maxItemNumHigh = (int)((maskTranV2.y - layout.padding.right - layout.padding.bottom) / layoutCellSizeYAndSpacingY);

        scrollType = scrollRect.horizontal ? ScrollType.Horizontal : ScrollType.Vertical;
        //设置布局
        SetLayout();
    }
    public void SetObjectList(List<object> objectList)
    {
        this.objectList = objectList;
        //设置总的item数量
        totalCount = objectList.Count;
        fixedCount = totalCount <= fixedCount ? totalCount : fixedCount;
        //设置Content大小
        SetContentSize();
        //设置头下标和尾下标
        headIndex = 0;
        tailIndex = fixedCount - 1;
        //实例化Item
        InitItem();

        //得到第一个Item的锚点位置
        GetFirstItemAnchoredPos();
    }

    /// <summary>
    /// 设置显示
    /// </summary>
    public void SetShow(T itemScript, int index)
    {
        itemScript.UpdateItem(objectList[index]);
    }

    /// <summary>
    /// 销毁所有的元素
    /// </summary>
    public void DestoryAll()
    {
        for (int i = dataList.Count - 1; i >= 0; i--)
        {
            //Destroy(dataList[i].gameObject);
            //DestroyImmediate(dataList[i].gameObject);
        }
        dataList.Clear();
    }

    #endregion
}

/// <summary>
/// 滑动类型
/// </summary>
public enum ScrollType
{
    Horizontal,//竖直滑动
    Vertical,//水平滑动
}
