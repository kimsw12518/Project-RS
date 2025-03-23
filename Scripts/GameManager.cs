using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public static GameManager Instance
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
    float m_time = 0;
    [SerializeField]
    float m_realTimeDiff = 0;
    [SerializeField]
    int m_speed = 100;
    [SerializeField]
    int m_selfSpeed = 100;
    [SerializeField]
    int m_ratio = 100;
    [SerializeField]
    int m_targetRatio= 100;
    [SerializeField]
    float m_bpm;
    [SerializeField]
    float m_baseBpm;
    [SerializeField]
    Transform m_lane;
    [SerializeField]
    Color m_noteA = new Color(0.5f,1,1,1);
    [SerializeField]
    Color m_noteB = new Color(1, 0.5f, 1, 1);

    public Color NoteColorA { get { return m_noteA; } }
    public Color NoteColorB { get { return m_noteB; } }

    public float CurTime{ get { return m_time; } }
    public float RealTimeDiff { get { return m_realTimeDiff; } }
    public int Speed { get { return m_speed; } }
    public int SelfSpeed { get { return m_selfSpeed; } set { m_selfSpeed = value; } }
    public int Ratio { get { return m_ratio; } set { m_targetRatio = value; } }
    public float Bpm { set { m_bpm = value; } }
    public float BaseBpm { set { m_baseBpm = value; m_bpm = value; } }

    [SerializeField]
    NoteManager m_noteManager;
    NoteData ND = new NoteData();

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_time += (Time.fixedDeltaTime * 1000 * m_bpm / m_baseBpm);
        m_realTimeDiff += (Time.fixedDeltaTime * 1000 * (1 - (m_bpm / m_baseBpm)));
    }

    private void Update()
    {
        
    }
}
