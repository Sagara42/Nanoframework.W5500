# nanoframework esp32 W5500 module Driver
Currently works only TCP socket in client mode
Based on https://github.com/rschlaikjer/w5500/tree/master
### Example
```C#
var eth_driver = new W5500Driver();
eth_driver.Init(sck: 18, miso: 19, mosi: 23, cs: 4);
eth_driver.Reset();
eth_driver.SetPhyMode(W5500CommonEnums.PhyOperationMode.BASE100_FULL_DUPLEX);
eth_driver.SetMac(new byte[] { 0x01, 0x01, 0x01, 0x06, 0x06, 0x06});
eth_driver.SetIp(new byte[] { 9, 9, 9, 6 });
eth_driver.SetGateway(new byte[] { 9, 9, 9, 1 });
eth_driver.SetSubnetMask(new byte[] { 255, 255, 255, 0 });                     
while(eth_driver.IsLinkUp() == false)           
    Thread.Sleep(1000);

var ip = eth_driver.GetIp();
var gateway = eth_driver.GetGateway();
var mask = eth_driver.GetSubnetMask();
var mac = eth_driver.GetMac();
            
Debug.WriteLine($"Link Up, info:\n" +
    $"Ip: {ip[0]}.{ip[1]}.{ip[2]}.{ip[3]}\n" +
    $"Gateway: {gateway[0]}.{gateway[1]}.{gateway[2]}.{gateway[3]}\n" +
    $"Mask: {mask[0]}.{mask[1]}.{mask[2]}.{mask[3]}\n" +
    $"MAC: {mac[0].ToString("x2")}:{mac[1].ToString("x2")}:{mac[2].ToString("x2")}:{mac[3].ToString("x2")}");

var sock = new W5500TcpSocket(1, eth_driver);
eth_driver.SetSocketBufferSize(1, W5500BufferSize.SZ_4K);
var created = sock.Init();
if(created is false)
{
    var flags = sock.GetInterruptFlags();
    var sendok = flags & (byte)W5500InterruptFlags.SEND_OK;
    var timeout = flags & (byte)W5500InterruptFlags.TIMEOUT;
    var connect = flags & (byte)W5500InterruptFlags.CONNECT;
    var disconnect = flags & (byte)W5500InterruptFlags.DISCONNECT;
    var recv = flags & (byte)W5500InterruptFlags.RECV;
    Debug.WriteLine("Cant init socket, interrupts:\n" +
        $"SEND_OK {sendok}\n" +
        $"TIMEOUT {timeout}\n" +
        $"CONNECT {connect}\n" +
        $"DISCONT {disconnect}\n" +
        $"RECV {recv}");
}
if (created)
{               
    sock.Connect(new byte[] { 9, 9, 9, 5 }, 9889);
    sock.Send(new byte[] { 1, 2, 3, 4 }, 4);

    var read_buffer = new byte[10];
    sock.Read(read_buffer, read_buffer.Length);
}
```
