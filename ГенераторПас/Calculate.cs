using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

namespace Транспорт2017.ГенераторПас
{
    public class Calculate
    {
        const int COUNT_DISTRICT = 8; // размерность матрицы
        const int COUNT_HOUR = 15; // размерность матрицы доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия
        const int TYPE_PASS = 4; // Школьники - 0, Студенты - 1, Рабочие - 2, Пенсионеры - 3

        static Random rand = new Random();

        private static List<Stop> listStop;
        private static List<District> listDist;

        static int[,] countPass;
        static double[,] mornWork; // матрица доли рабочих и молодёжи утром
        static double[,] mornPens; // матрица доли пенсионеров и школьников утром
        static double[,] dayTime; // матрица доли людей в обеденное время
        static double[,] evenWork; // матрица доли рабочих и молодёжи вечером
        static double[,] evenPens; // матрица доли пенсионеров и школьников вечером
        static double[,] timeDist; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия
        static double[,] ballWork; // матрица количества рабочих и молодёжи, находящихся в районе по часам
        static double[,] ballPens; // матрица количества пенсионеров и школьников, находящихся в районе по часам
        static int[,,] matrCountPasWork, matrCountPasPens;



        public static void LoadStopFromExcel()
        {
            listStop = new List<Stop>();
            listDist = new List<District>();
            FileInfo file = new FileInfo("данные\\1.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                int code;
                ExcelWorksheet population = package.Workbook.Worksheets["Население"];
                int x = 10;
                //загрузка списка районов
                do
                {
                    code = population.Cells[x, 1].GetValue<int>();
                    string nameDist = population.Cells[x, 2].GetValue<string>();
                    if (code != 0)
                    {
                        listDist.Add(
                        new District { CodeDistrict = code, NameDistrict = nameDist }
                        );
                    }
                    x++;
                }
                while (code != 0);

                ExcelWorksheet routes = package.Workbook.Worksheets["Маршруты"];
                x = 2;
                //загрузка списка остановок
                do
                {
                    code = routes.Cells[x, 1].GetValue<int>();
                    string nameStop = routes.Cells[x, 2].GetValue<string>();
                    string district = routes.Cells[x, 3].GetValue<string>();
                    int countPass = routes.Cells[x, 4].GetValue<int>();
                    int attraction = routes.Cells[x, 5].GetValue<int>();
                    if (code != 0)
                    {
                        Stop stop = new Stop { CodeStop = code, NameStop = nameStop, District = district, CountPass = countPass, Attraction = attraction };
                        listStop.Add(stop);
                        District d = listDist.Find(xx => xx.NameDistrict == district);
                        d.ListStop.Add(stop);
                    }
                    x++;

                }
                while (code != 0);

                //загрузка долей жителей зоны остановок
                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    x = 41;
                    do
                    {
                        code = population.Cells[x, 1 + 4 * i_region].GetValue<int>();
                        double part_area = population.Cells[x, 4 + 4 * i_region].GetValue<double>();
                        if (code != 0)
                        {
                            listStop[code - 1].PercentageSitizen = part_area;
                        }
                        x++;
                    }
                    while (code != 0);
                }

                //countPass = new int[COUNT_DISTRICT, TYPE_PASS]; // матрица количества людей каждого типа, проживающих в каком-либо районе
                mornWork = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли рабочих и молодёжи утром
                mornPens = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли пенсионеров и школьников утром
                dayTime = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли людей в обеденное время
                evenWork = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли рабочих и молодёжи вечером
                evenPens = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли пенсионеров и школьников вечером
                timeDist = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия
                ballWork = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица количества рабочих и молодёжи, находящихся в районе по часам
                ballPens = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица количества пенсионеров и школьников, находящихся в районе по часам


                //загрузка числа пассажиров, находящихся в районах по часам
                for (int i = 0; i < COUNT_DISTRICT; i++)
                {
                    for (int j = 0; j < COUNT_HOUR; j++)
                    {
                        ballWork[i, j] = population.Cells[i + 402 + (13 * j), 3].GetValue<double>();
                        // Cells(401 + i_region + 13 * (n_hour - 1), 3).Value
                        ballPens[i, j] = population.Cells[i + 402 + (13 * j), 4].GetValue<double>();
                    }
                }

                //загрузка прочих матриц
                for (int i = 0; i < COUNT_DISTRICT; i++)
                {
                    for (int j = 0; j < COUNT_DISTRICT; j++)
                    {
                        mornWork[i, j] = population.Cells[i + 160, j + 3].GetValue<double>();
                        mornPens[i, j] = population.Cells[i + 173, j + 3].GetValue<double>();
                        dayTime[i, j] = population.Cells[i + 194, j + 3].GetValue<double>();
                        evenWork[i, j] = population.Cells[i + 207, j + 3].GetValue<double>();
                        evenPens[i, j] = population.Cells[i + 207, j + 15].GetValue<double>();
                    }
                }

                //загрузка матрицы распределния по времени
                for (int i = 0; i < COUNT_DISTRICT; i++)
                {
                    for (int j = 0; j < COUNT_HOUR; j++)
                    {
                        timeDist[i, j] = population.Cells[i + 220, j + 3].GetValue<double>();
                    }
                }
            }
        }

