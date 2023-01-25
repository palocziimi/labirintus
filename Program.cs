using System;
using System.IO;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace Térképszerkesztő
{
    internal class Program
    {
        public static string lang = "Magyar";
        static void Main()
        {
            //Kezdőlap
            static void landPage()
            {
                Console.Clear();
                Console.WriteLine("Labirintus Játék | Térkép Készítő");
                Console.WriteLine("Nyomjon egy gombot a továbblépéshez!");
                char r = Console.ReadKey(intercept: true).KeyChar;
            }
            landPage();

            //Beállítások meghívása
            //string lang = "Magyar";
            int height = 7;
            int width = 25;
            bool run = true;
            while (run)
            {
                Console.Clear();
                Console.WriteLine("(Válaszoljon Számmal majd nyomjon ENTER-t!)");
                Console.WriteLine("1) Nyelvválasztás");
                Console.WriteLine("2) Saját pálya készítése");
                Console.WriteLine("3) Pálya Betöltése (Nem módosítás esetén a pálya mérete : (7*25))");
                string choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.Clear();
                    Console.WriteLine("1) English");
                    Console.WriteLine("2) Magyar");
                    string langChoice = Console.ReadLine();
                    if (langChoice == "1")
                    {
                        lang = "English";
                        Console.Clear();
                    }
                    else if (langChoice == "2")
                    {
                        lang = "Magyar";
                        Console.Clear();
                    }
                    else
                    {
                        lang = "Magyar";
                        Console.Clear();
                    }
                }
                else if (choice == "2")
                {
                    string inp;
                    bool valid = false;
                    while (!valid)
                    {
                        Console.Clear();
                        Console.WriteLine("A pálya magassága (Horizontális méret) : (min 5)");
                        inp = Console.ReadLine();
                        if (int.TryParse(inp, out height))
                        {
                            height = Convert.ToInt32(inp);
                            valid = true;
                        }
                    }

                    if (height < 5)
                    {
                        height = 5;
                    }

                    valid = false;

                    while (!valid)
                    {
                        Console.Clear();
                        Console.WriteLine("A pálya szélessége (Vertikális méret) : (min 5)");
                        inp = Console.ReadLine();
                        if (int.TryParse(inp, out width))
                        {
                            width = Convert.ToInt32(inp);
                            valid = true;
                        }
                    }

                    if (width < 5)
                    {
                        width = 5;
                    }

                    Console.Clear();
                    Console.WriteLine("1) Vissza a főmenübe");
                    Console.WriteLine("2) Tovább a pályaszerkesztőhöz");
                    string next = Console.ReadLine();
                    if (next == "2")
                    {
                        run = false;
                    }
                    Console.Clear();
                }
                else if (choice == "3")
                {
                    run = false;
                }
            }
            string settings = lang + " " + Convert.ToString(height) + " " + Convert.ToString(width);
            Console.Clear();

            //A pálya határainak legenerálása
            char[,] currentMap = general(height, width);
            static char[,] general(int height, int width)
            {
                char[,] baseMap = new char[height, width];
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


            //Kezdőpozíció
            int kx = 0;
            int ky = 0;



            //Építőelemek
            string components = "═║╗╣╝╩╚╠╔╦╬";
            char room = '█';


            //Alap információk kiírása
            static void welcome(int currentLength, string lang)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(currentLength + 15, 0);
                Console.WriteLine("| Mozgás : WASD | Pályaelemek letétele : 0-9 |");
                Console.SetCursorPosition(currentLength + 15, 1);
                Console.WriteLine("| Kereszteződés : K | Terem : T |");
                Console.SetCursorPosition(currentLength + 15, 2);
                Console.WriteLine("| Beállítások : B | Kilépés : Q |");
                Console.SetCursorPosition(currentLength + 15, 3);
                Console.WriteLine($"| Nyelv : {lang} | Törlés : Backspace |");
            }


            //Teremkereső
            static int roomSeek(char[,] map)
            {
                int roomCount = 0;
                for (int i = 0; i != map.GetLength(0); i++)
                {
                    for (int j = 0; j != map.GetLength(1); j++)
                    {
                        if (map[i, j] == '█')
                        {
                            roomCount++;
                        }
                    }
                }
                Console.BackgroundColor = ConsoleColor.Red;
                Console.SetCursorPosition(0, map.GetLength(0) + 7);
                Console.WriteLine($"Elhelyezett termek száma: {roomCount}");
                Console.BackgroundColor = ConsoleColor.Black;
                return roomCount;
            }
            

            //Kijárat számoló
            static int exitFinder(char[,] map)
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
                        if (map[i, map.GetLength(1) - 1] == RComponents[j])
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
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Kijáratok száma: {exitCount}");
                Console.BackgroundColor = ConsoleColor.Black;
                return exitCount;
            }


            //Koordináta Kiíratás
            static void write(int kx, int ky, int currentHeight)
            {
                Console.SetCursorPosition(0, currentHeight + 5);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine($"Szélesség: {kx.ToString("D2")} Magasság: {ky.ToString("D2")}");
                Console.SetCursorPosition(kx, ky);
                Console.BackgroundColor = ConsoleColor.Black;
            }


            //Kezdőpozíció | Alap kiírás
            Console.SetCursorPosition(kx, ky);
            welcome(currentMap.GetLength(1), lang);
            write(kx, ky, currentMap.GetLength(0));


            //Irányítás
            run = true;
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
                            write(kx, ky, currentMap.GetLength(0));
                        }
                        break;
                    case 's':
                        if (ky + 1 != currentMap.GetLength(0))
                        {
                            Console.SetCursorPosition(kx, ky + 1);
                            ky += 1;
                            write(kx, ky, currentMap.GetLength(0));
                        }
                        break;
                    case 'a':
                        if (kx - 1 > -1)
                        {
                            Console.SetCursorPosition(kx - 1, ky);
                            kx -= 1;
                            write(kx, ky, currentMap.GetLength(0));
                        }
                        break;
                    case 'd':
                        if (kx + 1 != currentMap.GetLength(1))
                        {
                            Console.SetCursorPosition(kx + 1, ky);
                            kx += 1;
                            write(kx, ky, currentMap.GetLength(0));
                        }
                        break;
                    case 'b':
                        Main();
                        run = false;
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
                    case '\b':
                        Console.Write(".");
                        currentMap[ky, kx] = '.';
                        Console.SetCursorPosition(kx, ky);
                        break;
                    case 'q':
                        roomSeek(currentMap);
                        exitFinder(currentMap);
                        if (roomSeek(currentMap) < 1 || exitFinder(currentMap) < 2)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.SetCursorPosition(currentMap.GetLength(1) + 15, currentMap.GetLength(0));
                            Console.WriteLine("A pályában muszáj minimum 1 szobának, és 2 kijáratnak lennie.");
                            Console.SetCursorPosition(kx, ky);
                            Console.BackgroundColor = ConsoleColor.Black;
                        }
                        else
                        {
                            Console.SetCursorPosition(currentMap.GetLength(1) + 15, currentMap.GetLength(0));
                            Console.BackgroundColor = ConsoleColor.Black;
                            string del = new String(' ', 100);
                            Console.WriteLine(del);
                            run = false;
                        }
                        break;
                }
            }
            //Mentés
            Console.Clear();
            run = true;
            while (run)
            {
                Console.WriteLine("1) Kilépés");
                Console.WriteLine("2) Fáljba mentés és Kilépés");
                string exit = Console.ReadLine();
                if (exit == "1")
                {
                    Console.WriteLine("Kilépés...");
                    Thread.Sleep(1000);
                    run = false;
                }
                else if (exit == "2")
                {
                    // Fájlba mentés
                    Console.WriteLine("Mentés helye : (Alap : Dokumentumok mappa)");
                    string docPath = Console.ReadLine();
                    if (docPath == "")
                    {
                        docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    }

                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "Labyrinth.sav")))
                    {
                        for (int i = 0; i != currentMap.GetLength(0); i++)
                        {
                            for (int j = 0; j != currentMap.GetLength(1); j++)
                            {
                                outputFile.Write(currentMap[i, j]);
                            }
                            outputFile.WriteLine();
                        }
                    }
                    Console.WriteLine("Fájl elmentve.");
                    Thread.Sleep(1000);
                    run = false;
                }
            }

        }
    }
}
        //todo 1: Konzisztencia
        //todo 2: nyelv