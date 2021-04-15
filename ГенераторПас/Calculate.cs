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
                ExcelWorksheet routes = package.Workbook.Worksheets["Маршруты"];
                int x = 2;
                int codeStop;
                do
                {
                    codeStop = routes.Cells[x, 1].GetValue<int>();
                    string nameStop = routes.Cells[x, 2].GetValue<string>();
                    string district = routes.Cells[x, 3].GetValue<string>();
                    int countPass = routes.Cells[x, 4].GetValue<int>();
                    int attraction = routes.Cells[x, 5].GetValue<int>();
                    if (codeStop != 0) { 
                        listStop.Add(
                        new Stop { CodeStop = codeStop, NameStop = nameStop, District = district, CountPass = countPass, Attraction = attraction }
                        );
                    }
                    x++;

                }
                while (codeStop != 0);

                ExcelWorksheet population = package.Workbook.Worksheets["Население"];

                x = 0; // строки матрицы
                int y = 0;  // столбцы матрицы
                int N = 8; // размерность матрицы
                int M = 15; // размерность матрицы доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия

                double[,] mornWork = new double[N, N]; // матрица доли рабочих и молодёжи утром
                double[,] mornPens = new double[N, N]; // матрица доли пенсионеров и школьников утром
                double[,] dayTime = new double[N, N]; // матрица доли людей в обеденное время
                double[,] evenWork = new double[N, N]; // матрица доли рабочих и молодёжи вечером
                double[,] evenPens = new double[N, N]; // матрица доли пенсионеров и школьников вечером
                double[,] timeDist = new double[N, M]; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия

                for (int i = 160; i < N + 160; i++)
                {
                    for (int j = 3; j < N + 3; j++)
                    {
                        mornWork[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }

                x = 0;

                for (int i = 173; i < N + 173; i++)
                {
                    for (int j = 3; j < N + 3; j++)
                    {
                        mornPens[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }

                x = 0;

                for (int i = 194; i < N + 194; i++)
                {
                    for (int j = 3; j < N + 3; j++)
                    {
                        dayTime[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }

                x = 0;

                for (int i = 207; i < N + 207; i++)
                {
                    for (int j = 3; j < N + 3; j++)
                    {
                        evenWork[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }

                x = 0;

                for (int i = 207; i < N + 207; i++)
                {
                    for (int j = 15; j < N + 15; j++)
                    {
                        evenPens[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }

                x = 0;

                for (int i = 220; i < N + 220; i++)
                {
                    for (int j = 3; j < M + 3; j++)
                    {
                        timeDist[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }
            }
        }
    }
}
