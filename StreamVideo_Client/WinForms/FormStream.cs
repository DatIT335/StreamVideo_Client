using StreamVideo_Client.Network;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace StreamVideo_Client.WinForms
{
    public partial class FormStream : Form
    {
        private TcpClientManager _clientManager;
        private FlowLayoutPanel _videoGrid;
        private PictureBox _pbServerScreen;

        // Biến lưu trữ
        private bool _dangGhiHinh = true;
        private string _folderLuu;

        public FormStream(TcpClientManager clientManager)
        {
            _clientManager = clientManager;
            InitUI();

            // Đăng ký sự kiện
            _clientManager.OnVideoFrameReceived += HienThiAnh;
        }

        private void InitUI()
        {
            // Tạo thư mục lưu video
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _folderLuu = Path.Combine(documents, "StreamRecordings", DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            Directory.CreateDirectory(_folderLuu);

            // Cấu hình Form
            this.Text = "Phòng họp trực tuyến - Đang Ghi Hình...";
            this.BackColor = Color.FromArgb(32, 33, 36);
            this.WindowState = FormWindowState.Maximized;

            _videoGrid = new FlowLayoutPanel();
            _videoGrid.Dock = DockStyle.Fill;
            _videoGrid.BackColor = Color.FromArgb(32, 33, 36);
            this.Controls.Add(_videoGrid);

            _pbServerScreen = new PictureBox();
            _pbServerScreen.Width = 800;
            _pbServerScreen.Height = 450;
            _pbServerScreen.BackColor = Color.Black;
            _pbServerScreen.SizeMode = PictureBoxSizeMode.Zoom;
            _pbServerScreen.Margin = new Padding(20);
            _pbServerScreen.BorderStyle = BorderStyle.FixedSingle;
            _videoGrid.Controls.Add(_pbServerScreen);

            // Nút Kết thúc
            Panel pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 80;
            pnlBottom.BackColor = Color.FromArgb(20, 20, 20);
            this.Controls.Add(pnlBottom);

            Button btnEnd = new Button();
            btnEnd.Text = "KẾT THÚC";
            btnEnd.BackColor = Color.Red;
            btnEnd.ForeColor = Color.White;
            btnEnd.Size = new Size(120, 40);
            btnEnd.Location = new Point((Screen.PrimaryScreen.Bounds.Width / 2) - 60, 20);
            btnEnd.Click += BtnEnd_Click;
            pnlBottom.Controls.Add(btnEnd);
        }

        private void HienThiAnh(byte[] imgData)
        {
            try
            {
                if (InvokeRequired) { Invoke(new Action<byte[]>(HienThiAnh), imgData); return; }

                using (MemoryStream ms = new MemoryStream(imgData))
                {
                    Image newImg = Image.FromStream(ms);
                    Image oldImg = _pbServerScreen.Image;
                    _pbServerScreen.Image = newImg;
                    if (oldImg != null) oldImg.Dispose();
                }

                // LƯU HÌNH ẢNH
                if (_dangGhiHinh)
                {
                    string filename = Path.Combine(_folderLuu, $"Frame_{DateTime.Now.Ticks}.jpg");
                    File.WriteAllBytesAsync(filename, imgData);
                }
            }
            catch { }
        }

        private void BtnEnd_Click(object sender, EventArgs e)
        {
            _dangGhiHinh = false;
            MessageBox.Show($"Cuộc gọi kết thúc.\nDữ liệu đã lưu tại:\n{_folderLuu}", "Thông báo");
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_clientManager != null)
            {
                _clientManager.OnVideoFrameReceived -= HienThiAnh;
                _clientManager.Disconnect();
            }
        }
    }
}