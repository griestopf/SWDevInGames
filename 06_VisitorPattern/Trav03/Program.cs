// See https://aka.ms/new-console-template for more information

Group scene = new Group
{
    Name = "Da Scene",
    Children =
    {
        new Sphere {Name = "Sphere One", Radius = 3},
        new Group 
        {
            Name = "Child Group",
            Children = 
            {
                new Cuboid {Name = "Just a cube", Width = 2, Height = 3, Depth = 4},
                new Sphere {Name = "Sphere Two", Radius = 3.1415},                           
            }
        },
        new Cuboid {Name = "Another Cuboid", Width = 6, Height = 7, Depth = 8},
    }
};

RenderVisitor v = new RenderVisitor();
v.Visit(scene);


public interface Visitor
{
    public void Visit(Sphere s);
    public void Visit(Cuboid c);
    public void Visit(Group g);
}

public class RenderVisitor : Visitor
{
    public void Visit(Cuboid c)
    {
        Console.WriteLine($"Hi I am {c.Name}, the cuboid with size {c.Width}, {c.Height}, {c.Depth}");
    }    
    public void Visit(Sphere s)
    {
        Console.WriteLine($"I am a sphere with radius {s.Radius}");
    }
    public void Visit(Group g)
    {
         Console.WriteLine($"I am the group named {g.Name}. I have {g.Children.Count} children.");
         g.TraverseChildren(this);
    }
}
public abstract class GraphOb
{
    public string Name;
    public abstract void Accept(Visitor v);
}

public class Sphere : GraphOb
{
    public double Radius;

    public override void Accept(Visitor v)
    {
        v.Visit(this);
    }
}

public class Cuboid : GraphOb
{
    public double Width;
    public double Height;
    public double Depth;

    public override void Accept(Visitor v)
    {
        v.Visit(this);
    }    
}

public class Group : GraphOb
{
    public List<GraphOb> Children = new List<GraphOb>();

    public override void Accept(Visitor v)
    {
        v.Visit(this);
    }

    public void TraverseChildren(Visitor v)
    {
        foreach(GraphOb child in Children)
        {
            child.Accept(v);
            // v.Visit(child);
        }
    }
}

