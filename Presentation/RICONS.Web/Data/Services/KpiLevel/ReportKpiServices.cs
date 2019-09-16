using Newtonsoft.Json.Linq;
using RICONS.Core.Functions;
using RICONS.DataServices;
using RICONS.DataServices.DataClass;
using RICONS.Logger;
using RICONS.Web.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RICONS.Web.Data.Services
{
    public class ReportKpiServices : BaseData
    {
        public ReportKpiServices()
        {
            logger = new Log4Net(typeof(ReportKpiServices));
        }

        #region  BÁO CÁO KPI NHÂN VIÊN THÁNG

        public int CountRows_ReportKpi_Month(KpiReport_MonthModels clparam)
        {
            logger.Start("CountRows_ReportKpi_Month");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                if (clparam.hovaten == "" || clparam.hovaten == null || clparam.hovaten == "undefined")
                    param["hovaten"] = "";
                if (clparam.chucdanh == "1")
                {
                    param["nguoitao"] = clparam.nguoitao;
                    param["maphongban"] = clparam.maphongban;
                }
                else
                {
                    param["nguoitao"] = "";
                    param["maphongban"] = clparam.maphongban;
                }
                if (clparam.xoa == "1")
                {
                    param["nguoitao"] = "";
                }
                iResult = (int)sqlMap.ExecuteQueryForObject("KpiReport_Month.CountRows_KpiReportMonth", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_ReportKpi_Month");
            return iResult;
        }

        public List<KpiReport_MonthModels> SelectRow_ReportKpi_Month(KpiReport_MonthModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_ReportKpi_Month");
            List<KpiReport_MonthModels> lstResult = new List<KpiReport_MonthModels>();

            List<KpiReport_MonthModels> lstResultnew = new List<KpiReport_MonthModels>();

            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;
                param["thangnam"] = clParam.thangnam;
                if (clParam.hovaten == "" || clParam.hovaten == null || clParam.hovaten == "undefined")
                    param["hovaten"] = "";
                if (clParam.chucdanh == "1")
                {
                    param["nguoitao"] = clParam.nguoitao;
                    param["maphongban"] = clParam.maphongban;
                }
                else
                {
                    param["userid"] = "";
                    param["maphongban"] = clParam.maphongban;
                }
                if (clParam.xoa == "1")
                {
                    param["nguoitao"] = "";
                }
                if (clParam.maphongban.Trim() == "0") param["maphongban"] = "";

                Hashtable param1 = new Hashtable();
                List<PhongBanModels> lstPhongban = new List<PhongBanModels>();
                IList ilist1 = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows", param1);
                CastDataType cast1 = new CastDataType();
                lstPhongban = cast1.AdvanceCastDataToList<PhongBanModels>(ilist1);
                IList ilist = null;
                if (clParam.kiemtra == "1")
                {
                    ilist = sqlMap.ExecuteQueryForList("KpiReport_Month.SelectRow_KpiReportMonth", param);
                    CastDataType cast = new CastDataType();
                    lstResultnew = cast.AdvanceCastDataToList<KpiReport_MonthModels>(ilist);
                }
                else
                {
                    ilist = sqlMap.ExecuteQueryForList("KpiReport_Month.SelectRows_KpiEmployeeDetail_load", param);
                    CastDataType cast = new CastDataType();
                    lstResult = cast.AdvanceCastDataToList<KpiReport_MonthModels>(ilist);
                    string tongso = "0"; int dem = 0;
                    foreach (var itempb in lstPhongban)
                    {
                        var lstdulieutontai = lstResult.Where(p => p.maphongban == itempb.maphongban.Trim()).ToList();
                        if (lstdulieutontai.Count > 0)
                        {
                            try
                            {
                                tongso = lstdulieutontai.Where(p => p.maphongban == itempb.maphongban.Trim()).Sum(p => int.Parse(p.ketquakpi.Replace("%", "").Trim())).ToString()+"%";
                            }
                            catch (Exception)
                            {
                                tongso = "0%";
                            }
                            lstResultnew.Add(new KpiReport_MonthModels { hovaten = itempb.tenphongban, maphongban=itempb.maphongban, ketquakpi = tongso, groupcha="1",stt=(dem+1).ToString() });
                            dem = dem + 1;
                            foreach (var item in lstdulieutontai)
                            {
                                string ketqua = item.ketquakpi.Trim().Replace("%", "");
                                if (ketqua != "" && ketqua != null)
                                {
                                    if (int.Parse(ketqua) == 100)
                                    {
                                        item.maphanloaiketqua = "1";
                                        item.phanloaiketqua = "Xuất sắc (A+)";
                                    }
                                    else if (int.Parse(ketqua) > 90 && int.Parse(ketqua) < 100)
                                    {
                                        item.maphanloaiketqua = "2";
                                        item.phanloaiketqua = "Hoàn thành tốt (A)";
                                    }
                                    else if (int.Parse(ketqua) > 80 && int.Parse(ketqua) <= 90)
                                    {
                                        item.maphanloaiketqua = "3";
                                        item.phanloaiketqua = "Hoàn thành (B)";
                                    }
                                    else if (int.Parse(ketqua) > 70 && int.Parse(ketqua) <= 80)
                                    {
                                        item.maphanloaiketqua = "4";
                                        item.phanloaiketqua = "Cần cố gắng (C)";
                                    }
                                    else
                                    {
                                        item.maphanloaiketqua = "5";
                                        item.phanloaiketqua = "Không hoàn thành (D)";
                                    }   
                                }
                                lstResultnew.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_ReportKpi_Month");
            return lstResultnew;
        }

        public string Save_Kpi_Month(string madanhsach, string nguoitao)
        {
            logger.Start("Save_Kpi_Month");
            string  strResult = "1";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(madanhsach);
                JArray json_tiendo_giatri = (JArray)json["data1"];
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));

                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param["matonghopketqua"] = AES.DecryptText(json_tiendo_giatri[i]["matonghopketqua"].ToString().Trim(), function.ReadXMLGetKeyEncrypt());
                    param["groupcha"] = json_tiendo_giatri[i]["groupcha"].ToString().Trim();
                    param["thangnam"] = json_tiendo_giatri[i]["thangnam"].ToString().Trim();
                    param["stt"] = json_tiendo_giatri[i]["stt"].ToString().Trim();
                    if (json_tiendo_giatri[i]["manhanvien"].ToString().Trim() == "" || json_tiendo_giatri[i]["manhanvien"].ToString().Trim() == null || json_tiendo_giatri[i]["manhanvien"].ToString().Trim() == "undefined")
                        param["manhanvien"] = "";
                    else
                        param["manhanvien"] = json_tiendo_giatri[i]["manhanvien"].ToString().Trim();
                    param["maphongban"] = json_tiendo_giatri[i]["maphongban"].ToString().Trim();
                    param["hovaten"] = json_tiendo_giatri[i]["hovaten"].ToString().Trim();
                    param["hanhvithaido"] = json_tiendo_giatri[i]["hanhvithaido"].ToString().Trim();

                    param["tenchucdanhns"] = json_tiendo_giatri[i]["tenchucdanhns"].ToString().Trim();

                    param["giaiquyetcongviec"] = json_tiendo_giatri[i]["giaiquyetcongviec"].ToString().Trim();
                    param["ketquakpi"] = json_tiendo_giatri[i]["ketquakpi"].ToString().Trim();
                    
                    param["maphanloaiketqua"] = json_tiendo_giatri[i]["maphanloaiketqua"].ToString().Trim();
                    if (param["maphanloaiketqua"].ToString().Trim() == "1")
                        param["phanloaiketqua"] = "Xuất sắc (A+)";
                    else param["phanloaiketqua"] = json_tiendo_giatri[i]["phanloaiketqua"].ToString().Trim();
                    param["ghichu"] = json_tiendo_giatri[i]["ghichu"].ToString().Trim();
                    param["nguoitao"] = nguoitao;

                    param["ghichu"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["ghichu"].ToString().Trim(), @"\n", "\\n").Replace("'", "");

                    if (param["matonghopketqua"].ToString().Trim() == "" || param["matonghopketqua"].ToString().Trim() == "0" || param["matonghopketqua"].ToString().Trim() == "undefined")
                    {
                        IList ilist = sqlMap.ExecuteQueryForList("KpiReport_Month.SelectRows_ExitKPIMonth", param);
                        if (ilist.Count > 0)
                        {
                            Hashtable param1 = (Hashtable)ilist[0];
                            param["matonghopketqua"] = param1["matonghopketqua"].ToString();
                            sqlMap.Update("KpiReport_Month.UpdateRow_KpiResultMonth", param);
                        }
                        else
                        {
                            sqlMap.Insert("KpiReport_Month.InsertRow_KpiResultMonth", param);
                        }
                        
                    }
                    else
                        sqlMap.Update("KpiReport_Month.UpdateRow_KpiResultMonth", param);
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("Save_Kpi_Month");
            return strResult;
        }

        #endregion

        #region  BÁO CÁO KPI NHÂN VIÊN NĂM

        public int CountRows_ReportKpi_Year(KpiReport_MonthModels clparam)
        {
            logger.Start("CountRows_ReportKpi_Month");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                if (clparam.hovaten == "" || clparam.hovaten == null || clparam.hovaten == "undefined")
                    param["hovaten"] = "";
                if (clparam.chucdanh == "1")
                {
                    param["nguoitao"] = clparam.nguoitao;
                    param["maphongban"] = clparam.maphongban;
                }
                else
                {
                    param["nguoitao"] = "";
                    param["maphongban"] = clparam.maphongban;
                }
                if (clparam.xoa == "1")
                {
                    param["nguoitao"] = "";
                }
                iResult = (int)sqlMap.ExecuteQueryForObject("KpiReport_Month.CountRows_KpiReportMonth", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_ReportKpi_Month");
            return iResult;
        }

        public List<KpiReport_YearModels> SelectRow_ReportKpi_Year(KpiReport_YearModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_ReportKpi_Year");
            List<PhongBanModels> lstResult_phongban = new List<PhongBanModels>();
            List<KpiReport_YearModels> lstResult_YearNew = new List<KpiReport_YearModels>();
            Hashtable param = new Hashtable();
            param["trangbd"] = trangbd;
            param["trangkt"] = trangkt;
            param["namkpi"] = clParam.thangnam;
            if (clParam.maphongban.Trim() == "0") param["maphongban"] = "";
            param["phongban_congtruong"] = "0";
            IList ilist = sqlMap.ExecuteQueryForList("KpiReport_Month.SelectRow_ListDepartment", param);
            CastDataType cast = new CastDataType();
            lstResult_phongban = cast.AdvanceCastDataToList<PhongBanModels>(ilist);
            if (lstResult_phongban.Count > 0)
            {
                string listdepartment = "";
                for (int i = 0; i < lstResult_phongban.Count; i++)
                {
                    if (i == lstResult_phongban.Count - 1)
                    {
                        listdepartment = listdepartment + "vemp.OrgID= " + lstResult_phongban[i].maphongban;
                    }
                    else
                    {
                        listdepartment = listdepartment + "vemp.OrgID= " + lstResult_phongban[i].maphongban + " or ";
                    }
                }
                param["maphongban"] = listdepartment;

                List<KpiReport_YearModels> lstResult_Year = new List<KpiReport_YearModels>();
                ilist = null;
                ilist = sqlMap.ExecuteQueryForList("KpiReport_Month.SelectRows_KPIYear", param);
                CastDataType cast1 = new CastDataType();
                lstResult_Year = cast1.AdvanceCastDataToList<KpiReport_YearModels>(ilist);
                int dem = 0;
                foreach (var itempb in lstResult_phongban)
                {
                    lstResult_Year.Add(new KpiReport_YearModels { hovaten = itempb.tenphongban, group = "1", stt = (dem++).ToString() });
                    var lstdulieutontai = lstResult_Year.Where(p => p.maphongban == itempb.maphongban.Trim()).ToList();
                    if (lstdulieutontai.Count > 0)
                    {
                        string manhanvien = "";
                        foreach (var item in lstdulieutontai)
                        {
                            if (manhanvien.Trim() != item.manhanvien.Trim())
                            {
                                manhanvien = item.manhanvien;
                                double tongdiem = 0;

                                #region Dữ liệu tháng 01
                                var dulieu = lstdulieutontai.Where(p => p.thangkpi == "01").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi01 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi01 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi01 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi01 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi01 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi01 = "D";
                                }
                                else
                                {
                                    item.diemkpi01 = "X";
                                    item.xeploaikpi01 = "X";
                                }
                                #endregion

                                #region Dữ liệu tháng 02
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "02").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi02 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi02 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi02 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi02 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi02 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi02 = "D";
                                }
                                else
                                {
                                    item.diemkpi01 = "X";
                                    item.xeploaikpi01 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 03
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "03").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi03 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi03 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi03 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi03 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi03 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi03 = "D";
                                }
                                else
                                {
                                    item.diemkpi03 = "X";
                                    item.xeploaikpi03 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 04
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "04").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi04 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi04 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi04 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi04 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi04 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi04 = "D";
                                }
                                else
                                {
                                    item.diemkpi04 = "X";
                                    item.xeploaikpi04 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 05
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "05").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi05 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi05 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi05 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi05 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi05 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi05 = "D";
                                }
                                else
                                {
                                    item.diemkpi05 = "X";
                                    item.xeploaikpi05 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 06
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "06").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi06 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi06 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi06 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi06 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi06 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi06 = "D";
                                }
                                else
                                {
                                    item.diemkpi06 = "X";
                                    item.xeploaikpi06 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 07
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "07").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi07 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi07 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi07 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi07 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi07 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi07 = "D";
                                }
                                else
                                {
                                    item.diemkpi07 = "X";
                                    item.xeploaikpi07 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 08
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "08").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi08 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi08 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi08 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi08 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi08 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi08 = "D";
                                }
                                else
                                {
                                    item.diemkpi08 = "X";
                                    item.xeploaikpi08 = "X";
                                }
                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 09
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "09").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi09 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi09 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi09 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi09 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi09 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi09 = "D";
                                }
                                else
                                {
                                    item.diemkpi09 = "X";
                                    item.xeploaikpi09 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 10
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "10").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi10 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi10 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi10 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi10 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi10 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi10 = "D";
                                }
                                else
                                {
                                    item.diemkpi10 = "X";
                                    item.xeploaikpi10 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 11
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "11").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi11 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi11 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi11 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi11 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi11 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi11 = "D";
                                }
                                else
                                {
                                    item.diemkpi11 = "X";
                                    item.xeploaikpi11 = "X";
                                }

                                lstResult_Year.Add(item);
                                #endregion

                                #region Dữ liệu tháng 12
                                dulieu = lstdulieutontai.Where(p => p.thangkpi == "12").ToList();
                                if (dulieu.Count > 0)
                                {
                                    item.diemkpi12 = dulieu[0].ketqua;
                                    double xeploai = double.Parse(dulieu[0].ketqua.Replace("%", ""));
                                    tongdiem = tongdiem + xeploai;
                                    if (xeploai >= 100) item.xeploaikpi12 = "A+";
                                    else if (xeploai > 90 && xeploai < 100) item.xeploaikpi12 = "A";
                                    else if (xeploai > 80 && xeploai <= 90) item.xeploaikpi12 = "B";
                                    else if (xeploai > 70 && xeploai <= 80) item.xeploaikpi12 = "C";
                                    else if (xeploai <= 70) item.xeploaikpi12 = "D";
                                }
                                else
                                {
                                    item.diemkpi12 = "X";
                                    item.xeploaikpi12 = "X";
                                }
                                #endregion

                                #region Dữ liệu cả năm

                                item.diemkpinam = (tongdiem / 12).ToString();
                                double xeploaicuoinam = tongdiem / 12;
                                if (xeploaicuoinam >= 100) item.xeploaikpinam = "A+";
                                else if (xeploaicuoinam > 90 && xeploaicuoinam < 100) item.xeploaikpinam = "A";
                                else if (xeploaicuoinam > 80 && xeploaicuoinam <= 90) item.xeploaikpinam = "B";
                                else if (xeploaicuoinam > 70 && xeploaicuoinam <= 80) item.xeploaikpinam = "C";
                                else if (xeploaicuoinam <= 70) item.xeploaikpinam = "D";
                                #endregion

                                lstResult_Year.Add(item);
                            }
                        }
                    }
                }
            }
            logger.End("SelectRow_ReportKpi_Year");
            return lstResult_YearNew;
        }

       

        #endregion






















































        public List<ddns_tiendo_giatriModels> SelectRows_ddns_tiendo_giatri_hieuchinh(string mabosungnhansu)
        {
            logger.Start("SelectRows_ddns_tiendo_giatri");
            List<ddns_tiendo_giatriModels> lstResult = new List<ddns_tiendo_giatriModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("AddStaff.SelectRows_ddns_tiendo_giatri_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_tiendo_giatriModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_tiendo_giatri");
            return lstResult;
        }

        public List<ddns_congtruong_kehoachbosungnhansuModels> SelectRows_ddns_congtruong_kehoachbosungnhansu_hieuchinh(string mabosungnhansu)
        {
            logger.Start("SelectRows_ddns_congtruong_kehoachbosungnhansu_hieuchinh");
            List<ddns_congtruong_kehoachbosungnhansuModels> lstResult = new List<ddns_congtruong_kehoachbosungnhansuModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("AddStaff.SelectRows_ddns_congtruong_kehoachbosungnhansu_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_congtruong_kehoachbosungnhansuModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_congtruong_kehoachbosungnhansu_hieuchinh");
            return lstResult;
        }

        public bool UpdateRow_Giamdocduan_duyet_khongduyet(string mabosungnhansu, string dongy)
        {
            logger.Start("UpdateRow_Giamdocduan_duyet_khongduyet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                param["giamdocduan_ptgd_duyet"] = dongy;
                param["giamdocduan_ptgd_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("AddStaff.UpdateRow_Giamdocduan_duyet_khongduyet", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Giamdocduan_duyet_khongduyet");
            return strResult;
        }

        public bool UpdateRow_Quantringuonnhanluc_duyet_khongduyet(string mabosungnhansu, string dongy)
        {
            logger.Start("UpdateRow_Quantringuonnhanluc_duyet_khongduyet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                param["phongqtnnl_duyet"] = dongy;
                param["phongqtnnl_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("AddStaff.UpdateRow_Quantringuonnhanluc_duyet_khongduyet", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Quantringuonnhanluc_duyet_khongduyet");
            return strResult;
        }

        public bool UpdateRow_Bantonggiamdoc_duyet_khongduyet(string mabosungnhansu, string dongy)
        {
            logger.Start("UpdateRow_Bantonggiamdoc_duyet_khongduyet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                param["bantonggiamdoc_duyet"] = dongy;
                param["bantonggiamdoc_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("AddStaff.UpdateRow_Bantonggiamdoc_duyet_khongduyet", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Bantonggiamdoc_duyet_khongduyet");
            return strResult;
        }

        // LẬP DANH SÁCH DIỀU ĐỘNG NHÂN SỰ

        public int CountRow_Lapdanhsachdieudong(AddStaffModels clparam)
        {
            logger.Start("CountRow_Lapdanhsachdieudong");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                iResult = (int)sqlMap.ExecuteQueryForObject("Listchange.CountRow_Lapdanhsachdieudong", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRow_Lapdanhsachdieudong");
            return iResult;
        }

        public List<AddStaffModels> SelectRow_Lapdanhsachdieudong(AddStaffModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_Lapdanhsachdieudong");
            List<AddStaffModels> lstResult = new List<AddStaffModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRow_Lapdanhsachdieudong", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<AddStaffModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_Lapdanhsachdieudong");
            return lstResult;
        }

        public List<ddns_lapdanhsachdieudongModels> SelectRows_ddns_lapdanhsachdieudong_taomoi(string mabosungnhansu)
        {
            logger.Start("SelectRows_ddns_lapdanhsachdieudong_taomoi");
            List<ddns_lapdanhsachdieudongModels> lstResult = new List<ddns_lapdanhsachdieudongModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRows_ddns_lapdanhsachdieudong_taomoi", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_lapdanhsachdieudongModels>(ilist);
                //if (lstResult.Count == 0)
                //{
                //    lstResult.Add(new ddns_lapdanhsachdieudongModels { madanhsach = 0, mabosungnhansu = 0,noilamviec_cu="0",noilamviec_moi="0",chucdanh_cu="0",chucdanh_moi="0" });
                //}
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_lapdanhsachdieudong_taomoi");
            return lstResult;
        }

        public List<ddns_lapdanhsachdieudongModels> SelectRows_ddns_lapdanhsachdieudong_taomoi_timkiem(string thongtinnhansu)
        {
            logger.Start("SelectRows_ddns_lapdanhsachdieudong_taomoi_timkiem");
            List<ddns_lapdanhsachdieudongModels> lstResult = new List<ddns_lapdanhsachdieudongModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["thongtinnhansu"] = thongtinnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRows_ddns_lapdanhsachdieudong_taomoi_timkiem", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_lapdanhsachdieudongModels>(ilist);
                foreach (var item in lstResult)
                {
                    item.madanhsach = 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_lapdanhsachdieudong_taomoi_timkiem");
            return lstResult;
        }

        public List<ddns_lapdanhsachdieudongModels> SelectRows_ddns_lapdanhsachdieudong_thongtinlienlac(string mabosungnhansu)
        {
            logger.Start("SelectRows_ddns_lapdanhsachdieudong_thongtinlienlac");
            List<ddns_lapdanhsachdieudongModels> lstResult = new List<ddns_lapdanhsachdieudongModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRows_ddns_lapdanhsachdieudong_thongtinlienlac", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_lapdanhsachdieudongModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_lapdanhsachdieudong_thongtinlienlac");
            return lstResult;
        }

        public string Save_ddns_lapdanhsachdieudong(string DataJson, string nguoitao,string daguidsdd)
        {
            logger.Start("Save_ddns_lapdanhsachdieudong");
            string mabosungnhansu = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);
                mabosungnhansu = json["data1"]["mabosungnhansu"].ToString();
                param["mabosungnhansu"] = mabosungnhansu;
                param["ngaylapdanhsachdieudong"] = DateTime.Now.ToString("dd/MM/yyyy");

                param["nguoilap_dieudong"] = json["data1"]["nguoilap_dieudong"].ToString();
                param["nguoilapemail_dieudong"] = json["data1"]["nguoilapemail_dieudong"].ToString();
                param["nguoilap_dieudong_duyet"] = daguidsdd;

                param["bangiamdoc_dieudong"] = json["data1"]["bangiamdoc_dieudong"].ToString();
                param["bangiamdocemail_dieudong"] = json["data1"]["bangiamdocemail_dieudong"].ToString();
                if (daguidsdd == "1")
                {
                    param["bangiamdoc_dieudong_duyet"] = "1";
                    param["bangiamdoc_dieudong_ngayduyet"] = DateTime.Now.ToString("dd/MM/yyyy");
                }

                param["nguoitao"] = nguoitao;
                sqlMap.Update("Listchange.UpdateRow_ddns_addstaff_lapdsdieudong", param);
                Hashtable param1 = new Hashtable();
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param1 = new Hashtable();
                    param1["mabosungnhansu"] = param["mabosungnhansu"].ToString().Trim();
                    param1["manhanvien"] = json_tiendo_giatri[i]["manhanvien"].ToString().Trim();
                    param1["hovaten"] = json_tiendo_giatri[i]["hovaten"].ToString().Trim();
                    param1["noilamviec_cu"] = json_tiendo_giatri[i]["noilamviec_cu"].ToString().Trim();
                    param1["chucdanh_cu"] = json_tiendo_giatri[i]["chucdanh_cu"].ToString().Trim();
                    param1["noilamviec_moi"] = json_tiendo_giatri[i]["noilamviec_moi"].ToString().Trim();
                    param1["chucdanh_moi"] = json_tiendo_giatri[i]["chucdanh_moi"].ToString().Trim();

                    try
                    {
                        param1["dienthoai"] = json_tiendo_giatri[i]["dienthoai"].ToString().Trim();
                        param1["email"] = json_tiendo_giatri[i]["email"].ToString().Trim();
                    }
                    catch (Exception)
                    {
                        param1["dienthoai"] = "";
                        param1["email"] = "";
                    }

                    param1["ngaydieudongdukien"] = json_tiendo_giatri[i]["ngaydieudongdukien"].ToString().Trim();
                    param1["ghichu"] = json_tiendo_giatri[i]["ghichu"].ToString().Trim();

                    param1["vungmien"] = json_tiendo_giatri[i]["vungmien"].ToString().Trim();

                    param1["thongtinlienlac"] = json_tiendo_giatri[i]["thongtinlienlac"].ToString().Trim();

                    if (json_tiendo_giatri[i]["vungmien"].ToString() == "true") 
                        param1["vungmien"] = "1";
                    else param1["vungmien"] = "0";

                    param1["nguoitao"] = nguoitao;
                    if (json_tiendo_giatri[i]["madanhsach"].ToString() == "" || json_tiendo_giatri[i]["madanhsach"].ToString() == "0" || json_tiendo_giatri[i]["madanhsach"].ToString() == null)
                    {
                        sqlMap.Insert("Listchange.InsertRow_ddns_lapdanhsachdieudong", param1);
                    }
                    else
                    {
                        param1["madanhsach"] = json_tiendo_giatri[i]["madanhsach"].ToString().Trim();
                        sqlMap.Update("Listchange.UpdateRow_ddns_lapdanhsachdieudong", param1);
                    }
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_ddns_lapdanhsachdieudong");
            return mabosungnhansu;
        }

        public bool UpdateRow_Duyetky_quyetdinh_dieudong(string mabosungnhansu, string dongy)
        {
            logger.Start("UpdateRow_Duyetky_quyetdinh_dieudong");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                param["bangiamdoc_dieudong_duyet"] = dongy;
                param["bangiamdoc_dieudong_ngayduyet"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("Listchange.UpdateRow_Duyetky_quyetdinh_dieudong", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Duyetky_quyetdinh_dieudong");
            return strResult;
        }

        public bool Save_chuyenhoso_congtruong_phongban(string madanhsach)
        {
            logger.Start("Save_chuyenhoso_congtruong_phongban");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                
                param["chuyenracongtruong"] = "1";

                JObject json = JObject.Parse(madanhsach);
                // string adb = json_tiendo_giatri[0]["matiendo"].ToString().Trim();

                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param["madanhsach"] = json_tiendo_giatri[i]["madanhsach"].ToString().Trim();
                    sqlMap.Update("Listchange.UpdateRow_chuyenhoso_congtruong_phongban", param);
                }

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Save_chuyenhoso_congtruong_phongban");
            return strResult;
        }

        public List<ddns_lapdanhsach_dieudong_thongbaoModels> SelectRows_ddns_lapdanhsach_dieudong_thongbao(string madanhsach)
        {
            logger.Start("SelectRows_ddns_lapdanhsach_dieudong_thongbao");
            List<ddns_lapdanhsach_dieudong_thongbaoModels> lstResult = new List<ddns_lapdanhsach_dieudong_thongbaoModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["madanhsach"] = madanhsach;
                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRows_ddns_lapdanhsach_dieudong_thongbao", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_lapdanhsach_dieudong_thongbaoModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_lapdanhsach_dieudong_thongbao");
            return lstResult;
        }

        #region DANH SÁCH NHÂN SỰ ĐẾN CÔNG TRƯỜNG PHÒNG BAN LÀM VIỆC

        public int CountRow_Danhsachdenlamviec(string noilamviec_moi)
        {
            logger.Start("CountRow_Danhsachdenlamviec");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                if (noilamviec_moi!=null&&noilamviec_moi!="0"&&noilamviec_moi!="")
                    param["noilamviec_moi"] = noilamviec_moi.Trim();
                else param["noilamviec_moi"] = "";
                iResult = (int)sqlMap.ExecuteQueryForObject("Listchange.CountRow_Danhsachdenlamviec", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRow_Danhsachdenlamviec");
            return iResult;
        }

        public List<ddns_lapdanhsach_dieudong_thongbaoModels> SelectRow_Danhsachdenlamviec(ddns_lapdanhsach_dieudong_thongbaoModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_Danhsachdenlamviec");
            List<ddns_lapdanhsach_dieudong_thongbaoModels> lstResult = new List<ddns_lapdanhsach_dieudong_thongbaoModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRow_Danhsachdenlamviec", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_lapdanhsach_dieudong_thongbaoModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_Danhsachdenlamviec");
            return lstResult;
        }

        public bool Save_employee_worker(string madanhsach)
        {
            logger.Start("Save_chuyenhoso_congtruong_phongban");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["chuyenracongtruong"] = "1";
                JObject json = JObject.Parse(madanhsach);
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param["madanhsach"] = json_tiendo_giatri[i]["madanhsach"].ToString().Trim();
                    param["nhansuden_ct_pb"] = "1";

                    if (json_tiendo_giatri[i]["nhansuden_ct_pb_ngayden"].ToString() == "" || json_tiendo_giatri[i]["nhansuden_ct_pb_ngayden"].ToString() == null || json_tiendo_giatri[i]["nhansuden_ct_pb_ngayden"].ToString() == "NULL")
                        param["nhansuden_ct_pb_ngayden"] = DateTime.Now.ToString("dd/MM/yyyy");
                    else
                        param["nhansuden_ct_pb_ngayden"] = json_tiendo_giatri[i]["nhansuden_ct_pb_ngayden"].ToString().Trim();
                    sqlMap.Update("Listchange.UpdateRow_employee_worker", param);
                }

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Save_chuyenhoso_congtruong_phongban");
            return strResult;
        }

        #endregion

        #region Thêm mới bổ sung nhân sự văn phòng

        public List<ddns_vanphong_kehoachbosungnhansuModels> SelectRows_ddns_vanphong_kehoachbosungnhansu_hieuchinh(string mabosungnhansu)
        {
            logger.Start("SelectRows_ddns_vanphong_kehoachbosungnhansu_hieuchinh");
            List<ddns_vanphong_kehoachbosungnhansuModels> lstResult = new List<ddns_vanphong_kehoachbosungnhansuModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["mabosungnhansu"] = mabosungnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("AddStaff.SelectRows_ddns_vanphong_kehoachbosungnhansu_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_vanphong_kehoachbosungnhansuModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_vanphong_kehoachbosungnhansu_hieuchinh");
            return lstResult;
        }


        public string Save_bosung_vanphong(string DataJson, string nguoitao, string chtdaduyet)
        {
            logger.Start("Save_bosung_vanphong");
            string mabosungnhansu = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);

                mabosungnhansu = json["data1"]["mabosungnhansu"].ToString();
                param["tenduan"] = json["data1"]["tenduan"].ToString();
                param["goithau"] = json["data1"]["goithau"].ToString();
              
                param["ngayyeucau"] = json["data1"]["ngayyeucau"].ToString();
                try
                {
                    if (int.Parse(json["data1"]["soluongnhansuhientai"].ToString()) > 0)
                        param["soluongnhansuhientai"] = json["data1"]["soluongnhansuhientai"].ToString();
                    else param["soluongnhansuhientai"] = "0";
                }
                catch (Exception) { param["soluongnhansuhientai"] = "0"; }
                
                param["thuyenchuyennoibo"] = json["data1"]["thuyenchuyennoibo"].ToString();
                param["tuyenmoi"] = json["data1"]["tuyenmoi"].ToString();
                param["khongbosung"] = json["data1"]["khongbosung"].ToString();

                param["truongbophan_cht"] = json["data1"]["truongbophan_cht"].ToString();
                param["truongbophan_cht_email"] = json["data1"]["truongbophan_cht_email"].ToString();
                param["truongbophan_cht_duyet"] = chtdaduyet;
                param["truongbophan_cht_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");

                param["giamdocduan_ptgd"] = json["data1"]["giamdocduan_ptgd"].ToString();
                param["giamdocduan_ptgd_email"] = json["data1"]["giamdocduan_ptgd_email"].ToString();
                param["giamdocduan_ptgd_duyet"] = "0";

                param["phongqtnnl"] = json["data1"]["phongqtnnl"].ToString();
                param["phongqtnnl_email"] = json["data1"]["phongqtnnl_email"].ToString();
                param["phongqtnnl_duyet"] = "0";

                param["bantonggiamdoc"] = json["data1"]["bantonggiamdoc"].ToString();
                param["bantonggiamdoc_email"] = json["data1"]["bantonggiamdoc_email"].ToString();
                param["bantonggiamdoc_duyet"] = "0";

                param["phongban_congtruong"] = json["data1"]["phongban_congtruong"].ToString();


                param["nguoitao"] = nguoitao;

                if (mabosungnhansu.Trim() == "" || mabosungnhansu.Trim() == "0" || mabosungnhansu.Trim() == null)
                {
                    param["mabosungnhansu"] = GetSequence_All("dm_seq", "ddns_addstaff");
                    param["mahoso"] = int.Parse(param["mabosungnhansu"].ToString().Trim()).ToString("000000");
                    sqlMap.Insert("AddStaff.InsertRow_addstaff", param);
                }
                else
                {
                    param["mabosungnhansu"] = mabosungnhansu;
                    sqlMap.Update("AddStaff.UpdateRow_ddns_addstaff", param);
                    sqlMap.Update("AddStaff.Deleted_ddns_vanphong_kehoachbosungnhansu", param);
                }
                mabosungnhansu = param["mabosungnhansu"].ToString();
                Hashtable param1 = new Hashtable();
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param1 = new Hashtable();
                    param1["mabosungnhansu"] = param["mabosungnhansu"].ToString().Trim();
                    param1["makehoach"] = json_tiendo_giatri[i]["makehoach"].ToString().Trim();

                    param1["vitricongtac"] = json_tiendo_giatri[i]["vitricongtac"].ToString().Trim();
                    try {
                        param1["soluong"] = int.Parse(json_tiendo_giatri[i]["soluong"].ToString().Trim());
                    }
                    catch (Exception) { param1["soluong"] = "1"; }

                    param1["chuyenmon"] = json_tiendo_giatri[i]["chuyenmon"].ToString().Trim();
                    param1["trinhdo"] = json_tiendo_giatri[i]["trinhdo"].ToString().Trim();
                    param1["thoigiantiepnhan"] = json_tiendo_giatri[i]["thoigiantiepnhan"].ToString().Trim();
                    param1["tieuchuan_ghichu"] = json_tiendo_giatri[i]["tieuchuan_ghichu"].ToString().Trim();
                    param1["nguoitao"] = nguoitao;
                    sqlMap.Insert("AddStaff.InsertRow_ddns_vanphong_kehoachbosungnhansu", param1);
                }

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_bosung_vanphong");
            return mabosungnhansu;
        }

        #endregion





        #region CHUYỂN TRẢ NHÂN SỰ DỰ ÁN

        public int CountRows_Denghichuyentranhansu(ddns_paystaffModels clparam)
        {
            logger.Start("CountRows_Denghichuyentranhansu");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                iResult = (int)sqlMap.ExecuteQueryForObject("PayStaff.CountRows_Denghichuyentranhansu", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_Denghichuyentranhansu");
            return iResult;
        }

        public List<ddns_paystaffModels> SelectRow_Denghichuyentranhansu(ddns_paystaffModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_Denghibosungnhansu");
            List<ddns_paystaffModels> lstResult = new List<ddns_paystaffModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("PayStaff.SelectRow_Denghichuyentranhansu", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_paystaffModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_Denghibosungnhansu");
            return lstResult;
        }


        public List<AddStaffModels> SelectRow_Laydanhsach_nsu_ctruong_loadcombo(string phongban_congtruong)
        {
            logger.Start("SelectRow_Laydanhsach_nsu_ctruong_loadcombo");
            List<AddStaffModels> lstResult = new List<AddStaffModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["phongban_congtruong"] = phongban_congtruong;
                IList ilist = sqlMap.ExecuteQueryForList("PayStaff.SelectRow_ds_nhansu_nsu_ctruong", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<AddStaffModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_Laydanhsach_nsu_ctruong_loadcombo");
            return lstResult;
        }

        public string Save_paystaff(string DataJson, string nguoitao, string chtdaduyet)
        {
            logger.Start("Save_paystaff");
            string matranhansu = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);

                matranhansu = json["data1"]["matranhansu"].ToString();
                param["matranhansu"] = json["data1"]["matranhansu"].ToString();
                param["mabosungnhansu"] = json["data1"]["mabosungnhansu"].ToString();
                param["tenduan"] = json["data1"]["tenduan"].ToString();
                param["goithau"] = json["data1"]["goithau"].ToString();
                param["diachi"] = json["data1"]["diachi"].ToString();
                param["ngayyeucau"] = json["data1"]["ngayyeucau"].ToString();

                if (json["data1"]["congnghiep"].ToString() == "on")
                    param["congnghiep"] = "1";
                else param["congnghiep"] = "0";

                if (json["data1"]["thuongmai"].ToString() == "on")
                    param["thuongmai"] = "1";
                else param["thuongmai"] = "0";

                if (json["data1"]["dandung"].ToString() == "on")
                    param["dandung"] = "1";
                else param["dandung"] = "0";

                if (json["data1"]["nghiduong"].ToString() == "on")
                    param["nghiduong"] = "1";
                else param["nghiduong"] = "0";

                if (json["data1"]["hatang"].ToString() == "on")
                    param["hatang"] = "1";
                else param["hatang"] = "0";

                param["khac_noidung"] = json["data1"]["khac_noidung"].ToString();
                if (param["khac_noidung"].ToString().Trim() != null && param["khac_noidung"].ToString().Trim() != "" && param["khac_noidung"].ToString().Trim() != "undefined")
                {
                    param["khac"] = "1";
                    param["khac_noidung"] = json["data1"]["khac_noidung"].ToString();
                }
                else
                {
                    param["khac"] = "0";
                    param["khac_noidung"] = "";
                }

                param["tiendovagiatrithang"] = json["data1"]["tiendovagiatrithang"].ToString();
                param["ngaykhoicong"] = json["data1"]["ngaykhoicong"].ToString();
                param["ngayhoanthanh"] = json["data1"]["ngayhoanthanh"].ToString();

                param["dieudongdiduankhac"] = json["data1"]["dieudongdiduankhac"].ToString();
                param["dieudongvephongban"] = json["data1"]["dieudongvephongban"].ToString();
                param["dexuatchonghiviec"] = json["data1"]["dexuatchonghiviec"].ToString();


                param["truongbophan_cht"] = json["data1"]["truongbophan_cht"].ToString();
                param["truongbophan_cht_email"] = json["data1"]["truongbophan_cht_email"].ToString();
                param["truongbophan_cht_duyet"] = chtdaduyet;
                param["truongbophan_cht_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");

                param["giamdocduan_ptgd"] = json["data1"]["giamdocduan_ptgd"].ToString();
                param["giamdocduan_ptgd_email"] = json["data1"]["giamdocduan_ptgd_email"].ToString();
                param["giamdocduan_ptgd_duyet"] = "0";

                param["phongqtnnl"] = json["data1"]["phongqtnnl"].ToString();
                param["phongqtnnl_email"] = json["data1"]["phongqtnnl_email"].ToString();
                param["phongqtnnl_duyet"] = "0";


                param["phongban_congtruong"] = json["data1"]["phongban_congtruong"].ToString();
                param["soluongnhansuhientai"] = "0";

                param["nguoitao"] = nguoitao;

                if (matranhansu.Trim() == "" || matranhansu.Trim() == "0" || matranhansu.Trim() == null)
                {
                    param["matranhansu"] = GetSequence_All("dm_seq", "ddns_paystaff");
                    param["mahoso"] = int.Parse(param["matranhansu"].ToString().Trim()).ToString("000000");
                    sqlMap.Insert("PayStaff.InsertRow_ddns_paystaff", param);
                }
                else
                {
                    param["matranhansu"] = matranhansu;
                    sqlMap.Update("PayStaff.UpdateRow_ddns_paystaff", param);
                    sqlMap.Update("PayStaff.Deleted_ddns_paystaff_congtruong_kehoachbosungnhansu", param);
                }
                matranhansu = param["matranhansu"].ToString();
                Hashtable param1 = new Hashtable();
                JArray json_congtruong = (JArray)json["data2"];
                for (int i = 0; i < json_congtruong.Count(); i++)
                {
                    param1 = new Hashtable();
                    param1["matranhansu"] = param["matranhansu"].ToString().Trim();
                    param1["makehoach"] = json_congtruong[i]["makehoach"].ToString().Trim();
                    param1["vitricongtac"] = json_congtruong[i]["vitricongtac"].ToString().Trim();

                    try
                    {
                        param1["soluong"] = int.Parse(json_congtruong[i]["soluong"].ToString().Trim());
                    }
                    catch (Exception) { param1["soluong"] = "0"; }
                    param1["thoigianchuyentra"] = json_congtruong[i]["thoigianchuyentra"].ToString().Trim();

                    try
                    {
                        param1["soluong1"] = int.Parse(json_congtruong[i]["soluong1"].ToString().Trim());
                    }
                    catch (Exception) { param1["soluong1"] = "0"; }
                    param1["thoigianchuyentra1"] = json_congtruong[i]["thoigianchuyentra1"].ToString().Trim();

                    try
                    {
                        param1["soluong2"] = int.Parse(json_congtruong[i]["soluong2"].ToString().Trim());
                    }
                    catch (Exception) { param1["soluong2"] = "0"; }
                    param1["thoigianchuyentra2"] = json_congtruong[i]["thoigianchuyentra2"].ToString().Trim();

                    param1["ghichu"] = json_congtruong[i]["ghichu"].ToString().Trim();

                    param1["nguoitao"] = nguoitao;
                    //if (param1["makehoach"].ToString() == "" || param1["makehoach"].ToString() == "0" || param1["makehoach"].ToString() == null)
                    //{
                        sqlMap.Insert("PayStaff.InsertRow_ddns_paystaff_congtruong_kehoachbosungnhansu", param1);
                    //}
                    //else
                    //{
                    //    sqlMap.Update("PayStaff.UpdateRow_ddns_paystaff_congtruong_kehoachbosungnhansu", param1);
                    //}
                }

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
                matranhansu = "0";
            }
            logger.End("Save_paystaff");
            return matranhansu;
        }

        public bool UpdateRow_Chuyentra_Giamdocduan_duyet_khongduyet(string matranhansu, string dongy)
        {
            logger.Start("UpdateRow_Chuyentra_Giamdocduan_duyet_khongduyet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["matranhansu"] = matranhansu;
                param["giamdocduan_ptgd_duyet"] = dongy;
                param["giamdocduan_ptgd_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("PayStaff.UpdateRow_Chuyentra_Giamdocduan_duyet_khongduyet", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Chuyentra_Giamdocduan_duyet_khongduyet");
            return strResult;
        }

        public bool UpdateRow_Chuyentra_Quantringuonnhanluc_duyet_khongduyet(string matranhansu, string dongy)
        {
            logger.Start("UpdateRow_Chuyentra_Quantringuonnhanluc_duyet_khongduyet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["matranhansu"] = matranhansu;
                param["phongqtnnl_duyet"] = dongy;
                param["phongqtnnl_ngayky"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("PayStaff.UpdateRow_Chuyentra_Quantringuonnhanluc_duyet_khongduyet", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Chuyentra_Quantringuonnhanluc_duyet_khongduyet");
            return strResult;
        }

        public List<ddns_paystaffModels> SelectRows_Denghi_chuyentra_nhansu_hieuchinh(string matranhansu)
        {
            logger.Start("SelectRows_Denghi_chuyentra_nhansu_hieuchinh");
            List<ddns_paystaffModels> lstResult = new List<ddns_paystaffModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["matranhansu"] = matranhansu;
                IList ilist = sqlMap.ExecuteQueryForList("PayStaff.SelectRows_Denghi_chuyentra_nhansu_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_paystaffModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Denghi_chuyentra_nhansu_hieuchinh");
            return lstResult;
        }

        public List<ddns_paystaff_congtruong_kehoachbosungnhansuModels> SelectRows_ddns_paystaff_congtruong_hieuchinh(string matranhansu)
        {
            logger.Start("SelectRows_ddns_paystaff_congtruong_hieuchinh");
            List<ddns_paystaff_congtruong_kehoachbosungnhansuModels> lstResult = new List<ddns_paystaff_congtruong_kehoachbosungnhansuModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["matranhansu"] = matranhansu;
                IList ilist = sqlMap.ExecuteQueryForList("PayStaff.SelectRows_ddns_paystaff_congtruong_kehoachbosungnhansu_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_paystaff_congtruong_kehoachbosungnhansuModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_paystaff_congtruong_hieuchinh");
            return lstResult;
        }

        #endregion


        #region DANH SÁCH NHÂN SỰ CHUYỂN TRẢ VỀ PHÒNG BAN

        public int CountRow_Danhsachnhansutrave(string noilamviec_moi)
        {
            logger.Start("CountRow_Danhsachnhansutrave");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                if (noilamviec_moi != null && noilamviec_moi != "0" && noilamviec_moi != "")
                    param["noilamviec_moi"] = noilamviec_moi.Trim();
                else param["noilamviec_moi"] = "";
                iResult = (int)sqlMap.ExecuteQueryForObject("Listchange.CountRow_Danhsachnhansutrave", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRow_Danhsachnhansutrave");
            return iResult;
        }

        public List<ddns_lapdanhsach_dieudong_thongbaoModels> SelectRow_Danhsachnhansutrave(ddns_lapdanhsach_dieudong_thongbaoModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_Danhsachnhansutrave");
            List<ddns_lapdanhsach_dieudong_thongbaoModels> lstResult = new List<ddns_lapdanhsach_dieudong_thongbaoModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRow_Danhsachnhansutrave", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_lapdanhsach_dieudong_thongbaoModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_Danhsachnhansutrave");
            return lstResult;
        }

        public bool Save_Pay_employee_worker(string madanhsach)
        {
            logger.Start("Save_Pay_employee_worker");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["chuyenracongtruong"] = "1";
                JObject json = JObject.Parse(madanhsach);
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param["madanhsach"] = json_tiendo_giatri[i]["madanhsach"].ToString().Trim();
                    param["nhansuden_ct_pb"] = "2";
                    if (json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString() == "" || json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString() == null || json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString() == "NULL")
                        param["nhansuden_ct_pb_ngaytra"] = DateTime.Now.ToString("dd/MM/yyyy");
                    else
                        param["nhansuden_ct_pb_ngaytra"] = json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString().Trim();
                    sqlMap.Update("Listchange.UpdateRow_Pay_employee_worker", param);
                }

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Save_Pay_employee_worker");
            return strResult;
        }

        public bool Save_Pay_employe_PhongNS_hoantat(string madanhsach)
        {
            logger.Start("Save_Pay_employe_PhongNS_hoantat");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                //param["chuyenracongtruong"] = "1";
                JObject json = JObject.Parse(madanhsach);
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param["madanhsach"] = json_tiendo_giatri[i]["madanhsach"].ToString().Trim();
                    param["nhansuden_ct_pb"] = "3";
                    //if (json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString() == "" || json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString() == null || json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString() == "NULL")
                    //    param["nhansuden_ct_pb_ngaytra"] = DateTime.Now.ToString("dd/MM/yyyy");
                    //else
                    //    param["nhansuden_ct_pb_ngaytra"] = json_tiendo_giatri[i]["nhansuden_ct_pb_ngaytra"].ToString().Trim();
                    sqlMap.Update("Listchange.UpdateRow_Pay_employe_PhongNS_hoantat", param);
                }

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Save_Pay_employe_PhongNS_hoantat");
            return strResult;
        }


        public List<ddns_paystaff_vanphong_kehoachbosungnhansuModels> SelectRows_ddns_lapdanhsachchuyentravephongban_vephongban_taomoi_timkiem(string thongtinnhansu)
        {
            logger.Start("SelectRows_ddns_lapdanhsachdieudong_taomoi_timkiem");
            List<ddns_paystaff_vanphong_kehoachbosungnhansuModels> lstResult = new List<ddns_paystaff_vanphong_kehoachbosungnhansuModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["thongtinnhansu"] = thongtinnhansu;
                IList ilist = sqlMap.ExecuteQueryForList("Listchange.SelectRows_ddns_lapdanhsachchuyentravephongban_vephongban_taomoi_timkiem", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ddns_paystaff_vanphong_kehoachbosungnhansuModels>(ilist);
                foreach (var item in lstResult)
                {
                    item.makehoach = 0;
                }
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_ddns_lapdanhsachdieudong_taomoi_timkiem");
            return lstResult;
        }

        #endregion












































        //==============================PHAN NAY BO CHUA BIET CO LAY DU LIEU =================================/////////

        public List<DangkyvppModels> SelectRows_DS_Vanphongpham_xuatkho_hieuchinh(DangkyvppModels clParam)
        {
            logger.Start("SelectRows_DS_Vanphongpham_xuatkho_hieuchinh");
            List<DangkyvppModels> lstResult = new List<DangkyvppModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["nguoitao"].ToString() == "0" || param["nguoitao"].ToString() == "1")
                    param["nguoitao"] = "";
                IList ilist = sqlMap.ExecuteQueryForList("Dangkyvanphongpham.SelectRows_DS_Vanphongpham_xuatkho_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<DangkyvppModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_DS_Vanphongpham_xuatkho_hieuchinh");
            return lstResult;
        }


        public List<DangkyvppModels> SelectRows_DS_Vanphongpham_hieuchinh(DangkyvppModels clParam)
        {
            logger.Start("SelectRows_DS_Vanphongpham");
            List<DangkyvppModels> lstResult = new List<DangkyvppModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["madangky"].ToString().Trim() == "0")
                    param["madangky"] = "";
                IList ilist = sqlMap.ExecuteQueryForList("Dangkyvanphongpham.SelectRows_DS_Vanphongpham_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<DangkyvppModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_DS_Vanphongpham");
            return lstResult;
        }


        public List<Dangkyvpp_chitietModels> SelectRows_Dangkyvpp_chitiet(DangkyvppModels clParam)
        {
            logger.Start("SelectRows_DS_Vanphongpham");
            List<Dangkyvpp_chitietModels> lstResult = new List<Dangkyvpp_chitietModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                //if (param["madangky"].ToString().Trim() == "0")
                //    param["madangky"] = "";
                IList ilist = sqlMap.ExecuteQueryForList("Dangkyvanphongpham.SelectRows_Dangky_Vanphongpham_chitiet", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<Dangkyvpp_chitietModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_DS_Vanphongpham");
            return lstResult;
        }


        public List<Dangkyvpp_chitiet_xuatkhoModels> SelectRows_Dangkyvpp_chitiet_xuatkho(DangkyvppModels clParam)
        {
            logger.Start("SelectRows_Dangkyvpp_chitiet_xuatkho");
            List<Dangkyvpp_chitiet_xuatkhoModels> lstResult = new List<Dangkyvpp_chitiet_xuatkhoModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                //if (param["madangky"].ToString().Trim() == "0")
                //    param["madangky"] = "";
                IList ilist = sqlMap.ExecuteQueryForList("Xuatkho.SelectRows_Dangky_Vanphongpham_chitiet", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<Dangkyvpp_chitiet_xuatkhoModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Dangkyvpp_chitiet_xuatkho");
            return lstResult;
        }

        

        public List<Danhmuc_VanPhongPhamModels> SelectRows_Danhmuccha_vpp(Danhmuc_VanPhongPhamModels clParam)
        {
            logger.Start("SelectRows_Danhmuccha_vpp");
            List<Danhmuc_VanPhongPhamModels> lstResult = new List<Danhmuc_VanPhongPhamModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_vanphongpham_cha", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<Danhmuc_VanPhongPhamModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Danhmuccha_vpp");
            return lstResult;
        }

        public List<PhongBanModels>SelectRows(PhongBanModels clParam)
        {
            logger.Start("SelectRows");
            List<PhongBanModels> lstResult = new List<PhongBanModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<PhongBanModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows");
            return lstResult;
        }

        public List<ChucDanhModels> SelectRows_chucvu(ChucDanhModels clParam)
        {
            logger.Start("SelectRows_chucvu");
            List<ChucDanhModels> lstResult = new List<ChucDanhModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_chucvu", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ChucDanhModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_chucvu");
            return lstResult;
        }

        public int CountRows(PhongBanModels clparam)
        {
            logger.Start("CountRows");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                iResult = (int)sqlMap.ExecuteQueryForObject("Danhmuc.CountRows", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows");
            return iResult;
        }

        

        public bool DeleteRows(List<KeHoachModels> clParam)
        {
            logger.Start("DeleteRows");
            bool lstResult = false;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                for (int i = 0; i < clParam.Count; i++)
                {
                    param = new Hashtable();
                    param = base.SetDataToHashtable(false, clParam[i]);
                    sqlMap.Update("cv_kehoach.DeleteRow", param);
                }
                sqlMap.CommitTransaction();
                lstResult = true;
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("DeleteRows");
            return lstResult;
        }

        public string InsertRow_vanphongpham(Danhmuc_VanPhongPhamModels clParam)
        {
            logger.Start("InsertRow_vanphongpham");
            string strResult = "";
            try
            {
                Hashtable param = new Hashtable();
                strResult = GetSequence_All("dm_seq", "dm_vanphongpham");
                clParam.mavanphongpham = int.Parse(strResult);
                if (clParam.danhmuccha == "0")
                {
                    clParam.danhmuccha = strResult;
                    clParam.danhmucgoc = "0";
                }
                base.InsertData(clParam, "dm_vanphongpham");
            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("InsertRow_vanphongpham");
            return strResult;
        }

        public bool UpdateRow_vanphongpham(Danhmuc_VanPhongPhamModels clParam)
        {
            logger.Start("UpdateRow_vanphongpham");
            bool bResult = false;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();

                param["mavanphongpham"] = clParam.mavanphongpham;
                if (clParam.danhmuccha == "0")
                {
                    clParam.danhmuccha = clParam.mavanphongpham.ToString();
                    clParam.danhmucgoc = "0";
                }
                else 
                {
                    clParam.danhmuccha = clParam.danhmuccha.ToString();
                    clParam.danhmucgoc = "";
                }
                param["tenvanphongpham"] = clParam.tenvanphongpham;
                param["dongia"] = clParam.dongia;
                param["donvitinh"] = clParam.donvitinh;
                param["danhmuccha"] = clParam.danhmuccha;
                param["danhmucgoc"] = clParam.danhmucgoc;
                param["ghichu"] = clParam.ghichu;
                param["xoa"] = "0";
                param["nguoihieuchinh"] = clParam.nguoihieuchinh;

                sqlMap.Update("Danhmuc.UpdateRow_Vanphongpham", param);
                sqlMap.CommitTransaction();
                bResult = true;
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                bResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_vanphongpham");
            return bResult;
        }

        public string InsertRow_chucdanh(ChucDanhModels clParam)
        {
            logger.Start("InsertRow_chucdanh");
            string strResult = "";
            try
            {
                Hashtable param = new Hashtable();
                strResult = GetSequence_All("dm_seq", "m_donvi_chucdanh");
                clParam.machucdanh = int.Parse(strResult);
                base.InsertData(clParam, "m_donvi_chucdanh");
            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("InsertRow_chucdanh");
            return strResult;
        }

        public bool UpdateRow_chucdanh(ChucDanhModels clParam)
        {
            logger.Start("UpdateRow_vanphongpham");
            bool bResult = false;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["machucdanh"] = clParam.machucdanh;
                param["tenchucdanh"] = clParam.tenchucdanh;
                param["tengiaodich"] = clParam.tengiaodich;
                param["ghichu"] = clParam.ghichu;
                param["xoa"] = "0";
                param["nguoihieuchinh"] = clParam.nguoihieuchinh;
                sqlMap.Update("Danhmuc.UpdateRow_chucdanh", param);
                sqlMap.CommitTransaction();
                bResult = true;
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                bResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_vanphongpham");
            return bResult;
        }

       


        public bool DeletedRow_Dangky_vpp(string madangky, string userid)
        {
            logger.Start("DeletedRow_Dangky_vpp");
            bool strResult = true;
            try
            {
                Hashtable param = new Hashtable();
                param["madangky"] = madangky;
                param["nguoitao"] = userid;
                sqlMap.Update("Dangkyvanphongpham.DeletedRow_Dangky_vpp", param);
                sqlMap.Update("Dangkyvanphongpham.DeletedRow_Dangky_vpp_chitiet", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("DeletedRow_Dangky_vpp");
            return strResult;
        }


    }
}