using System;
using System.Drawing.Imaging;
using Microsoft.Office.Interop.Word;
using System.IO;
using System.Collections;
using System.Data;
using RICONS.Core.Functions;
using DataTable = System.Data.DataTable;

namespace ExportHelper.Repository
{
    public class BaseWord
    {

        public BaseWord()
        {

        }

        /// <summary>
        /// This is simply a helper method to find/replace text. 
        /// Thay thế chuổi dạng text
        /// </summary>
        /// <param name="wordApp">Word Application to use</param>
        /// <param name="findText">Text to find</param>
        /// <param name="replaceWithText">Replacement text</param>
        public bool FindAndReplace(Application wordApp, object findText, object replaceWithText)
        {
            bool isresult = true;
            try
            {
                //Phan biet chu hoa hay chu thuong
                object matchCase = true;
                object matchWholeWord = true;
                object matchWildCards = false;
                object matchSoundsLike = false;
                object nmatchAllWordForms = false;
                object forward = true;
                object format = false;
                object matchKashida = false;
                object matchDiacritics = false;
                object matchAlefHamza = false;
                object matchControl = false;
                object read_only = false;
                object visible = true;
                object replace = 2;
                object wrap = 1;

                wordApp.Selection.Find.Execute(ref findText,
                                               ref matchCase, ref matchWholeWord,
                                               ref matchWildCards, ref matchSoundsLike,
                                               ref nmatchAllWordForms, ref forward,
                                               ref wrap, ref format, ref replaceWithText,
                                               ref replace, ref matchKashida,
                                               ref matchDiacritics, ref matchAlefHamza,
                                               ref matchControl);
            }
            catch (Exception ex)
            {
                isresult = false;

            }
            return isresult;
        }

