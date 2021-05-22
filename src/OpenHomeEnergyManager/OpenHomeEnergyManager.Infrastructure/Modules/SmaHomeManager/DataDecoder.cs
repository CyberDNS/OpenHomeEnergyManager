using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenHomeEnergyManager.Infrastructure.Modules.SmaHomeManager
{
    class DataDecoder
    {
        private static Dictionary<int, string> _valueDefinitions = new Dictionary<int, string>()
        {
            {1, "pconsume"},
            {2, "psupply"},
            {3, "sconsume" },
            {4, "ssupply"},
            {9,"qconsume"},
            {10, "qsupply"},
            {13, "cosphi"},
            {14, "frequency"},
            // phase 1
            {21, "p1consume"},
            {22, "p1supply"},
            {23, "s1consume"},
            {24, "s1supply"},
            {29, "q1consume"},
            {30, "q1supply"},
            {31, "i1"},
            {32, "u1"},
            {33, "cosphi1"},
            // phase 2
            {41, "p2consume"},
            {42, "p2supply"},
            {43, "s2consume"},
            {44, "s2supply"},
            {49, "q2consume"},
            {50, "q2supply"},
            {51, "i2"},
            {52, "u2"},
            {53, "cosphi2"},
            // phase 3
            {61, "p3consume"},
            {62, "p3supply"},
            {63, "s3consume"},
            {64, "s3supply"},
            {69, "q3consume"},
            {70, "q3supply"},
            {71, "i3"},
            {72, "u3"},
            {73, "cosphi3"},
            // common
            {36864, "speedwire-version"}
        };


        public static (string Name, DataType Type) DecodeValueHeader(byte[] data)
        {
            int valueId = BitConverter.ToUInt16(data[0..2].Reverse().ToArray());
            byte typeId = data[2];

            DataType dataType = DataType.Unknown;

            switch (typeId)
            {
                case 4:
                    dataType = DataType.Current;
                    break;
                case 8:
                    dataType = DataType.Counter;
                    break;
                case 0:
                    if (valueId == 36864)
                    {
                        dataType = DataType.Version;
                    }
                    break;
            }

            return (Name: _valueDefinitions[valueId], Type: dataType);
        }


        public enum DataType
        {
            Current,
            Counter,
            Version,
            Unknown
        }
    }
}
