using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitwiseUtil
{
    /// <summary>
    /// is kth bit 1 in n
    /// </summary>
    /// <returns>The contains.</returns>
    /// <param name="n">N.</param>
    /// <param name="k">K.</param>
    public static bool IsSet(int n, int k)
    {
        int bit = n >> k & 1;
        return bit == 1;
    }

    public static int SetKthBit(int n, int k)
    {
        n = n | 1 << k;
        return n;
    }

    public static int UnsetKthBit(int n, int k)
    {
        n = n & ~(1 << k);
        return n;
    }

    /// <summary>
    /// list of active bits (index)
    /// </summary>
    /// <returns>The list.</returns>
    /// <param name="n">N.</param>
    public static int[] GetList(int n)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < 32; i++)
        {
            if (IsSet(n, i))
                result.Add(i);
        }
        return result.ToArray();
    }
}