        public bool FindAndReplaceShapeTextBox(Application wordApp, object[][] textreplace, int rows)
        {
            bool isresult = true;
            try
            {

                foreach (Microsoft.Office.Interop.Word.Shape s in wordApp.ActiveDocument.Shapes)
                {
                    if (s.Type == Microsoft.Office.Core.MsoShapeType.msoTextBox)
                    {

                        // do something with shape.TextFrame.TextRange.Text
                        for (int i = 0; i < rows; i++)
                        {
                            if (s.AlternativeText.ToLower() == textreplace[i][0].ToString().ToLower())
                            {
                                s.TextFrame.TextRange.Text = textreplace[i][1].ToString();
                                break;
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                isresult = false;


            }
            return isresult;
        }

        public bool FindAndReplaceShapeTextBox(Application wordApp, Hashtable paramvalue)
        {
            bool isresult = true;
            try
            {

                ArrayList columns = new ArrayList(paramvalue.Keys);

                foreach (Microsoft.Office.Interop.Word.Shape s in wordApp.ActiveDocument.Shapes)
                {

                    if (paramvalue.Count == 0 || columns.Count == 0)
                        break;
                    if (s.Type == Microsoft.Office.Core.MsoShapeType.msoTextBox)
                    {

                        // do something with shape.TextFrame.TextRange.Text
                        for (int i = columns.Count - 1; i >= 0; i--)
                        {
                            try
                            {
                                if (s.AlternativeText.ToLower() == columns[i].ToString().ToLower())
                                {
                                    s.TextFrame.TextRange.Text = paramvalue[columns[i].ToString()].ToString();
                                    paramvalue.Remove(columns[i]);
                                    columns.RemoveAt(i);
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {


                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                isresult = false;


            }
            return isresult;
        }

        /// <summary>
        /// Phương thức chuyển các cột dữ liệu của dataRow thành tham số hashtable
        /// </summary>
        public Hashtable GetTextBoxParams(DataRow dataRow)
        {
            Hashtable param = new Hashtable();
            foreach (DataColumn column in dataRow.Table.Columns)
            {
                if (column.DataType == typeof(byte[]))
                    continue;

                string name = column.ColumnName;
                param["txt" + name] = dataRow[name];
            }
            return param;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="saveAs"></param>
        /// <param name="_textreplace"></param>
        /// <param name="_textboxreplace"></param>
        /// <returns></returns>
        public bool CreateWordDocument(
          string _fileName
          , object[][] _textreplace, int _textreplacerows
          , object[][] _textboxreplace, int _textboxreplacerows
          )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }

            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {


                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;
                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Insert row,column

                    //Replace shape
                    if (_textboxreplace != null && _textboxreplacerows > 0)
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace, _textboxreplacerows);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplacerows > 0)
                    {
                        for (int i = 0; i < _textreplacerows; i++)
                        {
                            FindAndReplace(wordApp, _textreplace[i][0].ToString(), _textreplace[i][1].ToString());
                        }
                    }
                    saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + aDoc.Name;

                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocument(
          string _fileName
          , Hashtable _textreplace
          , Hashtable _textboxreplace
          , DataSet dtsDetail
          )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0)
                    {
                        //Get the first table in the document
                        for (int k = 1; k <= tables.Count; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataRowCollection _row = dtsDetail.Tables[k - 1].Rows;
                            for (int i = 0; i < _row.Count; i++)
                            {
                                Row row = table.Rows.Add(ref missing);

                                for (int j = 1; j <= coulmnsCount; j++)
                                {
                                    row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                    row.Cells[j].WordWrap = true;
                                    row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                    row.Cells[j].Range.Bold = 0;
                                }
                            }
                        }
                    }
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + aDoc.Name;
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocument(
          string _fileName
          , Hashtable _textreplace
          , Hashtable _textboxreplace
          )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + aDoc.Name;
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocument_xuphatvanhoa(
       string _fileName
      , Hashtable _textboxreplace
      , Hashtable _textreplace
      , DataTable tblDetail
      , DataTable tblDetail2
      , DataTable tblDetail3
      , DataTable tblDetail4
      , string tentemplate
      )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();


            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("yyyyMMdd hhmmss") + tentemplate;
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;

            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k <= 4; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();
                            if (k == 2)
                                tbl = tblDetail2.Copy();
                            if (k == 3)
                                tbl = tblDetail3.Copy();
                            if (k == 4)
                            {
                                if (tblDetail4 != null && tblDetail4.Rows[0]["thongtincoso4"].ToString().Trim() != "")
                                    tbl = tblDetail4.Copy();
                                else tbl = null;
                            }
                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                tbl.AcceptChanges();
                                DataRowCollection _row = tbl.Rows;
                                int _columnsCount = tbl.Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    //if (tblDetail.Columns.Contains("ORDERING"))
                                    //    _row[i]["ORDERING"] = (i + 1).ToString();
                                    Row row = table.Rows.Add(ref missing);
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                    if (k == 4)
                                        table.Rows[2].Delete();
                                    else table.Rows.First.Delete();
                                }
                            }
                            else if (tbl == null)
                            {
                                table.Delete();
                            }
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }


        public bool CreateWordDocument_xuphatdiachinh(
        string _fileName
       , Hashtable _textboxreplace
       , Hashtable _textreplace
       , DataTable tblDetail
       , DataTable tblDetail2
       , DataTable tblDetail3
       , DataTable tblDetail4
       )
        {
            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k <= 4; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();
                            if (k == 2)
                                tbl = tblDetail2.Copy();
                            if (k == 3)
                                tbl = tblDetail3.Copy();
                            if (k == 4)
                            {
                                if (tblDetail4 != null && tblDetail4.Rows[0]["thongtincoso4"].ToString().Trim() != "")
                                    tbl = tblDetail4.Copy();
                                else tbl = null;
                            }
                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                tbl.AcceptChanges();
                                DataRowCollection _row = tbl.Rows;
                                int _columnsCount = tbl.Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    //if (tblDetail.Columns.Contains("ORDERING"))
                                    //    _row[i]["ORDERING"] = (i + 1).ToString();
                                    Row row = table.Rows.Add(ref missing);
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                    if (k == 4)
                                        table.Rows[2].Delete();
                                    else table.Rows.First.Delete();
                                }
                            }
                            else table.Delete();
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocument_xuphattrattudothi(
        string _fileName
        , Hashtable _textboxreplace
        , Hashtable _textreplace
        , DataTable tblDetail
        )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k <= 1; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();

                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                tbl.AcceptChanges();
                                DataRowCollection _row = tbl.Rows;
                                int _columnsCount = tbl.Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    //if (tblDetail.Columns.Contains("ORDERING"))
                                    //    _row[i]["ORDERING"] = (i + 1).ToString();
                                    Row row = table.Rows.Add(ref missing);
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                    if (k == 4)
                                        table.Rows[2].Delete();
                                    else table.Rows.First.Delete();
                                }
                            }
                            else table.Delete();
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocumentsbks(
       string _fileName
      , Hashtable _textboxreplace
      , Hashtable _textreplace
      , DataTable tblDetail
      )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k < 2; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();
                            tbl.AcceptChanges();
                            DataRowCollection _row = tbl.Rows;
                            int _columnsCount = tbl.Columns.Count;
                            for (int i = 1; i < _row.Count; i++)
                            {
                                //if (tblDetail.Columns.Contains("ORDERING"))
                                //    _row[i]["ORDERING"] = (i + 1).ToString();
                                Row row = table.Rows.Add(ref missing);
                                for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                {
                                    row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                    row.Cells[j].WordWrap = true;
                                    //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                    row.Cells[j].Range.Bold = 0;
                                }
                            }
                            table.Rows.First.Delete();
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocumenVanbantraloi(
     string _fileName
    , Hashtable _textboxreplace
    , Hashtable _textreplace
    , DataTable tblDetail
    )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        Table table = tables[1];
                        int rowsCount = table.Rows.Count;
                        int coulmnsCount = table.Columns.Count;
                        DataRowCollection _row = tblDetail.Rows;
                        int _columnsCount = tblDetail.Columns.Count;
                        for (int i = 0; i < _row.Count; i++)
                        {
                            Row row = table.Rows.Add(ref missing);
                            for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                            {
                                row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                row.Cells[j].WordWrap = true;
                                row.Cells[j].Range.Bold = 0;
                            }
                        }
                        table.Rows.First.Delete();
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocument_totrinhtrattudothi(
    string _fileName
    , Hashtable _textboxreplace
    , Hashtable _textreplace
    , DataTable tblDetail
    , DataTable tblDetail2
    )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k <= 4; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();
                            if (k == 2)
                                tbl = tblDetail2.Copy();
                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                tbl.AcceptChanges();
                                DataRowCollection _row = tbl.Rows;
                                int _columnsCount = tbl.Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    //if (tblDetail.Columns.Contains("ORDERING"))
                                    //    _row[i]["ORDERING"] = (i + 1).ToString();
                                    Row row = table.Rows.Add(ref missing);
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                    if (k == 4)
                                        table.Rows[2].Delete();
                                    else table.Rows.First.Delete();
                                }
                            }
                            else table.Delete();
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocumentsbks(
       string _fileName
      , Hashtable _textboxreplace
      , Hashtable _textreplace
      , DataTable tblDetail
      , DataTable tblDetail1
      )
        {
            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k < 3; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();
                            if (k == 2)
                                tbl = tblDetail1.Copy();
                            tbl.AcceptChanges();
                            DataRowCollection _row = tbl.Rows;
                            int _columnsCount = tbl.Columns.Count;
                            for (int i = 0; i < _row.Count; i++)
                            {
                                //if (tblDetail.Columns.Contains("ORDERING"))
                                //    _row[i]["ORDERING"] = (i + 1).ToString();
                                Row row = table.Rows.Add(ref missing);
                                for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                {
                                    row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                    row.Cells[j].WordWrap = true;
                                    //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                    row.Cells[j].Range.Bold = 0;
                                }
                            }
                            table.Rows.First.Delete();
                        }


                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        //In so bo khai sinh
        public bool CreateWordDocumentsbkhaisinh(
        string _fileName
       , Hashtable _textreplace
       , Hashtable _textboxreplace
       , DataTable tblDetail
       , int position
       )
        {
            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        Table table = tables[position];
                        int rowsCount = table.Rows.Count;
                        int coulmnsCount = table.Columns.Count;
                        DataRowCollection _row = tblDetail.Rows;
                        int _columnsCount = tblDetail.Columns.Count;
                        for (int i = 0; i < _row.Count; i++)
                        {
                            //if (tblDetail.Columns.Contains("ORDERING"))
                            //    _row[i]["ORDERING"] = (i + 1).ToString();
                            Row row = table.Rows.Add(ref missing);
                            for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                            {
                                row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                row.Cells[j].WordWrap = true;
                                //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                row.Cells[j].Range.Bold = 0;
                            }
                        }
                        try
                        {
                            Cell cell;

                            cell = table.Cell(1, 3);
                            cell.Merge(table.Cell(1, 4));//Ngay sinh ghep lai
                            cell = table.Cell(1, 5);
                            cell.Merge(table.Cell(2, 6));//Ho ten cha me
                            cell = table.Cell(1, 6);
                            cell.Merge(table.Cell(2, 7));//Ngay sinh cha me
                            cell = table.Cell(1, 7);
                            cell.Merge(table.Cell(2, 8));//Dan toc cha me
                            cell = table.Cell(1, 8);
                            cell.Merge(table.Cell(2, 9));//Thuong tru tam tru cha me
                            cell = table.Cell(1, 9);
                            cell.Merge(table.Cell(2, 10));//Số/quyen số
                            cell = table.Cell(1, 10);
                            cell.Merge(table.Cell(2, 11));//Số/quyen số
                            int cot = 4;
                            for (int dong = 3; dong <= _row.Count + 2; dong = dong + 2)
                            {
                                //cell = table.Cell(dong, 1);
                                //cell.Merge(table.Cell(cot, 1));//Số/quyen số

                                cell = table.Cell(dong, 2);
                                cell.Merge(table.Cell(cot, 2));//Ho ten

                                cell = table.Cell(dong, 3);
                                cell.Merge(table.Cell(dong, 4));//Ngay sinh

                                cell = table.Cell(dong, 4);
                                cell.Merge(table.Cell(cot, 5));//Dan toc, quoc tich

                                cell = table.Cell(dong, 5);
                                cell.Merge(table.Cell(cot, 6));// Ho ten cha me

                                cell = table.Cell(dong, 6);
                                cell.Merge(table.Cell(cot, 7));//Ngay sinh cham me
                                cell = table.Cell(dong, 7);
                                cell.Merge(table.Cell(cot, 8));// Dan toc cha me

                                cell = table.Cell(dong, 8);
                                cell.Merge(table.Cell(cot, 9));//Thuong tru, tạm trú cha mẹ

                                cell = table.Cell(dong, 9);
                                cell.Merge(table.Cell(cot, 10));//Ho ten, ngay sinh, cmnd nguoi di khai

                                cell = table.Cell(dong, 10);
                                cell.Merge(table.Cell(cot, 11));//Ho ten can bo ho tich

                                cot = cot + 2;
                            }
                        }
                        catch (Exception ex) { }

                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }
        //In so bo khai tu
        public bool CreateWordDocumentsbkt(
         string _fileName
        , Hashtable _textreplace
        , Hashtable _textboxreplace
        , DataTable tblDetail
        , int position
        )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        Table table = tables[position];
                        int rowsCount = table.Rows.Count;
                        int coulmnsCount = table.Columns.Count;
                        DataRowCollection _row = tblDetail.Rows;
                        int _columnsCount = tblDetail.Columns.Count;
                        for (int i = 0; i < _row.Count; i++)
                        {
                            //if (tblDetail.Columns.Contains("ORDERING"))
                            //    _row[i]["ORDERING"] = (i + 1).ToString();
                            Row row = table.Rows.Add(ref missing);
                            for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                            {
                                row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                row.Cells[j].WordWrap = true;
                                //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                row.Cells[j].Range.Bold = 0;
                            }
                        }
                        try
                        {
                            Cell cell;
                            int cot = 4;
                            for (int dong = 3; dong <= _row.Count + 2; dong = dong + 2)
                            {
                                //cell = table.Cell(dong, 1);
                                //cell.Merge(table.Cell(cot, 1));//Số/quyen số

                                cell = table.Cell(dong, 2);
                                cell.Merge(table.Cell(cot, 2));//Ngay cap

                                cell = table.Cell(dong, 3);
                                cell.Merge(table.Cell(cot, 3));//ho va ten

                                cell = table.Cell(dong, 4);
                                cell.Merge(table.Cell(cot, 4));//Nam sinh

                                cell = table.Cell(dong, 5);
                                cell.Merge(table.Cell(cot, 5));// dan tộc

                                cell = table.Cell(dong, 6);
                                cell.Merge(table.Cell(cot, 6));//Que quan

                                cell = table.Cell(dong, 7);
                                cell.Merge(table.Cell(cot, 7));// dia chỉ

                                cell = table.Cell(dong, 8);
                                cell.Merge(table.Cell(cot, 8));//cmnd

                                cot = cot + 2;
                            }
                        }
                        catch (Exception ex) { }

                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        //In so bo ket hon
        public bool CreateWordDocumentkethon(
         string _fileName
        , Hashtable _textreplace
        , Hashtable _textboxreplace
        , DataTable tblDetail
        , int position
        )
        {
            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        Table table = tables[position];
                        int rowsCount = table.Rows.Count;
                        int coulmnsCount = table.Columns.Count;
                        DataRowCollection _row = tblDetail.Rows;
                        int _columnsCount = tblDetail.Columns.Count;
                        for (int i = 0; i < _row.Count; i++)
                        {
                            //if (tblDetail.Columns.Contains("ORDERING"))
                            //    _row[i]["ORDERING"] = (i + 1).ToString();
                            Row row = table.Rows.Add(ref missing);
                            for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                            {
                                row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                row.Cells[j].WordWrap = true;
                                //row.Cells[j].Range.Underline = WdUnderline.wdUnderlineNone;
                                row.Cells[j].Range.Bold = 0;
                            }
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }


        //In bieu mau Nhadat040000_02.01.TD
        public bool CreateWordDocument(
                string _fileName
               , Hashtable _textreplace
               , Hashtable _textboxreplace
               , DataTable tblDetail
               , int position
               )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        Table table = tables[position];
                        int rowsCount = table.Rows.Count;
                        int coulmnsCount = table.Columns.Count;
                        DataRowCollection _row = tblDetail.Rows;
                        int _columnsCount = tblDetail.Columns.Count;
                        for (int i = 0; i < _row.Count; i++)
                        {

                            Row row = table.Rows.Add(ref missing);
                            for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                            {
                                row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                row.Cells[j].WordWrap = true;
                                row.Cells[j].Range.Bold = 0;
                            }
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }
        public bool CreateWordDocument(
               string _fileName
              , Hashtable _textreplace
              , Hashtable _textboxreplace
              , DataSet tblDetail
              , ArrayList position
              , Hashtable arraytablename
              )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int vt = 0; vt < position.Count; vt++)
                        {

                            Table table = tables[int.Parse(position[vt].ToString())];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            {
                                DataRowCollection _row = tblDetail.Tables[arraytablename[position[vt].ToString()].ToString()].Rows;
                                int _columnsCount = tblDetail.Tables[arraytablename[position[vt].ToString()].ToString()].Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    //if (i < rowsCount)
                                    //    row = table.Rows[i];
                                    //else
                                    Row row = table.Rows.Add(ref missing);
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                }
                            }
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        //In phieu hen
        public bool CreateWordDocumentphieuhen(
         string _fileName
        , Hashtable _textreplace
        , Hashtable _textboxreplace
        , DataTable tblDetail
        , int position
        , string ten
        )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + ten.ToString() + DateTime.Now.ToString("ddmmYYYY") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();

                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        Table table = tables[position];
                        int rowsCount = table.Rows.Count;
                        int coulmnsCount = table.Columns.Count;
                        DataRowCollection _row = tblDetail.Rows;
                        int _columnsCount = tblDetail.Columns.Count;
                        for (int i = 0; i < _row.Count; i++)
                        {
                            Row row = table.Rows.Add(ref missing);
                            for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                            {
                                row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                row.Cells[j].WordWrap = true;
                                row.Cells[j].Range.Bold = 0;
                            }
                        }
                        try
                        {
                            Cell cell;
                            cell = table.Cell(1, 1);
                            cell.Merge(table.Cell(1, 2));

                        }
                        catch (Exception ex) { }

                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        //Thong ke ket hon (dan so)
        public bool CreateWordThongKeKetHon(
          string _fileName
         , Hashtable _textreplace
         , Hashtable _textboxreplace
         )
        {

            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    saveAs = folder + DateTime.Now.ToString("ddMMyyyy HHmmss") + aDoc.Name;
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        /// <summary>
        ///  Set table RowStyle
        /// </summary>
        public void SetTableRowStyle(Row row, int fontSize, bool isBold, WdColor backgroundColor)
        {
            row.Range.Font.Bold = isBold ? 1 : 0;
            row.Range.Font.Size = fontSize;
            row.Range.Shading.BackgroundPatternColor = backgroundColor;
        }

        /// <summary>
        ///  Set table RowStyle
        /// </summary>
        public void SetTableRowStyle(Microsoft.Office.Interop.Word.Row row, int fontSize, bool isBold, WdColor backgroundColor, WdColor foreColor, WdParagraphAlignment alignment)
        {
            row.Range.Font.Bold = isBold ? 1 : 0;
            row.Range.Font.Size = fontSize;
            row.Range.Shading.BackgroundPatternColor = backgroundColor;
            row.Range.Shading.ForegroundPatternColor = foreColor;
            row.Range.ParagraphFormat.Alignment = alignment;
        }


        /// <summary>
        ///  Set table CellStyle
        /// </summary>
        public void SetTableCellStyle(Cell cell, string text, int fontSize, bool isBold, WdColor backgroundColor, WdColor foreColor)
        {
            cell.WordWrap = true;
            cell.Range.Text = text;
            cell.Range.Font.Bold = isBold ? 1 : 0;
            cell.Range.Font.Size = fontSize;
            cell.Range.Shading.BackgroundPatternColor = backgroundColor;
            cell.Range.Shading.ForegroundPatternColor = foreColor;
        }


        /// <summary>
        ///  Set table CellStyle
        /// </summary>
        public void SetTableCellStyle(Microsoft.Office.Interop.Word.Cell cell, string text, int fontSize, bool isBold, WdColor backgroundColor, WdColor foreColor, WdParagraphAlignment alignment)
        {
            cell.WordWrap = true;
            cell.Range.Text = text;
            cell.Range.Font.Bold = isBold ? 1 : 0;
            cell.Range.Font.Size = fontSize;
            cell.Range.Shading.BackgroundPatternColor = backgroundColor;
            cell.Range.Shading.ForegroundPatternColor = foreColor;
            cell.Range.ParagraphFormat.Alignment = alignment;
        }

        #region Diachinh0104
        public bool CreateWordDocumentTableFormat(
    string _fileName
    , Hashtable _textboxreplace
    , Hashtable _textreplace
    , ArrayList rowstable
    , Hashtable rowsvalue
            , int tableindex
    )
        {
            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0)
                    {
                        Table table = tables[tableindex];
                        for (int i = 0; i < rowstable.Count; i++)
                        {
                            int rowsindex = int.Parse(rowstable[i].ToString());
                            Row row = table.Rows[rowsindex];
                            row.Cells[1].Range.Text = rowsvalue[rowstable[i].ToString()].ToString();
                        }
                    }

                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }
        #endregion


