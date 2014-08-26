using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace De.AHoerstemeier.Tambon
{
    /// <summary>
    /// Some basic helper methods.
    /// </summary>
    public static class BasicHelper
    {
        /// <summary>
        /// Copies the content of one stream into a second.
        /// </summary>
        /// <param name="input">Input stream.</param>
        /// <param name="output">Output stream.</param>
        public static void StreamCopy(Stream input, Stream output)
        {
            byte[] buffer = new byte[2048];
            int readCount = 0;

            do
            {
                readCount = input.Read(buffer, 0, buffer.Length);
                output.Write(buffer, 0, readCount);
            } while ( readCount > 0 );
        }
    }
}