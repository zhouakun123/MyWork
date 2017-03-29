// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="">
//   
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RelevancyAnalysis
{
    #region Using

    using System;
    using System.IO;
    using System.Text;

    using Microsoft.Office.Interop.Excel;

    #endregion

    /// <summary>The program.</summary>
    internal class Program
    {
        /// <summary>The main.</summary>
        /// <param name="args">The args.</param>
        private static void Main(string[] args)
        {
            string filePath;
            if (args.Length != 0 && args[0] != string.Empty)
            {
                filePath = args[0];
            }
            else
            {
                Console.WriteLine("请输入要处理的Excel文件的绝对路径：");
                filePath = Console.ReadLine();
            }

            while (!File.Exists(filePath))
            {
                Console.WriteLine("Excel文件的绝对路径错误，请重新输入：");
                filePath = Console.ReadLine();
            }

            var excel = new Application();
            var wbs = excel.Workbooks;
            var wb = wbs.Add(filePath);
            Worksheet baseSheet = wb.Worksheets[0];
            Worksheet newSheet = wb.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            int colnum = 0;
            while (!string.IsNullOrEmpty(baseSheet.Cells[0, colnum].ToString().Trim()))
            {
                newSheet.Cells[0, colnum] = baseSheet.Cells[0, colnum];
                newSheet.Cells[colnum, 0] = baseSheet.Cells[0, colnum];
                ++colnum;
            }

            var sb = new StringBuilder();
            var row = 0;
            Console.Write("处理药品数：...............................");
            while (!string.IsNullOrEmpty(baseSheet.Cells[row, 0].ToString()))
            {
                var col = 0;
                while (string.IsNullOrEmpty(baseSheet.Cells[row, col].ToString()))
                {
                    int result;
                    if (int.TryParse(baseSheet.Cells[row, col].ToString().Trim(), out result) && result.ToString().Length == 1)
                    {
                        sb.Append(result.ToString());
                    }

                    ++col;
                }

                baseSheet.Cells[row, col] = sb.ToString();
                sb.Clear();
                for (var i = 0; i < row.ToString().Length; ++i)
                {
                    Console.Write('\u0008');
                }

                ++row;
                Console.Write(row.ToString());
            }

            wb.Save();

            // sb.Clear();
            // sb.Append();
            // TryParse()
            // ws.Cells[];
            Console.WriteLine("完成任务！");
        }
    }
}