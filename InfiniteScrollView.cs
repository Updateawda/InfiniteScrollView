using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// ���޻����б�
/// </summary>
public class InfiniteScrollView : MonoBehaviour
{
    private ScrollRect scrollRect;//���������
    private RectTransform content;//�������Content
    private GridLayoutGroup layout;//�������

    [Header("��������")]
    public ScrollType scrollType;
    [Header("�̶���Item����")]
    public int fixedCount;
    [Header("Item��Ԥ����")]
    public GameObject itemPrefab;

    private int totalCount;//�ܵ���������
    private List<RectTransform> dataList = new List<RectTransform>();//����ʵ���б�
    private int headIndex;//ͷ�±�
    private int tailIndex;//β�±�
    private Vector2 firstItemAnchoredPos;//��һ��Item��ê������

    #region Init
    private void Start()
    {
        SetTotalCount(100);
        Init();
    }
    /// <summary>
    /// ��ʼ��
    /// </summary>
    public void Init()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
        layout = content.GetComponent<GridLayoutGroup>();
        scrollRect.onValueChanged.AddListener((Vector2 v) => OnScroll(v));

        //���ò���
        SetLayout();

        //����ͷ�±��β�±�
        headIndex = 0;
        tailIndex = fixedCount - 1;

        //����Content��С
        SetContentSize();

        //ʵ����Item
        InitItem();

        //�õ���һ��Item��ê��λ��
        GetFirstItemAnchoredPos();
    }
    /// <summary>
    /// ʵ����Item
    /// </summary>
    private void InitItem()
    {
        for (int i = 0; i < fixedCount; i++)
        {
            GameObject tempItem = Instantiate(itemPrefab, content);
            dataList.Add(tempItem.GetComponent<RectTransform>());
            SetShow(tempItem.GetComponent<RectTransform>(), i);
        }
    }

    /// <summary>
    /// ����Content��С
    /// </summary>
    private void SetContentSize()
    {
        content.sizeDelta = new Vector2
            (
                layout.padding.left + layout.padding.right + totalCount * (layout.cellSize.x + layout.spacing.x) - layout.spacing.x - content.rect.width,
                layout.padding.top + layout.padding.bottom + totalCount * (layout.cellSize.y + layout.spacing.y) - layout.spacing.y
            );
    }

    /// <summary>
    /// ���ò���
    /// </summary>
    private void SetLayout()
    {
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.constraintCount = 1;
        if (scrollType == ScrollType.Horizontal)
        {
            scrollRect.horizontal = true;
            scrollRect.vertical = false;
            layout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        }
        else if (scrollType == ScrollType.Vertical)
        {
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
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
            while (content.anchoredPosition.y >= layout.padding.top + (headIndex + 1) * (layout.cellSize.y + layout.spacing.y)
            && tailIndex != totalCount - 1)
            {
                //�������б��еĵ�һ��Ԫ���ƶ������һ��
                RectTransform item = dataList[0];
                dataList.Remove(item);
                dataList.Add(item);

                //����λ��
                SetPos(item, tailIndex + 1);
                //������ʾ
                SetShow(item, tailIndex + 1);

                headIndex++;
                tailIndex++;
            }
            //���»�
            while (content.anchoredPosition.y <= layout.padding.top + headIndex * (layout.cellSize.y + layout.spacing.y)
                && headIndex != 0)
            {
                //�������б��е����һ��Ԫ���ƶ�����һ��
                RectTransform item = dataList.Last();
                dataList.Remove(item);
                dataList.Insert(0, item);

                //����λ��
                SetPos(item, headIndex - 1);
                //������ʾ
                SetShow(item, headIndex - 1);

                headIndex--;
                tailIndex--;
            }
        }
        else if (scrollType == ScrollType.Horizontal)
        {
            //����
            while (content.anchoredPosition.x <= -layout.padding.left - (headIndex + 1) * (layout.cellSize.x + layout.spacing.x)
            && tailIndex != totalCount - 1)
            {
                //�������б��еĵ�һ��Ԫ���ƶ������һ��
                RectTransform item = dataList[0];
                dataList.Remove(item);
                dataList.Add(item);

                //����λ��
                SetPos(item, tailIndex + 1);
                //������ʾ
                SetShow(item, tailIndex + 1);

                headIndex++;
                tailIndex++;
            }
            //���һ�
            while (content.anchoredPosition.x >= -layout.padding.left - headIndex * (layout.cellSize.x + layout.spacing.x)
            && headIndex != 0)
            {
                //�������б��е����һ��Ԫ���ƶ�����һ��
                RectTransform item = dataList.Last();
                dataList.Remove(item);
                dataList.Insert(0, item);

                //����λ��
                SetPos(item, headIndex - 1);
                //������ʾ
                SetShow(item, headIndex - 1);

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
            trans.anchoredPosition = new Vector2
            (
                index == 0 ? layout.padding.left + firstItemAnchoredPos.x :
                layout.padding.left + firstItemAnchoredPos.x + index * (layout.cellSize.x + layout.spacing.x),
                firstItemAnchoredPos.y
            );
        }
        else if (scrollType == ScrollType.Vertical)
        {
            trans.anchoredPosition = new Vector2
            (
                firstItemAnchoredPos.x,
                index == 0 ? -layout.padding.top + firstItemAnchoredPos.y :
                -layout.padding.top + firstItemAnchoredPos.y - index * (layout.cellSize.y + layout.spacing.y)
            );
        }
    }

    #endregion

    #region �ⲿ����

    

    /// <summary>
    /// ������ʾ
    /// </summary>
    public void SetShow(RectTransform trans, int index)
    {
        //=====����������б�д
        trans.GetComponentInChildren<Text>().text = index.ToString();
        trans.name = index.ToString();
    }

    /// <summary>
    /// �����ܵ���������
    /// </summary>
    public void SetTotalCount(int count)
    {
        totalCount = count;
    }

    /// <summary>
    /// �������е�Ԫ��
    /// </summary>
    public void DestoryAll()
    {
        for (int i = dataList.Count - 1; i >= 0; i--)
        {
            DestroyImmediate(dataList[i].gameObject);
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