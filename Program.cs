// 2 digits:         91 x         99 =                 9009, 2ms
// 3 digits:        913 x        993 =               906609, 0ms
// 4 digits:       9901 x       9999 =             99000099, 0ms
// 5 digits:      99681 x      99979 =           9966006699, 1ms
// 6 digits:     999001 x     999999 =         999000000999, 10ms
// 7 digits:    9997647 x    9998017 =       99956644665999, 97ms
// 8 digits:   99990001 x   99999999 =     9999000000009999, 783ms
// 9 digits:  999920317 x  999980347 =   999900665566009999, 61158ms
//10 digits: 9999641743 x 9999850499 =  7761202105012021677, 1411124ms

// Multithreaded:
// 2 digits:          0 x          0 =                 9009, 5ms
// 3 digits:          0 x          0 =               906609, 0ms
// 4 digits:          0 x          0 =             99000099, 0ms
// 5 digits:          0 x          0 =           9966006699, 3ms
// 6 digits:          0 x          0 =         999000000999, 25ms
// 7 digits:          0 x          0 =       99956644665999, 210ms
// 8 digits:          0 x          0 =     9999000000009999, 964ms
// 9 digits:          0 x          0 =   999900665566009999, 96352ms
//10 digits:          0 x          0 =  7761202105012021677, 1995026ms

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

static ulong PartialMax(ulong i, ulong startHigh)
{
    ulong localMax = 0;

    for (ulong j = i; j <= startHigh; j++)
    {
        ulong palindrome = i * j;

        if (
            localMax < palindrome &&
            palindrome % 11 == 0 &&
            IsPalindromeBogdan(palindrome))
        {
            //x1 = i;
            //x2 = j;
            localMax = palindrome;
        }
    }

    return localMax;
}

static ulong PalindromeParallel(int nrDigits, out ulong x1, out ulong x2)
{
    x1 = 0;
    x2 = 0;

    // 999999
    ulong startFromMax = (ulong)Math.Pow(10, nrDigits) - 1;

    // 999999 - 10000
    ulong endWithSmallest = startFromMax - (ulong)Math.Pow(10, (int)Math.Round(nrDigits / 2m, 0) + 1) + 1;
    //ulong endWithSmallest = (ulong)Math.Pow(10, nrDigits - 1);

    ulong max = 0;
    int degreeOfParallelism = 2;

    Task<ulong>[] tasks = new Task<ulong>[degreeOfParallelism];

    for (ulong i = startFromMax; i >= endWithSmallest; i--)
    {
        if (max >= i * startFromMax)
        {
            break;
        }

        for (int taskNumber = 0; taskNumber < degreeOfParallelism; taskNumber++)
        {
            ulong diff = startFromMax - endWithSmallest;
            ulong part = diff / (ulong)degreeOfParallelism;
            ulong startHigh = startFromMax - ((ulong)taskNumber * part);

            tasks[taskNumber] = Task.Run(() => PartialMax(i, startHigh));
        }

        Task.WaitAll(tasks);

        ulong localMax = tasks.Max(t => t.Result);

        if (max < localMax)
        {
            max = localMax;
        }
    }

    return max;
}

static ulong Palindrome(int nrDigits, out ulong x1, out ulong x2)
{
    x1 = 0;
    x2 = 0;

    // 999999
    ulong startFromMax = (ulong)Math.Pow(10, nrDigits) - 1;

    // 999999 - 10000
    //ulong endWithSmallest = startFromMax - (ulong)Math.Pow(10, (int)Math.Round(nrDigits / 2m, 0) + 1) + 1;
    ulong endWithSmallest = (ulong)Math.Pow(10, nrDigits - 1);

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

for (int x = 2; x <= 10; x++)
{
    sw.Restart();
    ulong max = Palindrome(x, out ulong i, out ulong j);
    Console.WriteLine($"{x,2} digits: {i,10} × {j,10} = {max,20}, {sw.ElapsedMilliseconds}ms");
}
