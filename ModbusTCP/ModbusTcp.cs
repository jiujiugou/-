using System.Net;
using System.Net.Sockets;

namespace ModbusTCP
{
    public class ModbusTcp
    {
        public int SendTimeout { get; set; } = 2000;
        public int ReceiveTimeout { get; set; } = 2000;
        public int SleepTime { get; set; } = 1;
        public int MaxWaitTimes { get; set; } = 10;
        public byte Slaved { get; set; } = 0x01;
        private Socket socket;
        SimpleHybirdLock SimpleHybirdLock = new SimpleHybirdLock();
        public bool Connect(string ip, int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.SendTimeout = SendTimeout;
            socket.ReceiveTimeout = ReceiveTimeout;
            try
            {
                if (IPAddress.TryParse(ip, out IPAddress address))
                {
                    socket.Connect(address, port);
                }
                else
                {

                    socket.Connect(ip, port);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        public void DisConnect()
        {
            if (socket != null)
            {
                socket.Close();
            }
        }

        #region 01H读取输出线圈状态
        public byte[] ReadOutPutCoils(ushort start,ushort length)
        {
            //事务处理+协议标识符+长度+单元标识符+功能码+起始地址+长度
            ByteArray SendCommand = new ByteArray();
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            SendCommand.Add(0x00, 0x06, Slaved, 0x01);
            SendCommand.Add(start);
            SendCommand.Add(length);
            byte[] receive = null;
            int byteLength=length%8== 0 ? length / 8 : length / 8 + 1;
            if (SendAndReceive(SendCommand.Array,ref receive))
            {
                if (receive.Length == 9 +byteLength)
                {
                    if (receive[6]==Slaved && receive[7] == 0x01 && receive[8]==byteLength)
                    {
                        byte[] result = new byte[length];
                        Array.Copy(receive, 9, result, 0, byteLength);
                        return result;
                    }
                }
            }
            return null;
        }
        #endregion

        #region 02H读取输入线圈状态
        public byte[] ReadInPutCoils(ushort start, ushort length)
        {
            //事务处理+协议标识符+长度+单元标识符+功能码+起始地址+长度
            ByteArray SendCommand = new ByteArray();
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            SendCommand.Add(0x00, 0x06, Slaved, 0x02);
            SendCommand.Add(start);
            SendCommand.Add(length);
            byte[] receive = null;
            int byteLength = length % 8 == 0 ? length / 8 : length / 8 + 1;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                if (receive.Length == 9 + byteLength)
                {
                    if (receive[6] == Slaved && receive[7] == 0x02 && receive[8] == byteLength)
                    {
                        byte[] result = new byte[length];
                        Array.Copy(receive, 9, result, 0, byteLength);
                        return result;
                    }
                }
            }
            return null;
        }
        #endregion

        #region 03H读取输出寄存器状态
        public byte[] ReadOutPutRegister(ushort start, ushort length)
        {
            //事务处理+协议标识符+长度+单元标识符+功能码+起始寄存器地址+长度
            ByteArray SendCommand = new ByteArray();
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            SendCommand.Add(0x00, 0x06, Slaved, 0x03);
            SendCommand.Add(start);
            SendCommand.Add(length);
            byte[] receive = null;
            int byteLength = length * 2;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                if (receive.Length == 9 + byteLength)
                {
                    if (receive[6] == Slaved && receive[7] == 0x03 && receive[8] == byteLength)
                    {
                        byte[] result = new byte[length];
                        Array.Copy(receive, 9, result, 0, byteLength);
                        return result;
                    }
                }
            }
            return null;
        }
        #endregion

        #region 04H读取输入寄存器状态
        public byte[] ReadInPutRegister(ushort start, ushort length)
        {
            //事务处理+协议标识符+长度+单元标识符+功能码+起始寄存器地址+长度
            ByteArray SendCommand = new ByteArray();
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            SendCommand.Add(0x00, 0x06, Slaved, 0x04);
            SendCommand.Add(start);
            SendCommand.Add(length);
            byte[] receive = null;
            int byteLength = length * 2;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                if (receive.Length == 9 + byteLength)
                {
                    if (receive[6] == Slaved && receive[7] == 0x04 && receive[8] == byteLength)
                    {
                        byte[] result = new byte[length];
                        Array.Copy(receive, 9, result, 0, byteLength);
                        return result;
                    }
                }
            }
            return null;
        }
        #endregion

        #region 05H预置单线圈
        public bool PreSetSingleCoil(ushort start, bool value)
        {
            ByteArray SendCommand = new ByteArray();
            //事务处理+协议标识符+长度+单元标识符+功能码+线圈地址+线圈值(0xff 0x00/0x00 0x00)
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            SendCommand.Add(0x00, 0x06, Slaved, 0x05);
            SendCommand.Add(start);
            SendCommand.Add(value?(byte)0xff:(byte)0x00,0x00);
            byte[] receive = null;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                if (receive.Length == 12)
                {
                    return ByteArrayEquals(SendCommand.Array, receive);
                }
            }
            return false;
        }
        #endregion

