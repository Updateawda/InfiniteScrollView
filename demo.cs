using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class demo : MonoBehaviour
{
    public GameObject scrollView;
    public GameObject itemPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //���һ������
        List<string> strList = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            strList.Add($"{i}");
        }
      
        //ʵ����һ��InfiniteScrollView
        //����Item��,�����滻Ϊ����item�ű�
        InfiniteScrollView<Item> item = new InfiniteScrollView<Item>(scrollView, itemPrefab);

        List<object> datas = new List<object>();
        strList.ForEach(a => datas.Add(a));
        //����item�ϵ�����,������һ��List,����ͨ������Object��ʽ
        item.SetObjectList(datas);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

