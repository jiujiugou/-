using ModbusRTUlib;
using System.IO.Ports;
using System.Windows.Forms;
using thinger.DataConvertLib;

namespace Modbus.Project
{
    public enum StoreArea
    {
        �����Ȧ0x,
        ������Ȧ1x,
        ����Ĵ���3x,
        ����Ĵ���4x
    }
    public partial class Form1 : Form
    {
        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            InitParam();
        }
        #endregion
        
        #region ����
        /// <summary>
        /// ModbusRTUͨ�Ŷ���
        /// </summary>
        ModbusRTU Modbus = new ModbusRTU();
        /// <summary>
        /// ��ǰ����״̬
        /// </summary>
        private bool IsConnected = false;
        /// <summary>
        /// ���ݸ�ʽ
        /// </summary>
        private DataFormat DataFormat = DataFormat.ABCD;
        #endregion

        #region ��ʼ��
        /// <summary>
        /// ��ʼ������
        /// </summary>
        private void InitParam()
        {
            //��ȡ�����˿ں��б�
            string[] portList = SerialPort.GetPortNames();
            if (portList.Length > 0)
            {
                this.cmb_Port.Items.AddRange(portList);
                this.cmb_Port.SelectedIndex = 0;
            }
            //������
            this.cmb_BoudRate.Items.AddRange(new string[] { "9600", "19200", "38400", "57600", "115200" });
            this.cmb_BoudRate.SelectedIndex = 0;

            //У��λ
            this.cmb_Parity.DataSource = Enum.GetNames(typeof(Parity));
            this.cmb_Parity.SelectedIndex = 0;
            //ֹͣλ
            this.cmb_StopBits.DataSource = Enum.GetNames(typeof(StopBits));
            this.cmb_StopBits.SelectedIndex = 1;
            //����λ
            this.cmb_DataBits.Items.AddRange(new string[] { "7", "8" });
            this.cmb_DataBits.SelectedIndex = 1;
            //��С��
            this.cmb_DataFormat.DataSource = Enum.GetNames(typeof(DataFormat));
            this.cmb_DataFormat.SelectedIndex = 0;
            //�洢��
            this.cmb_StoreArea.DataSource = Enum.GetNames(typeof(StoreArea));
            this.cmb_StoreArea.SelectedIndex = 0;
            //��������
            this.cmb_DataType.DataSource = Enum.GetNames(typeof(DataType));
            this.cmb_DataType.SelectedIndex = 0;
        }
        #endregion
        
