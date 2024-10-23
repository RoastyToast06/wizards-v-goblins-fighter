namespace COIS2020.AidenGomes0801606.Assignments;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using COIS2020.StarterCode.Assignments;

public class ArrayList<T> : ISriList<T> where T : IComparable<T>, IEquatable<T>
{
    /// <summary>
    /// How many items an ArrayList should have space for when it is first constructed.
    /// </summary>
    private const int DEFAULT_CAPACITY = 4;

    /// <summary>
    /// Internal buffer of items.
    /// </summary>
    protected T[] buffer;

    // The number of items currently in the list, AKA the number of `buffer` slots that currently have an item in them.
    public int Count { get; protected set; }

    /// <summary>
    /// The total number of items this list's internal buffer has space for. When <c>Capacity</c> equals
    /// <see cref="Count"/>, an insertion will trigger a resize/reallocation.
    /// </summary>
    public int Capacity { get => buffer.Length; }


    /// <summary>
    /// Creates a new ArrayList.
    /// </summary>
    public ArrayList()
    {
        buffer = new T[DEFAULT_CAPACITY];
        Count = 0;
    }

    /// <summary>
    /// Creates a new ArrayList with a specified pre-allocated capacity.
    /// </summary>
    ///
    /// <param name="capacity">How many items to reserve space for.</param>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// If <paramref name="capacity"/> is less than or equal to zero.
    /// </exception>
    public ArrayList(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity));

        buffer = new T[capacity];
        Count = 0;
    }

    protected void Grow()
    {
        // Allocate a new buffer with double the capacity of the current buffer.
        T[] newBuffer = new T[buffer.Length * 2];

        // Copy the items from the old buffer to the new buffer.
        for (int i = 0; i < Count; i++)
            newBuffer[i] = buffer[i];

        // Replace the old buffer with the new buffer.
        buffer = newBuffer;
    }

    public T Get(int index)
    {
        //Get the item at the specified index.
        return buffer[index];
    }

    public int FindIndex(T item)
    {
        //Find the index of the specified item.
        for (int i = 0; i < Count; i++)
        {
            if (buffer[i].Equals(item))
                return i;
        }
        return -1;
    }
    public void InsertAt(int index, T item)
    {
        // If the list is full, grow the buffer.
        if (Count == Capacity)
            Grow();
        // Move all items from the index to the end of the list one position to the right.
        for (int i = Count; i > index; i--)
            buffer[i] = buffer[i - 1];
        buffer[index] = item;
        // Increment the count of items in the list.
        Count++;
    }

    public void AddBack(T item)
    {
        //Add an item to the end of the list using the InsertAt method
        InsertAt(Count, item);
    }

    public void AddFront(T item)
    {
        //Add an item to the front of the list.
        InsertAt(0, item);
    }

    public T RemoveAt(int index)
    {
        //Remove the item at the specified index.
        T item = buffer[index];
        // Move all items from the index to the end of the list one position to the left.
        for (int i = index; i < Count - 1; i++)
            buffer[i] = buffer[i + 1];
        // Decrement the count of items in the list.
        Count--;
        return item;
    }

    public T RemoveFirst()
    {
        //Remove the first item in the list.
        return RemoveAt(0);
    }

    public T RemoveLast()
    {
        //Remove the last item in the list.
        return RemoveAt(Count - 1);
    }

    public void Clear()
    {
        //Delete all items in the list.
        Count = 0;
    }

    public void Swap(int index1, int index2)
    {
        //Swap the items at the specified indices.
        T temp = buffer[index1];
        buffer[index1] = buffer[index2];
        buffer[index2] = temp;
    }

    public void RotateLeft()
    {
        //Rotate all items in the list one position to the left. The first item becomes the last.
        T temp = buffer[0];
        for (int i = 0; i < Count - 1; i++)
            buffer[i] = buffer[i + 1];
        buffer[Count - 1] = temp;
    }

    public void RotateRight()
    {
        //Rotate all items in the list one position to the right. The last item becomes the first.
        T temp = buffer[Count - 1];
        for (int i = Count - 1; i > 0; i--)
            buffer[i] = buffer[i - 1];
        buffer[0] = temp;
    }

    public void Sort()
    {
        //Sorting items using a merge sort algorithm
        MergeSort(buffer);
    }

    //Extra methods to implement the merge sort algorithm
    public static void MergeSort(T[] values)
    {
        //Prevents the array from being sorted if it has only one item or less
        if (values.Length <= 1)
            return;
        //Divide the array into two halves
        int mid = values.Length / 2;
        T[] left = new T[mid];
        T[] right = new T[values.Length - mid];
        //Copy values into each half
        for (int i = 0; i < mid; i++)
            left[i] = values[i];
        for (int i = mid; i < values.Length; i++)
            right[i - mid] = values[i];
        //Recursively sort each half
        MergeSort(left);
        MergeSort(right);
        //Merge the halves back together
        MergeArray(values, left, right);
    }

    public static void MergeArray(T[] array, T[] left, T[] right)
    {
        //Merge the two halves together, sorting each element each time
        int i = 0, j = 0, k = 0;
        while (i < left.Length && j < right.Length)
        {
            if (left[i].CompareTo(right[j]) <= 0)
            {
                array[k] = left[i];
                i++;
            }
            else
            {
                array[k] = right[j];
                j++;
            }
            k++;
        }
        //Copy the remaining elements of both halves
        while (i < left.Length)
        {
            array[k] = left[i];
            i++;
            k++;
        }
        while (j < right.Length)
        {
            array[k] = right[j];
            j++;
            k++;
        }
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return buffer[i];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
