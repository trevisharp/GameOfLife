using System;
using System.Text;

public class Game
{
    private int width = 5;
    public int Width => width;

    private int height = 5;
    public int Height => height;

    private bool[] data;

    public Game(int wid, int hei)
    {
        this.width = wid;
        this.height = hei;
        this.data = new bool[wid * hei];
    }

    public bool this[int i, int j]
    {
        get => this.data[i + j * width];
        set => this.data[i + j * width] = value;
    }

    public void RunGeneration()
    {
        Span<bool> span = stackalloc bool[data.Length];
        int count, wid = width - 1, hei = height - 1;

        if (this[0, 0])
            span[0] = (this[1, 0] && this[0, 1]) ||
                      (this[1, 1] && this[0, 1]) ||
                      (this[1, 0] && this[1, 1]);
        else
            span[0] = this[1, 0] && this[0, 1] && this[1, 1];

        if (this[wid, 0])
            span[wid] = (this[wid - 1, 0] && this[wid, 1]) ||
                        (this[wid - 1, 1] && this[wid, 1]) ||
                        (this[wid - 1, 0] && this[wid - 1, 1]);
        else
            span[wid] = this[wid - 1, 0] && this[wid, 1] && this[wid - 1, 1];

        if (this[0, hei])
            span[hei * width] = (this[1, hei] && this[0, hei - 1]) ||
                              (this[1, hei - 1] && this[0, hei - 1]) ||
                              (this[1, hei] && this[1, hei - 1]);
        else
            span[hei * width] = this[1, hei] && this[0, hei - 1] && this[1, hei - 1];

        if (this[wid, hei])
            span[wid + hei * width] = (this[wid - 1, hei] && this[wid, hei - 1]) ||
                                    (this[wid - 1, hei - 1] && this[wid, hei - 1]) ||
                                    (this[wid - 1, hei] && this[wid - 1, hei - 1]);
        else
            span[0] = this[wid - 1, hei] && this[wid, hei - 1] && this[wid - 1, hei - 1];

        for (int j = 1; j < hei; j++)
        {
            count = 0;
            if (this[0, j + 1]) count++;
            if (this[1, j + 1]) count++;
            if (this[1, j]) count++;
            if (this[0, j - 1]) count++;
            if (this[1, j - 1]) count++;

            if (this[0, j])
                span[j * width] = !(count < 2 || count > 3);
            else
                span[j * width] = count == 3;

            count = 0;
            if (this[wid - 1, j + 1]) count++;
            if (this[wid, j + 1]) count++;
            if (this[wid - 1, j]) count++;
            if (this[wid - 1, j - 1]) count++;
            if (this[wid, j - 1]) count++;

            if (this[wid, j])
                span[wid + j * width] = !(count < 2 || count > 3);
            else
                span[wid + j * width] = count == 3;
        }

        for (int i = 1; i < wid; i++)
        {
            count = 0;
            if (this[i - 1, 1]) count++;
            if (this[i, 1]) count++;
            if (this[i + 1, 1]) count++;
            if (this[i - 1, 0]) count++;
            if (this[i + 1, 0]) count++;

            if (this[i, 0])
                span[i] = !(count < 2 || count > 3);
            else
                span[i] = count == 3;

            count = 0;
            if (this[i - 1, hei]) count++;
            if (this[i + 1, hei]) count++;
            if (this[i - 1, hei - 1]) count++;
            if (this[i, hei - 1]) count++;
            if (this[i + 1, hei - 1]) count++;

            if (this[i, hei])
                span[i + hei * width] = !(count < 2 || count > 3);
            else
                span[i + hei * width] = count == 3;
        }

        for (int j = 1; j < hei; j++)
        {
            for (int i = 1; i < wid; i++)
            {
                count = 0;
                if (this[i - 1, j + 1]) count++;
                if (this[i, j + 1]) count++;
                if (this[i + 1, j + 1]) count++;
                if (this[i - 1, j]) count++;
                if (this[i + 1, j]) count++;
                if (this[i - 1, j - 1]) count++;
                if (this[i, j - 1]) count++;
                if (this[i + 1, j - 1]) count++;
                if (this[i, j])
                    span[i + j * width] = !(count < 2 || count > 3);
                else
                    span[i + j * width] = count == 3;
            }
        }
    
        for (int n = 0; n < data.Length; n++)
            this.data[n] = span[n];
    }

    public string Copy(int i, int j, int wid, int hei)
    {
        StringBuilder s = new StringBuilder();
        wid += i;
        hei += j;
        for (int y = j; y < hei; y++)
        {
            for (int x = i; x < wid; x++)
            {
                if (this[x, y])
                    s.Append("*");
                else s.Append(" ");
            }
            s.Append("\n");
        }
        return s.ToString();
    }

    public void Past(int i, int j, string data)
    {
        int pi = i, pj = j;
        foreach (char c in data)
        {
            if (c == ' ')
                this[pi, pj] = false;
            else if (c == '*')
                this[pi, pj] = true;
            else if (c == '\n')
            {
                pi = i - 1;
                pj++;
            }
            if (++pi >= Width)
            {
                pi = i;
                pj++;
            }
            if (pj >= Height)
                return;
        }
    }
}