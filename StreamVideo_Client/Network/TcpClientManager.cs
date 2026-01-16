using StreamVideo_Client.DTO;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Net.Security;
using NAudio.Wave; // Thư viện âm thanh

namespace StreamVideo_Client.Network
{
    public class TcpClientManager
    {
        private TcpClient _client;
        private SslStream _sslStream;
        private BinaryReader _reader;
        private BinaryWriter _writer;
        private Thread _listenThread;

        // Sự kiện Video
        public event Action<byte[]> OnVideoFrameReceived;

        // Xử lý Âm thanh
        private BufferedWaveProvider _waveProvider;
        private WaveOutEvent _waveOut;

        // Biến Login
        private AutoResetEvent _loginWaitHandle = new AutoResetEvent(false);
        private bool _lastLoginResult = false;

        public bool Connect(string ip, int port)
        {
            try
            {
                _client = new TcpClient();
                _client.Connect(ip, port);

                // Bypass SSL check
                _sslStream = new SslStream(_client.GetStream(), false, (s, c, ch, e) => true);
                _sslStream.AuthenticateAsClient("StreamServer");

                _reader = new BinaryReader(_sslStream);
                _writer = new BinaryWriter(_sslStream);

                // --- CẤU HÌNH LOA ---
                _waveProvider = new BufferedWaveProvider(new WaveFormat(44100, 1));
                _waveOut = new WaveOutEvent();
                _waveOut.Init(_waveProvider);
                _waveOut.Play();
                // ---------------------

                _listenThread = new Thread(ListenLoop);
                _listenThread.IsBackground = true;
                _listenThread.Start();
                return true;
            }
            catch { return false; }
        }

        private void ListenLoop()
        {
            try
            {
                while (_client.Connected)
                {
                    int length = _reader.ReadInt32();
                    byte type = _reader.ReadByte();
                    byte[] payload = _reader.ReadBytes(length);

                    if (type == 1) // Login Response
                    {
                        string json = Encoding.UTF8.GetString(payload);
                        var res = JsonSerializer.Deserialize<LoginResponseDTO>(json);
                        _lastLoginResult = res.ThanhCong;
                        _loginWaitHandle.Set();
                    }
                    else if (type == 2) // VIDEO
                    {
                        // Gọi sự kiện để FormStream hiển thị
                        OnVideoFrameReceived?.Invoke(payload);
                    }
                    else if (type == 3) // AUDIO
                    {
                        // Đẩy vào loa
                        if (_waveProvider != null)
                        {
                            _waveProvider.AddSamples(payload, 0, payload.Length);
                        }
                    }
                }
            }
            catch
            {
                Disconnect();
            }
        }

        public bool Login(string user, string pass)
        {
            if (_client == null || !_client.Connected) return false;
            var req = new BaseRequestDTO
            {
                Type = RequestType.LOGIN,
                Payload = JsonSerializer.Serialize(new LoginRequestDTO { TenDangNhap = user, MatKhau = pass })
            };
            GuiDuLieu(1, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(req)));
            _loginWaitHandle.WaitOne(3000);
            return _lastLoginResult;
        }

        public void GuiDuLieu(byte type, byte[] data)
        {
            try
            {
                lock (_writer)
                {
                    _writer.Write((int)data.Length);
                    _writer.Write(type);
                    _writer.Write(data);
                    _writer.Flush();
                }
            }
            catch { }
        }

        public void Disconnect()
        {
            try
            {
                _waveOut?.Stop();
                _waveOut?.Dispose();
                _client?.Close();
            }
            catch { }
        }
    }
}