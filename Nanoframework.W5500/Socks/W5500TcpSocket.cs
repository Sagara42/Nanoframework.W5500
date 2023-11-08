namespace Nanoframework.W5500.Socks
{
    public class W5500TcpSocket : W5500Socket
    {
        private ushort _client_port = 1;
        private byte _socket_ptr;
        private W5500Driver _driver;
        public W5500TcpSocket(byte socket_ptr, W5500Driver driver) : base(driver, socket_ptr)
        {
            _socket_ptr = socket_ptr;
            _driver = driver;
        }

        public bool Init()
        {
            _driver.SetSocketMode(_socket_ptr, W5500CommonEnums.W5500SocketMode.TCP);
            _driver.SendSocketCommand(_socket_ptr, W5500SocketEnums.W5500CommandValue.OPEN);

            return Ready();
        }
        private bool Ready()
        {
            var status = _driver.GetSocketStatus(_socket_ptr);
            return status == W5500SocketEnums.W5500StatusValue.INIT
                   || status == W5500SocketEnums.W5500StatusValue.LISTEN
                   || status == W5500SocketEnums.W5500StatusValue.SYN_SENT
                   || status == W5500SocketEnums.W5500StatusValue.SYN_RECV
                   || status == W5500SocketEnums.W5500StatusValue.ESTABLISHED;
        }
        public bool Connecting() => _driver.GetSocketStatus(_socket_ptr) == W5500SocketEnums.W5500StatusValue.SYN_SENT;
        public bool Connected() => _driver.GetSocketStatus(_socket_ptr) == W5500SocketEnums.W5500StatusValue.ESTABLISHED;
        public void Connect(byte[] ip, ushort port)
        {
            SetDestIp(ip);
            SetDestPort(port);
            SetSourcePort(_client_port++);
            Connect();
        }
    }
}
