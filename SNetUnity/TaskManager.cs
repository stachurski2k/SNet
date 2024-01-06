using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class TaskManager : MonoBehaviour
{
    Queue<Action> tasks=new Queue<Action>();
    object guard=new object();
    public static TaskManager instance;
    private void Awake()
    {
        instance = this;
    }
    void Update()
    {
        lock (guard)
        {
            while (tasks.Count > 0)
            {
                tasks.Peek()();
                tasks.Dequeue();
            }
        }
    }
    public void AddTask(Action a)
    {
        lock (guard)
        {
            tasks.Enqueue(a);
        }
    }
}
