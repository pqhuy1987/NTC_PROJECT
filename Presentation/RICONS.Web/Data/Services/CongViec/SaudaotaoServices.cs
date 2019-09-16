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
    public class SaudaotaoServices : BaseData
    {
        public SaudaotaoServices()
        {
            logger = new Log4Net(typeof(SaudaotaoServices));
        }


        public int CountRows_Danhsachdanhgiasaudaotao(daotao_ykiensaodaotaoModels clparam)
        {
            logger.Start("CountRows_Danhsachdanhgiasaudaotao");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);
                if (clparam.malop == 0)
                    param["malop"] = "";
                //if (param["madangky"].ToString() == "0")
                //    param["madangky"] = "";

                //if (param["maphongban"].ToString() == "0")
                //    param["maphongban"] = "";

                iResult = (int)sqlMap.ExecuteQueryForObject("Saudaotao.CountRows_Danhsachdanhgiasaudaotao", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_Danhsachdanhgiasaudaotao");
            return iResult;
        }

        public List<daotao_ykiensaodaotaoModels> SelectRows_Danhsachdanhgiasaudaotao(daotao_ykiensaodaotaoModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows_Danhsachlophoc");
            List<daotao_ykiensaodaotaoModels> lstResult = new List<daotao_ykiensaodaotaoModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (clParam.malop == 0)
                    param["malop"] = "";
                //if (param["maphongban"].ToString() == "0")
                //    param["maphongban"] = "";

                //if (param["nguoitao"].ToString() == "0" || param["nguoitao"].ToString() == "1")
                //    param["nguoitao"] = "";

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("Saudaotao.SelectRows_Danhsachdanhgiasaudaotao", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_ykiensaodaotaoModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Danhsachlophoc");
            return lstResult;
        }


        public List<daotao_taolopModels> SelectRows_Laydslopdaotao(daotao_taolopModels clParam)
        {
            logger.Start("SelectRows_Laydslopdaotao");
            List<daotao_taolopModels> lstResult = new List<daotao_taolopModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (clParam.malop == 0)
                    param["malop"] = "";
                IList ilist = sqlMap.ExecuteQueryForList("Saudaotao.SelectRows_Laydslopdaotao", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_taolopModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Laydslopdaotao");
            return lstResult;
        }



        public List<daotao_taolopModels> SelectRows_Laydslophoctheo_nhanvien(daotao_taolopModels clParam)
        {
            logger.Start("SelectRows_Laydslophoctheo_nhanvien");
            List<daotao_taolopModels> lstResult = new List<daotao_taolopModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Saudaotao.SelectRows_Laydslophoctheo_nhanvien", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_taolopModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Laydslophoctheo_nhanvien");
            return lstResult;
        }


        public List<daotao_ykiensaodaotaoModels> SelectRows_daotao_ykiensaodaotao_malop_email(daotao_ykiensaodaotaoModels clParam)
        {
            logger.Start("SelectRows_Laydslophoctheo_nhanvien_theolop");
            List<daotao_ykiensaodaotaoModels> lstResult = new List<daotao_ykiensaodaotaoModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Saudaotao.SelectRows_daotao_ykiensaodaotao_malop_email", param);
                if (ilist.Count > 0)
                {
                    CastDataType cast = new CastDataType();
                    lstResult = cast.AdvanceCastDataToList<daotao_ykiensaodaotaoModels>(ilist);
                }
                else 
                {
                    ilist = sqlMap.ExecuteQueryForList("Saudaotao.SelectRows_Laydslophoctheo_nhanvien_theolop", param);
                    CastDataType cast = new CastDataType();
                    lstResult = cast.AdvanceCastDataToList<daotao_ykiensaodaotaoModels>(ilist);
                }
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_daotao_ykiensaodaotao_malop_email");
            return lstResult;
        }

        public List<daotao_ykiensaodaotao_chitietModels> SelectRows_daotao_ykiensaodaotao_chitiet(string matiepnhan)
        {
            logger.Start("SelectRows_daotao_ykiensaodaotao_chitiet");
            List<daotao_ykiensaodaotao_chitietModels> lstResult = new List<daotao_ykiensaodaotao_chitietModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["matiepnhan"] = matiepnhan;
                IList ilist = sqlMap.ExecuteQueryForList("Saudaotao.SelectRows_daotao_ykiensaodaotao_chitiet", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_ykiensaodaotao_chitietModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_daotao_ykiensaodaotao_chitiet");
            return lstResult;
        }

        public string Save_ykiensaodautao(string DataJson, string nguoitao,string matiepnhan)
        {
            logger.Start("Save_ykiensaodautao");
            string matiepnhanid = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);
                param["malop"] = json["data1"]["malop"].ToString();
                param["manv"] = json["data1"]["manv"].ToString();
                param["email"] = json["data1"]["email"].ToString();
                param["hovaten"] = json["data1"]["hovaten"].ToString();
                param["maphongban"] = json["data1"]["maphongban"].ToString();
                param["tieudekhoahoc"] = json["data1"]["tieudekhoahoc"].ToString();
                param["tengiaovien"] = json["data1"]["tengiaovien"].ToString();
                param["ngaydaotao"] = json["data1"]["ngaydaotao"].ToString();
                param["nguoitao"] = nguoitao;

                param["matiepnhan"] = json["data1"]["matiepnhan"].ToString();

                if (param["matiepnhan"].ToString().Trim() == "0")
                {
                    param["matiepnhan"] = GetSequence_All("dm_seq", "daotao_ykiensaodaotao");
                    sqlMap.Insert("Saudaotao.InsertRow_Saudaotao", param);
                }
                else
                {
                    param["matiepnhan"] = param["matiepnhan"].ToString().Trim();
                    sqlMap.Update("Saudaotao.UpdateRow_Saudaotao", param);
                }
                matiepnhanid = param["matiepnhan"].ToString();
                Hashtable param1 = new Hashtable();
                JArray json_vpp_chitiet = (JArray)json["data2"];
                FunctionXML function = new FunctionXML(Functions.MapPath("~/Xml/Config/encryptionkeyEncodeLink.config"));
                for (int i = 0; i < json_vpp_chitiet.Count(); i++)
                {
                    param1 = new Hashtable();

                    param1["matiepnhan_tieuchidaotao"] = AES.DecryptText(json_vpp_chitiet[i]["matiepnhan_tieuchidaotao"].ToString().Trim(), function.ReadXMLGetKeyEncrypt());
                    param1["matieuchi"] = AES.DecryptText(json_vpp_chitiet[i]["matieuchi"].ToString().Trim(), function.ReadXMLGetKeyEncrypt());
                    param1["matiepnhan"] = matiepnhanid;

                    param1["tentieuchi"] = json_vpp_chitiet[i]["tentieuchi"].ToString().Trim();// ["kem"] = "undefined"

                    if (json_vpp_chitiet[i]["kem"].ToString().Trim() == "true")
                        param1["kem"] = "1";
                    else param1["kem"] = "0";

                    if (json_vpp_chitiet[i]["trungbinh"].ToString().Trim() == "true")
                        param1["trungbinh"] = "1";
                    else param1["trungbinh"] = "0";

                    if (json_vpp_chitiet[i]["kha"].ToString().Trim() == "true")
                        param1["kha"] = "1";
                    else param1["kha"] = "0";

                    if (json_vpp_chitiet[i]["tot"].ToString().Trim() == "true")
                        param1["tot"] = "1";
                    else param1["tot"] = "0";

                    if (json_vpp_chitiet[i]["rattot"].ToString().Trim() == "true")
                        param1["rattot"] = "1";
                    else param1["rattot"] = "0";

                    param1["danhmuccha"] = json_vpp_chitiet[i]["danhmuccha"].ToString().Trim();
                    param1["danhmucgoc"] = json_vpp_chitiet[i]["danhmucgoc"].ToString().Trim();

                    param1["nguoitao"] = nguoitao;
                    if (param1["matiepnhan_tieuchidaotao"].ToString() == "" || param1["matiepnhan_tieuchidaotao"].ToString() == "0" || param1["matiepnhan_tieuchidaotao"].ToString() == null)
                    {
                        sqlMap.Insert("Saudaotao.InsertRow_Saudaotao_chitiet", param1);
                    }
                    else
                    {
                        sqlMap.Update("Saudaotao.UpdateRow_Saudaotao_chitiet", param1);
                    }

                }
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_ykiensaodautao");
            return matiepnhanid;
        }


        public string Save_Capnhatkhiguimail_lophoc(string DataJson, string nguoitao)
        {
            logger.Start("Save_Capnhatkhiguimail_lophoc");
            string malopid = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);

                param["tieudeguimail"] = json["tieudeguimail"].ToString();
                param["mailcc"] = json["mailcc"].ToString();
                param["kinhgui"] = json["kinhgui"].ToString();
                param["baocao"] = json["baocao"].ToString();
                param["noidungguimail"] = json["noidungguimail"].ToString();
                param["malop"] = json["malop"].ToString();
                param["nguoitao"] = nguoitao;
                sqlMap.Update("Quanlylopdaotao.UpdateRow_Capnhatkhiguimail_lophoc", param);
                malopid = param["malop"].ToString();
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_Capnhatkhiguimail_lophoc");
            return malopid;
        }

        public string Save_Capnhatfileupload(string malop, string tenfile)
        {
            logger.Start("Save_Capnhatfileupload");
            string malopid = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["tenfile"] = tenfile;
                param["malop"] = malop;
                sqlMap.Update("Quanlylopdaotao.UpdateRow_Capnhatfileupload", param);
                malopid = param["malop"].ToString();
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_Capnhatfileupload");
            return malopid;
        }


      

        public List<daotao_taolopModels> SelectRows_daotao_taolop_hieuchinh(daotao_taolopModels clParam)
        {
            logger.Start("SelectRows_daotao_taolop_hieuchinh");
            List<daotao_taolopModels> lstResult = new List<daotao_taolopModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Quanlylopdaotao.SelectRows_daotao_taolop_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_taolopModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_daotao_taolop_hieuchinh");
            return lstResult;
        }

        public List<daotao_taolopModels> SelectRows_Danhsachlophoc_guiemail(daotao_taolopModels clParam)
        {
            logger.Start("SelectRows_Danhsachlophoc_guiemail");
            List<daotao_taolopModels> lstResult = new List<daotao_taolopModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Quanlylopdaotao.SelectRows_Danhsachlophoc_guiemail", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_taolopModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Danhsachlophoc_guiemail");
            return lstResult;
        }


        public int Count_Danhsachnhanvien_guiemail(daotao_taolop_chitietModels clparam)
        {
            logger.Start("CountRows_Danhsachlophoc");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (param["malop"].ToString() == "0" || param["malop"].ToString() == "undefined" || param["malop"].ToString() == null)
                    param["malop"] = "";

                if (param["hovaten"].ToString() == "0" || param["hovaten"].ToString() == "undefined" || param["hovaten"].ToString() == null)
                    param["hovaten"] = "";

                if (param["manv"].ToString() == "0" || param["manv"].ToString() == "undefined" || param["manv"].ToString() == null)
                    param["manv"] = "";

                //if (param["maphongban"].ToString() == "0")
                //    param["maphongban"] = "";

                iResult = (int)sqlMap.ExecuteQueryForObject("Quanlylopdaotao.Count_Danhsachnhanvien_guiemail", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_Danhsachlophoc");
            return iResult;
        }

        public List<daotao_taolop_chitietModels> SelectRows_Danhsachnhanvien_guiemail(daotao_taolop_chitietModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows_Danhsachnhanvien_guiemail");
            List<daotao_taolop_chitietModels> lstResult = new List<daotao_taolop_chitietModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["malop"].ToString() == "0" || param["malop"].ToString() == "undefined" || param["malop"].ToString() == null)
                    param["malop"] = "";

                if (param["hovaten"].ToString() == "0" || param["hovaten"].ToString() == "undefined" || param["hovaten"].ToString() == null)
                    param["hovaten"] = "";

                if (param["manv"].ToString() == "0" || param["manv"].ToString() == "undefined" || param["manv"].ToString() == null)
                    param["manv"] = "";

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;
                IList ilist = sqlMap.ExecuteQueryForList("Quanlylopdaotao.SelectRows_Danhsachnhanvien_guiemail", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_taolop_chitietModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Danhsachnhanvien_guiemail");
            return lstResult;
        }


        public string Save_diemdanh(string malopchitiet, string diemdanh,string nguoitao)
        {
            logger.Start("Save_diemdanh");
            string malopid = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["malopchitiet"] = malopchitiet;
                param["nguoitao"] = nguoitao;
                if(diemdanh.Trim()=="0")
                    param["diemdanh"] = "1";
                else param["diemdanh"] = "0";
                sqlMap.Update("Quanlylopdaotao.UpdateRow_diemdanh", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_dangkylophoc");
            return malopid;
        }

        public bool UpdateRow_Duyetthamgiadaotao(string malopchitiet, string dongy)
        {
            logger.Start("UpdateRow_Duyetthamgiadaotao");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["malopchitiet"] = malopchitiet;
                param["xacnhanthamgia"] = dongy;
                sqlMap.Update("Quanlylopdaotao.UpdateRow_Duyetthamgiadaotao", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Duyetthamgiadaotao");
            return strResult;
        }

        public bool UpdateRow_Daguiemailthanhcong(int malopchitiet)
        {
            logger.Start("UpdateRow_Daguiemailthanhcong");
            bool strResult = true;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param["malopchitiet"] = malopchitiet;
                sqlMap.Update("Quanlylopdaotao.UpdateRow_Daguiemailthanhcong", param);
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("UpdateRow_Daguiemailthanhcong");
            return strResult;
        }








































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