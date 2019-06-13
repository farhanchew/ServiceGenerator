using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ServiceGenerator
{
    class Program
    {
        public static string Path = "";
        //public static string OriginNamespace = "";
        public static DirectoryInfo DirectoryInfo;
        public static FileInfo[] Files;
        public static string ClassName;

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

                var output = new StringBuilder();
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
