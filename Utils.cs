using System.Text;

namespace OBLibrary;

public static class Utils
{
    
    public static Stream GetStreamFromAscii ( string pathIniFile )
    {
        try
        {
            Encoding.RegisterProvider ( CodePagesEncodingProvider.Instance );
            Encoding ascii1251 = Encoding.GetEncoding ( 1251 );

            byte [ ] codepage1251Bytes = File.ReadAllBytes ( pathIniFile );

            byte [ ] utf8Byte = Encoding.Convert ( ascii1251, Encoding.UTF8, codepage1251Bytes );

            return new MemoryStream ( utf8Byte );
        }
        catch
        {
            return new MemoryStream ( );
        }


    }
}