using System.Data.SQLite;

namespace p32bx_datebasse_2
{
    internal class Program
    {
        static string connectionString = "Data Source=students.db;Version=3;";

        static void Main()
        {
            CreateDatabaseAndTable();

           
            InsertStudent("Иван Иванов", "Группа1", 85);
            InsertStudent("Мария Петрова", "Группа2", 92);

           
            Console.WriteLine("Студенты из Группа1:");
            SelectStudentsByGroup("Группа1");

            Console.WriteLine("\nСтуденты с оценкой выше 80:");
            SelectStudentsByGrade(80);

            Console.WriteLine("\nСтуденты, имя которых содержит 'Мар':");
            SelectStudentsByNamePart("Мар");

            Console.WriteLine("\nОбновим оценку студента с Id=1 на 90");
            UpdateStudentGrade(1, 90);

            Console.WriteLine("\nПереведём студентов из Группа2 в Группа3");
            UpdateStudentsGroup("Группа2", "Группа3");

            Console.WriteLine("\nУдалим студента с Id=2");
            DeleteStudentById(2);

            Console.WriteLine("\nУдалим студентов с оценкой ниже 50");
            DeleteStudentsByGradeThreshold(50);

            Console.WriteLine("\nКоличество студентов в Группа1 с оценкой выше 70:");
            int count = CountStudentsByGroupAndGrade("Группа1", 70);
            Console.WriteLine(count);
        }

        static void CreateDatabaseAndTable()
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = @"
            CREATE TABLE IF NOT EXISTS students (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                GroupName TEXT NOT NULL,
                Grade INTEGER NOT NULL CHECK(Grade BETWEEN 0 AND 100)
            );";

            using var cmd = new SQLiteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        
        static void InsertStudent(string name, string groupName, int grade)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "INSERT INTO students (Name, GroupName, Grade) VALUES (@name, @group, @grade);";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@group", groupName);
            cmd.Parameters.AddWithValue("@grade", grade);

            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine($"Добавлен студент: {name}, группа: {groupName}, оценка: {grade}");
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"Ошибка вставки: {ex.Message}");
            }
        }

        
        static void SelectStudentsByGroup(string groupName)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "SELECT * FROM students WHERE GroupName = @group;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@group", groupName);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Group: {reader["GroupName"]}, Grade: {reader["Grade"]}");
            }
        }

        
        static void SelectStudentsByGrade(int minGrade)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "SELECT * FROM students WHERE Grade > @grade;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@grade", minGrade);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Group: {reader["GroupName"]}, Grade: {reader["Grade"]}");
            }
        }

        
        static void SelectStudentsByNamePart(string namePart)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "SELECT * FROM students WHERE Name LIKE @namePart;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@namePart", $"%{namePart}%");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"Id: {reader["Id"]}, Name: {reader["Name"]}, Group: {reader["GroupName"]}, Grade: {reader["Grade"]}");
            }
        }

        
        static void UpdateStudentGrade(int id, int newGrade)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "UPDATE students SET Grade = @grade WHERE Id = @id;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@grade", newGrade);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "Оценка обновлена." : "Студент не найден.");
        }

       
        static void UpdateStudentsGroup(string oldGroup, string newGroup)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "UPDATE students SET GroupName = @newGroup WHERE GroupName = @oldGroup;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@newGroup", newGroup);
            cmd.Parameters.AddWithValue("@oldGroup", oldGroup);

            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"Обновлено групп у {rows} студентов.");
        }

        static void DeleteStudentById(int id)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "DELETE FROM students WHERE Id = @id;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine(rows > 0 ? "Студент удалён." : "Студент не найден.");
        }

       
        static void DeleteStudentsByGradeThreshold(int threshold)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "DELETE FROM students WHERE Grade < @threshold;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@threshold", threshold);

            int rows = cmd.ExecuteNonQuery();
            Console.WriteLine($"Удалено {rows} студентов с оценкой ниже {threshold}.");
        }

        
        static int CountStudentsByGroupAndGrade(string groupName, int gradeThreshold)
        {
            using var conn = new SQLiteConnection(connectionString);
            conn.Open();

            string sql = "SELECT COUNT(*) FROM students WHERE GroupName = @group AND Grade > @grade;";
            using var cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.AddWithValue("@group", groupName);
            cmd.Parameters.AddWithValue("@grade", gradeThreshold);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}