        public static void Balance(string sheets_name, int i_region, double[,] morning_workers, double[,] morning_pensioners, double[,] day_time,
            double[,] evening_workers, double[,] evening_pensioners, double[,] time_distribution, int n_hour)
        {
            // коэффициенты для расчёта перерпспределения между потоками
            double[,] k_flow_workers_morning = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_pensioners_morning = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_workers_daytime = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_pensioners_daytime = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_workers_evening = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_pensioners_evening = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[] hour_region_workers = new double[COUNT_DISTRICT];
            double[] hour_region_pensioners = new double[COUNT_DISTRICT];
            n_hour = 1;
            for (int i = 0; i < i_region; i++)
            {

            }
        }

        public static void GeneratePass()
        {
            matrCountPasWork = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT];
            matrCountPasPens = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT];

            for (int n_hour = 0; n_hour < COUNT_HOUR; n_hour++)
            {
                ////Заполнение исходных данных для расчета пуассоновских пассажиров (строки 234-239 для всех районов)
                //for (int i_region = 0; i_region < 8; i_region++)
                //{
                //    // For i_region = 1 To 8
                //    //n_region_stops(i_region) = Cells(238, 3 + 23 * (i_region - 1)).Value
                //    double number_of_workers = ballWork[i_region, n_hour];
                //    double number_of_pensioners = ballPens[i_region, n_hour];


                //    double number_of_workers_hour = number_of_workers * timeDist[i_region, n_hour];
                //    //Cells(234, 2 + 23 * (i_region - 1)).Value = number_of_workers_hour
                //    double number_of_pensioners_hour = number_of_pensioners * timeDist[i_region, n_hour];
                //    //Cells(236, 2 + 23 * (i_region - 1)).Value = number_of_pensioners_hour
                //}

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
                double[,] matrFlowPasWork = new double[listStop.Count, COUNT_DISTRICT];
                double[,] matrFlowPasPens = new double[listStop.Count, COUNT_DISTRICT];
                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    double number_of_workers = ballWork[i_region, n_hour];// Cells(401 + i_region + 13 * (n_hour - 1), 3).Value
                    double number_of_pensioners = ballPens[i_region, n_hour];// Cells(401 + i_region + 13 * (n_hour - 1), 4).Value


                    double number_of_workers_hour = number_of_workers * timeDist[i_region, n_hour];
                    double number_of_pensioners_hour = number_of_pensioners * timeDist[i_region, n_hour];

                    for (int j_stops = 0; j_stops < listDist[i_region].CountStops; j_stops++)
                    {
                        Stop stop = listDist[i_region].GetStop(j_stops);
                        matrFlowPasWork[stop.CodeStop - 1, /*n_hour, */i_region] = number_of_workers_hour * stop.PercentageSitizen;
                        //Cells(240 + j_stops, 4 + 23 * (i_region - 1)).Value = _
                        //    Cells(234, 2 + 23 * (i_region - 1)).Value * Cells(40 + j_stops, 4 + 4 * (i_region - 1)).Value

                        matrFlowPasPens[stop.CodeStop - 1, /*n_hour, */i_region] = number_of_pensioners_hour * stop.PercentageSitizen;
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
                            Stop stop = listDist[i_region].ListStop[k_stops];
                            //For k = 1 To n_region_stops(region)
                            //row_lambda = 240 + k
                            //row_result = row_lambda
                            //column_lambda1 = 4 + 23 * (region - 1)
                            //column_lambda2 = 5 + 23 * (region - 1)
                            //column_result1 = i + 5 + 23 * (region - 1)
                            //column_result2 = i + 13 + 23 * (region - 1)

                            double lambda1 = matrFlowPasWork[stop.CodeStop - 1, /*n_hour,*/ i_region] * time_percent1;
                            double lambda2 = matrFlowPasPens[stop.CodeStop - 1, /*n_hour,*/ i_region] * time_percent2;
                            //lambda1 = Cells(row_lambda, column_lambda1) * time_percent1
                            //lambda2 = Cells(row_lambda, column_lambda2) * time_percent2
                            int x_lambda1 = Poisson_value(lambda1);
                            int x_lambda2 = Poisson_value(lambda2);
                            matrCountPasWork[stop.CodeStop - 1, n_hour, i_region] = x_lambda1;
                            matrCountPasPens[stop.CodeStop - 1, n_hour, i_region] = x_lambda2;
                            //Cells(row_result, column_result1).Value = x_lambda1
                            //Cells(row_result, column_result2).Value = x_lambda2

                        }
                    }
                }
            }
        }

        public static void SaveToSheets()
        {
            FileStream file = File.Create("данные\\2.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet excelSh = package.Workbook.Worksheets.Add("Test");
                for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                {
                    excelSh.Cells[1, 1 + 20 * i_hour].Value = (i_hour + 6) + ":00";
                    for (int j_dist = 0; j_dist < COUNT_DISTRICT; j_dist++)
                    {
                        excelSh.Cells[2, 2 + j_dist + 20 * i_hour].Value = listDist[j_dist].NameDistrict;
                        excelSh.Cells[2, 2 + COUNT_DISTRICT + j_dist + 20 * i_hour].Value = listDist[j_dist].NameDistrict;
                        for (int k_stop = 0; k_stop < listStop.Count; k_stop++)
                        {
                            excelSh.Cells[3 + k_stop, 2 + j_dist + 20 * i_hour].Value = matrCountPasWork[k_stop, i_hour, j_dist];
                            excelSh.Cells[3 + k_stop, 2 + COUNT_DISTRICT + j_dist + 20 * i_hour].Value = matrCountPasPens[k_stop, i_hour, j_dist];
                        }
                    }
                }
                package.Save();
            }
            file.Close();
        }

        public static int Poisson_value(double lambda)
        {
            if (lambda <= 30)
            {
                int x = 0;
                double a = Math.Exp(-lambda);

                double FRand = rand.NextDouble();
                double s = FRand;
                while (s >= a)
                {
                    x = x + 1;
                    s = s * FRand;
                }
                return x;
            }
            else
            {
                double multi = rand.NextDouble();
                double a1 = Math.Pow(-2 * Math.Log(multi), 0.5);
                double a2 = Math.Cos(2 * Math.PI * multi);
                double a3 = Math.Pow(lambda, 0.5) * a1 * a2 + lambda;
                return (int)a3;
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
