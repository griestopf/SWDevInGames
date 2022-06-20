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

scene.Render();





public abstract class GraphOb
{
    public string Name;
    public abstract void Render();
}

public class Sphere : GraphOb
{
    public double Radius;

    public override void Render()
    {
        Console.WriteLine($"I am a sphere with radius {Radius}");
    }
}

public class Cuboid : GraphOb
{
    public double Width;
    public double Height;
    public double Depth;

    public override void Render()
    {
        Console.WriteLine($"Hi I am {Name}, the cuboid with size {Width}, {Height}, {Depth}");
    }    
}

public class Group : GraphOb
{
    public List<GraphOb> Children = new List<GraphOb>();

    public override void Render()
    {
         Console.WriteLine($"I am the group named {Name}. I have {Children.Count} children.");
         TraverseChildren();
    }

    public void TraverseChildren()
    {
        foreach(GraphOb child in Children)
        {
            child.Render();
        }
    }
}
