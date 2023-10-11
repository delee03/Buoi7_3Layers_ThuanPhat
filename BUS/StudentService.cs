using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using DAL.Enities;

using System.Drawing;
using System.Xml.Linq;

namespace BUS
{
    public class StudentService
    {
        public List<Student> GetAll()
        {
            StudentModel context = new StudentModel();
            return context.Students.ToList();
        }
        public List<Student> GetAllHasNoMajor()
        {
            StudentModel context = new StudentModel();
            return context.Students.Where(c=>c.MajorID ==null).ToList();
        }
      

        public List<Student> GetAllHasNoMajor(int facultyID) {
            StudentModel context = new StudentModel();
            return context.Students.Where(c=>c.MajorID == null && c.FacultyID == facultyID).ToList();
            
        }

        public Student FindByID(string  id)
        {
            StudentModel context = new StudentModel();
            
           return context.Students.FirstOrDefault(c => c.StudentID == id);
             
           
        }

        public void InsertAdd(Student s)
        {
            StudentModel context = new StudentModel();
            context.Students.Add(s);
            context.SaveChanges();
        }
        public void InsertUpdate(Student s)
        {
            StudentModel context = new StudentModel();
            context.Students.AddOrUpdate(s);
            context.SaveChanges();
        }
        public void DeleteById(string studentId)
        {
            StudentModel context = new StudentModel();
            Student student = context.Students.FirstOrDefault(p => p.StudentID == studentId);
            if (student == null)
                throw new System.Exception("Không tìm thấy MSSV cần xóa");
            context.Students.Remove(student);
            context.SaveChanges();
        }

    }
}
