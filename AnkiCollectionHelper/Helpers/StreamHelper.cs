using System;
using System.IO;
using System.Text;

namespace AnkiCollectionHelper.Helpers
{
    public static class StreamHelper
    {
        public static MemoryStream FileToMemoryStream(string fileName)
        {
            const int bufferSize = 32768;

            using (var inputFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var fileInfo = new FileInfo(fileName);

                var result = new MemoryStream((int)fileInfo.Length);

                var buffer = new byte[bufferSize];
                int len;
                while ((len = inputFileStream.Read(buffer, 0, bufferSize)) > 0)
                {
                    result.Write(buffer, 0, len);
                }

                result.Flush();

                result.Seek(0, SeekOrigin.Begin);
                return result;
            }
        }

        public static MemoryStream FileToMemoryStream(string fileName, int start, int length)
        {
            using (var inputFileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var result = new MemoryStream(length);
                inputFileStream.Seek(start, SeekOrigin.Begin);

                var buffer = new byte[length];
                int len = inputFileStream.Read(buffer, 0, length);
                result.Write(buffer, 0, len);
                result.Flush();
                result.Seek(0, SeekOrigin.Begin);

                return result;
            }
        }

        public static void WriteStreamToFile(Stream stream, string fileName)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            const int bufferSize = 32768;

            using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                var buffer = new byte[bufferSize];
                int len;
                while ((len = stream.Read(buffer, 0, bufferSize)) > 0)
                {
                    fileStream.Write(buffer, 0, len);
                }
                fileStream.Flush();
            }
        }

        public static string StreamToStringUtf8(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            stream.Position = 0;
            if (stream.CanRead && stream.CanSeek)
            {
                var by = new byte[stream.Length];
                stream.Read(by, 0, Convert.ToInt32(stream.Length));
                stream.Position = 0;
                return Encoding.UTF8.GetString(by);
            }

            throw new ArgumentException();
        }

        public static string StreamToString(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static string StreamToString(Stream stream, Encoding encoding)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            using (var streamReader = new StreamReader(stream, encoding))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static MemoryStream StringToStream(string str, bool writable)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            char[] buffer = str.ToCharArray();
            return new MemoryStream(Encoding.UTF8.GetBytes(buffer), writable);
        }

        public static MemoryStream StringToStream(string str, Encoding encoding)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }

            char[] buffer = str.ToCharArray();
            return new MemoryStream(encoding.GetBytes(buffer));
        }

        public static void CopyStream(Stream inputStream, Stream outputStream)
        {
            const long maxBufferSize = 4096;

            long bufferSize = inputStream.Length < maxBufferSize ? inputStream.Length : maxBufferSize;
            var buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                outputStream.Write(buffer, 0, bytesRead);
            }
        }
    }
}