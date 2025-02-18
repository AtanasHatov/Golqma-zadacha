// See https://aka.ms/new-console-template for more information
using Microsoft.Data.SqlClient;
using Task1;
SqlConnection connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
connection.Open();
using (connection)
{
    string checkDbQuery = "IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SchoolDB') CREATE DATABASE SchoolDB;";
    ExecuteQuery(connection, checkDbQuery);
    Console.WriteLine("Базата данни е създадена!");

    connection.ChangeDatabase("SchoolDB");

    CreateTables(connection);
    Console.WriteLine("Таблиците са създадени!");

    InsertTeachers(connection);
    InsertClassrooms(connection);
    InsertClasses(connection);
    InsertStudents(connection);
    InsertParents(connection);

    Request request = new Request();
    while (true)
    {
        Console.WriteLine("\nИзбери заявка за изпълнение:");
        Console.WriteLine("1. Имена на ученици от 11Б клас");
        Console.WriteLine("2. Учители и предмети");
        Console.WriteLine("3. Класове и класни ръководители");
        Console.WriteLine("4. Брой учители по предмети");
        Console.WriteLine("5. Класни стаи с капацитет над 26 ученици");
        Console.WriteLine("6. Ученици, групирани по класове");
        Console.WriteLine("7. Ученици от въведен клас");
        Console.WriteLine("8. Ученици, родени на въведена дата");
        Console.WriteLine("9. Брой предмети на ученик");
        Console.WriteLine("10. Учители и предмети на ученик");
        Console.WriteLine("11. Класове на децата на родител (по имейл)");
        Console.WriteLine("0. Изход");
        Console.Write("Въведи номер на заявката: ");

        string choice = Console.ReadLine();
        Console.WriteLine();

        switch (choice)
        {
            case "1":
                request.GetStudentsFromClass("11", "Б");
                break;
            case "2":
                request.GetTeachersBySubjects();
                break;
            case "3":
                request.GetClassesAndTeachers();
                break;
            case "4":
                request.GetSubjectsTeacherCount();
                break;
            case "5":
                request.GetLargeClassrooms();
                break;
            case "6":
                request.GetAllStudentsGroupedByClass();
                break;
            case "7":
                Console.Write("Въведи номер на клас: ");
                string classNumber = Console.ReadLine();
                Console.Write("Въведи буква на клас: ");
                string classLetter = Console.ReadLine();
                request.GetStudentsFromClass(classNumber, classLetter);
                break;
            case "8":
                Console.Write("Въведи дата на раждане (гггг-мм-дд): ");
                string birthDate = Console.ReadLine();
                request.GetStudentsByBirthDate(birthDate);
                break;
            case "9":
                Console.Write("Въведи име на ученик: ");
                string studentName1 = Console.ReadLine();
                request.GetSubjectsByStudent(studentName1);
                break;
            case "10":
                Console.Write("Въведи име на ученик: ");
                string studentName2 = Console.ReadLine();
                request.GetTeachersByStudent(studentName2);
                break;
            case "11":
                Console.Write("Въведи имейл на родител: ");
                string parentEmail = Console.ReadLine();
                request.GetClassesByParentEmail(parentEmail);
                break;
            case "0":
                Console.WriteLine("Изход...");
                return;
            default:
                Console.WriteLine("Невалиден избор, опитайте отново.");
                break;
        }
    }
    static void CreateTables(SqlConnection connection)
    {
        ExecuteQuery(connection, @"CREATE TABLE teachers(
            id INT PRIMARY KEY IDENTITY (1,1),
            teacher_code NVARCHAR(10) UNIQUE NOT NULL,
            full_name NVARCHAR(100) NOT NULL,
            gender NVARCHAR(10),
            date_of_birth DATE,
            email NVARCHAR(100),
            phone NVARCHAR(20),
            working_days NVARCHAR(50)
 );");

        ExecuteQuery(connection, @"CREATE TABLE subjects (
        id INT PRIMARY KEY IDENTITY(1,1),
        title NVARCHAR(100) NOT NULL
    );");

        ExecuteQuery(connection, @"CREATE TABLE classrooms (
        id INT PRIMARY KEY IDENTITY(1,1),
        floor INT NOT NULL,
        capacity INT NOT NULL,
        description NVARCHAR(600)
    );");

        ExecuteQuery(connection, @"CREATE TABLE classes (
        id INT PRIMARY KEY IDENTITY(1,1),
        class_number INT NOT NULL,
        class_letter NVARCHAR(1) NOT NULL,
        class_teacher_id INT,
        classroom_id INT,
        FOREIGN KEY (class_teacher_id) REFERENCES teachers(id),
        FOREIGN KEY (classroom_id) REFERENCES classrooms(id)
    );");

        ExecuteQuery(connection, @"CREATE TABLE students (
            id INT PRIMARY KEY IDENTITY(1,1),
            student_code NVARCHAR(10) UNIQUE NOT NULL,
            full_name NVARCHAR(100) NOT NULL,
            gender NVARCHAR(10),
            date_of_birth DATE,
            email NVARCHAR(100),
            phone NVARCHAR(20),
            class_id INT FOREIGN KEY REFERENCES classes(id),
            is_active BOOLEAN DEFAULT TRUE
);");

        ExecuteQuery(connection, @"CREATE TABLE parents (
            id INT PRIMARY KEY IDENTITY(1,1),
            parent_code NVARCHAR(10) UNIQUE NOT NULL,
            full_name NVARCHAR(100) NOT NULL,
            email NVARCHAR(100),
            phone VARCHAR(20)
);");

        ExecuteQuery(connection, @"CREATE TABLE teachers_subjects (
            id INT PRIMARY KEY IDENTITY(1,1),
            teacher_id INT FOREIGN KEY REFERENCES teachers(id),
            subject_id INT FOREIGN KEY REFERENCES subjects(id)
);");

        ExecuteQuery(connection, @"CREATE TABLE classes_subjects (
        id INT PRIMARY KEY IDENTITY(1,1),
        class_id INT FOREIGN KEY REFERENCES classes(id),
        subject_id INT FOREIGN KEY REFERENCES subjects(id)
    );");

        ExecuteQuery(connection, @"CREATE TABLE \students_parents (
         id INT PRIMARY KEY IDENTITY(1,1),
         student_id INT FOREIGN KEY REFERENCES students(id),
         parent_id  INT FOREIGN KEY REFERENCES parents(id)
     );");
    }

    static void InsertTeachers(SqlConnection connection)
    {
        for (int i = 0; i < 5; i++)
        {
            Console.Write("Въведете teacher_code: ");
            string teacherCode = Console.ReadLine();

            Console.Write("Въведете име на учителя: ");
            string fullName = Console.ReadLine();

            Console.Write("Въведете пол: ");
            string gender = Console.ReadLine();

            Console.Write("Въведете дата на раждане: ");
            string dateOfBirth = Console.ReadLine();

            Console.Write("Въведете email: ");
            string email = Console.ReadLine();

            Console.Write("Въведете телефон: ");
            string phone = Console.ReadLine();

            Console.Write("Въведете работни дни: ");
            string workingDays = Console.ReadLine();

            string insertTeacherQuery = @"INSERT INTO teachers (teacher_code, full_name, gender, date_of_birth, email, phone, working_days) 
                              VALUES (@teacher_code, @full_name, @gender, @date_of_birth, @email, @phone, @working_days);";

            using (SqlCommand cmd = new SqlCommand(insertTeacherQuery, connection))
            {
                cmd.Parameters.AddWithValue("@teacher_code", teacherCode);
                cmd.Parameters.AddWithValue("@full_name", fullName);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@date_of_birth", dateOfBirth);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@working_days", workingDays);
                cmd.ExecuteNonQuery();
            }

        }
    }

    static void InsertClassrooms(SqlConnection connection)
    {
        for (int i = 0; i < 5; i++)
        {
            Console.Write("Въведете етаж: ");
            int floor = int.Parse(Console.ReadLine());
            Console.Write("Въведете капацитет: ");
            int capacity = int.Parse(Console.ReadLine());
            Console.Write("Описание: ");
            string description = Console.ReadLine();

            string query = "INSERT INTO classrooms (floor, capacity, description) VALUES (@floor, @capacity, @description);";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@floor", floor);
                cmd.Parameters.AddWithValue("@capacity", capacity);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.ExecuteNonQuery();
            }
        }
    }

    static void InsertClasses(SqlConnection connection)
    {
        for (int i = 0; i < 5; i++)
        {
            Console.Write("Въведете номер на клас: ");
            int classNumber = int.Parse(Console.ReadLine());
            Console.Write("Въведете буква на класа: ");
            string classLetter = Console.ReadLine();

            string query = "INSERT INTO classes (class_number, class_letter, class_teacher_id, classroom_id) VALUES (@number, @letter, 1, 1);";

            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@number", classNumber);
                cmd.Parameters.AddWithValue("@letter", classLetter);
                cmd.ExecuteNonQuery();
            }
        }
    }

    static void InsertStudents(SqlConnection connection)
    {
        Console.Write("Въведете student_code: ");
        string studentCode = Console.ReadLine();

        Console.Write("Въведете име на ученика: ");
        string fullName = Console.ReadLine();

        Console.Write("Въведете пол: ");
        string gender = Console.ReadLine();

        Console.Write("Въведете дата на раждане: ");
        string dateOfBirth = Console.ReadLine();

        Console.Write("Въведете email: ");
        string email = Console.ReadLine();

        Console.Write("Въведете телефон: ");
        string phone = Console.ReadLine();

        Console.Write("Въведете ID на класа: ");
        int classId = int.Parse(Console.ReadLine());

        Console.Write("Активен ли е ученикът: ");
        bool isActive = Console.ReadLine() == "1";


        string insertStudentQuery = @"INSERT INTO students (student_code, full_name, gender, date_of_birth, email, phone, class_id, is_active) 
                              VALUES (@student_code, @full_name, @gender, @date_of_birth, @email, @phone, @class_id, @is_active);";

        using (SqlCommand cmd = new SqlCommand(insertStudentQuery, connection))
        {
            cmd.Parameters.AddWithValue("@student_code", studentCode);
            cmd.Parameters.AddWithValue("@full_name", fullName);
            cmd.Parameters.AddWithValue("@gender", gender);
            cmd.Parameters.AddWithValue("@date_of_birth", dateOfBirth);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@class_id", classId);
            cmd.Parameters.AddWithValue("@is_active", isActive);
            cmd.ExecuteNonQuery();
        }
    }
    static void InsertParents(SqlConnection connection)
    {
        Console.Write("Въведете parent_code: ");
        string parentCode = Console.ReadLine();
        Console.Write("Въведете име на родителя: ");
        string fullName = Console.ReadLine();
        Console.Write("Въведете email: ");
        string email = Console.ReadLine();
        Console.Write("Въведете телефон: ");
        string phone = Console.ReadLine();


        string query = @"INSERT INTO parents (parent_code, full_name, email, phone) 
                                 VALUES (@parent_code, @full_name, @email, @phone);";

        using (SqlCommand cmd = new SqlCommand(query, connection))
        {
            cmd.Parameters.AddWithValue("@parent_code", parentCode);
            cmd.Parameters.AddWithValue("@full_name", fullName);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.ExecuteNonQuery();
        }
    }

    static void ExecuteQuery(SqlConnection connection, string query)
    {
        using (SqlCommand cmd = new SqlCommand(query, connection))
        {
            cmd.ExecuteNonQuery();
        }
    }
}