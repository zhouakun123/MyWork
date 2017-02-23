// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace CombineResults
{
    #region Using

    using System;
    using System.IO;
    using System.Text;

    using Microsoft.Office.Interop.Excel;

    #endregion

    /// <summary>The program.</summary>
    public class Program
    {
        #region Static methods

        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        public static void Main(string[] args)
        {
            string filePath;
            if (args.Length != 0 && args[0] != string.Empty)
            {
                filePath = args[0];
            }
            else
            {
                Console.WriteLine("请输入要处理的Excel文件的绝对路径：");
                filePath = Console.ReadLine(); //@"C:\Users\cnzhohao\Downloads\ID-GN.xls";
            }

            while (!File.Exists(filePath))
            {
                Console.WriteLine("Excel文件的绝对路径错误，请重新输入：");
                filePath = Console.ReadLine();
            }

            var excel = new Application();
            var wbs = excel.Workbooks;
            var wb = wbs.Add(filePath);
            Worksheet ws = wb.Worksheets["Sheet1"];
            var sb = new StringBuilder();
            var row = 1;
            Console.Write("处理行数：...............................");
            var abc = GetCellValue(ws.Cells[1, 1]);
            while (!string.IsNullOrEmpty(GetCellValue(ws.Cells[row, 1])))
            {
                var col = 1;
                sb.Append("序列");
                while (!string.IsNullOrEmpty(GetCellValue(ws.Cells[row, col])))
                {
                    int result;
                    if (int.TryParse(GetCellValue(ws.Cells[row, col]).Trim(), out result)
                        && result.ToString().Length == 1)
                    {
                        sb.Append(result.ToString());
                    }

                    ++col;
                }

                ((Range)ws.Cells[row, col]).Value2 = sb.ToString();
                sb.Clear();
                for (var i = 0; i < row.ToString().Length; ++i)
                {
                    Console.Write('\u0008');
                }

                ++row;
                Console.Write(row.ToString());
            }

            try
            {
                wb.SaveAs(@"C:\test.xlsx");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            wbs.Close();
            // sb.Clear();
            // sb.Append();
            // TryParse()
            // ws.Cells[];
            Console.WriteLine("完成任务！");
            Console.WriteLine(@"已生成 C:\test.xlsx 文件。");
            Console.ReadKey();
        }

        internal static string GetCellValue(object target)
        {
            var range = target as Range; // return (target as Range).Value2.ToString();
            if (range != null && range.Value2 != null)
            {
                return range.Value2.ToString();
            }

            return null;
        }

        #endregion
    }
}