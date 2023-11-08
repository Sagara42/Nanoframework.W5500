namespace Nanoframework.W5500
{
    public class W5500Register
    {
        public W5500Register(byte addr, byte len = 1)
        {
            Address = addr;
            Length = len;
        }
        public byte Address { get; set; }
        public byte Length { get; set; }
    }

    public class W5500SockRegister : W5500Register
    {
        public W5500SockRegister(byte addr, byte len = 1) : base(addr, len)
        {
        }
    }

    public static class W5500Common
    {
        public static byte SOCKET_REG(byte socket) => (byte)(socket * 4 + 1);
        public static byte SOCKET_TX_BUFFER(byte socket) => (byte)(socket * 4 + 2);
        public static byte SOCKET_RX_BUFFER(byte socket) => (byte)(socket * 4 + 3);
    }

    public static class W5500CommonRegs
    {
        public static W5500Register Mode = new(0x00);
        public static W5500Register GatewayAddress = new(0x01, 4);
        public static W5500Register SubnetMaskAddress = new(0x05, 4);
        public static W5500Register SourceHardwareAddress = new(0x09, 6);
        public static W5500Register SourceIpAddress = new(0x0F, 4);
        public static W5500Register InterruptLevel = new(0x13, 2);
        public static W5500Register Interrupt = new(0x15);
        public static W5500Register InterruptMask = new(0x16);
        public static W5500Register SocketInterrupt = new(0x17);
        public static W5500Register SocketInterruptMask = new(0x18);
        public static W5500Register RetryTime = new(0x19, 2);
        public static W5500Register RetryCount = new(0x1B);
        public static W5500Register PPPLCPRequestTimer = new(0x1C);
        public static W5500Register PPPLCPRequestMagic = new(0x1D);
        public static W5500Register PPPDestinationMAC = new(0x1E, 6);
        public static W5500Register PPPSessionId = new(0x24, 2);
        public static W5500Register PPPMaxSegmentSize = new(0x26, 2);
        public static W5500Register UnreachableIP = new(0x28, 4);
        public static W5500Register UnreachablePort = new(0x2C, 2);
        public static W5500Register PhyConfig = new(0x2E);
        public static W5500Register ChipVersion = new(0x39);
    }

    public static class W5500SocketRegs
    {
        public static W5500SockRegister Mode = new(0x00);
        public static W5500SockRegister Command = new(0x01);
        public static W5500SockRegister Interrupt = new(0x02);
        public static W5500SockRegister Status = new(0x03);
        public static W5500SockRegister SourcePort = new(0x04, 2);
        public static W5500SockRegister DestHardwareAddress = new(0x06, 6);
        public static W5500SockRegister DestIPAddress = new(0x0C, 4);
        public static W5500SockRegister DestPort = new(0x10, 2);
        public static W5500SockRegister MaxSegmentSize = new(0x12, 2);
        public static W5500SockRegister IP_TOS = new(0x15);
        public static W5500SockRegister IP_TTL = new(0x16);
        public static W5500SockRegister RxBufferSize = new(0x1E);
        public static W5500SockRegister TxBufferSize = new(0x1F);
        public static W5500SockRegister TxFreeSize = new(0x20, 2);
        public static W5500SockRegister TxReadPointer = new(0x22, 2);
        public static W5500SockRegister TxWritePointer = new(0x24, 2);
        public static W5500SockRegister RxReceivedSize = new(0x26, 2);
        public static W5500SockRegister RxReadPointer = new(0x28, 2);
        public static W5500SockRegister RxWritePointer = new(0x2A, 2);
        public static W5500SockRegister InterruptMask = new(0x2C);
        public static W5500SockRegister FragmentOffset = new(0x2D, 2);
        public static W5500SockRegister KeepAliveTimer = new(0x2F);
    }

    public static class W5500CommonEnums
    {
        public enum W5500SocketMode
        {
            ClOSED = 0b0000,
            TCP = 0b0001,
            UDP = 0b0010,
            MACRAW = 0b0100
        }

        public enum W5500ModeFlags : byte
        {
            RESET = (1 << 7),
            WAKE_ON_LAN = (1 << 5),
            PING_BLOCK = (1 << 4),
            PPOE_MODE = (1 << 3),
            FORCE_ARP = (1 << 1),
        }

        public enum W5500InterruptFlags : byte
        {
            IP_CONFLICT = (1 << 7),
            UNREACHABLE = (1 << 6),
            PPOE_CLOSED = (1 << 5),
            MAGIC_PACKET = (1 << 4)
        }

        public enum InterruptMaskFlags : byte
        {
            IP_CONFLICT = (1 << 7),
            UNREACHABLE = (1 << 6),
            PPOE_CLOSED = (1 << 5),
            MAGIC_PACKET = (1 << 4)
        }

        public enum PhyOperationMode : byte
        {
            BASE10_HALF_DUPLEX = 0b000,
            BASE10_FULL_DUPLEX = 0b001,
            BASE100_HALF_DUPLEX = 0b010,
            BASE100_FULL_DUPLEX = 0b011,
            BASE100_HALF_DUPLEX_AUTONEGOTIATE = 0b100,
            POWER_DOWN = 0b110,
            AUTO_ALL = 0b111
        }

        public enum PhyConfigFlags : byte
        {
            RESET = (1 << 7),
            OPERATION_MODE = (1 << 6),
            DUPLEX_STATUS = (1 << 2),
            SPEED_STATUS = (1 << 1),
            LINK_STATUS = (1 << 0),
        }
    }

    public static class W5500SocketEnums
    {
        public enum W5500ModeFlags : byte
        {
            MULTI_MFEN = (1 << 7),
            BROADCAST_BLOCK = (1 << 6),
            ND_MC_MMC = (1 << 5),
            UCASTB_MIP6B = (1 << 4),
        }

        public enum W5500CommandValue : byte
        {
            OPEN = 0x01,
            LISTEN = 0x02,
            CONNECT = 0x04,
            DISCONNECT = 0x08,
            CLOSE = 0x10,
            SEND = 0x20,
            SEND_MAC = 0x21,
            SEND_KEEPALIVE = 0x22,
            RECV = 0x40
        }

        public enum W5500InterruptFlags : byte
        {
            SEND_OK = (1 << 4),
            TIMEOUT = (1 << 3),
            RECV = (1 << 2),
            DISCONNECT = (1 << 1),
            CONNECT = (1 << 0)
        }

        public enum W5500InterruptMaskFlags : byte
        {
            SEND_OK = (1 << 4),
            TIMEOUT = (1 << 3),
            RECV = (1 << 2),
            DISCONNECT = (1 << 1),
            CONNECT = (1 << 0)
        }

        public enum W5500StatusValue : byte
        {
            CLOSED = 0x00,
            INIT = 0x13,
            LISTEN = 0x14,
            SYN_SENT = 0x15,
            SYN_RECV = 0x16,
            ESTABLISHED = 0x17,
            FIN_WAIT = 0x18,
            CLOSING = 0x1A,
            TIME_WAIT = 0x1B,
            CLOSE_WAIT = 0x1C,
            LAST_ACK = 0x1D,
            UDP = 0x22,
            MACRAW = 0x42
        }

        public enum W5500BufferSize : byte
        {
            SZ_ZERO = 0,
            SZ_1K = 0x01,
            SZ_2K = 0x02,
            SZ_4K = 0x04,
            SZ_8K = 0x08,
            SZ_16K = 0x10
        }
    }
}
