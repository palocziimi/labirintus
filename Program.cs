using System;
using System.Collections.Generic;
using System.IO;

namespace labirintus
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.CursorVisible= false;

            DisplayMap(LoadMap("minta.txt"),2,3);

            bool kijutott = false;

            //while (!kijutott)
            //{
            //
            //}

            Console.ReadKey();
        }

        //Pályabetöltés - Loading map
        static char[,] LoadMap(string path)
        {
            string[] loaded = File.ReadAllLines(path);
            
            char[,] map = new char[loaded.Length, loaded[0].Length];

            for (int i = 0; i < loaded.Length; i++)
            {
                for (int j = 0; j < loaded[0].Length; j++)
                {
                    map[i, j] = loaded[i][j];
                }
            }

            return map;
        }

        //todo boviteni paraméterekkel
        static void DisplayMap(char[,] map, int kx, int ky)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.SetCursorPosition(j + kx, i + ky);
                    Console.Write(map[i,j]);
                }
            }
        }

        //Egy útszakasz lehetséges irányai - Directions of a piece of road
        static string CharDirection(char character)
        {
            switch (character)
            {
                case '╬':
                    return "nswe";
                    break;

                case '═':
                    return "we";
                    break;

                case '╦':
                    return "nswe";
                    break;

                case '╩':
                    return "nwe";
                    break;

                case '║':
                    return "ns";
                    break;

                case '╣':
                    return "nsw";
                    break;

                case '╠':
                    return "nse";
                    break;

                case '╗':
                    return "sw";
                    break;

                case '╝':
                    return "nw";
                    break;

                case '╚':
                    return "ne";
                    break;

                case '╔':
                    return "se";
                    break;


                default:
                    return "-";
                    break;
            }
        }

        //Összegyűjti a bejáratokat - Collect all entries
        static List<string> AllEntry(char[,] map)
        {
            List<string> entries = new List<string>();

            for (int topCounter = 1; topCounter < map.GetLength(1) - 1; topCounter++)
            {
                if (CharDirection(map[0, topCounter]).Contains('n'))
                    entries.Add($"0-{topCounter}");
            }
            
            for (int bottomCounter = 1; bottomCounter < map.GetLength(1) - 1; bottomCounter++)
            {
                if (CharDirection(map[map.GetLength(0) - 1, bottomCounter]).Contains('s'))
                    entries.Add($"{map.GetLength(0) - 1}-{bottomCounter}");
            }
            
            for (int leftCounter = 1; leftCounter < map.GetLength(0) - 1; leftCounter++)
            {
                if (CharDirection(map[leftCounter, 0]).Contains('w'))
                    entries.Add($"{leftCounter}-0");
            }
            
            for (int rightCounter = 1; rightCounter < map.GetLength(0) - 1; rightCounter++)
            {
                if (CharDirection(map[rightCounter, map.GetLength(1) - 1]).Contains('e'))
                    entries.Add($"{rightCounter}-{map.GetLength(1) - 1}");
            }

            if (CharDirection(map[0, 0]).Contains('n') || CharDirection(map[0, 0]).Contains('w'))
                entries.Add("0-0");

            if (CharDirection(map[0, map.GetLength(1) - 1]).Contains('n') || CharDirection(map[0, map.GetLength(1) - 1]).Contains('e'))
                entries.Add($"0-{map.GetLength(1) - 1}");

            if (CharDirection(map[map.GetLength(0) - 1, 0]).Contains('s') || CharDirection(map[map.GetLength(0) - 1, 0]).Contains('e'))
                entries.Add($"{map.GetLength(0) - 1}-0");

            if (CharDirection(map[map.GetLength(0) - 1, map.GetLength(1) - 1]).Contains('s') || CharDirection(map[map.GetLength(0) - 1, map.GetLength(1) - 1]).Contains('w'))
                entries.Add($"{map.GetLength(0) - 1}-{map.GetLength(1) - 1}");

            return entries;
        }

        static void Character(ConsoleKey key)
        {

        }





    }
}
