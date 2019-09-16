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
    public class DanhmucServices : BaseData
    {
        #region khai bao ban dau
        private const string strTableName = "m_donvi_phongban";
        #endregion

        public DanhmucServices()
        {
            logger = new Log4Net(typeof(DanhmucServices));
        }

        public List<thongtingiamdocModels> SelectRows_thongtingiamdoc(thongtingiamdocModels clParam)
        {
            logger.Start("SelectRows_thongtingiamdoc");
            List<thongtingiamdocModels> lstResult = new List<thongtingiamdocModels>();
            try
            {
                Hashtable param = new Hashtable();
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_thongtingiamdoc", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<thongtingiamdocModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_thongtingiamdoc");
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

        public List<PhongBanModels> SelectRows_ma(string maphongban)
        {
            logger.Start("SelectRows");
            List<PhongBanModels> lstResult = new List<PhongBanModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["maphongban"] = maphongban;
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_ma", param);
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

        public string Insert_phongban(PhongBanModels clParam)
        {
            logger.Start("InsertRow");
            string strResult = "";
            try
            {
                if (clParam.maphongban.ToString().Trim() == "")
                {
                    strResult = GetSequence_All("dm_seq", "m_donvi_phongban");
                    //string[] chuoi = clParam.tenphongban.ToString().Trim().Split(' ');
                    //for (int i = 0; i < chuoi.Length; i++)
                    //{
                    //    maphongban += chuoi[i].Substring(0, 1);
                    //}
                    clParam.maphongban = int.Parse(strResult).ToString();
                    base.InsertData(clParam, strTableName);
                }
                else
                {
                    Hashtable param = new Hashtable();
                    param = base.SetDataToHashtable(false, clParam);
                    sqlMap.Update("Danhmuc.UpdateRow_phongban", param);
                }
                strResult = clParam.maphongban;
                
            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("InsertRow");
            return strResult;
        }

        public string Deleted_phongban(string maphongban,string nguoitao)
        {
            logger.Start("InsertRow");
            string strResult = "";
            try
            {
                Hashtable param = new Hashtable();
                param["maphongban"] = maphongban;
                param["nguoitao"] = nguoitao;
                sqlMap.Update("Danhmuc.DeletedRow_phongban", param);
                strResult = maphongban;
            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("InsertRow");
            return strResult;
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

        public List<ChucDanhModels> SelectRows_danhmucchucdanh(ChucDanhModels clParam)
        {
            logger.Start("SelectRows_danhmucchucdanh");
            List<ChucDanhModels> lstResult = new List<ChucDanhModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_danhmucchucdanh", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ChucDanhModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_danhmucchucdanh");
            return lstResult;
        }







        public List<ChucDanhModels> SelectRows_chucvu_ma(string machudanh)
        {
            logger.Start("SelectRows_chucvu_ma");
            List<ChucDanhModels> lstResult = new List<ChucDanhModels>();
            try
            {
                Hashtable param = new Hashtable();
                param["machucdanh"] = machudanh;
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_chucvu_ma", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<ChucDanhModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_chucvu_ma");
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


        /// Văn phòng phẩm
        public List<Danhmuc_VanPhongPhamModels> SelectRows_Vanphongpham(Danhmuc_VanPhongPhamModels clParam)
        {
            logger.Start("SelectRows_Vanphongpham");
            List<Danhmuc_VanPhongPhamModels> lstResult = new List<Danhmuc_VanPhongPhamModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_vanphongpham", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<Danhmuc_VanPhongPhamModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_Vanphongpham");
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

        public bool DeleteRows_DM_Vanphongpham(Danhmuc_VanPhongPhamModels model)
        {
            logger.Start("DeleteRows_DM_Vanphongpham");
            bool lstResult = false;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param = new Hashtable();
                param["mavanphongpham"] = model.mavanphongpham;
                param["nguoihieuchinh"] = model.nguoihieuchinh;
                sqlMap.Update("Danhmuc.DeleteRows_DM_Vanphongpham", param);
                sqlMap.CommitTransaction();
                lstResult = true;
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("DeleteRows_chucdanh");
            return lstResult;
        }

        //Chức danh

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

        public bool DeleteRows_chucdanh(ChucDanhModels clParam)
        {
            logger.Start("DeleteRows_chucdanh");
            bool lstResult = false;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param = new Hashtable();
                param["machucdanh"] = clParam.machucdanh;
                param["nguoihieuchinh"] = clParam.nguoihieuchinh;
                sqlMap.Update("Danhmuc.DeleteRow_chucdanh", param);
                sqlMap.CommitTransaction();
                lstResult = true;
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("DeleteRows_chucdanh");
            return lstResult;
        }





        //=====================DANH MUC GIAO VIEN========================
        public List<Item_weedModels> SelectRows_giaovien(Item_weedModels clParam)
        {
            logger.Start("SelectRows_giaovien");
            List<Item_weedModels> lstResult = new List<Item_weedModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (clParam.matuan == "0")
                    param["matuan"] = "";
                IList ilist = sqlMap.ExecuteQueryForList("Danhmuc.SelectRows_Item_weed", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<Item_weedModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows_giaovien");
            return lstResult;
        }

        public string Insert_giaovien(daotao_dm_giaovienModels clParam)
        {
            logger.Start("Insert_giaovien");
            string strResult = "";
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (clParam.magiaovien.ToString().Trim() == "" || clParam.magiaovien.ToString().Trim() == "0" || clParam.magiaovien.ToString().Trim() == null)
                {
                    sqlMap.Insert("Danhmuc.InsertRow_giaovien", param);
                }
                else
                {
                    sqlMap.Update("Danhmuc.UpdateRow_giaovien", param);
                }
                strResult = param["magiaovien"].ToString();

            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("Insert_giaovien");
            return strResult;
        }

        public bool DeleteRows_giaovien(daotao_dm_giaovienModels clParam)
        {
            logger.Start("DeleteRows_giaovien");
            bool lstResult = false;
            try
            {
                sqlMap.BeginTransaction();
                Hashtable param = new Hashtable();
                param = new Hashtable();
                param["magiaovien"] = clParam.magiaovien;
                param["nguoitao"] = clParam.nguoihieuchinh;
                sqlMap.Update("Danhmuc.DeletedRow_giaovien", param);
                sqlMap.CommitTransaction();
                lstResult = true;
            }
            catch (Exception ex)
            {
                sqlMap.RollbackTransaction();
                logger.Error(ex.Message);
            }
            logger.End("DeleteRows_giaovien");
            return lstResult;
        }



    }
}