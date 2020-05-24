using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary
{
    /// <summary>
    /// Класс, реализующий метод имитации отжига.
    /// </summary>
    public class SimulatedAnnealingClass
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Умен.температуры.
        /// </summary>
        /// <param name="initialTemperature">Начальная температура</param>
        /// <param name="i">Номер итерации</param>
        /// <returns>Новая умен. температура</returns>
        public static double DecreaseTemperature(double initialTemperature, int i)
        {
            return initialTemperature / i / 10;
        }

        /// <summary>
        /// Вычисление вероятности перехода в новое состояние.
        /// </summary>
        /// <param name="energyDifference">Разница энергий dE</param>
        /// <param name="temperature">Температура на данной итерации</param>
        /// <returns>Вероятность перехода в новое состояние</returns>
        public static double GetTransitionProbability(double energyDifference, double temperature)
        {
            return Math.Exp(-energyDifference / temperature);
        }

        /// <summary>
        /// Получение решения о переходе в новое состояние.
        /// </summary>
        /// <param name="probability">Вероятность перехода в новое состояние</param>
        /// <returns>true - переходим; false - игнорируем возможность</returns>
        public static bool IsTransition(double probability)
        {
            return random.NextDouble() <= probability;
        }

        /// <summary>
        /// Инвертировать одномерный массив в промежутке [a; b].
        /// </summary>
        /// <param name="array">Обрабатываемый массив</param>
        /// <param name="a">Левая граница инвертирумого промежутка (входит в промежуток)</param>
        /// <param name="b">Правая граница инвертирумого промежутка (входит в промежуток)</param>
        /// <returns>Перечисление элементов инвертированного массива</returns>
        public static IEnumerable<int> Reverse(int[] array, int a, int b)
        {
            for (int i = 0; i < a; i++)
            {
                yield return array[i];
            }

            for (int i = b; i >= a; i--)
            {
                yield return array[i];
            }

            for (int i = b + 1; i < array.Length; i++)
            {
                yield return array[i];
            }
        }

        /// <summary>
        /// Генерация нового состояния-кандидата путём инвертирования
        /// подпоследовательности нынешнего состояния.
        /// Например: 1 2 3 4 5 6 -> 1 5 4 3 2 6
        /// </summary>
        /// <param name="previousState">Пред. состояние</param>
        /// <returns>Новое состояние-кандидат</returns>
        public static int[] GenerateStateCandidate(int[] previousState)
        {
            int previousStateLength = previousState.Length;

            if (previousStateLength < 2)
            {
                throw new Exception("Длина массива (" + previousStateLength + ") меньше 2 - невозможно получить новое состояние-кандидат путём инвертирования.");
            }

            //Получение случайных границ a и b, при условии, что a должно быть меньше b.
            int a = random.Next(0, previousStateLength - 1); //Левая граница текущего состояния для последующего инвертирования.
            int b = random.Next(a + 1, previousStateLength); //Правая граница текущего состояния для последующего инвертирования.

            return Reverse(previousState, a, b).ToArray();
        }

        /// <summary>
        /// Получение последовательности неповторяющихся целых чисел.
        /// </summary>
        /// <param name="array">Заполняемый массив</param>
        public static void GenerateNonrepeatingNumberSequence(int[] array)
        {
            int number; //Новое случайное число-кандидат.
            int j; //Счётчик для проверки занесённых значений.

            //Заполняем массив неповторяющимися целыми числами.
            for (int i = 0; i < array.Length;)
            {
                number = random.Next(1, array.Length + 1);

                //Проверяем: заносилось ли наше случайное число-кандидат ранее.
                for (j = 0; j < i; j++)
                {
                    if (number == array[j])
                    {
                        break;
                    }
                }

                //Если число-кандидат ранее не использовалось, то заносим его в массив.
                if (j == i)
                {
                    array[i] = number;
                    i++;
                }
            }
        }

        /// <summary>
        /// Подсчёт энергии данного состояния.
        /// </summary>
        /// <param name="state">Состояние системы</param>
        /// <param name="energy">Матрица смежности, содержащая значения энергии</param>
        /// <returns>Значение энергии данного состояния</returns>
        public static int CalculateEnergy(int[] state, int[,] energy)
        {
            int stateEnergy = 0;

            for (int i = 0; i < state.Length - 1; i++)
            {
                //На след. строке вычитаем 1, дабы 1 элемент относился к индексу 0, а последний - к последнему индексу.
                //Это позволяет обеспечить соответствие: индексы начинаются с 0, а элементы с 1.
                stateEnergy += energy[state[i] - 1, state[i + 1] - 1];
            }

            //Тоже вычитаем 1 (см. комментарий выше)
            stateEnergy += energy[state[state.Length - 1] - 1, state[0] - 1];

            return stateEnergy;
        }

        private static int GetFactorial(int N)
        {
            int factorial = 1;

            for (int i = 2; i <= N; i++)
            {
                factorial *= i;
            }

            return factorial;
        }

        /// <summary>
        /// Поиск оптимального состояния методом имитации отжига.
        /// </summary>
        /// <param name="energy">Матрица смежности, содержащая значения энергии</param>
        /// <param name="initialTemperature">Начальная температура</param>
        /// <param name="endTemperature">Мин., конечная температура</param>
        /// <param name="currentEnergy">Энергия оптимального состояния</param>
        /// <returns>Оптимальное состояние</returns>
        public static int[] SimulateAnnealing(int[,] energy, double initialTemperature, double endTemperature, out int currentEnergy)
        {
            double currentTemperature = initialTemperature;
            double probability;

            int[] state = new int[energy.GetLength(0)];
            int[] stateCandidate;

            int maxIterationsNumber;
            int candidateEnergy;
            int i = 0;

            GenerateNonrepeatingNumberSequence(state);

            currentEnergy = CalculateEnergy(state, energy);

            switch (state.Length)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    maxIterationsNumber = GetFactorial(state.Length);

                    break;
                case 5:
                case 6:
                    maxIterationsNumber = 100;

                    break;
                default:
                    maxIterationsNumber = GetFactorial(state.Length) / 10 * 9;

                    break;
            }
            
            //Ограничение количества итераций
            for (; i < maxIterationsNumber; i++)
            {
                stateCandidate = GenerateStateCandidate(state);
                candidateEnergy = CalculateEnergy(stateCandidate, energy);

                //Если кандидат обладает меньшей энергией, то он становится текущим состоянием.
                if (candidateEnergy < currentEnergy)
                {
                    currentEnergy = candidateEnergy;
                    state = stateCandidate;
                }

                //Иначе считаем вероятность перехода в состояние-кандидат и определяемся:
                //совершать ли переход.
                else
                {
                    probability = GetTransitionProbability(candidateEnergy - currentEnergy, currentTemperature);

                    if (IsTransition(probability))
                    {
                        currentEnergy = candidateEnergy;
                        state = stateCandidate;
                    }
                }

                currentTemperature = DecreaseTemperature(initialTemperature, i);

                if (currentTemperature <= endTemperature)
                {
                    break;
                }
            }

            Console.WriteLine("Количество итераций: " + i + ".\n"); //Просто чтобы посмотреть количество итераций.

            return state;
        }
    }
}
