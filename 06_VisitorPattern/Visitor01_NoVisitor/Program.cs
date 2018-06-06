using System;
using System.Collections.Generic;

namespace VisitorNoVisitor
{
    public abstract class GraphicsOb
    {
        public abstract void Render();
        public string Name;
    }

    public class Sphere : GraphicsOb
    {
        public float Radius;        
        public override void Render()
        {
            Console.WriteLine($"Hey, I'm a sphere with radius {Radius} and my name is {Name}.");
        }
    }

    public class Cuboid : GraphicsOb
    {
        public float Width, Length, Height;
        public override void Render()
        {
            Console.WriteLine($"Greets from {Name}, the cuboid with dimensions {Width}, {Length} and {Height}.");
        }
    }

    public class Group : GraphicsOb
    {
        public List<GraphicsOb> Children;

        public override void Render()
        {
            Console.WriteLine($"I am the group named {Name}.");
            TraverseChildren();
        }


        public void TraverseChildren()
        {
            if (Children == null)
                return;
            foreach (var child in Children)
            {
                child.Render();
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Group scene = new Group
            {
                Name = "The Parent Group",
                Children = new List<GraphicsOb>
                {
                    new Group
                    {
                        Name = "The Child Group",
                        Children = new List<GraphicsOb>
                        {
                            new Sphere 
                            {
                                Name = "The Sphere",
                                Radius = 3
                            },
                            new Cuboid
                            {
                                Name = "First Cuboid",
                                Width = 6, Length = 7, Height = 8
                            }
                        }
                    },
                    new Cuboid
                    {
                        Name = "Second Cuboid",
                        Width = 2, Length = 4, Height = 6
                    }
                }
            };
            
            scene.Render();
        }
    }
}
