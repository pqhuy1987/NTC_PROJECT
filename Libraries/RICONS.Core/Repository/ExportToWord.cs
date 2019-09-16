using System;
using System.Drawing.Imaging;
using Microsoft.Office.Interop.Word;
using wordns = Microsoft.Office.Interop.Word;
using System.IO;
using System.Collections;

using System.Data;
using RICONS.Core.Functions;
using DataTable = System.Data.DataTable;

namespace ExportHelper.Repository
{
    public class ExportToWord
    {

        private readonly BaseWord _baseWord;
        private readonly string _docTemplate;

        public ExportToWord(string docTemplate)
        {
            _docTemplate = docTemplate;
            FileInfo fileInfo = new FileInfo(_docTemplate);
            string ext = fileInfo.Extension.ToUpper();
            if (string.IsNullOrWhiteSpace(_docTemplate) || !fileInfo.Exists || (!ext.Equals(".DOC") && !ext.Equals(".DOCX")))
            {
                throw new ArgumentException("File word template không đúng.");
            }

            _baseWord = new BaseWord();
        }



        #region In hồ sơ nhân sự


        /// <summary>
        /// In HS nhân sự
        /// </summary>
        /// <param name="hoSoData">DataSet phải chứa các bảng sau:
        /// + VW_IN_HSNS_THONGTINCHINH : View chứa thông tin chính của nhân sự
        /// 
        /// </param>
        /// <returns></returns>
        public bool InHoSoNhanSu(DataSet hoSoData)
        {
            bool result = false;
            object missing = System.Reflection.Missing.Value;
            Application wordApp = new Application();
            Document aDoc = null;
            try
            {
                string folder = Constants.FolderTempate;
                //if (Function.GetDefVal(Constants.MDFolderTemplateCer) != "")
                //    folder = Function.GetDefVal(Constants.MDFolderTemplateCer);

                // object fileName = _docTemplate;

                //string fileName = "C:\\RICONS\\TEMPLATE\\" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "Denghimuasamthietbi.xls"; 

                object saveAs = folder + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + "print.doc";

                if (!Directory.Exists(folder + "TMP"))
                {
                    Directory.CreateDirectory(folder + "TMP");
                }

                object readOnly = false;
                object isVisible = false;

                //Set Word to be not visible.
                wordApp.Visible = false;

                //Open the word document
                object fileNameTemp = _docTemplate;
                aDoc = wordApp.Documents.Open(ref fileNameTemp, ref missing,
                  ref readOnly, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref isVisible, ref missing, ref missing,
                  ref missing, ref missing);

                // Activate the document
                aDoc.Activate();

                DataRow nhanVienRow = hoSoData.Tables["VW_IN_HSNS_THONGTINCHINH"].Rows[0];

                // Param replace shape
                Hashtable param = _baseWord.GetTextBoxParams(nhanVienRow); // Chắc chắn rằng HS có dữ liệu

                /* Tạm thời lấy giá trị dòng đầu tiên của trình độ tin học và trình độ ngoại ngữ để fill vào trường tin học & ngoại ngũ trong HS */
                DataTable nvTinHoc = hoSoData.Tables["hrm_nhanvien_tinhoc"];
                if (nvTinHoc.Rows.Count > 0)
                {
                    param["txttinhoc"] = nvTinHoc.Rows[0]["bangcap"];
                }

                DataTable nvNgoaiNgu = hoSoData.Tables["hrm_nhanvien_ngoaingu"];
                if (nvNgoaiNgu.Rows.Count > 0)
                {
                    param["txtngoaingu"] = nvNgoaiNgu.Rows[0]["ngoaingu_ten"];
                }

                //Replace shape
                _baseWord.FindAndReplaceShapeTextBox(wordApp, param);

                // Nếu tồn tại hình ảnh thì xử lý insert hình ảnh 
                if (nhanVienRow["anhemb"] != DBNull.Value)
                {
                    string fileImageTemp = Path.GetDirectoryName(_docTemplate) + "\\" + Guid.NewGuid() + ".JPG";

                    //Utilities ult = new DT.CommonLib.Utilities();
                    //System.Drawing.Image image = ult.ConvertBinaryToImage((byte[])nhanVienRow["anhemb"]);
                    //image.Save(fileImageTemp, ImageFormat.Jpeg);

                    // Insert image to doc
                    InlineShape inlineShape = aDoc.Tables[1].Cell(1, 1)
                      .Range.InlineShapes.AddPicture(fileImageTemp, ref missing, ref missing, ref missing);
                    inlineShape.Width = 100;
                    inlineShape.Height = 120;

                    //// Xóa hình tạm:
                    FileInfo f = new FileInfo(fileImageTemp);
                    f.Delete();
                }

                /* Fill quá trình công tác */
                wordns.Table docQtct = aDoc.Tables[6];
                DataTable nvQuaTrinhCongTac = hoSoData.Tables["HRM_NHANVIEN_QTCONGTAC"];

                int qtctRowIndex = 2;
                foreach (DataRow row in nvQuaTrinhCongTac.Rows)
                {
                    string tuNgay = row["ngaybatdau"].ToString();
                    string denNgay = row["ngayketthuc"].ToString();
                    string thoiGian = tuNgay.Length > 7 ? tuNgay.Substring(tuNgay.Length - 7) : tuNgay;
                    thoiGian = thoiGian + " - ";
                    thoiGian += denNgay.Length > 7 ? denNgay.Substring(denNgay.Length - 7) : denNgay;
                    wordns.Row docRow = docQtct.Rows[qtctRowIndex];
                    docRow.Cells[1].Range.Text = thoiGian;
                    docRow.Cells[2].Range.Text = row["congviec"].ToString();
                    qtctRowIndex += 1;
                }

                /* Fill quan hệ gia đình */
                DataTable nvQhgd = hoSoData.Tables["HRM_NHANVIEN_QUANHEGD"];
                if (nvQhgd.Rows.Count > 0)
                {
                    int qhBanThanRowIndex = 2;
                    int qhVoHoacChongRowIndex = 2;
                    wordns.Table docBanThan = aDoc.Tables[7];
                    wordns.Table docBenVoHoacChong = aDoc.Tables[8];
                    bool isNam = nhanVienRow["gioitinh"].Equals("Nam");
                    string banThan = isNam ? "0" : "1";
                    foreach (DataRow row in nvQhgd.Rows)
                    {
                        string quanHe = row["quanhe_ten"].ToString();
                        string hoTen = row["hoten"].ToString();
                        string namSinh = row["ngaysinh"].ToString();
                        namSinh = (namSinh.Length <= 4) ? namSinh : namSinh.Substring(0, 4); // Chỉ lấy năm sinh
                        string noiDung = string.Format("{0}, {1}", row["hkthuongtru"], row["nghenghiep"]);

                        if (row["benvo"].Equals(banThan)) /* Fill DL vào bảng quan hệ gia đình - Bản thân */
                        {
                            wordns.Row docBanThanRow = docBanThan.Rows[qhBanThanRowIndex];
                            docBanThanRow.Cells[1].Range.Text = quanHe;
                            docBanThanRow.Cells[2].Range.Text = hoTen;
                            docBanThanRow.Cells[3].Range.Text = namSinh;
                            docBanThanRow.Cells[4].Range.Text = noiDung;
                            qhBanThanRowIndex += 1;
                        }
                        else /* Fill DL vào bảng quan hệ gia đình - Bên vợ | chồng */
                        {
                            wordns.Row docVoHoacChongRow = docBenVoHoacChong.Rows[qhVoHoacChongRowIndex];
                            docVoHoacChongRow.Cells[1].Range.Text = quanHe;
                            docVoHoacChongRow.Cells[2].Range.Text = hoTen;
                            docVoHoacChongRow.Cells[3].Range.Text = namSinh;
                            docVoHoacChongRow.Cells[4].Range.Text = noiDung;
                            qhVoHoacChongRowIndex += 1;
                        }
                    }
                }

                aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing);

                aDoc.Close(ref missing, ref missing, ref missing);

                //Open the document as the correct file name
                wordApp.Documents.Open(ref saveAs, ref missing,
                  ref readOnly, ref missing, ref missing, ref missing,
                  ref missing, ref missing, ref missing, ref missing,
                  ref missing, ref isVisible, ref missing, ref missing,
                  ref missing, ref missing);

                wordApp.Visible = true;

                result = true;
            }
            catch (Exception ex)
            {

                aDoc = null;
                wordApp.Documents.Close();
            }
            return result;
        }



        #endregion


    }
}
