using ModbusRTUlib;
using ModbusTCP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using thinger.DataConvertLib;

namespace Modbus.Project
{
    public partial class FrmModbusTCP : Form
    {
        public FrmModbusTCP()
        {
            InitializeComponent();
            InitParam();
        }
        #region 属性
        /// <summary>
        /// ModbusRTU通信对象
        /// </summary>
        ModbusTcp Modbus = new ModbusTcp();
        /// <summary>
        /// 当前连接状态
        /// </summary>
        private bool IsConnected = false;
        /// <summary>
        /// 数据格式
        /// </summary>
        private DataFormat DataFormat = DataFormat.ABCD;
        #endregion

        #region 初始化
        /// <summary>
        /// 初始化参数
        /// </summary>
        private void InitParam()
        {
            //大小端
            this.cmb_DataFormat.DataSource = Enum.GetNames(typeof(DataFormat));
            this.cmb_DataFormat.SelectedIndex = 0;
            //存储区
            this.cmb_StoreArea.DataSource = Enum.GetNames(typeof(StoreArea));
            this.cmb_StoreArea.SelectedIndex = 0;
            //数据类型
            this.cmb_DataType.DataSource = Enum.GetNames(typeof(DataType));
            this.cmb_DataType.SelectedIndex = 0;
        }
        #endregion

