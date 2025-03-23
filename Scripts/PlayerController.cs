using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    KeyCode Key1;
    [SerializeField]
    KeyCode Key2;
    [SerializeField]
    KeyCode Key3;
    [SerializeField]
    KeyCode Key4;

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
            NoteManager.GetNote(1).Judge();
        }
        if (Input.GetKeyDown(Key2)) 
        {
            NoteManager.GetNote(2).Judge();
        }
        if (Input.GetKeyDown(Key3))
        {
            NoteManager.GetNote(3).Judge();
        }
        if (Input.GetKeyDown(Key4))
        {
            NoteManager.GetNote(4).Judge();
        }
    }
}
