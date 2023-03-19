using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace THiGK
{
    public partial class Form2 : Form
    {
        private int _masp;
        // add
        public Form2()
        {
            InitializeComponent();
            LoadMatHangSX();
            btnOk.Click += new System.EventHandler(this.btnOkADD_Click);
        }

        // update
        public Form2(int masp)
        {
            _masp = masp;
            InitializeComponent();
            InitUpdate();
        }

        // load data update
        public void InitUpdate()
        {
            txtMaSP.Enabled = false;    
            btnOk.Click += new System.EventHandler(this.btnOk_ClickEdit);
            
            LoadMatHangSX();
            btnOk.Click += new System.EventHandler(this.btnOkADD_Click);

            

            // load sv
            var table = DBHelper.Instance.GetAllRecord(new SqlCommand($"SELECT sp.MaSanPham, sp.TenSanPham, mh.nhasanxuat, sp.NgayNhapHang, mh.tenmathang, sp.tinhtrang " +
                $"FROM MatHang as MH join SanPham as SP\r\non MH.MaMatHang = SP.MaMatHang\r\n where sp.MaSanPham= {_masp}"));


            var sp = table.Rows[0];

            txtMaSP.Text = sp["masanpham"].ToString();
            txtTenSP.Text = sp["Tensanpham"].ToString();
            cbMatHang.SelectedIndex = cbMatHang.Items.IndexOf(sp["tenmathang"].ToString());
            

            string query = $"SELECT NhaSanXuat FROM mathang WHERE tenmathang = N'{sp["tenmathang"].ToString()}';";
            cbNhaSX.DataSource = DBHelper.Instance.GetList(query);

            if ((bool)(sp["tinhtrang"]))
            {
                rbtConHang.Checked = true;
            }
            else
            {
                rbtHetHang.Checked = true;
            }
           
            dtNgayNhap.Text = sp["ngaynhaphang"].ToString();

        }

        // Load mat hang SX
        private void LoadMatHangSX()
        {
            rbtConHang.Checked=true;
            cbMatHang.DataSource= DBHelper.Instance.GetList("SELECT tenmathang from mathang");
            cbNhaSX.DataSource = DBHelper.Instance.GetList("SELECT NhaSanXuat from mathang");
        }

        private void cbMatHang_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tenMH = cbMatHang.SelectedItem as string;     
            string query = $"SELECT NhaSanXuat FROM mathang WHERE tenmathang = N'{tenMH}';";
            cbNhaSX.DataSource = DBHelper.Instance.GetList(query);

        }

        private void btnOkADD_Click(object sender, EventArgs e)
        {
            string maSp = txtMaSP.Text;
            string tenSp = txtTenSP.Text;
            

            var ngaySx = Convert.ToDateTime(dtNgayNhap.Value.ToShortDateString());

            if (maSp != "" && tenSp != "")
            {
                try
                {
                    int maSanpham = Int32.Parse(maSp);
                    // ktra ma sp co ton tai hay khong
                    int? masp = DBHelper.Instance.ExcuteSacarla($"SELECT masanpham from sanpham where MaSanPham = {maSanpham}");
                    if(masp !=null)
                    {
                        MessageBox.Show("Mã sản phẩm này đã tồn tại");
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("INSERT INTO SanPham (MaSanPham, TenSanPham, NgayNhapHang, TinhTrang, MaMatHang)" +
                            $"VALUES   (@MaSP, @TenSP, @NgaySX, @TinhTrang, @MaMH)");

                        cmd.Parameters.AddWithValue("@MaSP", maSanpham);

                        cmd.Parameters.AddWithValue("@TenSP", tenSp);

                        cmd.Parameters.AddWithValue("@NgaySX", ngaySx);
                        
                        cmd.Parameters.AddWithValue("@TinhTrang", rbtConHang.Checked?1:0);
                      

                        int ?mahang = DBHelper.Instance.ExcuteSacarla($"SELECT MaMatHang FROM mathang WHERE tenmathang = N'{cbMatHang.SelectedItem.ToString()}';");
                        cmd.Parameters.AddWithValue("@MaMH", mahang);

                        DBHelper.Instance.ExcueteUpdateDB(cmd);
                        this.Dispose();
                        Form1.Instance.btnSearch_Click(sender,e);
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show("Mã sản phẩm phải là số");
                }
                
            }

        }

        private void btnOk_ClickEdit(object sender, EventArgs e)
        {
            var masp = txtMaSP.Text;
            var tensp = txtTenSP.Text;
            var ngaynhap = Convert.ToDateTime(dtNgayNhap.Value.ToShortDateString());

            SqlCommand cmd = new SqlCommand($"update SanPham set TenSanPham = @TenSP,NgayNhapHang =@NgayNhap, Mamathang =@MaMH, TinhTrang =@TinhTrang  where masanpham= {_masp}");

         

            cmd.Parameters.AddWithValue("@TenSP", tensp);

            int ?mahang = DBHelper.Instance.ExcuteSacarla($"SELECT MaMatHang FROM mathang WHERE tenmathang = N'{cbMatHang.SelectedItem.ToString()}';");
            cmd.Parameters.AddWithValue("@MaMH", mahang);


            cmd.Parameters.AddWithValue("@NgayNhap", ngaynhap);

            cmd.Parameters.AddWithValue("@TinhTrang", rbtConHang.Checked ? 1 : 0);

            DBHelper.Instance.ExcueteUpdateDB(cmd);
            this.Dispose();
            Form1.Instance.btnSearch_Click(sender, e);

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
