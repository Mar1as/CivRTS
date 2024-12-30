using System.Collections.Generic;
using UnityEngine;

public class HexCellPriorityQueue
{
    public List<MainHexCell> list = new List<MainHexCell>();
    public MainHexCell NextWithSamePriority { get; set; }
    int count = 0;
    int minimum = int.MaxValue;


    public int Count
    {
        get
        {
            return count;
        }
    }

    public void Enqueue(MainHexCell cell)
    {
        count += 1;
        int priority = cell.dataHexCell.hexCellDistance.SearchPriority;
        if (priority < minimum)
        {
            minimum = priority;
        }
        while (priority >= list.Count)
        {
            list.Add(null);
        }
        cell.dataHexCell.hexCellDistance.NextWithSamePriority = list[priority];
        list[priority] = cell;
    }

    public MainHexCell Dequeue()
    {
        count -= 1;
        for (; minimum < list.Count; minimum++)
        {
            MainHexCell cell = list[minimum];
            if (cell != null)
            {
                list[minimum] = cell.dataHexCell.hexCellDistance.NextWithSamePriority;
                return cell;
            }
        }
        return null;
    }

    public void Change(MainHexCell cell, int oldPriority)
    {
        MainHexCell current = list[oldPriority];
        MainHexCell next = current.dataHexCell.hexCellDistance.NextWithSamePriority;
        if (current == cell)
        {
            list[oldPriority] = next;
        }
        else
        {
            while (next != cell)
            {
                current = next;
                next = current.dataHexCell.hexCellDistance.NextWithSamePriority;
            }
            current.dataHexCell.hexCellDistance.NextWithSamePriority = cell.dataHexCell.hexCellDistance.NextWithSamePriority;
        }
        Enqueue(cell);
        count -= 1;
    }

    public void Clear()
    {
        list.Clear();
        count = 0;
        minimum = int.MaxValue;
    }
}
