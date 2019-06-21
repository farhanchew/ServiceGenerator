using System;
using System.Collections.Generic;
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

        public static string Entity;
        public static string Plural;
        public static string ContextClass;

        public static string NamespaceName;

        static void Main(string[] args)
        {

            try
            {
                Console.WriteLine("Enter DbContext Path");
                Path = Console.ReadLine();
                Console.WriteLine("Enter Namespace Name, (namespace wont generate if empty)");
                Console.WriteLine("Tags can be used : [Entity]");
                Console.WriteLine("Example: ProjectName.Application.[Entity].Service");
                Console.WriteLine("Example: ProjectName.Application.Service");
                NamespaceName = Console.ReadLine();
                GenerateService();
            }
            catch (Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        static void GenerateService()
        {
            try
            {
                Console.WriteLine($"Generating Services . . .");
                string contents = File.ReadAllText(@Path);
                var lines = contents.Replace("\r", "").Split('\n').ToList();
                List<string> trimmedLines = new List<string>();
                foreach (var l in lines)
                {
                    if (!string.IsNullOrEmpty(l))
                    {
                        var trimmed = l.Trim();
                        if (trimmed != null)
                        {
                            trimmedLines.Add(l.Trim());
                        }
                    }
                }
                var classline = trimmedLines.Where(x => x.Contains("class") && x != null && !x[0].Equals('/')).FirstOrDefault();

                ContextClass = classline
                    .Replace("public", string.Empty)
                    .Replace("private", string.Empty)
                    .Replace("protected", string.Empty)
                    .Replace("partial", string.Empty)
                    .Replace("class", string.Empty)
                    .Replace("sealed", string.Empty).Split(':')
                    .First().Trim();

                var dbSets = trimmedLines.Where(x => x.Contains("DbSet<") && x != null && !x[0].Equals('/')).ToList();
                foreach(var db in dbSets)
                {
                    var filtered = db
                        .Replace("public", string.Empty)
                        .Replace("private", string.Empty)
                        .Replace("protected", string.Empty)
                        .Replace("partial", string.Empty)
                        .Replace("virtual", string.Empty)
                        .Replace("class", string.Empty)
                        .Replace("sealed", string.Empty)
                        .Replace("DbSet<",string.Empty)
                        .Replace(">",string.Empty)
                        .Replace("{",string.Empty)
                        .Replace("}", string.Empty)
                        .Replace("get;", string.Empty)
                        .Replace("get ;", string.Empty)
                        .Replace("set;", string.Empty)
                        .Replace("set ;", string.Empty)
                        .Trim();
                    Console.WriteLine(filtered);
                    var splitted = filtered.Split(' ').ToList();
                    if (splitted.Count() != 2)
                    {
                        Console.WriteLine("Invalid Format");
                        return;
                    }
                    Entity = splitted.First();
                    Plural = splitted.Last();
                    

                    var output = new StringBuilder();
                    if(!string.IsNullOrEmpty(NamespaceName))
                    {
                        output.AppendLine("using System;                       ");
                        output.AppendLine("using System.Threading.Tasks;       ");
                        output.AppendLine("using System.Collections.Generic;   ");
                        output.AppendLine("using System.Linq;                  ");
                        output.AppendLine("using System.Linq.Expressions;      ");
                        output.AppendLine("using Microsoft.EntityFrameworkCore;");
                        output.AppendLine("");
                        output.AppendLine($"namespace {NamespaceName}");
                        output.AppendLine("{");

                    }
                    output.AppendLine("    public class [Entity]Service");
                    output.AppendLine("    {");

                    //Get All
                    output.AppendLine("        public static async Task<(List<[Entity]>,string)> GetAll[Plural]([ContextClass] db, bool include = false)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                List<[Entity]> results = new List<[Entity]>();");
                    output.AppendLine("                results = include ? await db.[Plural]");
                    output.AppendLine("                    //.Include(x => x.)");
                    output.AppendLine("                    .ToListAsync()");
                    output.AppendLine("                    : await db.[Plural].ToListAsync();");
                    output.AppendLine("                ");
                    output.AppendLine("                return (results, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Get by predicate
                    output.AppendLine("        public static async Task<(List<[Entity]>,string)> Get[Plural]([ContextClass] db, Expression<Func<[Entity], bool>> predicate, bool include = false)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                var results = include ? await db.[Plural].Where(predicate)");
                    output.AppendLine("                    .ToListAsync()");
                    output.AppendLine("                    : await db.[Plural].Where(predicate).ToListAsync();");
                    output.AppendLine("                return (results, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null, ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Get by Id
                    output.AppendLine("        public static async Task<([Entity],string)> Get[Entity]ById([ContextClass] db, int id, bool include = false)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                var result = include ? await db.[Plural].Where(x => x.Id == id)");
                    output.AppendLine("                    //.Include(x => x.)");
                    output.AppendLine("                    .FirstOrDefaultAsync()");
                    output.AppendLine("                    : await db.[Plural].Where(x => x.Id == id).FirstOrDefaultAsync();");
                    output.AppendLine("");
                    output.AppendLine("                return (result, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Insert Single
                    output.AppendLine("        public static async Task<([Entity], string)> Insert[Entity]([ContextClass] db, [Entity] model)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                await db.[Plural].AddAsync(model);");
                    output.AppendLine("                await db.SaveChangesAsync();");
                    output.AppendLine("                return (model, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Insert Range
                    output.AppendLine("        public static async Task<(IEnumerable<[Entity]>, string)> Insert[Plural]Range([ContextClass] db, IEnumerable<[Entity]> model)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                await db.[Plural].AddRangeAsync(model);");
                    output.AppendLine("                await db.SaveChangesAsync();");
                    output.AppendLine("                return (model, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Update Single
                    output.AppendLine("        public static async Task<([Entity],string)> Update[Entity]([ContextClass] db, [Entity] model)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                db.[Plural].Update(model);");
                    output.AppendLine("                await db.SaveChangesAsync();");
                    output.AppendLine("                return (model, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Update Range
                    output.AppendLine("        public static async Task<(IEnumerable<[Entity]>,string)> Update[Plural]Range([ContextClass] db, IEnumerable<[Entity]> model)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                db.[Plural].UpdateRange(model);");
                    output.AppendLine("                await db.SaveChangesAsync();");
                    output.AppendLine("                return (model, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (null,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Delete Single
                    output.AppendLine("        public static async Task<(bool, string)> Delete[Entity]([ContextClass] db, [Entity] model)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                db.[Plural].Remove(model);");
                    output.AppendLine("                await db.SaveChangesAsync();");
                    output.AppendLine("                return (true, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (false,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("");

                    //Delete Range
                    output.AppendLine("        public static async Task<(bool, string)> Delete[Plural]Range([ContextClass] db, IEnumerable<[Entity]> model)");
                    output.AppendLine("        {");
                    output.AppendLine("            try");
                    output.AppendLine("            {");
                    output.AppendLine("                db.[Plural].RemoveRange(model);");
                    output.AppendLine("                await db.SaveChangesAsync();");
                    output.AppendLine("                return (true, string.Empty);");
                    output.AppendLine("            }");
                    output.AppendLine("            catch (Exception ex)");
                    output.AppendLine("            {");
                    output.AppendLine("                return (false,ex.Message);");
                    output.AppendLine("            }");
                    output.AppendLine("        }");
                    output.AppendLine("    }");
                    if (!string.IsNullOrEmpty(NamespaceName))
                    {
                        output.AppendLine("");
                        output.AppendLine("}");
                    }
                    output = output.Replace("[Plural]", Plural)
                        .Replace("[ContextClass]", ContextClass)
                        .Replace("[Entity]", Entity);

                    Console.Write(output.ToString());
                    System.IO.Directory.CreateDirectory("Output");
                    System.IO.File.WriteAllText(@"Output\" + Entity +"Service.cs", output.ToString());
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

    }
}
