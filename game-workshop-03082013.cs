using System;
using System.Threading;

namespace JustJewels
{
    class JustJewels
    {
        public static bool selectionExist = false;
        public static int[] lastSelection = { -1, -1 };
        public static int cursorX = 0;
        public static int cursorY = 0;
        public static int score = 0;
        public static ConsoleColor[] colors = { ConsoleColor.Yellow, ConsoleColor.Blue, ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Magenta };
        public static Random randColor = new Random();
        public static bool[,] boxesToRemove = new bool[8, 8];
        public static Box[,] playField = InitPlayField();

        static void Main()
        {
            //Console.Beep(223, 35);
            // We can implement method that can turn music on and off
            // IMPLEMENT MENU
            // IMPLEMENT SCORE SCREEN WITH TEXT FILE!!

            Settings();
            ScoreField();
            FallDownAndGenerateNewJewels();
            Engine();
        }

        private static void Engine()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyPressed = Console.ReadKey(true);
                    if (keyPressed.Key == ConsoleKey.LeftArrow)
                    {
                        if (cursorX > 0)
                        {
                            cursorX--;
                        }
                        if (selectionExist)
                        {
                            Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            // Ако след разменянето - нямаме комбинация - връщаме оригиналните позиции.
                            // TODO: Може да се вкара анимация, понеже в момента изглежда все едно, че нищо не се случва.
                            if (isEmpty(FindBoxesForRemove(playField)))
                            {
                                Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            }
                            // Ако имаме комбинация - пускаме метода за чистене
                            else
                            {
                                playField[cursorX, cursorY].isCursorPosition = false;
                                playField[cursorX, cursorY].DrawBox();
                                FallDownAndGenerateNewJewels();
                            }
                        }
                    }
                    if (keyPressed.Key == ConsoleKey.RightArrow)
                    {
                        if (cursorX < 7)
                        {
                            cursorX++;
                        }
                        if (selectionExist)
                        {
                            Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            // Ако след разменянето - нямаме комбинация - връщаме оригиналните позиции.
                            // TODO: Може да се вкара анимация, понеже в момента изглежда все едно, че нищо не се случва.
                            if (isEmpty(FindBoxesForRemove(playField)))
                            {
                                Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            }
                            // Ако имаме комбинация - пускаме метода за чистене
                            else
                            {
                                playField[cursorX, cursorY].isCursorPosition = false;
                                playField[cursorX, cursorY].DrawBox();
                                FallDownAndGenerateNewJewels();
                            }
                        }
                    }
                    if (keyPressed.Key == ConsoleKey.UpArrow)
                    {
                        if (cursorY > 0)
                        {
                            cursorY--;
                        }
                        if (selectionExist)
                        {
                            Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            // Ако след разменянето - нямаме комбинация - връщаме оригиналните позиции.
                            // TODO: Може да се вкара анимация, понеже в момента изглежда все едно, че нищо не се случва.
                            if (isEmpty(FindBoxesForRemove(playField)))
                            {
                                Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            }
                            // Ако имаме комбинация - пускаме метода за чистене
                            else
                            {
                                playField[cursorX, cursorY].isCursorPosition = false;
                                playField[cursorX, cursorY].DrawBox();
                                FallDownAndGenerateNewJewels();
                            }
                        }
                    }
                    if (keyPressed.Key == ConsoleKey.DownArrow)
                    {
                        if (cursorY < 7)
                        {
                            cursorY++;
                        }
                        if (selectionExist)
                        {
                            Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            // Ако след разменянето - нямаме комбинация - връщаме оригиналните позиции.
                            // TODO: Може да се вкара анимация, понеже в момента изглежда все едно, че нищо не се случва.
                            if (isEmpty(FindBoxesForRemove(playField)))
                            {
                                Swap(playField[lastSelection[0], lastSelection[1]], playField[cursorX, cursorY], playField);
                            }
                            // Ако имаме комбинация - пускаме метода за чистене
                            else
                            {
                                playField[cursorX, cursorY].isCursorPosition = false;
                                playField[cursorX, cursorY].DrawBox();
                                FallDownAndGenerateNewJewels();
                            }
                        }
                    }
                    if (keyPressed.Key == ConsoleKey.Spacebar)
                    {
                        playField[cursorX, cursorY].isSelected = true;
                        playField[cursorX, cursorY].DrawBox();
                        selectionExist = true;
                        lastSelection[0] = cursorX;
                        lastSelection[1] = cursorY;
                    }
                    else
                    {
                        for (int i = 0; i < playField.GetLength(0); i++)
                        {
                            for (int j = 0; j < playField.GetLength(1); j++)
                            {
                                if (playField[i, j].isCursorPosition)
                                {
                                    playField[i, j].isCursorPosition = false;
                                }
                                playField[i, j].DrawBox();
                            }
                        }
                        playField[cursorX, cursorY].isCursorPosition = true;
                        playField[cursorX, cursorY].DrawBox();
                    }
                    // ScoreField(); - Here we can update the score field
                }
            }
        }

        private static void FallDownAndGenerateNewJewels()
        {
            do
            {
                boxesToRemove = FindBoxesForRemove(playField);
                //TestMatrix(boxesToRemove);
                // Ако има нещо в bool матрицата - броим елементите, които се намират вътре и
                // добавяме по 10 точки за всеки jewel
                // Може да се имплементира бонус система която да удвоява точките ако имаме 6 9 13 и т.н jewelчета.
                int jewelCount = 0;
                int bonus = 1;
                for (int y = 0; y < boxesToRemove.GetLength(0); y++)
                {
                    for (int x = 0; x < boxesToRemove.GetLength(1); x++)
                    {
                        if (boxesToRemove[x, y])
                        {
                            jewelCount += 10;
                        }
                    }
                }
                bonus = jewelCount / 30; // Тука може да гръмне ако bonus стане 0-ла
                DisplayCombo(bonus);
                score += jewelCount * bonus;
                ScoreField();

                DestroyJewels(playField, boxesToRemove);
                while (!isFull(playField))
                {
                    for (int y = playField.GetLength(0) - 2; y >= 0; y--) // very Important to be GetLength(0) - 2 becouse we dont want to check the last ROW!
                    {
                        for (int x = playField.GetLength(1) - 1; x >= 0; x--)
                        {
                            // Ако текущия Jewel не е черен и Jewel-a под него е черен - правим SWAP
                            if (playField[x, y].color != ConsoleColor.Black && playField[x, y + 1].color == ConsoleColor.Black)
                            {
                                Thread.Sleep(50);
                                lastSelection[0] = x;
                                lastSelection[1] = y;
                                cursorX = x;
                                cursorY = y + 1;
                                Swap(playField[x, y], playField[x, y + 1], playField);
                            }
                            // Ако сме на нулевия ред и квадратчето е черно - го пречертаваме и му даваме Random цвят!
                            if (y == 0 && playField[x, y].color == ConsoleColor.Black)
                            {
                                playField[x, y].color = colors[randColor.Next(0, colors.Length)];
                                Thread.Sleep(30);
                                playField[x, y].InitBox('\u2591'); // Light-Shade
                                playField[x, y].DrawBox();
                                Thread.Sleep(50);
                                playField[x, y].InitBox('\u2592'); // Medium-Shade
                                playField[x, y].DrawBox();
                                Thread.Sleep(50);
                                playField[x, y].InitBox('\u2593'); // Dark-Shade 
                                playField[x, y].DrawBox();
                                Thread.Sleep(50);
                                playField[x, y].InitBox('\u2588'); // Restore FULL BLOCK when BLACK!
                                playField[x, y].DrawBox();
                            }
                        }
                    }
                }
                // Set the bool matrix to False
                Array.Clear(boxesToRemove, 0, boxesToRemove.Length);
            } while (!isEmpty(FindBoxesForRemove(playField)));
            //TestMatrix(boxesToRemove);
        }

        private static void DisplayCombo(int bonus)
        {
            Console.SetCursorPosition(4, 36);
                switch (bonus)
                {
                    case 2: // must be 2
                        Console.Beep(223, 35);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("\u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510\n    |D| |O| |U| |B| |L| |E|\n    \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518\n      \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n      |C| |O| |M| |B| |O|\n      \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518");
                        break;
                    case 3:
                        Console.Beep(223, 35);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510\n    |T| |R| |I| |P| |L| |E|\n    \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518\n      \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n      |C| |O| |M| |B| |O|\n      \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518");
                        break;
                    case 4:
                        Console.Beep(223, 35);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("\u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510\n    |Q| |U| |A| |D| |R| |O|\n    \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518\n      \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n      |C| |O| |M| |B| |O|\n      \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518");
                        break;
                    case 5:
                        Console.Beep(223, 35);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("    \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n        |I| |M| |P| |O|\n        \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518\n      \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n      |S| |I| |B| |L| |E|\n      \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518");
                        break;
                    case 6:
                        Console.Beep(223, 35);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("    \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n        |I| |M| |P| |O|\n        \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518\n      \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n      |S| |I| |B| |L| |E|\n      \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("\u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510\n    |D| |O| |U| |B| |L| |E|\n    \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518\n      \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \u250c-\u2510 \n      |C| |O| |M| |B| |O|\n      \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518 \u2514-\u2518");
                        break;
                }
        }

        private static bool isFull(Box[,] playField)
        {
            for (int y = 0; y < playField.GetLength(0); y++)
            {
                for (int x = 0; x < playField.GetLength(1); x++)
                {
                    if (playField[x, y].color == ConsoleColor.Black)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        //isEmpty(boxesToRemove); // - This method checks if the boxesToRemove bool matrix is empty or not
        private static bool isEmpty(bool[,] boxesToRemove)
        {
            for (int y = 0; y < boxesToRemove.GetLength(0); y++)
            {
                for (int x = 0; x < boxesToRemove.GetLength(1); x++)
                {
                    if (boxesToRemove[x, y])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static void DestroyJewels(Box[,] playField, bool[,] boxesToRemove)
        {
            Thread.Sleep(400); //TODO: Adjust Speed
            for (int y = 0; y < playField.GetLength(0); y++)
            {
                for (int x = 0; x < playField.GetLength(1); x++)
                {
                    if (boxesToRemove[x, y] == true)
                    {
                        playField[x, y].InitBox('\u2593'); // Dark-Shade
                        playField[x, y].DrawBox();
                        Thread.Sleep(50);
                        playField[x, y].InitBox('\u2592'); // Medium-Shade
                        playField[x, y].DrawBox();
                        Thread.Sleep(50);
                        playField[x, y].InitBox('\u2591'); // Light-Shade
                        playField[x, y].DrawBox();
                        Thread.Sleep(50);
                        playField[x, y].color = ConsoleColor.Black;
                        Thread.Sleep(50);
                        playField[x, y].InitBox('\u2588'); // Restore FULL BLOCK when BLACK!
                        playField[x, y].DrawBox();
                    }
                }
            }
        }

        private static void TestMatrix(bool[,] boxesToRemove)
        {
            Console.SetCursorPosition(0, 38);
            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < boxesToRemove.GetLength(0); i++)
            {
                for (int j = 0; j < boxesToRemove.GetLength(1); j++)
                {
                    if (boxesToRemove[j, i] == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("T ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.Write("F ");
                    }
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }

        private static Box[,] InitPlayField()
        {
            Box[,] playField = new Box[8, 8];

            for (int i = 0; i < playField.GetLength(0); i++)
            {
                for (int j = 0; j < playField.GetLength(1); j++)
                {
                    playField[i, j] = new Box(i * 4 + 1, j * 4 + 1, colors[randColor.Next(0, colors.Length)]);
                    playField[i, j].InitBox('\u2588'); // Dark-Shade: \u2593; MediumShade: \u2592; LightShade: \u2591
                    playField[i, j].DrawBox();
                }
            }
            return playField;
        }

        private static void ScoreField()
        {
            Console.SetCursorPosition(0, 33);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("---------------------------------");
            Console.SetCursorPosition(1, 34);
            Console.Write("Score: {0}", score);
            Console.SetCursorPosition(20, 35);
        }

        private static void Settings()
        {
            Console.CursorVisible = false;
            Console.BufferHeight = Console.WindowHeight = 45; // 38 default
            Console.BufferWidth = Console.WindowWidth = 33;
            Console.Title = "Just Jewels";
        }



        static void Swap(Box first, Box second, Box[,] playField)
        {
            int tempX = first.x;
            int tempY = first.y;
            first.x = second.x;
            first.y = second.y;
            second.x = tempX;
            second.y = tempY;

            selectionExist = false;
            first.isSelected = false;
            second.isSelected = false;
            first.DrawBox();
            second.DrawBox();

            Box tempJewel = playField[lastSelection[0], lastSelection[1]];
            playField[lastSelection[0], lastSelection[1]] = playField[cursorX, cursorY];
            playField[cursorX, cursorY] = tempJewel;
        }

        public static bool[,] FindBoxesForRemove(Box[,] playField)
        {
            int currentSeq = 1;
            int bestSeq = int.MinValue;
            int bestSeqX = 0;
            int bestSeqY = 0;
            int bestSeqDirection = 1; // 1 = horizontal; 2 = right
            bool finishFlag = false;
            bool[,] selectedCells = new bool[playField.GetLength(0), playField.GetLength(1)];

            do
            {
                // horizontal sequences - left to right
                for (int x = 0; x < playField.GetLength(0); x++)
                {
                    for (int y = 0; y < playField.GetLength(1) - 1; y++)
                    {
                        // Ако цветовете им съвпадат и не са били селектирани вече - ги добавяме в редицата
                        if (playField[x, y].color == playField[x, y + 1].color && selectedCells[x, y] == false)
                        {
                            currentSeq++;
                        }
                        else
                        {
                            currentSeq = 1;
                        }

                        if (currentSeq > bestSeq)
                        {
                            bestSeq = currentSeq;
                            bestSeqX = x;
                            bestSeqY = y + 1;
                            bestSeqDirection = 1; // 1 = horizontal
                        }
                    }
                    currentSeq = 1;
                }

                // vertical sequences - top to down
                for (int y = 0; y < playField.GetLength(1); y++)
                {
                    for (int x = 0; x < playField.GetLength(0) - 1; x++)
                    {
                        if (playField[x, y].color == playField[x + 1, y].color && selectedCells[x, y] == false)
                        {
                            currentSeq++;
                        }
                        else
                        {
                            currentSeq = 1;
                        }

                        if (currentSeq > bestSeq)
                        {
                            bestSeq = currentSeq;
                            bestSeqY = y;
                            bestSeqX = x + 1;
                            bestSeqDirection = 2; // 2 = down
                        }
                    }
                    currentSeq = 1;
                }

                // Populate the bool matrix for selected cells only when the sequence length is longer than 2
                if (bestSeq >= 3)
                {
                    switch (bestSeqDirection)
                    {
                        case 1: // 1 = horizontal
                            for (int i = bestSeqY; i >= Math.Abs(bestSeq - bestSeqY - 1); i--)
                            {
                                selectedCells[bestSeqX, i] = true;
                            }
                            break;
                        case 2: // 2 = down
                            for (int i = bestSeqX; i >= Math.Abs(bestSeq - bestSeqX - 1); i--)
                            {
                                selectedCells[i, bestSeqY] = true;
                            }
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    finishFlag = true;
                }
                currentSeq = 1;
                bestSeq = int.MinValue;
                bestSeqX = 0;
                bestSeqY = 0;
                bestSeqDirection = 1; // 1 = horizontal; 2 = right
            } while (finishFlag == false);
            return selectedCells;
        }

    }
}

class Box
{
    public Box(int x, int y, ConsoleColor color)
    {
        this.x = x;
        this.y = y;
        this.color = color;
        this.isSelected = false;
        this.isCursorPosition = false;
    }

    public int x;
    public int y;
    public ConsoleColor color;
    public bool isSelected;
    public bool isCursorPosition;
    char[,] symbols = new char[3, 3];

    public void InitBox(char symbol)
    {
        for (int i = 0; i < symbols.GetLength(0); i++)
        {
            for (int j = 0; j < symbols.GetLength(1); j++)
            {
                symbols[i, j] = symbol;
            }
        }
    }


    public void DrawBox()
    {

        Console.ForegroundColor = this.color;
        for (int i = 0; i < this.symbols.GetLength(0); i++)
        {
            for (int j = 0; j < this.symbols.GetLength(1); j++)
            {
                Console.SetCursorPosition(this.x + i, this.y + j);
                Console.Write(this.symbols[i, j]);
            }
        }

        switch (this.isSelected)
        {
            case false: // Not Selected 
                Console.SetCursorPosition(this.x + 1, this.y - 1);
                Console.Write(' ');
                Console.SetCursorPosition(this.x + 3, this.y + 1);
                Console.Write(' ');
                Console.SetCursorPosition(this.x + 3, this.y);
                Console.Write(' ');
                Console.SetCursorPosition(this.x + 3, this.y + 2);
                Console.Write(' ');
                Console.SetCursorPosition(this.x + 1, this.y + 3);
                Console.Write(' ');
                Console.SetCursorPosition(this.x - 1, this.y + 1);
                Console.Write(' ');
                Console.SetCursorPosition(this.x - 1, this.y);
                Console.Write(' ');
                Console.SetCursorPosition(this.x - 1, this.y + 2);
                Console.Write(' ');
                break;
            case true: // isSelected
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(this.x + 3, this.y + 1);
                Console.Write('|');
                Console.SetCursorPosition(this.x + 3, this.y);
                Console.Write('|');
                Console.SetCursorPosition(this.x + 3, this.y + 2);
                Console.Write('|');
                Console.SetCursorPosition(this.x - 1, this.y + 1);
                Console.Write('|');
                Console.SetCursorPosition(this.x - 1, this.y);
                Console.Write('|');
                Console.SetCursorPosition(this.x - 1, this.y + 2);
                Console.Write('|');
                break;
        }

        switch (isCursorPosition)
        {
            case false: // Not Selected
                Console.SetCursorPosition(this.x - 1, this.y - 1);
                Console.Write(' ');
                Console.SetCursorPosition(this.x + 3, this.y - 1);
                Console.Write(' ');
                Console.SetCursorPosition(this.x + 3, this.y + 3);
                Console.Write(' ');
                Console.SetCursorPosition(this.x - 1, this.y + 3);
                Console.Write(' ');
                break;
            case true: // isSelected
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(this.x - 1, this.y - 1);
                Console.Write('\u250c');
                Console.SetCursorPosition(this.x + 3, this.y - 1);
                Console.Write('\u2510');
                Console.SetCursorPosition(this.x + 3, this.y + 3);
                Console.Write('\u2518');
                Console.SetCursorPosition(this.x - 1, this.y + 3);
                Console.Write('\u2514');
                break;
        }
    }
}

// http://patorjk.com/software/taag/#p=display&h=0&v=0&f=Cricket&t=DOUBLE%0ACOMBO
