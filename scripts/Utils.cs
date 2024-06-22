using System;
using System.Collections.Generic;
using Godot;

public static class Utils
{
    public static string ApplyRLE(string s)
    {
        char currChar = '-';
        int count = 0;
        string encodedString = "";

        for (int i = 0; i < s.Length; i++)
        {
            if (currChar != s[i])
            {
                if (count > 0)
                {
                    encodedString += $"{currChar}{count}";
                    count = 0;
                }

                currChar = s[i];
            }

            count++;
        }

        if (count > 0)
        {
            encodedString += $"{currChar}{count}";
        }

        return encodedString;
    }

    private static string DuplicateChar(char c, string numString)
    {
        if (c == Char.MinValue || numString.Length <= 0)
        {
            return "";
        }

        string s = "";
        int count = numString.ToInt();

        for (int i = 0; i < count; i++)
        {
            s += c;
        }

        return s;
    }

    private static bool AppendSubstring(char c, string numString, ref string decodedString)
    {
        if (c == Char.MinValue || numString.Length <= 0)
        {
            return false;
        }

        int count = numString.ToInt();
        if (count <= 0)
        {
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            decodedString += c;
        }

        return true;
    }

    public static string ReverseRLE(string s)
    {
        string decodedString = "";
        string numString = "";
        char currChar = Char.MinValue;

        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            if (Char.IsLetter(c))
            {
                if (AppendSubstring(currChar, numString, ref decodedString))
                {
                    currChar = Char.MinValue;
                    numString = "";
                }

                currChar = c;
            }
            else if (Char.IsNumber(c))
            {
                numString += c;
            }
        }

        AppendSubstring(currChar, numString, ref decodedString);

        return decodedString;
    }
}
