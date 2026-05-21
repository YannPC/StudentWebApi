using Microsoft.Data.SqlClient;
using StudentApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Your connection string
string connectionString =
    "Server=.\\SQLEXPRESS;Database=MyFirstDB;Integrated Security=True;TrustServerCertificate=True;";

// GET /students  → returns all students as JSON
app.MapGet("/students", () =>
{
    var students = new List<Student>();

    using SqlConnection connection = new SqlConnection(connectionString);
    connection.Open();

    string query = "SELECT Id, Name, Age, Grade FROM Students";
    using SqlCommand command = new SqlCommand(query, connection);
    using SqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        students.Add(new Student
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Age = reader.GetInt32(2),
            Grade = reader.GetString(3)
        });
    }

    return Results.Ok(students);
});

// GET /students/{id}  → returns one student by ID
app.MapGet("/students/{id}", (int id) =>
{
    using SqlConnection connection = new SqlConnection(connectionString);
    connection.Open();

    string query = "SELECT Id, Name, Age, Grade FROM Students WHERE Id = @Id";
    using SqlCommand command = new SqlCommand(query, connection);
    command.Parameters.AddWithValue("@Id", id);
    using SqlDataReader reader = command.ExecuteReader();

    if (reader.Read())
    {
        var student = new Student
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Age = reader.GetInt32(2),
            Grade = reader.GetString(3)
        };
        return Results.Ok(student);
    }

    return Results.NotFound($"Student with ID {id} not found.");
});

app.Run();