        public bool CreateWordDocumentchungthuc(
          string _fileName
          , Hashtable _textreplace
          , Hashtable _textboxreplace
          , DataTable tblchungthuca
          , DataTable tblchungthucb
          )
        {

            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblchungthuca != null)
                    {
                        for (int k = 1; k <= 2; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblchungthuca.Copy();
                            if (k == 2)
                                tbl = tblchungthucb.Copy();
                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                tbl.AcceptChanges();
                                DataRowCollection _row = tbl.Rows;
                                int _columnsCount = tbl.Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    Row row = table.Rows.Add(ref missing);
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                }
                                table.Rows.First.Delete();
                            }
                            else if (tbl == null)
                            {
                                table.Delete();
                            }
                        }
                    }
                    saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + aDoc.Name;
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocumentthongtincoban(
      string _fileName
     , Hashtable _textboxreplace
     , Hashtable _textreplace
     , DataTable tblDetail
     )
        {
            string folder = Constants.FolderTempate;


            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Microsoft.Office.Interop.Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Microsoft.Office.Interop.Word.Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                        ref readOnly, ref missing, ref missing, ref missing,
                        ref missing, ref missing, ref missing, ref missing,
                        ref missing, ref isVisible, ref missing, ref missing,
                        ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblDetail != null)
                    {
                        for (int k = 1; k < 2; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblDetail.Copy();
                            tbl.AcceptChanges();
                            DataRowCollection _row = tbl.Rows;
                            int _columnsCount = tbl.Columns.Count;
                            for (int i = 0; i < _row.Count; i++)
                            {
                                Row row = table.Rows.Add(ref missing);
                                for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                {
                                    row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                    row.Cells[j].WordWrap = true;
                                    row.Cells[j].Range.Bold = 0;
                                }
                            }
                            table.Rows[2].Delete();
                        }
                    }
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing,
                            ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                       ref readOnly, ref missing, ref missing, ref missing,
                       ref missing, ref missing, ref missing, ref missing,
                       ref missing, ref isVisible, ref missing, ref missing,
                       ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }

        public bool CreateWordDocumen_chungthuc(
        string _fileName
        , Hashtable _textreplace
        , Hashtable _textboxreplace
        , DataTable tblchungthuca
        , int delete
        )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }
                    //Insert row,column
                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0 && tblchungthuca != null)
                    {
                        for (int k = 1; k <= 1; k++)
                        {
                            Table table = tables[k];
                            int rowsCount = table.Rows.Count;
                            int coulmnsCount = table.Columns.Count;
                            DataTable tbl = new DataTable();
                            if (k == 1)
                                tbl = tblchungthuca.Copy();

                            if (tbl != null && tbl.Rows.Count > 0)
                            {
                                tbl.AcceptChanges();
                                DataRowCollection _row = tbl.Rows;
                                int _columnsCount = tbl.Columns.Count;
                                for (int i = 0; i < _row.Count; i++)
                                {
                                    Row row = table.Rows.Add(ref missing);
                                    row.Height = 83;
                                    for (int j = 1; j <= coulmnsCount && j <= _columnsCount; j++)
                                    {
                                        row.Cells[j].Range.Text = string.Format(_row[i][j - 1].ToString());
                                        row.Cells[j].WordWrap = true;
                                        row.Cells[j].Range.Bold = 0;
                                    }
                                }
                                table.Rows[delete].Delete();
                            }
                            else if (tbl == null)
                            {
                                table.Delete();
                            }
                        }
                    }
                    saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + aDoc.Name;
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    aDoc = wordApp.Documents.Open(ref saveAs, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    wordApp.Visible = true;
                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }


        public bool CreateWordDocument2C(
          string _fileName
          , Hashtable _textreplace
          , Hashtable _textboxreplace
            , DataSet dts2C
          )
        {

            string folder = Constants.FolderTempate;
            //
            //    
            object fileName = _fileName.ToString();
            object saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + "print.doc";
            if (!Directory.Exists(folder + "TMP"))
            {
                Directory.CreateDirectory(folder + "TMP");
            }
            bool isresult = true;
            //Set Missing Value parameter - used to represent
            // a missing value when calling methods through
            // interop.
            object missing = System.Reflection.Missing.Value;
            //Setup the Word.Application class.
            Application wordApp = new Microsoft.Office.Interop.Word.Application();

            //Setup our Word.Document class we'll use.
            Document aDoc = null;
            try
            {
                // Check to see that file exists
                if (File.Exists((string)fileName))
                {
                    object readOnly = false;
                    object isVisible = false;

                    //Set Word to be not visible.
                    wordApp.Visible = false;

                    //Open the word document
                    aDoc = wordApp.Documents.Open(ref fileName, ref missing,
                                                  ref readOnly, ref missing, ref missing, ref missing,
                                                  ref missing, ref missing, ref missing, ref missing,
                                                  ref missing, ref isVisible, ref missing, ref missing,
                                                  ref missing, ref missing);
                    // Activate the document
                    aDoc.Activate();
                    //Replace shape
                    if ((_textboxreplace != null) && (_textboxreplace.Count > 0))
                    {
                        FindAndReplaceShapeTextBox(wordApp, _textboxreplace);
                    }
                    //Replace Text
                    if (_textreplace != null && _textreplace.Count > 0)
                    {
                        ArrayList columns = new ArrayList(_textreplace.Keys);
                        for (int i = 0; i < columns.Count; i++)
                        {
                            FindAndReplace(wordApp, columns[i].ToString(), _textreplace[columns[i].ToString()].ToString());
                        }
                    }

                    #region Table 2C#


                    Tables tables = aDoc.Tables;
                    if (tables.Count > 0)
                    { //Quá trình học tập
                        Table tblhocvan = tables[1];
                        for (int i = 0; i < dts2C.Tables["HRM_NHANVIEN_HOCVAN"].Rows.Count; i++)
                        {
                            DataRow rowtmp = dts2C.Tables["HRM_NHANVIEN_HOCVAN"].Rows[i];
                            Row row = tblhocvan.Rows.Add(ref missing);
                            row.Cells[1].Range.Text = rowtmp["tentruong"].ToString();
                            row.Cells[2].Range.Text = rowtmp["nganhhoc"].ToString();
                            row.Cells[3].Range.Text = rowtmp["ngaybatdau"].ToString() + " -" + rowtmp["ngayketthuc"].ToString();
                            row.Cells[4].Range.Text = rowtmp["nganhan"].ToString();
                            row.Cells[5].Range.Text = rowtmp["tenloaibang"].ToString();
                        }
                        //Lý lịch làm việc
                        tblhocvan = tables[2];
                        for (int i = 0; i < dts2C.Tables["HRM_NHANVIEN_CTY"].Rows.Count; i++)
                        {
                            DataRow rowtmp = dts2C.Tables["HRM_NHANVIEN_CTY"].Rows[i];
                            Row row = tblhocvan.Rows.Add(ref missing);
                            row.Cells[1].Range.Text = rowtmp["ngaybatdau"].ToString() + " -" + rowtmp["ngayketthuc"].ToString();
                            row.Cells[2].Range.Text = rowtmp["tencongty"].ToString() + ", chức vụ:" + rowtmp["chucvu"].ToString()
                                + ", công việc:" + rowtmp["congviec"].ToString();
                        }
                        //Quan hệ gia đình
                        tblhocvan = tables[3];
                        for (int i = 0; i < dts2C.Tables["HRM_NHANVIEN_GIADINH"].Rows.Count; i++)
                        {
                            DataRow rowtmp = dts2C.Tables["HRM_NHANVIEN_GIADINH"].Rows[i];
                            Row row = tblhocvan.Rows.Add(ref missing);
                            row.Cells[1].Range.Text = rowtmp["tenquanhe"].ToString();
                            row.Cells[2].Range.Text = rowtmp["hoten"].ToString();
                            row.Cells[3].Range.Text = rowtmp["ngaysinh"].ToString();
                            row.Cells[4].Range.Text = rowtmp["nghenghiep"].ToString();
                        }
                        //Ben vo
                        tblhocvan = tables[4];
                        for (int i = 0; i < dts2C.Tables["HRM_NHANVIEN_GIADINH1"].Rows.Count; i++)
                        {
                            DataRow rowtmp = dts2C.Tables["HRM_NHANVIEN_GIADINH1"].Rows[i];
                            Row row = tblhocvan.Rows.Add(ref missing);
                            row.Cells[1].Range.Text = rowtmp["tenquanhe"].ToString();
                            row.Cells[2].Range.Text = rowtmp["hoten"].ToString();
                            row.Cells[3].Range.Text = rowtmp["ngaysinh"].ToString();
                            row.Cells[4].Range.Text = rowtmp["nghenghiep"].ToString();
                        }
                        //Luong
                        //tblhocvan = tables[5];
                        //for (int i = 0; i < dts2C.Tables["doanvien_bangluong"].Rows.Count; i++)
                        //{
                        //    DataRow rowtmp = dts2C.Tables["doanvien_bangluong"].Rows[i];
                        //    Column col = tblhocvan.Columns.Add(ref missing);
                        //    col.Cells[1].Range.Text = rowtmp["thangnam"].ToString();
                        //    col.Cells[2].Range.Text = rowtmp["ngachbac"].ToString();
                        //    col.Cells[3].Range.Text = rowtmp["hesoluong"].ToString();
                        //}
                    }

                    #endregion

                    saveAs = folder + "TMP\\" + DateTime.Now.ToString("ddMMyyyy HHmmss") + "_" + aDoc.Name;
                    //Save the document as the correct file name.
                    aDoc.SaveAs(ref saveAs, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing,
                                ref missing, ref missing, ref missing, ref missing);
                    aDoc.Close(ref missing, ref missing, ref missing);
                    //Open the document as the correct file name
                    System.Diagnostics.Process.Start(saveAs.ToString());

                }
            }
            catch (Exception ex)
            {
                isresult = false;



            }
            try
            {
                if (wordApp != null)
                    wordApp = null;
            }
            catch (Exception)
            {
            }
            return isresult;

        }




    }
}
