using System.Collections;
using System.Collections.Generic;
using DialogBox;
using UnityEngine;

public class test : MonoBehaviour {

	// Use this for initialization
	public void messagetest () {
        DialogBoxManager.dialogBoxManager.ShowMessage("测试消息", "这是一条测试消息");
	}
    public void confirmtest()
    {
        DialogBoxManager.dialogBoxManager.ShowConfirm("测试确认", "这是一条测试确认",()=> {
            Debug.Log("已确认");
        });
    }
    public void datatest()
    {
        DialogBoxManager.dialogBoxManager.EditData("编辑值-浮点数","测试名称",new Data(),data=> {
            Debug.Log(data.GetDialogBoxShowString());
        });
    }
    public void selecttest()
    {
        List<DialogBoxDataBase> datas = new List<DialogBoxDataBase>();
        for (int i = 0; i < 10; i++) {
            datas.Add(new Data { data = 100 + i });
        }
        DialogBoxManager.dialogBoxManager.ListSelect("选择值-整数索引", "数据列表", datas,index=> {
            Debug.Log("返回索引" + index);
        });
    }

    public void datalist()
    {
        List<DialogBoxDataBase> datas = new List<DialogBoxDataBase>();
        for (int i = 0; i < 10; i++)
        {
            datas.Add(new person {name ="susu" +i });
        }
        DialogBoxManager.dialogBoxManager.EditDataList("编辑值-人物列表", "人物", datas, list => {
            foreach (var t in list) {
                Debug.Log(t.GetDialogBoxShowString());
            }
        });
    }

}
class Data : DialogBoxDataBase {
    public float data;
    public override string GetDialogBoxShowString()
    {
        return data.ToString();
    }
    public override void SetValue(string strdata)
    {
        data=float.Parse(strdata);
    }
}

class person : DialogBoxDataBase
{
    public string name;
    public override string GetDialogBoxShowString()
    {
        return name.ToString();
    }
    public override void SetValue(string strdata)
    {
        name = strdata;
    }
}