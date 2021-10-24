using System;
using System.Drawing;

namespace RSM_TestTask
{
    /// <summary>
    /// Базовый класс для фигур
    /// </summary>
    public abstract class Figure
    {
        /// <summary>
        /// Значения, показывающие тип фигуры
        /// </summary>
        public enum Type
        {
            Cirle,
            Square,
            Rectangle
        };
        protected int id;
        protected int mainSize;
        protected int? parentId;
        public int Id
        {
            get { return id; }
        }
        public int Size
        {
            get { return mainSize; }
        }
        public int? ParentId
        {
            get { return id; }
        }
        /// <summary>
        /// Метод для переопределения, который возвращает строку с хранящимися значениями фигуры (На данный момент)
        /// </summary>
        virtual public string DisplayValues
        {
            get;
        }

    }
    class Circle : Figure
    {
        public Circle(int Id, int Radius, int? ParentId)
        {
            id = Id;
            mainSize = Radius;
            parentId = ParentId;
        }
        override public string DisplayValues
        {
            get
            {
                var ParentIdString = parentId == null ? "Null" : parentId.ToString();
                return $"Figure type = Circle\n" +
                       $"Id = {id}\n" +
                       $"Radius = {mainSize}\n" +
                       $"Parent Id = {ParentIdString}\n";
            }

        }
        public Figure AddNewChild(Context Context, Type Type, int Radius, int? Parentid)
        {
            if (Type == Figure.Type.Rectangle)
                throw new Exception("Wrong figure type \nIf you want to add rectangle use overload method");
            return Context.AddNewFigure(Type, Radius, null, this.Id);
        }

    }
    class Square : Figure
    {
        public Square(int Id, int Side, int? ParentId)
        {
            id = Id;
            mainSize = Side;
            parentId = ParentId;
        }
        override public string DisplayValues
        {
            get
            {
                var ParentIdString = parentId == null ? "Null" : parentId.ToString();
                return $"Figure type = Square\n" +
                       $"Id = {id}\n" +
                       $"Side lenght = {mainSize}\n" +
                       $"Parent Id = {ParentIdString}\n";
            }
        }
        public Figure AddNewChild(Context Context, Type Type, int SideLenght, int? Parentid)
        {
            if (Type == Type.Rectangle)
                throw new Exception("Wrong figure type \nIf you want to add rectangle use overload method");
            return Context.AddNewFigure(Type, SideLenght, null, this.Id);
        }

    }
    class Rectangle : Figure
    {
        public Rectangle(int Id, int Width, int Lenght, int? ParentId)
        {
            id = Id;
            mainSize = Width;
            addictSize = Lenght;
            parentId = ParentId;
        }
        private int addictSize;
        new public Size Size
        {
            get { return new Size(mainSize, addictSize); }
        }
        override public string DisplayValues
        {
            get
            {
                var ParentIdString = parentId == null ? "Null" : parentId.ToString();
                return $"Figure type = Rectangle\n" +
                       $"Id = {id}\n" +
                       $"Lenght = {addictSize}\n" +
                       $"Width = {mainSize}\n" +
                       $"Parent Id = {ParentIdString}\n";
            }
        }
        public Figure AddNewChild(Context Context, Type Type, int Lenght, int Width)
        {
            if (Type == Type.Rectangle)
                throw new Exception("Wrong figure type \nIf you want to add Circle or Square use overload method");
            return Context.AddNewFigure(Type, Lenght, Width, this.Id);
        }
    }
}
