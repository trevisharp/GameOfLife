using System.Collections;
using System.Collections.Generic;

public class PopulationRegister : IEnumerable<int>
{
    private int lasti = 0;
    private LinkedList<int[]> population = new LinkedList<int[]>();

    public int Count => lasti;

    public int Min { get; set; } = int.MaxValue;
    public int Max { get; set; } = int.MinValue;

    public PopulationRegister() { }

    public void Add(int pop)
    {
        if (lasti % 1000 == 0)
            population.AddLast(new int[1000]);
        population.Last.Value[lasti % 1000] = pop;
        lasti++;
        if (pop < Min)
            Min = pop;
        if (pop > Max)
            Max = pop;
    }

    public IEnumerator<int> GetEnumerator()
    {
        int gi = 0;
        var crr = population.First;
        while (crr != population.Last)
        {
            for (int i = 0; i < 1000; i++)
                yield return crr.Value[i];
            gi += 1000;
            crr = crr.Next;
        }
        for (int i = 0; i < 1000; i++, gi++)
        {
            if (gi == lasti)
                yield break;
            yield return crr.Value[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public int this[int i]
    {
        get
        {
            var crr = population.First;
            while (i >= 1000)
            {
                crr = crr.Next;
                i -= 1000;
            }
            return crr.Value[i];
        }
    }
}