using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/MyName", (string name) => $"My Name is {name}");

Cat mycat = new Cat()
{
    Name="kity",
    Aget =11
};  T 

app.MapPost("/Cat", (Cat cat) => mycat = cat);
app.MapGet("/Cat", () => mycat);


// Get : Read
// Post : 
// Patch
// Put
// delete

app.Run();

public class Cat{
    public string Name {get; set;}
    public int Aget {get; set;}
}