using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

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
    int m_type;
    [SerializeField]
    Sprite[] m_sprites;
    [SerializeField]
    bool m_active;
    float m_speed;
    Vector3 m_pos;
    float m_curTime;
    int m_playerSpeed;
    GameManager Play;

    public int JudgeTime { get {  return m_judgeTime; } }

    public void init(int lane, int time, int judge, int type, int data = 0)
    {
        SpriteRenderer SR = GetComponent<SpriteRenderer>();
        LineRenderer LR = GetComponent<LineRenderer>();
        m_lane = lane;
        m_targetTime = time;
        m_selfspeed = data;
        m_judgeTime = judge;
        m_type = type;
        m_active = true;
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
            if (type == 2)
            {
                SR.enabled = false;
                LR.enabled = false;
            }
            else if (type == 1)
            {
                SR.enabled = true;
                SR.sprite = m_sprites[0];
                LR.enabled = true;
                if (m_selfnote)
                {
                    SR.color = GameManager.Instance.NoteColorB;
                    LR.startColor = GameManager.Instance.NoteColorB;
                    LR.endColor = GameManager.Instance.NoteColorB;
                }
                else
                {
                    SR.color = GameManager.Instance.NoteColorA;
                    LR.startColor = GameManager.Instance.NoteColorA;
                    LR.endColor = GameManager.Instance.NoteColorA;
                }
                LR.SetPositions(new Vector3[2] { new Vector3(0, 0.15f, 0.2f), new Vector3(0, 0, 0.2f) });
            }
            else
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

    public void Judge(bool miss=false)
    {
        if (m_type == 3)
        {
            return;
        }
        float Difference = Play.CurTime + Play.RealTimeDiff - m_judgeTime;
        if ( Difference < -160)
        {
            return;
        }
        m_active = false;
        NoteManager.Instance.m_notes[m_lane - 1].remove();
        if (m_type == 2)
        {
            if (Difference < 0)
            {
                NoteManager.Instance.LaneJudge[m_lane - 1] = 20;
            }
            StartCoroutine(LongTickJudge());
            return;
        }
        else
        {
            float absDiff = Mathf.Abs(Difference);
            int score = 0;

            if (!miss && absDiff <= 80)
            {
                if (absDiff <= 40)
                    score = 100;
                else if (absDiff <= 60)
                    score = 50;
                else
                    score = 20;

                Play.Score += score;
            }
            else
            {
                score = 0;
            }

            if (m_type == 1)
            {
                NoteManager.Instance.LaneJudge[m_lane - 1] = score;
                NoteManager.Instance.LongSustain[m_lane - 1] = gameObject;
            }
            else
            {
                NoteManager.Instance.returnToPool(gameObject);
            }
        }
    }

    IEnumerator LongTickJudge()
    {
        while (NoteManager.Instance.LaneJudge[m_lane - 1] == -1)
        {
            if (Play.CurTime + Play.RealTimeDiff < m_judgeTime - 80)
            {
                yield return null;
            }
            else
            {
                NoteManager.Instance.LaneJudge[m_lane - 1] = 0;
                NoteManager.Instance.returnToPool(gameObject);
                yield break;
            }
        }

        if (!PlayerController.Instance.KeyPressed[m_lane - 1])
        {
            NoteManager.Instance.LaneJudge[m_lane - 1] = 0;
        }

        Play.Score += NoteManager.Instance.LaneJudge[m_lane - 1] / 10;
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
        if (!m_active)
        {
            if (m_type == 1)
            {
                m_pos = transform.localPosition;
                m_pos.y = 0;
                transform.localPosition = m_pos;
                LineRenderer LR = GetComponent<LineRenderer>();
                Vector3 m_pairPos = NoteManager.Instance.LongPairs[transform].position;
                LR.SetPosition(1, new Vector3(0, m_pairPos.y + 4 - m_pos.y, 0.2f));
            }
            return;
        }

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

        if (m_type == 1)
        {
            if (m_targetTime <= m_curTime)
            {
                m_pos.y = 0;
                transform.localPosition = m_pos;
            }
            LineRenderer LR = GetComponent<LineRenderer>();
            Vector3 m_pairPos = NoteManager.Instance.LongPairs[transform].position;
            LR.SetPosition(1, new Vector3(0, m_pairPos.y+4-m_pos.y, 0.2f));
        }

        if (m_targetTime <= m_curTime)
        {
            if (m_lane == 0)
            {
                Play.Bpm = m_selfspeed;
                NoteManager.Instance.returnToPool(gameObject);
                return;
            }
            else if (m_lane == -1)
            {
                Play.Ratio = m_selfspeed;
                NoteManager.Instance.returnToPool(gameObject);
                return;
            }
            if (m_type == 2)
            {
                Judge();
                m_active = false;
                return;
            }
            if (m_type == 3)
            {
                m_active = false;
                Transform startTransform = NoteManager.Instance.LongSustain[m_lane - 1].transform;
                NoteManager.Instance.LongPairs.Remove(startTransform);
                NoteManager.Instance.returnToPool(startTransform.gameObject);
                NoteManager.Instance.LongSustain[m_lane - 1] = null;
                NoteManager.Instance.m_notes[m_lane - 1].remove();
                NoteManager.Instance.returnToPool(gameObject);
                return;
            }
            else
            {

                if (m_lane > 0 && m_judgeTime <= Play.CurTime + Play.RealTimeDiff - 80)
                {
                    m_active = false;
                    if (m_lane < 5 && m_lane > 0)
                    {
                        //print(Play.CurTime + Play.RealTimeDiff - m_judgeTime);
                        Judge(true);
                    }
                    else
                    {
                        NoteManager.Instance.returnToPool(gameObject);
                    }
                }
            }
        }
    }
}
