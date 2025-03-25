using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using System.ComponentModel;
using Unity.Mathematics;

public class PatternParser : MonoBehaviour
{
    [SerializeField]
    List<string> m_rawPattern = new List<string>();
    [SerializeField]
    List<NoteData> m_pattern = new List<NoteData>();

    [SerializeField]
    GameObject m_notePrefab;
    [SerializeField]
    Transform m_lane;
    [SerializeField]
    int m_introTime;
    [SerializeField]
    string m_title;
    [SerializeField]
    string m_writer;
    [SerializeField]
    float m_baseBpm;
    [SerializeField]
    string m_song;
    
    [SerializeField]
    string m_fileName;
    [Space(10f)]
    [SerializeField]
    [ReadOnly(true)]
    int m_index=0;



    NoteManager m_noteManager;

    void ReadTxt(string fileName)
    {
        FileInfo fileInfo = new FileInfo("Assets/Patterns/" + fileName + ".txt");
        string value = " ";

        if (fileInfo.Exists)
        {
            StreamReader reader = new StreamReader("Assets/Patterns/" + fileName + ".txt");
            while (value != null)
            {
                value = reader.ReadLine();
                m_rawPattern.Add(value);
            }
            m_rawPattern.Remove(null);
            reader.Close();
        }
    }

    List<NoteData> SortByTime(List<NoteData> list,int Type=0)
    {
        if (Type == 0) list.Sort((a, b) => a.generateTime.CompareTo(b.generateTime));
        else list.Sort((a, b) => a.targetTime.CompareTo(b.targetTime));
        return list;
    }

