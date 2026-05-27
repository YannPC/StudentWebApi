using Microsoft.Data.SqlClient;
using StudentApi;

var builder = WebApplication.CreateBuilder(args);

// ADD THIS - Allow Angular app to call the API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Angular default port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ADD THIS - Use the CORS policy
app.UseCors("AllowAngular");

string connectionString =
    "Server=.\\SQLEXPRESS;Database=MyFirstDB;Integrated Security=True;TrustServerCertificate=True;";

// GET /students
app.MapGet("/students", () =>
{
    var students = new List<Student>();

    using SqlConnection connection = new SqlConnection(connectionString);
    connection.Open();

    string query = "SELECT Id, FirstName, Age, Grade, LastName FROM Students"; 
    using SqlCommand command = new SqlCommand(query, connection);
    using SqlDataReader reader = command.ExecuteReader();

    while (reader.Read())
    {
        students.Add(new Student
        {
            Id = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            Age = reader.GetInt32(2),
            Grade = reader.GetString(3),
            LastName = reader.GetString(4)

        });
    }

    return Results.Ok(students);
});

// GET /students/{id}
app.MapGet("/students/{id}", (int id) =>
{
using SqlConnection connection = new SqlConnection(connectionString);
connection.Open();

    string query = "SELECT Id, FirstName, Age, Grade, LastName FROM Students WHERE Id = @Id";
    using SqlCommand command = new SqlCommand(query, connection);
command.Parameters.AddWithValue("@Id", id);
using SqlDataReader reader = command.ExecuteReader();

if (reader.Read())
{
        var student = new Student
        {
            Id = reader.GetInt32(0),
            FirstName = reader.GetString(1),
            Age = reader.GetInt32(2),
            Grade = reader.GetString(3),
            LastName = reader.GetString(4)
        };
return Results.Ok(student);
 }

    return Results.NotFound($"Student with ID {id} not found.");
});

app.Run();