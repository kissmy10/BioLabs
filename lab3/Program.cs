using System;
using System.Collections.Generic;
using System.Linq;

namespace lab3
{
    class Program
    {
        static void Main()
        {
            const string str = "abeccafaddabbadew";
            const string needle = "abbad";

            // Алгоритм Бойера-Мура
            var position = GetBoerMur(str, needle);
            PrintAnswer(str, needle, position);

            // Алгоритм Кнута-Морриса-Пратта
            position = GetKmp(str, needle);
            PrintAnswer(str, needle, position);

            Console.ReadKey();
        }

        private static int GetKmp(string str, string needle)
        {
            var position = -1;

            // Ищем префикс функцию
            var concat = needle + "#" + str;
            var prefixFunction = GetPrefixFunction(concat);

            // Ищем значение значение префикс функции, которое равно |needle|
            var n = needle.Length;
            for (var i = 0; i < prefixFunction.Length; i++)
                if (prefixFunction[i] == n)
                    position = i - 2*n;

            return position;
        }

        // Вычисление префикс функции
        private static int[] GetPrefixFunction(string concat)
        {
            var answer = new int[concat.Length];
            var n = concat.Length;
	        for (var i = 0; i < n; i++)
		        for (var k = 0; k <= i; k++)
                    if (concat.Substring(0, k) == concat.Substring(i - k + 1, k))
                        answer[i] = k;

            return answer;
        }

        private static int GetBoerMur(string str, string needle)
        {
            var alphabet = str.GroupBy(el => el);


            // Заполняем таблицу стоп-символов
            var stopTable = new Dictionary<char, int>();
            foreach (var symbol in alphabet)
            {
                var idx = Math.Max(needle.LastIndexOf(symbol.Key), 0);
                stopTable.Add(symbol.Key, idx);
            }

            // Заполняем таблицу суффиксов
            var suffixTable = new Dictionary<string, int> { { String.Empty, 1 } };

            for (var i = 1; i < needle.Length; i++)
            {
                var suffix = needle.Substring(needle.Length - i, i);
                int j;
                for (j = i; j < needle.Length - i; j++)
                {
                    var substr = needle.Substring(j, i);
                    if (substr == suffix)
                    {
                        suffixTable.Add(suffix, j - i);
                        break;
                    }
                }

                // Случай если не найдено ни одно совпадение, тогда значение сдвига будет |needle|
                if (j >= needle.Length - i)
                    suffixTable.Add(suffix, needle.Length);
            }

            // выполняем проход по строке
            var position = 0;
            while (true)
            {
                // посимвольно сравниваем строку и шаблон
                var i = needle.Length - 1;
                while (i >= 0)
                {
                    if (needle[i] != str[i + position])
                        break;
                    i--;
                }

                // если все символы шаблона совпали - то мы нашли совпадение
                if (i < 0)
                    break;

                // пробуем воспользоваться стоп-символами
                var stopShift = stopTable.SingleOrDefault(el => el.Key == str[i + position]);
                if (i + 1 - stopShift.Value > 0)
                {
                    position += i + 1 - stopShift.Value;
                    continue;
                }

                // пробуем использовать таблицу суффиксов
                if (i <= needle.Length - 1)
                {
                    var subStr = needle.Substring(i);
                    var suffixShift = suffixTable.SingleOrDefault(el => el.Key == subStr);
                    position += suffixShift.Value;
                }

            }

            return position;
        }
        
        private static void PrintAnswer(string str, string needle, int position)
        {
            // Выводим ответ
            Console.WriteLine();
            Console.WriteLine("  " + str);
            for (int i = 0; i < position + 2; i++)
                Console.Write(" ");
            Console.WriteLine(needle);

            Console.WriteLine("  Position: " + position);
            Console.WriteLine(" ");
        }
    }
}
