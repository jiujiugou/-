using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusTCP
{
    public class ByteArray
    {
        private List<byte> list = new List<byte>();
        /// <summary>
        /// 获取字节集合
        /// </summary>
        public List<byte> List
        {
            get { return list; }
        }
        /// <summary>
        /// 获取字节数组
        /// </summary>
        public byte[] Array
        {
            get { return list.ToArray(); }
        }
        /// <summary>
        /// 获取字节数组的长度
        /// </summary>
        public int Length
        {
            get { return list.Count; }
        }
        /// <summary>
        /// 添加一个字节
        /// </summary>
        /// <param name="item"></param>
        public void Add(byte item)
        {
            list.Add(item);
        }
        /// <summary>
        /// 添加一个字节数组
        /// </summary>
        /// <param name="items"></param>
        public void Add(byte[] items)
        {
            list.AddRange(items);
        }
        /// <summary>
        /// 添加一个集合
        /// </summary>
        /// <param name="list"></param>
        public void Add(List<byte> list)
        {
            list.AddRange(list);
        }
        /// <summary>
        /// 添加两个字节
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        public void Add(byte item1, byte item2)
        {
            Add(new byte[] { item1, item2 });
        }
        /// <summary>
        /// 添加三个字节
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        public void Add(byte item1, byte item2, byte item3)
        {
            Add(new byte[] { item1, item2, item3 });
        }
        /// <summary>
        /// 添加四个字节
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <param name="item4"></param>
        public void Add(byte item1, byte item2, byte item3, byte item4)
        {
            Add(new byte[] { item1, item2, item3, item4 });
        }
        /// <summary>
        /// 添加五个字节
        /// </summary>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="item3"></param>
        /// <param name="item4"></param>
        /// <param name="item5"></param>
        public void Add(byte item1, byte item2, byte item3, byte item4, byte item5)
        {
            Add(new byte[] { item1, item2, item3, item4, item5 });
        }
        /// <summary>
        /// 添加一个byte Array
        /// </summary>
        /// <param name="byteArray"></param>
        public void Add(ByteArray byteArray)
        {
            Add(byteArray.Array);
        }
        /// <summary>
        /// 添加一个short
        /// </summary>
        /// <param name="value"></param>
        public void Add(short value)
        {
            Add((byte)(value >> 8));
            Add((byte)value);
        }
        /// <summary>
        /// 添加一个ushort
        /// </summary>
        /// <param name="value"></param>
        public void Add(ushort value)
        {
            Add((byte)(value >> 8));
            Add((byte)value);
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void Clear()
        {
            list.Clear();
        }
    }
}
