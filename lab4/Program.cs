using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace lab4_new
{
    // Реализация алгоритма Укконена
    // Программа писалась последовательно, реализую сначало решение за O(|n|^3) и доводя ее до O(|n|)

    // 1) Сначала был реализован жадный алгоритм O(|n|^3)
    // 2) Добавлены суффиксные ссылки, что дало O(|n|^2)
    // 3) Переписан спуск, который стал пропорционален числу вершин, итого O(|n|)

    
    // Класс вершины суффиксного дерева
    class Elem
    {
        public string Symbol;       // Метка дуги, ведущая в текущую вершину
        public List<Elem> Childres; // Список ссылок на дочерние вершины

        public Elem(string symbol)  
        {
            Symbol = symbol;
            Childres = new List<Elem>();
        }

        public bool HasSuffixLink { get; set; }  // Флаг наличия суффикснов ссылки
        public Elem SuffixLink { get; set; }     // Суффиксная ссылка
        public Elem Parent { get; set; }         // Ссылка на родителя
    }

    class Program
    {
        static Elem _root = new Elem(" "); // Суффиксное дерево
        private static Elem _currentLeaf;  // Указатель на текущую вершину

        // Точка входа
        static void Main()
        {
            const string input = "axabxa"; // Исходная строка
            Ukkonen(input);                // Запуск алгоритма
            Console.ReadKey();             // Остановка программы
        }

        private static void Ukkonen(string input)
        {
            // Первая фаза - тривиальна
            var first = new Elem(input[0].ToString(CultureInfo.InvariantCulture));
            first.Parent = _root;
            _root.Childres.Add(first);

            // Отдаем на обработку подстроки остальных фаз.
            for (var i = 2; i <= input.Length; i++)
            {
                // Обработку начинаем с листа 1, устанавливая ссылку на него перед запуском каждой фазы
                _currentLeaf = first;
                Insert(input.Substring(0, i));
            }
        }

        // Обработка текущей фазы
        private static void Insert(string str)
        {
            //первое продолжение - тривиально
            var save = _currentLeaf.Symbol;
            _currentLeaf.Symbol = _currentLeaf.Symbol + str.Last();
            var i = 1;

            // После первого продолжения суффиксные ссылки никому устанавливать не надо
            var setSuffixLink = false;
            Elem savedSplit = null;

            // прочие продолжения
            while (true)
            {
                // шагаем вверх или по суффиксным ссылкам до корня
                while (_currentLeaf != _root)
                {
                    // идем по суффиксной ссылке если есть
                    if (_currentLeaf.HasSuffixLink)
                    {
                        _currentLeaf = _currentLeaf.SuffixLink;
                    }
                    else // или к родителю
                    {
                        _currentLeaf = _currentLeaf.Parent;

                        if ((_currentLeaf == _root) || (!_currentLeaf.HasSuffixLink))
                            save = save.Substring(1);
                    }
                }
                
                // Что-то надо достроить прям от корня
                if (save == "")
                {
                    // спускаемся вниз 
                    var next = _currentLeaf.Childres.FirstOrDefault(el => el.Symbol.StartsWith(str[i].ToString()));
                    var prev = next;
                    while (next != default(Elem))
                    {
                        prev = next;
                        next = next.Childres.FirstOrDefault(el => el.Symbol.StartsWith(str[i].ToString()));
                    }

                    // Если есть что достраивать - строим, иначе правило 3 - ничего не делаем
                    if (prev == null)
                    {
                        // строим от корня новый лист
                        var newLeaf = new Elem(str[i].ToString());
                        newLeaf.Parent = _currentLeaf;
                        _currentLeaf.Childres.Add(newLeaf);

                        // Если это следующее продолжение, надо установить суффиксную ссылку сохраненной ранее вершине
                        if (setSuffixLink)
                        {
                            savedSplit.SuffixLink = _currentLeaf;
                            savedSplit.HasSuffixLink = true;
                            setSuffixLink = false;
                        }
                    }
                    else
                    {
                        // сработало правило 3 - заканчивай дело, значит дальше нет смысла выполнять продолжения
                        break;
                    }
                }
                else // надо еще немного спуститься от корня
                {
                    // спускаемся вниз 
                    var answ = FindNext(_currentLeaf, save);
                    var prev = answ.prev; // Вершина от которой надо достроить продолжение
                    save = answ.save;     // Та часть строки, которую будем достраивать
                    
                    // Если попали в лист
                    if (prev.Symbol == save)
                    {
                        // Достраиваем ему необходимую часть строки
                        prev.Symbol = prev.Symbol + str.Last();
                        _currentLeaf = prev;
                    }
                    else
                    { 
                        // Если вершина мнимая - надо ее разбить на две
                        if (!prev.Symbol.StartsWith(save + str.Last()))
                        {
                            // Объявили расщепляющую вершинку
                            var splitLeaf = new Elem(save);
                            splitLeaf.Parent = prev.Parent;
                            splitLeaf.Parent.Childres.Remove(prev);
                            splitLeaf.Parent.Childres.Add(splitLeaf);

                            // Устанавливаем ей в потомки вершинку которая уже существовала, с соответственным изменением метки
                            prev.Symbol = prev.Symbol.Substring(save.Length);
                            splitLeaf.Childres.Add(prev);
                            prev.Parent = splitLeaf;
                            
                            // А также новую, только что созданную вершинку
                            var newLeaf = new Elem(str.Substring(prev.Symbol.Length));
                            newLeaf.Parent = splitLeaf;
                            splitLeaf.Childres.Add(newLeaf);

                            _currentLeaf = splitLeaf;
                            setSuffixLink = true;
                            savedSplit = splitLeaf;
                        }
                    }
                }

                i++;
                if (i == str.Length) break;
            }
        }

        // Спуск вниз по вершинам в дереве
        private static Pair FindNext(Elem currentLeaf, string save)
        {
            var answ = new Pair(currentLeaf, save);
            var next = currentLeaf.Childres.FirstOrDefault(el => el.Symbol.StartsWith(save));
            var prev = next;

            // Если сразу не попали в нужную вершину, переходим к одному из ее потомков
            if (next == null)
            {
                //for (int j = 1; j < save.Length; j++)
                //{
                    next = _currentLeaf.Childres.FirstOrDefault(el => el.Symbol.StartsWith(save.Substring(0, 1)));
                    if (next != null)
                    {
                        save = save.Substring(1);
                        return FindNext(next, save);
                    }
                //}
            }
            else // Нашли нужную вершину, возвращаемся
            {
                //while (next != default(Elem))
                //{
                    prev = next;
                    //next = next.Childres.FirstOrDefault(el => el.Symbol.StartsWith(save));
                //}
                return new Pair(prev, save);
            }

            return answ;
        }
    }

    // Вспомогательная структура, для возврата значений при поиске
    class Pair
    {
        public Elem prev;
        public string save;

        public Pair(Elem prev, string save)
        {
            this.prev = prev;
            this.save = save;
        }
    }
}
