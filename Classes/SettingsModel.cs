using System.IO;
using System.Xml;

namespace Транспорт2017
{
    public static class SettingsModel
    {
        const string SETTINGSFILE = "settings.xml";
        public static string FileNameModel { get; set; }
        public static string FileNameCorresp { get; set; }
        public static string FileNameTrafic { get; set; }
        public static int КолЧасовМоделирования { get; set; }
        public static int МаксВремяОжидания { get; set; }
        public static double ВероятностьПродолженияПоездки { get; set; }
        public static bool ПолныйОтчет { get; set; }
        public static int НачЧасДляТрафика { get; set; }
        public static bool ПоВсемМаршрутам { get; set; }
        public static bool ПасажировВОтчет { get; set; }

        static SettingsModel()
        {
            if (!Load())
            {
                FileNameModel = "model4+data13_5-00to21-00.xlsm";
                FileNameCorresp = "матрица корреспонденций_все маршруты.xlsx";
                FileNameTrafic = "trafic_pass_с 5-00.xlsx";
                КолЧасовМоделирования = 17;
                МаксВремяОжидания = 60;
                ВероятностьПродолженияПоездки = 0.2;
                ПолныйОтчет = false;
                НачЧасДляТрафика = 6;
                ПоВсемМаршрутам = true;
                ПасажировВОтчет = false;
            }
        }
        public static void Save()
        {
            //получить свойства всех наблюдаемых пар
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<SettingsModel/>");
            XmlNode items = doc.DocumentElement;
            XmlElement elem = doc.CreateElement("FileNameModel");
            elem.InnerText = FileNameModel;
            items.AppendChild(elem);
            elem = doc.CreateElement("FileNameCorresp");
            elem.InnerText = FileNameCorresp;
            items.AppendChild(elem);
            elem = doc.CreateElement("FileNameTrafic");
            elem.InnerText = FileNameTrafic;
            items.AppendChild(elem);
            elem = doc.CreateElement("КолЧасовМоделирования");
            elem.InnerText = КолЧасовМоделирования.ToString();
            items.AppendChild(elem);
            elem = doc.CreateElement("МаксВремяОжидания");
            elem.InnerText = МаксВремяОжидания.ToString();
            items.AppendChild(elem);
            elem = doc.CreateElement("ВероятностьПродолженияПоездки");
            elem.InnerText = ВероятностьПродолженияПоездки.ToString();
            items.AppendChild(elem);
            elem = doc.CreateElement("ПолныйОтчет");
            elem.InnerText = ПолныйОтчет.ToString();
            items.AppendChild(elem);
            elem = doc.CreateElement("НачЧасДляТрафика");
            elem.InnerText = НачЧасДляТрафика.ToString();
            items.AppendChild(elem);
            elem = doc.CreateElement("ПоВсемМаршрутам");
            elem.InnerText = ПоВсемМаршрутам.ToString();
            items.AppendChild(elem);
            elem = doc.CreateElement("ПасажировВОтчет");
            elem.InnerText = ПасажировВОтчет.ToString();
            items.AppendChild(elem);
            //записать в файл
            XmlTextWriter writer = new XmlTextWriter(SETTINGSFILE, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            doc.Save(writer);
            writer.Close();
        }
        public static bool Load()
        {
            if (!File.Exists(SETTINGSFILE))
                return false;
            //считать из файла
            XmlReader reader = XmlReader.Create(SETTINGSFILE);
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            reader.Close();

            XmlElement elem = doc.DocumentElement;
            FileNameModel = elem.GetElementsByTagName("FileNameModel")[0].InnerText;
            FileNameCorresp = elem.GetElementsByTagName("FileNameCorresp")[0].InnerText;
            FileNameTrafic = elem.GetElementsByTagName("FileNameTrafic")[0].InnerText;
            КолЧасовМоделирования = int.Parse(elem.GetElementsByTagName("КолЧасовМоделирования")[0].InnerText);
            МаксВремяОжидания = int.Parse(elem.GetElementsByTagName("МаксВремяОжидания")[0].InnerText);
            ПолныйОтчет = bool.Parse(elem.GetElementsByTagName("ПолныйОтчет")[0].InnerText);
            НачЧасДляТрафика = int.Parse(elem.GetElementsByTagName("НачЧасДляТрафика")[0].InnerText);
            ПоВсемМаршрутам = bool.Parse(elem.GetElementsByTagName("ПоВсемМаршрутам")[0].InnerText);
            if (elem.GetElementsByTagName("ВероятностьПродолженияПоездки").Count != 0)
                ВероятностьПродолженияПоездки = double.Parse(elem.GetElementsByTagName("ВероятностьПродолженияПоездки")[0].InnerText);
            if (elem.GetElementsByTagName("ПасажировВОтчет").Count != 0)
                ПасажировВОтчет = bool.Parse(elem.GetElementsByTagName("ПасажировВОтчет")[0].InnerText);
            return true;
        }
    }
}