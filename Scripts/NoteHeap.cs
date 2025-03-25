using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class NoteHeap
{
    public int count = 0;
    public GameObject root;
    [Serializable]
    class node
    {
        public int key;
        public GameObject value;
        public node(int nul=0)
        {
            value = null;
            key = -1;
        }
        public void reset()
        {
            value = null;
            key = -1;
        }
        public void set(GameObject note)
        {
            value = note;
            key = note.GetComponent<NoteController>().JudgeTime;
        }
        public node(node n)
        {
            value = n.value;
            key = n.key;
        }
    }
    [SerializeField]
    List<node> heap = new List<node>() { new node() };


    int getKey(int index)
    {
        return heap[index].key;
    }
    int getLeastChild(int index)
    {
        if (index * 2 > count)
        {
            return -1;
        }
        else if (index * 2 == count)
        {
            return index * 2;
        }
        else
        {
            if (getKey(index * 2) <= getKey(index * 2 + 1))
            {
                return index * 2;
            }
            else
            {
                return index * 2 + 1;
            }
        }
    }

    public void insert(GameObject newNote)
    {
        count++;
        if (count >= heap.Count)
        {
            heap.Add(new node());
        }
        node Node = heap[count];
        Node.set(newNote);
        int index = count;
        while (index != 1 && Node.key < getKey(index/2))
        {
            heap[index] = heap[index/2];
            index = index / 2;
        }
        heap[index] = Node;
        root = heap[1].value;
    }
    public GameObject Peek()
    {
        return heap[1].value;
    }
    public void remove()
    {
        if (count == 0)
        {
            return;
        }
        heap[1] = new node(heap[count]);
        heap[count].reset();
        count--;
        int index = 1;
        while (true)
        {
            int child = getLeastChild(index);
            if (child == -1)
            {
                break;
            }
            if (getKey(child) < getKey(index))
            {
                (heap[index], heap[child]) = (heap[child], heap[index]);
                index = child;
            }
            else
            {
                break;
            }
        }
        root=heap[1].value;
    }
}
