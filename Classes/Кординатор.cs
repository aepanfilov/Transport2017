using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace “ранспорт2017
{
    /// <summary>
    /// отвечает за моделирование, согласованность работы модели
    /// </summary>
    public class  ординатор
    {
        public const double —–—коростьƒвижени€ = 20;

        private List<јвто> массивјвто;
        private ќстановка[] массивќстановок;
        private ћаршрут[] массивћаршрутов;

        //private int такт;
        public List<ѕассажир> архивѕассажиров;//список  пассажиров, по€вившихс€ на остановке
        public List<List<јвто>> архивјвтоѕоћаршрутам;//список авто, завершивших рейс, в списке маршрутов
        public ѕоказатели–аботыƒн€ показатели–аботы;
        public int кол„асов–аботы;
        public bool ћодель–асчитана;
        public ћатрица орреспонденций ћатрица орр;

        public int  олћаршрутов { get { return массивћаршрутов != null ? массивћаршрутов.Length : 0; ; } }
        public int  олќстановок { get { return массивќстановок != null ? массивќстановок.Length : 0; } }

        public event EventHandler<IntEventArg> “актќтработан;
        //конструктор класса
        public  ординатор()
        {
            массивјвто = new List<јвто>();
            архивѕассажиров = new List<ѕассажир>();
            показатели–аботы = new ѕоказатели–аботыƒн€();

            //св€зать событие "јвто.приездЌаќстановку" с обработчиком " оординатор.добавитьјвтоЌаќстановку"
            јвто.—обытиеѕриездЌаќстановку += ƒобавитьјвтоЌаќстановку;
            //св€зать событие "ѕассажир.—обытиеѕо€вилс€" с обработчиком " оординатор.пассажирѕо€вилс€Ќаќстановке"
            ѕассажир.—обытиеѕо€вилс€ += ѕассажирѕо€вилс€Ќаќстановке;
            //св€зать событие "ѕассажир.—обытиеƒоехал" с обработчиком " оординатор.пассажирƒоехал"
            ѕассажир.—обытиеƒоехал += ѕассажирƒоехал;
            //св€зать событие "јвто.—обытиеќтправление—ќстановки" с обработчиком " оординатор.отправитьјвто—ќстановки"
            јвто.—обытиеќтправление—ќстановки += ќтправитьјвто—ќстановки;
            //св€зать событие "ћаршрут.—обытиеЌачать–ейс" с обработчиком " оординатор.начатьЌовый–ейс"
            ћаршрут.—обытиеЌачать–ейс += Ќачать–ейс;
        }

        //    public Property ћассивћаршрутов() As ћаршрут()
        //        Get
        //            return _массивћаршрутов
        //         Get
        //        Set(ByVal value As ћаршрут())
        //            добавитьћаршруты(value)
        //         Set
        //     Property

        //    public Property ћассивќстановок() As “ранспорт.ќстановка()
        //        Get
        //            return _массивќстановок
        //         Get
        //        Set(ByVal value As “ранспорт.ќстановка())
        //            добавитьќстановки(value)
        //         Set
        //     Property

        //    public ReadOnly Property “акт() As Integer
        //        Get
        //            return _такт
        //         Get
        //     Property
        //подготовка дл€ очередного прогона модели
        private void »нициализаци€()
        {
            if (массивјвто.Count > 0)
                массивјвто.Clear();
            foreach (ћаршрут марш in массивћаршрутов)
                марш.»нициализаци€();
            foreach (ќстановка ост in массивќстановок)
                ост.»нициализаци€();
            if (архивјвтоѕоћаршрутам != null)
                for (int i = 0; i < архивјвтоѕоћаршрутам.Count; i++)
                    архивјвтоѕоћаршрутам[i].Clear();
            архивѕассажиров.Clear();
            показатели–аботы.»нициализаци€();
        }

        //рассчет модели на текущем такте такт
        private void ќтработать“акт(int тек“акт)
        {
            //такт = тек“акт;
            //начать новый рейс
            foreach (ћаршрут марш in массивћаршрутов)
                марш.ќтработать“акт(тек“акт);
            //приезд/отъезд авто с остановки
            foreach (јвто авто in массивјвто)
                авто.ќтработать“акт(тек“акт);
            //удаление авто завершивших рейс из массивајвто
            int i = 0;
            while (i < массивјвто.Count)
            {
                јвто авто = массивјвто[i];
                if (авто.состо€ние == —осто€ниејвтоEnum.рейс«авершен)
                    «авершить–ейс(авто);
                else
                    i++;
            }
            //генераци€ пассажиров, посадка на авто
            foreach (ќстановка ост in массивќстановок)
                ост.ќтработать“акт(тек“акт);
        }

        //удалить авто с соответствующей остановки
        private void ќтправитьјвто—ќстановки(object obj, AutoEventArg arg)
        {
            јвто авто = obj as јвто;
            массивќстановок[arg.кодќст - 1].ќтправлениејвто(авто);
            //foreach (ќстановка ост in массивќстановок)
            //    if (ост. од == arg.кодќст)
            //    {
            //        ост.отправлениејвто(авто);
            //        break;
            //    }
        }

        //обработчик событи€ "јвто.приездЌаќстановку"
        private void ƒобавитьјвтоЌаќстановку(object obj, AutoEventArg arg)
        {
            јвто авто = obj as јвто;
            if (массивќстановок[arg.кодќст - 1].ѕриездјвто(авто) == false)
                //место дл€ авто нет
                авто.∆дать—вободногоћеста();
            else
                //место дл€ авто есть
                авто.ѕриездЌаќстановку–азрешен(arg.тек“акт);
            //foreach (ќстановка ост in массивќстановок)
            //    if (ост. од == arg.кодќст)
            //    {
            //        if (ост.приездјвто(авто) == false)
            //            //место дл€ авто нет
            //            авто.ждать—вободногоћеста();
            //        else
            //            //место дл€ авто есть
            //            авто.приездЌаќстановку–азрешен(arg.тек“акт);
            //        break;
            //    }
        }

        //добавление данных об остановке при загрузке из Ѕƒ
        public void ƒобавитьќстановки(ќстановка[] остановки)
        { массивќстановок = остановки; }

        //добавление данных о маршруте при загрузке из Ѕƒ
        public void ƒобавитьћаршруты(ћаршрут[] маршруты)
        {
            массивћаршрутов = маршруты;
            //сформировать список(массив) дл€ архива маршрутов, каждый элемент которого €вл€етс€ 
            // списком(массивом) авто данного маршрута
            архивјвтоѕоћаршрутам = new List<List<јвто>>(маршруты.Length);
            for (int i = 0; i < массивћаршрутов.Length; i++)
            {
                List<јвто> архивћаршрута = new List<јвто>();
                архивјвтоѕоћаршрутам.Add(архивћаршрута);
                массивћаршрутов[i].архивјвто = архивћаршрута;
            }
        }

        // ѕроведение моделировани€ движени€ авто по всем маршрутам одного дн€
        //(ѕредуслови€: массивы "массивќстановок" и "массивћаршрутов" должны содержать справочные данные)
        public void ћоделирование1ƒн€()//object ser)
        {
            //Dim bgw As BackgroundWorker = CType(ser, BackgroundWorker)
            »нициализаци€();
            //*** моделирование по тактам времени (по минутно)
            for (int тек“акт = 0; тек“акт < кол„асов–аботы * 60; тек“акт++)
            {
                ќтработать“акт(тек“акт);
                //bgw.ReportProgress(тек“акт); /// кол„асов–аботы * 60 - 1 )
                “актќтработан?.Invoke(this, new IntEventArg(тек“акт + 1));
            }

            //*** расчет показателей
            //расчет показателей по остановкам
            foreach (ќстановка ост in массивќстановок)
                ост.показатели–аботы.завершениећоделировани€(архивѕассажиров, кол„асов–аботы, ост. олѕас);

            //расчет показателей по маршрутам
            foreach (ћаршрут марш in массивћаршрутов)
                марш.ѕоказатели–аботы.–асчетѕоказателей(кол„асов–аботы);
            //расчет показателей за весь период моделировани€ (день)
            показатели–аботы.–асчетѕоказателей(массивћаршрутов, массивќстановок, массивјвто, кол„асов–аботы);
            ћодель–асчитана = true;
        }

        //обработчик событи€ "ѕассажир.—обытиеƒоехал"
        private void ѕассажирƒоехал(object obj, IntEventArg arg)
        {
            массивќстановок[arg.value - 1].¬ысадкаѕассажира(obj as ѕассажир);
            //foreach (ќстановка ост in массивќстановок)
            //    if (ост. од == arg.value)
            //    {
            //        ост.высадкаѕассажира(obj as ѕассажир);
            //        break;
            //    }
        }

        //обработчик событи€ "ѕассажир.—обытиеѕо€вилс€"
        public void ѕассажирѕо€вилс€Ќаќстановке(object obj, IntEventArg arg)
        {
            архивѕассажиров.Add(obj as ѕассажир);
        }

        //обработчик завершени€ рейса авто
        private void «авершить–ейс(јвто авто)
        {
            массивјвто.Remove(авто);
            архивјвтоѕоћаршрутам[авто. одћаршрута - 1].Add(авто);
            ////найти маршрут в массиве архивјвтоѕоћаршрутам дл€ доблени€ в него авто
            //for (int i = 0; i < массивћаршрутов.Length; i++)
            //    if (массивћаршрутов[i]. од == авто. одћаршрута)
            //    {
            //        архивјвтоѕоћаршрутам[i].Add(авто);
            //        break;
            //    }
        }

        //обработчик событи€ "ћаршрут.—обытиеЌачать–ейс"
        private void Ќачать–ейс(object obj, IntEventArg arg)
        {
            ћаршрут маршрут–ейса = obj as ћаршрут;
            //добавление нового авто на начальную остановку
            јвто auto = new јвто();
            auto.»нициализаци€(маршрут–ейса, маршрут–ейса.¬местимость—лед–ейса, arg.value);
            массивјвто.Add(auto);
        }

        //запись отчета по всем остановкам, авто, маршрутам в Excel после их моделировани€
        public bool «аписьќтчета¬Excel(out string errMsg)
        {
            try
            {
                if (!ѕротоколExcel.ќткрыть‘айл(out errMsg))
                    return false;
                foreach (ћаршрут марш in массивћаршрутов)
                    if (марш.ѕоказатели–аботы.кол¬ыполнен–ейсов > 0)
                    {
                        ѕротоколExcel.ƒобавитьЋист("маршрут_" + марш. од);

                        ѕротоколExcel.«аписатьћаршрут(марш);
                    }

                if (SettingsModel.ѕолныйќтчет)
                {
                    ѕротоколExcel.ƒобавитьЋист("авто_завершен");
                    ѕротоколExcel.ѕереместитьЋист("авто_завершен", 1);
                    foreach (List<јвто> marsh in архивјвтоѕоћаршрутам)
                        foreach (јвто auto in marsh)
                            ѕротоколExcel.«аписатьјвто(auto);

                    ѕротоколExcel.ƒобавитьЋист("авто_в_пути");
                    ѕротоколExcel.ѕереместитьЋист("авто_в_пути", 1);
                    foreach (јвто auto in массивјвто)
                        ѕротоколExcel.«аписатьјвто(auto);

                    ѕротоколExcel.ƒобавитьЋист("авто_таблица");
                    ѕротоколExcel.ѕереместитьЋист("авто_таблица", 1);
                    ѕротоколExcel.«аписатьјвто–ейсы(архивјвтоѕоћаршрутам, массивјвто);
                }

                if (SettingsModel.ѕасажиров¬ќтчет)
                {
                    ѕротоколExcel.ƒобавитьЋист("пас");
                    ѕротоколExcel.ѕереместитьЋист("пас", 1);
                    ѕротоколExcel.«аписатьѕас(архивѕассажиров);
                }

                ѕротоколExcel.ƒобавитьЋист("остановки");
                ѕротоколExcel.ѕереместитьЋист("остановки", 1);
                ѕротоколExcel.«аписать¬сеќстановки(массивќстановок);

                //foreach (ќстановка ост in массивќстановок)
                //    ////        if ост.ѕоказатели–аботы.—умм олѕо€вившѕассажиров > 0 )
                //    ѕротоколExcel.«аписатьќстановку(ост.показатели–аботы, ост.Ќазвание);

                //таблица по маршрутам
                ѕротоколExcel.ƒобавитьЋист("рейсы_по_маршрутам");
                ѕротоколExcel.ѕереместитьЋист("рейсы_по_маршрутам", 1);
                ѕротоколExcel.«аписать“аблћаршрут(массивћаршрутов);

                ѕротоколExcel.ƒобавитьЋист("распределение_“—");
                ѕротоколExcel.ѕереместитьЋист("распределение_“—", 1);
                ѕротоколExcel.«аписать–аспределение“—(массивћаршрутов);

                ѕротоколExcel.ƒобавитьЋист("день");
                ѕротоколExcel.ѕереместитьЋист("день", 1);
                ѕротоколExcel.«аписатьƒень(показатели–аботы, кол„асов–аботы);

                ѕротоколExcel.«акрыть‘айл();
                //ѕротоколExcel.ѕоказать();
            }
            catch (Exception exc)
            {
                errMsg = exc.Message;
                return false;
            }
            return true;
        }
        private –ейс[] GenerateReis(int capacity, int interval)//генераци€ списка рейсов с заданным интервалом
        {
            int cnt = SettingsModel. ол„асовћоделировани€ * 60 / interval;
            –ейс[] listReis = new –ейс[cnt];
            for (int i = 0; i < cnt; i++)
            {
                listReis[i] = new –ейс
                {
                    врем€ќтправлени€ = i * interval,//"врем€"
                    ћакс¬местимость = capacity //"вместимость"
                };
            }
            return listReis;
        }
        private List<ћаршрут> GetDoublicateRoute(ћаршрут marsh) //список маршрутов, которые дублируют по остановкам на 25% marsh
        {
            int[] masOst = marsh. одыќстановок;
            int limit = (int)(masOst.Length * 0.25);
            List<ћаршрут> listRoute = new List<ћаршрут>();
            foreach (ћаршрут марш in массивћаршрутов)
            {
                if (марш == marsh || марш.“ип“ранспорта == “ип“ранспортаћаршрутаEnum.троллейбус || марш.“ип“ранспорта == “ип“ранспортаћаршрутаEnum.трамвай)
                    continue;
                int[] masOst2 = марш. одыќстановок;
                int count = 0;
                for (int i = 0; i < masOst.Length; i++)
                {
                    int ost = masOst[i];
                    for (int j = 0; j < masOst2.Length; j++)
                        if (ost == masOst2[j])
                            count++;
                }
                if (count >= limit)
                    listRoute.Add(марш);
            }
            return listRoute;
        }
        //public string testTrol()
        public List<“роллейбусыƒл€ѕротокола> TestTrol()
        {
            const int COUNTTROLLALL = 240;
            //StringBuilder sb = new StringBuilder();
            List<“роллейбусыƒл€ѕротокола> listRes = new List<“роллейбусыƒл€ѕротокола>();
            //получить список троллейбусных маршрутов
            ћаршрут[] masTrol = (from m in массивћаршрутов where m.“ип“ранспорта == “ип“ранспортаћаршрутаEnum.троллейбус select m).ToArray<ћаршрут>();
            bool[] masPaired = new bool[masTrol.Length];

            //найти маршруты-дублеры троллейбусных маршрутов
            List<List<ћаршрут>> listDubl = new List<List<ћаршрут>>();
            List<List<int>> listDublCountTC = new List<List<int>>();
            List<List<int>> listDublInterval = new List<List<int>>();
            List<List<bool>> listDublPaired = new List<List<bool>>();
            bool nextIsPairedDubl;
            for (int i = 0; i < masTrol.Length; i++)
            {
                List<ћаршрут> listD = GetDoublicateRoute(masTrol[i]);
                listDubl.Add(listD);
                //сохранить параметры базового варианта
                listDublCountTC.Add((from m in listD select m.„исло“—наћаршруте).ToList<int>());
                listDublInterval.Add((from m in listD select (int)Math.Round(2.0 * m.¬рем€1–ейса / m.„исло“—наћаршруте)).ToList<int>());
                //парность маршрутов-дублей
                List<bool> lb = new List<bool>(listD.Count);
                for (int j = 0; j < listD.Count-1; j++)
                {
                    nextIsPairedDubl = TestNextPaired(listD[j], listD[j + 1]);
                    lb.Add(nextIsPairedDubl);
                    if (nextIsPairedDubl)
                    {
                        lb.Add(false);
                        j++;
                    }
                }
                listDublPaired.Add(lb);
                //проверка, что следующий - пр€мой/обратный маршрут
                masPaired[i] = i< masTrol.Length - 1 && TestNextPaired(masTrol[i], masTrol[i+1]);
                if (masPaired[i])
                {
                    listDubl.Add(null);
                    //сохранить параметры базового варианта
                    listDublCountTC.Add(null);
                    listDublInterval.Add(null);
                    listDublPaired.Add(null);
                    i++;
                }                
            }

            int[] masOst;
            –ейс[] masRoute;
            int countTrollAll = masTrol.Sum((ћаршрут m) => m.„исло“—наћаршруте);
            //задать интервал движени€ троллейбусному маршруту
            for (int i_marsh = 0; i_marsh < masTrol.Length; i_marsh++)
            {
                masOst=null;
                ////проверка, что следующий - пр€мой/обратный маршрут
                //nextIsPaired = i_marsh < masTrol.Length - 1 ? TestNextPaired(masTrol[i_marsh], masTrol[i_marsh+1]) : false;
                //сохранить параметры базового варианта
                int countTCBase = masTrol[i_marsh].„исло“—наћаршруте;//число “— по базовому варианту
                int intervalBase = (int)Math.Round(2.0 * masTrol[i_marsh].¬рем€1–ейса / countTCBase);//интервал движени€ “— по базовому варианту
                //int prevCounTC=0, prevInterval;//параметры предыдущего расчета - чтобы не расчитывать варианты с одинаковыми параметрами
                for (int i_int = intervalBase; i_int >= 1; i_int--)
                {
                    //проверить число “— троллейбусов
                    int countTC = (int)Math.Round(2.0 * masTrol[i_marsh].¬рем€1–ейса / i_int);//число “— по очередному варианту
                    //int countTC_ = nextIsPaired ? countTrollAll - 2 * countTCBase + 2 * countTC : countTrollAll - countTCBase + countTC;
                    int countTC_ = masPaired[i_marsh] ? countTrollAll - 2 * countTCBase + 2 * countTC : countTrollAll - countTCBase + countTC;
                    if (countTC_ > COUNTTROLLALL)
                        break;

                    //задать интервал движени€ троллейбусному маршруту
                    masRoute = GenerateReis(masTrol[i_marsh].¬местимость1–ейса, i_int);
                    masTrol[i_marsh].«адатьћассив–ейсов(masRoute);
                    masTrol[i_marsh].„исло“—наћаршруте = countTC;
                    if (masPaired[i_marsh])//nextIsPaired)
                    {
                        masTrol[i_marsh + 1].«адатьћассив–ейсов(masRoute);//GenerateReis??
                        masTrol[i_marsh + 1].„исло“—наћаршруте = countTC;
                    }
                    double kInterval = 1.0 * (i_int - 1) / (intervalBase - 1);
                    //задать интервалы движени€ дублеров
                    if (kInterval < 1.0)
                        for (int i_dub = 0; i_dub < listDubl[i_marsh].Count; i_dub++)
                        {
                            //проверка, что следующий - пр€мой/обратный маршрут
                            //nextIsPairedDubl = i_dub < listDubl[i_marsh].Count - 1 ? TestNextPaired(listDubl[i_marsh][i_dub], listDubl[i_marsh][i_dub + 1]) : false;
                            nextIsPairedDubl = listDublPaired[i_marsh][i_dub];
                            int newInt = (int)Math.Round(2.0 * listDubl[i_marsh][i_dub].¬рем€1–ейса / listDublCountTC[i_marsh][i_dub] / kInterval);
                            int newCountTC = (int)Math.Round(2.0 * listDubl[i_marsh][i_dub].¬рем€1–ейса / newInt);//число “— по очередному варианту
                            newCountTC = Math.Max(newCountTC, 1);
                            if (listDubl[i_marsh][i_dub].„исло“—наћаршруте != newCountTC)
                            {
                                listDubl[i_marsh][i_dub].„исло“—наћаршруте = newCountTC;
                                newInt = (int)Math.Round(2.0 * listDubl[i_marsh][i_dub].¬рем€1–ейса / newCountTC);
                                masRoute = GenerateReis(listDubl[i_marsh][i_dub].¬местимость1–ейса, newInt);
                                listDubl[i_marsh][i_dub].«адатьћассив–ейсов(masRoute);
                                if (nextIsPairedDubl)
                                {
                                    listDubl[i_marsh][i_dub + 1].„исло“—наћаршруте = newCountTC;
                                    listDubl[i_marsh][i_dub + 1].«адатьћассив–ейсов(masRoute);//GenerateReis??
                                }
                            }
                            if (nextIsPairedDubl)
                                i_dub++;
                        }

                    //скорректировть генерацию пассажиров на остановках троллейбусного маршрута /// и дублерах
                    masOst = masTrol[i_marsh]. одыќстановок;
                    double k = 12.0 / (60/i_int + 12); //коэф. вычеркивани€ пассажиров
                    for (int i = 0; i < masOst.Length; i++)
                        массивќстановок[masOst[i] - 1]. оэф¬ычеркивани€ = k;//коррекци€√енераторѕас(k);

                    //запуск моделировани€
                    ћоделирование1ƒн€();

                    //сохранение результатов моделировани€
                    //TempSaveResult(sb, показатели–аботы, masTrol[i_marsh]. од, i_int);
                    TempSaveResult2(listRes, masTrol, masPaired, i_marsh, i_int);
                }
                //восстановить базовый вариант дл€ i_marsh ?
                if (masTrol[i_marsh].„исло“—наћаршруте != countTCBase)
                {
                    masTrol[i_marsh].«адатьћассив–ейсов(GenerateReis(masTrol[i_marsh].¬местимость1–ейса, intervalBase));
                    masTrol[i_marsh].„исло“—наћаршруте = countTCBase;
                    if (masPaired[i_marsh])//nextIsPaired)
                    {
                        masTrol[i_marsh + 1].«адатьћассив–ейсов(GenerateReis(masTrol[i_marsh].¬местимость1–ейса, intervalBase));
                        masTrol[i_marsh + 1].„исло“—наћаршруте = countTCBase;
                    }
                }
                //восстановить базовый вариант дл€ дублеров i_marsh ?
                for (int i_dub = 0; i_dub < listDubl[i_marsh].Count; i_dub++)
                {
                    if (listDubl[i_marsh][i_dub].„исло“—наћаршруте != listDublCountTC[i_marsh][i_dub])
                    {
                        //nextIsPairedDubl = i_dub < listDubl[i_marsh].Count - 1 ? TestNextPaired(listDubl[i_marsh][i_dub], listDubl[i_marsh][i_dub + 1]) : false;
                        nextIsPairedDubl = listDublPaired[i_marsh][i_dub];
                        masRoute = GenerateReis(listDubl[i_marsh][i_dub].¬местимость1–ейса, listDublInterval[i_marsh][i_dub]);
                        listDubl[i_marsh][i_dub].«адатьћассив–ейсов(masRoute);
                        listDubl[i_marsh][i_dub].„исло“—наћаршруте = listDublCountTC[i_marsh][i_dub];
                        if (nextIsPairedDubl)
                        {
                            listDubl[i_marsh][i_dub + 1].«адатьћассив–ейсов(masRoute);
                            listDubl[i_marsh][i_dub + 1].„исло“—наћаршруте = listDublCountTC[i_marsh][i_dub];
                            i_dub++;
                        }
                    }
                }
                //восстановить генерацию пассажиров на остановках троллейбусного маршрута 
                if(masOst!=null)
                    for (int i = 0; i < masOst.Length; i++)
                        массивќстановок[masOst[i] - 1]. оэф¬ычеркивани€ = 0;
                if (masPaired[i_marsh])//nextIsPaired)
                    i_marsh++;
            }
            //return sb.ToString();
            return listRes;
        }

        private bool TestNextPaired(ћаршрут m1, ћаршрут m2)//проверка, что следующий - пр€мой/обратный маршрут
        {
            if (m1!=null && m2!=null)
            {
                //определение номера маршрута - пр€мой и обратный
                string curCode = m1.Ќазвание.Split(new char[] { ' ' }, 2)[0];
                string nextCode = m2.Ќазвание.Split(new char[] { ' ' }, 2)[0];
                if (nextCode == curCode && m1.“ип“ранспорта == m2.“ип“ранспорта)
                    return true;
            }
            return false;
        }
        public ћаршрут[] GetUniqeTrollMarsh()
        {
            List<ћаршрут> listTrol= (from m in массивћаршрутов where m.“ип“ранспорта == “ип“ранспортаћаршрутаEnum.троллейбус select m).ToList<ћаршрут>();
            for (int i = listTrol.Count - 2; i >= 0; i--)
                if (TestNextPaired(listTrol[i], listTrol[i + 1]))
                {
                    listTrol.RemoveAt(i + 1);
                    i--;
                }
            return listTrol.ToArray();
        }

        private void TempSaveResult(StringBuilder sb, ѕоказатели–аботыƒн€ pkday, int i_marsh, int i_int)//сохранение результатов моделировани€
        {
            sb.AppendLine("  *** –езультаты моделировани€ ***");
            sb.AppendFormat("маршрут = {0}, интервал = {1}\n", i_marsh, i_int);
            sb.AppendLine(string.Format("  (маршрутов={0}, остановок={1}, период моделировани€={2} ч)",  олћаршрутов,  олќстановок, кол„асов–аботы));
            sb.AppendLine("ќбщее число завершенных рейсов = " + pkday.сумм„исло–ейсов.ToString("N0"));
            sb.AppendLine("ќбщее число рейсов в пути = " + pkday.сумм„ислоЌезаверш–ейсов.ToString("N0"));
            sb.AppendLine(" оличество по€вившихс€ пассажиров = " + pkday.сумм олѕо€вившѕассажиров.ToString("N0"));
            sb.AppendLine(" оличество перевезенных пассажиров = " + pkday.сум олѕеревезѕассаж.ToString("N0"));
            sb.AppendLine(" оличество недождавшихс€ пассажиров = " + pkday.—умм олЌедождѕассажиров.ToString("N0"));
            sb.AppendLine(" оличество пассажиров оставшихс€ на остановках = " + pkday.—умм олѕасќставшЌаќст.ToString("N0"));
            sb.AppendLine(" оличество пассажиров оставшихс€ в “— = " + pkday.—умм олѕасќставш¬јвто.ToString("N0"));
            sb.AppendLine(" оличество пересадочных пассажиров = " + pkday.—умм олѕересадочѕассажиров.ToString("N0"));
            sb.AppendLine("—реднее число перевезенных пассажиров в час = " + pkday.ср олѕеревезѕас¬„ас.ToString("N0"));
            sb.AppendLine("—редний коэффициент использовани€ вместимости “— = " + pkday.коэф»спольз¬местим.ToString("N2"));
            sb.AppendLine("—редн€€ дальность поездки в км = " + pkday.срƒальностьѕоездки м.ToString("N2"));
            sb.AppendLine("—редн€€ дальность поездки в остановках = " + pkday.срƒальностьѕоездкиќст.ToString("N2"));
            sb.AppendLine("¬ыручка = " + pkday.¬ыручка.ToString("N0") + " руб");
        }
        private void TempSaveResult2(List<“роллейбусыƒл€ѕротокола> listRes, ћаршрут[] masTrol, bool[] masPaired, int i_marsh, int i_int)//сохранение результатов моделировани€
        {
            “роллейбусыƒл€ѕротокола item=new “роллейбусыƒл€ѕротокола();
            int cnt = masPaired.Count((bool b) => b);
            item.AllTrollKm = new int[cnt];
            item.IntervalsTroll = new int[cnt];
            item.CountTCTroll = new int[cnt];
            item.PassTroll= new int[cnt];
            item.AllTrollPass = 0;
            item.AllTrollProfit = 0;
            for (int i = 0, j=0; i < masTrol.Length; i++)
            {
                ћаршрут марш = массивћаршрутов[masTrol[i]. од - 1];
                if (masPaired[i])
                {
                    item.CountTCTroll[j] = masTrol[i].„исло“—наћаршруте;
                    item.IntervalsTroll[j] = (i != i_marsh ? (int)Math.Round(2.0 * masTrol[i].¬рем€1–ейса / masTrol[i].„исло“—наћаршруте) : i_int);
                    item.AllTrollKm[j] =(int)(марш.ѕоказатели–аботы.кол¬ыполнен–ейсов * 2 * марш.ƒлинаћаршрута);
                    item.PassTroll[j] = марш.ѕоказатели–аботы.сум олѕеревезѕассаж;
                    if (i + 1 < masTrol.Length && !masPaired[i + 1]) 
                        item.PassTroll[j] += массивћаршрутов[masTrol[i+1]. од - 1].ѕоказатели–аботы.сум олѕеревезѕассаж;
                    j++;
                }
                item.AllTrollPass += марш.ѕоказатели–аботы.сум олѕеревезѕассаж;
                item.AllTrollProfit+=марш.ѕоказатели–аботы.¬ыручка;
            }
            listRes.Add(item);
        }
    }
}