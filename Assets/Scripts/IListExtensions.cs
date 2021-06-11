using Immersive.FillInTheBlank;
using System;
using System.Collections.Generic;

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    //public static string FormateWord(this string word)
    //{
    //    switch (letterCase)
    //    {
    //        case LetterCase.Upper:
    //            word = word.ToUpper();
    //            break;
    //        case LetterCase.Lower:
    //            word = word.ToLower();
    //            break;
    //        case LetterCase.Capital:
    //            word = Char.ToLowerInvariant(word[0]) + word.Substring(1).ToLower();
    //            break;
    //        default:
    //            return word;
    //    }

    //    return word;
    //}
}