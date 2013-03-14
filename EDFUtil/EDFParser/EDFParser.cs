using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace yGet.EDFParser
{
    public class EDFConstants
    {
        public const int HeaderLength = 64;
    }

    public static class EDFBlockName
    {
        public const String FCDRW = "46434452";
        public const String RGTL = "5247544C";
        public const String EDWK = "4544574B";

    }



    public class EDFParser: IDisposable 
    {
        #region Private variables
        private byte[] headerData = null;
        private String _edfParserFile;
        private BinaryReader binaryReader;
        private EdfHeader edfHeader = null;
        private Dictionary<String, EdfComponentInfo> edfComponentInfoDictionary;
        private String _edfFilePath;
        #endregion

        public EDFParser(String fileName)
        {
            headerData = new byte[64];
            binaryReader = null;
            edfComponentInfoDictionary = new Dictionary<string, EdfComponentInfo>();
            _edfFilePath = fileName ;
            OpenBinaryReader(); 
        }

        

        public String EDFFile
        {
            get
            {
                return _edfParserFile;
            }
        }


        public bool GetComponentData(String componentName, out Byte[] componentData)
        {
            bool retResult = false;
             
            EdfComponentInfo edfComponentInfo = GetComponentInfo(componentName);
            componentData = null;
            if (null != edfComponentInfo)
            {
                componentData = new Byte[edfComponentInfo.Pitch];
                binaryReader.BaseStream.Position = edfComponentInfo.Offset;  
                int length = binaryReader.BaseStream.Read(componentData,0, componentData.Length);
                if (length == componentData.Length)
                    retResult = true;
            }
            
            return retResult;

        }

        public bool IsDrawingSheetNamePresent()
        {
            String constValidator = "MXDB:";
            String prefix = "ST:";
            bool isPresent = false;
            Byte[] componentData = null;
            bool isSuccess = GetComponentData(EDFBlockName.EDWK,out componentData );
            if (isSuccess)
            {
                String componentString = ConversionUtil.GetAsciiString(componentData);
                if (0 == String.Compare(componentString, 0, constValidator,0, constValidator.Length, true))
                {
                    int prefixIndex = componentString.IndexOf(prefix);
                    prefixIndex = prefixIndex + prefix.Length;   
                    int teminalIndex = componentString.LastIndexOf(";");
                    if ((teminalIndex - prefixIndex) > 0)
                    {
                        isPresent = true;
                    }
  
                }
            }
            return isPresent;
        }
        

        public EdfHeader  GetEdfHeader()
        {
            EdfHeader  edfHeader = null;
            long length = binaryReader.BaseStream.Length;
            if( length >= EDFConstants.HeaderLength)  
            {
                binaryReader.Read(headerData,0,EDFConstants.HeaderLength);
                edfHeader = new EdfHeader(headerData);  
            }
            return edfHeader;
        }

        #region Private Methods

        private void OpenBinaryReader()
        {
            if (binaryReader == null)
            {
                binaryReader = new BinaryReader(File.Open(_edfFilePath, FileMode.Open, FileAccess.Read));
                
            }
        }

        private void CloseBinaryReader()
        {
            binaryReader.Close();
            binaryReader = null;
        }

        private EdfComponentInfo GetComponentInfo(String componentName)
        {
            EdfComponentInfo edfContent = null;
            int position = 0;

            long length = binaryReader.BaseStream.Length;
            Byte[] edfComponentDataTemp = new Byte[16];
            if (!edfComponentInfoDictionary.ContainsKey(componentName))
            {
                while (position < length)
                {
                    if (binaryReader.BaseStream.Read(edfComponentDataTemp, 0, edfComponentDataTemp.Length) == 16)
                    {
                        if (ConversionUtil.GetHexString(edfComponentDataTemp, 4).Equals(componentName))
                        {
                            edfComponentInfoDictionary.Add(componentName, new EdfComponentInfo(edfComponentDataTemp));
                            break;
                        }
                    }
                    position += 16;
                }
            }
            if (edfComponentInfoDictionary.ContainsKey(componentName))
            {
                edfContent = edfComponentInfoDictionary[componentName];
            }
            return edfContent;
        }

        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            CloseBinaryReader();
            binaryReader = null;
        }

        #endregion
    }

    #region Classes representing EDF sub component Structures
    public class EdfHeader
    {
        Byte[] _FileName;//12
	    UInt32 _EditorID;//4
	    Byte[] _ProName;//8
	    Byte[] _HostName;//8
	    Byte[] _dummy;//12
	    UInt32 _ItemNumber;//4
	    UInt32 _CreateTime;//4
	    UInt32 _ModifyTime;//4
	    Byte[] _FileType;//8

        public EdfHeader( byte[] headerData)
        {
            if (headerData.Length != 64)
            {
                throw new Exception ("Invalid Length");
            }
            _FileName = new byte[12];
            Buffer.BlockCopy(headerData, 0, _FileName, 0, _FileName.Length);
            
         }
        public String FileName 
        {
            get
            {
                return ConversionUtil.GetHexString(_FileName)   ; 
            }
        }


    }

    public class EdfComponentInfo
    {
        String _componentName;//4
        UInt32 _Offset;//4
        UInt32 _Number;//4
        UInt32 _Pitch;//4

        public EdfComponentInfo(byte[] edfContent)
        {
            if (edfContent.Length != 16)
            {
                throw new Exception("Invalid Content Length");
            }
            
            _componentName = ConversionUtil.GetHexString(edfContent,4) ;
            _Offset  = (UInt32)(edfContent[4] | (edfContent[5] << 8) | (edfContent[6] << 16)|(edfContent[7] << 24));
            _Number = (UInt32)(edfContent[8] | (edfContent[9] << 8) | (edfContent[10] << 16) | (edfContent[11] << 24));
            _Pitch  = (UInt32)(edfContent[12] | (edfContent[13] << 8) | (edfContent[14] << 16) | (edfContent[15] << 24));
            
        }

        public String ComponentName
        {
            get { return _componentName; }
        }

        public UInt32 Offset
        {
            get{return _Offset ;}
        }
        public UInt32 Number
        {
            get { return _Number; }
        }

        public UInt32 Pitch
        {
            get { return _Pitch; }
        }

    }
    #endregion

    #region Utility class
    public static class ConversionUtil
    {
        public static String GetHexString(Byte[] hexByteArray)
        {
            String hexString = String.Empty;

            StringBuilder sb = new StringBuilder();
            foreach (Byte hexByte in hexByteArray)
            {
                sb.Append(hexByte.ToString("X02"));
            }
            hexString = sb.ToString();
            return hexString;
        }

        public static String GetAsciiString(Byte[] hexByteArray)
        {
            String hexString = String.Empty;
            char[] chararr = new char[hexByteArray.Length];

            for (UInt32 index = 0; index < hexByteArray.Length; index++)
            {
                chararr[index] = Convert.ToChar(hexByteArray[index]);
            }
            return (new String(chararr)).Replace("\0","") ; 
        }

        public static String GetHexString(Byte[] hexByteArray, UInt32 length)
        {
            String hexString = String.Empty;

            StringBuilder sb = new StringBuilder();
            for (UInt32 startIndex = 0; startIndex < length; startIndex++)
            {
                sb.Append(hexByteArray[startIndex].ToString("X02"));
            }
            hexString = sb.ToString();
            return hexString;
        }

    }

    #endregion
}
