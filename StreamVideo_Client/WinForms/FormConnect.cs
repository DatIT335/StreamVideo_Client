using StreamVideo_Client.Network;
using StreamVideo_Client.DTO;
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

                LoginResponseDTO ketQua =
                    await _client.DangNhapAsync(user, pass);

                MessageBox.Show(ketQua.ThongBao);

                if (ketQua.ThanhCong)
                {
                    Form1 frm = new Form1();
                    frm.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đăng nhập: " + ex.Message);
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
