using System;
using System.IO;

namespace vcdestroy
{
    public class vcdestroyProcessor
    {
        /// <summary>
        /// This is the main method for purging files
        /// </summary>
        /// <param name="iterations"></param>
        /// <param name="fileSpec"></param>
        public vcdestroyProcessor(int iterations,  string fileSpec, bool nodelete)
        {
            DateTime startTime = DateTime.Now;

            wipe(fileSpec, iterations, nodelete);

            TimeSpan elapsedTime = DateTime.Now - startTime;
            Console.WriteLine("{0} Destroyed in {1:f1} Seconds", fileSpec,  elapsedTime.TotalSeconds);
        }

        private void wipe(string filespec, int iterations, bool nodelete)
        {
            const int BUFFER_SIZE = 65536;

            FileInfo File = new FileInfo(filespec);
            if (!File.Exists)
                throw new FileNotFoundException();


                byte[] buffer = new byte[BUFFER_SIZE];
                Random rnd = new Random();
                FileStream fs = File.OpenWrite();
            Console.WriteLine("Wiping headers");
            // overwrite headers 99 times
            for (int i = 0; i < 99; i++)
            {

                // write 65K random bytes at the beginning of the file
                //   this wipes out the volume header and master keys
                fs.Seek(0, SeekOrigin.Begin);
                rnd.NextBytes(buffer);
                int BytesToWrite = (int)Math.Min(BUFFER_SIZE, File.Length);
                fs.Write(buffer, 0, BytesToWrite);

                // if the file size is more than 65K the write random bytes at the end of the file
                //   this wipes out the backup volume header for any hidden volume
                long endbytes = File.Length - BUFFER_SIZE;
                if (endbytes > 0)
                {
                    fs.Seek(-BUFFER_SIZE, SeekOrigin.End);
                    rnd.NextBytes(buffer);
                    fs.Write(buffer, 0, BUFFER_SIZE);
                }

                // write to second set of 65K
                //     this wipes out the backup header for the hidden volume

                if (File.Length > BUFFER_SIZE)
                {
                    fs.Seek(BUFFER_SIZE, SeekOrigin.Begin);
                    rnd.NextBytes(buffer);
                    BytesToWrite = (int)Math.Min(BUFFER_SIZE, File.Length - BUFFER_SIZE);
                    fs.Write(buffer, 0, BytesToWrite);
                }


                // write to second from end set of 65K
                //      this wipes out the backup header for the main volume
                if (File.Length > BUFFER_SIZE * 2)
                {
                    fs.Seek(-BUFFER_SIZE * 2, SeekOrigin.End);
                    rnd.NextBytes(buffer);
                    fs.Write(buffer, 0, BUFFER_SIZE);
                }

                fs.Flush();
            }

            // write random bytes to random locations throughout the rest of the file
            if (File.Length > 2* BUFFER_SIZE)
            {
                Console.WriteLine("Writing random bytes in random locations");
                while (iterations-- > 0)
                {
                    long location = (long) (rnd.NextDouble() * (File.Length - 2*BUFFER_SIZE)) + BUFFER_SIZE;
                    fs.Seek(location, SeekOrigin.Begin);
                    int bytes = rnd.Next(1, BUFFER_SIZE);
                    rnd.NextBytes(buffer);
                    fs.Write(buffer, 0, bytes);
                }
            }

               fs.Dispose();

            if (!nodelete)
               File.Delete();
        }
    }
}
