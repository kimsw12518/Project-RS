using System;

[Serializable]
public struct NoteData
{
    public int targetTime;
    public int judgeTime;
    public int selfSpeed;
    public int generateTime;
    public int lane;
    public int longtype;

    public NoteData(int lt = 0)
    {
        targetTime = 0;
        judgeTime = 0;
        selfSpeed = 0;
        generateTime = 0;
        lane = 0;
        longtype = lt;
    }
}

