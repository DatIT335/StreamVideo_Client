using StreamVideo_Client.Network;
using System;
using System.Drawing;
using System.IO; // <--- Đã thêm thư viện này để dùng MemoryStream
using System.Windows.Forms;

namespace StreamVideo_Client.WinForms
{
    public partial class FormStream : Form
    {
        private TcpClientManager _clientManager;

        // Đã sửa: Thêm tham số 'TcpClientManager clientManager' vào trong ngoặc
        public FormStream(TcpClientManager clientManager)
        {
            InitializeComponent();

            // Bây giờ 'clientManager' đã tồn tại (lấy từ tham số bên trên)
            _clientManager = clientManager;

            // Đăng ký sự kiện nhận ảnh
            _clientManager.OnVideoFrameReceived += HienThiAnh;
        }

        private void HienThiAnh(byte[] imgData)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<byte[]>(HienThiAnh), imgData);
                    return;
                }

                using (MemoryStream ms = new MemoryStream(imgData))
                {
                    Image oldImg = pbVideo.Image;
                    pbVideo.Image = Image.FromStream(ms);

                    if (oldImg != null) oldImg.Dispose();
                }
            }
            catch
            {
                // Bỏ qua lỗi frame
            }
        }

        // Quan trọng: Hủy đăng ký sự kiện khi tắt form để tránh lỗi ngầm
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (_clientManager != null)
            {
                _clientManager.OnVideoFrameReceived -= HienThiAnh;
            }
        }
    }
}
