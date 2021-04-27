using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Транспорт2017.ГенераторПас;

namespace Транспорт2017
{
    public partial class MainForm : Form
    {
        const string CORRPASFILENAME = "trafic.xlsx";
        const string TRAFTOROUTEFILENAME = "Расклад трафика по маршрутам.xlsx";
        const string MATRCORRFILENAME = "матрица корреспонденций.xlsx";

        private Кординатор coord;
        private WaitForm waitForm;

        private bool LoadDataToCoordinator(out string errMsg)
        {
            errMsg = string.Empty;
            //*** открыть файл modelХХ.xlsm
            FileInfo file = new FileInfo(SettingsModel.FileNameModel);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //st.AppendLine("load=" + (DateTime.Now - dt).TotalMilliseconds.ToString());
                ExcelWorksheet wkSheet = package.Workbook.Worksheets["ИД_Остановки"];
                int КолОстановок = wkSheet.Cells[1, 1].GetValue<int>();

                //*** сформировать матрицу достижимости остановок из файла
                coord.МатрицаКорр = new МатрицаКорреспонденций(КолОстановок);
                if (!FillMatrixCorrespondents(coord.МатрицаКорр, SettingsModel.FileNameCorresp, КолОстановок, out errMsg))
                    return false;

                //*** открыть книгу с трафиком пассажиров
                FileInfo file2 = new FileInfo(SettingsModel.FileNameTrafic);
                ExcelPackage packageTrafic = new ExcelPackage(file2);
                ExcelWorksheet sheetTrafic = packageTrafic.Workbook.Worksheets[1];
                //*** добавление данных по остановкам
                Остановка[] masOsts = new Остановка[КолОстановок];
                int[] masCodeOst = new int[КолОстановок];
                ПассажирДляТрафика[] traficMas = null;
                bool ostPresent;
                int irow, icol, cnt, tmp;
                for (int i = 0; i < КолОстановок; i++)
                {
                    irow = i + 3;
                    //создание новой остановки
                    masOsts[i] = new Остановка(wkSheet.Cells[irow, 1].GetValue<int>(), wkSheet.Cells[irow, 2].Text, 100, coord.МатрицаКорр); //wkSheet.Cells[i+1, 4].GetValue<int>() - число мест для авто неограничено
                    masCodeOst[i] = masOsts[i].Код;

                    //*** создание генератора для очередной остановки
                    //поиск столбцов нужной остановки
                    icol = 5 * i;
                    ostPresent = true;
                    //проверяем "подходящие" столбцы
                    if (sheetTrafic.Cells[3, icol + 1].GetValue<int>() != masCodeOst[i])
                    {
                        //перебираем все столбцы-остановки с первой
                        int codeost;
                        do
                        {
                            codeost = sheetTrafic.Cells[3, icol + 1].GetValue<int>();
                            icol += 5;
                        } while (codeost != 0 && codeost != masCodeOst[i]);
                        ostPresent = (codeost != 0); //остановки нет
                    }
                    if (ostPresent)
                    {
                        // + проверка на наличие пассажиров
                        if (!string.IsNullOrEmpty(sheetTrafic.Cells[5, icol + 1].Text))
                        {
                            cnt = sheetTrafic.Cells[5, icol + 1, sheetTrafic.Dimension.End.Row, icol + 1].Count((c) => !string.IsNullOrEmpty(c.Text));
                            traficMas = new ПассажирДляТрафика[cnt];
                            for (int k = 5; k < cnt + 5; k++)
                            {
                                if (!int.TryParse(sheetTrafic.Cells[k, icol + 3].Text, out tmp))
                                    tmp = masCodeOst[i];
                                //заменить "Нет вариантов" в 3-м столбце на остановку появления
                                traficMas[k - 5] = new ПассажирДляТрафика(tmp, sheetTrafic.Cells[k, icol + 1].GetValue<int>());
                            }
                        }
                    }

                    ГенераторПассажиров генПас = new ГенераторПассажиров(masCodeOst[i]);
                    генПас.ЗадатьМатрицуПоявлПас(traficMas);
                    //связать генератор пассажиров с остановкой
                    masOsts[i].ЗадатьГенераторПас(генПас);
                }
                //отсортировать остановки по коду - на всякий случай, т.к. массив должен быть отсортирован по кодам остановок
                Array.Sort<Остановка>(masOsts, (Остановка a, Остановка b) => a.Код.CompareTo(b.Код));

                coord.ДобавитьОстановки(masOsts);
                //закрыть книгу с трафиком
                sheetTrafic.Dispose();
                //    t2 = Timer: Debug.Print "формирование списка остановок с генераторами " & t2 - t1

                //*** добавление данных по маршрутам
                int колМаршрутов, codeMarsh;
                wkSheet = package.Workbook.Worksheets["ИД_Маршруты"];
                колМаршрутов = wkSheet.Cells[1, 1].GetValue<int>();
                int колВсехРейсов = wkSheet.Cells["K1"].GetValue<int>();
                Маршрут[] masMarsh = new Маршрут[колМаршрутов];
                ExcelWorksheet tripSheet = package.Workbook.Worksheets["для рейсов"];

                //*** загрузить и обработать все маршруты (по остановкам) "N3"
                cnt = wkSheet.Cells[3, 14, wkSheet.Dimension.End.Row, 14].Count((c) => !string.IsNullOrEmpty(c.Text));
                Dictionary<int, List<Перегон>> dictPer = new Dictionary<int, List<Перегон>>(колМаршрутов);
                for (int i = 0; i < cnt; i++)
                {
                    Перегон p = new Перегон
                    {
                        codeOst = wkSheet.Cells[i + 3, 17].GetValue<int>(),//"кодОстановки"
                        number = wkSheet.Cells[i + 3, 16].GetValue<int>()//"номер Остановки"
                    };
                    if (!double.TryParse(wkSheet.Cells[i + 3, 19].Text, out p.length))//"длинаПерегона"
                        p.length = 1;
                    else
                    {
                        p.timeInt = (int)Math.Ceiling(p.length * 60 / Кординатор.СРСкоростьДвижения); //"времяДоСледОстановки"
                        if (p.timeInt < 1)
                            p.timeInt = 1;
                    }
                    codeMarsh = wkSheet.Cells[i + 3, 15].GetValue<int>();
                    if (!dictPer.ContainsKey(codeMarsh))
                        dictPer.Add(codeMarsh, new List<Перегон>());
                    dictPer[codeMarsh].Add(p);
                }
                //отсортировать перегоны маршрутов
                foreach (KeyValuePair<int, List<Перегон>> kvp in dictPer)
                    kvp.Value.Sort();

                //*** загрузить и обработать все рейсы маршрутов "H3"
                Dictionary<int, List<Рейс>> dictTrip = new Dictionary<int, List<Рейс>>(колМаршрутов);
                for (int i = 0; i < колВсехРейсов; i++)
                {
                    codeMarsh = wkSheet.Cells[i + 3, 8].GetValue<int>();
                    if (!dictTrip.ContainsKey(codeMarsh))
                        dictTrip.Add(codeMarsh, new List<Рейс>());
                    dictTrip[codeMarsh].Add(new Рейс
                    {
                        времяОтправления = wkSheet.Cells[i + 3, 9].GetValue<int>(),//"время"
                        МаксВместимость = wkSheet.Cells[i + 3, 10].GetValue<int>() //"вместимость"
                    });
                }
                //отсортировать рейсы маршрутов
                foreach (KeyValuePair<int, List<Рейс>> kvp in dictTrip)
                    kvp.Value.Sort();

                string nameMarsh;
                ТипТранспортаМаршрутаEnum typeTC;
                int priceMarsh;
                for (int i = 0; i < колМаршрутов; i++)
                {
                    codeMarsh = wkSheet.Cells[i + 3, 1].GetValue<int>();//Range("A" & (i + 3)).Value
                    nameMarsh = wkSheet.Cells[i + 3, 2].Text;//Range("B" & (i + 3)).Value
                    typeTC = ConvertТипТранспортаМаршрута(wkSheet.Cells[i + 3, 3].Text);//Range("C" & (i + 3)).Value)
                    priceMarsh = wkSheet.Cells[i + 3, 4].GetValue<int>();//Range("D" & (i + 3)).Value
                    List<Перегон> listPer = null;
                    dictPer.TryGetValue(codeMarsh, out listPer);
                    List<Рейс> listReis = null;
                    dictTrip.TryGetValue(codeMarsh, out listReis);
                    masMarsh[i] = new Маршрут(codeMarsh, nameMarsh, typeTC, priceMarsh, listPer?.ToArray(), listReis?.ToArray())
                    {
                        ЧислоТСнаМаршруте = tripSheet.Cells[i + 5, 3].GetValue<int>(),
                        Время1Рейса = tripSheet.Cells[i + 5, 2].GetValue<int>() / 2
                    };
                }

                //отсортировать маршруты по коду - на всякий случай, т.к. массив должен быть отсортирован по кодам маршрутов
                Array.Sort<Маршрут>(masMarsh, (Маршрут a, Маршрут b) => a.Код.CompareTo(b.Код));
                coord.ДобавитьМаршруты(masMarsh);
            }
            //    t2 = Timer: Debug.Print "формирование списка маршрутов и рейсов " & t2 - t1
            return true;
        }
        private ТипТранспортаМаршрутаEnum ConvertТипТранспортаМаршрута(string ttm)
        {
            if (ttm == "а")
                return ТипТранспортаМаршрутаEnum.автобус;
            else if (ttm == "т")
                return ТипТранспортаМаршрутаEnum.троллейбус;
            else if (ttm == "тв")
                return ТипТранспортаМаршрутаEnum.трамвай;
            else if (ttm == "м")
                return ТипТранспортаМаршрутаEnum.маршрутка;
            return ТипТранспортаМаршрутаEnum.маршрутка;
        }
        bool FillMatrixCorrespondents(МатрицаКорреспонденций МатрицаКорреспонденций, string fileNameCorresp, int countOst, out string errMsg)
        {
            errMsg = string.Empty;
            FileInfo file = new FileInfo(fileNameCorresp);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                try
                {
                    ExcelWorksheet wkSheet = package.Workbook.Worksheets[1];
                    //проверка на число остановок
                    int cnt = wkSheet.Cells[1, 1, 1, wkSheet.Dimension.End.Column].Count(c => !string.IsNullOrEmpty(c.Text));
                    if (cnt < countOst)
                    {
                        errMsg = "Число остановок в файле " + fileNameCorresp + " меньше чем в файле модели!";
                        //DialogResult res = MessageBox.Show("Число остановок в файле " + fileNameCorresp + " меньше чем в файле модели!\n" +
                        //   "Продолжить работу с имеющимися достижимостями остановок!", "Проверка",
                        //   MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
                        //if (res == DialogResult.No)
                        return false;
                    }

                    //обработка матрицы
                    string myStr;
                    Regex myRegExp1 = new Regex(@"\[(\d+), (\d+); (\d+)\]\s+\((\d+,?\d*)\),"); //остановки с пересадками: [38, 6; 21] (3,577), [28, 6; 21]  (3,577),
                    Regex myRegExp2 = new Regex(@"(\d+) \((\d+,?\d*)\),"); //остановки без пересадок: 28 (0,885), 30 (1,064), 6 (1,064),
                    MatchCollection objMatches;
                    ПутьКорреспонденции новПуть;
                    for (int i = 3; i < cnt + 3; i++)
                    {
                        for (int j = 3; j < cnt + 3; j++)
                        {
                            //разобрать очередную ячейку
                            myStr = wkSheet.Cells[i, j].Text;
                            if (!string.IsNullOrEmpty(myStr))
                            {
                                int ostFrom = int.Parse(wkSheet.Cells[i, 1].Text);
                                int ostTo = int.Parse(wkSheet.Cells[1, j].Text);
                                //если первый символ "[" - остановки с пересадками: [38, 6; 21]  (3,577), [28, 6; 21]  (3,577),
                                if (myStr[0] == '[')
                                {
                                    objMatches = myRegExp1.Matches(myStr);
                                    for (int k = 0; k < objMatches.Count; k++)
                                    {
                                        Match myMatchs = objMatches[k];
                                        новПуть = new ПутьКорреспонденции
                                        {
                                            КодМаршрута = int.Parse(myMatchs.Groups[1].Value),
                                            КодМаршрутаПослеПересадки = int.Parse(myMatchs.Groups[2].Value),
                                            КодОстановкиПересадки = int.Parse(myMatchs.Groups[3].Value),
                                            Критерий = double.Parse(myMatchs.Groups[4].Value),
                                            ТипПутиКорресп = ТипПутиКорреспонденцииEnum.сПересадкой
                                        };
                                        //МатрицаКорреспонденций.ДобавитьПуть(i - 2, j - 2, новПуть);
                                        МатрицаКорреспонденций.ДобавитьПуть(ostFrom, ostTo, новПуть);
                                    }
                                }
                                else
                                {
                                    //остановки без пересадок: 28 (0,885), 30 (1,064), 6 (1,064),
                                    objMatches = myRegExp2.Matches(myStr);
                                    for (int k = 0; k < objMatches.Count; k++)
                                    {
                                        Match myMatchs = objMatches[k];
                                        новПуть = new ПутьКорреспонденции
                                        {
                                            КодМаршрута = int.Parse(myMatchs.Groups[1].Value),
                                            Критерий = double.Parse(myMatchs.Groups[2].Value),
                                            ТипПутиКорресп = ТипПутиКорреспонденцииEnum.безПересадки
                                        };
                                        //МатрицаКорреспонденций.ДобавитьПуть(i - 2, j - 2, новПуть);
                                        МатрицаКорреспонденций.ДобавитьПуть(ostFrom, ostTo, новПуть);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    errMsg = "Ошибка заполнения матрицы достижимости.\n" + exc.Message + "\nПодготовка данных прекращена!";
                    //MessageBox.Show("Ошибка заполнения матрицы достижимости.\n" + exc.Message +
                    //    "\nПодготовка данных прекращена!", "Ошибка разбора трафика", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            return true;
        }
        void ShowResultDay(ПоказателиРаботыДня pkday)
        {
            listBox1.Items.Clear();
            listBox1.Items.Add("  *** Результаты моделирования ***");
            listBox1.Items.Add(string.Format("  (маршрутов={0}, остановок={1}, период моделирования={2} ч)", coord.КолМаршрутов, coord.КолОстановок, coord.колЧасовРаботы));
            listBox1.Items.Add("Общее число завершенных рейсов = " + pkday.суммЧислоРейсов.ToString("N0"));
            listBox1.Items.Add("Общее число рейсов в пути = " + pkday.суммЧислоНезавершРейсов.ToString("N0"));
            listBox1.Items.Add("Количество появившихся пассажиров = " + pkday.суммКолПоявившПассажиров.ToString("N0"));
            listBox1.Items.Add("Количество перевезенных пассажиров = " + pkday.сумКолПеревезПассаж.ToString("N0"));
            listBox1.Items.Add("Количество недождавшихся пассажиров = " + pkday.СуммКолНедождПассажиров.ToString("N0"));
            listBox1.Items.Add("Количество пассажиров оставшихся на остановках = " + pkday.СуммКолПасОставшНаОст.ToString("N0"));
            listBox1.Items.Add("Количество пассажиров оставшихся в ТС = " + pkday.СуммКолПасОставшВАвто.ToString("N0"));
            listBox1.Items.Add("Количество пересадочных пассажиров = " + pkday.СуммКолПересадочПассажиров.ToString("N0"));
            listBox1.Items.Add("Среднее число перевезенных пассажиров в час = " + pkday.срКолПеревезПасВЧас.ToString("N0"));
            listBox1.Items.Add("Средний коэффициент использования вместимости ТС = " + pkday.коэфИспользВместим.ToString("N2"));
            listBox1.Items.Add("Средняя дальность поездки в км = " + pkday.срДальностьПоездкиКм.ToString("N2"));
            listBox1.Items.Add("Средняя дальность поездки в остановках = " + pkday.срДальностьПоездкиОст.ToString("N2"));
            listBox1.Items.Add("Выручка = " + pkday.Выручка.ToString("N0") + " руб");
        }
        void ShowResultDay(string strResult)
        {
            listBox1.Items.Clear();
            StringReader sr = new StringReader(strResult);
            string str;
            while(!string.IsNullOrEmpty(str=sr.ReadLine()))
                listBox1.Items.Add(str);            
        }
        private bool ProcessPassangersGeneration(string fileName, out string errMsg)//обработка файла с генерациями пассажиров
        {
            errMsg = string.Empty;
            DateTime dt = DateTime.Now;

            //*** открыть файл modelХХ.xlsm
            FileInfo file = new FileInfo(fileName);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Общий_трафик"];
                if (sheet == null)
                    sheet = package.Workbook.Worksheets[1];
                int colPeriod = 8;
                int count = sheet.Cells[2, 1, 2, sheet.Dimension.End.Column].Count((c) => !string.IsNullOrEmpty(c.Text)) / colPeriod;

                int codeost;
                //*** создать файл для обработанного трафика пассажиров
                FileStream file2 = new FileStream(Application.StartupPath + "\\" + CORRPASFILENAME, FileMode.Create);//c пересозданием файла
                ExcelPackage packageTrafic = new ExcelPackage(file2);
                ExcelWorksheet shTarget = packageTrafic.Workbook.Worksheets.Add("trafic");
                //цикл по столбцам-остановкам обрабатываемой книги

                StringBuilder sb = new StringBuilder();//---del

                for (int i = 0; i < count; i++)
                {
                    codeost = sheet.Cells[3, 1 + colPeriod * i].GetValue<int>();
                    shTarget.Cells[3, 5 * i + 1].Value = codeost;//записать код остановки в 3-ю строку
                    //записать заголовки столбцов
                    shTarget.Cells[4, 5 * i + 1].Value = "Время";
                    shTarget.Cells[4, 5 * i + 2].Value = "Р-н прибытия";
                    shTarget.Cells[4, 5 * i + 3].Value = "Код ост прибытия";
                    //проверка на пустой диапазон для заданного часа
                    if (!string.IsNullOrEmpty(sheet.Cells[3, 4 + colPeriod * i].Text))
                    {
                        int maxK = 2 + sheet.Cells[3, 4 + colPeriod * i, sheet.Dimension.End.Row, 4 + colPeriod * i].Count((c) => !string.IsNullOrEmpty(c.Text));
                        double tt2, tt1;
                        //первый пассажир
                        tt1 = sheet.Cells[3, 4 + colPeriod * i].GetValue<DateTime>().TimeOfDay.TotalDays;
                        shTarget.Cells[5, 5 * i + 1].Value = (int)Math.Round(tt1 * 1440 - SettingsModel.НачЧасДляТрафика * 60);
                        shTarget.Cells[5, 5 * i + 2].Value = sheet.Cells[3, 5 + colPeriod * i].Value;
                        shTarget.Cells[5, 5 * i + 3].Value = sheet.Cells[3, 6 + colPeriod * i].Value;
                        for (int j = 4; j <= maxK; j++)
                        {
                            tt2 = sheet.Cells[j, 4 + colPeriod * i].GetValue<DateTime>().TimeOfDay.TotalDays;
                            if (tt2 < tt1)
                            {
                                sb.AppendFormat("код остановки={0}, строка={1}\n", codeost, j);
                                break;
                            }
                            //пересчитать время в такты - цикл по строкам отдельного часа
                            shTarget.Cells[2 + j, 5 * i + 1].Value = (int)Math.Round(tt2 * 1440 - SettingsModel.НачЧасДляТрафика * 60);
                            shTarget.Cells[2 + j, 5 * i + 2].Value = sheet.Cells[j, 5 + colPeriod * i].Value;
                            shTarget.Cells[2 + j, 5 * i + 3].Value = sheet.Cells[j, 6 + colPeriod * i].Value;
                            tt1 = tt2;
                        }
                    }
                }
                packageTrafic.Save();
                file2.Close();
                errMsg = "Обработка файла с пассажирами произведена успешно (" + (DateTime.Now - dt).ToString() +
                    ")\n Сохранено в файл \"" + CORRPASFILENAME + "\"";
            }
            return true;
        }
        private bool РасчетВсехМаршрутов(string fileNamePas, string fileNameModel, out string errMsg)
        {
            errMsg = string.Empty;
            DateTime dt = DateTime.Now;

            //*** 1) сформировать маршруты
            Маршрут[] masMarsh = LoadMarshrut(fileNameModel, allMarsh: true);
            int countMarsh = masMarsh.Length;

            //*** 2) занести всех пассажиров в словарь
            FileInfo file = new FileInfo(fileNamePas);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                Dictionary<string, ПассажирыПоМаршрутам> dict = new Dictionary<string, ПассажирыПоМаршрутам>();
                ExcelWorksheet sheetTrafic = package.Workbook.Worksheets["пас"];
                if (sheetTrafic == null)
                    sheetTrafic = package.Workbook.Worksheets[1];
                int ostIn, ostOut, codeMarsh;
                string keyStr;
                ПассажирыПоМаршрутам ptr;
                int col=0;
                //цикл по колонкам (миллионам) пассажиров
                while(sheetTrafic.Cells[3, col*10+2].GetValue<int>()!=0)
                {
                    for (int i = 3; true; i++)
                    {
                        ostIn = sheetTrafic.Cells[i, col*10+2].GetValue<int>();
                        if (ostIn == 0)
                            break;
                        ostOut = sheetTrafic.Cells[i, col * 10 + 3].GetValue<int>();
                        codeMarsh = sheetTrafic.Cells[i, col * 10 + 8].GetValue<int>();
                        keyStr = ostIn + "_" + ostOut;
                        if (!dict.ContainsKey(keyStr))
                        {
                            //проверка на обратный перегон
                            keyStr = ostOut + "_" + ostIn;
                            if (!dict.ContainsKey(keyStr))
                                //новый перегон
                                dict[keyStr] = new ПассажирыПоМаршрутам();
                        }
                        ptr = dict[keyStr];
                        ptr.sumPas++;
                        if (codeMarsh != 0)
                            if (ptr.dict.ContainsKey(codeMarsh))
                                ptr.dict[codeMarsh]++;
                            else
                                ptr.dict.Add(codeMarsh, 1);
                    }
                    //переход к другой колонке пассажиров
                    col++;
                }

                //*** 3) сформировать таблички на отдельные маршруты с сохранением их на лист
                FileStream fileTarget = new FileStream(TRAFTOROUTEFILENAME, FileMode.Create);
                using (ExcelPackage packageTarget = new ExcelPackage(fileTarget))
                {
                    ExcelWorksheet shTarget = packageTarget.Workbook.Worksheets.Add("расклад");
                    ПассажирыПоМаршрутам ptrRes;
                    string numMarsh = "", prevNumMarsh; //предыдущий номер маршрута, чтобы не выводить отдельно прямой и обратный
                    int icol = 1, irow;
                    for (int i = 0; i < countMarsh; i++)
                    {
                        Маршрут marsh = masMarsh[i];
                        //проверка на тот же маршрут (прямой - обратный) !коды должны идти рядом
                        prevNumMarsh = numMarsh;
                        numMarsh = marsh.Название.Split(new char[] { ' ' }, 2)[0];
                        if (numMarsh != prevNumMarsh)
                        {
                            int[] masOst = marsh.КодыОстановок;
                            int iOst, jOst;
                            //формирование таблички
                            ptrRes = new ПассажирыПоМаршрутам();
                            for (iOst = 0; iOst < masOst.Length - 1; iOst++)
                            {
                                ostIn = masOst[iOst];
                                for (jOst = iOst + 1; jOst < masOst.Length; jOst++)
                                {
                                    ostOut = masOst[jOst];
                                    keyStr = ostIn + "_" + ostOut;
                                    if (!dict.ContainsKey(keyStr))
                                    {
                                        //проверка на обратный перегон
                                        keyStr = ostOut + "_" + ostIn;
                                        if (!dict.ContainsKey(keyStr))
                                            //перегона нет - перейти к следующему
                                            continue;
                                    }
                                    ptr = dict[keyStr];
                                    foreach (KeyValuePair<int, int> kvp in ptr.dict)
                                    {
                                        if (ptrRes.dict.ContainsKey(kvp.Key))
                                            ptrRes.dict[kvp.Key] += kvp.Value;
                                        else
                                            ptrRes.dict.Add(kvp.Key, kvp.Value);
                                    }
                                    ptrRes.sumPas += ptr.sumPas;
                                }
                            }

                            //вывод таблички
                            shTarget.Cells[2, icol].Value = "коды маршрутов";
                            shTarget.Cells[2, icol + 1].Value = marsh.Код;
                            shTarget.Cells[2, icol + 2].Value = marsh.СокращениеТипаТС;
                            shTarget.Cells[3, icol + 1].Value = masMarsh[i].Название;
                            shTarget.Cells[4, icol].Value = "кол желающих";
                            shTarget.Cells[4, icol + 1].Value = ptrRes.sumPas;
                            shTarget.Cells[5, icol].Value = "кол перевезенных";
                            //shTarget.Cells[5, icol + 1].Formula = string.Format("Sum({0}:{1})", shTarget.Cells[7, icol + 2].Address, shTarget.Cells[6 + ptrRes.dict.Count, icol + 2].Address);
                            shTarget.Cells[6, icol].Value = "код маршрута";
                            shTarget.Cells[6, icol + 1].Value = "название маршрута";
                            shTarget.Cells[6, icol + 2].Value = "кол перевезенных";
                            shTarget.Cells[6, icol + 3].Value = "% маршрута от перевезенных";
                            irow = 7;
                            Compress(ptrRes.dict, masMarsh);
                            int[] masSortKey = SortKey(ptrRes.dict);
                            int key;
                            string nom;
                            for (int ind = 0; ind < ptrRes.dict.Count; ind++)
                            {
                                key = masSortKey[ind];
                                shTarget.Cells[irow, icol].Value = key;
                                nom = masMarsh[key - 1].Название;
                                nom = nom.Substring(0, nom.LastIndexOf('('));
                                shTarget.Cells[irow, icol + 1].Value = nom;// masMarsh[key - 1].Название;
                                shTarget.Cells[irow, icol + 2].Value = ptrRes.dict[key];
                                irow++;
                            }
                            shTarget.Cells[5, icol + 1].Formula = string.Format("Sum({0}:{1})", shTarget.Cells[7, icol + 2].Address, shTarget.Cells[6 + ptrRes.dict.Count, icol + 2].Address);
                            shTarget.Cells[7, icol + 3, ptrRes.dict.Count + 6, icol + 3].FormulaR1C1 = "=RC[-1]/R5C[-2]";
                            shTarget.Cells[7, icol + 3, ptrRes.dict.Count + 6, icol + 3].Style.Numberformat.Format = "0.0%";
                            shTarget.Cells[7, icol, ptrRes.dict.Count + 6, icol].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            icol += 5;
                        }
                        else
                            shTarget.Cells[2, icol - 4].Value += ", " + marsh.Код;
                    }
                    packageTarget.Save();
                }
                fileTarget.Close();
                errMsg = "Обработка расклада трафика по маршрутом произведено успешно (" + (DateTime.Now - dt).ToString() +
                    ")\n Сохранено в файл \"" + TRAFTOROUTEFILENAME + "\"";
            }
            return true;
        }

        private void Compress(Dictionary<int, int> dict, Маршрут[] masMarsh)
        {
            int key;
            string numMarsh, numMarsh2;
            for (int i = 0; i < dict.Count; i++)
            {
                key = dict.ElementAt(i).Key;
                Маршрут marsh = masMarsh[key-1];
                //проверка на тот же маршрут (прямой - обратный) !коды должны идти рядом
                numMarsh = marsh.Название.Split(new char[] { ' ' }, 2)[0];
                numMarsh2="";
                if(key>1)
                    numMarsh2 = masMarsh[key-2].Название.Split(new char[] { ' ' }, 2)[0];
                if (numMarsh == numMarsh2 && dict.ContainsKey(key-1))
                {
                    //оставляем запись с меньшим кодом маршрута
                    dict[key-1]+=dict[key]; 
                    dict.Remove(key);
                    i--;
                    continue;
                }
                if (key < masMarsh.Length)
                    numMarsh2 = masMarsh[key ].Название.Split(new char[] { ' ' }, 2)[0];
                if (numMarsh == numMarsh2 && dict.ContainsKey(key + 1))
                {
                    dict[key] += dict[key + 1];
                    dict.Remove(key + 1);
                }
            }
            
        }
        //сортировка массив ключей по убыванию кол перевезенных пас (максимальным элементом)
        private int[] SortKey(Dictionary<int, int> dict)
        {
            int max, indmax, tmp;
            int[] res = new int[dict.Count];
            for (int i = 0; i < dict.Count; i++)
                res[i] = dict.ElementAt(i).Key;
            for (int i = 0; i < dict.Count - 1; i++)
            {
                max = dict[res[i]];
                indmax = i;
                for (int j = i + 1; j < dict.Count; j++)
                    if (dict[res[j]] > max)
                    {
                        max = dict[res[j]];
                        indmax = j;
                    }
                if (i != indmax)
                {
                    tmp = res[i];
                    res[i] = res[indmax];
                    res[indmax] = tmp;
                }
            }
            return res;
        }
        private МатрицаКорреспонденций InitMatrixCorrespondents(out string[] masNameOst)
        {
            FileInfo file = new FileInfo(SettingsModel.FileNameModel);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["ИД_Остановки"];
                int КолОстановок = sheet.Cells["A1"].GetValue<int>();
                masNameOst = new string[КолОстановок];
                for (int i = 0; i < КолОстановок; i++)
                    masNameOst[i] = sheet.Cells[i + 3, 2].Value.ToString();
                return new МатрицаКорреспонденций(КолОстановок);
            }
        }
        private bool MakeMatrixCorrespondents(ref МатрицаКорреспонденций МатрицаКорр, bool ПоВсемМаршрутам, string[] masNameOst, out string errMsg)
        {
            DateTime dt = DateTime.Now;
            //загрузка маршрутов
            Маршрут[] masMarsh = LoadMarshrut(SettingsModel.FileNameModel, ПоВсемМаршрутам);
            //загрузка маршрутов в матрицу
            МатрицаКорр.Инициализация(masMarsh);
            //формирование марицы достижимости
            МатрицаКорр.УчетПересадок(masMarsh);
            //отсортировать пути по возрастинию критерия
            МатрицаКорр.СортироватьПути();
            //записать результаты
            SaveMC(МатрицаКорр, masNameOst);
            errMsg = "Расчет матрицы достижимостей закончен (" + (DateTime.Now - dt).ToString() +
                ")\n Сохранено в файл \"" + MATRCORRFILENAME + "\"";
            return true;
        }
        //загрузка маршрутов, НО БЕЗ рейсов , ТОЛЬКО коды остановок и перегоны: allMarsh = True - ВСЕ маршруты, = False - ТОЛЬКО тех маршрутов, по которым есть рейсы
        private Маршрут[] LoadMarshrut(string fileNameModel, bool allMarsh)
        {
            FileInfo file = new FileInfo(fileNameModel);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //*** добавление данных по маршрутам
                int колМаршрутов, codeMarsh;
                ExcelWorksheet wkSheet = package.Workbook.Worksheets["ИД_Маршруты"];
                int колВсехМаршрутов = wkSheet.Cells["A1"].GetValue<int>();
                int колВсехРейсов = wkSheet.Cells["K1"].GetValue<int>();
                Dictionary<int, int> dictMarsh = null;
                if (allMarsh)
                    колМаршрутов = колВсехМаршрутов;
                else
                {
                    dictMarsh = new Dictionary<int, int>();
                    // получение списка кодов маршрутов из таблицы рейсов
                    for (int i = 3; i < колВсехРейсов + 3; i++)
                    {
                        codeMarsh = wkSheet.Cells[i, 8].GetValue<int>();//H3:Hxxx
                        dictMarsh[codeMarsh] = codeMarsh;
                    }
                    колМаршрутов = dictMarsh.Count;
                }

                Маршрут[] masMarsh = new Маршрут[колМаршрутов];

                //*** загрузить и обработать все маршруты (по остановкам) "N3"
                int cnt = wkSheet.Cells[3, 14, wkSheet.Dimension.End.Row, 14].Count((c) => !string.IsNullOrEmpty(c.Text));
                Dictionary<int, List<Перегон>> dictPer = new Dictionary<int, List<Перегон>>(колМаршрутов);
                for (int i = 0; i < cnt; i++)
                {
                    Перегон p = new Перегон
                    {
                        codeOst = wkSheet.Cells[i + 3, 17].GetValue<int>(),//"кодОстановки"
                        number = wkSheet.Cells[i + 3, 16].GetValue<int>()//"номер Остановки"
                    };
                    if (!double.TryParse(wkSheet.Cells[i + 3, 19].Text, out p.length))//"длинаПерегона"
                        p.length = 1;
                    else
                    {
                        p.timeInt = (int)Math.Ceiling(p.length * 60 / Кординатор.СРСкоростьДвижения); //"времяДоСледОстановки"
                        if (p.timeInt < 1)
                            p.timeInt = 1;
                    }
                    codeMarsh = wkSheet.Cells[i + 3, 15].GetValue<int>();
                    if (!dictPer.ContainsKey(codeMarsh))
                        dictPer.Add(codeMarsh, new List<Перегон>());
                    dictPer[codeMarsh].Add(p);
                }
                //отсортировать перегоны маршрутов
                foreach (KeyValuePair<int, List<Перегон>> kvp in dictPer)
                    kvp.Value.Sort();

                string nameMarsh;
                ТипТранспортаМаршрутаEnum typeTC;
                int priceMarsh;
                for (int i = 0, k = 0; i < колВсехМаршрутов; i++)
                {
                    codeMarsh = wkSheet.Cells[i + 3, 1].GetValue<int>();//Range("A" & (i + 3)).Value
                    if (allMarsh || dictMarsh.ContainsKey(codeMarsh))
                    {
                        nameMarsh = wkSheet.Cells[i + 3, 2].Text;//Range("B" & (i + 3)).Value
                        typeTC = ConvertТипТранспортаМаршрута(wkSheet.Cells[i + 3, 3].Text);//Range("C" & (i + 3)).Value)
                        priceMarsh = wkSheet.Cells[i + 3, 4].GetValue<int>();//Range("D" & (i + 3)).Value
                        List<Перегон> listPer = null;
                        dictPer.TryGetValue(codeMarsh, out listPer);
                        masMarsh[k++] = new Маршрут(codeMarsh, nameMarsh, typeTC, priceMarsh, listPer?.ToArray(), null);
                    }
                }

                //отсортировать маршруты по коду - на всякий случай, т.к. массив должен быть отсортирован по кодам маршрутов
                Array.Sort<Маршрут>(masMarsh, (Маршрут a, Маршрут b) => a.Код.CompareTo(b.Код));
                return masMarsh;
            }
        }
        private void SaveMC(МатрицаКорреспонденций МатрицаКорр, string[] masNameOst)
        {
            FileStream fileTarget = new FileStream(MATRCORRFILENAME, FileMode.Create);
            using (ExcelPackage packageTarget = new ExcelPackage(fileTarget))
            {
                ExcelWorksheet wrksheet = packageTarget.Workbook.Worksheets.Add("матрица");
                int countOst = МатрицаКорр.Размерность;
                for (int i = 0; i < countOst; i++)
                {
                    //сформировать шапки таблицы
                    wrksheet.Cells[i + 3, 1].Value = i + 1;
                    wrksheet.Cells[1, i + 3].Value = i + 1;
                    wrksheet.Cells[i + 3, 2].Value = masNameOst[i];
                    wrksheet.Cells[2, i + 3].Value = masNameOst[i];
                    for (int j = 0; j < countOst; j++)
                        wrksheet.Cells[i + 3, j + 3].Value = МатрицаКорр.СписокМаршрутов(i, j);
                }
                packageTarget.Save();
            }
            fileTarget.Close();
        }


        //
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            coord = new Кординатор();
            //progressForm = new ProgressForm();
            waitForm = new WaitForm();
            solve_toolStripButton.Enabled = false;
            report_toolStripButton.Enabled = false;
            openRes_button.Enabled = false;
        }
        private void Load_toolStripButton_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            string errMsg;
            if (!LoadDataToCoordinator(out errMsg))
                MessageBox.Show(errMsg, "Ошибка разбора трафика", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            coord.колЧасовРаботы = SettingsModel.КолЧасовМоделирования;
            Остановка.МАКСВремяОжиданияПасДоУхода = SettingsModel.МаксВремяОжидания;
            Остановка.ВЕРОЯТНОСТЬПродолженияПоездки = SettingsModel.ВероятностьПродолженияПоездки;

            solve_toolStripButton.Enabled = true;
            MessageBox.Show("База параметров модели загружена успешно (" + (DateTime.Now - dt).ToString() + ")");
        }
        private void Solve_toolStripButton_Click(object sender, EventArgs e)
        {
            coord.колЧасовРаботы = SettingsModel.КолЧасовМоделирования;
            //model_DoWork(null, null);
            //return;

            waitForm.IsNumericProgressBar = true;
            waitForm.SetMaxValue(coord.колЧасовРаботы * 60);
            waitForm.Text = "Идет процесс расчета модели...";
            waitForm.backgroundWorker1.DoWork += Model_DoWork;
            waitForm.backgroundWorker1.RunWorkerCompleted += Model_RunWorkerCompleted;
            waitForm.backgroundWorker1.RunWorkerAsync(this);
            waitForm.Show(this);
        }
        private void Model_DoWork(object sender, DoWorkEventArgs e)
        {
            coord.ТактОтработан += Coord_ТактОтработан;
            //моделирование()
            coord.Моделирование1Дня();
            coord.ТактОтработан -= Coord_ТактОтработан;
            //BeginInvoke(new Action<string>(waitForm.SetTextLabel), "Идет сохранение показателей ТС в файл ...");
        }
        private void Coord_ТактОтработан(object sender, IntEventArg e)
        {
            waitForm.backgroundWorker1.ReportProgress(e.value);
        }
        private void Model_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                //разбор результатов
                //bgw.ReportProgress(coord.колЧасовРаботы * 60 - 1, "Вывод результатов моделирования...")
                ////вывод результатов моделирования
                //showResultPassangers()
                ////progForm.Label1.Text = "Результаты по автобусам..."
                //bgw.ReportProgress(coord.колЧасовРаботы * 60 - 1, "Вывод результатов по автобусам...")
                //showResultAutos()
                ////progForm.Label1.Text = "Результаты по маршрутам..."
                //bgw.ReportProgress(coord.колЧасовРаботы * 60 - 1, "Вывод результатов по маршрутам...")
                //showResultMarsh()
                ////progForm.Label1.Text = "Результаты по остановкам..."
                //bgw.ReportProgress(coord.колЧасовРаботы * 60 - 1, "Вывод результатов по остановкам...")
                //showResultOst()

                ShowResultDay(coord.показателиРаботы);
                report_toolStripButton.Enabled = true;
            }
            else if (e.Error != null)
                MessageBox.Show(e.Error.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //отписаться от событий 
            waitForm.backgroundWorker1.DoWork -= Model_DoWork;
            waitForm.backgroundWorker1.RunWorkerCompleted -= Model_RunWorkerCompleted;
            waitForm.Hide();
            MessageBox.Show("ok");
        }
        private void Report_toolStripButton_Click(object sender, EventArgs e)
        {
            string errMsg;
            if (!coord.ЗаписьОтчетаВExcel(out errMsg))
                MessageBox.Show(errMsg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //            //progForm.Label1.Text = "Идет подготовка к выводу результатов..."
            ////progForm.Label1.Text = "Результаты по пассажирам..."
            ////вывод результатов моделирования
            //showResultPassangers()
            ////progForm.Label1.Text = "Результаты по автобусам..."
            //showResultAutos()
            ////progForm.Label1.Text = "Результаты по маршрутам..."
            //showResultMarsh()
            ////progForm.Label1.Text = "Результаты по остановкам..."
            //showResultOst()
            //UseWaitCursor = False
            //progForm.Hide()
            //ОтчетВExcelToolStripMenuItem.Enabled = true
            //MessageBox.Show("Модель расчитана")
        }
        private void Settings_toolStripButton_Click(object sender, EventArgs e)
        {
            SettingsForm form = new SettingsForm();
            form.ShowDialog(this);
        }
        ///
        private void AllSolve_toolStripButton_Click(object sender, EventArgs e)
        {
            waitForm.Text = "Расчет ТС";
            waitForm.IsNumericProgressBar = false;
            waitForm.backgroundWorker1.DoWork += AllSolve_DoWork;
            waitForm.backgroundWorker1.RunWorkerCompleted += AllSolve_Completed;
            waitForm.backgroundWorker1.RunWorkerAsync(new object[] { this, 0 });
            waitForm.Show(this);
        }
        private void AllSolve_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] obj = e.Argument as object[];
            MainForm form = obj[0] as MainForm;
            int code = (int)obj[1];
            string errMsg = "";
            switch (code)
            {
                case 0: //пустой запуск
                    break;
                case 1: //загрузка данных из файлов Excel
                    LoadDataToCoordinator(out errMsg);
                    coord.колЧасовРаботы = SettingsModel.КолЧасовМоделирования;
                    Остановка.МАКСВремяОжиданияПасДоУхода = SettingsModel.МаксВремяОжидания;
                    Остановка.ВЕРОЯТНОСТЬПродолженияПоездки = SettingsModel.ВероятностьПродолженияПоездки;
                    break;
                case 2: //начать расчет модели
                    coord.ТактОтработан += Coord_ТактОтработан;
                    coord.Моделирование1Дня();
                    coord.ТактОтработан -= Coord_ТактОтработан;
                    break;
                case 3: //формирование отчета с результатами
                    coord.ЗаписьОтчетаВExcel(out errMsg);
                    break;
            }
            e.Result = new object[] { code, errMsg };
        }
        private void AllSolve_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
                MessageBox.Show(e.Error.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            object[] obj = e.Result as object[];
            int code = (int)obj[0];
            string errMsg = (string)obj[1];
            if (errMsg != "")
                MessageBox.Show(errMsg, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

            switch (code)
            {
                case 0: //закончилась инициализация 
                    waitForm.SetTextLabel("Загрузка данных из файлов Excel ...");
                    waitForm.backgroundWorker1.RunWorkerAsync(new object[] { this, 1 });
                    break;
                case 1: //начать расчет модели
                    waitForm.IsNumericProgressBar = true;
                    waitForm.SetMaxValue(coord.колЧасовРаботы * 60);
                    waitForm.SetTextLabel("Расчет модели ТС ...");
                    waitForm.backgroundWorker1.RunWorkerAsync(new object[] { this, 2 });
                    break;
                case 2: //формирование отчета с результатами
                    waitForm.IsNumericProgressBar = false;
                    //waitForm.SetMaxValue(10);
                    waitForm.SetTextLabel("Формирование отчета с результатами моделирования ТС ...");
                    waitForm.backgroundWorker1.RunWorkerAsync(new object[] { this, 3 });
                    break;
                case 3://завершить обработку
                    waitForm.backgroundWorker1.DoWork -= AllSolve_DoWork;
                    waitForm.backgroundWorker1.RunWorkerCompleted -= AllSolve_Completed;
                    waitForm.Hide(); //здесь возможна ошибка из-за доступа из дочернего потока

                    //вывод результатов
                    ShowResultDay(coord.показателиРаботы);
                    openRes_button.Enabled = true;
                    break;
            }
        }

        private void OpenRes_button_Click(object sender, EventArgs e)
        {
            ПротоколExcel.Показать();
        }
        private void MatrCorr_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] masNameOst;
            МатрицаКорреспонденций МатрицаКорр = InitMatrixCorrespondents(out masNameOst);
            string errMsg;
            if (!MakeMatrixCorrespondents(ref МатрицаКорр, SettingsModel.ПоВсемМаршрутам, masNameOst, out errMsg))
                ;
            MessageBox.Show(errMsg);//"Расчет матрицы достижимостей закончен"
        }
        private void TrafPas_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл с генерациями пассажиров для обработки",
                Filter = "Файлы Excel (*.xlsx, *.xlsm)|*.xlsx;*.xlsm|Все файлы|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string errMsg;
                //обработать файл с пассажирами
                if (!ProcessPassangersGeneration(openFileDialog.FileName, out errMsg))
                    ;
                MessageBox.Show(errMsg);//"Обработка файла с пассажирами произведена");
            }
        }
        private void StructMarsh_ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите отчет с результатами",
                Filter = "Файлы Excel (*.xls, *.xlsx, *.xlsm)|*.xls;*.xlsx;*.xlsm|Все файлы|*.*"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string errMsg;
                //обработать файл с пассажирами
                if (!РасчетВсехМаршрутов(openFileDialog.FileName, SettingsModel.FileNameModel, out errMsg))
                    ;
                MessageBox.Show(errMsg);//"Обработка файла с пассажирами произведена");
            }
        }
        //временное  - не используется
        private void ToolStripButton1_Click(object sender, EventArgs e)//формирование соседних перегонов для ВСЕХ маршрутов
        {
            Остановка[] masOst = LoadOst(SettingsModel.FileNameModel);
            //загрузка маршрутов
            Маршрут[] masMarsh = LoadMarshrut(SettingsModel.FileNameModel, true);
            Dictionary<string, Перегон> dict = new Dictionary<string, Перегон>();
            string str;
            for (int i = 0; i < masMarsh.Length; i++)
            {
                int[] masO = masMarsh[i].КодыОстановок;
                for (int j = 0; j < masO.Length - 1; j++)
                {
                    str = masO[j] + "_" + masO[j + 1];
                    if (dict.ContainsKey(str))
                        continue;
                    str = masO[j + 1] + "_" + masO[j];
                    if (dict.ContainsKey(str))
                        continue;
                    dict[str] = new Перегон
                    {
                        codeOst = masO[j],
                        number = masO[j + 1]
                    };
                }
            }
            //сохранение списка перегонов в файл
            FileStream file = new FileStream("все перегоны.xlsx", FileMode.Create);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet wkSheet = package.Workbook.Worksheets.Add("перегоны");
                int irow = 1;
                wkSheet.Cells[irow, 1].Value = "код ост от";
                wkSheet.Cells[irow, 2].Value = "код ост до";
                wkSheet.Cells[irow, 3].Value = "Название от";
                wkSheet.Cells[irow, 4].Value = "Название до";
                irow++;
                foreach (KeyValuePair<string, Перегон> kvp in dict)
                {
                    wkSheet.Cells[irow, 1].Value = kvp.Value.codeOst;
                    wkSheet.Cells[irow, 2].Value = kvp.Value.number;
                    wkSheet.Cells[irow, 3].Value = masOst[kvp.Value.codeOst - 1].Название;
                    wkSheet.Cells[irow, 4].Value = masOst[kvp.Value.number - 1].Название;
                    irow++;
                }
                package.Save();
            }
            file.Close();
            MessageBox.Show("готово");
        }
        private Остановка[] LoadOst(string fileNameModel)//формирование списка всех остановок
        {
            Остановка[] masOsts;
            //*** открыть файл modelХХ.xlsm
            FileInfo file = new FileInfo(fileNameModel);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet wkSheet = package.Workbook.Worksheets["ИД_Остановки"];
                int КолОстановок = wkSheet.Cells[1, 1].GetValue<int>();
                masOsts = new Остановка[КолОстановок];
                int irow;
                for (int i = 0; i < КолОстановок; i++)
                {
                    irow = i + 3;
                    //создание новой остановки
                    masOsts[i] = new Остановка(wkSheet.Cells[irow, 1].GetValue<int>(), wkSheet.Cells[irow, 2].Text, 100, null);
                }
            }
            return masOsts;
        }

        private void ToolStripButton1_Click_1(object sender, EventArgs e)
        {
            string errMsg;
            if (coord.КолМаршрутов == 0) //если не было загрузки из файла
            {
                if (!LoadDataToCoordinator(out errMsg))
                    MessageBox.Show(errMsg, "Ошибка разбора трафика", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                coord.колЧасовРаботы = SettingsModel.КолЧасовМоделирования;
                Остановка.МАКСВремяОжиданияПасДоУхода = SettingsModel.МаксВремяОжидания;
                Остановка.ВЕРОЯТНОСТЬПродолженияПоездки = SettingsModel.ВероятностьПродолженияПоездки;
            }
            //
            //string strResult=coord.testTrol();
            List<ТроллейбусыДляПротокола> listRes = coord.TestTrol();
            Маршрут[] masTrol = coord.GetUniqeTrollMarsh();

            //
            //ShowResultDay(strResult);
            if(!ЗаписьОтчетаТроллейбусыВExcel(listRes, masTrol, out errMsg))
                MessageBox.Show(errMsg, "Ошибка разбора трафика", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
        public bool ЗаписьОтчетаТроллейбусыВExcel(List<ТроллейбусыДляПротокола> listRes,Маршрут[] masTrol, out string errMsg)
        {
            try
            {
                if (!ПротоколExcel.ОткрытьФайл(out errMsg))
                    return false;
                ПротоколExcel.ДобавитьЛист("троллейбусы");
                ПротоколExcel.ЗаписатьТроллейбусы(listRes,masTrol);
                ПротоколExcel.ЗакрытьФайл();
            }
            catch (Exception exc)
            {
                errMsg = exc.Message;
                return false;
            }
            return true;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Calculate.LoadStopFromExcel();
            Calculate.GeneratePass();
            Calculate.SaveToSheets();
        }
    }
}