        #region �ؼ��¼�
        /// <summary>
        /// ���Ӱ�ť����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtn_Connect_Click(object sender, EventArgs e)
        {
            if (IsConnected)
            {
                AddLog(0, "modbusRTU�Ѿ���������");
                return;
            }
            Parity parity = (Parity)Enum.Parse(typeof(Parity), this.cmb_Parity.Text, true);
            StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), this.cmb_StopBits.Text, true);
            IsConnected = Modbus.Connect(this.cmb_Port.Text, Convert.ToInt32(this.cmb_BoudRate.Text), parity, Convert.ToInt32(this.cmb_DataBits.Text), stopBits);
            if (IsConnected)
            {
                AddLog(0, "modbusRTU���ӳɹ�");
            }
            else
            {
                AddLog(2, "modbusRTU����ʧ��");
            }
        }
        /// <summary>
        /// �Ͽ����Ӱ�ť����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Disconnect_Click(object sender, EventArgs e)
        {
            Modbus.Disconnect();
            IsConnected = false;
            AddLog(0, "modbusRTU�Ͽ�����");
        }
        /// <summary>
        /// ��ȡ��ť����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Read_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                byte devld = byte.Parse(this.txt_Slaveld.Text.Trim());
                ushort start = ushort.Parse(this.txt_Start.Text.Trim());
                ushort length = ushort.Parse(this.txt_Length.Text.Trim());
                DataType dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_DataType.Text, true);
                StoreArea storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.Text, true);
                DataFormat dataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.Text, true);
                switch (dataType)
                {
                    case DataType.Bool:
                        ReadBool(storeArea, devld, start, length);
                        break;
                    case DataType.Byte:
                        ReadByte(storeArea, devld, start, length);
                        break;
                    case DataType.UShort:
                        ReadUShort(storeArea, devld, start, length);
                        break;
                    case DataType.Short:
                        ReadShort(storeArea, devld, start, length);
                        break;
                    case DataType.UInt:
                        ReadUInt(storeArea, devld, start, length);
                        break;
                    case DataType.Int:
                        ReadInt(storeArea, devld, start, length);
                        break;
                    case DataType.Float:
                        ReadFloat(storeArea, devld, start, length);
                        break;
                    case DataType.Long:
                    case DataType.Double:
                    case DataType.String:
                    case DataType.ByteArray:
                        AddLog(2, "��֧����������");
                        return;
                }
            }
        }
        /// <summary>
        /// д�밴ť����¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Write_Click(object sender, EventArgs e)
        {
            if (CommonVerify())
            {
                byte devld = byte.Parse(this.txt_Slaveld.Text.Trim());
                ushort start = ushort.Parse(this.txt_Start.Text.Trim());
                DataType dataType = (DataType)Enum.Parse(typeof(DataType), this.cmb_DataType.Text, true);
                StoreArea storeArea = (StoreArea)Enum.Parse(typeof(StoreArea), this.cmb_StoreArea.Text, true);
                DataFormat dataFormat = (DataFormat)Enum.Parse(typeof(DataFormat), this.cmb_DataFormat.Text, true);
                string setValue = this.txt_WriteValue.Text.Trim();                
                switch (dataType)
                {
                    case DataType.Bool:
                        WriteBool(storeArea, devld, start,setValue);
                        break;
                    case DataType.Byte:
                        WriteByte(storeArea, devld, start, setValue);
                        break;
                    case DataType.UShort:
                        WriteUShort(storeArea, devld, start, setValue);
                        break;
                    case DataType.Short:
                        WriteShort(storeArea, devld, start, setValue);
                        break;
                    case DataType.UInt:
                        WriteUInt(storeArea, devld, start, setValue);
                        break;
                    case DataType.Int:
                        WriteInt(storeArea, devld, start, setValue);
                        break;
                    case DataType.Float:
                        WriteFloat(storeArea, devld, start, setValue);
                        break;
                    case DataType.Long:
                    case DataType.Double:
                    case DataType.String:
                    case DataType.ByteArray:
                        AddLog(2, "��֧����������");
                        return;
                }
            }
        }
        #endregion
        
        #region ͨ�÷���
        /// <summary>
        /// ͨ����־��¼
        /// </summary>
        /// <param name="level"></param>
        /// <param name="info"></param>
        private void AddLog(int level, string info)
        {
            ListViewItem item = new ListViewItem(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), level);
            item.SubItems.Add(info);
            //�����µ�������������
            this.lst_info.Items.Insert(0, item);
        }
        /// <summary>
        /// ͨ�ò���У��
        /// </summary>
        /// <returns></returns>
        private bool CommonVerify()
        {
            if (!IsConnected)
            {
                AddLog(1, "modbusRTUδ��������");
                return false;
            }
            if (byte.TryParse(this.txt_Slaveld.Text, out _) == false)
            {
                AddLog(1, "���վ��ַ��ʽ�Ƿ���ȷ");
                return false;
            }
            if (ushort.TryParse(this.txt_Start.Text, out _) == false)
            {
                AddLog(1, "�����ʼ��ַ��ʽ�Ƿ���ȷ");
                return false;
            }
            if (ushort.TryParse(this.txt_Length.Text, out _) == false)
            {
                AddLog(1, "��鳤�ȸ�ʽ�Ƿ���ȷ");
                return false;
            }
            return true;
        }
        #endregion

        #region ��ȡ����
        /// <summary>
        /// ��ȡbool��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadBool(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                    result = Modbus.ReadOutputColls(devld, start, length);
                    break;
                case StoreArea.������Ȧ1x:
                    result = Modbus.ReadInputColls(devld, start, length);
                    break;
                case StoreArea.����Ĵ���3x:
                case StoreArea.����Ĵ���4x:
                    AddLog(1, "��ȡʧ�ܣ���֧�ָô洢");
                    return;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(BitLib.GetBitArrayFromByteArray(result, 0, length)));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        /// <summary>
        /// ��ȡbyte��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadByte(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                    result = Modbus.ReadOutputColls(devld, start, length);
                    break;
                case StoreArea.������Ȧ1x:
                    result = Modbus.ReadInputColls(devld, start, length);
                    break;
                case StoreArea.����Ĵ���3x:
                    result = Modbus.ReadOutInputRegisters(devld, start, length);
                    break;
                case StoreArea.����Ĵ���4x:
                    result = Modbus.ReadOutPutRegisters(devld, start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(result));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        /// <summary>
        /// ��ȡshort��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadShort(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                    AddLog(2, "��֧����������");
                    return;
                case StoreArea.����Ĵ���3x:
                    result = Modbus.ReadOutInputRegisters(devld, start, length);
                    break;
                case StoreArea.����Ĵ���4x:
                    result = Modbus.ReadOutPutRegisters(devld, start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(ShortLib.GetShortArrayFromByteArray(result)));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        /// <summary>
        /// ��ȡUshort��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadUShort(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                    AddLog(2, "��֧����������");
                    return;
                case StoreArea.����Ĵ���3x:
                    result = Modbus.ReadOutInputRegisters(devld, start, length);
                    break;
                case StoreArea.����Ĵ���4x:
                    result = Modbus.ReadOutPutRegisters(devld, start, length);
                    break;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(UShortLib.GetUShortArrayFromByteArray(result)));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        /// <summary>
        /// ��ȡInt��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadInt(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                    AddLog(2, "��֧����������");
                    return;
                case StoreArea.����Ĵ���3x:
                    result = Modbus.ReadOutInputRegisters(devld, start, (ushort)(length * 2));
                    break;
                case StoreArea.����Ĵ���4x:
                    result = Modbus.ReadOutPutRegisters(devld, start, (ushort)(length * 2));
                    break;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(IntLib.GetIntArrayFromByteArray(result, this.DataFormat)));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        /// <summary>
        /// ��ȡUInt��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadUInt(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                    AddLog(2, "��֧����������");
                    return;
                case StoreArea.����Ĵ���3x:
                    result = Modbus.ReadOutInputRegisters(devld, start, (ushort)(length * 2));
                    break;
                case StoreArea.����Ĵ���4x:
                    result = Modbus.ReadOutPutRegisters(devld, start, (ushort)(length * 2));
                    break;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(UIntLib.GetUIntArrayFromByteArray(result, this.DataFormat)));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        /// <summary>
        /// ��ȡFloat��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        private void ReadFloat(StoreArea storeArea, byte devld, ushort start, ushort length)
        {
            byte[] result = null;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                    AddLog(2, "��֧����������");
                    return;
                case StoreArea.����Ĵ���3x:
                    result = Modbus.ReadOutInputRegisters(devld, start, (ushort)(length * 2));
                    break;
                case StoreArea.����Ĵ���4x:
                    result = Modbus.ReadOutPutRegisters(devld, start, (ushort)(length * 2));
                    break;
            }
            if (result != null)
            {
                AddLog(0, "��ȡ�ɹ�" + StringLib.GetStringFromValueArray(FloatLib.GetFloatArrayFromByteArray(result, this.DataFormat)));
            }
            else
            {
                AddLog(2, "��ȡʧ��,�����������");
            }
        }
        #endregion

        #region д�����
        /// <summary>
        /// д��bool��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteBool(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                    bool[] values=BitLib.GetBitArrayFromBitArrayString(setValue);
                    if (values.Length == 1)
                    {
                        result=Modbus.PreSetSingleCoil(devld, start, values[0]);
                    }
                    else
                    {
                        result = Modbus.PresetMultCoils(devld, start, values);
                    }
                    break;
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                case StoreArea.����Ĵ���4x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        /// <summary>
        /// д��byte��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteByte(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
                case StoreArea.����Ĵ���4x:
                    result=Modbus.PreSetMultiRegisters(devld, start, ByteArrayLib.GetByteArrayFromHexString(setValue));
                    break;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        /// <summary>
        /// д��short��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteShort(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
                case StoreArea.����Ĵ���4x:
                    short[] values= ShortLib.GetShortArrayFromString(setValue);
                    if (values.Length == 1)
                    {
                        result=Modbus.PreSetSingleRegister(devld, start, values[0]);
                    }
                    result = Modbus.PreSetMultiRegisters(devld, start, ByteArrayLib.GetByteArrayFromShortArray(values));
                    break;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        /// <summary>
        /// д��int��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteInt(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
                case StoreArea.����Ĵ���4x:
                    int[] values = IntLib.GetIntArrayFromString(setValue);
                    result=Modbus.PreSetMultiRegisters(devld, start, ByteArrayLib.GetByteArrayFromIntArray(values, this.DataFormat));
                    break;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        /// <summary>
        /// д��uint��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteUInt(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
                case StoreArea.����Ĵ���4x:
                    uint[] values = UIntLib.GetUIntArrayFromString(setValue);
                    result = Modbus.PreSetMultiRegisters(devld, start, ByteArrayLib.GetByteArrayFromUIntArray(values));
                    break;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        /// <summary>
        /// д��ushort��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteUShort(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
                case StoreArea.����Ĵ���4x:
                    ushort[] values = UShortLib.GetUShortArrayFromString(setValue);
                    if (values.Length == 1)
                    {
                        result = Modbus.PreSetSingleRegister(devld, start, values[0]);
                    }
                    else
                    {
                        result = Modbus.PreSetMultiRegisters(devld, start, ByteArrayLib.GetByteArrayFromUShortArray(values));
                    }
                    
                    break;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        /// <summary>
        /// д��float��������
        /// </summary>
        /// <param name="storeArea"></param>
        /// <param name="devld"></param>
        /// <param name="start"></param>
        /// <param name="setValue"></param>
        private void WriteFloat(StoreArea storeArea, byte devld, ushort start, string setValue)
        {
            bool result = false;
            switch (storeArea)
            {
                case StoreArea.�����Ȧ0x:
                case StoreArea.������Ȧ1x:
                case StoreArea.����Ĵ���3x:
                    AddLog(1, "д��ʧ�ܣ���֧�ָô洢");
                    return;
                case StoreArea.����Ĵ���4x:
                    float[] values = FloatLib.GetFloatArrayFromString(setValue);
                    result = Modbus.PreSetMultiRegisters(devld, start, ByteArrayLib.GetByteArrayFromFloatArray(values));
                    break;
            }
            if (result)
            {
                AddLog(0, "д��ɹ�");
            }
            else
            {
                AddLog(1, "д��ʧ��,�����������");
            }
        }
        #endregion
    }
}
