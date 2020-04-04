using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentManager : MonoBehaviour
{
    public myDelegate GetIndexDel;
    private int index;
    public Instrument ins;
    void Start()
    {
        GetIndex(ins.ReturnIndex);
        index = GetIndexDel();
    }

    public void GetIndex(myDelegate SendIndex)
    {
        GetIndexDel = SendIndex;
    }
}
