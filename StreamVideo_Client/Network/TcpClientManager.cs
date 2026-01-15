using StreamVideo_Client.Client.Security; // Namespace chứa AesHelper bên Client
using StreamVideo_Client.Common;
using StreamVideo_Client.DTO;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StreamVideo_Client.DTO;

namespace StreamVideo_Client.Network
{
    public class TcpClientManager
    {
        private TcpClient _client;
        private SslStream _sslStream;
        private BinaryReader _reader;
        private BinaryWriter _writer;

        // Sự kiện để báo UI cập nhật ảnh
        public event Action<byte[]> OnVideoFrameReceived;

        public async Task KetNoiAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            // Bypass SSL check cho demo
            _sslStream = new SslStream(_client.GetStream(), false, (s, c, ch, e) => true);
            await _sslStream.AuthenticateAsClientAsync("Server");

            _reader = new BinaryReader(_sslStream);
            _writer = new BinaryWriter(_sslStream);

            // Bắt đầu luồng lắng nghe dữ liệu từ server
            _ = Task.Run(ListenLoop);
        }

        private void ListenLoop()
        {
            try
            {
                while (_client.Connected)
                {
                    // 1. Đọc Header
                    int length = _reader.ReadInt32();
                    byte type = _reader.ReadByte();
                    byte[] payload = _reader.ReadBytes(length);

                    if (type == 1) // JSON response (Login)
                    {
                        string json = Encoding.UTF8.GetString(payload);
                        // Xử lý login response (Cần cơ chế signal/await cho hàm DangNhapAsync)
                        // Để đơn giản, ta lưu kết quả vào biến tạm hoặc event
                        _lastLoginResponse = JsonSerializer.Deserialize<LoginResponseDTO>(json);
                        _loginWaitHandle.Set(); // Báo hiệu đã nhận
                    }
                    else if (type == 2) // Binary Video (AES Encrypted)
                    {
                        // 2. Giải mã AES
                        byte[] imageBytes = AesHelper.Decrypt(payload);

                        // 3. Bắn sự kiện ra ngoài UI
                        OnVideoFrameReceived?.Invoke(imageBytes);
                    }
                }
            }
            catch { }
        }

        // Cơ chế đợi phản hồi Login đồng bộ
        private AutoResetEvent _loginWaitHandle = new AutoResetEvent(false);
        private LoginResponseDTO _lastLoginResponse;

        public async Task<LoginResponseDTO> DangNhapAsync(string user, string pass)
        {
            var login = new LoginRequestDTO { TenDangNhap = user, MatKhau = pass };
            var baseReq = new BaseRequestDTO { Type = RequestType.LOGIN, Payload = JsonSerializer.Serialize(login) }; // Copy file RequestType và BaseRequestDTO sang client nhé

            string json = JsonSerializer.Serialize(baseReq);
            byte[] data = Encoding.UTF8.GetBytes(json);

            // Gửi gói tin loại 1
            lock (_writer)
            {
                _writer.Write(data.Length);
                _writer.Write((byte)1); // Type JSON
                _writer.Write(data);
                _writer.Flush();
            }

            // Đợi phản hồi từ luồng ListenLoop
            return await Task.Run(() =>
            {
                _loginWaitHandle.WaitOne(5000); // Timeout 5s
                return _lastLoginResponse ?? new LoginResponseDTO { ThanhCong = false, ThongBao = "Timeout" };
            });
        }
    }
}