using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperationManager : MonoBehaviour {

    public Text OperationModeText;
    public Operation OperationMode;
    public void SetOperations(List<Operation> opers)
    {
        int count = opers.Count > 9 ? 9 : opers.Count;
        for(int i = 0; i < 9; i++)
        {
            Button btn = transform.GetChild(i).GetComponent<Button>();
            Text text = btn.GetComponentInChildren<Text>();
            if (i < count)
            {
                text.text = opers[i].Name;
                Operation op = opers[i];
                btn.onClick.AddListener(() => { SetOperationMode(op); });
                btn.enabled = true;
            }
            else
            {
                text.text = "";
                btn.onClick.RemoveAllListeners();
                btn.enabled = false;
            }
            
        }
        if (count > 0) SetOperationMode(opers[0]);
        else SetOperationMode(null);
    }
	public void SetOperationMode(Operation mode)
    {
        OperationMode = mode;
        if (OperationMode == null) OperationModeText.text = "当前模式：None";
        else OperationModeText.text = "当前模式：" + OperationMode.Name;

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
