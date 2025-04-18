using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeybeamController : MonoBehaviour
{
    [SerializeField]
    PlayerController m_pCon;
    [SerializeField]
    List<SpriteRenderer> m_KB = new List<SpriteRenderer>();
    List<short> m_KT = new List<short>(4) { 0,0,0,0};
    // Start is called before the first frame update
    void Start()
    {
        GetComponentsInChildren<SpriteRenderer>(m_KB);
    }

    // Update is called once per frame
    void Update()
    {
        float timeInterval = Time.deltaTime;
        for (int i=0; i<4; i++)
        {
            if (m_pCon.KeyPressed[i])
            {
                m_KB[i].color = new Color(1,1,1,0.3f);
                m_KT[i] = 1000;
            }
            else
            {
                if (m_KT[i] > 0)
                {
                    m_KT[i] -= (short)(timeInterval * 1000 / 0.1);
                }
                if (m_KT[i] < 0)
                {
                    m_KT[i] = 0;
                }
                m_KB[i].color = new Color(1, 1, 1, (float)m_KT[i] / 1000);
            }
        }
    }
}
