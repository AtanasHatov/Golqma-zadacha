using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1
{
    public class Request
    {
        private string connectionString = "@\"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False\"";
        public void ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddRange(parameters);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine(reader[0]);
                        }
                    }
                }
            }
        }
        public void GetStudentsFromClass(string classNumber, string classLetter) => ExecuteQuery(
            "SELECT full_name FROM students WHERE class_id IN (SELECT id FROM classes WHERE class_number = @classNumber AND class_letter = @classLetter)",
            new SqlParameter("@classNumber", classNumber),
             new SqlParameter("@classLetter", classLetter));

        public void GetTeachersBySubjects() => ExecuteQuery(
            "SELECT subjects.title, teachers.full_name FROM teachers " +
            "JOIN teachers_subjects ON teachers.id = teachers_subjects.teacher_id " +
            "JOIN subjects ON teachers_subjects.subject_id = subjects.id ORDER BY subjects.title");

        public void GetClassesAndTeachers() => ExecuteQuery(
            "SELECT class_number, class_letter, teachers.full_name FROM classes " +
            "JOIN teachers ON classes.class_teacher_id = teachers.id");

        public void GetSubjectsTeacherCount() => ExecuteQuery(
            "SELECT subjects.title, COUNT(teachers_subjects.teacher_id) AS teacher_count FROM subjects " +
            "LEFT JOIN teachers_subjects ON subjects.id = teachers_subjects.subject_id GROUP BY subjects.title");

        public void GetLargeClassrooms() => ExecuteQuery(
            "SELECT id, capacity FROM classrooms WHERE capacity > 26 ORDER BY floor");

        public void GetAllStudentsGroupedByClass() => ExecuteQuery(
            "SELECT classes.class_number, classes.class_letter, students.full_name FROM students " +
            "JOIN classes ON students.class_id = classes.id ORDER BY classes.class_number, classes.class_letter");

        public void GetStudentsByBirthDate(string birthDate) => ExecuteQuery(
            "SELECT full_name FROM students WHERE date_of_birth = @birthDate",
            new SqlParameter("@birthDate", birthDate));

        public void GetSubjectsByStudent(string studentName) => ExecuteQuery(
            "SELECT COUNT(*) FROM classes_subjects WHERE class_id = " +
            "(SELECT class_id FROM students WHERE full_name = @studentName)",
            new SqlParameter("@studentName", studentName));

        public void GetTeachersByStudent(string studentName) => ExecuteQuery(
            "SELECT teachers.full_name, subjects.title FROM teachers " +
            "JOIN teachers_subjects ON teachers.id = teachers_subjects.teacher_id " +
            "JOIN subjects ON teachers_subjects.subject_id = subjects.id " +
            "JOIN classes_subjects ON subjects.id = classes_subjects.subject_id " +
            "WHERE classes_subjects.class_id = (SELECT class_id FROM students WHERE full_name = @studentName)",
            new SqlParameter("@studentName", studentName));

        public void GetClassesByParentEmail(string parentEmail) => ExecuteQuery(
            "SELECT classes.class_number, classes.class_letter FROM classes " +
            "JOIN students ON classes.id = students.class_id " +
            "JOIN students_parents ON students.id = students_parents.student_id " +
            "JOIN parents ON students_parents.parent_id = parents.id " +
            "WHERE parents.email = @parentEmail",
            new SqlParameter("@parentEmail", parentEmail));
    }
}
