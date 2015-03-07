using System;

namespace lab2
{
    class Program
    {
        static void Main()
        {
            const string A = " ADHBBAJWWAAHAAHH";
            const string B = " ADBBAWAAAA";
            
            // матрица оценок
            var F = new int[A.Length, B.Length];

            // линейный штраф
            const int d = -5;

            // Вычисляем матрицу F
            for (var i = 0; i < A.Length; i++)
                F[i, 0] = d * i;

            for (var j = 0; j < B.Length; j++)
                F[0, j] = d * j;

            for (var i = 1; i < A.Length; i++)
                for (var j = 1; j < B.Length; j++)
                {
                    var match = F[i - 1, j - 1] + S(A[i], B[j]);
                    var delete = F[i - 1, j] + d;
                    var insert = F[i, j - 1] + d;
                    F[i, j] = Math.Max(Math.Max(match, insert), delete);
                }

            var alignmentA = "";
            var alignmentB = "";

            var _i = A.Length-1;
            var _j = B.Length-1;

            // Алгоритм Нидлмана — Вунша
            while (_i > 0 && _j > 0)
            {
                var score = F[_i, _j];
                var scoreDiag = F[_i - 1, _j - 1];
                var scoreUp = F[_i, _j - 1];
                var scoreLeft = F[_i - 1, _j];

                if (score == scoreDiag + S(A[_i], B[_j]))
                {
                    alignmentA = A[_i] + alignmentA;
                    alignmentB = B[_j] + alignmentB;
                    _i = _i - 1;
                    _j = _j - 1;
                    continue;
                }
                
                if (score == scoreLeft + d)
                {
                    alignmentA = A[_i] + alignmentA;
                    alignmentB = "-" + alignmentB;
                    _i = _i - 1;
                    continue;
                } 
                
                if (score == scoreUp + d)
                {
                     alignmentA = "-" + alignmentA;
                     alignmentB = B[_j] + alignmentB;
                     _j = _j - 1;
                }
              }


              Console.WriteLine(alignmentA);
              Console.WriteLine(alignmentB);
              Console.ReadKey();
        }

        private static readonly int[] Blosum62 =
        {
            4, -1, -2, -2, 0, -1, -1, 0, -2, -1, -1, -1, -1, -2, -1, 1, 0, -3, -2, 0, -2, -1, 0, -4,
            -1, 5, 0, -2, -3, 1, 0, -2, 0, -3, -2, 2, -1, -3, -2, -1, -1, -3, -2, -3, -1, 0, -1, -4,
            -2,  0,  6,  1, -3,  0,  0,  0,  1, -3, -3,  0, -2, -3, -2,  1,  0, -4, -2, -3,  3,  0, -1, -4, 
            -2, -2,  1,  6 -3,  0,  2 -1, -1, -3, -4, -1, -3, -3, -1,  0, -1, -4, -3, -3,  4,  1, -1, -4, 
            0, -3, -3, -3,  9, -3, -4, -3, -3, -1, -1, -3, -1, -2, -3, -1, -1, -2, -2, -1, -3, -3, -2, -4, 
            -1,  1,  0,  0, -3,  5,  2 -2,  0, -3, -2,  1,  0, -3, -1,  0, -1, -2, -1, -2,  0,  3, -1, -4, 
            -1,  0,  0,  2, -4,  2,  5, -2,  0, -3, -3,  1, -2, -3, -1,  0, -1, -3, -2, -2,  1,  4 -1, -4, 
            0, -2,  0, -1, -3, -2, -2,   6, -2, -4, -4, -2, -3, -3, -2,  0, -2, -2, -3, -3, -1, -2, -1, -4, 
            -2,  0,  1, -1, -3,  0,  0, -2,  8, -3, -3, -1, -2, -1, -2, -1, -2, -2,  2 -3,  0,  0, -1, -4, 
            -1, -3, -3, -3, -1, -3, -3, -4, -3,  4,  2, -3,  1,  0, -3, -2, -1, -3, -1,  3, -3, -3, -1, -4, 
            -1, -2, -3, -4, -1, -2, -3, -4, -3,  2,  4, -2,  2,  0, -3, -2, -1, -2, -1,  1, -4, -3, -1, -4, 
            -1,  2,  0, -1, -3,  1,  1, -2, -1, -3, -2,  5, -1, -3, -1,  0, -1, -3, -2, -2,  0,  1, -1, -4, 
            -1, -1, -2, -3, -1,  0, -2, -3, -2,  1,  2 -1,  5,  0, -2, -1, -1, -1, -1,  1, -3, -1, -1, -4, 
            -2, -3, -3, -3, -2, -3, -3, -3, -1,  0,  0, -3,  0,  6 -4, -2, -2,  1,  3, -1, -3, -3, -1, -4, 
            -1, -2, -2, -1, -3, -1, -1, -2, -2, -3, -3, -1, -2, -4, 7, -1, -1, -4, -3, -2, -2, -1, -2, -4, 
            1, -1,  1,  0, -1,  0,  0,  0, -1, -2, -2,  0, -1, -2, -1,  4,  1, -3, -2, -2,  0,  0,  0, -4, 
            0, -1,  0, -1, -1, -1, -1, -2, -2, -1, -1, -1, -1, -2, -1,  1,  5, -2, -2,  0, -1, -1,  0, -4, 
            -3, -3, -4, -4, -2, -2, -3, -2, -2, -3, -2, -3, -1,  1, -4, -3, -2, 1, 1,  2, -3, -4, -3, -2, -4, 
            -2, -2, -2, -3, -2, -1, -2, -3, 2, -1, -1, -2, -1,  3, -3, -2, -2,  2,  7, -1, -3, -2, -1, -4, 
            0, -3, -3, -3, -1, -2, -2, -3, -3,  3,  1, -2,  1, -1, -2, -2,  0, -3, -1,  4 -3, -2, -1, -4, 
            -2, -1,  3,  4 -3,  0,  1, -1,  0, -3, -4,  0, -3, -3, -2,  0, -1, -4, -3, -3,  4,  1, -1, -4, 
            -1,  0,  0,  1, -3,  3,  4 -2,  0, -3, -3,  1, -1, -3, -1,  0, -1, -3, -2, -2,  1,  4, -1, -4, 
            0, -1, -1, -1, -2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -2,  0,  0, -2, -1, -1, -1, -1, -1, -4, 
            -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4,  1 
        };

        static int S(char a, char b)
        {
            var i = GetIdx(a);
            var j = GetIdx(b);
            return Blosum62[i*23 + j];
        }

        private static int GetIdx(char c)
        {
            int r;
            switch (c)
            {
                default: r = 23; break;
                case 'A': r = 0; break;
                case 'R': r = 1; break;
                case 'N': r = 2; break;
                case 'D': r = 3; break;
                case 'C': r = 4; break;
                case 'Q': r = 5; break;
                case 'E': r = 6; break;
                case 'G': r = 7; break;
                case 'H': r = 8; break;
                case 'I': r = 9; break;
                case 'L': r = 10; break;
                case 'K': r = 11; break;
                case 'M': r = 12; break;
                case 'F': r = 13; break;
                case 'P': r = 14; break;
                case 'S': r = 15; break;
                case 'T': r = 16; break;
                case 'W': r = 17; break;
                case 'Y': r = 18; break;
                case 'V': r = 19; break;
                case 'B': r = 20; break;
                case 'Z': r = 21; break;
                case 'X': r = 22; break;
            }

            return r;
        }
    }
}
