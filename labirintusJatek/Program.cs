using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace labirintus
{
    internal class Program
    {
        static void Main(string[] args)
        {

            bool finishGame = false;

            while (!finishGame)
            {
                bool gotOut = false;

                string[] data = MazeMap.Setup(
                    Other.OptionChoose(new string[] { "Válasszon nyelvet!", "Choose a language!", "Wählen Sie eine Sprache!" },
                                       new string[] { "Magyar", "English", "Deutsch" }, speed: 8));

                if (data[0] == "quit")
                {
                    finishGame = true;
                    break;
                }
                
                MazeMap maze = new MazeMap(data);
                Character character = new Character(data);

                MazeMap.Start(maze, character);

                while (!gotOut)
                {
                    maze.PrintStats(Character.WhereToMove(maze.map, character.x, character.y), character.stepCount, character.roomsExplored.Count);
                    Console.SetCursorPosition(character.x + 5, character.y + (maze.easymode ? 8 : 6));
                    character.Move(maze, Console.ReadKey(true).Key);
                    if (character.GotOut(maze)) gotOut = true;
                }
                Other.ConsoleCSS();

            }

            Console.BackgroundColor = ConsoleColor.Black;
            Other.PrintTheEnd(ConsoleColor.DarkCyan);
            Console.ReadKey(true);
            Console.SetCursorPosition(0, 26);
            



        }

    }

    public class MazeMap
    {
        public char[,] map;
        public string path;
        public string lang;
        Random rnd = new Random();
        public ConsoleColor bgColor = ConsoleColor.Green;

        //Nyelvek - Languages
        public string[] words = new string[36];


        //true = easy(visible map)
        //false = hard(dynamically visible map);
        public bool easymode;

        //public MazeMap() { }
        public MazeMap(string[] data)
        {
            string[] colors = { "Red", "Blue", "Yellow", "Magenta", "Green" };
            
            if (data[0] == "0") lang = "hu"; 
            else if (data[0] == "1") lang = "en";
            else if (data[0] == "2") lang = "de";

            map = LoadMap(".\\saves\\" + data[1] + ".txt");
            path = data[1];

            bgColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colors[Convert.ToInt32(data[3]) - 13]);

            if (data[4] == "8") easymode = true;
            else if (data[4] == "9") easymode = false;

            words = File.ReadAllLines($"lang-{lang}.txt");
        }

        
        public static string[] Setup(int chosenLang, ConsoleColor bgColor = ConsoleColor.Black)
        {
            string[] langWords = new string[37];
            if (chosenLang == -1) return new string[] {"quit"};
            else if (chosenLang == 0) langWords = File.ReadAllLines("lang-hu.txt");
            else if (chosenLang == 1) langWords = File.ReadAllLines("lang-en.txt");
            else if (chosenLang == 2) langWords = File.ReadAllLines("lang-de.txt");

            
            Other.PrintCompleteData(langWords[1].Split(';'));
            
            //lang0, mapName, saveBool, mapColor0, difficulty0, charName, charColor0, x, y, stepCount, roomsExplored
            string[] data = new string[11]; 
            ConsoleKey decision = ConsoleKey.R;

            //Set the yes key
            ConsoleKey yes = ConsoleKey.I;
            if (langWords[0] == "Válasszon nyelvet!") yes = ConsoleKey.I;
            else if (langWords[0] == "Choose a language!") yes = ConsoleKey.Y;
            else if (langWords[0] == "Wählen Sie eine Sprache!") yes = ConsoleKey.J;

            data[0] = chosenLang.ToString();
            Other.ConsoleCSS(back: bgColor);

            //Map name
            do
            {
                Other.TypeWrite($"{langWords[2]}: ", 0, 2, false);
                Console.SetCursorPosition(langWords[2].Length + 2, 2);
                data[1] = Console.ReadLine();
                if (File.Exists(".\\saves\\" + data[1] + ".txt")) break;
                else
                {
                    Other.TypeWrite($"{langWords[3]}", 0, 2, false, 8);
                    for (int i = 3; i > 0; i--)
                    {
                        Console.SetCursorPosition(langWords[3].Length + 2, 2);
                        Console.Write(i);
                        Thread.Sleep(1000);
                    }
                    Console.SetCursorPosition(0, 2);
                    for (int i = 0; i < langWords[3].Length + 3; i++) Console.Write(' ');
                }
            } while (true);
            Other.PrintCompleteData(new string[] { langWords[2], data[1] }, 1);
            Console.SetCursorPosition(0, 2);
            for (int i = 0; i < langWords[2].Length + data[1].Length + 3; i++) Console.Write(' ');

            //Savefile + if continue then load and play
            if (File.Exists(".\\saves\\" + data[1] + ".sav"))
            {
                Other.TypeWrite($"{langWords[4]}: ", 0, 3, false, 8);
                do
                {
                    decision = Console.ReadKey(true).Key;
                    if (decision == yes) data[2] = "true";
                    else if (decision == ConsoleKey.N) data[2] = "false";
                
                } while (decision != yes && decision != ConsoleKey.N);
                
                for (int i = 0; i < 2; i++)
                {
                    Console.SetCursorPosition(0, i + 2);
                    for (int j = 0; j < langWords[4].Length + 3; j++) Console.Write(' ');
                }
                Other.PrintCompleteData(new string[] { langWords[5].Split(';')[0], langWords[5].Split(';')[data[2] == "true" ? 2 : 1] }, 2);

                if (data[2] == "true")
                {
                    Thread.Sleep(700);

                    string[] saveData = File.ReadAllLines(".\\saves\\" + data[1] + ".sav");
                    for (int i = 3; i < data.Length; i++)
                    {
                        if (saveData[i - 3].Split(':')[0] == "x" || saveData[i - 3].Split(':')[0] == "y")
                        {
                            data[i] = saveData[i - 3].Split(':')[1];
                            continue;
                        }
                        data[i] = saveData[i - 3].Split(':')[2];
                    }

                    for (int i = 3; i < data.Length - 4; i++)
                    {
                        if (i != 5) Other.PrintCompleteData(new string[] { langWords[Convert.ToInt32(saveData[i - 3].Split(':')[1])],
                                                                           langWords[Convert.ToInt32(saveData[i - 3].Split(':')[2])] }, i);
                        else Other.PrintCompleteData(new string[] { langWords[Convert.ToInt32(saveData[i - 3].Split(':')[1])],
                                                                    saveData[i - 3].Split(':')[2] }, i);
                        Thread.Sleep(700);
                    }
                    Thread.Sleep(1000);
                    Console.Clear();
                    return data;
                }
            }

            data[2] = "false";
            Other.PrintCompleteData(new string[] { langWords[5].Split(';')[0], langWords[5].Split(';')[1] }, 2);

            //Mapcolor
            int chosen = Other.OptionChoose(new string[] { langWords[6] },
                               new string[] { langWords[13], langWords[14], langWords[15], langWords[16], langWords[17] }, 
                               0, 4, txtColor: ConsoleColor.DarkYellow
                               );
            if (chosen == -1) return new string[] { "quit" };
            else data[3] = (13 + chosen).ToString();
            Other.PrintCompleteData(new string[] { langWords[6], langWords[Convert.ToInt32(data[3])] }, 3);

            //Difficulty
            chosen = Other.OptionChoose(new string[] { langWords[7] },
                               new string[] { langWords[8], langWords[9] },
                               0, 5, txtColor: ConsoleColor.Red
                               );
            if (chosen == -1) return new string[] { "quit" };
            else data[4] = (8 + chosen).ToString();
            Other.PrintCompleteData(new string[] { langWords[7], langWords[Convert.ToInt32(data[4])] }, 4);

            //Character name
            Other.TypeWrite($"{langWords[11]}: ", 0, 6, false);
            Console.SetCursorPosition(langWords[11].Length + 2, 6);
            data[5] = Console.ReadLine();
               
            Other.PrintCompleteData(new string[] { langWords[11], data[5] }, 5);
            Console.SetCursorPosition(0, 6);
            for (int i = 0; i < langWords[11].Length + data[5].Length + 3; i++) Console.Write(' ');

            //Character color
            bool button = false;
            int[] x = new int[4];
            for (int i = 0; i < 4; i++) 
            {
                if ($"{(13 + i)}" == data[3]) button = true;
                if (!button) x[i] = 13 + i;
                else x[i] = 14 + i;
            } 
            
            chosen = Other.OptionChoose( new string[] { langWords[12] }, 
                                         new string [] { langWords[x[0]], langWords[x[1]], langWords[x[2]], langWords[x[3]], }, 
                                         0, 7, txtColor: ConsoleColor.DarkCyan);

            if (chosen == -1) return new string[] { "quit" };
            else data[6] = x[chosen].ToString();
            Other.PrintCompleteData(new string[] { langWords[12], langWords[Convert.ToInt32(data[6])] }, 6);

            Other.TypeWrite($"{langWords[36]}", 5, 15, false, 10);
            Console.ReadKey(true);

            Console.Clear();
            data[7] = "-1";
            data[8] = "-1";
            data[9] = "0";
            data[10] = "";
            return data;
        }
        public static void Start(MazeMap maze, Character character)
        {
            Console.CursorVisible = false;
            if (character.x == -1 && character.y == -1)
            {
                int[] startKoords = maze.RandomEntry(AllEntry(maze.map));
                character.y = startKoords[0];
                character.x = startKoords[1];
            }

            maze.DisplayMap(Character.WhereToMove(maze.map, character.x, character.y));

            Console.SetCursorPosition(character.x + 5, character.y + (maze.easymode ? 8 : 6));
            Console.BackgroundColor = character.charColor;
            Console.Write(maze.map[character.y, character.x]);
            Console.SetCursorPosition(character.x + 5, character.y + (maze.easymode ? 8 : 6));
        }

        //Pályabetöltés - Loading map
        public static char[,] LoadMap(string path)
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

        //todo boviteni paraméterekkel ?
        public void DisplayMap(List<char> stepOptions)
        {
            Console.SetCursorPosition(0, 0);
            Console.BackgroundColor = ConsoleColor.Green;
            Console.Write($"{words[18]}:{path}, {words[19]}: {map.GetLength(0)} {words[20]} x {map.GetLength(1)} {words[21]}");
            Console.BackgroundColor = ConsoleColor.Black;

            PrintStats(stepOptions);

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    Console.SetCursorPosition(j + 5, i + (easymode ? 8 : 6));
                    Console.BackgroundColor = bgColor;

                    if (easymode) Console.Write(map[i, j]);
                    else Console.Write(' ');
                }
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }


        public void PrintStats(List<char> stepOptions, int stepCount = 0, int exploredRooms = 0)
        {
            Console.BackgroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(0, 1);
            if (easymode)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.SetCursorPosition(0, 1);
                Console.WriteLine(
                "                                         " +
                "\n" +
                "                                                                 "
                );
                Console.BackgroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(0, 1);
                string dirs = "";
                if (stepOptions.Contains('d')) dirs += $"{words[30]},";
                if (stepOptions.Contains('a')) dirs += $"{words[31]},";
                if (stepOptions.Contains('w')) dirs += $"{words[28]},";
                if (stepOptions.Contains('s')) dirs += $"{words[29]},";
                dirs = dirs.Substring(0, dirs.Length - 1);

                Console.WriteLine(
                    $"{words[22]}: {stepCount}, {words[23]}: {exploredRooms}" +
                    "\n" +
                    $"{words[24]}: {dirs}" +
                    "\n");
                Console.SetCursorPosition(0, 3);
            }
            Console.BackgroundColor = ConsoleColor.Black;
        }

        //'╬','═','╦','╩','║','╣','╠','╗','╝','╚', '╔'
        //Egy útszakasz lehetséges irányai - Directions of a piece of road
        public static List<int[]> AllEntry(char[,] mapInput)
        {
            List<int[]> entries = new List<int[]>();

            char[] up = { '╬', '╩', '╣', '╠', '║', '╝', '╚' };
            char[] down = { '╬', '╦', '╣', '╠', '║', '╗', '╔' };
            char[] left = { '╬', '╦', '╩', '╣', '═', '╗', '╝' };
            char[] right = { '╬', '╦', '╩', '╠', '═', '╚', '╔' };


            for (int i = 0; i < mapInput.GetLength(0); i++)
            {
                for (int j = 0; j < mapInput.GetLength(1); j++)
                {
                    if (i == 0 && up.Contains(mapInput[i, j]))
                    {
                        entries.Add(new int[] { i, j });
                    }
                    else if (i == mapInput.GetLength(0) - 1 && down.Contains(mapInput[i, j]))
                    {
                        entries.Add(new int[] { i, j });
                    }

                    else if (j == 0 && left.Contains(mapInput[i, j]))
                    {
                        entries.Add(new int[] { i, j });
                    }
                    else if (j == mapInput.GetLength(1) - 1 && right.Contains(mapInput[i, j]))
                    {
                        entries.Add(new int[] { i, j });
                    }
                }
            }

            return entries;
        }

        //Bejáratsorsoló - Random entry
        public int[] RandomEntry(List<int[]> allentry) { return allentry[rnd.Next(allentry.Count())]; }


        /// <summary>
        /// Megadja, hogy hány termet tartamaz a térkép
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Termek száma</returns>
        public static int GetRoomNumber(char[,] map)
        {
            int roomNumber = 0;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] == '█') roomNumber++;
                }
            }
            return roomNumber;
        }
        /// <summary>
        /// A kapott térkép széleit végignézve megállapítja, hogy hány kijárat van.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>Az alkalmas kijáratok száma</returns>
        public static int GetSuitableEntrance(char[,] map)
        {
            return MazeMap.AllEntry(map).Count;
        }
        /// <summary>
        /// Megnézi, hogy van-e a térképen meg nem engedett karakter?
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>true - A térkép tartalmaz szabálytalan karaktert, false - nincs benne ilyen</returns>
        public static bool IsInvalidElement(char[,] map)
        {
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (!("╬═╦╩║╣╠╗╝╚╔.█".Contains(map[i, j]))) return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Visszaadja azoknak a járatkaraktereknek a pozícióját, amelyekhez egyetlen szomszéd pozícióból sem lehet eljutni.
        /// </summary>
        /// <param name="map">Labirintus mátrixa</param>
        /// <returns>A pozíciók "sor_index:oszlop_index" formátumban szerepelnek a lista elemeiként
        public static List<string> GetUnavailableElements(char[,] map)
        {
            List<string> unavailables = new List<string>();

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i, j] != '.' && Character.WhereToMove(map, j, i).Count == 0) unavailables.Add($"{i}:{j}");
                }
            }
            // pld: string poz = "4:12"; 
            return unavailables;
        }
        /// <summary>
        /// Labiritust generál a kapott pozíciókat tartalmazó lista alapján. A lista elemei egymáshoz kapcsolódó járatok pozíciói.
        /// </summary>
        /// <param name="positionsList">"sor_index:oszlop_index" formátumban az egymáshoz kapcsolódó járatok pozícióit tartalmazó lista </param>
        /// <returns>A létrehozott labirintus térképe</returns>
        public static char[,] GenerateLabyrinth(List<string> positionsList)
        {
            Random rnd = new Random();
            // 1.: y, 2.: x
            int[] largestKoords = { 0, 0 };
            for (int i = 0; i < positionsList.Count; i++)
            {
                int y = Convert.ToInt32(positionsList[i].Substring(0, positionsList[i].IndexOf(':')));
                int x = Convert.ToInt32(positionsList[i].Substring(positionsList[i].IndexOf(':') + 1));
                if (largestKoords[0] < y) largestKoords[0] = y;
                if (largestKoords[1] < x) largestKoords[1] = x;
            }

            char[,] generated = new char[largestKoords[0] + 1, largestKoords[1] + 1];
            for (int i = 0; i < positionsList.Count; i++)
            {
                int y = Convert.ToInt32(positionsList[i].Substring(0, positionsList[i].IndexOf(':')));
                int x = Convert.ToInt32(positionsList[i].Substring(positionsList[i].IndexOf(':') + 1));
                generated[y, x] = '.';
            }

            char[] up = { '╬', '╩', '╣', '╠', '║', '╝', '╚' };
            char[] down = { '╬', '╦', '╣', '╠', '║', '╗', '╔' };
            char[] left = { '╬', '╦', '╩', '╣', '═', '╗', '╝' };
            char[] right = { '╬', '╦', '╩', '╠', '═', '╚', '╔' };//'╬', '╦', '╩', '╠', '═', '╚', '╔'

            List<char> directions = new List<char>();
            List<char> allPath = new List<char>();
            allPath.AddRange(up); allPath.AddRange(down); allPath.AddRange(left); allPath.AddRange(right);
            List<char> specPath = new List<char>();



            for (int i = 0; i < generated.GetLength(0); i++)
            {
                for (int j = 0; j < generated.GetLength(1); j++)
                {
                    if (generated[i, j] == '.')
                    {
                        if (i == 0 || generated[i - 1, j] == '.' || down.Contains(generated[i - 1, j])) directions.Add('w');
                        if (i == generated.GetLength(0) - 1 || generated[i + 1, j] == '.' || up.Contains(generated[i + 1, j])) directions.Add('s');
                        if (j == 0 || generated[i, j - 1] == '.' || right.Contains(generated[i, j - 1])) directions.Add('a');
                        if (j == generated.GetLength(1) - 1 || generated[i, j + 1] == '.' || left.Contains(generated[i, j + 1])) directions.Add('d');

                        foreach (char item in allPath) if (!specPath.Contains(item)) specPath.Add(item);

                        if (!directions.Contains('w')) for (int k = 0; k < up.Length; k++) if (specPath.Contains(up[k])) specPath.Remove(up[k]);
                        if (!directions.Contains('s')) for (int k = 0; k < down.Length; k++) if (specPath.Contains(down[k])) specPath.Remove(down[k]);
                        if (!directions.Contains('a')) for (int k = 0; k < left.Length; k++) if (specPath.Contains(left[k])) specPath.Remove(left[k]);
                        if (!directions.Contains('d')) for (int k = 0; k < right.Length; k++) if (specPath.Contains(right[k])) specPath.Remove(right[k]);

                        if (specPath.Count > 0) generated[i, j] = specPath[0];
                        else generated[i, j] = allPath[rnd.Next(allPath.Count)];

                    }
                    else generated[i, j] = ' ';

                    specPath.Clear(); directions.Clear();
                }
            }

            return generated;

        }

    }

    // Karakter osztály - Character class -----------------------------------------------------------------------------------------------------------------------------------------------------
    public class Character
    {
        //Koordináták - Koords
        public int x = -1;
        public int y = -1;
        public List<int[]> roomsExplored = new List<int[]>();
        public int stepCount = 0;

        public ConsoleColor charColor;
        public string charName;

        //lang0, mapName, saveBool, mapColor0, difficulty0, charName, charColor0
        public Character(string[] data)
        {
            string[] colors = { "Red", "Blue", "DarkYellow", "Magenta", "Green" };

            charName = data[5];
            charColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), colors[Convert.ToInt32(data[6]) - 13]);
            x = Convert.ToInt32(data[7]);
            y = Convert.ToInt32(data[8]);
            stepCount = Convert.ToInt32(data[9]);

            if (data[10].Length > 0)
            {
                string[] koords = data[10].Split(';');
                for (int i = 0; i < koords.Length; i++)
                {
                    roomsExplored.Add(new int[] { Convert.ToInt32(koords[i].Split('-')[0]), Convert.ToInt32(koords[i].Split('-')[1]) });
                }
            }
        }

        public static List<char> WhereToMove(char[,] mapInput, int kx, int ky)
        {
            List<char> directions = new List<char>();

            char[] up = { '╬', '╩', '╣', '╠', '║', '╝', '╚', '█' };
            char[] down = { '╬', '╦', '╣', '╠', '║', '╗', '╔', '█' };
            char[] left = { '╬', '╦', '╩', '╣', '═', '╗', '╝', '█' };
            char[] right = { '╬', '╦', '╩', '╠', '═', '╚', '╔', '█' };

            if (up.Contains(mapInput[ky, kx]))
            {
                if ((ky != 0 && !".╩═╝╚".Contains(mapInput[ky - 1, kx])) || ky == 0) directions.Add('w');
            }
            if (down.Contains(mapInput[ky, kx]))
            {
                if ((ky != mapInput.GetLength(0) - 1 && !".═╦╗╔".Contains(mapInput[ky + 1, kx])) || ky == mapInput.GetLength(0) - 1) directions.Add('s');
            }
            if (left.Contains(mapInput[ky, kx]))
            {
                if ((kx != 0 && !".║╣╗╝".Contains(mapInput[ky, kx - 1])) || kx == 0) directions.Add('a');
            }
            if (right.Contains(mapInput[ky, kx]))
            {
                if ((kx != mapInput.GetLength(1) - 1 && !".║╠╚╔".Contains(mapInput[ky, kx + 1])) || kx == mapInput.GetLength(1) - 1) directions.Add('d');
            }
            return directions;
        }
        public void Move(MazeMap maze, ConsoleKey key)
        {
            Console.SetCursorPosition(x + 5, y + (maze.easymode ? 8 : 6));
            Console.BackgroundColor = maze.bgColor;
            Console.Write(maze.map[y, x]);

            if (key == ConsoleKey.W && y > 0 && WhereToMove(maze.map, x, y).Contains('w') && maze.map[y - 1, x] != '.')
            {
                Console.SetCursorPosition(x + 5, --y + (maze.easymode ? 8 : 6));
                stepCount++;
            }
            else if (key == ConsoleKey.S && y < maze.map.GetLength(0) - 1 && WhereToMove(maze.map, x, y).Contains('s') && maze.map[y + 1, x] != '.')
            {
                Console.SetCursorPosition(x + 5, ++y + (maze.easymode ? 8 : 6));
                stepCount++;
            }
            else if (key == ConsoleKey.A && x > 0 && WhereToMove(maze.map, x, y).Contains('a') && maze.map[y, x - 1] != '.')
            {
                Console.SetCursorPosition(--x + 5, y + (maze.easymode ? 8 : 6));
                stepCount++;
            }
            else if (key == ConsoleKey.D && x < maze.map.GetLength(1) - 1 && WhereToMove(maze.map, x, y).Contains('d') && maze.map[y, x + 1] != '.')
            {
                Console.SetCursorPosition(++x + 5, y + (maze.easymode ? 8 : 6));
                stepCount++;
            }
            else if (key == ConsoleKey.Q)
            {
                Other.SaveGameFile(maze, this);
            }
            else
            {
                Console.BackgroundColor = charColor;
                Console.SetCursorPosition(x + 5, y + (maze.easymode ? 8 : 6));
                Console.Write(maze.map[y, x]);
                Console.SetCursorPosition(x + 5, y + (maze.easymode ? 8 : 6));
            }

            RoomExplored(maze);

            Console.SetCursorPosition(x + 5, y + (maze.easymode ? 8 : 6));
            Console.BackgroundColor = charColor;
            Console.Write(maze.map[y, x]);
            Console.SetCursorPosition(x + 5, y + (maze.easymode ? 8 : 6));
        }

        public void RoomExplored(MazeMap maze)
        {
            bool contains = false;
            if (maze.map[y, x] == '█')
            {
                for (int i = 0; i < roomsExplored.Count; i++) if (roomsExplored[i][0] == x && roomsExplored[i][1] == y) contains = true;
                if (!contains) roomsExplored.Add(new int[] { x, y });
            }
        }

        public bool GotOut(MazeMap maze)
        {
            if (roomsExplored.Count == MazeMap.GetRoomNumber(maze.map))
            {
                for (int i = 0; i < MazeMap.AllEntry(maze.map).Count; i++)
                {
                    if (MazeMap.AllEntry(maze.map)[i][0] == y && MazeMap.AllEntry(maze.map)[i][1] == x)
                    {
                        Other.ConsoleCSS();
                        Console.SetCursorPosition(5, 15);
                        Console.Write(maze.words[33]);
                        ConsoleKey answer = Console.ReadKey(true).Key;
                        if (answer == ConsoleKey.I && maze.lang == "hu" ||
                            answer == ConsoleKey.Y && maze.lang == "en" ||
                            answer == ConsoleKey.J && maze.lang == "de") 
                        {
                            Other.ConsoleCSS();
                            Console.Clear();
                            return true;
                        } 
                        else if (answer == ConsoleKey.N) return false;
                        else Move(maze, answer);
                    }
                }
                Console.SetCursorPosition(5, 15);
                Console.BackgroundColor = ConsoleColor.Black;
                for (int i = 0; i < 60; i++) Console.Write(" ");
                Console.BackgroundColor = charColor;
                return false;

            }
            else return false;

        }

    }

    public class Other
    {
        public bool end = false;
        public static void TypeWrite(
            string text = "Testificate",
            int x = 0,
            int y = 0,
            bool delete = true,
            int speed = 5
            )
        {
            const int maxWait = 500;
            const char cursor = '█';

            //Reset
            Console.SetCursorPosition(x, y);
            ConsoleCSS(back: ConsoleColor.Black);
            for (int i = 0; i < text.Length + 15; i++) Console.Write(' ');

            //Write
            Console.SetCursorPosition(x, y);
            ConsoleCSS(back: ConsoleColor.Black);

            for (int i = 0; i < text.Length + 1; i++)
            {
                Console.SetCursorPosition(x, y);
                ConsoleCSS();
                Console.Write(text.Substring(0, i) + cursor);
                Thread.Sleep(maxWait / speed);
            }
            if (delete)
            {
                for (int i = 0; i < 4; i++)
                {
                    Console.SetCursorPosition(x + text.Length, y);
                    ConsoleCSS();
                    if (i % 2 == 0) Console.Write(' ');
                    else Console.Write(cursor);
                    Thread.Sleep(maxWait / (speed / 3 * 2));
                }
                for (int i = text.Length - 1; i > -1; i--)
                {
                    Console.SetCursorPosition(x, y);
                    ConsoleCSS();
                    Console.Write(text.Substring(0, i) + cursor + " ");
                    Thread.Sleep(maxWait / speed);
                }
                Console.SetCursorPosition(x, y);
                ConsoleCSS();
                Console.Write(' ');
            }
            else
            {
                Console.SetCursorPosition(x, y);
                ConsoleCSS();
                Console.Write(text + ' ');
                Console.SetCursorPosition(x, y);
            }
        }

        public static int Options(
            string[] options,
            int x = 3,
            int y = 2,
            int space = 3,
            ConsoleColor defBgColor = ConsoleColor.Black,
            ConsoleColor txtColor = ConsoleColor.Green
            )
        {
            int chosen = 0;
            ConsoleKey pressed = ConsoleKey.R;
            int startIndex = x;

            OptionsPrint(options, x, y, space, false, defBgColor, txtColor);

            //Choosing itself
            do
            {
                for (int i = 0; i < chosen; i++) startIndex += options[i].Length + space;

                ConsoleCSS(defBgColor, txtColor);
                Console.SetCursorPosition(startIndex, y);
                Console.Write(options[chosen]);

                pressed = Console.ReadKey(true).Key;
                if (pressed == ConsoleKey.A || pressed == ConsoleKey.LeftArrow)
                {
                    if (chosen == 0) chosen = options.Length - 1;
                    else chosen--;
                }
                else if (pressed == ConsoleKey.D || pressed == ConsoleKey.RightArrow)
                {
                    if (chosen == options.Length - 1) chosen = 0;
                    else chosen++;
                }
                else if (pressed == ConsoleKey.Q || pressed == ConsoleKey.Escape)
                {
                    return -1;
                }

                OptionsPrint(options, x, y, space, false, defBgColor, txtColor);

                startIndex = x;

            } while (pressed != ConsoleKey.Enter);

            OptionsPrint(options, x, y, space, true, defBgColor, txtColor);

            return chosen;
        }

        public static void OptionsPrint(
            string[] options,
            int x = 3,
            int y = 2,
            int space = 3,
            bool delete = false,
            ConsoleColor defBgColor = ConsoleColor.Black,
            ConsoleColor txtColor = ConsoleColor.Green
            )
        {
            int index = x;

            for (int i = 0; i < options.Length; i++)
            {
                for (int j = 0; j < options[i].Length + space; j++)
                {
                    ConsoleCSS(txtColor, defBgColor);
                    Console.SetCursorPosition(index, y);
                    if (j < options[i].Length && !delete) Console.Write(options[i][j]);
                    else Console.Write(' ');
                    index++;
                }
            }

            //Just in case
            ConsoleCSS(txtColor, defBgColor);
            Console.SetCursorPosition(index, y);
            Console.Write("                      ");
        }

        public static void ConsoleCSS(
            ConsoleColor fore = ConsoleColor.White,
            ConsoleColor back = ConsoleColor.Black,
            bool curVis = false
            )
        {
            Console.CursorVisible = curVis;
            Console.BackgroundColor = back;
            Console.ForegroundColor = fore;
        }

        public static void PrintCompleteData(string[] keyPair, int y = 0, ConsoleColor bgColor = ConsoleColor.Magenta)
        {
            ConsoleCSS(back: bgColor);
            Console.SetCursorPosition(0, y);
            Console.Write($"■ {keyPair[0]}: {keyPair[1]}");
            ConsoleCSS();
        }

        public static void PrintTheEnd(ConsoleColor foreColor = ConsoleColor.DarkGreen)
        {
            string[] end = File.ReadAllLines("theend.txt");
            ConsoleCSS(foreColor);
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < end.Length; i++)
            {
                Console.WriteLine(end[i]);
            }
        }

        public static int OptionChoose(string[] toWrite, string[] optionList, 
                                       int x = 0, int y = 0, int space = 3, 
                                       int speed = 5, ConsoleColor bgColor = ConsoleColor.Black, 
                                       ConsoleColor txtColor = ConsoleColor.Green)
        {
            Other.ConsoleCSS(back: bgColor);

            int chosen = -2;
            int index = 0;
            Thread thread2 = new Thread(() => chosen = Other.Options(optionList, 3, y + 2, space, bgColor, txtColor));
            thread2.Start();
            while (chosen > optionList.Length || chosen < -1)
            {
                Other.TypeWrite(toWrite[index], x, y, true, speed);
                index = (index == toWrite.Length - 1) ? 0 : index + 1;

            }
            return chosen;
        }

        public static void SaveGameFile(MazeMap maze, Character character)
        {
            ConsoleCSS();
            Console.SetCursorPosition(5, 20);
            
            string warning = "";
            if (maze.lang == "hu") warning = "Felül szeretnéd írni a mentésed? (I/N) :";
            else if (maze.lang == "en") warning = "Do you want to overwrite your save? (Y/N) :";
            else if (maze.lang == "de") warning = "Möchten Sie Ihren Speicherstand überschreiben? (J/N) :";
            TypeWrite(warning, 5, 20, false, 9);

            ConsoleKey answer = Console.ReadKey(intercept: true).Key;
            Console.SetCursorPosition(5, 20);
            for (int i = 0; i < warning.Length; i++) Console.Write(' ');
            if ((maze.lang == "hu" && answer == ConsoleKey.I) || (maze.lang == "en" && answer == ConsoleKey.Y) || (maze.lang == "de" && answer == ConsoleKey.J))
            {
                string[] colors = { "Red", "Blue", "Yellow", "Magenta", "Green" };
                int[] colorNums = new int[2];
                
                for (int i = 0; i < colors.Length; i++)
                {
                    if (maze.bgColor.ToString() == colors[i]) colorNums[0] = i;
                    if (character.charColor.ToString() == colors[i]) colorNums[1] = i;
                }
                string roomsExplored = "";
                for (int i = 0; i < character.roomsExplored.Count; i++)
                {
                    roomsExplored += $"{character.roomsExplored[i][0]}-{character.roomsExplored[i][1]}";
                    if (i + 1 < character.roomsExplored.Count) roomsExplored += ";";
                }

                string[] data = {
                        $"mapColor:6:{colorNums[0] + 13}",
                        $"difficulty:7:{((maze.easymode) ? 8 : 9)}",
                        $"charName:11:{character.charName}",
                        $"charColor:12:{colorNums[1] + 13}",
                        $"x:{character.x}",
                        $"y:{character.y}",
                        $"stepCount:22:{character.stepCount}",
                        $"roomsExplored:23:{roomsExplored}"
                };
                File.WriteAllLines(".\\saves\\" + maze.path + ".sav", data);
            }

        }

    }
}
