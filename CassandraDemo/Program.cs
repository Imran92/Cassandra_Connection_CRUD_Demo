using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassandra;
using attr = Cassandra.Mapping.Attributes;
using Cassandra.Data.Linq;
using Cassandra.Mapping;

namespace CassandraDemo
{
    public class Student
    {
        [attr.PartitionKey(0)]
        public int Class { get; set; }
        [attr.ClusteringKey(1)]
        public string Section { get; set; }
        [attr.ClusteringKey(2)]
        public int Roll { get; set; }
        [attr.ClusteringKey(3)]
        public string FirstName { get; set; }
        [attr.ClusteringKey(4)]
        public string SurName { get; set; }
        public DateTime DOB { get; set; }
        [attr.Frozen]
        public List<Subject> Subjects { get; set; }
    }
    public class Subject
    {
        public int Id { get; set; }
        public string SubjectName { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var cluster = Cluster.Builder().AddContactPoints("127.0.0.1").Build();
            var session = cluster.Connect("mykeyspace");

            session.UserDefinedTypes.Define(UdtMap.For<Subject>());

            var studentTable = new Table<Student>(session);
            studentTable.CreateIfNotExists();

            var stdnt = new Student
            {
                Class = 5,
                Section = "A",
                Roll = 3,
                FirstName = "Moltu",
                SurName = "Miya",
                DOB = DateTime.UtcNow,
                Subjects = new List<Subject>
                {
                    new Subject { Id = 1, SubjectName = "Ban"},
                    new Subject { Id = 2, SubjectName = "Eng"}
                }
            };

            var queryObject = session.GetTable<Student>().Insert(stdnt);
            session.Execute(queryObject);

            var mapper = new Mapper(session);
            var res = mapper.Fetch<Student>(string.Format("select * from student where class = {0}", 5)).ToList();
        }
    }
}
