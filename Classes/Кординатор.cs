using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ���������2017
{
    /// <summary>
    /// �������� �� �������������, ��������������� ������ ������
    /// </summary>
    public class ����������
    {
        public const double ������������������ = 20;

        private List<����> ����������;
        private ���������[] ���������������;
        private �������[] ���������������;

        //private int ����;
        public List<��������> ���������������;//������  ����������, ����������� �� ���������
        public List<List<����>> ��������������������;//������ ����, ����������� ����, � ������ ���������
        public ������������������� ����������������;
        public int ��������������;
        public bool ���������������;
        public ���������������������� �����������;

        public int ������������ { get { return ��������������� != null ? ���������������.Length : 0; ; } }
        public int ������������ { get { return ��������������� != null ? ���������������.Length : 0; } }

        public event EventHandler<IntEventArg> �������������;
        //����������� ������
        public ����������()
        {
            ���������� = new List<����>();
            ��������������� = new List<��������>();
            ���������������� = new �������������������();

            //������� ������� "����.�����������������" � ������������ "�����������.�����������������������"
            ����.������������������������ += �����������������������;
            //������� ������� "��������.���������������" � ������������ "�����������.���������������������������"
            ��������.��������������� += ���������������������������;
            //������� ������� "��������.�������������" � ������������ "�����������.��������������"
            ��������.������������� += ��������������;
            //������� ������� "����.����������������������������" � ������������ "�����������.�����������������������"
            ����.���������������������������� += �����������������������;
            //������� ������� "�������.�����������������" � ������������ "�����������.���������������"
            �������.����������������� += ����������;
        }

        //    public Property ���������������() As �������()
        //        Get
        //            return _���������������
        //         Get
        //        Set(ByVal value As �������())
        //            ����������������(value)
        //         Set
        //     Property

        //    public Property ���������������() As ���������.���������()
        //        Get
        //            return _���������������
        //         Get
        //        Set(ByVal value As ���������.���������())
        //            �����������������(value)
        //         Set
        //     Property

        //    public ReadOnly Property ����() As Integer
        //        Get
        //            return _����
        //         Get
        //     Property
        //���������� ��� ���������� ������� ������
        private void �������������()
        {
            if (����������.Count > 0)
                ����������.Clear();
            foreach (������� ���� in ���������������)
                ����.�������������();
            foreach (��������� ��� in ���������������)
                ���.�������������();
            if (�������������������� != null)
                for (int i = 0; i < ��������������������.Count; i++)
                    ��������������������[i].Clear();
            ���������������.Clear();
            ����������������.�������������();
        }

        //������� ������ �� ������� ����� ����
        private void ��������������(int �������)
        {
            //���� = �������;
            //������ ����� ����
            foreach (������� ���� in ���������������)
                ����.��������������(�������);
            //������/������ ���� � ���������
            foreach (���� ���� in ����������)
                ����.��������������(�������);
            //�������� ���� ����������� ���� �� �����������
            int i = 0;
            while (i < ����������.Count)
            {
                ���� ���� = ����������[i];
                if (����.��������� == �������������Enum.������������)
                    �������������(����);
                else
                    i++;
            }
            //��������� ����������, ������� �� ����
            foreach (��������� ��� in ���������������)
                ���.��������������(�������);
        }

        //������� ���� � ��������������� ���������
        private void �����������������������(object obj, AutoEventArg arg)
        {
            ���� ���� = obj as ����;
            ���������������[arg.������ - 1].���������������(����);
            //foreach (��������� ��� in ���������������)
            //    if (���.��� == arg.������)
            //    {
            //        ���.���������������(����);
            //        break;
            //    }
        }

        //���������� ������� "����.�����������������"
        private void �����������������������(object obj, AutoEventArg arg)
        {
            ���� ���� = obj as ����;
            if (���������������[arg.������ - 1].����������(����) == false)
                //����� ��� ���� ���
                ����.��������������������();
            else
                //����� ��� ���� ����
                ����.�������������������������(arg.�������);
            //foreach (��������� ��� in ���������������)
            //    if (���.��� == arg.������)
            //    {
            //        if (���.����������(����) == false)
            //            //����� ��� ���� ���
            //            ����.��������������������();
            //        else
            //            //����� ��� ���� ����
            //            ����.�������������������������(arg.�������);
            //        break;
            //    }
        }

        //���������� ������ �� ��������� ��� �������� �� ��
        public void �����������������(���������[] ���������)
        { ��������������� = ���������; }

        //���������� ������ � �������� ��� �������� �� ��
        public void ����������������(�������[] ��������)
        {
            ��������������� = ��������;
            //������������ ������(������) ��� ������ ���������, ������ ������� �������� �������� 
            // �������(��������) ���� ������� ��������
            �������������������� = new List<List<����>>(��������.Length);
            for (int i = 0; i < ���������������.Length; i++)
            {
                List<����> ������������� = new List<����>();
                ��������������������.Add(�������������);
                ���������������[i].��������� = �������������;
            }
        }

        // ���������� ������������� �������� ���� �� ���� ��������� ������ ���
        //(�����������: ������� "���������������" � "���������������" ������ ��������� ���������� ������)
        public void �������������1���()//object ser)
        {
            //Dim bgw As BackgroundWorker = CType(ser, BackgroundWorker)
            �������������();
            //*** ������������� �� ������ ������� (�� �������)
            for (int ������� = 0; ������� < �������������� * 60; �������++)
            {
                ��������������(�������);
                //bgw.ReportProgress(�������); /// �������������� * 60 - 1 )
                �������������?.Invoke(this, new IntEventArg(������� + 1));
            }

            //*** ������ �����������
            //������ ����������� �� ����������
            foreach (��������� ��� in ���������������)
                ���.����������������.�����������������������(���������������, ��������������, ���.������);

            //������ ����������� �� ���������
            foreach (������� ���� in ���������������)
                ����.����������������.�����������������(��������������);
            //������ ����������� �� ���� ������ ������������� (����)
            ����������������.�����������������(���������������, ���������������, ����������, ��������������);
            ��������������� = true;
        }

        //���������� ������� "��������.�������������"
        private void ��������������(object obj, IntEventArg arg)
        {
            ���������������[arg.value - 1].����������������(obj as ��������);
            //foreach (��������� ��� in ���������������)
            //    if (���.��� == arg.value)
            //    {
            //        ���.����������������(obj as ��������);
            //        break;
            //    }
        }

        //���������� ������� "��������.���������������"
        public void ���������������������������(object obj, IntEventArg arg)
        {
            ���������������.Add(obj as ��������);
        }

        //���������� ���������� ����� ����
        private void �������������(���� ����)
        {
            ����������.Remove(����);
            ��������������������[����.����������� - 1].Add(����);
            ////����� ������� � ������� �������������������� ��� �������� � ���� ����
            //for (int i = 0; i < ���������������.Length; i++)
            //    if (���������������[i].��� == ����.�����������)
            //    {
            //        ��������������������[i].Add(����);
            //        break;
            //    }
        }

        //���������� ������� "�������.�����������������"
        private void ����������(object obj, IntEventArg arg)
        {
            ������� ������������ = obj as �������;
            //���������� ������ ���� �� ��������� ���������
            ���� auto = new ����();
            auto.�������������(������������, ������������.��������������������, arg.value);
            ����������.Add(auto);
        }

        //������ ������ �� ���� ����������, ����, ��������� � Excel ����� �� �������������
        public bool �������������Excel(out string errMsg)
        {
            try
            {
                if (!��������Excel.�����������(out errMsg))
                    return false;
                foreach (������� ���� in ���������������)
                    if (����.����������������.����������������� > 0)
                    {
                        ��������Excel.������������("�������_" + ����.���);

                        ��������Excel.���������������(����);
                    }

                if (SettingsModel.�����������)
                {
                    ��������Excel.������������("����_��������");
                    ��������Excel.���������������("����_��������", 1);
                    foreach (List<����> marsh in ��������������������)
                        foreach (���� auto in marsh)
                            ��������Excel.������������(auto);

                    ��������Excel.������������("����_�_����");
                    ��������Excel.���������������("����_�_����", 1);
                    foreach (���� auto in ����������)
                        ��������Excel.������������(auto);

                    ��������Excel.������������("����_�������");
                    ��������Excel.���������������("����_�������", 1);
                    ��������Excel.�����������������(��������������������, ����������);
                }

                if (SettingsModel.���������������)
                {
                    ��������Excel.������������("���");
                    ��������Excel.���������������("���", 1);
                    ��������Excel.�����������(���������������);
                }

                ��������Excel.������������("���������");
                ��������Excel.���������������("���������", 1);
                ��������Excel.��������������������(���������������);

                //foreach (��������� ��� in ���������������)
                //    ////        if ���.����������������.������������������������ > 0 )
                //    ��������Excel.�����������������(���.����������������, ���.��������);

                //������� �� ���������
                ��������Excel.������������("�����_��_���������");
                ��������Excel.���������������("�����_��_���������", 1);
                ��������Excel.�������������������(���������������);

                ��������Excel.������������("�������������_��");
                ��������Excel.���������������("�������������_��", 1);
                ��������Excel.�����������������������(���������������);

                ��������Excel.������������("����");
                ��������Excel.���������������("����", 1);
                ��������Excel.������������(����������������, ��������������);

                ��������Excel.�����������();
                //��������Excel.��������();
            }
            catch (Exception exc)
            {
                errMsg = exc.Message;
                return false;
            }
            return true;
        }
        private ����[] GenerateReis(int capacity, int interval)//��������� ������ ������ � �������� ����������
        {
            int cnt = SettingsModel.��������������������� * 60 / interval;
            ����[] listReis = new ����[cnt];
            for (int i = 0; i < cnt; i++)
            {
                listReis[i] = new ����
                {
                    ���������������� = i * interval,//"�����"
                    ��������������� = capacity //"�����������"
                };
            }
            return listReis;
        }
        private List<�������> GetDoublicateRoute(������� marsh) //������ ���������, ������� ��������� �� ���������� �� 25% marsh
        {
            int[] masOst = marsh.�������������;
            int limit = (int)(masOst.Length * 0.25);
            List<�������> listRoute = new List<�������>();
            foreach (������� ���� in ���������������)
            {
                if (���� == marsh || ����.������������� == ���������������������Enum.���������� || ����.������������� == ���������������������Enum.�������)
                    continue;
                int[] masOst2 = ����.�������������;
                int count = 0;
                for (int i = 0; i < masOst.Length; i++)
                {
                    int ost = masOst[i];
                    for (int j = 0; j < masOst2.Length; j++)
                        if (ost == masOst2[j])
                            count++;
                }
                if (count >= limit)
                    listRoute.Add(����);
            }
            return listRoute;
        }
        //public string testTrol()
        public List<�����������������������> TestTrol()
        {
            const int COUNTTROLLALL = 240;
            //StringBuilder sb = new StringBuilder();
            List<�����������������������> listRes = new List<�����������������������>();
            //�������� ������ ������������� ���������
            �������[] masTrol = (from m in ��������������� where m.������������� == ���������������������Enum.���������� select m).ToArray<�������>();
            bool[] masPaired = new bool[masTrol.Length];

            //����� ��������-������� ������������� ���������
            List<List<�������>> listDubl = new List<List<�������>>();
            List<List<int>> listDublCountTC = new List<List<int>>();
            List<List<int>> listDublInterval = new List<List<int>>();
            List<List<bool>> listDublPaired = new List<List<bool>>();
            bool nextIsPairedDubl;
            for (int i = 0; i < masTrol.Length; i++)
            {
                List<�������> listD = GetDoublicateRoute(masTrol[i]);
                listDubl.Add(listD);
                //��������� ��������� �������� ��������
                listDublCountTC.Add((from m in listD select m.�����������������).ToList<int>());
                listDublInterval.Add((from m in listD select (int)Math.Round(2.0 * m.�����1����� / m.�����������������)).ToList<int>());
                //�������� ���������-������
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
                //��������, ��� ��������� - ������/�������� �������
                masPaired[i] = i< masTrol.Length - 1 && TestNextPaired(masTrol[i], masTrol[i+1]);
                if (masPaired[i])
                {
                    listDubl.Add(null);
                    //��������� ��������� �������� ��������
                    listDublCountTC.Add(null);
                    listDublInterval.Add(null);
                    listDublPaired.Add(null);
                    i++;
                }                
            }

            int[] masOst;
            ����[] masRoute;
            int countTrollAll = masTrol.Sum((������� m) => m.�����������������);
            //������ �������� �������� �������������� ��������
            for (int i_marsh = 0; i_marsh < masTrol.Length; i_marsh++)
            {
                masOst=null;
                ////��������, ��� ��������� - ������/�������� �������
                //nextIsPaired = i_marsh < masTrol.Length - 1 ? TestNextPaired(masTrol[i_marsh], masTrol[i_marsh+1]) : false;
                //��������� ��������� �������� ��������
                int countTCBase = masTrol[i_marsh].�����������������;//����� �� �� �������� ��������
                int intervalBase = (int)Math.Round(2.0 * masTrol[i_marsh].�����1����� / countTCBase);//�������� �������� �� �� �������� ��������
                //int prevCounTC=0, prevInterval;//��������� ����������� ������� - ����� �� ����������� �������� � ����������� �����������
                for (int i_int = intervalBase; i_int >= 1; i_int--)
                {
                    //��������� ����� �� ������������
                    int countTC = (int)Math.Round(2.0 * masTrol[i_marsh].�����1����� / i_int);//����� �� �� ���������� ��������
                    //int countTC_ = nextIsPaired ? countTrollAll - 2 * countTCBase + 2 * countTC : countTrollAll - countTCBase + countTC;
                    int countTC_ = masPaired[i_marsh] ? countTrollAll - 2 * countTCBase + 2 * countTC : countTrollAll - countTCBase + countTC;
                    if (countTC_ > COUNTTROLLALL)
                        break;

                    //������ �������� �������� �������������� ��������
                    masRoute = GenerateReis(masTrol[i_marsh].�����������1�����, i_int);
                    masTrol[i_marsh].������������������(masRoute);
                    masTrol[i_marsh].����������������� = countTC;
                    if (masPaired[i_marsh])//nextIsPaired)
                    {
                        masTrol[i_marsh + 1].������������������(masRoute);//GenerateReis??
                        masTrol[i_marsh + 1].����������������� = countTC;
                    }
                    double kInterval = 1.0 * (i_int - 1) / (intervalBase - 1);
                    //������ ��������� �������� ��������
                    if (kInterval < 1.0)
                        for (int i_dub = 0; i_dub < listDubl[i_marsh].Count; i_dub++)
                        {
                            //��������, ��� ��������� - ������/�������� �������
                            //nextIsPairedDubl = i_dub < listDubl[i_marsh].Count - 1 ? TestNextPaired(listDubl[i_marsh][i_dub], listDubl[i_marsh][i_dub + 1]) : false;
                            nextIsPairedDubl = listDublPaired[i_marsh][i_dub];
                            int newInt = (int)Math.Round(2.0 * listDubl[i_marsh][i_dub].�����1����� / listDublCountTC[i_marsh][i_dub] / kInterval);
                            int newCountTC = (int)Math.Round(2.0 * listDubl[i_marsh][i_dub].�����1����� / newInt);//����� �� �� ���������� ��������
                            newCountTC = Math.Max(newCountTC, 1);
                            if (listDubl[i_marsh][i_dub].����������������� != newCountTC)
                            {
                                listDubl[i_marsh][i_dub].����������������� = newCountTC;
                                newInt = (int)Math.Round(2.0 * listDubl[i_marsh][i_dub].�����1����� / newCountTC);
                                masRoute = GenerateReis(listDubl[i_marsh][i_dub].�����������1�����, newInt);
                                listDubl[i_marsh][i_dub].������������������(masRoute);
                                if (nextIsPairedDubl)
                                {
                                    listDubl[i_marsh][i_dub + 1].����������������� = newCountTC;
                                    listDubl[i_marsh][i_dub + 1].������������������(masRoute);//GenerateReis??
                                }
                            }
                            if (nextIsPairedDubl)
                                i_dub++;
                        }

                    //�������������� ��������� ���������� �� ���������� �������������� �������� /// � ��������
                    masOst = masTrol[i_marsh].�������������;
                    double k = 12.0 / (60/i_int + 12); //����. ������������ ����������
                    for (int i = 0; i < masOst.Length; i++)
                        ���������������[masOst[i] - 1].���������������� = k;//���������������������(k);

                    //������ �������������
                    �������������1���();

                    //���������� ����������� �������������
                    //TempSaveResult(sb, ����������������, masTrol[i_marsh].���, i_int);
                    TempSaveResult2(listRes, masTrol, masPaired, i_marsh, i_int);
                }
                //������������ ������� ������� ��� i_marsh ?
                if (masTrol[i_marsh].����������������� != countTCBase)
                {
                    masTrol[i_marsh].������������������(GenerateReis(masTrol[i_marsh].�����������1�����, intervalBase));
                    masTrol[i_marsh].����������������� = countTCBase;
                    if (masPaired[i_marsh])//nextIsPaired)
                    {
                        masTrol[i_marsh + 1].������������������(GenerateReis(masTrol[i_marsh].�����������1�����, intervalBase));
                        masTrol[i_marsh + 1].����������������� = countTCBase;
                    }
                }
                //������������ ������� ������� ��� �������� i_marsh ?
                for (int i_dub = 0; i_dub < listDubl[i_marsh].Count; i_dub++)
                {
                    if (listDubl[i_marsh][i_dub].����������������� != listDublCountTC[i_marsh][i_dub])
                    {
                        //nextIsPairedDubl = i_dub < listDubl[i_marsh].Count - 1 ? TestNextPaired(listDubl[i_marsh][i_dub], listDubl[i_marsh][i_dub + 1]) : false;
                        nextIsPairedDubl = listDublPaired[i_marsh][i_dub];
                        masRoute = GenerateReis(listDubl[i_marsh][i_dub].�����������1�����, listDublInterval[i_marsh][i_dub]);
                        listDubl[i_marsh][i_dub].������������������(masRoute);
                        listDubl[i_marsh][i_dub].����������������� = listDublCountTC[i_marsh][i_dub];
                        if (nextIsPairedDubl)
                        {
                            listDubl[i_marsh][i_dub + 1].������������������(masRoute);
                            listDubl[i_marsh][i_dub + 1].����������������� = listDublCountTC[i_marsh][i_dub];
                            i_dub++;
                        }
                    }
                }
                //������������ ��������� ���������� �� ���������� �������������� �������� 
                if(masOst!=null)
                    for (int i = 0; i < masOst.Length; i++)
                        ���������������[masOst[i] - 1].���������������� = 0;
                if (masPaired[i_marsh])//nextIsPaired)
                    i_marsh++;
            }
            //return sb.ToString();
            return listRes;
        }

        private bool TestNextPaired(������� m1, ������� m2)//��������, ��� ��������� - ������/�������� �������
        {
            if (m1!=null && m2!=null)
            {
                //����������� ������ �������� - ������ � ��������
                string curCode = m1.��������.Split(new char[] { ' ' }, 2)[0];
                string nextCode = m2.��������.Split(new char[] { ' ' }, 2)[0];
                if (nextCode == curCode && m1.������������� == m2.�������������)
                    return true;
            }
            return false;
        }
        public �������[] GetUniqeTrollMarsh()
        {
            List<�������> listTrol= (from m in ��������������� where m.������������� == ���������������������Enum.���������� select m).ToList<�������>();
            for (int i = listTrol.Count - 2; i >= 0; i--)
                if (TestNextPaired(listTrol[i], listTrol[i + 1]))
                {
                    listTrol.RemoveAt(i + 1);
                    i--;
                }
            return listTrol.ToArray();
        }

        private void TempSaveResult(StringBuilder sb, ������������������� pkday, int i_marsh, int i_int)//���������� ����������� �������������
        {
            sb.AppendLine("  *** ���������� ������������� ***");
            sb.AppendFormat("������� = {0}, �������� = {1}\n", i_marsh, i_int);
            sb.AppendLine(string.Format("  (���������={0}, ���������={1}, ������ �������������={2} �)", ������������, ������������, ��������������));
            sb.AppendLine("����� ����� ����������� ������ = " + pkday.���������������.ToString("N0"));
            sb.AppendLine("����� ����� ������ � ���� = " + pkday.�����������������������.ToString("N0"));
            sb.AppendLine("���������� ����������� ���������� = " + pkday.������������������������.ToString("N0"));
            sb.AppendLine("���������� ������������ ���������� = " + pkday.�������������������.ToString("N0"));
            sb.AppendLine("���������� ������������� ���������� = " + pkday.�����������������������.ToString("N0"));
            sb.AppendLine("���������� ���������� ���������� �� ���������� = " + pkday.���������������������.ToString("N0"));
            sb.AppendLine("���������� ���������� ���������� � �� = " + pkday.���������������������.ToString("N0"));
            sb.AppendLine("���������� ������������ ���������� = " + pkday.��������������������������.ToString("N0"));
            sb.AppendLine("������� ����� ������������ ���������� � ��� = " + pkday.�������������������.ToString("N0"));
            sb.AppendLine("������� ����������� ������������� ����������� �� = " + pkday.������������������.ToString("N2"));
            sb.AppendLine("������� ��������� ������� � �� = " + pkday.��������������������.ToString("N2"));
            sb.AppendLine("������� ��������� ������� � ���������� = " + pkday.���������������������.ToString("N2"));
            sb.AppendLine("������� = " + pkday.�������.ToString("N0") + " ���");
        }
        private void TempSaveResult2(List<�����������������������> listRes, �������[] masTrol, bool[] masPaired, int i_marsh, int i_int)//���������� ����������� �������������
        {
            ����������������������� item=new �����������������������();
            int cnt = masPaired.Count((bool b) => b);
            item.AllTrollKm = new int[cnt];
            item.IntervalsTroll = new int[cnt];
            item.CountTCTroll = new int[cnt];
            item.PassTroll= new int[cnt];
            item.AllTrollPass = 0;
            item.AllTrollProfit = 0;
            for (int i = 0, j=0; i < masTrol.Length; i++)
            {
                ������� ���� = ���������������[masTrol[i].��� - 1];
                if (masPaired[i])
                {
                    item.CountTCTroll[j] = masTrol[i].�����������������;
                    item.IntervalsTroll[j] = (i != i_marsh ? (int)Math.Round(2.0 * masTrol[i].�����1����� / masTrol[i].�����������������) : i_int);
                    item.AllTrollKm[j] =(int)(����.����������������.����������������� * 2 * ����.�������������);
                    item.PassTroll[j] = ����.����������������.�������������������;
                    if (i + 1 < masTrol.Length && !masPaired[i + 1]) 
                        item.PassTroll[j] += ���������������[masTrol[i+1].��� - 1].����������������.�������������������;
                    j++;
                }
                item.AllTrollPass += ����.����������������.�������������������;
                item.AllTrollProfit+=����.����������������.�������;
            }
            listRes.Add(item);
        }
    }
}