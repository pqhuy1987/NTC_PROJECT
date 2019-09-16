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
    public class KpiConstructionServices : BaseData
    {
        public KpiConstructionServices()
        {
            logger = new Log4Net(typeof(KpiConstructionServices));
        }

        #region KPI NHÂN VIÊN RICONS ban chi huy cong truong

        public List<KpiLevelDepartmentDetailModels> SelectRows_KpiLevelDepartmentDetail(string maphongban)
        {
            logger.Start("SelectRows_KpiLevelDepartmentDetail_hieuchinh");
            List<KpiLevelDepartmentDetailModels> lstResult = new List<KpiLevelDepartmentDetailModels>();
            try
            {
                Hashtable param = new Hashtable();
                //param["makpiphongban"] = makpicongty;
                IList ilist = sqlMap.ExecuteQueryForList("KpiLevelDepartment.SelectRows_KpiLevelDepartmentDetail_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<KpiLevelDepartmentDetailModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_KpiLevelDepartmentDetail_hieuchinh");
            return lstResult;
        }


        public List<KpiEmployeeModels> SelectRow_KpiEmployee_XuatExcel(string maphongban)
        {
            logger.Start("SelectRow_KpiEmployee_XuatExcel");
            List<KpiEmployeeModels> lstResult = new List<KpiEmployeeModels>();
            try
            {
                Hashtable param = new Hashtable();
                if (maphongban == "0") param["maphongban"] = "";
                else param["maphongban"] = maphongban;
                IList ilist = sqlMap.ExecuteQueryForList("KpiConstruction.SelectRow_KpiEmployee_XuatExcel", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<KpiEmployeeModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_KpiEmployee_XuatExcel");
            return lstResult;
        }

        public int CountRows_KpiEmployee(KpiEmployeeModels clparam)
        {
            logger.Start("CountRows_KpiEmployee");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (clparam.trinhtranghoso == "" || clparam.trinhtranghoso == null || clparam.trinhtranghoso == "undefined") param["tinhtranghoso"] = "";
                else param["tinhtranghoso"] = clparam.trinhtranghoso;

                if (clparam.hovaten == "" || clparam.hovaten == null || clparam.hovaten == "undefined")
                    param["hovaten"] = "";
                if (clparam.chucdanh == "1")
                {
                    param["nguoitao"] = clparam.nguoitao;
                    param["maphongban"] = clparam.maphongban;
                    if (clparam.maphongban == "0") param["maphongban"] = "";
                    else param["maphongban"] = clparam.maphongban;
                }
                else
                {
                    param["nguoitao"] = "";
                    if (clparam.maphongban == "0") param["maphongban"] = "";
                    else param["maphongban"] = clparam.maphongban;
                }
                if (clparam.xoa == "1")
                {
                    param["nguoitao"] = "";
                }
                iResult = (int)sqlMap.ExecuteQueryForObject("KpiConstruction.CountRows_KpiEmployee", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_KpiEmployee");
            return iResult;
        }

        public List<KpiEmployeeModels> SelectRow_KpiEmployee(KpiEmployeeModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRow_KpiEmployee");
            List<KpiEmployeeModels> lstResult = new List<KpiEmployeeModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                if (clParam.trinhtranghoso == "" || clParam.trinhtranghoso == null || clParam.hovaten == "undefined") param["tinhtranghoso"] = "";
                else param["tinhtranghoso"] = clParam.trinhtranghoso;

                if (clParam.hovaten == "" || clParam.hovaten == null || clParam.hovaten == "undefined")
                    param["hovaten"] = "";
                if (clParam.chucdanh == "1")
                {
                    param["nguoitao"] = clParam.nguoitao;
                    if (clParam.maphongban == "0") param["maphongban"] = "";
                    else param["maphongban"] = clParam.maphongban;
                    param["nguoilapkpi_dagui"] = "";
                }
                else if (clParam.chucdanh == "2")
                {
                    param["nguoitao"] = "";
                    param["maphongban"] = clParam.maphongban;
                    param["nguoilapkpi_dagui"] = "1";
                }
                else
                {
                    param["nguoitao"] = "";
                    if (clParam.maphongban == "0") param["maphongban"] = "";
                    else param["maphongban"] = clParam.maphongban;
                    param["nguoilapkpi_dagui"] = "";
                }
                if (clParam.xoa == "1")
                {
                    param["nguoitao"] = "";
                }
                IList ilist = sqlMap.ExecuteQueryForList("KpiConstruction.SelectRow_KpiEmployee", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<KpiEmployeeModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRow_KpiEmployee");
            return lstResult;
        }


        public string Save_KpiEmployee(string DataJson, string nguoitao, string nguoilapgui, string truongphongduyet)
        {
            logger.Start("Save_KpiEmployee");
            string makpinhanvien = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);

                makpinhanvien = json["data1"]["makpinhanvien"].ToString();
                param["maphongban"] = json["data1"]["maphongban"].ToString();
                param["ngaybanhanh"] = json["data1"]["ngaybanhanh"].ToString();
                param["lancapnhat"] = "01";

                param["manhanvien"] = json["data1"]["manhanvien"].ToString();
                param["hovaten"] = json["data1"]["hovaten"].ToString();
                param["chucdanh"] = json["data1"]["chucdanh"].ToString();
                param["captrentructiep"] = json["data1"]["captrentructiep"].ToString();
                param["ngaydangky"] = json["data1"]["ngaydangky"].ToString();
                param["ngaydanhgia"] = json["data1"]["ngaydanhgia"].ToString();


                param["nguoilapkpi"] = json["data1"]["nguoilapkpi"].ToString();
                param["email"] = json["data1"]["email"].ToString(); 
                param["nguoilapkpi_dagui"] = nguoilapgui;
                param["nguoilapkpi_ngay"] = json["data1"]["nguoilapkpi_ngay"].ToString();

                param["truongphongxemxetkpi"] = json["data1"]["truongphongxemxetkpi"].ToString();
                param["truongphongemail"] = json["data1"]["truongphongemail"].ToString();
                param["truongphongxemxetkpi_duyet"] = truongphongduyet;
                param["truongphongxemxetkpi_ngay"] = json["data1"]["truongphongxemxetkpi_ngay"].ToString();
                
                if(json["data1"]["kpichinhsua"].ToString().Trim()=="0" && nguoilapgui=="1")
                    param["kpichinhsua"] = "1"; //Đã gửi mail và k dc chỉnh sữa (Gửi mail lần dầu)

                else if(json["data1"]["kpichinhsua"].ToString().Trim()=="2" && nguoilapgui=="1")
                    param["kpichinhsua"] = "5"; // Yeu cầu chỉnh sữa và gửi mail duyệt lại

                else param["kpichinhsua"] = json["data1"]["kpichinhsua"].ToString().Trim();

                param["nguoitao"] = nguoitao;
                if (makpinhanvien.Trim() == "" || makpinhanvien.Trim() == "0" || makpinhanvien.Trim() == null)
                {
                    param["makpinhanvien"] = GetSequence_All("dm_seq", "KPI_Employee");
                    sqlMap.Insert("KpiConstruction.InsertRow_KpiEmployee", param);
                }
                else
                {
                    param["makpinhanvien"] = makpinhanvien;
                    sqlMap.Update("KpiConstruction.UpdateRow_KpiEmployee", param);
                    //sqlMap.Delete("KpiConstruction.DeleteRow_KpiEmployeeDetail", param);
                }
                makpinhanvien = param["makpinhanvien"].ToString();
                Hashtable param1 = new Hashtable();
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param1 = new Hashtable();
                    param1["makpinhanvien"] = param["makpinhanvien"].ToString().Trim();
                    param1["stt"] = json_tiendo_giatri[i]["stt"].ToString().Trim();
                    param1["muctieu"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["muctieu"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["trongso"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["trongso"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["phuongphapdo"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["phuongphapdo"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["nguonchungminh"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["nguonchungminh"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["donvitinh"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["donvitinh"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["kehoach"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["kehoach"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["thuchien"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["thuchien"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["tilehoanthanh"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["tilehoanthanh"].ToString().Trim(), @"\n", "\\n");
                    param1["ketqua"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["ketqua"].ToString().Trim(), @"\n", "\\n");
                    param1["kyxacnhan"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["kyxacnhan"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["thuoccap"] = json_tiendo_giatri[i]["thuoccap"].ToString().Trim();
                    param1["nguoitao"] = nguoitao;

                    if (json_tiendo_giatri[i]["kpichinhsua"].ToString().Trim() == "0" && nguoilapgui == "1")
                        param1["kpichinhsua"] = "1";
                    else if (json_tiendo_giatri[i]["kpichinhsua"].ToString().Trim() == "2" && nguoilapgui == "1")
                        param1["kpichinhsua"] = "5"; // Da gui mail cho yeu cau chỉnh sữa là 5
                    else param1["kpichinhsua"] = json_tiendo_giatri[i]["kpichinhsua"].ToString().Trim();

                    string makpinhanvien_chitiet = json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim();

                    if (makpinhanvien_chitiet.Trim() == "" || makpinhanvien_chitiet.Trim() == "0" || makpinhanvien_chitiet.Trim() == null)
                    {
                        param1["kpichinhsua"] = "0";
                        sqlMap.Insert("KpiConstruction.InsertRow_KpiEmployeeDetail", param1);
                    }
                    else
                    {
                        param1["makpinhanvien_chitiet"] = makpinhanvien_chitiet;
                        sqlMap.Update("KpiConstruction.UpdateRow_KpiEmployeeDetail", param1);
                    }
                    
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
                makpinhanvien = "0";
            }
            logger.End("Save_KpiEmployee");
            return makpinhanvien;
        }

        public string Save_Ketquakpi(string DataJson, string nguoitao)
        {
            logger.Start("Save_Ketquakpi");
            string makpinhanvien = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);
                makpinhanvien = json["data1"]["makpinhanvien"].ToString();
                //param["ngaydanhgia"] = json["data1"]["ngaydanhgia"].ToString();
                //param["nguoitao"] = nguoitao;
                //param["makpinhanvien"] = makpinhanvien;
                //sqlMap.Update("KpiConstruction.UpdateRow_KpiEmployee", param);
                Hashtable param1 = new Hashtable();
                JArray json_tiendo_giatri = (JArray)json["data2"];
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param1 = new Hashtable();
                    param1["makpinhanvien"] = makpinhanvien;
                    param1["thuchien"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["thuchien"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    param1["tilehoanthanh"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["tilehoanthanh"].ToString().Trim(), @"\n", "\\n");
                    param1["ketqua"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["ketqua"].ToString().Trim(), @"\n", "\\n");
                    param1["nguoitao"] = nguoitao;
                    string makpinhanvien_chitiet = json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim();
                    if (makpinhanvien_chitiet.Trim() == "" || makpinhanvien_chitiet.Trim() == "0" || makpinhanvien_chitiet.Trim() == null)
                    {
                       
                    }
                    else
                    {
                        param1["makpinhanvien_chitiet"] = makpinhanvien_chitiet;
                        sqlMap.Update("KpiConstruction.UpdateRow_KpiEmployeeDetail_ketquakpi", param1);
                    }
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
                makpinhanvien = "0";
            }
            logger.End("Save_Ketquakpi");
            return makpinhanvien;
        }

        public string Save_KpiEmployee_nguoiguihoso(string makpinhanvien)
        {
            logger.Start("Save_KpiEmployee_nguoiguihoso");
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                sqlMap.Update("KpiConstruction.UpdateRow_KpiEmployee_nguoihoso", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
                makpinhanvien = "0";
            }
            logger.End("Save_KpiEmployee_nguoiguihoso");
            return makpinhanvien;
        }


        public List<KpiEmployeeModels> SelectRows_KpiEmployee_hieuchinh(string makpinhanvien)
        {
            logger.Start("SelectRows_KpiEmployee_hieuchinh");
            List<KpiEmployeeModels> lstResult = new List<KpiEmployeeModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                IList ilist = sqlMap.ExecuteQueryForList("KpiConstruction.SelectRows_KpiEmployee_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<KpiEmployeeModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_KpiEmployee_hieuchinh");
            return lstResult;
        }

        public List<KpiEmployeeDetailModels> SelectRows_KpiEmployeeDetail_hieuchinh(string makpinhanvien)
        {
            logger.Start("SelectRows_KpiEmployeeDetail_hieuchinh");
            List<KpiEmployeeDetailModels> lstResult = new List<KpiEmployeeDetailModels>();
            List<KpiEmployeeDetailModels> lstResultDetail = new List<KpiEmployeeDetailModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                IList ilist = sqlMap.ExecuteQueryForList("KpiConstruction.SelectRows_KpiEmployeeDetail_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<KpiEmployeeDetailModels>(ilist);
                foreach (var item in lstResult.Where(p => p.thuoccap == "I"))
                    lstResultDetail.Add(item);
                foreach (var item in lstResult.Where(p => p.thuoccap == "II"))
                    lstResultDetail.Add(item);
                foreach (var item in lstResult.Where(p => p.thuoccap == ""))
                    lstResultDetail.Add(item);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_KpiEmployeeDetail_hieuchinh");
            return lstResultDetail;
        }

        public bool UpdateRow_Employee_duyet_khongduyet(string makpinhanvien, string dongy, string kpichinhsua,string kpict, string kpichinhsuact)
        {
            logger.Start("UpdateRow_Employee_duyet_khongduyet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                param["truongphongxemxetkpi_duyet"] = dongy;
                if (kpichinhsua == "0" && dongy=="1") kpichinhsua = "1";
                else if (kpichinhsua == "2" && dongy == "1") kpichinhsua = "6";
                else if ((kpichinhsua == "1" || kpichinhsua == "0" || kpichinhsua=="2") && dongy == "2") kpichinhsua = "0"; 
                param["kpichinhsua"] = kpichinhsua;
                param["truongphongxemxetkpi_ngay"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("KpiConstruction.UpdateRow_Employee_duyet_khongduyet", param);
                if(kpict.Length>0)
                {
                    string[] machuoi = kpict.Split(',');
                    string[] kpics = kpichinhsuact.Split(',');
                    for (int i = 0; i < machuoi.Length; i++)
                    {
                        if (machuoi[i].ToString().Trim() == "0") param["makpinhanvien_chitiet"] = "";
                        else param["makpinhanvien_chitiet"] = machuoi[i].ToString().Trim();

                        if (kpics[i].ToString().Trim() == "0" && dongy=="1") param["kpichinhsua"] = "1";
                        else if (kpics[i].ToString().Trim() == "2" && dongy=="1") param["kpichinhsua"] = "6";
                        else if ((kpics[i].ToString().Trim() == "1" || kpics[i].ToString().Trim() == "0" || kpics[i].ToString().Trim() == "2") && dongy == "2") param["kpichinhsua"] = "0";
                        else param["kpichinhsua"] = kpics[i].ToString().Trim();
                        sqlMap.Update("KpiConstruction.UpdateRow_Employee_detail_kpichinhsua", param);
                    }
                }
                else sqlMap.Update("KpiConstruction.UpdateRow_Employee_detail_kpichinhsua", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Employee_duyet_khongduyet");
            return strResult;
        }


        public bool UpdateRow_Employee_duyet_khongduyet_html(string makpinhanvien, string dongy)
        {
            logger.Start("UpdateRow_Employee_duyet_khongduyet_html");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                param["truongphongxemxetkpi_duyet"] = dongy;
                param["truongphongxemxetkpi_ngay"] = DateTime.Now.ToString("dd/MM/yyyy");
                IList ilist = sqlMap.ExecuteQueryForList("KpiConstruction.SelectRows_KpiEmployeeDetail_html", param);
                if (ilist.Count > 0)
                {
                    for (int i = 0; i < ilist.Count; i++)
                    {
                        Hashtable param1 = new Hashtable();
                        param1 = (Hashtable)ilist[i];
                        if (param1["kpichinhsua"].ToString().Trim() == "5" && dongy == "1")
                        {
                            param1["kpichinhsua"] = "6";
                            param["kpichinhsua"] = "6";
                        }
                        else if (param1["kpichinhsua"].ToString().Trim() == "5" && dongy == "2")
                        {
                            param1["kpichinhsua"] = "2";
                            param["kpichinhsua"] = "2";
                        }
                        else if (param1["kpichinhsua"].ToString().Trim() == "1" && dongy == "2")
                        {
                            param1["kpichinhsua"] = "0";
                            param["kpichinhsua"] = "0";
                        }
                        sqlMap.Update("KpiConstruction.UpdateRow_Employee_detail_kpichinhsua", param1);
                    }
                }
                sqlMap.Update("KpiConstruction.UpdateRow_Employee_duyet_khongduyet", param);

                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Employee_duyet_khongduyet_html");
            return strResult;
        }


        public bool Save_Yeucauchinhsua_chitiet(string madanhsach)
        {
            logger.Start("Save_Yeucauchinhsua_chitiet");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["kpichinhsua"] = "4";
                param["ngayyeucauchinhsua"] = DateTime.Now.ToString("dd/MM/yyyy");
                JObject json = JObject.Parse(madanhsach);
                JArray json_tiendo_giatri = (JArray)json["data1"];
                param["makpinhanvien"] = json_tiendo_giatri[0]["makpinhanvien"].ToString().Trim();
                sqlMap.Update("KpiConstruction.UpdateEmployee_duyet_Yeucauchinhsua", param);
                for (int i = 0; i < json_tiendo_giatri.Count(); i++)
                {
                    param["makpinhanvien_chitiet"] = json_tiendo_giatri[i]["makpinhanvien_chitiet"].ToString().Trim();
                    param["makpinhanvien"] = json_tiendo_giatri[i]["makpinhanvien"].ToString().Trim();
                    param["kyxacnhan"] = System.Text.RegularExpressions.Regex.Replace(json_tiendo_giatri[i]["kyxacnhan"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                    sqlMap.Update("KpiConstruction.Save_Yeucauchinhsua_chitiet", param);
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Save_Yeucauchinhsua_chitiet");
            return strResult;
        }

        public bool UpdateRow_Employee_duyet_Yeucauchinhsua(string makpinhanvien_chitiet, string dongy, string makpinhanvien)
        {
            logger.Start("UpdateRow_Employee_duyet_Yeucauchinhsua");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                param["truongphongxemxetkpi_duyet"] = dongy;
                if (dongy == "1") param["kpichinhsua"] = "2";
                else param["kpichinhsua"] = "3";
                param["ngayyeucauchinhsua_duyet"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("KpiConstruction.UpdateEmployee_duyet_Yeucauchinhsua", param);
                string [] machuoi = makpinhanvien_chitiet.Split(',');
                for (int i = 0; i < machuoi.Length; i++)
                {
                    param["makpinhanvien_chitiet"] = machuoi[i].ToString().Trim();
                    sqlMap.Update("KpiConstruction.UpdateEmployee_duyet_Yeucauchinhsua_chitiet", param);
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Employee_duyet_Yeucauchinhsua");
            return strResult;
        }

        public bool UpdateRow_Employee_Khoaketquakpi(string makpinhanvien)
        {
            logger.Start("UpdateRow_Employee_Khoaketquakpi");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                //param["khoaketquakpi_ngay"] = DateTime.Now.ToString("dd/MM/yyyy");
                sqlMap.Update("KpiConstruction.UpdateRow_Employee_Khoaketquakpi", param);
                sqlMap.Update("KpiConstruction.UpdateRow_Employee_Khoaketquakpi_chitiet", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Employee_Khoaketquakpi");
            return strResult;
        }


        public List<KpiLevelDepartmentDetailModels> SelectRows_KpiLevelDepartmentDetail_hieuchinh(string maphongban, string nam)
        {
            logger.Start("SelectRows_KpiLevelDepartmentDetail_hieuchinh");
            List<KpiLevelDepartmentDetailModels> lstResult = new List<KpiLevelDepartmentDetailModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["maphongban"] = maphongban;
                param["nam"] = nam;
                IList ilist = sqlMap.ExecuteQueryForList("KpiConstruction.SelectRows_KpiLevelDepartmentDetail_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<KpiLevelDepartmentDetailModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_KpiLevelDepartmentDetail_hieuchinh");
            return lstResult;
        }

        public bool DeletedRow_KpiEmployee(string makpinhanvien, string userid)
        {
            logger.Start("DeletedRow_KpiEmployee");
            bool strResult = true;
            try
            {
                Hashtable param = new Hashtable();
                param["makpinhanvien"] = makpinhanvien;
                param["nguoitao"] = userid;
                sqlMap.Update("KpiConstruction.DeletedRow_KpiEmployee", param);
                sqlMap.Update("KpiConstruction.DeletedRow_KpiEmployee_detail", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("DeletedRow_KpiEmployee");
            return strResult;
        }

        public bool Save_KpiEmployee_Xoadongdulieu(string makpinhanvien, string userid)
        {
            logger.Start("Save_KpiEmployee_Xoadongdulieu");
            bool strResult = true;
            try
            {
                Hashtable param = new Hashtable();
                param["makpinhanvien_chitiet"] = makpinhanvien;
                param["nguoitao"] = userid;
                sqlMap.Update("KpiConstruction.DeletedRow_KpiEmployee_detail_Deleted", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Save_KpiEmployee_Xoadongdulieu");
            return strResult;
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