using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DAL.Enities;
using BUS;
using System.Runtime.CompilerServices;
using System.IO;
using System.Runtime.Remoting.Contexts;


namespace GUI
{
    
    public partial class QuanLySinhVien : Form
    {
        private readonly StudentService studentService = new StudentService();
        private readonly FacultyService facultyService = new FacultyService();
        StudentModel context = new StudentModel();
        private bool isUpdate = false;
        public QuanLySinhVien()
        {
            InitializeComponent();
        }
        public void LoadData()
        {
            try
            {
                setGridViewStyle(dgvStudent);
                var listFaculty = facultyService.GetAll();
                var listStudents = studentService.GetAll();
                FillFacultyComboBox(listFaculty);
                BindGrid(listStudents);
                cboFac.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void QuanLySinhVien_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void FillFacultyComboBox(List<Faculty> list)
        {
            list.Insert(0, new Faculty());
            this.cboFac.DataSource = list;
            this.cboFac.DisplayMember = "FacultyName";
            this.cboFac.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> list)
        {
            dgvStudent.Rows.Clear();
            foreach (var item in list)
            {
                int index = dgvStudent.Rows.Add();// cần search AI tìm hiểu về dòng này;
                dgvStudent.Rows[index].Cells[0].Value = item.StudentID;
                dgvStudent.Rows[index].Cells[1].Value = item.FullName;
                if(item.Faculty != null)    
                     dgvStudent.Rows[index].Cells[2].Value = item.Faculty.FacultyName;
                dgvStudent.Rows[index].Cells[3].Value = item.AvarageScore.ToString();
                if (item.MajorID != null)
                    dgvStudent.Rows[index].Cells[4].Value = item.Major.MajorName.ToString();
                ShowAvatar(item.Avatar);

            }
        }
        private void ShowAvatar(string imageName)
        {
            if (string.IsNullOrEmpty(imageName))
                picAvatar.Image = null;
            else
            {
                string parentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                string imagePath = Path.Combine(parentDirectory, "Images", imageName);
                picAvatar.Image = Image.FromFile(imagePath);
                picAvatar.Refresh();
            }    
        }
        public void setGridViewStyle(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.BackgroundColor = Color.White;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void checkBoxCN_CheckedChanged(object sender, EventArgs e)
        {
           
             var listStudent = new List<Student>();
             if (this.checkBoxCN.Checked)
                  listStudent = studentService.GetAllHasNoMajor();
             else
                 listStudent = studentService.GetAll();
             BindGrid(listStudent);
            
        }
        

        private void dgvStudent_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            isUpdate = true;
            if (dgvStudent.Rows.Count > 0)
            {
               DataGridViewRow row = dgvStudent.SelectedRows[0];
                txtID.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                cboFac.Text = row.Cells[2].Value.ToString();
                txtAg.Text = row.Cells[3].Value.ToString();
              
            }
        }
       
        private bool isNotNullValue()
        {
            if (txtID.Text == "")
                return false;
            if (txtName.Text == "")
                return false;
            if (txtAg.Text == "")
                return false;
            return true;
        }

        private bool isNumberic(string txtID)
        {
            bool checkNumb = long.TryParse(txtID, out long result);
            if (txtID.Contains(","))
                return false;
            return checkNumb;
        }

        private bool isScore(string txtID)
        {
            bool checkNumb = float.TryParse(txtID, out float result);
            if (txtID.Contains(","))
                return false;
            if (checkNumb)
            {
                if (float.Parse(txtID) > 10 || float.Parse(txtID) < 0)
                    return false;
                return true;
            }
            return false;
        }
        private void isValidInputData()
        {
            if (!isNotNullValue())
                throw new Exception("Vui lòng nhập đầy đủ thông tin.");
           /* if (txtID.Text.Length != 10)
                throw new Exception("Mã số sinh viên phải có 10 kí tự.");*/
            if (!isNumberic(txtID.Text))
                throw new Exception("Mã số sinh viên không bao gồm chữ.");
            if (!isScore(txtAg.Text))
                throw new Exception("Điểm không hợp lệ.");
        }
        
        private void btnRemove_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn có chắc muốn xóa ", "Xóa", MessageBoxButtons.OKCancel);
            if (rs == DialogResult.OK)
            {

               // var id = int.Parse(txtID.Text);
                Student s = context.Students.Where(r =>r.StudentID == txtID.Text && r.FullName == txtName.Text).SingleOrDefault();
                if (s != null)
                {
                    context.Students.Remove(s);
                    context.SaveChanges();
                    LoadData();

                }
            }
        }

        private void btnADDorUp_Click(object sender, EventArgs e)
        {
           
            try
            {
                isValidInputData();
                if (isUpdate)
                {
                    // Student student = new Student() { StudentID = txtID.Text, FullName = txtName.Text, FacultyID = Convert.ToInt32(cboFac.SelectedValue), AvarageScore = float.Parse(txtAg.Text) }; ;
                    var ID = dgvStudent.SelectedCells[0].OwningRow.Cells["Column1"].Value.ToString();
                    Student student = context.Students.Find(ID);
                    context.Students.Remove(student);
                    Student newst = new Student();
                    newst.StudentID = txtID.Text;
                    newst.FullName = txtName.Text;
                    newst.FacultyID = Convert.ToInt32(cboFac.SelectedValue);
                    newst.AvarageScore = float.Parse(txtAg.Text);
                    context.Students.Add(newst);
                    context.SaveChanges();
                    LoadData();
                    MessageBox.Show("Sửa thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Student student = new Student() { StudentID = txtID.Text, FullName = txtName.Text, FacultyID = Convert.ToInt32(cboFac.SelectedValue), AvarageScore = float.Parse(txtAg.Text) };
                    studentService.InsertAdd(student);
                    LoadData();
                    MessageBox.Show("Thêm thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } 
             isUpdate = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnAddImage_Click(object sender, EventArgs e)
        {
            String imageLocation = "";
            try
            {
                OpenFileDialog fileOpen = new OpenFileDialog();
                fileOpen.Title = "Chọn hình ảnh sinh viên";
                fileOpen.Filter = "Hình ảnh(*.jpg; *.jpeg; *.png)| *.jpg; *.jpeg; *.png | All files(*.*) | *.* ";
                if (fileOpen.ShowDialog() == DialogResult.OK)
                {
                    imageLocation = fileOpen.FileName;
                    picAvatar.Image = Image.FromFile(imageLocation);
                    ShowAvatar(imageLocation);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(" Lỗi không thể upload ảnh! ", " Lỗi ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }


        private void DangKiChuyenNganh_Click(object sender, EventArgs e)
        {
            DangKiChuyenNganh f = new DangKiChuyenNganh();
            f.Show();
            f.form = this;
        }

        private void loadẢnhToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult rs = MessageBox.Show("Bạn muốn thoát chương trình ?", "Thoát",
               MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (rs == DialogResult.OK)
            {
                Application.Exit();
            }
        }
    }
}
