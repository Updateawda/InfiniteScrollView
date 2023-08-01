using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPartItem
{
    public GameObject ga;
    public virtual void UpdateItem(object itemData)
    {
    }
    public virtual void OnBindItemElements(GameObject ga)
    {
        this.ga = ga;
    }
    public virtual void Dispose()
    {
    }
}

