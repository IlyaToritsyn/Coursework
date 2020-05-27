using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using ClassLibrary;

namespace Coursework
{
    public class MyClass
    {

        /// <summary>
        /// Вывод элементов массива в консоль.
        /// </summary>
        /// <param name="array">Наш массив.</param>
        public static void Output<T> (T[] array)
        {
            foreach (T i in array)
            {
                Console.Write(i + "\t");
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Обработка данных из файла с графами и занесение их в массив.
        /// </summary>
        /// <param name="array">Массив, куда складываем обработанные значения.</param>
        /// <param name="str">Считанная строка с данными из файла.</param>
        /// <param name="i">Порядковый номер строки, начиная с 0.</param>
        public static void ReadGraphs(int[,] array, string[] str, int i)
        {
            int current; //Обрабатываемое число.

            //Если есть лишняя строка.
            if (i == array.GetLength(1))
            {
                Console.WriteLine();

                throw new Exception("Ошибка: матрица не квадратная.");
            }

            //Если длина строки отличается от длины 1 строки.
            else if (str.Length != array.GetLength(1))
            {
                Console.WriteLine();

                throw new Exception("Ошибка: количество элементов в строках не совпадает.");
            }

            //Обработка элеметов строки и занесение их в массив с графами.
            for (int j = 0; j < str.Length; j++)
            {
                current = Convert.ToInt32(str[j]);

                if (current < 0)
                {
                    Console.WriteLine();

                    throw new Exception("Ошибка: отрицательное число.");
                }

                if (j == i)
                {
                    if (current != 0)
                    {
                        Console.WriteLine();

                        throw new Exception("Ошибка: элемент главной диагонали не равен 0.");
                    }
                }

                else
                {
                    if (current == 0)
                    {
                        Console.WriteLine();

                        throw new Exception("Ошибка: элемент вне главной диагонали равен 0.");
                    }

                    if (j < i)
                    {
                        if (current != array[j, i])
                        {
                            Console.WriteLine();

                            throw new Exception("Ошибка: матрица несимметрична.");
                        }
                    }
                }

                array[i, j] = current;
            }
        }

        public static void GetTxtFiles(string directoryPath, out List<string> txtFiles)
        {
            txtFiles = new List<string>();

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string[] files = Directory.GetFiles(directoryPath);

            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo fileInfo = new FileInfo(files[i]);
                    if (fileInfo.Extension == ".txt")
                    {
                        txtFiles.Add(fileInfo.Name);
                    }
                }
            }

            if (txtFiles.Count == 0)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

                throw new Exception("В папке с матрицами смежности, находящейся по пути:\n\t" + directoryInfo.FullName + " -\nнет ни одного файла с расширением .txt.");
            }
        }

        /// <summary>
        /// Ввод целого числа с клавиатуры.
        /// </summary>
        /// <param name="message">Сообщение для ввода</param>
        /// <returns>Целое число, введённое с клавиатуры</returns>
        public static int InputInteger(string message)
        {
            bool isParsed = false;

            int N = 0;

            while (!isParsed)
            {
                Console.WriteLine(message);

                isParsed = int.TryParse(Console.ReadLine(), out N);

                Console.WriteLine();
            }

            return N;
        }
    }

    class Program
    {
        static void Main()
        {
            while (true)
            {
                int[,] edgeMatrix = null;

                string directoryPath = @".\Texts\";
                string selectedFile;

                int fileNumber;

                try
                {
                    MyClass.GetTxtFiles(directoryPath, out List<string> txtFiles);

                    Console.WriteLine("Список доступных файлов:");

                    foreach (string file in txtFiles)
                    {
                        Console.WriteLine((txtFiles.IndexOf(file) + 1) + ". " + file);
                    }

                    Console.WriteLine();

                    fileNumber = MyClass.InputInteger("Введите номер нужного файла или 0, чтобы обновить список:");

                    if (fileNumber == 0)
                    {
                        continue;
                    }

                    try
                    {
                        if ((fileNumber < 1) || (fileNumber > txtFiles.Count))
                        {
                            throw new Exception("Введён неверный номер файла: " + fileNumber + ".");
                        }
                    }

                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + "\n");

                        continue;
                    }

                    selectedFile = txtFiles[fileNumber - 1];

                    Console.WriteLine("Выбран файл: " + selectedFile + "\n");

                    using (StreamReader reader = new StreamReader(directoryPath + selectedFile))
                    {
                        string str; //Текущая строка файла.
                        string[] temp; //Временный массив для элементов текущей строки.
                        int i = 0; //Счётчик считанных строк.

                        Console.WriteLine("Считываю граф...\n");

                        while ((str = reader.ReadLine()) != null)
                        {
                            temp = str.Split(new char[] { ' ', ',', ';', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                            //Если строка пустая, то пропускаем её.
                            if (temp.Length == 0)
                            {
                                continue;
                            }

                            else if ((temp.Length == 1) && (i == 0))
                            {
                                    throw new Exception("Ошибка: в матрице смежности " + selectedFile + " только 1 значение.");
                            }

                            MyClass.Output(temp);

                            //Создаём двумерный массив для графов, основываясь на длине 1 строки.
                            if (edgeMatrix == null)
                            {
                                edgeMatrix = new int[temp.Length, temp.Length];
                            }

                            MyClass.ReadGraphs(edgeMatrix, temp, i); //Обрабатываем строку с данными, и если всё хорошо, то заносим их в массив.

                            i++; //Ещё одна строка обработана.
                        }

                        if (i != edgeMatrix.GetLength(0))
                        {
                            throw new Exception("Ошибка: матрица не квадратная.\n");
                        }
                    }

                    Console.WriteLine();
                    Console.WriteLine("Граф успешно считан.\n");
                    Console.WriteLine("Ищу оптимальное решение задачи коммивояжёра...\n");

                    int[] answer = SimulatedAnnealingClass.SimulateAnnealing(edgeMatrix, 10, 0.00001, out int resultEnergy);

                    Console.WriteLine("Более-менее оптимальное решение:");

                    MyClass.Output(answer);

                    Console.WriteLine();
                    Console.WriteLine("Расстояние: " + resultEnergy + ".\n");
                }

                catch (FormatException)
                {
                    Console.WriteLine("Ошибка: неверный формат данных.\n");
                }

                catch (OverflowException)
                {
                    Console.WriteLine("Ошибка: отрицательное число.\n");
                }

                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Ошибка: матрица не квадратная.\n");
                }

                catch (NullReferenceException)
                {
                    Console.WriteLine("Ошибка: файл с графами пуст.\n");
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "\n");
                }

                Console.WriteLine("Нажмите любую клавишу, чтобы запустить программу заново.\n");

                Console.ReadKey(true);
            }
        }
    }
}
