using System;



namespace RSM_TestTask
{
    /// <summary>
    /// <para>
    /// Программа реализована с использованием вспомогательного класса <see cref="Context"/><br/>
    /// С его помощь происходит всё взаимодействие с БД (CRUD)<br/>
    /// Сделано это для оптимизации - чтобы не открывать/закрывать для каждого запроса новое подключение к БД
    /// </para>
    /// <para>
    /// Для реализации объектной модели фигур создан базовый абстрактный класс <see cref="Figure"/><br/>
    /// А также классы самих объектов : <see cref="Circle"/>,<see cref="Square"/>,<see cref="Rectangle"/>
    /// </para>
    /// <para>
    /// База данных заполнена случайно сгенерированными значениями:<br/>
    /// 10 тыс. строк объектов, которые находятся в случайной, иерархической структуре<br/>
    /// 10 тыс. строк изменений, которые случаным образом обновили строки (даты от 2015 года и старше)
    /// </para>
    /// <para>
    /// Основная часть запросов выполнена просто в виде строки<br/>
    /// Запросы для задания хранятся в папке Queries 
    /// (Все объекты дерева, упорядоченные от корня,
    /// Все объекты типа круг, у которых родителем является квадрат)
    /// </para>
    /// <para>
    /// БД находится в папке с проектом
    /// </para>
    /// <para>
    ///  В теле метода <see cref="Main"/> кусок кода, для проверки работы
    /// </para>
    /// </summary>
    class Program
    {
        static void Main()
        {
            var Context = new Context();
            #region Получения всех фигур упорядоченных от корня
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---\nQuery GetAllByDate\n---");
            Console.ResetColor();
            foreach (var figure in Context.GetAllFiguresByDate(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-3)))
                Console.WriteLine("Id="+figure.Id+"\n" +
                                  "Type = "+figure.Type+"\n" +
                                  "AreaSize="+figure.Area+"\n" +
                                  "----------------------");
            #endregion
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("---\nQuery GetCircle_ParentSquare\n---");
            Console.ResetColor();
            #region Получение объектов типа круг, у которых родителем является квадрат)
            foreach (var figure in Context.GetCircle_ParentSquare(DateTime.Now.AddYears(-4), DateTime.Now.AddYears(-1)))
                Console.WriteLine("Id=" + figure.Id + "\n" +
                                  "Type = " + figure.Type + "\n" +
                                  "AreaSize=" + figure.Area + "\n" +
                                  "----------------------");
            #endregion

            Console.WriteLine("Ready");
            Console.ReadKey ();
        }
    }
}
