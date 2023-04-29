using System;
using System.Text;
using System.Diagnostics;

class TestProgram
{
    // Global Comment
    double? globalDecsign = 29.2000;
    float globalDeclare;

    void Main(string[] args)
    {
        // Local Comment
        var localDecsign = "(Helo // World) (/* Test Comment */)";  // Inline Comment
        string? localDeclare;
        localDeclare = null;

        double LocalFunction()
        {
            return 3.14; /* Another Inline Comment With Code:  for (int i = 0; i < 10; i += 5) i++; */
        }

        if (1 > 2)
            return;

        if (5 >= 6) { }

        if (false) { }
        else if (true) { }
        else { }

        while (false) { }

        do { }
        while (false);

        int i = 0;
        for (Print(); i < 10; i += 5)
            i++;

        /*
        Multi Comment Code:
            /* for (int i = 0; i < 10; i += 5) i++; *\/
        */
    }

    void Print()
    {
        Console.WriteLine("For Loop Started");
    }
}