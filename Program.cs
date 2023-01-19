using System;

namespace Térképszerkesztő
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //A pálya határainak legenerálása
            char[,] currentMap = general();
            static char[,] general()
            {
                char[,] baseMap = new char[6, 24];
                for (int i = 0; i != baseMap.GetLength(0); i++)
                {
                    for (int j = 0; j != baseMap.GetLength(1); j++)
                    {
                        baseMap[i, j] = '.';
                    }
                }
                return baseMap;
            }


            //Megjelenítés
            for (int i = 0; i != currentMap.GetLength(0); i++)
            {
                for (int j = 0; j != currentMap.GetLength(1); j++)
                {
                    Console.Write(currentMap[i, j]);
                }
                Console.WriteLine();
            }


            //Teremkereső
            static void roomSeek(char[,] map)
            {
            int roomCount = 0;
                for (int i = 0; i != map.GetLength(0); i++)
                {
                    for (int j = 0; j != map.GetLength(1); j++)
                    {
                        if (map[i,j] == '█')
                        {
                            roomCount++;
                        }
                    }
                }
                Console.BackgroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(0,21);
                Console.WriteLine($"Elhelyezett termek száma: {roomCount}");
            }


            //Építőelemek
            string components = "═║╗╣╝╩╚╠╔╦╬";
            char room = '█';


            //Kijárat számoló
            static void exitFinder(char[,] map)
            {
                int exitCount = 0;
                string LComponents = "═╗╣╝╩╦╬";
                string RComponents = "═╩╚╠╔╦╬";
                string TRComponents = "║╣╝╩╚╠╬";
                string BRComponents = "║╗╣╠╔╦╬";
                for (int i = 0; i != map.GetLength(0); i++)
                {
                    //oszlopok
                    for (int j = 0; j != LComponents.Length; j++)
                    {
                        //bal szélső oszlop
                        if (map[i, 0] == LComponents[j])
                        {
                            exitCount++;
                        }
                        //jobb szélső oszlop
                        if (map[i, map.GetLength(1)-1] == RComponents[j])
                        {
                            exitCount++;
                        }
                    }
                }
                for (int i = 0; i != map.GetLength(1); i++)
                {
                    //sorok
                    for (int j = 0; j != TRComponents.Length; j++)
                    {
                        //Felső sor
                        if (map[0, i] == TRComponents[j])
                        {
                            exitCount++;
                        }
                        //Alsó sor
                        if (map[map.GetLength(0) - 1, i] == BRComponents[j])
                        {
                            exitCount++;
                        }
                    }
                }
                Console.WriteLine($"Kijáratok száma: {exitCount}");
            }


            //Kezdőpozíció
            int kx = 0;
            int ky = 0;
            Console.SetCursorPosition(kx, ky);


            //Koordináta Kiíratás
            static void write(int kx, int ky)
            {
                Console.SetCursorPosition(0, 20);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Szélesség: {kx.ToString("D2")} Magasság: {ky.ToString("D2")}");
                Console.SetCursorPosition(kx, ky);
                Console.BackgroundColor = ConsoleColor.Black;
            }


            //Kezdőpozíció
            Console.SetCursorPosition(kx, ky);


            //Irányítás
            bool run = true;
            while (run)
            {
                char input = Convert.ToChar(Convert.ToString(Console.ReadKey(intercept: true).KeyChar).ToLower());
                switch (input)
                {
                    case 'w':
                        if (ky - 1 > -1)
                        {
                            Console.SetCursorPosition(kx, ky - 1);
                            ky -= 1;
                            write(kx, ky);
                        }
                        break;
                    case 's':
                        if (ky + 1 != currentMap.GetLength(0))
                        {
                            Console.SetCursorPosition(kx, ky + 1);
                            ky += 1;
                            write(kx, ky);
                        }
                        break;
                    case 'a':
                        if (kx - 1 > -1)
                        {
                            Console.SetCursorPosition(kx - 1, ky);
                            kx -= 1;
                            write(kx, ky);
                        }
                        break;
                    case 'd':
                        if (kx + 1 != currentMap.GetLength(1))
                        {
                            Console.SetCursorPosition(kx + 1, ky);
                            kx += 1;
                            write(kx, ky);
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
                        currentMap[ky, kx] = components[input - '0'];
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 'k':
                        Console.Write(components[components.Length - 1]);
                        currentMap[ky, kx] = components[components.Length - 1];
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 't':
                        Console.Write(room);
                        currentMap[ky, kx] = room;
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 'q':
                        run = false;
                        roomSeek(currentMap);
                        exitFinder(currentMap);
                        break;
                }
            }
            Console.ReadKey();
        }
        //todo 1: Minimális szinten is meg kell akadályozni a kijárat és terem nélküli térkép mentését. | Ellenőrizni a begyűjtött értékeket
        //todo 2: Alap információk
    }
}