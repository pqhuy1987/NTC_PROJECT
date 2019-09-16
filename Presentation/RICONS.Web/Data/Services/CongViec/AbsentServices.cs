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
    public class AbsentServices : BaseData
    {
        #region khai bao ban dau
        private const string strTableName = "m_donvi_phongban";
        #endregion

        public AbsentServices()
        {
            logger = new Log4Net(typeof(DanhmucServices));
        }

        public int CountRows(AbsentModels clparam)
        {
            logger.Start("CountRows");
            int iResult = 0;
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clparam);

                if (param["manghiphep"].ToString() == "")
                    param["manghiphep"] = "";

                if (param["maphongban"].ToString() == "0")
                    param["maphongban"] = "";

                iResult = (int)sqlMap.ExecuteQueryForObject("Dangkynghiphep.CountRows", param);
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

        public List<AbsentModels> SelectRows_Danhsachnghiphep(AbsentModels clParam, int trangbd, int trangkt)
        {
            logger.Start("SelectRows");
            List<AbsentModels> lstResult = new List<AbsentModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["manghiphep"].ToString() == "")
                    param["manghiphep"] = "";

                if (param["maphongban"].ToString() == "0")
                    param["maphongban"] = "";

                if (param["nguoitao"].ToString() == "0")
                    param["nguoitao"] = "";

                param["trangbd"] = trangbd;
                param["trangkt"] = trangkt;

                IList ilist = sqlMap.ExecuteQueryForList("Dangkynghiphep.SelectRows_Danhsachnghiphep", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<AbsentModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows");
            return lstResult;
        }

        public List<AbsentModels> SelectRows_Danhsachnghiphep_11111(AbsentModels clParam)
        {
            logger.Start("SelectRows");
            List<AbsentModels> lstResult = new List<AbsentModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);
                if (param["manghiphep"].ToString() == "")
                    param["manghiphep"] = "";

                if (param["nguoitao"].ToString() == "0")
                    param["nguoitao"] = "";

                IList ilist = sqlMap.ExecuteQueryForList("Dangkynghiphep.SelectRows_Danhsachnghiphep_11111", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<AbsentModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows");
            return lstResult;
        }

        public List<AbsentModels> SelectRows_Danhsachnghiphep_songayphep(AbsentModels clParam)
        {
            logger.Start("SelectRows");
            List<AbsentModels> lstResult = new List<AbsentModels>();
            try
            {
                Hashtable param = new Hashtable();
                param = base.SetDataToHashtable(false, clParam);

                if (param["nguoitao"].ToString() == "0")
                    param["nguoitao"] = "";

                IList ilist = sqlMap.ExecuteQueryForList("Dangkynghiphep.SelectRows_Danhsachnghiphep_songayphep", param);
                CastDataType cast = new CastDataType();
                lstResult = cast.AdvanceCastDataToList<AbsentModels>(ilist);
            }
            catch (Exception ex)
            {
                logger.Error("Loi ---> " + ex.Message);
            }
            logger.End("SelectRows");
            return lstResult;
        }


       






        public string Insert_Donxinnghiphep(AbsentModels clParam)
        {
            logger.Start("InsertRow");
            string strResult = "";
            try
            {
                if (clParam.manghiphep.ToString().Trim() == "")
                {
                    Hashtable param = new Hashtable();
                    param = base.SetDataToHashtable(false, clParam);
                    strResult = GetSequence_All("dm_seq", "Dondangkynghiphep");
                    clParam.manghiphep = strResult;
                    base.InsertData(clParam, "Dondangkynghiphep");
                }
                else
                {
                    Hashtable param = new Hashtable();
                    param = base.SetDataToHashtable(false, clParam);
                    sqlMap.Update("Dangkynghiphep.UpdateRow_Dondangkynghiphep", param);
                }
                strResult = clParam.manghiphep;

            }
            catch (Exception ex)
            {
                strResult = "-1";
                logger.Error(ex.Message);
            }
            logger.End("InsertRow");
            return strResult;
        }

        public bool UpdateRow_Duyetnghiphep1(string manghiphep, string daduyet)
        {
            logger.Start("Insert_Donxinnghiphep");
            bool strResult=true;
            try
            {
                Hashtable param = new Hashtable();
                param["manghiphep"] = manghiphep;
                param["duyetcap1"] = daduyet;
                sqlMap.Update("Dangkynghiphep.UpdateRow_Duyetnghiphep1", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Insert_Donxinnghiphep");
            return strResult;
        }

        public bool UpdateRow_Duyetnghiphep2(string manghiphep, string daduyet)
        {
            logger.Start("Insert_Donxinnghiphep");
            bool strResult = true;
            try
            {
                Hashtable param = new Hashtable();
                param["manghiphep"] = manghiphep;
                param["duyetcap2"] = daduyet;
                sqlMap.Update("Dangkynghiphep.UpdateRow_Duyetnghiphep2", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Insert_Donxinnghiphep");
            return strResult;
        }

        public bool DeletedRow_Dondangkynghiphep(string manghiphep,string userid)
        {
            logger.Start("Insert_Donxinnghiphep");
            bool strResult = true;
            try
            {
                Hashtable param = new Hashtable();
                param["manghiphep"] = manghiphep;
                param["nguoitao"] = userid;
                sqlMap.Update("Dangkynghiphep.DeletedRow_Dondangkynghiphep", param);
            }
            catch (Exception ex)
            {
                strResult = false;
                logger.Error(ex.Message);
            }
            logger.End("Insert_Donxinnghiphep");
            return strResult;
        }

        


       

    }
}