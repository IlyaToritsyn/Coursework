using System;
using System.IO;
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
                throw new Exception("\nОшибка: матрица не квадратная.");
            }

            //Если длина строки отличается от длины 1 строки.
            else if (str.Length != array.GetLength(1))
            {
                throw new Exception("\nОшибка: количество элементов в строках не совпадает.");
            }

            //Обработка элеметов строки и занесение их в массив с графами.
            for (int j = 0; j < str.Length; j++)
            {
                current = Convert.ToInt32(str[j]);

                if (current < 0)
                {
                    throw new Exception("\nОшибка: отрицательное число.");
                }

                if (j == i)
                {
                    if (current != 0)
                    {
                        throw new Exception("\nОшибка: элемент главной диагонали не равен 0.");
                    }
                }

                else
                {
                    if (current == 0)
                    {
                        throw new Exception("\nОшибка: элемент вне главной диагонали равен 0.");
                    }

                    if (j < i)
                    {
                        if (current != array[j, i])
                        {
                            throw new Exception("\nОшибка: матрица несимметрична.");
                        }
                    }
                }

                array[i, j] = current;
            }
        }
    }

    class Program
    {
        static void Main()
        {
            int[,] edgeMatrix = null;

            try
            {
                using (StreamReader reader = new StreamReader(@"Matrix.txt"))
                {
                    string str; //Текущая строка файла.
                    string[] temp; //Временный массив для элементов текущей строки.
                    int i = 0; //Счётчик считанных строк.

                    Console.WriteLine("Считываю граф...\n");

                    while ((str = reader.ReadLine()) != null)
                    {
                        temp = str.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);

                        //Если строка пустая, то пропускаем её.
                        if (temp.Length == 0)
                        {
                            continue;
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
                        throw new Exception("\nОшибка: матрица не квадратная.");
                    }
                }

                Console.WriteLine("\nГраф успешно считан.\n");
                Console.WriteLine("Ищу оптимальное решение задачи коммивояжёра...\n");

                int[] answer = SimulatedAnnealingClass.SimulateAnnealing(edgeMatrix, 10, 0.00001, out int resultEnergy);

                Console.WriteLine("Более-менее оптимальное решение:");

                MyClass.Output(answer);

                Console.WriteLine("\nРасстояние: " + resultEnergy + ".");
            }

            catch (FormatException)
            {
                Console.WriteLine("\nОшибка: неверный формат данных.");
            }

            catch (OverflowException)
            {
                Console.WriteLine("\nОшибка: отрицательное число.");
            }

            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("\nОшибка: матрица не квадратная.");
            }

            catch (NullReferenceException)
            {
                Console.WriteLine("\nОшибка: файл с графами пуст.");
            }

            catch (Exception e)
            {
                Console.WriteLine("\n" + e.Message);
            }
        }
    }
}