        #region 05H预置单寄存器
        public bool PreSetSingleRegister(ushort start, byte[] value)
        {
            ByteArray SendCommand = new ByteArray();
            //事务处理+协议标识符+长度+单元标识符+功能码+寄存器地址+寄存器值
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            SendCommand.Add(0x00, 0x06, Slaved, 0x06);
            SendCommand.Add(start);
            SendCommand.Add(value);
            byte[] receive = null;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                if (receive.Length == 12)
                {
                    return ByteArrayEquals(SendCommand.Array, receive);
                }
            }
            return false;
        }
        public bool PreSetSingleRegister(ushort start, short value)
        {
            return PreSetSingleRegister(start, BitConverter.GetBytes(value).Reverse().ToArray());
        }
        public bool PreSetSingleRegister(ushort start, ushort value)
        {
            return PreSetSingleRegister(start, BitConverter.GetBytes(value).Reverse().ToArray());
        }
        #endregion

        #region 0FH预置多线圈
        /// <summary>
        /// 预置多线圈
        /// </summary>
        /// <param name="start"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool PreSetMultiCoils(ushort start, bool[] value)
        {
            ByteArray SendCommand = new ByteArray();
            byte[] setArray = GetByteArrayFromBoolArray(value);
            //事务处理+协议标识符+长度+单元标识符+功能码+起始线圈地址+线圈数量+字节数+字节数据
            //事务处理+协议标识
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            //长度
            SendCommand.Add((short)(7 + setArray.Length));
            //单元标识符+功能码
            SendCommand.Add(Slaved, 0x0f);
            //起始线圈地址
            SendCommand.Add(start);
            //线圈数量
            SendCommand.Add((ushort)value.Length);
            //字节数
            SendCommand.Add((byte)setArray.Length);
            //字节数据
            SendCommand.Add(setArray);
            byte[] receive = null;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                byte[] send = new byte[12];
                Array.Copy(SendCommand.Array, 0, send, 0, 12);
                send[4] = 0x00;
                send[5] = 0x06;
                return ByteArrayEquals(send, receive);
            }
            return false;
        }
        #endregion

        #region 10H预置多寄存器
        public bool PreSetMultiRegister(ushort start, byte[] value)
        {
            if (value == null || value.Length == 0 || value.Length % 2 == 1)
            {
                return false;
            }
            ByteArray SendCommand = new ByteArray();
            //事务处理+协议标识符+长度+单元标识符+功能码+寄存器地址+寄存器数量+字节数+字节数据
            SendCommand.Add(0x00, 0x00, 0x00, 0x00);
            //长度
            SendCommand.Add((short)(7 + value.Length));
            //单元标识符+功能码
            SendCommand.Add(Slaved, 0x10);
            //寄存器地址
            SendCommand.Add(start);
            //寄存器数量
            SendCommand.Add((short)(value.Length/2));
            //字节数
            SendCommand.Add((byte)value.Length);
            SendCommand.Add(value);
            byte[] receive = null;
            if (SendAndReceive(SendCommand.Array, ref receive))
            {
                byte[] send = new byte[12];
                Array.Copy(SendCommand.Array, 0, send, 0, 12);
                send[4] = 0x00;
                send[5] = 0x06;
                return ByteArrayEquals(send, receive);
            }
            return false;
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 发送和接收数据
        /// </summary>
        /// <param name="send"></param>
        /// <param name="receive"></param>
        /// <returns></returns>
        public bool SendAndReceive(byte[] send, ref byte[] receive)
        {
            byte[] buffer = new byte[1024];
            MemoryStream stream = new MemoryStream();
            try
            {
                SimpleHybirdLock.Enter();
                socket.Send(send, send.Length, SocketFlags.None);
                int waitTimes = 0;
                while (true)
                {
                    Thread.Sleep(SleepTime);
                    if (socket.Available > 0)
                    {
                        int count = socket.Receive(buffer, buffer.Length, SocketFlags.None);
                        stream.Write(buffer, 0, count);
                    }
                    else
                    {
                        waitTimes++;
                        if (waitTimes > MaxWaitTimes)
                        {
                            return false;
                        }
                        else if (stream.Length > 0)
                        {
                            break;
                        }
                    }
                }
                receive = stream.ToArray();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            finally
            {
                stream.Close();
                SimpleHybirdLock.Leave();
            }
        }

        #region 数组比较方法
        private bool ByteArrayEquals(byte[] b1, byte[] b2)
        {
            return BitConverter.ToString(b1)==BitConverter.ToString(b2);
        }
        #endregion

        #region 将布尔数组转换为字节数组
        private byte[] GetByteArrayFromBoolArray(bool[] value)
        {
            int byteLength = value.Length % 8 == 0 ? value.Length / 8 : value.Length / 8 + 1;
            byte[] result = new byte[byteLength];
            for (int i = 0; i < result.Length; i++)
            {
                //获取每个字节的值
                int total = value.Length < 8 * (i + 1) ? value.Length - 8 * i : 8;
                for (int j = 0; j < total; j++)
                {
                    result[i] = SetBitValue(result[i], j, value[8 * i + j]);
                }
            }
            return result;
        }
        /// <summary>
        /// 将某个字节某个位置位或复位
        /// </summary>
        /// <param name="src"></param>
        /// <param name="bit"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private byte SetBitValue(byte src, int bit, bool value)
        {
            return value ? (byte)(src | (1 << bit)) : (byte)(src & ~(1 << bit));
        }
        #endregion

        #endregion
    }
}
