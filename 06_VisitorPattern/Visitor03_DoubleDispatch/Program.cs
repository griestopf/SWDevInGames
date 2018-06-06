using System;
using System.Collections.Generic;

namespace VisitorDoubleDispatch
{

    public interface Visitor
    {
        void Visit(Sphere s);
        void Visit(Cuboid c);
        void Visit(Group g);

    }

    class Renderer : Visitor
    {
        public void Visit(Sphere s)
        {
            Console.WriteLine($"Hey, I'm a sphere with radius {s.Radius} and my name is {s.Name}.");
        }
        public void Visit(Cuboid c)
        {
            Console.WriteLine($"Greets from {c.Name}, the cuboid with dimensions {c.Width}, {c.Length} and {c.Height}.");
        }
        public void Visit(Group g)
        {
            Console.WriteLine($"I am the group named {g.Name}.");
            g.TraverseChildren(this);
        }
    }

    class JsonExporter : Visitor
    {
        public void Visit(Sphere s)
        {
            throw new NotImplementedException();
        }

        public void Visit(Cuboid c)
        {
            throw new NotImplementedException();
        }

        public void Visit(Group g)
        {
            throw new NotImplementedException();
        }
    }


    public abstract class GraphicsOb
    {
        public abstract void Accept(Visitor v);
        public string Name;
    }

    public class Sphere : GraphicsOb
    {
        public float Radius;

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class Cuboid : GraphicsOb
    {
        public float Width, Length, Height;
        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }
    }

    public class Group : GraphicsOb
    {
        public List<GraphicsOb> Children;

        public override void Accept(Visitor v)
        {
            v.Visit(this);
        }

        public void TraverseChildren(Visitor v)
        {
            if (Children == null)
                return;
            foreach (var child in Children)
            {
                child.Accept(v);
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
            
            Renderer renderer = new Renderer();

            renderer.Visit(scene);
        }
    }
}
