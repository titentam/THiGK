using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace THiGK
{
    public partial class Form1 : Form
    {
        public static Form1 Instance {  get; private set; }
        public Form1()
        {
            Instance =this;
            InitializeComponent();
            LoadData();
        }

        // Load data
        private void LoadData()
        {
             
            grv.AutoGenerateColumns = false;
            grv.Columns["STT"].DataPropertyName = "STT";
            grv.Columns["MaSanPham"].DataPropertyName = "MaSanPham";
            grv.Columns["TenSanPham"].DataPropertyName = "TenSanPham";
            grv.Columns["NhaSanXuat"].DataPropertyName = "NhaSanXuat";
            grv.Columns["NgayNhapHang"].DataPropertyName = "NgayNhapHang";
            grv.Columns["TenMatHang"].DataPropertyName = "TenMatHang";
            grv.Columns["TinhTrang"].DataPropertyName = "TinhTrang";
            
            string query = "SELECT sp.MaSanPham, sp.TenSanPham, mh.nhasanxuat, sp.NgayNhapHang, mh.tenmathang, sp.tinhtrang FROM MatHang as MH join SanPham as SP\r\non MH.MaMatHang = SP.MaMatHang";
            DataTable dt = DBHelper.Instance.GetAllRecord(new SqlCommand(query));
            grv.DataSource=dt;

            cbSort.SelectedIndex = 0;

        }

        public void btnSearch_Click(object sender, EventArgs e)
        {
            string tenSanPham = txtSearch.Text;
            string query = "SELECT sp.MaSanPham, sp.TenSanPham, mh.nhasanxuat, sp.NgayNhapHang, mh.tenmathang, sp.tinhtrang FROM MatHang as MH join SanPham as SP\r\non MH.MaMatHang = SP.MaMatHang\r\n" +
                $"WHERE sp.tensanpham like N'%{tenSanPham}%'";
            DataTable dt = DBHelper.Instance.GetAllRecord(new SqlCommand(query));
            grv.DataSource = dt;
        }

        private void btnSort_Click(object sender, EventArgs e)
        {
            string query;
            if(cbSort.SelectedIndex == 0) {
                // sort theo ten san pham
                query = "SELECT sp.MaSanPham, sp.TenSanPham, mh.nhasanxuat, sp.NgayNhapHang, mh.tenmathang, sp.tinhtrang " +
                    "FROM MatHang as MH join SanPham as SP\r\non MH.MaMatHang = SP.MaMatHang\r\norder by sp.TenSanPham"; 
            }
            else
            {
                // sort theo ngay nhap
                query = "SELECT sp.MaSanPham, sp.TenSanPham, mh.nhasanxuat, sp.NgayNhapHang, mh.tenmathang, sp.tinhtrang " +
                    "FROM MatHang as MH join SanPham as SP\r\non MH.MaMatHang = SP.MaMatHang\r\norder by sp.NgayNhapHang"; 
            }
            DataTable dt = DBHelper.Instance.GetAllRecord(new SqlCommand(query));
            grv.DataSource = dt;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(grv.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Bạn có muốn xoá sản phẩm không", "Xác nhận xoá", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    string query;
                    foreach (DataGridViewRow row in grv.SelectedRows)
                    {
                        int maSp = (int)(row.Cells[1].Value);
                        query = $"DELETE from SanPham WHERE MaSanPham = {maSp};";
                        DBHelper.Instance.ExcueteUpdateDB(new SqlCommand(query));
                    }
                    query = "SELECT sp.MaSanPham, sp.TenSanPham, mh.nhasanxuat, sp.NgayNhapHang, mh.tenmathang, sp.tinhtrang" +
                        " FROM MatHang as MH join SanPham as SP\r\non MH.MaMatHang = SP.MaMatHang";
                    DataTable dt = DBHelper.Instance.GetAllRecord(new SqlCommand(query));
                    grv.DataSource = dt;
                }              
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.ShowDialog();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (grv.SelectedRows.Count == 1)
            {
                int mamathang = (int)grv.SelectedRows[0].Cells[1].Value;
                Form2 f2 = new Form2(mamathang);
                f2.ShowDialog();
            }

        }
    }
}
