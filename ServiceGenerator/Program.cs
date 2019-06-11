using System;
using System.IO;
using System.Linq;

namespace ServiceGenerator
{
    class Program
    {
        public static string Path = "";
        public static string OriginNamespace = "";
        public static string TargetNamespace = "";
        public static DirectoryInfo DirectoryInfo;
        public static FileInfo[] Files;
        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("Enter Target Namespace Name");
                var TargetNamespace = Console.ReadLine();
                Console.WriteLine("Enter Domain/Model Path");
                Path = Console.ReadLine();
                GetFileInfos();
                GenerateService();
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        static void GenerateService()
        {
            Console.WriteLine($"Generating Services . . .");
            foreach (FileInfo file in Files)
            {
                string contents = File.ReadAllText(file.FullName);
                string[] lines = contents.Replace("\r", "").Split('\n');
                if(string.IsNullOrEmpty(OriginNamespace))
                {
                    var nspace = lines.Where(x => x.Contains("namespace")).FirstOrDefault();
                    if(!string.IsNullOrEmpty(nspace))
                    {
                        OriginNamespace = nspace.Replace("namespace ", string.Empty);
                        Console.WriteLine($"Origin Namespace : {OriginNamespace}");
                    }
                }
            }
        }

        static void GetFileInfos()
        {
            DirectoryInfo = new DirectoryInfo(@Path);//Assuming Test is your Folder
            Files = DirectoryInfo.GetFiles("*.cs"); //Getting Text files
            if (!Files.Any())
            {
                Console.WriteLine("No File Found");
                return;
            }
            string fname = "";
            foreach (FileInfo file in Files)
            {
                fname = fname + ", " + file.Name;
            }
            Console.WriteLine($"Found : {fname}");
        }
    }
}
