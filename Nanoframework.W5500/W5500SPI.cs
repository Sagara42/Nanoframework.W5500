using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Spi;

namespace Nanoframework.W5500
{
    public class W5500SPI
    {
        public SpiDevice Device => _device;
        private SpiDevice _device;

        public void Init(int sck, int miso, int mosi, int cs)
        {
            Configuration.SetPinFunction(mosi, DeviceFunction.SPI1_MOSI);
            Configuration.SetPinFunction(miso, DeviceFunction.SPI1_MISO);
            Configuration.SetPinFunction(sck, DeviceFunction.SPI1_CLOCK);
            _device = new SpiDevice(new SpiConnectionSettings(1, cs));
        }

        public byte ReadRegister8Bit(W5500Register register)
        {
            var value = ReadRegister(register);
            if (value == null)
                throw new Exception("8bit read was null");
            return value[0];
        }
        public byte ReadRegister8Bit(W5500SockRegister register, byte socket)
        {
            var value = ReadRegister(register, socket);
            if (value == null)
                throw new Exception("8bit read was null");
            return value[0];
        }
        public ushort ReadRegister16Bit(W5500Register register)
        {
            var value = ReadRegister(register);
            if (value == null)
                throw new Exception("16bit read was null");
            var t_a = value[0];
            var t_b = value[1];
            value[0] = t_b;
            value[1] = t_a;
            return BitConverter.ToUInt16(value, 0);
        }
        public ushort ReadRegister16Bit(W5500SockRegister register, byte socket)
        {
            var value = ReadRegister(register, socket);
            if (value == null)
                throw new Exception("16bit read was null");
            var t_a = value[0];
            var t_b = value[1];
            value[0] = t_b;
            value[1] = t_a;
            return BitConverter.ToUInt16(value, 0);
        }
        public byte[] ReadRegister(W5500Register register)
        {
            var cmd = new byte[]
            {
                0x00,
                register.Address,
                ((0x00 << 3) | (0 << 2) | 0x00 )
            };

            var data = new byte[register.Length + 3];
            _device.TransferFullDuplex(cmd, data);
            var result = new byte[register.Length];
            Array.Copy(data, 3, result, 0, data.Length - 3);

            return result;
        }
        public byte[] ReadRegister(W5500SockRegister register, byte socket)
        {
            var cmd = new byte[]
            {
                0x00,
                register.Address,
                (byte)((W5500Common.SOCKET_REG(socket) << 3) | (0 << 2) | 0x00)
            };

            var data = new byte[register.Length + 3];
            _device.TransferFullDuplex(cmd, data);
            var result = new byte[register.Length];
            Array.Copy(data, 3, result, 0, data.Length - 3);

            return result;
        }
        public byte[] WriteRegister8Bit(W5500Register register, byte value) => WriteRegister(register, new byte[] { value });
        public byte[] WriteRegister16Bit(W5500Register register, ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            var buff = new byte[] { bytes[1], bytes[0] };
            return WriteRegister(register, buff);
        }
        public byte[] WriteRegister8Bit(W5500SockRegister register, byte socket, byte value) => WriteRegister(register, socket, new byte[] { value });
        public byte[] WriteRegister16Bit(W5500SockRegister register, byte socket, ushort value)
        {
            var bytes = BitConverter.GetBytes(value);
            var buff = new byte[] { bytes[1], bytes[0] };
            return WriteRegister(register, socket, buff);
        }
        public byte[] WriteRegister(W5500Register register, byte[] data)
        {
            var cmd = new byte[data.Length + 3];
            cmd[0] = 0x00;
            cmd[1] = register.Address;
            cmd[2] = (
                        (0x00 << 3) |
                        (1 << 2) |
                        0x00
                     );

            Array.Copy(data, 0, cmd, 3, data.Length);

            _device.TransferFullDuplex(cmd, cmd);
            var result = new byte[register.Length];
            Array.Copy(cmd, 3, result, 0, data.Length);
            return result;
        }
        public byte[] WriteRegister(W5500SockRegister register, byte socket, byte[] data)
        {
            var cmd = new byte[data.Length + 3];
            cmd[0] = 0x00;
            cmd[1] = register.Address;
            cmd[2] = (byte)(
                                (W5500Common.SOCKET_REG(socket) << 3) |
                                (1 << 2) |
                                0x00
                            );

            Array.Copy(data, 0, cmd, 3, data.Length);

            _device.TransferFullDuplex(cmd, cmd);
            var result = new byte[register.Length];
            Array.Copy(cmd, 3, result, 0, data.Length);
            return result;
        }
    }
}
