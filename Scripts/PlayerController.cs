using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private static PlayerController instance;
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
    public static PlayerController Instance
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
    KeyCode Key1;
    [SerializeField]
    KeyCode Key2;
    [SerializeField]
    KeyCode Key3;
    [SerializeField]
    KeyCode Key4;
    public List<bool> KeyPressed = new List<bool>(4) { false, false, false, false };
    NoteController m_targetnote;

    NoteManager NoteManager;

    // Start is called before the first frame update
    void Start()
    {
        NoteManager = NoteManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Key1))
        {
            m_targetnote = NoteManager.GetNote(1);
            KeyPressed[0] = true;
            if (m_targetnote != null)
            {
                m_targetnote.Judge();
            }
        }
        else if (Input.GetKeyUp(Key1))
        {
            KeyPressed[0] = false;
        }
        if (Input.GetKeyDown(Key2))
        {
            m_targetnote = NoteManager.GetNote(2);
            KeyPressed[1] = true;
            if (m_targetnote != null)
            {
                m_targetnote.Judge();
            }
        }
        else if (Input.GetKeyUp(Key2))
        {
            KeyPressed[1] = false;
        }
        if (Input.GetKeyDown(Key3))
        {
            m_targetnote = NoteManager.GetNote(3);
            KeyPressed[2] = true;
            if (m_targetnote != null)
            {
                m_targetnote.Judge();
            }
        }
        else if (Input.GetKeyUp(Key3))
        {
            KeyPressed[2] = false;
        }
        if (Input.GetKeyDown(Key4))
        {
            m_targetnote = NoteManager.GetNote(4);
            KeyPressed[3] = true;
            if (m_targetnote != null)
            {
                m_targetnote.Judge();
            }
        }
        else if (Input.GetKeyUp(Key4))
        {
            KeyPressed[3] = false;
        }
    }
}
