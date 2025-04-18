using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    private static NoteManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }
    }
    public static NoteManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    
    [SerializeField]
    GameObject m_notePrefab;
    [SerializeField]
    Transform m_lane;
    ConcurrentQueue<GameObject> m_noteQueue = new ConcurrentQueue<GameObject>();
    static NoteHeap m_lane1 = new NoteHeap();
    static NoteHeap m_lane2 = new NoteHeap();
    static NoteHeap m_lane3 = new NoteHeap();
    static NoteHeap m_lane4 = new NoteHeap();
    [SerializeField]
    List<Transform> m_longDefault = new List<Transform>();
    [SerializeField]
    [ReadOnly]
    public List<NoteHeap> m_notes = new List<NoteHeap>() { m_lane1, m_lane2, m_lane3, m_lane4 };
    public Dictionary<Transform, Transform> LongPairs = new Dictionary<Transform, Transform>();
    List<Transform> m_unpaired = new List<Transform>() { null, null, null, null };
    public List<GameObject> LongSustain = new List<GameObject>() { null, null, null, null }; 
    public List<int> LaneJudge = new List<int>(4) { -1,-1,-1,-1 };


    public List<float> RealTimes = new List<float>();

    public NoteController GetNote(int lane)
    {
        if (m_notes[lane-1].count == 0)
        {
            return null;
        }
        return m_notes[lane-1].Peek().GetComponent<NoteController>();
    }

    public void returnToPool(GameObject note)
    {
        note.transform.position = new Vector3(0, 1000, 0);
        note.GetComponent<SpriteRenderer>().enabled = false;
        note.GetComponent<LineRenderer>().enabled = false;
        m_noteQueue.Enqueue(note);
        note.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public List<GameObject> ihatefuckingbugs = new List<GameObject>();

    private readonly object queueLock = new object();
    public void CreateNote(NoteData ND)
    {
        GameObject note;
        lock (queueLock)
        {
            ihatefuckingbugs=m_noteQueue.ToList<GameObject>();
            if (m_noteQueue.TryDequeue(out note))
            {
                //while (note.activeSelf == true)
                //{
                //    if (!m_noteQueue.TryDequeue(out note))
                //    {
                //        note = Instantiate(m_notePrefab, m_lane);
                //        note.SetActive(false);
                //    }
                //}
                note.SetActive(true);
                note.GetComponent<NoteController>().init(ND.lane, ND.targetTime, ND.judgeTime, ND.longtype, ND.selfSpeed);
                if (ND.longtype == 1) {
                    LongPairs[note.transform] = m_longDefault[ND.lane - 1];
                    m_unpaired[ND.lane - 1] = note.transform;
                }
                else if (ND.longtype == 3)
                {
                    LongPairs[m_unpaired[ND.lane - 1]] = note.transform;
                    m_unpaired[ND.lane - 1] = null;
                }
            }
            else
            {
                note = Instantiate(m_notePrefab, m_lane);
                note.GetComponent<NoteController>().init(ND.lane, ND.targetTime, ND.judgeTime, ND.longtype, ND.selfSpeed);
                if (ND.longtype == 1)
                {
                    LongPairs[note.transform] = m_longDefault[ND.lane - 1];
                    m_unpaired[ND.lane - 1] = note.transform;
                }
                else if (ND.longtype == 3)
                {
                    LongPairs[m_unpaired[ND.lane - 1]] = note.transform;
                    m_unpaired[ND.lane - 1] = null;
                }
            }
            if (ND.lane >= 1 && ND.lane <= 4)
            {
                m_notes[ND.lane - 1].insert(note);
            }
        }
    }
}
