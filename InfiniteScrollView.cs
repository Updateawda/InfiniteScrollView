using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.GridLayoutGroup;

/// <summary>
/// ���޻����б�
/// </summary>
public class InfiniteScrollView<T> where T : GridPartItem, new()
{
    /// <summary>
    /// ���������
    /// </summary>
    private ScrollRect scrollRect;
    /// <summary>
    /// �������Content
    /// </summary>
    private RectTransform content;
    /// <summary>
    /// �������
    /// </summary>
    private GridLayoutGroup layout;//�������

    /// <summary>
    /// ��������
    /// </summary>
    [Header("��������")]
    public ScrollType scrollType;
    /// <summary>
    /// ���ɳ�ʼ������item����
    /// </summary>
    private int fixedCount;
    [Header("Item��Ԥ����")]
    public GameObject itemPrefab;
    /// <summary>
    /// �ܵ�item��������
    /// </summary>
    private int totalCount;
    /// <summary>
    /// ����ʵ���� RectTransform
    /// </summary>
    private List<RectTransform> dataList = new List<RectTransform>();
    private List<T> itemScriptsList = new List<T>();
    private List<object> objectList = new List<object>();
    /// <summary>
    /// ͷ�±�
    /// </summary>
    private int headIndex;
    /// <summary>
    /// β�±�
    /// </summary>
    private int tailIndex;
    /// <summary>
    /// ��һ��Item��ê������
    /// </summary>
    private Vector2 firstItemAnchoredPos;
    /// <summary>
    /// ��ȡmask��ʼ���
    /// </summary>
    private Vector2 maskTranV2;
    /// <summary>
    /// ��mask��ʼ�����,һ���ܷ�item�������,һ�з�item�������
    /// </summary>
    private int maxItemNumWide, maxItemNumHigh;
    #region Init

    /// <summary>
    /// ʵ����Item
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
    /// ����Content��С
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
    /// ���ò���
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
    /// �õ���һ�����ݵ�ê��λ��
    /// </summary>
    private void GetFirstItemAnchoredPos()
    {
        firstItemAnchoredPos = new Vector2
            (
                layout.padding.left + layout.cellSize.x / 2,
                -layout.padding.top - layout.cellSize.y / 2
            );
        Debug.Log("��һ������ê��:" + firstItemAnchoredPos);
    }

    #endregion

    #region Main

    /// <summary>
    /// ������
    /// </summary>
    private void OnScroll(Vector2 v)
    {
        if (dataList.Count == 0)
        {
            Debug.LogWarning("�ȵ���SetTotalCount�������������������ٵ���Init�������г�ʼ��");
            return;
        }
        if (scrollType == ScrollType.Vertical)
        {
            //���ϻ�
            while (content.anchoredPosition.y >= (layout.padding.top + (headIndex / maxItemNumWide + 1) * layoutCellSizeYAndSpacingY)
            && tailIndex != totalCount - 1)
            {
                //�������б��еĵ�һ��Ԫ���ƶ������һ��
                RectTransform item = dataList[0];
                dataList.Remove(item);
                dataList.Add(item);
                //����λ��
                SetPos(item, tailIndex + 1);

                T itemScript = itemScriptsList[0];
                itemScriptsList.Remove(itemScript);
                itemScriptsList.Add(itemScript);

                SetShow(itemScript, tailIndex + 1);
                headIndex++;
                tailIndex++;
            }
            //���»�
            while (content.anchoredPosition.y <= (layout.padding.top + (headIndex / maxItemNumWide + 1) * layoutCellSizeYAndSpacingY)
                && headIndex != 0)
            {

                //�������б��е����һ��Ԫ���ƶ�����һ��
                RectTransform item = dataList.Last();
                dataList.Remove(item);
                dataList.Insert(0, item);

                //����λ��
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
            //����
            while (content.anchoredPosition.x <= layout.padding.left - (headIndex / maxItemNumHigh + 1) * layoutCellSizeXAndSpacingX
        && tailIndex != totalCount - 1)
            {
                //�������б��еĵ�һ��Ԫ���ƶ������һ��
                RectTransform item = dataList[0];
                dataList.Remove(item);
                dataList.Add(item);

                //����λ��
                SetPos(item, tailIndex + 1);

                T itemScript = itemScriptsList[0];
                itemScriptsList.Remove(itemScript);
                itemScriptsList.Add(itemScript);
                SetShow(itemScript, tailIndex + 1);

                headIndex++;
                tailIndex++;
            }

            //���һ�
            while (content.anchoredPosition.x >= -layout.padding.left - (headIndex / maxItemNumHigh + 1) * layoutCellSizeXAndSpacingX
            && headIndex != 0)
            {
                // break;
                //�������б��е����һ��Ԫ���ƶ�����һ��
                RectTransform item = dataList.Last();
                dataList.Remove(item);
                dataList.Insert(0, item);

                //����λ��
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
    /// ����λ��
    /// </summary>
    private void SetPos(RectTransform trans, int index)
    {
        if (scrollType == ScrollType.Horizontal)
        {
            int rowNum = (int)Math.Ceiling((double)index / maxItemNumHigh);
            rowNum = index % maxItemNumHigh == 0 ? rowNum + 1 : rowNum;
            //��
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
            //��
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

    #region �ⲿ����

    public InfiniteScrollView(GameObject ga, GameObject itemPrefab)
    {
        this.itemPrefab = itemPrefab;
        Init(ga);
    }
    private int layoutCellSizeYAndSpacingY, layoutCellSizeXAndSpacingX;
    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init(GameObject ga)
    {
        //��ʾitem�Ľ�����
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
        //���ò���
        SetLayout();
    }
    public void SetObjectList(List<object> objectList)
    {
        this.objectList = objectList;
        //�����ܵ�item����
        totalCount = objectList.Count;
        fixedCount = totalCount <= fixedCount ? totalCount : fixedCount;
        //����Content��С
        SetContentSize();
        //����ͷ�±��β�±�
        headIndex = 0;
        tailIndex = fixedCount - 1;
        //ʵ����Item
        InitItem();

        //�õ���һ��Item��ê��λ��
        GetFirstItemAnchoredPos();
    }

    /// <summary>
    /// ������ʾ
    /// </summary>
    public void SetShow(T itemScript, int index)
    {
        itemScript.UpdateItem(objectList[index]);
    }

    /// <summary>
    /// �������е�Ԫ��
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
/// ��������
/// </summary>
public enum ScrollType
{
    Horizontal,//��ֱ����
    Vertical,//ˮƽ����
}
