using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Enities;

namespace BUS
{
    public class MajorService
    {
        public List<Major> GetAllByFaculty(int facultyId)
        {
            StudentModel context = new StudentModel();
            return context.Majors.Where(p=>p.FacultyID == facultyId).ToList();  
        }
    }
}
