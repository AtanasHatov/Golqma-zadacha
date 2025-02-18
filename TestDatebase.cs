using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class TestDatebase
    {
        private string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";
        [SetUp]
        public void Setup()
        {
        }
        [Test]
        public void Database_ShouldExist()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT DB_ID('SchoolDB')", con);
                Assert.IsNotNull(cmd.ExecuteScalar());
            }
        }

        [Test]
        public void TeachersTable_ShouldExist()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("SELECT OBJECT_ID('teachers')", con);
                Assert.IsNotNull(cmd.ExecuteScalar());
            }
        }

        [Test]
        public void InsertTeacher_ShouldWork()
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                var cmd = new SqlCommand("INSERT INTO teachers (teacher_code, full_name) VALUES ('T001', 'John Doe')", con);
                int rowsAffected = cmd.ExecuteNonQuery();
                Assert.AreEqual(1, rowsAffected);
            }
        }
    }
}
