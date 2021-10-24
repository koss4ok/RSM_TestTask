using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace RSM_TestTask
{
    /// <summary>
    /// Класс реализующий функциональность взаимодействия с БД, посредством объектной модели.
    /// </summary>
    class Context
    {
        private const string Null = "null";
        SqlConnection connection;
        public Context()
        {
            connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Костя\Desktop\RSM_TestTask\RSM_TestTask\RSM_DB.mdf;Integrated Security=True");
            connection.Open();
        }
        #region Методы для работы с фигурами как они есть в БД
        /// <summary>
        /// Получение обектов по заданном в Reader запросу
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns></returns>
        private List<Figure> GetObjects(SqlDataReader Reader)
        {
            var figures = new List<Figure>();
            while (Reader.Read())
            {
                switch ((byte)Reader["ObjectType"])
                {
                    case ((byte)Figure.Type.Cirle):
                        {
                            figures.Add(
                                new Circle((int)Reader["Id"], (int)Reader["MainSize"], (int?)(Reader["ParentId"] == DBNull.Value ? null : Reader["ParentId"])));
                        }
                        break;
                    case ((byte)Figure.Type.Square):
                        {
                            figures.Add(
                                new Square((int)Reader["Id"], (int)Reader["MainSize"], (int?)(Reader["ParentId"] == DBNull.Value ? null : Reader["ParentId"])));
                        }
                        break;
                    case ((byte)Figure.Type.Rectangle):
                        {
                            figures.Add(
                            new Rectangle((int)Reader["Id"], (int)Reader["MainSize"], (int)Reader["AddictSize"], (int?)(Reader["ParentId"] == DBNull.Value ? null : Reader["ParentId"])));
                        }
                        break;
                }
            }
            return figures;
        }
        public Figure GetParent(Figure Figure)
        {
            var command = new SqlCommand($"SELECT *  FROM Objects WHERE Id = {Figure.ParentId}", connection);
            using (var reader = command.ExecuteReader())
            {
                return GetObjects(reader)[0];
            }
        }
        public List<Figure> GetChilds(Figure Figure)
        {
            var command = new SqlCommand($"SELECT *  FROM Objects WHERE ParentId = {Figure.Id}", connection);
            using (var reader = command.ExecuteReader())
            {
                return GetObjects(reader); ;
            }
        }
        public Figure GetFigure(int Id)
        {
            var command = new SqlCommand($"SELECT TOP 1 *  FROM [Objects] Where Id = {Id}", connection);
            using (var reader = command.ExecuteReader())
            {
                return GetObjects(reader)[0];
            }
        }
        public Figure AddNewFigure(Figure.Type Type, int MainSize, int? AddictSize, int? Parentid)
        {
            var parentIdString = Parentid == null ? "null" : Parentid.ToString();
            SqlCommand command;
            if (Type == Figure.Type.Rectangle)
                command = new SqlCommand($"INSERT INTO Objects VALUES ({(int)Type}, {MainSize}, {AddictSize}, {parentIdString})", connection);
            else
                command = new SqlCommand($"INSERT INTO Objects VALUES({(int)Type}, {MainSize}, {Null}, {parentIdString})", connection);
            command.ExecuteNonQuery();
            return GetLastAddedFigure();

        }
        public List<Figure> GetAllFigures(int? Limit = null)
        {
            SqlCommand command = new SqlCommand($"SELECT * FROM Objects", connection);
            if (Limit != null)
                command.CommandText = $"SELECT TOP {Limit} * FROM Objects"; //Если есть лимит записей - меняем текст запроса
            using (var reader = command.ExecuteReader())
            {
                return GetObjects(reader);
            }
        }
        /// <summary>
        /// Remove figure. Remove child figures too. 
        /// </summary>
        /// <param name="Id"></param>
        public void RemoveFigure(int Id)
        {
            var deletedIds = new List<int>() { Id };
            deletedIds.AddRange(GetAllChildsIds(Id));
            foreach (var id in deletedIds)
            {
                var command = new SqlCommand($"DELETE FROM Objects WHERE Id= {id}", connection);
                command.ExecuteNonQuery();
            }
        }
        private Figure GetLastAddedFigure()
        {
            var command = new SqlCommand($"SELECT TOP 1* FROM [Objects] ORDER BY Id DESC", connection);
            using (var reader = command.ExecuteReader())
            {
                return GetObjects(reader)[0];
            }
        }
        /// <summary>
        /// Возвращает список Id взях дочерних фигур всех уровней вложенности
        /// </summary>
        /// <returns></returns>
        private List<int> GetAllChildsIds(int ParentId)
        {
            var childIds = new List<int>();
            SqlDataReader reader;
            var command = new SqlCommand($"SELECT Id FROM [Objects] WHERE ParentId = {ParentId}", connection);
            using (reader = command.ExecuteReader())
            {
                while (reader.Read())
                    childIds.Add((int)reader["Id"]);
            }
            var count = childIds.Count;
            for (int i = 0; i < count; i++)// (var id in ChildIds)
                childIds.AddRange(GetAllChildsIds(childIds[i]));
            return childIds;
        }
        public void UpdateFigure(int FigureId, int MainSize, int? AddictSize, DateTime date)
        {
            var addictSizeString = AddictSize == null ? "null" : AddictSize.ToString();

            var command = new SqlCommand($"UPDATE Objects " +
                $"SET MainSize = {MainSize}, AddictSize={addictSizeString} " +
                $"WHERE Id = {FigureId}", connection);
            command.ExecuteNonQuery();
            command.CommandText = $"INSERT INTO Changes" +
                $" VALUES({FigureId}, '{DateTime.Now.Month}.{DateTime.Now.Day}.{DateTime.Now.Year}', {MainSize}, {addictSizeString} )";
            command.ExecuteNonQuery();
        }
        #endregion
        #region Методы реализующие задание
        /// <summary>
        /// Структура для значений по заданию 
        /// </summary>
        public struct TaskValue
        {
            public TaskValue(int Id, Figure.Type Type, int Area)
            {
                this.Id = Id;
                this.Type = Type;
                this.Area = Area;
            }
            public int Id;
            public Figure.Type Type;
            public int Area;
        }
        private List<TaskValue> GetValuesForTask(SqlDataReader Reader)
        {
            var list = new List<TaskValue>();
            while (Reader.Read())
            {
                list.Add(new TaskValue((int)Reader["Id"], (Figure.Type)(byte)Reader["ObjectType"], (int)Reader["Area"]));
            }
            return list;
        }
        public List<TaskValue> GetAllFiguresByDate(DateTime MinDate, DateTime MaxDate)
        {
            var command = new SqlCommand(new StreamReader(@"..\..\..\Queries\GetAllByDate.sql").ReadToEnd(), connection);
            command.Parameters.AddWithValue("@maxDate", MaxDate);
            command.Parameters.AddWithValue("@minDate", MinDate);
            using (var reader = command.ExecuteReader())
                return GetValuesForTask(reader);
        }
        public List<TaskValue> GetCircle_ParentSquare(DateTime MinDate, DateTime MaxDate)
        {
            var command = new SqlCommand(new StreamReader(@"..\..\..\Queries\GetCircle_ParentSquare.sql").ReadToEnd(), connection);
            command.Parameters.AddWithValue("@maxDate", MaxDate);
            command.Parameters.AddWithValue("@minDate", MinDate);
            using (var reader = command.ExecuteReader())
                return GetValuesForTask(reader);
        }
        #endregion
    }
}
