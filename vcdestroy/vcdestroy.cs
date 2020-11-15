using System;
using System.IO;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Data;

namespace vcdestroy
{
    /// <summary>
    /// Entrypoint for utility
    /// Get parameters and then call the vcdestroyprocessor
    /// </summary>
    class vcdestroy
    {
        static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<int>(
                    aliases: new string[]{"--iterations", "-i" },
                    getDefaultValue: () => 1000,
                    description: "Number of random passes to make after destroying headers"),

                new Option<bool>(
                    aliases: new string[]{"--generate", "-g" },
                    description: "Generate a test file of binary zeros"),

                new Option<bool>(
                    aliases: new string[]{"--all", "-a" },
                    description: "Wipe all bytes in the file"),

                new Option<bool>(
                    aliases: new string[]{"--nodelete", "-n" },
                    description: "Don't delete the file."),

                new Option<int>(
                    aliases: new string[]{"--size", "-s" },
                    getDefaultValue: () => 1,
                    description: "Size of test file in MB"),

                new Argument<string>("FileSpec",
                    description: "File path of file to generate or destroy" )
            };

            rootCommand.Description = "Obilterates a TrueCrypt or VeraCrypt container";

            rootCommand.Handler = CommandHandler.Create<int, bool, bool, bool, int, string>
                ((iterations, generate, all, nodelete, size, fileSpec) =>
                {

                    Console.WriteLine($"File Spec = {fileSpec}");
                    if (!generate) Console.WriteLine($"Iterations = {iterations}");
                    if (!generate) Console.WriteLine($"All = {all}");
                    if (generate) Console.WriteLine($"gen = {generate}");
                    if (!generate) Console.WriteLine($"Don't delete = {nodelete}");
                    if (generate) Console.WriteLine($"size = {size}");

                    if (generate)
                    {
                        FileStream fs = new FileStream(fileSpec, FileMode.Create);

                        while (size-- > 0)
                        {
                            byte[] buffer = new byte[1024 * 1024];
                            Array.Clear(buffer, 0, buffer.Length);
                            fs.Write(buffer, 0, buffer.Length);
                        }
                        fs.Dispose();
                    }
                    else
                    { 

                    vcdestroyProcessor pp = new vcdestroyProcessor(iterations, fileSpec, all, nodelete);
                    }
                   
                });

            return rootCommand.InvokeAsync(args).Result;

        }
    }
}
