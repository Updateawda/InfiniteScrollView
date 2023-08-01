
using UnityEngine;
using UnityEngine.UI;
public class Item : GridPartItem
{
    public Text text;
    public override void OnBindItemElements(GameObject ga)
    {
        base.OnBindItemElements(ga);
        text = ga.transform.Find("Text").GetComponent<Text>();
    }
    public override void UpdateItem(object itemData)
    {
        //text= 
        ga.name = (string)itemData;

        text.text = (string)itemData;
    }
}

