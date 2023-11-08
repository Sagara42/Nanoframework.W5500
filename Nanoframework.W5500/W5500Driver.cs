using System;

namespace Nanoframework.W5500
{
    public class W5500Driver
    {
        private W5500SPI _spi;

        public void Init(int sck, int miso, int mosi, int cs)
        {
            _spi = new W5500SPI();
            _spi.Init(sck, miso, mosi, cs);
        }
        public bool IsVersionCorrect()
        {
            return _spi.ReadRegister8Bit(W5500CommonRegs.ChipVersion) == 0x04;
        }
        public bool IsLinkUp()
        {
            var config = _spi.ReadRegister8Bit(W5500CommonRegs.PhyConfig);
            return (config & (byte)W5500CommonEnums.PhyConfigFlags.LINK_STATUS) == 1;
        }
        public void SetMac(byte[] mac)
        {
            _spi.WriteRegister(W5500CommonRegs.SourceHardwareAddress, mac);
        }
        public void SetGateway(byte[] gateway)
        {
            _spi.WriteRegister(W5500CommonRegs.GatewayAddress, gateway);
        }
        public void SetSubnetMask(byte[] mask)
        {
            _spi.WriteRegister(W5500CommonRegs.SubnetMaskAddress, mask);
        }
        public void SetIp(byte[] ip)
        {
            _spi.WriteRegister(W5500CommonRegs.SourceIpAddress, ip);
        }
        public byte[] GetMac()
        {
            return _spi.ReadRegister(W5500CommonRegs.SourceHardwareAddress);
        }
        public byte[] GetGateway()
        {
            return _spi.ReadRegister(W5500CommonRegs.GatewayAddress);
        }
        public byte[] GetSubnetMask()
        {
            return _spi.ReadRegister(W5500CommonRegs.SubnetMaskAddress);
        }
        public byte[] GetIp()
        {
            return _spi.ReadRegister(W5500CommonRegs.SourceIpAddress);
        }
        public W5500SocketEnums.W5500StatusValue GetSocketStatus(byte socket)
        {
            var value = _spi.ReadRegister8Bit(W5500SocketRegs.Status, socket);
            return (W5500SocketEnums.W5500StatusValue)value;
        }
        public void SetSocketMode(byte socket, W5500CommonEnums.W5500SocketMode mode)
        {
            var value = (byte)mode;
            _spi.WriteRegister8Bit(W5500SocketRegs.Mode, socket, value);
        }
        public void SendSocketCommand(byte socket, W5500SocketEnums.W5500CommandValue command)
        {
            var value = (byte)command;
            _spi.WriteRegister8Bit(W5500SocketRegs.Command, socket, value);
        }
        public void SetSocketDestIpAddress(byte socket, byte[] target_ip)
        {
            _spi.WriteRegister(W5500SocketRegs.DestIPAddress, socket, target_ip);
        }
        public void SetSocketDestPort(byte socket, ushort port)
        {
            _spi.WriteRegister16Bit(W5500SocketRegs.DestPort, socket, port);
        }
        public void SetSocketSrcPort(byte socket, ushort port)
        {
            _spi.WriteRegister16Bit(W5500SocketRegs.SourcePort, socket, port);
        }
        public void SetSocketDestMac(byte socket, byte[] mac)
        {
            _spi.WriteRegister(W5500SocketRegs.DestHardwareAddress, socket, mac);
        }
        public byte[] GetSocketDestMac(byte socket)
        {
            return _spi.ReadRegister(W5500SocketRegs.DestHardwareAddress, socket);
        }
        public void SetSocketTxBufferSize(byte socket, W5500SocketEnums.W5500BufferSize size)
        {
            _spi.WriteRegister(W5500SocketRegs.TxBufferSize, socket, new byte[] { (byte)size });
        }
        public void SetSocketRxBufferSize(byte socket, W5500SocketEnums.W5500BufferSize size)
        {
            _spi.WriteRegister(W5500SocketRegs.RxBufferSize, socket, new byte[] { (byte)size });
        }
        public void SetSocketBufferSize(byte socket, W5500SocketEnums.W5500BufferSize size)
        {
            SetSocketTxBufferSize(socket, size);
            SetSocketRxBufferSize(socket, size);
        }
        public W5500SocketEnums.W5500BufferSize GetSocketTxBufferSize(byte socket)
        {
            return (W5500SocketEnums.W5500BufferSize)_spi.ReadRegister(W5500SocketRegs.TxBufferSize, socket)[0];
        }
        public W5500SocketEnums.W5500BufferSize GetSocketRxBufferSize(byte socket)
        {
            return (W5500SocketEnums.W5500BufferSize)_spi.ReadRegister(W5500SocketRegs.RxBufferSize, socket)[0];
        }

