using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DSparx.YARL.Compiler;
using System.Reflection;
using System.IO;

namespace DSparx.Test
{
    [TestClass]
    public class ScriptTest
    {
        [TestMethod]
        public void TestOne()
        {
            YARLDomain domain = new YARLDomain();
            string path = GetMainAssemblyPath();
            domain.Compile(String.Format("{0}\\test.yarl", path));
            Console.WriteLine("YARL v1.0 > Compilation completed with no errors");
                
            double water = 10;
            domain.Ground("water", water);
            domain.Execute();
            Console.WriteLine(String.Format("YARL v1.0 > Water={0} and Power={1}", water, domain.Result));

            Assert.AreEqual(50, domain.Result);
        }

        private string GetMainAssemblyPath()
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string pathTwo = Path.GetDirectoryName(path);

            return String.Format("{0}scripts", pathTwo.Substring(0, pathTwo.IndexOf("bin")));
        }
    }
}
