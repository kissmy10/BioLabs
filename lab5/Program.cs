using System;
using System.IO;

namespace lab5
{
    class Program
    {
        const string Input = "ACTACGTTGAGTGAGATCGTTGACCGGTCACCAACAGCGCCCGCACGGCACGGTCCGGTGTATGCAAGAGAAAAGCGCCG";
        private const int StatesCount = 2;


        public static double[,] a = new double[N, N]; // Матрица перемещений
        public static double[,] V;
        public static double[,] e;


        public static double[] PI; // вектор вероятностей начальных состояний

        public const int N = 2; // Число состояний
        public const int L = 4; // Наблюдаемая величина принимает одно из L значений
        public const int T = 10; // Время

        static void Main()
        {
            V = new double[StatesCount, Input.Length];
            e = new double[N, L];
            PI = new double[N];

            InitializeAandE();

            var sequence = GetLearningSequence();
            BaumWelshAlgorithm(sequence);

            var Pi = ViterbiAlgorithm();

            for (var i = 0; i < Input.Length; i++)
                switch (Pi[i])
                {
                    case 0: Console.Write("F"); break;
                    case 1: Console.Write("T"); break;
                }

            Console.ReadKey();
        }

        private static string GetLearningSequence()
        {
            var stream = new StreamReader("CpG.fasta");
            return stream.ReadToEnd();
        }

        // Алгоритм Баум­Велша
        private static void BaumWelshAlgorithm(string sequence)
        {
            // Вычисляем fk(i) алгоритмом просмотра вперед
            var fk = Forward(sequence);

            // Вычисляем bk(i) алгоритмом просмотра назад
            var bk = Backward(sequence);

            // Добавляем вклад последовательности в Akl и Ek(b):
            //     Akl - количество переходов из k в l + rkl
            //     Ek(b) - количество генераций b в состоянии k + rk(b)
            var A = new double[StatesCount, sequence.Length];
            var E = new double[StatesCount, sequence.Length];

            for (var k = 0; k < StatesCount; k++)
            {
                for (var l = 0; l < sequence.Length; l++)
                {
                    var Px = 0.0;
                    for (var j = 0; j < StatesCount; j++)
                        Px += fk[j, X(sequence[sequence.Length-1])];
                    
                    for (var i = 0; i < sequence.Length; i++)
                    {
                        A[k, l] += (fk[k, X(sequence[i])] * a[k, l] * e[i, X(sequence[i + 1])] * bk[i, X(sequence[i + 1])]) / Px;
                        E[k, l] += (fk[k, X(sequence[i])] * bk[k, X(sequence[i])]) / Px;
                    }
                }
            }

            // Вычисляем новые параметры модели akl и ek(b) и повторяем итерации

            // Вычислим значение правдоподобия модели
        }

        private static double[,] Backward(string sequence)
        {
            var bk = new double[StatesCount, Input.Length];

            // Инцииализация
            for (var k = 0; k < StatesCount; k++)
                bk[k, 0] = a[k, 0];

            // Рекукрсия
            for (var i = Input.Length-2; i > 0; i--)
                for (var k = 0; k < StatesCount; k++)
                {
                    double summj = 0;
                    for (var j = 0; j < StatesCount; j++)
                        summj += e[j, X(sequence[i + 1])] * a[j, k] * bk[j, i + 1];

                    bk[k, i] = summj;
                }

            return bk;
        }

        private static double[,] Forward(string sequence)
        {
            var fk = new double[StatesCount, Input.Length];

            // Инцииализация
            fk[0, 0] = 1;
            for (var i = 1; i < StatesCount; i++)
                fk[i, 0] = 0;

            // Рекукрсия
            for (var i = 1; i < Input.Length; i++)
                for (var k = 0; k < StatesCount; k++)
                {
                    double summj = 0;
                    for (var j = 0; j < StatesCount; j++)
                        summj += a[j, k]*fk[k, i - 1];

                    fk[k, i] = e[k, X(sequence[i])]*summj;
                }

            return fk;
        }

        private static int X(char c)
        {
            switch (c)
            {
                default: return 0;
                case 'C': return 1;
                case 'G': return 2;
                case 'T': return 3;
            }
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
                V[k, 0] = e[k,k];

            // Рекурсия
            for (int i = 1; i < Input.Length; i++)
            {
                for (int k = 0; k < StatesCount; k++)
                {
                    // Поиск max j
                    double maxj = 0;
                    for (int j = 0; j < StatesCount; j++)
                    {
                        var value = a[j, k] * V[k, i - 1];
                        if (maxj <= value)
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
            for (int i = Input.Length-2; i > 0; i--)
                answer[i - 1] = indexes[answer[i], i];

            return answer;
        }

        private static void InitializeAandE()
        {
            var random = new Random();

            for (var i = 0; i < StatesCount; i++)
                for (var j = 0; j < StatesCount; j++)
                {
                    a[i, j] = random.NextDouble();
                    //e[i, j] = 1 / (double)StatesCount;
                }

            for (var i = 0; i < N; i++)
                for (var j = 0; j < L; j++)
                {
                    e[i, j] = random.NextDouble();
                }
        }
    }
}
