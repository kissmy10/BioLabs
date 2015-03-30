using System;

namespace lab5
{
    class Program
    {
        const string Input = "ACTACGTTGAGTGAGATCGTTGACCGGTCACCAACAGCGCCCGCACGGCACGGTCCGGTGTATGCAAGAGAAAAGCGCCG";
        private const int StatesCount = 10;


        public static double[,] A = new double[StatesCount, StatesCount];
        public static double[,] V;
        public static double[,] e;

        static void Main(string[] args)
        {
            V = new double[StatesCount, Input.Length];
            e = new double[StatesCount, Input.Length];

            InitializeA();

            BaumWelshAlgorithm();

            var PI = ViterbiAlgorithm();

            Console.ReadKey();
        }

        // Алгоритм Баум­Велша
        private static void BaumWelshAlgorithm()
        {
            
        }

        // Алгоритм Витерби
        private static int[] ViterbiAlgorithm()
        {
            var answer = new int[Input.Length];
            answer[0] = 0;

            var indexes = new int[StatesCount, Input.Length];

            // Инициализация
            V[0, 0] = 1;

            for (int k = 1; k < StatesCount; k++)
                V[k, 0] = 0;

            // Рекурсия
            for (int i = Input.Length; i > 0; i--)
            {
                for (int k = 0; k < StatesCount; k++)
                {
                    // Поиск max j
                    double maxj = 0;
                    for (int j = 0; j < StatesCount; j++)
                    {
                        var value = A[j, k] * V[k, i - 1];
                        if (maxj < value)
                        {
                            maxj = value;       // ищем max j
                            indexes[k, i] = j;  // сохраняем arg max j
                        }
                    }

                    // Сама рекурсивна формула вычисления
                    V[k, i] = e[k, i] * maxj;
                }
            }

            // Пробегаемся обратно по стобцам, собирая индексы наиболее вероятных состояний
            for (int i = Input.Length; i > 2; i--)
                answer[i - 1] = indexes[answer[i], i];

            return answer;
        }

        private static void InitializeA()
        {
            for (int i = 0; i < StatesCount; i++)
                for (int j = 0; j < StatesCount; j++)
                    A[i, j] = 1/StatesCount;
        }
    }
}
