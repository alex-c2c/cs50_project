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
                    encodedString += $"{count}{currChar}";
                    count = 0;
                }

                currChar = s[i];
            }

            count++;
        }

        if (count > 0)
        {
            encodedString += $"{count}{currChar}";
        }

        return encodedString;
    }

    public static string ReverseRLE(string s)
    {
        string decodedString = "";
        string numString = "";

        for (int i = 0; i < s.Length; i++)
        {
            if (Char.IsNumber(s[i]))
            {
                numString += s[i];
            }
            else
            {
                int num = numString.ToInt();
                for (int j = 0; j < num; j++)
                {
                    decodedString += s[i];
                }

                numString = "";
                continue;
            }
        }

        return decodedString;
    }
}
