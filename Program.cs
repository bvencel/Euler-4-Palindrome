/*
     3 digits:            913 x            993 =                       906609, 7ms
     4 digits:           9999 x           9901 =                     99000099, 0ms
     5 digits:          99681 x          99979 =                   9966006699, 11ms
     6 digits:         999999 x         999001 =                 999000000999, 0ms
     7 digits:        9997647 x        9998017 =               99956644665999, 812ms
     8 digits:       99999999 x       99990001 =             9999000000009999, 0ms
     9 digits:      999920317 x      999980347 =           999900665566009999, 508259ms
    10 digits:     9999999999 x     9999900001 =         99999000000000099999, 0ms
    11 digits:    99999943851 x    99999996349 =       9999994020000204999999, 420031ms
    12 digits:   999999999999 x   999999000001 =     999999000000000000999999, 0ms
    13 digits: ?
    14 digits: 99999999999999 x 99999990000001 = 9999999000000000000009999999, 0ms
*/

using System.Diagnostics;
using System.Numerics;

static bool IsPalindrome(BigInteger palindromCandidate)
{
    string palindromeString = palindromCandidate.ToString();
    int length = palindromeString.Length;

    if (length % 2 != 0)
    {
        return false;
    }

    int i = 0;

    while (i < length / 2)
    {
        if (palindromeString[i] != palindromeString[length - 1 - i])
        {
            return false;
        }

        i++;
    }

    return true;
}

/// <summary>
/// Palindromes with even number of digits in their multipliers always follow a 
/// pattern:
///  6:     999001 x     999999 =         999000000999
///  8:   99990001 x   99999999 =     9999000000009999
/// 10: 9999900001 x 9999999999 = 99999000000000099999
/// </summary>
static BigInteger GeneratePalindromeFromEvenDigits(
    int nrDigits,
    out BigInteger x1,
    out BigInteger x2)
{
    if (nrDigits % 2 != 0)
    {
        x1 = 0;
        x2 = 0;

        return 0;
    }

    // E.g. 9999999999
    x1 = (BigInteger)(Math.Pow(10, nrDigits) - 1);

    // E.g. 9999900001
    x2 = x1 - (BigInteger)Math.Pow(10, nrDigits / 2) + 2;

    return x1 * x2;
}

static BigInteger Palindrome(int nrDigits, out BigInteger x1, out BigInteger x2)
{
    const bool reduceSpace = true;
    x1 = 0;
    x2 = 0;

    // 1 digit numbers cannot form a palindrome
    if (nrDigits <= 1)
    {
        return 0;
    }

    if (nrDigits % 2 == 0)
    {
        // In case of even numbered digits we cen directly construct the results
        return GeneratePalindromeFromEvenDigits(nrDigits, out x1, out x2);
    }

    // 999999
    BigInteger startFromMax = (BigInteger)Math.Pow(10, nrDigits) - 1;

    // 100000
    BigInteger endWithSmallest = (BigInteger)Math.Pow(10, nrDigits - 1);

    if (reduceSpace)
    {
        // Cheat, by artificially reducing the explored space
        // 990000
        endWithSmallest =
            startFromMax -
            (BigInteger)Math.Pow(10, (nrDigits / 2) + 1);
    }

    BigInteger max = 0;


    for (BigInteger i = startFromMax; i >= endWithSmallest; i--)
    {
        if (max >= i * startFromMax)
        {
            break;
        }

        // Since i*j = j * i you only need to calculate the product for all j>= i
        for (BigInteger j = i; j <= startFromMax; j++)
        {
            BigInteger palindrome = i * j;

            if (
                max < palindrome &&
                palindrome % 11 == 0 &&
                IsPalindrome(palindrome))
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

for (int x = 13; x <= 13; x++)
{
    //// Only count pair multipliers, because the odd ones take too long
    //if (x > 6)
    //{
    //    x++;
    //}

    sw.Restart();
    BigInteger max = Palindrome(x, out BigInteger i, out BigInteger j);

    if (max == 0)
    {
        Console.WriteLine($"{x,2} digits: No solution was found, " +
            $"{sw.ElapsedMilliseconds}ms");
    }
    else
    {
        Console.WriteLine($"{x,2} digits: {i,14} × {j,14} = {max,28}, " +
            $"{sw.ElapsedMilliseconds}ms");
    }
}