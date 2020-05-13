using System;

namespace InterfaceVsPattern
{
    // Rect and Circle both implement a common interface requiring the `CalculateArea` method
    interface Object2D
    {
        double CalculateArea();
    }

    class Circle2D : Object2D
    {
        public double Radius;

        public double CalculateArea()
        {
            return 3.141592 * Radius * Radius;
        }
    }

    class Rect2D : Object2D
    {
        public double Width;
        public double Height;

        public double CalculateArea()
        {
            return Width * Height;
        }
    }


    // Here, Rect and Circle have no relation, nor do they know how to calculate their area
    class SimpleCircle
    {
        public double Radius;
    }

    class SimpleRect
    {
        public double Width;
        public double Height;
    }

    class Program
    {
        static void Main(string[] args)
        {
            /////////////////////////////////////////////////////////////////////////////////////////////

            // List of 2D objects
            Object2D[] object2DList = new Object2D[]
            {
                new Circle2D { Radius = 2},
                new Rect2D { Width = 3, Height = 5},
                new Rect2D { Width = 3, Height = 7},
                new Circle2D { Radius = 6},
            };

            // Polymorphic area calculation using the common interface method `CalcuateArea`
            foreach (Object2D object2D in object2DList)
            {
                double calculatedArea = object2D.CalculateArea();
                Console.WriteLine("Area of object2D: " + calculatedArea);
            }

            /////////////////////////////////////////////////////////////////////////////////////////////

            // Simple objects list
            object[] simpleObjectList = new object[]
            {
                new SimpleCircle { Radius = 2},
                new SimpleRect { Width = 3, Height = 5},
                new SimpleRect { Width = 3, Height = 7},
                new SimpleCircle { Radius = 6},
            };

            // Polymorphic area calculation implemented using the new switch pattern matching feature with a  type pattern 
            foreach (object simpleOb in simpleObjectList)
            {
                double calculatedArea = simpleOb switch
                {
                    SimpleCircle  circle  => circle.Radius * circle.Radius * 3.141692,
                    SimpleRect    rect    => rect.Height * rect.Width,
                    _               => 0
                };

                Console.WriteLine("Area of simple object: " + calculatedArea);
            }

        }

    }
}
