using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DealerManagementSystem.CrystalReportToPdf
{
    public static class ConvertData
    {
        public static byte[] StreamToBytes(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}