    // Start is called before the first frame update
    void Start()
    {
        List<NoteData> tempPattern = new List<NoteData>();
        m_noteManager = GetComponent<NoteManager>();
        int beat;
        char type;
        int subbeat;
        int selfSpeed=100;
        int data;
        int lane;
        int beatLineMode = 1;
        float simTime=0;
        float simRealtime = 0;
        int offset = 0;
        int curBeat = -2;

        //        GameObject note;
        ReadTxt(m_fileName);
        m_title = m_rawPattern[0];
        m_rawPattern.RemoveAt(0);
        m_writer = m_rawPattern[0];
        m_rawPattern.RemoveAt(0);
        m_song = m_rawPattern[0];
        m_rawPattern.RemoveAt(0);
        m_baseBpm = float.Parse(m_rawPattern[0]);
        m_rawPattern.RemoveAt(0);
        m_introTime = int.Parse(m_rawPattern[0]);
        m_rawPattern.RemoveAt(0);
        GameManager.Instance.BaseBpm = m_baseBpm;
        while (m_rawPattern.Count > 0) //패턴을 읽어들임
        {
            NoteData ND = new NoteData();
            beat = int.Parse(m_rawPattern[0].Substring(0,3));
            type = m_rawPattern[0][3];
            subbeat = int.Parse(m_rawPattern[0].Substring(4, 3));
            data = int.Parse(m_rawPattern[0].Substring(7, 4));
            ND.targetTime = Convert.ToInt32(1000/m_baseBpm*(beat*192+subbeat)+m_introTime)-offset;
            ND.judgeTime = ND.targetTime;
            while (curBeat<beat+subbeat/200) //마디선 배치
            {
                curBeat++;
                NoteData BeatLane = new NoteData();
                BeatLane.lane = 5;
                BeatLane.targetTime = Convert.ToInt32(1000 / m_baseBpm * (curBeat * 192) + m_introTime) - offset;
                if(beatLineMode==1)
                {
                    BeatLane.selfSpeed = 0;
                    BeatLane.generateTime = BeatLane.targetTime - 4000;
                    tempPattern.Add(BeatLane);
                }
                else if(beatLineMode==2)
                {
                    BeatLane.selfSpeed = selfSpeed;
                    BeatLane.generateTime = BeatLane.targetTime - 400000 / selfSpeed;
                    tempPattern.Add(BeatLane);
                }  
            }
            if (type == 'N') //일반노트
            {
                for (lane = 4; data > 0; lane--, data = data / 10)
                {
                    if( data % 10 == 1)
                    {
                        ND.lane = lane;
                        ND.generateTime = ND.targetTime-4000;
                        ND.judgeTime = ND.targetTime;
                        tempPattern.Add( ND );
                    }
                }
            }
            else if (type == 'K') //배속 무시 노트
            {
                for (lane = 4; data > 0; lane--, data = data / 10)
                {
                    if (data % 10 == 1)
                    {
                        ND.lane = lane;
                        ND.selfSpeed = selfSpeed;
                        ND.judgeTime = ND.targetTime;
                        ND.generateTime = ND.targetTime - 400000 / selfSpeed;
                        tempPattern.Add(ND);
                    }
                }
            }
            else if (type == 'B') //BPM 변경
            {
                ND.lane = 0;
                ND.selfSpeed = data;
                ND.generateTime = ND.targetTime - 500;
                tempPattern.Add(ND);
            }
            else if (type == 'S') //스크롤 속도 변경
            {
                ND.lane = -1;
                ND.selfSpeed = data;
                ND.generateTime = ND.targetTime - 500;
                tempPattern.Add(ND);
            }
            else if (type == 'J') //이후 처리되는 SelfNote의 속도
            {
                selfSpeed = data;
            }
            else if (type == 'E') //곡 종료
            {
                ND.lane = -2;
                ND.generateTime = ND.targetTime - 500;
                tempPattern.Add(ND);
            }
            else if (type == 'R') //마디 스킵 및 마디선 설정 변경
            {
                if (data == 1 || data == 4)
                {
                    beatLineMode = 1;
                }
                else if (data == 2 || data == 5)
                {
                    beatLineMode = 2;
                }
                else if (data == 3 || data == 6)
                {
                    beatLineMode = 3;
                }
                if (data < 4)
                {
                    offset += Convert.ToInt32(1000 / m_baseBpm * (192 - subbeat));
                }
            }
            curBeat = beat;
            m_rawPattern.RemoveAt(0);
        }
        tempPattern = SortByTime(tempPattern,1);
        List<Tuple<int, float>> speedChange = new List<Tuple<int, float>>() {new Tuple<int,float>(0,m_baseBpm)};
        for (int i = 0; i < tempPattern.Count; i++) //시뮬레이션을 위한 변속 시점
        {
            if (tempPattern[i].lane == 0)
            {
                speedChange.Add(new Tuple<int, float>(tempPattern[i].targetTime, tempPattern[i].selfSpeed));
            }
        }
        for (int i=0;i<tempPattern.Count;i++)    //모든 노트 시뮬레이션 및 처리 시점을 위한 절대시간 계산
        {
            NoteData ND = tempPattern[i];
            if(ND.lane >= 1)
            {
                simTime = 0;
                simRealtime = 0;
                int bpmIndex =1;
                while (speedChange.Count>bpmIndex&& ND.targetTime > speedChange[bpmIndex].Item1)
                {
                    simRealtime += (speedChange[bpmIndex].Item1 - simTime) * m_baseBpm / speedChange[bpmIndex-1].Item2;
                    simTime = speedChange[bpmIndex].Item1;
                    bpmIndex++;
                }
                simRealtime += (ND.targetTime - simTime) * m_baseBpm / speedChange[bpmIndex-1].Item2;
                if(ND.selfSpeed != 0)
                {
                    ND.targetTime = Convert.ToInt32(simRealtime);
                    ND.judgeTime = Convert.ToInt32(simRealtime);
                    ND.generateTime = Convert.ToInt32(simRealtime-400000/ND.selfSpeed);
                }
                else
                {
                    ND.judgeTime = Convert.ToInt32(simRealtime);
                }
                m_pattern.Add(ND);
            }
            else
            {
                m_pattern.Add(ND);
            }
        }
        m_pattern = SortByTime(m_pattern);
    }

    int cnt=0;
    // Update is called once per frame
    void Update()
    {
        if (m_index < m_pattern.Count)
        {
            if (m_pattern[m_index].generateTime <= GameManager.Instance.CurTime)
            {
                m_noteManager.CreateNote(m_pattern[m_index]);
                m_index++;
            }
        }
    }
}
