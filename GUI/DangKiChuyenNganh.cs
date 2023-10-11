using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BUS;
using DAL.Enities;

namespace GUI
{
    public partial class DangKiChuyenNganh : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        private readonly MajorService majorService = new MajorService();
        public QuanLySinhVien form;
        public DangKiChuyenNganh()
        {
            InitializeComponent();
        }

        private void DangKiChuyenNganh_Load(object sender, EventArgs e)
        {
            try {
                var listFacultys = facultyService.GetAll();
                FillFacultyCombobox(listFacultys);
                    
             }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void FillFacultyCombobox(List<Faculty> listFacultys)
        {
            this.cmbFaculty.DataSource = listFacultys;
            this.cmbFaculty.ValueMember = "FacultyID";
            this.cmbFaculty.DisplayMember = "FacultyName";
        }
        private void FillMajorCombobox(List<Major> listMajors)
        {
            this.cmbMajor.DataSource = listMajors;
            this.cmbMajor.ValueMember = "MajorID";
            this.cmbMajor.DisplayMember = "MajorName";
        }
        private void cmbFaculty_SelectedIndexChanged(object sender, EventArgs e)
        {
            Faculty selectedFaculty = cmbFaculty.SelectedItem as Faculty;
            if (selectedFaculty != null)
            {
                var listMajor = majorService.GetAllByFaculty(selectedFaculty.FacultyID);
                FillMajorCombobox(listMajor);
                var listStudents = studentService.GetAllHasNoMajor(selectedFaculty.FacultyID);
                BindGrid(listStudents);
            }
        }

        private void BindGrid(List<Student> listStudent)
        {
            dgvStudent.Rows.Clear();
            foreach(var item in listStudent)
            {
                int index = dgvStudent.Rows.Add();
                dgvStudent.Rows[index].Cells[1].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[2].Value = item.FullName;
                if (item.Faculty != null)
                    dgvStudent.Rows[index].Cells[3].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[4].Value = item.AvarageScore.ToString();
                if(item.MajorID != null)
                    dgvStudent.Rows[index].Cells[5].Value = item.Major.MajorName.ToString();    
            }
        }

        private void btnDangKi_Click(object sender, EventArgs e)
        {
            try {

            for (int i = 0; i < dgvStudent.RowCount; i++)
            {
                DataGridViewRow row = dgvStudent.Rows[i];
                    if (row.Cells[0].Value != null)
                    {
                        Student student = studentService.FindByID(row.Cells[1].Value.ToString());
                       student.MajorID = Convert.ToInt32(cmbMajor.SelectedValue);
                        studentService.InsertUpdate(student);

                    }
                    MessageBox.Show("Đăng kí cn thành công.");
                    form.QuanLySinhVien_Load(sender, e);
                    this.Close();
                }
             }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);    
            }
            
        }
    }


}