        #region Interrupts
        public void SetInterruptMask(params W5500CommonEnums.InterruptMaskFlags[] flags)
        {
            byte mask = 0x00;
            foreach (var flag in flags)
                mask |= (byte)flag;
            _spi.WriteRegister8Bit(W5500CommonRegs.InterruptMask, mask);
        }
        public byte GetInterruptState()
        {
            var value = _spi.ReadRegister8Bit(W5500CommonRegs.Interrupt);
            return value;
        }
        public bool HasInterruptFlag(W5500CommonEnums.W5500InterruptFlags flag)
        {
            var state = GetInterruptState();
            return (state & (byte)flag) > 0;
        }
        public void ClearInterruptFlag(W5500CommonEnums.W5500InterruptFlags flag)
        {
            _spi.WriteRegister8Bit(W5500CommonRegs.Interrupt, (byte)flag);
        }

        public byte GetSocketInterruptFlags(byte socket)
        {
            var val = _spi.ReadRegister8Bit(W5500SocketRegs.Interrupt, socket);
            return val;
        }

        public bool SocketHasInterruptFlag(byte socket, W5500SocketEnums.W5500InterruptFlags flag)
        {
            var value = GetSocketInterruptFlags(socket);
            return (value & (byte)flag) > 0;
        }

        public void ClearSocketInterruptFlag(byte socket, W5500SocketEnums.W5500InterruptFlags flag)
        {
            _spi.WriteRegister8Bit(W5500SocketRegs.Interrupt, socket, (byte)flag);
        }
        #endregion

