using Newtonsoft.Json.Linq;
using RICONS.Core.Functions;
using RICONS.DataServices;
using RICONS.DataServices.DataClass;
using RICONS.Logger;
using RICONS.Web.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RICONS.Web.Data.Services
{
    public class NhapkhoServices : BaseData
    {
        #region khai bao ban dau
        private const string strTableName = "m_donvi_phongban";
        #endregion

        public NhapkhoServices()
        {
            logger = new Log4Net(typeof(DanhmucServices));
        }

        public int CountRows(WarehousingModels clparam)
        {
            logger.Start("CountRows");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                if (param["nguoitao"].ToString() == "0" || param["nguoitao"].ToString() == "1")
                    param["nguoitao"] = "";
                iResult = (int)sqlMap.ExecuteQueryForObject("Nhapkho.CountRows_Nhapkho", param);
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

        public List<WarehousingModels> SelectRows_Nhapkho_ma(WarehousingModels clParam)
        {
            logger.Start("SelectRows_Nhapkho");
            List<WarehousingModels> lstResult = new List<WarehousingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["manhapkho"].ToString() == "")
                    param["manhapkho"] = "";

                IList ilist = sqlMap.ExecuteQueryForList("Nhapkho.SelectRows_Nhapkho_ma", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WarehousingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Nhapkho");
            return lstResult;
        }

        public List<WarehousingModels> SelectRows_Nhapkho(WarehousingModels clParam, int trangbd, int trangkt,string tungay,string denngay)
        {
            logger.Start("SelectRows_Nhapkho");
            List<WarehousingModels> lstResult = new List<WarehousingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["manhapkho"].ToString() == "")
                    param["manhapkho"] = "";

                if (param["nguoitao"].ToString() == "0" || param["nguoitao"].ToString() == "1")
                    param["nguoitao"] = "";

                param["tungay"] = tungay;
                param["denngay"] = denngay;

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("Nhapkho.SelectRows_Nhapkho", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WarehousingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Nhapkho");
            return lstResult;
        }

        public List<Warehousing_detailModels> SelectRows_Nhapkho_chitiet(Warehousing_detailModels clParam)
        {
            logger.Start("SelectRows_Nhapkho_chitiet");
            List<Warehousing_detailModels> lstResult = new List<Warehousing_detailModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Nhapkho.SelectRows_Nhapkho_chitiet", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<Warehousing_detailModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Nhapkho_chitiet");
            return lstResult;
        }

        public string Save_nhapkho(string DataJson, string nguoitao, string manhapkho)
        {
            logger.Start("Save_nhapkho");
            string manhapkhohang = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);
                param["sochungtu"] = json["data1"]["sochungtu"].ToString();
                param["sohoadon"] = json["data1"]["sohoadon"].ToString();
                param["ngaynhapchungtu"] = json["data1"]["ngaynhapchungtu"].ToString();
                param["nhacungcap"] = json["data1"]["nhacungcap"].ToString();
                param["tongtien"] = json["data1"]["tongtien"].ToString();
                param["noidungnhaphang"] = json["data1"]["noidungnhaphang"].ToString();
                param["nguoitao"] = nguoitao;

                if (manhapkho.Trim() == "")
                {
                    param["manhapkho"] = GetSequence_All("dm_seq", "warehousing");
                    sqlMap.Insert("Nhapkho.Insert_Nhapkho", param);
                }
                else
                {
                    param["manhapkho"] = manhapkho;
                    sqlMap.Update("Nhapkho.UpdateRow_Nhapkho", param);
                    sqlMap.Update("Nhapkho.DeletedRow_Nhapkho_chitiet", param);
                }
                manhapkhohang = param["manhapkho"].ToString();
                Hashtable param1 = new Hashtable();
                JArray json_nhapkho_chitiet = (JArray)json["data2"];

                for (int i = 1; i < json_nhapkho_chitiet.Count(); i++)
                {
                    param1 = new Hashtable();
                    param1["manhapkho"] = param["manhapkho"].ToString();
                    //param1["machitiet"] = GetSequence_All("dm_seq", "warehousing_detail");

                    string madanhmuc = json_nhapkho_chitiet[i]["mavanphongpham"].ToString();
                    FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                    param1["mavanphongpham"] = AES.DecryptText(json_nhapkho_chitiet[i]["mavanphongpham"].ToString(), function.ReadXMLGetKeyEncrypt());
                    param1["tenvanphongpham"] = json_nhapkho_chitiet[i]["tenvanphongpham"].ToString();
                    param1["donvitinh"] = json_nhapkho_chitiet[i]["donvitinh"].ToString();
                    param1["soluong"] = json_nhapkho_chitiet[i]["soluong"].ToString();
                    param1["dongia"] = json_nhapkho_chitiet[i]["dongia"].ToString();
                    param1["thanhtien"] = json_nhapkho_chitiet[i]["thanhtien"].ToString();
                    param1["danhmuccha"] = json_nhapkho_chitiet[i]["danhmuccha"].ToString();
                    //param1["danhmucgoc"] = json_nhapkho_chitiet[i]["danhmucgoc"].ToString();
                    param1["nguoitao"] = nguoitao;
                    sqlMap.Insert("Nhapkho.Insert_Nhapkho_chitiet", param1);
                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                manhapkhohang = "-1";
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_nhapkho");
            return manhapkhohang;
        }


        public bool DeletedRow_Nhapkho(string manhapkho, string userid)
        {
            logger.Start("DeletedRow_Nhapkho");
            bool strResult = true;
            try
            {
                Hashtable param = new Hashtable();
                param["manhapkho"] = manhapkho;
                param["nguoitao"] = userid;
                sqlMap.Update("Nhapkho.DeletedRow_Nhapkho", param);
                sqlMap.Update("Nhapkho.DeletedRow_Nhapkho_chitiet", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("DeletedRow_Nhapkho");
            return strResult;
        }


        public string Save_Importdl(DataTable tblSource)
        {
            logger.Start("InsertRow_vanphongpham");
            string strResult = "";
            try
            {
                string ma = "";
                foreach (DataRow row in tblSource.Rows)
                {
                    Danhmuc_VanPhongPhamModels clParam = new Danhmuc_VanPhongPhamModels();
                    strResult = GetSequence_All("dm_seq", "dm_vanphongpham");
                    clParam.mavanphongpham = int.Parse(strResult);

                    clParam.tenvanphongpham = row["tenvanphongpham"].ToString();
                    clParam.dongia = row["dongia"].ToString();
                    clParam.danhmuccha = row["danhmuccha"].ToString();
                   
                    //clParam.ghichu = row["ghichu"].ToString();

                    clParam.xoa = "0";
                    clParam.nguoitao = 1;
                    clParam.ngaytao = "GETDATE()";
                    clParam.donvitinh = row["donvitinh"].ToString();

                    if (clParam.danhmuccha == "0")
                    {
                        ma = strResult;
                        clParam.danhmuccha = strResult;
                        clParam.danhmucgoc = "0";
                    }
                    else
                        clParam.danhmuccha = ma;
                    base.InsertData(clParam, "dm_vanphongpham");
                }
            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("InsertRow_vanphongpham");
            return strResult;
        }

       

    }
}