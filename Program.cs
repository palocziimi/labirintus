using System;

namespace Térképszerkesztő
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //A pálya határainak legenerálása
            char[,] baseMap = new char[6, 22];
            for (int i = 0; i != baseMap.GetLength(0); i++)
            {
                for (int j = 0; j != baseMap.GetLength(1); j++)
                {
                    baseMap[i, j] = '.';
                    Console.Write(baseMap[i,j]);
                }
                Console.WriteLine();
            }

            //Kezdőpozíció
            int kx = 0;
            int ky = 0;
            Console.SetCursorPosition(kx, ky);

            //Építőelemek
            string components = "═║╗╣╝╩╚╠╔╦╬";
            char room = '█';

            //Irányítás
            bool run = true;
            while (run)
            {
                char input = Convert.ToChar(Convert.ToString(Console.ReadKey(intercept: true).KeyChar).ToLower());
                switch (input)
                {
                    case 'w':
                        Console.SetCursorPosition(kx, ky - 1);
                        ky -= 1;
                        break;
                    case 's':
                        Console.SetCursorPosition(kx, ky + 1);
                        ky += 1;
                        //Pályatér Betartás | Alsó határ
                        if (ky + 1 > baseMap.GetLength(0))
                        {
                            ky -= 1;
                            Console.SetCursorPosition(kx, ky);
                        }
                        break;
                    case 'a':
                        Console.SetCursorPosition(kx - 1, ky);
                        kx -= 1;
                        break;
                    case 'd':
                        Console.SetCursorPosition(kx + 1, ky);
                        kx += 1;
                        //Pályatér Betartás | Külső Határ
                        if (kx + 1 > baseMap.GetLength(1))
                        {
                            kx -= 1;
                            Console.SetCursorPosition(kx, ky);
                        }
                        break;

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        Console.Write(components[input - '0']);
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 'k':
                        Console.Write(components[components.Length - 1]);
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 't':
                        Console.Write(room);
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 'q':
                        run = false;
                        break;
                }
            }
            Console.ReadKey();
        }
        //todo 1: Minimális szinten is meg kell akadályozni a kijárat és terem nélküli térkép mentését.
    }
}