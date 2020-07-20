using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialogBox {
    public class DialogBoxManager : MonoBehaviour
    {
        public static DialogBoxManager dialogBoxManager;

        public GameObject MessageDialogBoxTemplate;
        public GameObject ConfirmDialogBoxTemplate;
        public GameObject DataDialogBoxTemplate;
        public GameObject DataListDialogBoxTemplate;
        public GameObject ListSelectDialogBoxTemplate;
        public GameObject ColorEditorDialogBoxTemplate;
        public GameObject EventDataEditorDialogBoxTemplate;
        public void ShowMessage(string title,string content, UnityAction confirm=null) {
            var msgbox=Instantiate(MessageDialogBoxTemplate, transform).GetComponent<MessageDialogBox>();
            msgbox.Title.text = title;
            msgbox.Content.text = content;
            msgbox.ConfirmEvent = confirm;
            msgbox.gameObject.SetActive(true);
        }
        public void ShowConfirm(string title, string content,UnityAction confirm,UnityAction cancel=null)
        {
            var cfmbox = Instantiate(ConfirmDialogBoxTemplate, transform).GetComponent<ConfirmDialogBox>();
            cfmbox.Title.text = title;
            cfmbox.Content.text = content;
            cfmbox.ConfirmEvent = confirm;
            cfmbox.CancelEvent = cancel;
            cfmbox.gameObject.SetActive(true);
        }
        public void EditData(string title,string name, DialogBoxDataBase defaultval, UnityAction<DialogBoxDataBase> confirm, UnityAction cancel = null)
        {
            var databox = Instantiate(DataDialogBoxTemplate, transform).GetComponent<DataDialogBox>();
            databox.Title.text = title;
            databox.Name.text = name;
            databox.SetValue(defaultval);
            databox.ConfirmEvent = confirm;
            databox.CancelEvent = cancel;
            databox.dataBase = defaultval;
            databox.gameObject.SetActive(true);

        }
        public void EditDataList(string title,string name, List<DialogBoxDataBase> defaultdatas, List<DialogBoxDataBase> totaldatas, UnityAction<List<DialogBoxDataBase>> confirm, UnityAction cancel = null)
        {
            var datalistbox = Instantiate(DataListDialogBoxTemplate, transform).GetComponent<DataListDialogBox>();
            datalistbox.Title.text = title;
            datalistbox.Name.text = name;
            datalistbox.ConfirmEvent = confirm;
            datalistbox.CancelEvent = cancel;
            datalistbox.TotalDatas = totaldatas;
            datalistbox.SetOptions(defaultdatas);
            datalistbox.gameObject.SetActive(true);
        }
        public void ListSelect(string title,string name,int defaultval, List<DialogBoxDataBase> opts, UnityAction<int> confirm, UnityAction cancel = null)
        {
            var listselectbox = Instantiate(ListSelectDialogBoxTemplate, transform).GetComponent<ListSelectDialogBox>();
            listselectbox.Title.text = title;
            listselectbox.Name.text = name;
            listselectbox.SetOption(opts,defaultval);
            listselectbox.ConfirmEvent = confirm;
            listselectbox.CancelEvent = cancel;
            listselectbox.gameObject.SetActive(true);
        }

        public void EditColor(string title, string name, Color32 defaultval, UnityAction<Color32> confirm, UnityAction cancel = null)
        {
            var databox = Instantiate(ColorEditorDialogBoxTemplate, transform).GetComponent<ColorEditorDialogBox>();
            databox.Title.text = title;
            databox.Name.text = name;
            databox.SetValue(defaultval);
            databox.ConfirmEvent = confirm;
            databox.CancelEvent = cancel;
            databox.gameObject.SetActive(true);
        }

        public void EditConditionData(ResourceLoad resourceLoad, CLCondition condition, UnityAction<object> confirm, UnityAction cancel = null)
        {
            var databox = Instantiate(EventDataEditorDialogBoxTemplate, transform).GetComponent<EventDataDialogBox>();
            databox.resourceLoad = resourceLoad;
            databox.SetEventData(condition);
            databox.ConfirmEvent = confirm;
            databox.CancelEvent = cancel;
            databox.gameObject.SetActive(true);
        }
        public void EditCLValueData(ResourceLoad resourceLoad, CLValue clvalue, UnityAction<object> confirm, UnityAction cancel = null)
        {
            var databox = Instantiate(EventDataEditorDialogBoxTemplate, transform).GetComponent<EventDataDialogBox>();
            databox.resourceLoad = resourceLoad;
            databox.SetEventData(clvalue);
            databox.ConfirmEvent = confirm;
            databox.CancelEvent = cancel;
            databox.gameObject.SetActive(true);
        }

        private void Awake()
        {
            if (dialogBoxManager == null)
                dialogBoxManager = this;
            else
                Destroy(this.gameObject);
        }

    }

   
}
