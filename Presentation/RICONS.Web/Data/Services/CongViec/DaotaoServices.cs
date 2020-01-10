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
    public class DaotaoServices : BaseData
    {
        public DaotaoServices()
        {
            logger = new Log4Net(typeof(DaotaoServices));
        }

        public int CountRows_WeedMeeting(WeedMeetingModels clparam)
        {
            logger.Start("CountRows_WeedMeeting");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (clparam.maphongban == "0" || clparam.maphongban == null) param["maphongban"] = "";
                else param["maphongban"] = clparam.maphongban;

                iResult = (int)sqlMap.ExecuteQueryForObject("WeedMeeting.CountRows_WeedMeeting", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_WeedMeeting");
            return iResult;
        }

        public int CountRows_WeedMeeting2(WeedMeetingModels clparam)
        {
            logger.Start("CountRows_WeedMeeting2");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (clparam.maphongban == "0" || clparam.maphongban == null) param["maphongban"] = "";
                else param["maphongban"] = clparam.maphongban;

                if (clparam.loaibaocao == 0 || clparam.loaibaocao == null) param["loaibaocao"] = "";
                else param["loaibaocao"] = clparam.loaibaocao;

                iResult = (int)sqlMap.ExecuteQueryForObject("WeedMeeting.CountRows_WeedMeeting2", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_WeedMeeting2");
            return iResult;
        }

        public int CountRows_WeedMeetingGDDA(WeedMeetingModels clparam)
        {
            logger.Start("CountRows_WeedMeetingGDDA");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (clparam.maphongban == "0" || clparam.maphongban == null) param["maphongban"] = "";
                else param["maphongban"] = clparam.maphongban;

                if (clparam.loaibaocao == 0 || clparam.loaibaocao == null) param["loaibaocao"] = "";
                else param["loaibaocao"] = clparam.loaibaocao;

                iResult = (int)sqlMap.ExecuteQueryForObject("WeedMeeting.CountRows_WeedMeetingGDDA", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_WeedMeetingGDDA");
            return iResult;
        }

        public int CountRows_WeedMeetingBCTC(WeedMeetingModels clparam)
        {
            logger.Start("CountRows_WeedMeetingBCTC");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (clparam.maphongban == "0" || clparam.maphongban == null) param["maphongban"] = "";
                else param["maphongban"] = clparam.maphongban;

                if (clparam.loaibaocao == 0 || clparam.loaibaocao == null) param["loaibaocao"] = "";
                else param["loaibaocao"] = clparam.loaibaocao;

                iResult = (int)sqlMap.ExecuteQueryForObject("WeedMeeting.CountRows_WeedMeetingBCTC", param);
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                iResult = 0;
                logger.Error(ex.Message);
            }
            logger.End("CountRows_WeedMeetingBCTC");
            return iResult;
        }

        public List<WeedMeetingModels> SelectRows_WeedMeeting(WeedMeetingModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows_WeedMeeting");
            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                if (clParam.nguoitao == 0) param["nguoitao"] = "";

                if (clParam.maphongban == "0") param["maphongban"] = "";
                else param["maphongban"] = clParam.maphongban;

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("WeedMeeting.SelectRow_WeedMeeting", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WeedMeetingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_WeedMeeting");
            return lstResult;
        }

        public List<WeedMeetingModels> SelectRows_WeedMeeting2(WeedMeetingModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows_WeedMeeting2");
            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                if (clParam.nguoitao == 0) param["nguoitao"] = "";

                if (clParam.maphongban == "0") param["maphongban"] = "";
                else param["maphongban"] = clParam.maphongban;

                if (clParam.loaibaocao == 0) param["loaibaocao"] = "";
                else param["loaibaocao"] = clParam.loaibaocao;

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("WeedMeeting.SelectRow_WeedMeeting2", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WeedMeetingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_WeedMeeting2");
            return lstResult;
        }

        public List<WeedMeetingModels> SelectRows_WeedMeetingGDDA(WeedMeetingModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows_WeedMeetingGDDA");
            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                if (clParam.nguoitao == 0) param["nguoitao"] = "";

                if (clParam.maphongban == "0") param["maphongban"] = "";
                else param["maphongban"] = clParam.maphongban;

                if (clParam.loaibaocao == 0) param["loaibaocao"] = "";
                else param["loaibaocao"] = clParam.loaibaocao;

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("WeedMeeting.SelectRow_WeedMeetingGDDA", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WeedMeetingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_WeedMeetingGDDA");
            return lstResult;
        }

        public List<WeedMeetingModels> SelectRows_WeedMeetingBCTC(WeedMeetingModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows_WeedMeetingBCTC");
            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                if (clParam.nguoitao == 0) param["nguoitao"] = "";

                if (clParam.maphongban == "0") param["maphongban"] = "";
                else param["maphongban"] = clParam.maphongban;

                if (clParam.loaibaocao == 0) param["loaibaocao"] = "";
                else param["loaibaocao"] = clParam.loaibaocao;

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("WeedMeeting.SelectRow_WeedMeetingBCTC", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WeedMeetingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_WeedMeetingBCTC");
            return lstResult;
        }

        public string Save_WeedMeeting(string DataJson, string nguoitao)
        {
            logger.Start("Save_WeedMeeting");
            string macuochop = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);
                param["macuochop"] = json["macuochop"].ToString();
                param["matuan"] = json["matuan"].ToString();
                param["giohop"] = json["giohop"].ToString();
                param["ngayhop"] = json["ngayhop"].ToString();
                param["phonghop"] = json["phonghop"].ToString();
                param["lydobuoihop"] = json["lydobuoihop"].ToString();
                param["thanhphanthamdu"] = json["thanhphanthamdu"].ToString();
                param["caplanhdao"] = json["caplanhdao"].ToString();
                param["noidungcuochop"] = System.Text.RegularExpressions.Regex.Replace(json["noidungcuochop"].ToString().Trim(), @"\n", "\\n").Replace("'", "");
                param["uploadfile"] = json["uploadfile"].ToString().Replace("/", ".");
                param["tenfile"] = json["tenfile"].ToString();
                param["nguoitao"] = nguoitao;
                param["maphongban"] = json["maphongban"].ToString();
                param["loaicuochop"] = json["loaicuochop"].ToString();
                param["phongban_congtruong"] = json["phongban_congtruong"].ToString();
                param["loaibaocao"] = json["loaibaocao"].ToString();
                if (param["macuochop"].ToString().Trim() == "0")
                {
                    param["macuochop"] = GetSequence_All("dm_seq", "weedmeeting");
                    sqlMap.Insert("WeedMeeting.InsertRow_WeedMeeting", param);
                }
                else
                {
                    sqlMap.Update("WeedMeeting.UpdateRow_WeedMeeting", param);
                }
                macuochop = param["macuochop"].ToString();
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                macuochop = "-1";
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Save_WeedMeeting");
            return macuochop;
        }

        public string Delete_WeedMeeting(string DataJson, string nguoitao)
        {
            logger.Start("Delete_WeedMeeting");
            string macuochop = "";
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                JObject json = JObject.Parse(DataJson);
                param["macuochop"] = json["macuochop"].ToString();
                param["xoa"] = json["xoa"].ToString();
                if (param["macuochop"].ToString().Trim() == "0")
                {
                    param["macuochop"] = GetSequence_All("dm_seq", "weedmeeting");
                    sqlMap.Insert("WeedMeeting.InsertRow_WeedMeeting", param);
                }
                else
                {
                    sqlMap.Update("WeedMeeting.DeleteRow_KpiEmployee", param);
                }
                macuochop = param["macuochop"].ToString();
                sqlMap.CommitTransaction();
            }
            catch (Exception ex)
            {
                macuochop = "-1";
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("Delete_WeedMeeting");
            return macuochop;
        }

        public List<WeedMeetingModels> SelectRows_WeedMeeting_hieuchinh(string macuochop)
        {
            logger.Start("SelectRows_WeedMeeting");
            List<WeedMeetingModels> lstResult = new List<WeedMeetingModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["macuochop"] = macuochop;
                IList ilist = sqlMap.ExecuteQueryForList("WeedMeeting.SelectRows_WeedMeeting_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<WeedMeetingModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_WeedMeeting");
            return lstResult;
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


        public List<daotao_taolopModels> SelectRows_Danhsachlophoc_hieuchinh(daotao_taolopModels clParam)
        {
            logger.Start("SelectRows_Danhsachlophoc");
            List<daotao_taolopModels> lstResult = new List<daotao_taolopModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Quanlylopdaotao.SelectRows_Danhsachlophoc_hieuchinh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<daotao_taolopModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Danhsachlophoc");
            return lstResult;
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