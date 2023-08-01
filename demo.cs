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
        //添加一个数据
        List<string> strList = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            strList.Add($"{i}");
        }
      
        //实例化一个InfiniteScrollView
        //其中Item类,可以替换为其他item脚本
        InfiniteScrollView<Item> item = new InfiniteScrollView<Item>(scrollView, itemPrefab);

        List<object> datas = new List<object>();
        strList.ForEach(a => datas.Add(a));
        //设置item上的数据,数据是一个List,采用通用数据Object格式
        item.SetObjectList(datas);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

