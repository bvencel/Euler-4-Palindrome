// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

static bool IsPalindromeBogdan(ulong palindromCandidate)
{
    string nr = palindromCandidate.ToString();
    int i = 0;

    while (i < nr.Length / 2)
    {
        if (nr[i] != nr[nr.Length - 1 - i])
        {
            return false;
        }

        i++;
    }

    return true;
}

static ulong Palindrome(int nrDigits, out ulong x1, out ulong x2)
{
    x1 = 0;
    x2 = 0;

    // 999999
    ulong startFromMax = (ulong)Math.Pow(10, nrDigits) - 1;

    // 999999 - 10000
    ulong endWithSmallest = startFromMax - (ulong)Math.Pow(10, (int)Math.Round(nrDigits / 2m, 0) + 1) + 1;
    endWithSmallest = endWithSmallest / 2;

    ulong max = 0;

    for (ulong i = startFromMax; i >= endWithSmallest; i--)
    {
        if (max >= i * startFromMax)
        {
            break;
        }

        // Since i*j = j * i you only need to calculate the product for all j>= i
        for (ulong j = i; j <= startFromMax; j++)
        {
            ulong palindrome = i * j;

            if (
                max < palindrome &&
                palindrome % 11 == 0 &&
                IsPalindromeBogdan(palindrome))
            {
                x1 = i;
                x2 = j;
                max = palindrome;
            }
        }
    }

    return max;
}

Stopwatch sw = new();

for (int x = 2; x < 20; x++)
{
    sw.Restart();
    ulong max = Palindrome(x, out ulong i, out ulong j);
    Console.WriteLine($"{x,2} digits: {i,10} × {j,10} = {max,20}, {sw.ElapsedMilliseconds}ms");
}
