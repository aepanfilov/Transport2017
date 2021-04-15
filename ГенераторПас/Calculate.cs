using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Транспорт2017.ГенераторПас
{
    public class Calculate
    {
        private static List<Stop> listStop;

        public static void LoadStopFromExcel()
        {
            listStop = new List<Stop>();
            FileInfo file = new FileInfo("данные\\1.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet wkSheet = package.Workbook.Worksheets["Маршруты"];
                //int КолОстановок = wkSheet.Cells[1, 1].GetValue<int>();
                int x = 2;
                int codeStop;
                do
                {
                    codeStop = wkSheet.Cells[x, 1].GetValue<int>();
                    string nameStop = wkSheet.Cells[x, 2].GetValue<string>();
                    string district = wkSheet.Cells[x, 3].GetValue<string>();
                    int countPass = wkSheet.Cells[x, 4].GetValue<int>();
                    int attraction = wkSheet.Cells[x, 5].GetValue<int>();
                    if (codeStop != 0) { 
                        listStop.Add(
                        new Stop { CodeStop = codeStop, NameStop = nameStop, District = district, CountPass = countPass, Attraction = attraction }
                        );
                    }
                    x++;

                }
                while (codeStop != 0);


            }
        }
    }
}
