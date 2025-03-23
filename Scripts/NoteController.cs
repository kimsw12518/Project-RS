using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

public class NoteController : MonoBehaviour
{
    [SerializeField]
    int m_targetTime;
    [SerializeField]
    int m_judgeTime;
    [SerializeField]
    int m_lane;
    [SerializeField]
    int m_selfspeed;
    [SerializeField]
    bool m_selfnote;
    [SerializeField]
    Sprite[] m_sprites;
    float m_speed;
    Vector3 m_pos;
    float m_curTime;
    int m_playerSpeed;
    GameManager Play;

    public int JudgeTime { get {  return m_judgeTime; } }

    public void init(int lane, int time, int judge, int data = 0)
    {
        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        m_lane = lane;
        m_targetTime = time;
        m_selfspeed = data;
        m_judgeTime = judge;

        if (m_selfspeed != 0 && lane > 0)
        {
            m_selfnote = true;
        }
        else
        {
            m_selfnote = false;
        }
        if (lane > 0 && lane < 5)
        {
            SR.enabled = true;
            SR.sprite = m_sprites[0];
            if (m_selfnote)
            {
                SR.color = GameManager.Instance.NoteColorB;
            }
            else
            {
                SR.color = GameManager.Instance.NoteColorA;
            }
        }
        else if (lane == 5)
        {
            SR.enabled = true;
            SR.sprite = m_sprites[1];
            SR.color = Color.white;
        }
        else
        {
            SR.enabled = false;
        }
    }

    public void Judge()
    {
        NoteManager.Instance.RealTimes.Add(Play.CurTime+Play.RealTimeDiff);
        NoteManager.Instance.m_notes[m_lane - 1].remove();
        NoteManager.Instance.returnToPool(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Play = GameManager.Instance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        m_curTime = Play.CurTime;
        m_playerSpeed = Play.Speed;
        m_pos = transform.localPosition;
        if (m_lane == 5)
        {
            m_pos.x = 0;
        }
        else
        {
            m_pos.x = m_lane * 1.3f - 3.25f;
        }
        if (m_selfnote)
        {
            m_curTime += Play.RealTimeDiff;
            m_speed = m_playerSpeed * m_selfspeed / 100f;
            m_pos.y = (m_targetTime - m_curTime) * m_speed * 0.000025f;
        }
        else
        {
            m_speed = m_playerSpeed * Play.Ratio / 100f;
            m_pos.y = (m_targetTime - m_curTime) * m_speed * 0.000025f;
        }
        transform.localPosition= m_pos;

        if (m_targetTime <= m_curTime)
        {
            if (m_lane == 0)
            {
                Play.Bpm = m_selfspeed;
                NoteManager.Instance.returnToPool(gameObject);
            }
            else if (m_lane == -1)
            {
                Play.Ratio = m_selfspeed;
                NoteManager.Instance.returnToPool(gameObject);
            }


            if (m_targetTime <= m_curTime - 0)
            {
                if (m_lane < 5 && m_lane > 0)
                {
                    print(Play.CurTime+Play.RealTimeDiff-m_targetTime);
                    Judge();
                }
                else
                {
                    NoteManager.Instance.returnToPool(gameObject);
                }
            }
        }
    }
}
