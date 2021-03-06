﻿using OfficeOpenXml;
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

        static Random rand = new Random(); //случайное число

        private static List<Stop> listStop; // создание списка для класса Stop
        private static List<District> listDist; // создание списка для класса District

        static double[,] mornWork; // матрица доли рабочих и молодёжи утром
        static double[,] mornPens; // матрица доли пенсионеров и школьников утром
        static double[,] dayTime; // матрица доли людей в обеденное время
        static double[,] evenWork; // матрица доли рабочих и молодёжи вечером
        static double[,] evenPens; // матрица доли пенсионеров и школьников вечером
        static double[,] timeDist; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия
        static double[,] ballWork; // матрица количества рабочих и молодёжи, находящихся в районе по часам
        static double[,] ballPens; // матрица количества пенсионеров и школьников, находящихся в районе по часам
        static int[,,] matrCountPasWork, matrCountPasPens; // матрицы численности пассажиров, пришедших на определённую остановку и определённое для отправления в конкретный район
        static double probability_of_arbitrary_choise; // Вероятность произвольного выбора остановки 
        static double probability_without_jump; // Вероятность выбора остановки без пересадки
        static double probability_use_pub_trans; // Доля пассажиров общественного транспорта, предпочитающего общественный транспорт
        static double probability_use_taxi; // Доля пассажиров общественного транспорта, предпочитающего маршрутки
        static double count_work_trip; // Количество поездок работников и молодежи за день
        static double count_pens_trip; // Количество поездок школьников и пенсионеров за день
        static double[,] attractive; // Вероятность выбора остановок прибытия



        public static void LoadFromExcel()
        {
            // создание списка для класса Stop
            listStop = new List<Stop>();
            // создание списка для класса District
            listDist = new List<District>();
            // Подгрузка файла с исходными данными
            FileInfo file = new FileInfo("данные\\Source_data.xlsx");
            using (ExcelPackage package = new ExcelPackage(file))
            {
                int code; // код i-го района
                int route; 
                //Сохранение листа "Население" в переменную
                ExcelWorksheet population = package.Workbook.Worksheets["Население"];
                int x = 10; // вспомогательная переменная
                //загрузка списка районов
                do //начало цикла
                {
                    code = population.Cells[x, 1].GetValue<int>(); //запись кода района с листа "Население", где х - строка, 1 - столбец, в которых находится нужная ячейка
                    string nameDist = population.Cells[x, 2].GetValue<string>(); //запись названия района с листа "Население"
                    int countWork = population.Cells[x, 4].GetValue<int>() + population.Cells[x, 5].GetValue<int>(); //запись количества жителей первой категории данного района с листа "Население"
                    int countPens = population.Cells[x, 3].GetValue<int>() + population.Cells[x, 6].GetValue<int>(); //запись количества жителей второй категории данного района с листа "Население"
                    // запись данных о районе в класс District
                    if (code != 0) //если переменная code получила код района
                    {
                        listDist.Add(
                        new District { CodeDistrict = code, NameDistrict = nameDist, CountWork = countWork, CountPens = countPens }
                        );
                    }
                    x++; // переход к следующей строке в Excel
                }
                while (code != 0); // цикл выполняется до тех пор, пока не дойдёт до пустой ячейки
                //загрузка доли пассажиров общественного транспорта, предпочитающего общественный транспорт
                probability_use_pub_trans = population.Cells["D3"].GetValue<double>();
                //загрузка доли пассажиров общественного транспорта, предпочитающего маршрутки
                probability_use_taxi = population.Cells["D4"].GetValue<double>();
                //загрузка количества поездок работников и молодежи за день
                count_work_trip = population.Cells["H3"].GetValue<double>();
                //загрузка количества поездок школьников и пенсионеров за день
                count_pens_trip = population.Cells["H4"].GetValue<double>();
                //загрузка вероятности выбора остановки без пересадки
                probability_of_arbitrary_choise = population.Cells["L6"].GetValue<double>();
                //загрузка вероятности произвольного выбора остановки 
                probability_without_jump = population.Cells["H6"].GetValue<double>();
                //загрузка вероятности выбора остановок прибытия
                attractive = new double[5, 2];
                for (int i = 0; i < attractive.GetLength(0); i++)
                {
                    for (int j = 0; j < attractive.GetLength(1); j++)
                    {
                        attractive[i, j] = population.Cells[3 + i, 14 + j].GetValue<double>();
                    }
                }
                //Сохранение листа "Маршруты" в переменную
                ExcelWorksheet routes = package.Workbook.Worksheets["Маршруты"];
                x = 2;
                //загрузка списка остановок
                do
                {
                    code = routes.Cells[x, 1].GetValue<int>(); // запись кода остановки с листа "Маршруты"
                    string nameStop = routes.Cells[x, 2].GetValue<string>(); // запись названия остановки с листа "Маршруты"
                    string district = routes.Cells[x, 3].GetValue<string>(); // запись названия района, в котором находится остановка с листа "Маршруты"
                    int countPass = routes.Cells[x, 4].GetValue<int>(); // запись количества пассажиров, приходящих на остановку, с листа "Маршруты"
                    int attraction = routes.Cells[x, 5].GetValue<int>(); // запись привлекательности остановки с листа "Маршруты"
                    List<int> listRoutes = new List<int>(); // создание списка маршрутов, проходящих через конкретную остановку
                    int countRoutes = routes.Cells[x, 6].GetValue<int>(); // запись количества маршрутов, проходящих через конкретную остановку
                    //заполнение списка маршрутов для каждой остановки
                    for (int i = 0; i < countRoutes; i++)
                    {
                        route = routes.Cells[x, i + 7].GetValue<int>();
                        listRoutes.Add(route);
                    }
                    // запись данных об остановке в класс Stop
                    if (code != 0)
                    {
                        Stop stop = new Stop { CodeStop = code, NameStop = nameStop, District = district, CountPass = countPass, Attraction = attraction, ListOfRoutes = listRoutes };
                        listStop.Add(stop);
                        //Запись в класс District списка остановок для каждого района
                        District d = listDist.Find(xx => xx.NameDistrict == district); //нахождение остановок, у которых название района совпадает с названием конкретного района
                        d.ListStop.Add(stop);
                    }
                    x++;
                }
                while (code != 0);
                //загрузка долей жителей зоны остановок
                for (int i_stop = 0; i_stop < listStop.Count(); i_stop++)
                {
                    for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                    {
                        // просчитываем и записываем доли жителей зоны остановки в класс Stop 
                        if (listStop[i_stop].District == listDist[i_region].NameDistrict) //нахождение остановок, у которых название района совпадает с названием конкретного района
                        {
                            double part_area = (double)listStop[i_stop].CountPass / (double)listDist[i_region].CountPass; //количество пассажиров, приходящих на остановку, делится на общее количество жителей района
                            listStop[i_stop].PercentageSitizen = part_area;
                        }
                    }
                }
                mornWork = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли рабочих и молодёжи утром
                mornPens = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли пенсионеров и школьников утром
                dayTime = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли людей в обеденное время
                evenWork = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли рабочих и молодёжи вечером
                evenPens = new double[COUNT_DISTRICT, COUNT_DISTRICT]; // матрица доли пенсионеров и школьников вечером
                timeDist = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия
                ballWork = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица количества рабочих и молодёжи, находящихся в районе по часам
                ballPens = new double[COUNT_DISTRICT, COUNT_HOUR]; // матрица количества пенсионеров и школьников, находящихся в районе по часам
                //загрузка прочих матриц
                for (int i = 0; i < COUNT_DISTRICT; i++)
                {
                    for (int j = 0; j < COUNT_DISTRICT; j++)
                    {
                        mornWork[i, j] = population.Cells[i + 25, j + 3].GetValue<double>(); // матрица доли рабочих и молодёжи утром
                        mornPens[i, j] = population.Cells[i + 37, j + 3].GetValue<double>(); // матрица доли пенсионеров и школьников утром
                        dayTime[i, j] = population.Cells[i + 49, j + 3].GetValue<double>(); // матрица доли людей в обеденное время
                        evenWork[i, j] = population.Cells[i + 61, j + 3].GetValue<double>(); // матрица доли рабочих и молодёжи вечером
                        evenPens[i, j] = population.Cells[i + 61, j + 15].GetValue<double>(); // матрица доли пенсионеров и школьников вечером
                    }
                }
                //загрузка матрицы распределения по времени
                for (int i = 0; i < COUNT_DISTRICT; i++)
                {
                    for (int j = 0; j < COUNT_HOUR; j++)
                    {
                        timeDist[i, j] = population.Cells[i + 73, j + 3].GetValue<double>(); // матрица доли пассажиров, отъезжающих от остановки, в зависимости от времени прибытия
                    }
                }
            }
        }

        public static void Balance()
        {
            double[] hour_region_workers = new double[COUNT_DISTRICT]; // количество жителей 1-й категории в определённом районе в конкретный час
            double[] hour_region_pensioners = new double[COUNT_DISTRICT]; // количество жителей 2-й категории в определённом районе в конкретный час
            double[] day_region_workers = new double[COUNT_DISTRICT]; // количество жителей 1-й категории в определённом районе
            double[] day_region_pensioners = new double[COUNT_DISTRICT]; // количество жителей 2-й категории в определённом районе
            double[] past_work = new double[COUNT_DISTRICT]; // прошлое значение жителей 1-й категории в районе
            double[] past_pens = new double[COUNT_DISTRICT]; // прошлое значение жителей 2-й категории в районе
            double[] actual_work = new double[COUNT_DISTRICT]; //жителей 1-й категории в районе к началу следующего часа
            double[] actual_pens = new double[COUNT_DISTRICT]; //жителей 2-й категории в районе к началу следующего часа
            // коэффициенты для расчёта перерпспределения между потоками
            double[,] k_flow_workers_morning = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_pensioners_morning = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_workers_daytime = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_pensioners_daytime = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_workers_evening = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            double[,] k_flow_pensioners_evening = new double[COUNT_DISTRICT, COUNT_DISTRICT];
            // Заполнение средних значений пассажиропотока значений, начиная с 6:00
            for (int n_hour = 0; n_hour < COUNT_HOUR; n_hour++)
            {
                // заполнение суммарного потока на начало текущего часа
                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    if (n_hour == 0) // Заполнение для 6:00 (0 соответствует 6:00, 1 - 7:00 и т.д.)
                    {
                        // общее число работников в районах на текущий час
                        hour_region_workers[i_region] = listDist[i_region].CountWork * probability_use_pub_trans * (1 - probability_use_taxi) * count_work_trip * timeDist[i_region, n_hour];
                        // общее число пенсионеров в районах на текущий час
                        hour_region_pensioners[i_region] = listDist[i_region].CountPens * (1 - probability_use_taxi) * count_pens_trip * timeDist[i_region, n_hour]; 
                        // общее число работников в районах за день
                        day_region_workers[i_region] = hour_region_workers[i_region] / timeDist[i_region, n_hour]; 
                        // общее число пенсионеров в районах за день
                        day_region_pensioners[i_region] = hour_region_pensioners[i_region] / timeDist[i_region, n_hour];
                    }
                    //коэффициенты перераспределения потоков между районами     
                    else // Заполнение для следующих часов
                    {
                        hour_region_workers[i_region] = actual_work[i_region] * timeDist[i_region, n_hour]; // расчёт количества жителей 1-й категории в определённом районе в конкретный час
                        hour_region_pensioners[i_region] = actual_pens[i_region] * timeDist[i_region, n_hour]; // расчёт количества жителей 2-й категории в определённом районе в конкретный час
                    }
                    // расчёт 
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {
                        if (n_hour <= 4) // для утреннего времени
                        {
                            k_flow_workers_morning[i_region, j_region] = mornWork[i_region, j_region] * hour_region_workers[i_region] / listDist[i_region].CountStops;
                            k_flow_pensioners_morning[i_region, j_region] = mornPens[i_region, j_region] * hour_region_pensioners[i_region] / listDist[i_region].CountStops;
                        }
                        else if (n_hour <= 9) // для обеденного времени
                        {
                            k_flow_workers_daytime[i_region, j_region] = dayTime[i_region, j_region] * hour_region_workers[i_region] / listDist[i_region].CountStops;
                            k_flow_pensioners_daytime[i_region, j_region] = dayTime[i_region, j_region] * hour_region_pensioners[i_region] / listDist[i_region].CountStops;
                        }
                        else // для вечернего времени
                        {
                            k_flow_workers_evening[i_region, j_region] = evenWork[i_region, j_region] * hour_region_workers[i_region] / listDist[i_region].CountStops;
                            k_flow_pensioners_evening[i_region, j_region] = evenPens[i_region, j_region] * hour_region_pensioners[i_region] / listDist[i_region].CountStops;
                        }
                    }
                    double flow_from_region_workers = 0;
                    double flow_from_region_pensioners = 0;
                    double flow_into_region_workers = 0;
                    double flow_into_region_pensioners = 0;
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {
                        if (j_region != i_region)
                        {
                            if (n_hour <= 4)
                            {
                                flow_from_region_workers += k_flow_workers_morning[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_from_region_pensioners += k_flow_pensioners_morning[i_region, j_region] / timeDist[i_region, n_hour];
                                flow_from_region_pensioners += k_flow_pensioners_morning[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_into_region_workers += k_flow_workers_morning[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                                flow_into_region_pensioners += k_flow_pensioners_morning[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                            }
                            else if (n_hour > 4 && n_hour <= 7)
                            {
                                flow_from_region_workers += k_flow_workers_daytime[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_from_region_pensioners += k_flow_pensioners_daytime[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_into_region_workers += k_flow_workers_daytime[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                                flow_into_region_pensioners += k_flow_pensioners_daytime[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                            }
                            else if (n_hour > 7 && n_hour <= 9)
                            {
                                flow_into_region_workers += k_flow_workers_daytime[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_into_region_pensioners += k_flow_pensioners_daytime[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_from_region_workers += k_flow_workers_daytime[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                                flow_from_region_pensioners += k_flow_pensioners_daytime[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                            }
                            else
                            {
                                flow_into_region_workers +=  k_flow_workers_evening[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_into_region_pensioners += k_flow_pensioners_evening[i_region, j_region] / timeDist[j_region, n_hour];
                                flow_from_region_workers += k_flow_workers_evening[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                                flow_from_region_pensioners += k_flow_pensioners_evening[j_region, i_region] / timeDist[j_region, n_hour] * listDist[j_region].CountStops;
                            }
                        }
                    }
                    if (n_hour <= 7)
                    {
                        flow_into_region_workers = flow_into_region_workers * timeDist[i_region, n_hour];
                        flow_into_region_pensioners = flow_into_region_pensioners * timeDist[i_region, n_hour];
                        flow_from_region_workers = flow_from_region_workers * listDist[i_region].CountStops * timeDist[i_region, n_hour];
                        flow_from_region_pensioners = flow_from_region_pensioners * listDist[i_region].CountStops * timeDist[i_region, n_hour];
                    }
                    else
                    {
                        flow_from_region_workers = flow_from_region_workers * timeDist[i_region, n_hour];
                        flow_from_region_pensioners = flow_from_region_pensioners * timeDist[i_region, n_hour];
                        flow_into_region_workers = flow_into_region_workers * listDist[i_region].CountStops * timeDist[i_region, n_hour];
                        flow_into_region_pensioners = flow_into_region_pensioners * listDist[i_region].CountStops * timeDist[i_region, n_hour];
                    }
                    // запись данных на первый час
                    if (n_hour == 0)
                    {
                        past_work[i_region] = day_region_workers[i_region];
                        past_pens[i_region] = day_region_pensioners[i_region];
                        actual_work[i_region] = past_work[i_region] + flow_into_region_workers - flow_from_region_workers;
                        actual_pens[i_region] = past_pens[i_region] + flow_into_region_pensioners - flow_from_region_pensioners;
                    }
                    else
                    {
                        // пересчёт данных на начало следующего часа
                        past_work[i_region] = actual_work[i_region];
                        past_pens[i_region] = actual_pens[i_region];
                        actual_work[i_region] = past_work[i_region] + flow_into_region_workers - flow_from_region_workers;
                        actual_pens[i_region] = past_pens[i_region] + flow_into_region_pensioners - flow_from_region_pensioners;
                    }
                    if (n_hour == 0)
                    {
                        hour_region_workers[i_region] = actual_work[i_region] * timeDist[i_region, n_hour + 1];
                        hour_region_pensioners[i_region] = actual_pens[i_region] * timeDist[i_region, n_hour + 1];
                    }
                    //загрузка числа пассажиров, находящихся в районах по часам
                    ballWork[i_region, n_hour] = past_work[i_region];
                    ballPens[i_region, n_hour] = past_pens[i_region];
                }
            }
        }

        public static void GeneratePass()
        {
            Balance(); // выполняется метод Balance()
            SetCorrespondenceStops(); // выполняется метод SetCorrespondenceStops()
            matrCountPasWork = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT]; // матрицы численности пассажиров 1-й категории, пришедших на определённую остановку и определённое для отправления в конкретный район
            matrCountPasPens = new int[listStop.Count, COUNT_HOUR, COUNT_DISTRICT]; // матрицы численности пассажиров 2-й категории, пришедших на определённую остановку и определённое для отправления в конкретный район
            for (int n_hour = 0; n_hour < COUNT_HOUR; n_hour++)
            {
                //Заполнение численности пассажиров на остановках на текущий час
                double[] matrFlowPasWork = new double[listStop.Count];
                double[] matrFlowPasPens = new double[listStop.Count];
                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    double number_of_workers = ballWork[i_region, n_hour];
                    double number_of_pensioners = ballPens[i_region, n_hour];

                    double number_of_workers_hour = number_of_workers * timeDist[i_region, n_hour];
                    double number_of_pensioners_hour = number_of_pensioners * timeDist[i_region, n_hour];

                    for (int j_stops = 0; j_stops < listDist[i_region].CountStops; j_stops++)
                    {
                        Stop stop = listDist[i_region].GetStop(j_stops);
                        matrFlowPasWork[stop.CodeStop - 1] = number_of_workers_hour * stop.PercentageSitizen; // среднее количество поездок пассажиров 1-й категории с отдельной остановки определённого района за конкретный час
                        matrFlowPasPens[stop.CodeStop - 1] = number_of_pensioners_hour * stop.PercentageSitizen; // среднее количество поездок пассажиров 2-й категории с отдельной остановки определённого района за конкретный час
                    }
                }
                for (int i_region = 0; i_region < COUNT_DISTRICT; i_region++)
                {
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {
                        double time_percent1 = mornWork[i_region, j_region]; // учет различий в перетоках между районами для жителей 1-й категории
                        double time_percent2 = mornPens[i_region, j_region]; // учет различий в перетоках между районами для жителей 2-й категории
                        //Цикл по числу остановок района прибытия
                        for (int k_stops = 0; k_stops < listDist[i_region].CountStops; k_stops++)
                        {
                            Stop stop = listDist[i_region].ListStop[k_stops];
                            //Расчёт мат.ожиданий
                            double lambda1 = matrFlowPasWork[stop.CodeStop - 1] * time_percent1;
                            double lambda2 = matrFlowPasPens[stop.CodeStop - 1] * time_percent2;
                            // Генерация Пуассона; вызывается метод Poisson_value
                            int x_lambda1 = Poisson_value(lambda1); 
                            int x_lambda2 = Poisson_value(lambda2);
                            matrCountPasWork[stop.CodeStop - 1, n_hour, j_region] = x_lambda1; // генерация часового потока пассажиров 1-й категории
                            matrCountPasPens[stop.CodeStop - 1, n_hour, j_region] = x_lambda2; // генерация часового потока пассажиров 2-й категории
                        }
                    }
                }
            }
        }
        public static int Poisson_value(double lambda) // на вход поступает мат. ожидание, просчитанное в методе GeneratePass()
        {
            if (lambda <= 30) // если мат. ожидание не больше 30, то работает пуассоновское распределение
            {
                int x = 0;
                double a = Math.Exp(-lambda);

                double s = rand.NextDouble(); // генерируется случайное число в интервале от 0 до 1
                while (s >= a)
                {
                    x = x + 1;
                    s = s * rand.NextDouble();
                }
                return x; // возвращаем количество пришедших на остановку пассажиров
            }
            else
            {
                double multi = rand.NextDouble();  // генерируется случайное число в интервале от 0 до 1
                double a1 = Math.Pow(-2 * Math.Log(multi), 0.5);
                double a2 = Math.Cos(2 * Math.PI * multi);
                double a3 = Math.Pow(lambda, 0.5) * a1 * a2 + lambda;
                return (int)a3; // возвращаем количество пришедших на остановку пассажиров
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
            List<Passenger> listPass = new List<Passenger>(); // задаётся лист поминутного пассажиропотока для класса Passenger

            for (int i_stop = 0; i_stop < listStop.Count; i_stop++)
            {
                for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                {
                    int n_passengers_hour = 0;
                    for (int j_region = 0; j_region < COUNT_DISTRICT; j_region++)
                    {
                        int n_passengers_region = matrCountPasWork[i_stop, i_hour, j_region] + matrCountPasPens[i_stop, i_hour, j_region]; // создаётся общий список пассажиров
                        n_passengers_hour = n_passengers_hour + n_passengers_region;
                        if (n_passengers_region > 0)
                        {
                            for (int n_passengers = 0; n_passengers < n_passengers_region; n_passengers++)
                            {
                                int minutes = rand.Next(0, 60); // генерируются минуты
                                TimeSpan TimeS = new TimeSpan(6 + i_hour, minutes, 0); // задаётся случайное время
                                int codeFinish = ChoiseOfStop(listStop[i_stop], j_region); // генерируется остановка назначения; вызов метода ChoiseOfStop();
                                listPass.Add(new Passenger { CodeStopStart = i_stop, Time = TimeS, CodeDistrictFinish = j_region, CodeStopFinish = codeFinish }); // заполняется лист сгенерированных пассажиров
                            }
                        }
                    }
                }
            }
            int z = listPass.Where(x => x.CodeStopFinish == -1).Count();
            return listPass;
        }

        private static int ChoiseOfStop(Stop stopStart, int j_region) // входные параметры - остановка отправления и регион назначения
        {
            double shance = rand.NextDouble(); // шанс выбрать остановку случайно
            // вероятность выбора любой остановки
            if (shance < probability_of_arbitrary_choise) // если случайное число будет меньше вероятности произвольного выбора остановки, то задаётся случайная остановка назначения из списка остановок
            {
                return (int)(shance * listStop.Count) + 1;
            }
            // вероятность выбора остановки в заданном районе
            else
            {
                double zzz = rand.NextDouble(); // случайное число
                // вероятность выбора остановки без пересадки
                if (zzz <= probability_without_jump) // если случайное число не превышает вероятность выбора остановки без пересадки, то...
                {
                    List<Stop> list_stop_region = stopStart.StopWoTransfer.Where(x => x.District == listDist[j_region].NameDistrict).ToList(); // задаётся лист остановок района назначения
                    //если нет остановки в выбранном районе - задаётся случайная остановка
                    if (list_stop_region.Count == 0)
                        return (int)rand.Next(stopStart.StopWoTransfer.Count) + 1;

                    // формирование классов для каждой остановки
                    List<int> list_group = new List<int>(); // лист классов остановок
                    foreach (Stop stop in list_stop_region) // для каждой остановки из листа остановок района назначения
                    {
                        for (int i_a = 0; i_a < attractive.GetLength(0); i_a++)
                        {
                            if (stop.Attraction <= attractive[i_a, 0]) // сравнение: попадает ли привлекательность остановки в какую-либо одну из пяти интервалов привлекательности
                            {
                                list_group.Add(i_a);
                                break;
                            }
                        }
                    }
                    // выбирается номер класса остановок
                    var list_group_dist = list_group.Distinct().ToList();
                    int count_group = list_group_dist.Count();
                    double uuu = rand.NextDouble();
                    double border = 0;
                    int i_group = 0;
                    for (int i = 0; i < list_group_dist.Count; i++)
                    {
                        border += attractive[list_group_dist[i], 1];
                        if (uuu <= border)
                        {
                            i_group = list_group_dist[i];// номер класса, куда поедет пассажир
                            break;
                        }
                    }
                    // генерируется остановка из выбранного класса
                    int count_stop_group = list_group.Where(x => x == i_group).Count();
                    int number_of_stop = (int)(count_stop_group * rand.NextDouble());
                    int n_of_stop = 0;
                    for (int i = 0; i < list_group.Count; i++)
                    {
                        if (list_group[i] == i_group)
                        {
                            if (n_of_stop == number_of_stop)
                            {
                                return i; // возвращается номер остановки назначения
                            }
                            n_of_stop++;
                        }
                    }
                }
                // вероятность выбора остановки с одной пересадкой
                else
                {
                    List<Stop> list_stop_region = stopStart.StopWithTransfer.Where(x => x.District == listDist[j_region].NameDistrict).ToList();
                    //если нет остановки в выбранном районе - задаётся случайная остановка
                    if (list_stop_region.Count == 0)
                        return (int)rand.Next(stopStart.StopWithTransfer.Count) + 1;

                    // формирование классов для каждой остановки
                    List<int> list_group = new List<int>(); // лист классов остановок
                    foreach (Stop stop in list_stop_region) // для каждой остановки из листа остановок района назначения
                    {
                        for (int i_a = 0; i_a < attractive.GetLength(0); i_a++)
                        {
                            if (stop.Attraction <= attractive[i_a, 0]) // сравнение: попадает ли привлекательность остановки в какую-либо одну из пяти интервалов привлекательности
                            {
                                list_group.Add(i_a);
                                break;
                            }
                        }
                    }
                    // выбирается номер класса остановок
                    var list_group_dist = list_group.Distinct().ToList();
                    int count_group = list_group_dist.Count();
                    double uuu = rand.NextDouble();
                    double border = 0;
                    int i_group = 0;
                    for (int i = 0; i < list_group_dist.Count; i++)
                    {
                        border += attractive[list_group_dist[i], 1];
                        if (uuu <= border)
                        {
                            i_group = list_group_dist[i];// номер класса, куда поедет пассажир
                            break;
                        }
                    }
                    // генерируется остановка из выбранного класса
                    int count_stop_group = list_group.Where(x => x == i_group).Count();
                    int number_of_stop = (int)(count_stop_group * rand.NextDouble());
                    int n_of_stop = 0;
                    for (int i = 0; i < list_group.Count; i++)
                    {
                        if (list_group[i] == i_group)
                        {
                            if (n_of_stop == number_of_stop)
                            {
                                return i; // возвращается номер остановки назначения
                            }
                            n_of_stop++;
                        }
                    }
                }
            }
            return -2; // если ошибка
        }
        public static void SaveToSheets_test(List<Passenger> listPass) //получает на входе сгенерированный пассажиропоток
        {
            FileStream file = File.Create("данные\\traffic.xlsx"); // создание файла traffic.xlsx
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet excelSh = package.Workbook.Worksheets.Add("Hour_generate");
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
                    excelSh.Cells[3 + k_stop, 1].Value = k_stop + 1;
                    for (int j_dist = 0; j_dist < COUNT_DISTRICT; j_dist++)
                    {
                        excelSh.Cells[2, 2 + j_dist + 20 * COUNT_HOUR].Value = listDist[j_dist].NameDistrict;
                        excelSh.Cells[2, 2 + COUNT_DISTRICT + j_dist + 20 * COUNT_HOUR].Value = listDist[j_dist].NameDistrict;
                        int sumw = 0, sump = 0;
                        for (int i_hour = 0; i_hour < COUNT_HOUR; i_hour++)
                        {
                            sumw += matrCountPasWork[k_stop, i_hour, j_dist];
                            sump += matrCountPasPens[k_stop, i_hour, j_dist];
                        }
                        excelSh.Cells[3 + k_stop, 2 + j_dist + 20 * COUNT_HOUR].Value = sumw;
                        excelSh.Cells[3 + k_stop, 2 + COUNT_DISTRICT + j_dist + 20 * COUNT_HOUR].Value = sump;
                    }
                }

                excelSh = package.Workbook.Worksheets.Add("traffic"); // добавления листа "traffic"
                TimeSpan time_6 = new TimeSpan(6, 0, 0);
                // Форматирование пассажиропотока
                for (int i_stop = 0; i_stop < listStop.Count(); i_stop++)
                {
                    excelSh.Cells[3, 1 + 5 * i_stop].Value = listStop[i_stop].CodeStop; // запись остановки отправления пассажира
                    // Названия стобцов
                    excelSh.Cells[4, 1 + 5 * i_stop].Value = "Время";
                    excelSh.Cells[4, 2 + 5 * i_stop].Value = "Р-н прибытия";
                    excelSh.Cells[4, 3 + 5 * i_stop].Value = "Код ост прибытия";
                    List<Passenger> list = listPass.Where(x => x.CodeStopStart == i_stop).ToList(); 
                    list.Sort(); // сортировка по времени отправления
                    int i_pas = 0;
                    foreach (Passenger pass in list) // для каждого пассажира из списка пассажиров
                    {
                        excelSh.Cells[5 + i_pas, 1 + 5 * i_stop].Value = (pass.Time - time_6).TotalMinutes; // запись времени отправления пассажира (в минутах)
                        excelSh.Cells[5 + i_pas, 2 + 5 * i_stop].Value = pass.CodeDistrictFinish + 1; // запись района назначения пассажира
                        excelSh.Cells[5 + i_pas, 3 + 5 * i_stop].Value = pass.CodeStopFinish + 1; // запись остановки назначения пассажира
                        i_pas++;
                    }
                }
                package.Save(); // сохранить файл
            }
            file.Close();
        }
    }
}
