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
        const int COUNT_DISTRICT = 8; // размерность матрицы
        const int COUNT_HOUR = 15; // размерность матрицы доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия


        private static List<Stop> listStop;
        private static List<Dist> listDist;

        static double[,] mornWork; // матрица доли рабочих и молодёжи утром
        static double[,] mornPens; // матрица доли пенсионеров и школьников утром
        static double[,] dayTime; // матрица доли людей в обеденное время
        static double[,] evenWork; // матрица доли рабочих и молодёжи вечером
        static double[,] evenPens ; // матрица доли пенсионеров и школьников вечером
        static double[,] timeDist ; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия


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
                
                 mornWork = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли рабочих и молодёжи утром
                mornPens = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли пенсионеров и школьников утром
                dayTime = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли людей в обеденное время
                evenWork = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли рабочих и молодёжи вечером
                evenPens = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли пенсионеров и школьников вечером
                timeDist = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия

                for (int i = 0; i <COUNT_DISTRICT; i++)
                {
                    for (int j = 0; j < COUNT_DISTRICT; j++)
                    {
                        mornWork[x, y] = population.Cells[i + 160, j + 3].GetValue<double>();
                        mornPens[x, y] = population.Cells[i + 173, j + 3].GetValue<double>();
                        dayTime[x, y] = population.Cells[i + 194, j + 3].GetValue<double>();
                        evenWork[x, y] = population.Cells[i + 207, j + 3].GetValue<double>();
                        evenPens[x, y] = population.Cells[i + 207, j + 15].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }

                x = 0;

                for (int i = 220; i < COUNT_DISTRICT + 220; i++)
                {
                    for (int j = 3; j < COUNT_HOUR + 3; j++)
                    {
                        timeDist[x, y] = population.Cells[i, j].GetValue<double>();
                        y++;
                    }
                    y = 0;
                    x++;
                }
            }
        }

        public static void GeneratePass()
        {
            for (int n_hour = 1; n_hour <= COUNT_HOUR; n_hour++)
            {
                //Заполнение исходных данных для расчета пуассоновских пассажиров (строки 234-239 для всех районов)
                for (int i_region = 1; i_region <= 8; i_region++)
                {
                    // For i_region = 1 To 8
                    //n_region_stops(i_region) = Cells(238, 3 + 23 * (i_region - 1)).Value
                    double number_of_workers = 12;// Cells(401 + i_region + 13 * (n_hour - 1), 3).Value
                    double number_of_pensioners = 10;// Cells(401 + i_region + 13 * (n_hour - 1), 4).Value
                                                     //Cells(234, 1 + 23 * (i_region - 1)).Value = number_of_workers
                                                     //Cells(236, 1 + 23 * (i_region - 1)).Value = number_of_pensioners


                    double number_of_workers_hour = number_of_workers * timeDist[i_region, n_hour];
                    //Cells(234, 2 + 23 * (i_region - 1)).Value = number_of_workers_hour
                    double number_of_pensioners_hour = number_of_pensioners * timeDist[i_region, n_hour];
                    //Cells(236, 2 + 23 * (i_region - 1)).Value = number_of_pensioners_hour
                }

                /*For i_region = 1 To 8 'заполнение строки 239 (средние доли потока)для всех районов
                            ggg = Cells(234, 2 + 23 * (i_region - 1)).Value / n_region_stops(i_region)
                            hhh = Cells(236, 2 + 23 * (i_region - 1)).Value / n_region_stops(i_region)
                            For j_region = 1 To 8
                            If n_hour <= 5 Then
                                Cells(239, 5 + j_region + 23 * (i_region - 1)).Value = ggg * morning_workers(i_region, j_region)
                                Cells(239, 13 + j_region + 23 * (i_region - 1)).Value = hhh * morning_pensioners(i_region, j_region)


                                ElseIf n_hour <= 10 Then
                                    Cells(239, 5 + j_region + 23 * (i_region - 1)).Value = number_of_workers_hour / n_region_stops(i_region) * day_time(i_region, j_region)
                                    Cells(239, 13 + j_region + 23 * (i_region - 1)).Value = number_of_pensioners_hour / n_region_stops(i_region) * day_time(i_region, j_region)


                                    ElseIf n_hour <= 15 Then
                                        Cells(239, 5 + j_region + 23 * (i_region - 1)).Value = number_of_workers_hour / n_region_stops(i_region) * evening_workers(i_region, j_region)
                                        Cells(239, 13 + j_region + 23 * (i_region - 1)).Value = number_of_pensioners_hour / n_region_stops(i_region) * evening_pensioners(i_region, j_region)
                             End If
                        Next j_region
                Next i_region*/



                //Заполнение численности на остановках на текущий час
                double[,,,] matrFlowPas = new double[listStop.Count, COUNT_HOUR, COUNT_DISTRICT, 2];
                int[,,,] matrCountPas = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT, 2];
                for (int i_region = 1; i_region <= COUNT_DISTRICT; i_region++)
                {
                    double number_of_workers = 12;// Cells(401 + i_region + 13 * (n_hour - 1), 3).Value
                    double number_of_pensioners = 10;// Cells(401 + i_region + 13 * (n_hour - 1), 4).Value


                    double number_of_workers_hour = number_of_workers * timeDist[i_region, n_hour];
                    double number_of_pensioners_hour = number_of_pensioners * timeDist[i_region, n_hour];

                    for (int j_stops = 0; j_stops < listDist[i_region].CountStops; j_stops++)
                    {
                        Stop stop = listDist[i_region].GetStop(j_stops);
                        matrFlowPas[j_stops, n_hour, i_region, 0] = number_of_workers_hour * stop.PercentageSitizen;
                        //Cells(240 + j_stops, 4 + 23 * (i_region - 1)).Value = _
                        //    Cells(234, 2 + 23 * (i_region - 1)).Value * Cells(40 + j_stops, 4 + 4 * (i_region - 1)).Value

                        matrFlowPas[j_stops, n_hour, i_region, 1] = number_of_pensioners_hour * stop.PercentageSitizen;
                        //Cells(240 + j_stops, 5 + 23 * (i_region - 1)).Value = _
                        //    Cells(236, 2 + 23 * (i_region - 1)).Value * Cells(40 + j_stops, 4 + 4 * (i_region - 1)).Value

                        //Cells(240 + j_stops, 3 + 23 * (i_region - 1)).Value = _
                        //    Cells(240 + j_stops, 4 + 23 * (i_region - 1)).Value + Cells(240 + j_stops, 5 + 23 * (i_region - 1)).Value
                    }
                }

                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    //For region = 1 To 8 ???
                    //    column_data1 = 4 + 23 * (region - 1) ???
                    //    column_data2 = 5 + 23 * (region - 1) ???
                    //    j_max = Cells(238, 3 + 23 * (region - 1)).Value ???
                    //    Range(Cells(241, 6 + 23 * (region - 1)), Cells(240 + j_max, 21 + 23 * (region - 1))).Clear ???

                    //For i = 1 To 8 'Цикл по числу остановок для данного района
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {

                        double time_percent1 = mornWork[i_region, j_region];
                        double time_percent2 = mornPens[i_region, j_region];
                        //time_percent1 = Cells(159 + region, 2 + i).Value ' учет различий в перетоках между районами для работающих
                        //  time_percent2 = Cells(172 + region, 2 + i).Value ' то же, для пенсионеров
                        //'time_distribution = Cells(219 + region, 2 + i).Value ' учет неравномерности пассажиропотока по времени
                        for (int k_stops = 0; k_stops < listDist[i_region].CountStops; k_stops++)
                        {
                            //For k = 1 To n_region_stops(region)
                            //row_lambda = 240 + k
                            //row_result = row_lambda
                            //column_lambda1 = 4 + 23 * (region - 1)
                            //column_lambda2 = 5 + 23 * (region - 1)
                            //column_result1 = i + 5 + 23 * (region - 1)
                            //column_result2 = i + 13 + 23 * (region - 1)

                            double lambda1 = matrFlowPas[k_stops, n_hour, i_region, 0] * time_percent1;
                            double lambda2 = matrFlowPas[k_stops, n_hour, i_region, 1] * time_percent2;
                            //lambda1 = Cells(row_lambda, column_lambda1) * time_percent1
                            //lambda2 = Cells(row_lambda, column_lambda2) * time_percent2
                            int x_lambda1 = Poisson_value(lambda1);
                            int x_lambda2 = Poisson_value(lambda2);
                            matrCountPas[k_stops, n_hour, i_region, 0] = x_lambda1;
                            matrCountPas[k_stops, n_hour, i_region, 1] = x_lambda2;
                            //Cells(row_result, column_result1).Value = x_lambda1
                            //Cells(row_result, column_result2).Value = x_lambda2

                        }
                    }
                }
            }
        }
//        Function Poisson_value(lambda) 'при больших lambda закон распределения Пуассона
                                        //'приближенно заменяется нормальным.
//    If lambda <= 30 Then
//        x = 0
//        a = Exp(-lambda)
//        r = Rnd() * 10001
//        FRand = r / 10000
//        s = FRand
//        Do While s >= a
//            x = x + 1
//            r = Rnd() * 10001
//            FRand = r / 10000
//            s = s* FRand
//        Loop
//        Poisson_value = x
//        Else
//            multi = Rnd()
//            zzzz = Log(Exp(1))
//            a1 = (-2 * Log(multi)) ^ (0.5)
//            a2 = Cos(2 * 3.141592 * multi)
//            a3 = (lambda ^ 0.5) * a1* a2 + lambda
//           Poisson_value = Int(a3)


//  End If


//End Function
    }
}
