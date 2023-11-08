namespace Nanoframework.W5500.Socks
{
    public class W5500Socket
    {
        private W5500Driver _driver;
        private byte _socket_ptr;
        private ushort _write_offset;

        public W5500Socket(W5500Driver driver, byte socket_ptr)
        {
            _driver = driver;
            _socket_ptr = socket_ptr;
        }

        public bool IsLinkUp() => _driver.IsLinkUp();
        public ushort Available() => _driver.GetRxByteCount(_socket_ptr);
        public void SetDestIp(byte[] ip_arr) => _driver.SetSocketDestIpAddress(_socket_ptr, ip_arr);
        public void SetDestPort(ushort port) => _driver.SetSocketDestPort(_socket_ptr, port);
        public void SetSourcePort(ushort port) => _driver.SetSocketSrcPort(_socket_ptr, port);
        public void Connect() => _driver.SendSocketCommand(_socket_ptr, W5500SocketEnums.W5500CommandValue.CONNECT);
        public void Close()
        {
            _driver.SendSocketCommand(_socket_ptr, W5500SocketEnums.W5500CommandValue.CLOSE);
        }
        public byte GetInterruptFlags() => _driver.GetSocketInterruptFlags(_socket_ptr);
        public void ClearInterruptFlag(W5500SocketEnums.W5500InterruptFlags flag)
        {
            _driver.ClearSocketInterruptFlag(_socket_ptr, flag);
        }
        public void Flush() => _driver.Flush(_socket_ptr);
        public byte Read() => _driver.Read(_socket_ptr);
        public int Peek(byte[] buffer, int length) => _driver.Peek(_socket_ptr, buffer, length);
        public int Read(byte[] buffer, int length) => _driver.Read(_socket_ptr, buffer, length);
        public int Write(byte[] buffer, int length)
        {
            var ret = _driver.Write(_socket_ptr, buffer, _write_offset, length);
            _write_offset += (ushort)ret;
            return ret;
        }
        public int Send(byte[] buffer, int length)
        {
            var ret = _driver.Send(_socket_ptr, buffer, _write_offset, length);
            _write_offset = 0;
            return ret;
        }
        public void Send()
        {
            if (_write_offset == 0)
                return;
            _driver.Send(_socket_ptr);
            _write_offset = 0;
        }
    }
}