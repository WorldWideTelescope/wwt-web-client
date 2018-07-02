using System;
using System.Html;




namespace wwtlib
{
    public class BinaryReader
    {
        public int position = 0;
        Uint8Array data = null;

        public int Position
        {
            get { return position; }
        }

        public void Seek(int pos)
        {
            position = pos;
        }

        public void SeekRelative(int pos)
        {
            position += pos;
        }

        public int Length
        {
            get { return data.length; }
        }

        public BinaryReader(Uint8Array arraybuf)
        {
            data = arraybuf;
        }

        public byte ReadByte()
        {
            byte result;
            result = this.data[this.position];
            this.position += 1;

            return result;
        }

        public sbyte ReadSByte()
        {
            sbyte result;
            result = (sbyte)this.data[this.position];
            this.position += 1;

            return result;
        }

        public byte[] ReadBytes(int count)
        {

            byte[] buf = new byte[count];

            for (int i = 0; i < count; i++)
            {
                buf[i] = this.data[this.position + i];
            }

            this.position += count;

            return buf;
        }

        public string ReadByteString(int count)
        {
            string data = "";

            for (int i = 0; i < count; i++)
            {
                data += string.FromCharCode(this.data[this.position + i]);
            }

            this.position += count;

            return data;
        }

        public float ReadSingle()
        {
            //if (this.data.length < Position + 4)
            //{
            //    throw 'range error';

            //}

            Uint8Array tmp = new Uint8Array(4);

            tmp[0] = this.data[this.position];
            tmp[1] = this.data[this.position + 1];
            tmp[2] = this.data[this.position + 2];
            tmp[3] = this.data[this.position + 3];

            float result = (new Float32Array(tmp.buffer, 0, 1))[0];

            this.position += 4;

            return result;
        }
        public UInt32 ReadUInt32()
        {

            //if (this.data.length < this.position + 4)
            //{
            //    throw 'range error';
            //}
            UInt32 result = (UInt32)(this.data[this.position] + (this.data[this.position + 1] << 8) + (this.data[this.position + 2] << 16) + (this.data[this.position + 3] << 24));
            this.position += 4;
            return result;

        }
        public UInt16 ReadUInt16()
        {

            UInt16 result = (UInt16)(this.data[this.position] + (this.data[this.position + 1] << 8) );
            this.position += 2;
            return result;

        }

        public Int32 ReadInt32()
        {

            UInt32 result = this.ReadUInt32();

            if ((result & 0x80000000) != 0)
            {

                return (Int32)(-((result - 1) ^ 0xffffffff));

            }

            return (Int32)result;

        }
        public static int id = 1;
        public Int64 ReadInt64()
        {
            this.position += 8;
            return id++;
        }

        public void Close()
        {

        }
    }
}
