using System;
using System.IO;

namespace Labirintus_játék
{
    internal class Program
    {
        static void Main(string[] args)
        {
            DisplayMap(LoadMap("minta.txt"),2,3);
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

    }
}
