using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using StreamVideo_Client.DTO;


namespace StreamVideo_Client.Network

{
    /// <summary>
    /// Quản lý kết nối TCP + SSL tới server
    /// </summary>
    public class TcpClientManager
    {
        private TcpClient _client;
        private SslStream _sslStream;

        public async Task KetNoiAsync(string ip, int port)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(ip, port);

            _sslStream = new SslStream(
                _client.GetStream(),
                false,
                (sender, cert, chain, error) => true // chấp nhận cert self-signed
            );

            await _sslStream.AuthenticateAsClientAsync("Server");
        }

        public async Task GuiAsync(string noiDung)
        {
            byte[] data = Encoding.UTF8.GetBytes(noiDung);
            await _sslStream.WriteAsync(data);
        }
        public async Task<LoginResponseDTO> DangNhapAsync(string user, string pass)
        {
            // Tạo LoginRequestDTO
            LoginRequestDTO login = new LoginRequestDTO
            {
                TenDangNhap = user,
                MatKhau = pass
            };

            // Gói vào BaseRequestDTO
            var baseReq = new
            {
                Type = "LOGIN",
                Payload = JsonSerializer.Serialize(login)
            };

            string json = JsonSerializer.Serialize(baseReq);
            byte[] data = Encoding.UTF8.GetBytes(json);

            // Gửi lên server
            await _sslStream.WriteAsync(data);

            // Nhận phản hồi
            byte[] buffer = new byte[4096];
            int soByte = await _sslStream.ReadAsync(buffer, 0, buffer.Length);

            string jsonResp = Encoding.UTF8.GetString(buffer, 0, soByte);

            return JsonSerializer.Deserialize<LoginResponseDTO>(jsonResp);
        }

    }
}