        public void Reset()
        {
            var flag = _spi.WriteRegister(W5500CommonRegs.Mode, new byte[] { (byte)W5500CommonEnums.W5500ModeFlags.RESET })[0];
            while (true)
            {
                flag = _spi.ReadRegister8Bit(W5500CommonRegs.Mode);
                var status = flag & (byte)W5500CommonEnums.W5500ModeFlags.RESET;
                if (status == 0)
                    break;
            }

            while (true)
            {
                flag = _spi.ReadRegister(W5500CommonRegs.PhyConfig)[0];
                var status = flag & (byte)W5500CommonEnums.PhyConfigFlags.RESET;
                if (status > 0)
                    break;
            }
        }
        public void SetForceArp(bool enable)
        {
            var flag = _spi.ReadRegister8Bit(W5500CommonRegs.Mode);
            if (enable)
            {
                flag |= (byte)W5500CommonEnums.W5500ModeFlags.FORCE_ARP;
            }
            else
            {
                sbyte negative = ~(sbyte)W5500CommonEnums.W5500ModeFlags.FORCE_ARP;
                byte n_res = (byte)(flag & negative);
                flag = n_res;
            }
            _spi.WriteRegister8Bit(W5500CommonRegs.Mode, flag);
        }
        public ushort GetTxFreeSize(byte socket)
        {
            return _spi.ReadRegister16Bit(W5500SocketRegs.TxFreeSize, socket);
        }
        public ushort GetTxReadPointer(byte socket)
        {
            return (ushort)_spi.ReadRegister16Bit(W5500SocketRegs.TxReadPointer, socket);
        }
        public ushort GetTxWritePointer(byte socket)
        {
            return _spi.ReadRegister16Bit(W5500SocketRegs.TxWritePointer, socket);
        }
        public void SetTxWritePointer(byte socket, ushort offset)
        {
            _spi.WriteRegister16Bit(W5500SocketRegs.TxWritePointer, socket, offset);
        }
        public ushort GetRxByteCount(byte socket)
        {
            return _spi.ReadRegister16Bit(W5500SocketRegs.RxReceivedSize, socket);
        }
        public ushort GetRxReadPointer(byte socket)
        {
            return _spi.ReadRegister16Bit(W5500SocketRegs.RxReadPointer, socket);
        }
        public void SetRxReadPointer(byte socket, ushort offset)
        {
            _spi.WriteRegister16Bit(W5500SocketRegs.RxReadPointer, socket, offset);
        }
        public ushort GetRxWritePointer(byte socket)
        {
            return (ushort)_spi.ReadRegister16Bit(W5500SocketRegs.RxWritePointer, socket);
        }
        public void SetPhyMode(W5500CommonEnums.PhyOperationMode mode)
        {
            var currentPhySettings = _spi.ReadRegister8Bit(W5500CommonRegs.PhyConfig);
            byte newPhySettings =
                (
                    (byte)((currentPhySettings & ~(0b111 << 3)) |
                    (byte)W5500CommonEnums.PhyConfigFlags.OPERATION_MODE |
                    (byte)mode << 3)
                );

            _spi.WriteRegister8Bit(W5500CommonRegs.PhyConfig, newPhySettings);

            currentPhySettings = _spi.ReadRegister8Bit(W5500CommonRegs.PhyConfig);

            _spi.WriteRegister8Bit(W5500CommonRegs.PhyConfig,
                (byte)(newPhySettings & ~(byte)W5500CommonEnums.PhyConfigFlags.RESET));

            currentPhySettings = _spi.ReadRegister8Bit(W5500CommonRegs.PhyConfig);

            _spi.WriteRegister8Bit(W5500CommonRegs.PhyConfig, newPhySettings);
        }
        public int Flush(byte socket)
        {
            var read_ptr = GetRxReadPointer(socket);
            var write_ptr = GetRxWritePointer(socket);
            SetRxReadPointer(socket, write_ptr);
            SendSocketCommand(socket, W5500SocketEnums.W5500CommandValue.RECV);
            return write_ptr - read_ptr;
        }
        public int Write(byte socket, byte[] buffer, int offset, int length)
        {
            var free_buffer_size = GetTxFreeSize(socket);
            if (free_buffer_size == 0)
                return 0;

            if (length > free_buffer_size)
                throw new Exception("Buffer size higher then TxFreeSize, use SetSocketBufferSize to encrease buffer");

            var cmd = new byte[3 + buffer.Length];
            var write_ptr = GetTxWritePointer(socket);
            var write_offset = write_ptr + offset;

            cmd[0] = (byte)((write_offset >> 8) & 0xFF);
            cmd[1] = (byte)(write_offset & 0xFF);
            cmd[2] =
                (
                    (byte)((W5500Common.SOCKET_TX_BUFFER(socket) << 3) |
                    (1 << 2) |
                    0x00)
                );
            Array.Copy(buffer, 0, cmd, 3, buffer.Length);

            var preamb_buffer = new byte[3];

            _spi.Device.TransferFullDuplex(cmd, preamb_buffer);

            SetTxWritePointer(socket, (ushort)(length));

            return length;
        }
        public int Send(byte socket, byte[] buffer, int offset, int length)
        {
            var written = Write(socket, buffer, offset, length);
            SendSocketCommand(socket, W5500SocketEnums.W5500CommandValue.SEND);
            return written;
        }
        public void Send(byte socket)
        {
            SendSocketCommand(socket, W5500SocketEnums.W5500CommandValue.SEND);
        }
        public int Peek(byte socket, byte[] buffer, int size)
        {
            var available = GetRxByteCount(socket);
            if (available == 0)
                return 0;

            var cmd = new byte[3];
            var read_offset = GetRxReadPointer(socket);

            cmd[0] = (byte)((read_offset >> 8) & 0xFF);
            cmd[1] = (byte)(read_offset & 0xFF);
            cmd[2] =
                    (
                        (byte)((W5500Common.SOCKET_RX_BUFFER(socket) << 3) |
                        (0 << 2) |
                        0x00)
                    );

            var response = new byte[size + 3];
            _spi.Device.TransferFullDuplex(cmd, response);
            Array.Copy(response, 3, buffer, 0, size);

            return size;
        }
        public int Read(byte socket, byte[] buffer, int size)
        {
            var read = 0;
            if (buffer != null)
            {
                read = Peek(socket, buffer, size);
            }
            else
            {
                read = size;
            }

            SetRxReadPointer(socket, (ushort)(GetRxReadPointer(socket) + read));
            SendSocketCommand(socket, W5500SocketEnums.W5500CommandValue.RECV);
            return read;
        }
        public byte Read(byte socket)
        {
            byte result = 0;
            Read(socket, new byte[1], 1);
            return result;
        }
    }
}
