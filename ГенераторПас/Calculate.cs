using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        static double probability_of_arbitrary_choise;
        static double probability_without_jump;
        static double[,] attractive;



        public static void LoadStopFromExcel()
        {
            listStop = new List<Stop>();
            listDist = new List<District>();
            FileInfo file = new FileInfo("данные\\1.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                int code;
                int route;
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
                //загрузка вероятности выбора остановки без пересадки
                probability_of_arbitrary_choise = population.Cells["L6"].GetValue<double>();
                //загрузка вероятности произвольного выбора остановки 
                probability_without_jump = population.Cells["H6"].GetValue<double>();
                //загрузка вероятности выбора остановок прибытия
                attractive = new double[5, 3];
                for (int i = 0; i < attractive.GetLength(0); i++)
                {
                    for (int j = 0; j < attractive.GetLength(1); j++)
                    {
                        attractive[i, j] = population.Cells[3 + i, 14 + j].GetValue<double>();
                    }
                }
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
                    List<int> listRoutes = new List<int>();
                    int countRoutes = routes.Cells[x, 6].GetValue<int>();
                    for (int i = 0; i < countRoutes; i++)
                    {
                        route = routes.Cells[x, i + 7].GetValue<int>();
                        listRoutes.Add(route);
                    }
                    if (code != 0)
                    {
                        Stop stop = new Stop { CodeStop = code, NameStop = nameStop, District = district, CountPass = countPass, Attraction = attraction, ListOfRoutes = listRoutes };
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

        //static double[,] matrFlowPasWork;//2
        //static double[,] matrFlowPasPens;
        public static void GeneratePass()
        {
            SetCorrespondenceStops();
            matrCountPasWork = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT];
            matrCountPasPens = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT];

            //matrFlowPasWork = new double[listStop.Count, COUNT_HOUR];//2
            //matrFlowPasPens = new double[listStop.Count, COUNT_HOUR];

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
                //double[,] matrFlowPasWork = new double[listStop.Count, COUNT_DISTRICT];
                //double[,] matrFlowPasPens = new double[listStop.Count, COUNT_DISTRICT];

                double[] matrFlowPasWork = new double[listStop.Count];//1
                double[] matrFlowPasPens = new double[listStop.Count];

                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    double number_of_workers = ballWork[i_region, n_hour];// Cells(401 + i_region + 13 * (n_hour - 1), 3).Value
                    double number_of_pensioners = ballPens[i_region, n_hour];// Cells(401 + i_region + 13 * (n_hour - 1), 4).Value

                    double number_of_workers_hour = number_of_workers * timeDist[i_region, n_hour];
                    double number_of_pensioners_hour = number_of_pensioners * timeDist[i_region, n_hour];

                    for (int j_stops = 0; j_stops < listDist[i_region].CountStops; j_stops++)
                    {
                        Stop stop = listDist[i_region].GetStop(j_stops);
                        //matrFlowPasWork[stop.CodeStop - 1, /*n_hour, */i_region] = number_of_workers_hour * stop.PercentageSitizen;
                        matrFlowPasWork[stop.CodeStop - 1] = number_of_workers_hour * stop.PercentageSitizen;
                        //matrFlowPasWork[stop.CodeStop - 1,n_hour] = number_of_workers_hour * stop.PercentageSitizen;

                        //matrFlowPasPens[stop.CodeStop - 1, /*n_hour, */i_region] = number_of_pensioners_hour * stop.PercentageSitizen;
                        matrFlowPasPens[stop.CodeStop - 1] = number_of_pensioners_hour * stop.PercentageSitizen;
                        //matrFlowPasPens[stop.CodeStop - 1,n_hour] = number_of_pensioners_hour * stop.PercentageSitizen;
                    }
                }

                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {                   
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {
                        double time_percent1 = mornWork[i_region, j_region];//учет различий в перетоках между районами для работающих
                        double time_percent2 = mornPens[i_region, j_region];//то же, для пенсионеров
                         //Цикл по числу остановок района прибытия
                        for (int k_stops = 0; k_stops < listDist[i_region].CountStops; k_stops++)
                        {
                            Stop stop = listDist[i_region].ListStop[k_stops];
                            //double lambda1 = matrFlowPasWork[stop.CodeStop - 1, /*n_hour,*/ j_region] * time_percent1;
                            //double lambda2 = matrFlowPasPens[stop.CodeStop - 1, /*n_hour,*/ j_region] * time_percent2;
                            double lambda1 = matrFlowPasWork[stop.CodeStop - 1] * time_percent1;//1
                            double lambda2 = matrFlowPasPens[stop.CodeStop - 1] * time_percent2;
                            //double lambda1 = matrFlowPasWork[stop.CodeStop - 1,n_hour] * time_percent1;//2
                            //double lambda2 = matrFlowPasPens[stop.CodeStop - 1,n_hour] * time_percent2;
                            int x_lambda1 = Poisson_value(lambda1);
                            int x_lambda2 = Poisson_value(lambda2);
                            matrCountPasWork[stop.CodeStop - 1, n_hour, j_region] = x_lambda1;
                            matrCountPasPens[stop.CodeStop - 1, n_hour, j_region] = x_lambda2;
                        }
                    }
                }
            }
        }
        public static int Poisson_value(double lambda)
        {
            if (lambda <= 30)
            {
                int x = 0;
                double a = Math.Exp(-lambda);

                double s = rand.NextDouble();
                while (s >= a)
                {
                    x = x + 1;
                    s = s * rand.NextDouble();
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
        public static void SetCorrespondenceStops()
        {
            // поиск остановок без пересадок
            for (int i_stop_start = 0; i_stop_start < listStop.Count; i_stop_start++)
            {
                Stop stopStart = listStop[i_stop_start];
                stopStart.StopWoTransfer = new List<Stop>();
                int n_routs_of_stop = stopStart.ListOfRoutes.Count;
                for (int i_route = 0; i_route < n_routs_of_stop; i_route++)
                {
                    int route_current = stopStart.ListOfRoutes[i_route];
                    for (int i_stop_finish = 0; i_stop_finish < listStop.Count; i_stop_finish++)
                    {
                        Stop stopFinish = listStop[i_stop_finish];
                        if (i_stop_start != i_stop_finish)
                        {
                            if (stopFinish.ListOfRoutes.Contains(route_current))
                            {
                                stopStart.StopWoTransfer.Add(stopFinish);
                            }
                        }
                    }
                }
                // удаление дубликатов
                stopStart.StopWoTransfer = stopStart.StopWoTransfer.Distinct().ToList();
            }

            // поиск остановок с одной пересадкой
            foreach (Stop stopStart in listStop)
            {
                stopStart.StopWithTransfer = new List<Stop>();
                foreach (Stop stopFinish in listStop)
                {
                    if (stopStart != stopFinish && !stopStart.StopWoTransfer.Contains(stopFinish))
                    {
                        foreach (Stop stopMiddle in stopStart.StopWoTransfer)
                        {
                            if (stopFinish.StopWoTransfer.Contains(stopMiddle))
                            {
                                stopStart.StopWithTransfer.Add(stopFinish);
                                break;
                            }
                        }
                    }
                }
                // удаление дубликатов
                stopStart.StopWithTransfer = stopStart.StopWithTransfer.Distinct().ToList();
            }
        }
        public static List<Passenger> DistributePass()
        {
            List<Passenger> listPass = new List<Passenger>();

            for (int i_stop = 0; i_stop < listStop.Count; i_stop++)
            {
                for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                {
                    int n_passengers_hour = 0;
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {
                        int n_passengers_region = matrCountPasWork[i_stop, i_hour, j_region] + matrCountPasPens[i_stop, i_hour, j_region];
                        n_passengers_hour = n_passengers_hour + n_passengers_region;
                        if (n_passengers_region > 0)
                        {
                            for (int n_passengers = 0; n_passengers < n_passengers_region; n_passengers++)
                            {
                                int minutes = rand.Next(0, 60);
                                TimeSpan TimeS = new TimeSpan(6 + i_hour, minutes, 0);
                                int codeFinish = ChoiseOfStop(listStop[i_stop], j_region);
                                listPass.Add(new Passenger { CodeStopStart = i_stop, Time = TimeS, CodeDistrictFinish = j_region,  CodeStopFinish = codeFinish }); 
                            }
                        }
                    }
                }
            }
            int z = listPass.Where(x => x.CodeStopFinish == -1).Count();
            return listPass;
        }

        private static int ChoiseOfStop(Stop stopStart, int j_region)
        {
            double shance = rand.NextDouble();
            // вероятность выбора любой остановки
            if (shance < probability_of_arbitrary_choise)
            {
                return (int)(shance * listStop.Count) + 1;
            }
            // вероятность выбора остановки в заданном районе
            else
            {
                double zzz = rand.NextDouble();
                // вероятность выбора остановки без пересадки
                if (zzz <= probability_without_jump)
                {
                    List<Stop> list_stop_region = stopStart.StopWoTransfer.Where(x => x.District == listDist[j_region].NameDistrict).ToList();
                    //если нет остановки в выбранном районе - выход с -1
                    if (list_stop_region.Count == 0)
                        return -2;

                    // формирование классов для каждой остановки
                    List<int> list_group = new List<int>();
                    foreach (Stop stop in list_stop_region)
                    {
                        for (int i_a = 0; i_a < attractive.GetLength(0); i_a++)
                        {
                            if (stop.Attraction <= attractive[i_a, 0])
                            {
                                list_group.Add(i_a);
                                break;
                            }
                        }
                    }
                    //int count_group= list_group.Distinct().Count();
                    //int i_group = rand.Next(count_group); // номер класса, куда поедет пассажир
                    var list_group_dist = list_group.Distinct();
                    int count_group = list_group_dist.Count();
                    int uuu=rand.Next(count_group);
                    int i_group = list_group_dist.ElementAt(uuu);// номер класса, куда поедет пассажир

                    int count_stop_group = list_group.Where(x => x == i_group).Count();
                    int number_of_stop = (int)(count_stop_group * rand.NextDouble());
                    int n_of_stop = 0;
                    for (int i = 0; i < list_group.Count; i++)
                    {
                        if (list_group[i] == i_group)
                        {
                            if (n_of_stop == number_of_stop)
                            {
                                return i;
                            }
                            n_of_stop++;
                        }
                    }                              
                }
                // вероятность выбора остановки с одной пересадкой
                else
                {
                    List<Stop> list_stop_region = stopStart.StopWithTransfer.Where(x => x.District == listDist[j_region].NameDistrict).ToList();
                    //если нет остановки в выбранном районе - выход с -1
                    if (list_stop_region.Count == 0)
                        return -2;

                    // формирование классов для каждой остановки
                    List<int> list_group = new List<int>();
                    foreach (Stop stop in list_stop_region)
                    {
                        for (int i_a = 0; i_a < attractive.GetLength(0); i_a++)
                        {
                            if (stop.Attraction <= attractive[i_a, 0])
                            {
                                list_group.Add(i_a);
                                break;
                            }
                        }
                    }
                    ////int count_group = list_group.Distinct().Count();
                    //int i_group = rand.Next(count_group); // номер класса, куда поедет пассажир
                    var list_group_dist = list_group.Distinct();
                    int count_group = list_group_dist.Count();
                    int uuu = rand.Next(count_group);
                    int i_group = list_group_dist.ElementAt(uuu);// номер класса, куда поедет пассажир

                    int count_stop_group = list_group.Where(x => x == i_group).Count();
                    int number_of_stop = (int)(count_stop_group * rand.NextDouble());
                    int n_of_stop = 0;
                    for (int i = 0; i < list_group.Count; i++)
                    {
                        if (list_group[i] == i_group)
                        {
                            if (n_of_stop == number_of_stop)
                            {
                                return i;
                            }
                            n_of_stop++;
                        }
                    }
                }
            }
            return -2; // если ошибка
        }
        public static void SaveToSheets_test(List<Passenger> listPass)
        {
            FileStream file = File.Create("данные\\2.xlsx");
            //List<Passenger> listPass = DistributePass();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet excelSh = package.Workbook.Worksheets.Add("Test_1");
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
                /*[[[ итого за день, по остановкам и районам прибытия */
                for (int k_stop = 0; k_stop < listStop.Count; k_stop++)
                {
                    excelSh.Cells[1, 1 + 20 * COUNT_HOUR].Value = "за день";
                    excelSh.Cells[3+k_stop , 1 ].Value = k_stop+1;
                    for (int j_dist = 0; j_dist < COUNT_DISTRICT; j_dist++)
                    {
                        excelSh.Cells[2, 2 + j_dist + 20 * COUNT_HOUR].Value = listDist[j_dist].NameDistrict;
                        excelSh.Cells[2, 2 + COUNT_DISTRICT + j_dist + 20 * COUNT_HOUR].Value = listDist[j_dist].NameDistrict;
                        int sumw = 0, sump = 0;
                        for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                        {
                            sumw+= matrCountPasWork[k_stop, i_hour, j_dist];
                            sump+= matrCountPasPens[k_stop, i_hour, j_dist];
                        }
                        excelSh.Cells[3 + k_stop, 2 + j_dist + 20 * COUNT_HOUR].Value = sumw;
                        excelSh.Cells[3 + k_stop, 2 + COUNT_DISTRICT + j_dist + 20 * COUNT_HOUR].Value = sump;
                    }

                    //for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                    //{
                    //    excelSh.Cells[3 + k_stop, 2 + i_hour + 20 * (COUNT_HOUR+1)].Value = matrFlowPasWork[k_stop,i_hour];
                    //    excelSh.Cells[3 + k_stop, 2 + COUNT_HOUR + i_hour + 20 * (COUNT_HOUR+1)].Value = matrFlowPasPens[k_stop, i_hour];
                    //}
                }
                //for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                //{
                //    excelSh.Cells[2, 2 + i_hour + 20 * (COUNT_HOUR + 1)].Value = i_hour+6;
                //    excelSh.Cells[2, 2 + COUNT_HOUR + i_hour + 20 * (COUNT_HOUR + 1)].Value = 6+ i_hour;
                //}
                /*]]]*/

                excelSh = package.Workbook.Worksheets.Add("trafic");
                //int sum = 0;
                //int global = 0;
                TimeSpan time_6 = new TimeSpan(6,0,0);
                for (int i_stop = 0; i_stop < listStop.Count(); i_stop++)
                {
                    excelSh.Cells[3, 1 + 5 * i_stop].Value = listStop[i_stop].CodeStop;
                    excelSh.Cells[4, 1 + 5 * i_stop].Value = "Время";
                    excelSh.Cells[4, 2 + 5 * i_stop].Value = "Р-н прибытия";
                    excelSh.Cells[4, 3 + 5 * i_stop].Value = "Код ост прибытия";
                    //global = global + listPass.Where(x => x.CodeStopStart == i_stop).Count();
                    //for (int j = sum; j < global; j++)
                    //{
                    //    //string ghg = Convert.ToString(listPass[j].Time.ToString("H:mm"));
                    //    excelSh.Cells[5 + j - sum, 1 + 5 * i_stop].Value = (listPass[j].Time - time_6).TotalMinutes;
                    //    excelSh.Cells[5 + j - sum, 2 + 5 * i_stop].Value = listPass[j].CodeDistrictFinish + 1;
                    //    excelSh.Cells[5 + j - sum, 3 + 5 * i_stop].Value = listPass[j].CodeStopFinish + 1;
                    //}
                    //sum = global;
                    List<Passenger> list = listPass.Where(x => x.CodeStopStart == i_stop).ToList();
                    list.Sort();
                    int i_pas = 0;
                    foreach (Passenger pass in list)
                    {
                        excelSh.Cells[5 + i_pas, 1 + 5 * i_stop].Value = (pass.Time - time_6).TotalMinutes;
                        excelSh.Cells[5 + i_pas, 2 + 5 * i_stop].Value = pass.CodeDistrictFinish + 1;
                        excelSh.Cells[5 + i_pas, 3 + 5 * i_stop].Value = pass.CodeStopFinish + 1;
                        i_pas++;
                    }
                }
                package.Save();
            }
            file.Close();
        }
    }
}
