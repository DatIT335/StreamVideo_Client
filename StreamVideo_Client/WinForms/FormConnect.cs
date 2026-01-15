using StreamVideo_Client.DTO;
using StreamVideo_Client.Network;
using System;
using System.Windows.Forms;

namespace StreamVideo_Client.WinForms
{
    public partial class FormConnect : Form
    {
        private TcpClientManager _client;

        public FormConnect()
        {
            InitializeComponent();
            Text = "Client - Đăng nhập hệ thống";
        }

        private async void btnLogin_Click(object sender, EventArgs e)
{
    try
    {
        btnLogin.Enabled = false;

        string ip = txtIP.Text.Trim();
        int port = int.Parse(txtPort.Text.Trim());
        string user = txtUser.Text.Trim();
        string pass = txtPass.Text.Trim();

        _client = new TcpClientManager();
        await _client.KetNoiAsync(ip, port);

        // Khai báo biến ketQua ở đây
        LoginResponseDTO ketQua = await _client.DangNhapAsync(user, pass);

        MessageBox.Show(ketQua.ThongBao);

        if (ketQua.ThanhCong)
        {
            // Mở form stream
            // Lưu ý: Đảm bảo bạn đã sửa lỗi FormStream bên dưới trước
            FormStream frm = new FormStream(_client); 
            this.Hide();
            frm.ShowDialog();
            this.Close();
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show("Lỗi: " + ex.Message);
    }
    finally
    {
        btnLogin.Enabled = true;
    }
}

    }
}