        #region 控件事件
        /// <summary>
        /// 连接按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtn_Connect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                AddLog(0, "modbusRTU已经建立连接");
                return;
            }
            var Ip= this.txt_Address.Text.Trim();
            var Port = int.Parse(this.Txt_Port.Text.Trim());
            IsConnected = Modbus.Connect(Ip, Port);
            if (IsConnected)
            {
                AddLog(0, "modbusRTU连接成功");
            }
            else
            {
                AddLog(2, "modbusRTU连接失败");
            }
        }
        /// <summary>
        /// 断开连接按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            Modbus.DisConnect();
            IsConnected = false;
            AddLog(0, "modbusRTU断开连接");
        }
        /// <summary>
        /// 读取按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                ushort start = ushort.Parse(this.txt_Start.Text.Trim());
                ushort length = ushort.Parse(this.txt_Length.Text.Trim());
                DataType dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_DataType.Text, true);
                StoreArea storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.Text, true);
                DataFormat dataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.Text, true);
                switch (dataType)
                {
                    case DataType.Bool:
                        ReadBool(storeArea,start, length);
                        break;
                    case DataType.Byte:
                        ReadByte(storeArea,start, length);
                        break;
                    case DataType.UShort:
                        ReadUShort(storeArea, start, length);
                        break;
                    case DataType.Short:
                        ReadShort(storeArea, start, length);
                        break;
                    case DataType.UInt:
                        ReadUInt(storeArea, start, length);
                        break;
                    case DataType.Int:
                        ReadInt(storeArea, start, length);
                        break;
                    case DataType.Float:
                        ReadFloat(storeArea, start, length);
                        break;
                    case DataType.Long:
                    case DataType.Double:
                    case DataType.String:
                    case DataType.ByteArray:
                        AddLog(2, "不支持以上类型");
                        return;
                }
            }
        }
        /// <summary>
        /// 写入按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                ushort start = ushort.Parse(this.txt_Start.Text.Trim());
                DataType dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_DataType.Text, true);
                StoreArea storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.Text, true);
                DataFormat dataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.Text, true);
                string setValue = this.txt_WriteValue.Text.Trim();
                switch (dataType)
                {
                    case DataType.Bool:
                        WriteBool(storeArea, start, setValue);
                        break;
                    case DataType.Byte:
                        WriteByte(storeArea, start, setValue);
                        break;
                    case DataType.UShort:
                        WriteUShort(storeArea, start, setValue);
                        break;
                    case DataType.Short:
                        WriteShort(storeArea, start, setValue);
                        break;
                    case DataType.UInt:
                        WriteUInt(storeArea, start, setValue);
                        break;
                    case DataType.Int:
                        WriteUInt(storeArea,start, setValue);
                        break;
                    case DataType.Float:
                        WriteFloat(storeArea,start, setValue);
                        break;
                    case DataType.Long:
                    case DataType.Double:
                    case DataType.String:
                    case DataType.ByteArray:
                        AddLog(2, "不支持以上类型");
                        return;
                }
            }
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 通用日志记录
        /// </summary>
        /// <param name="level"></param>
        /// <param name="info"></param>
        private void AddLog(int level, string info)
        {
            ListViewItem item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), level);
            item.SubItems.Add(info);
            //让最新的数据在最上面
            this.lst_info.Items.Insert(0, item);
        }
        /// <summary>
        /// 通用参数校验
        /// </summary>
        /// <returns></returns>
        private bool CommonVerify()
        {
            if (!IsConnected)
            {
                AddLog(1, "modbusRTU未正常连接");
                return false;
            }
            if (byte.TryParse(this.txt_Slaveld.Text, out _) == false)
            {
                AddLog(1, "检查站地址格式是否正确");
                return false;
            }
            if (ushort.TryParse(this.txt_Start.Text, out _) == false)
            {
                AddLog(1, "检查起始地址格式是否正确");
                return false;
            }
            if (ushort.TryParse(this.txt_Length.Text, out _) == false)
            {
                AddLog(1, "检查长度格式是否正确");
                return false;
            }
            return true;
        }
        #endregion

        #region 读取操作
        /// <summary>
        /// 读取bool类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadBool(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
                case StoreArea.输入线圈1x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
                case StoreArea.输入寄存器3x:
                case StoreArea.输出寄存器4x:
                    AddLog(1, "读取失败，不支持该存储");
                    return;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(BitLib.GetBitArrayFromByteArray(result, 0, length)));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 读取byte类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadByte(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
                case StoreArea.输入线圈1x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
                case StoreArea.输入寄存器3x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
                case StoreArea.输出寄存器4x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(result));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 读取short类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadShort(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                    AddLog(2, "不支持以上类型");
                    return;
                case StoreArea.输入寄存器3x:
                    result = Modbus.ReadInPutRegister(start, length);               
                    break;
                case StoreArea.输出寄存器4x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(ShortLib.GetShortArrayFromByteArray(result)));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 读取Ushort类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadUShort(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                    AddLog(2, "不支持以上类型");
                    return;
                case StoreArea.输入寄存器3x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
                case StoreArea.输出寄存器4x:
                    result = Modbus.ReadOutPutCoils(start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(UShortLib.GetUShortArrayFromByteArray(result)));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 读取Int类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadInt(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                    AddLog(2, "不支持以上类型");
                    return;
                case StoreArea.输入寄存器3x:
                    result = Modbus.ReadInPutRegister(start, length);
                    break;
                case StoreArea.输出寄存器4x:
                    result = Modbus.ReadOutPutRegister(start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(IntLib.GetIntArrayFromByteArray(result, this.DataFormat)));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 读取UInt类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadUInt(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                    AddLog(2, "不支持以上类型");
                    return;
                case StoreArea.输入寄存器3x:
                    result = Modbus.ReadInPutRegister(start, length);
                    break;
                case StoreArea.输出寄存器4x:
                    result = Modbus.ReadOutPutRegister(start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(UIntLib.GetUIntArrayFromByteArray(result, this.DataFormat)));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 读取Float类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadFloat(StoreArea storeArea, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                    AddLog(2, "不支持以上类型");
                    return;
                case StoreArea.输入寄存器3x:
                    result = Modbus.ReadOutPutRegister(start, (ushort)(length * 2));
                    break;
                case StoreArea.输出寄存器4x:
                    result = Modbus.ReadOutPutRegister(start, (ushort)(length * 2));
                    break;
            }
            if (result != null)
            {
                AddLog(0, "读取成功" + StringLib.GetStringFromValueArray(FloatLib.GetFloatArrayFromByteArray(result, this.DataFormat)));
            }
            else
            {
                AddLog(2, "读取失败,请检查参数问题");
            }
        }
        #endregion

        #region 写入操作
        /// <summary>
        /// 写入bool类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteBool(StoreArea storeArea,ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                    bool[] values = BitLib.GetBitArrayFromBitArrayString(setValue);
                    if (values.Length == 1)
                    {
                        result = Modbus.PreSetSingleCoil(start, values[0]);
                    }
                    else
                    {
                        result = Modbus.PreSetMultiCoils(start, values);
                    }
                    break;
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                case StoreArea.输出寄存器4x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 写入byte类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteByte(StoreArea storeArea, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
                case StoreArea.输出寄存器4x:
                    result = Modbus.PreSetMultiRegister(start, ByteArrayLib.GetByteArrayFromHexString(setValue));
                    break;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 写入short类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteShort(StoreArea storeArea, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
                case StoreArea.输出寄存器4x:
                    short[] values = ShortLib.GetShortArrayFromString(setValue);
                    if (values.Length == 1)
                    {
                        result = Modbus.PreSetSingleRegister(start, values[0]);
                    }
                    result = Modbus.PreSetMultiRegister(start, ByteArrayLib.GetByteArrayFromShortArray(values));
                    break;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 写入int类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteInt(StoreArea storeArea, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
                case StoreArea.输出寄存器4x:
                    int[] values = IntLib.GetIntArrayFromString(setValue);
                    result = Modbus.PreSetMultiRegister(start, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat));
                    break;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 写入uint类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteUInt(StoreArea storeArea, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
                case StoreArea.输出寄存器4x:
                    uint[] values = UIntLib.GetUIntArrayFromString(setValue);
                    result = Modbus.PreSetMultiRegister(start, ByteArrayLib.GetByteArrayFromUIntArray(values));
                    break;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 写入ushort类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteUShort(StoreArea storeArea, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
                case StoreArea.输出寄存器4x:
                    ushort[] values = UShortLib.GetUShortArrayFromString(setValue);
                    if (values.Length == 1)
                    {
                        result = Modbus.PreSetSingleRegister(start, values[0]);
                    }
                    else
                    {
                        result = Modbus.PreSetMultiRegister(start, ByteArrayLib.GetByteArrayFromUShortArray(values));
                    }

                    break;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        /// <summary>
        /// 写入float类型数据
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteFloat(StoreArea storeArea,ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.输出线圈0x:
                case StoreArea.输入线圈1x:
                case StoreArea.输入寄存器3x:
                    AddLog(1, "写入失败，不支持该存储");
                    return;
                case StoreArea.输出寄存器4x:
                    float[] values = FloatLib.GetFloatArrayFromString(setValue);
                    result = Modbus.PreSetMultiRegister(start, ByteArrayLib.GetByteArrayFromFloatArray(values));
                    break;
            }
            if (result)
            {
                AddLog(0, "写入成功");
            }
            else
            {
                AddLog(1, "写入失败,请检查参数问题");
            }
        }
        #endregion
    }
}
