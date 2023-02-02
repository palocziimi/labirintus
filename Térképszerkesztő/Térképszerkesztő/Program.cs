using System;
using System.Collections.Generic;
using System.IO;
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
                Console.WriteLine((lang == "Magyar") ? Properties.Resources.landPageTextHU : Properties.Resources.landPageTextENG);
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
                Console.WriteLine((lang == "Magyar") ? Properties.Resources.settingsHU : Properties.Resources.settingsENG);
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
                        Console.WriteLine((lang == "Magyar") ? Properties.Resources.askHeightHU : Properties.Resources.askHeightENG);
                        inp = Console.ReadLine();
                        if (int.TryParse(inp, out height))
                        {
                            height = Convert.ToInt32(inp) + 1;
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
                        Console.WriteLine((lang == "Magyar") ? Properties.Resources.askWidthHU : Properties.Resources.askWidthENG);
                        inp = Console.ReadLine();
                        if (int.TryParse(inp, out width))
                        {
                            width = Convert.ToInt32(inp) + 1;
                            valid = true;
                        }
                    }

                    if (width < 5)
                    {
                        width = 5;
                    }

                    Console.Clear();
                    Console.WriteLine((lang == "Magyar") ? Properties.Resources.askNextHU : Properties.Resources.askNextENG);
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
                Console.WriteLine((lang == "Magyar") ? Properties.Resources.info1HU : Properties.Resources.info1ENG);
                Console.SetCursorPosition(currentLength + 15, 1);
                Console.WriteLine((lang == "Magyar") ? Properties.Resources.info2HU : Properties.Resources.info2ENG);
                Console.SetCursorPosition(currentLength + 15, 2);
                Console.WriteLine((lang == "Magyar") ? Properties.Resources.info3HU : Properties.Resources.info3ENG);
                Console.SetCursorPosition(currentLength + 15, 3);
                Console.WriteLine((lang == "Magyar") ? string.Format(Properties.Resources.info4HU, lang) : string.Format(Properties.Resources.info4ENG, lang));
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
                Console.WriteLine((lang == "Magyar") ? string.Format(Properties.Resources.roomHU, roomCount) : string.Format(Properties.Resources.roomENG, roomCount));
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
                Console.SetCursorPosition(0, map.GetLength(0) + 8);
                Console.WriteLine((lang == "Magyar") ? string.Format(Properties.Resources.exfilHU, exitCount) : string.Format(Properties.Resources.exfilENG, exitCount));
                Console.BackgroundColor = ConsoleColor.Black;
                return exitCount;
            }


            //Koordináta Kiíratás
            static void write(int kx, int ky, int currentHeight)
            {
                Console.SetCursorPosition(0, currentHeight + 5);
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine((lang == "Magyar") ? string.Format(Properties.Resources.cordsHU, kx.ToString("D2"), ky.ToString("D2")) : string.Format(Properties.Resources.cordsENG, kx.ToString("D2"), ky.ToString("D2")));
                Console.SetCursorPosition(kx, ky);
                Console.BackgroundColor = ConsoleColor.Black;
            }


            //Kezdőpozíció | Alap kiírás
            Console.SetCursorPosition(kx, ky);
            welcome(currentMap.GetLength(1), lang);
            write(kx, ky, currentMap.GetLength(0));

            //Konzisztencia ellenőrzés


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
                            Console.WriteLine((lang == "Magyar") ? Properties.Resources.errorHU : Properties.Resources.errorENG);
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
                Console.WriteLine((lang == "Magyar") ? Properties.Resources.exitHU : Properties.Resources.exitENG);
                string exit = Console.ReadLine();
                if (exit == "1")
                {
                    Console.WriteLine((lang == "Magyar") ? Properties.Resources.tempExHU : Properties.Resources.tempExENG);
                    Thread.Sleep(1000);
                    run = false;
                }
                else if (exit == "2")
                {
                    // Fájlba mentés
                    Console.WriteLine((lang == "Magyar") ? Properties.Resources.askSaveHU : Properties.Resources.askSaveENG);
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
                    Console.WriteLine((lang == "Magyar") ? Properties.Resources.tempSaveHU : Properties.Resources.tempSaveENG);
                    Thread.Sleep(1000);
                    run = false;
                }
            }

        }
    }
}
        //todo 1: Konzisztencia