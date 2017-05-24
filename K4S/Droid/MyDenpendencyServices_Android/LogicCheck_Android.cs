using System;
using System.Collections.Generic;
using System.Data;
using LoginXF.Droid;
using MyDependencyServices;
using Xamarin.Forms;
using System.Linq;
using K3S.Droid.Extensions;
using K3S.Model;
using K3S.Droid;
using Shared.Extensions;
using Acr.UserDialogs;
using System.Diagnostics;

[assembly: Dependency(typeof(LogicCheck_Android))]
namespace LoginXF.Droid
{
    public class LogicCheck_Android : ILogicCheck
    {
       
        public  string TextDisplay(string s, string AnswerID, List<String> listPageCodeAccessed, DataTable tblFinalResultValue,DataTable tblResultEmpty)
        {
             
            try
            {
                string tempS = s;

                if (tempS.Contains("$"))
                {
                    List<String> listContaint = new List<string>();
                   

                    Stopwatch w1 = new Stopwatch();
                    w1.Start();
                    //for (int i = data.Rows.Count - 1; i >= 0; i--)
                    //{
                    //    DataRow c_row = data.Rows[i];
                    //    if (listPageCodeAccessed.Contains(c_row.Item("PageCode")) == false)
                    //    {
                    //        data.Rows[i]["FieldValue"] = "";
                    //        data.Rows[i]["AnswerText"] = "";

                    //    }
                    //}
                    //w1.Stop();
                    //Debug.Write("s : " + w1.ElapsedMilliseconds);
                    //data.AcceptChanges();

                    var data = tblFinalResultValue.Copy();
                    DataTable result = PivotTable(data, tblResultEmpty);



                    //string temp = result.DataTableToJSON();
                    foreach (string sub_query in tempS.Split('$'))
                    {
                        if (listContaint.Contains(sub_query) == false)
                        {
                            if (sub_query.Contains("@"))
                            {
                                if (sub_query.Contains("#") == false)
                                {
                                    string a = sub_query.Split('@')[1];
                                 
                                    string value = data._FindValue("AnswerText", "FieldName", sub_query.Split('@')[1]);
                                    tempS = tempS.Replace("$" + sub_query + "$", value);
                                }
                                else
                                {
                                    string check = sub_query.Split('#')[1];
                                    string ifvalue = sub_query.Split('#')[2];
                                    string elsevalue = "";
                                    try
                                    {
                                        elsevalue = sub_query.Split('#')[3]; ;
                                    }
                                    catch
                                    {
                                        elsevalue = "";
                                    }

                                    if (result.Select(check).Count() > 0)
                                    {
                                        //tempS = ifvalue;
                                        if (ifvalue.Contains("@"))
                                        {

                                            var value = data._FindValue("AnswerText", "FieldName", ifvalue.Split('@')[1]);
                                            if (ifvalue.Contains("_K"))
                                            {
                                                value = data._FindValue("FieldValue", "FieldName", ifvalue.Split('@')[1]);
                                            }
                                            tempS = tempS.Replace("$" + sub_query + "$", value);

                                        }
                                        else
                                        {
                                            tempS = tempS.Replace("$" + sub_query + "$", ifvalue);
                                        }
                                    }
                                    else
                                    {
                                        tempS = tempS.Replace("$" + sub_query + "$", elsevalue);
                                        //tempS = elsevalue;
                                    }

                                }
                            }
                            else
                            {

                            }
                        }
                    }


                }
                else
                {

                }
                return tempS;
            }
            catch(Exception ex)
            {
                return s;
            }

        }
        public string TextDisplay(string s, string AnswerID, List<String> listPageCodeAccessed, DataTable tblResultEmpty,List<RmGenericFormValueItem> listFinalResultValue)
        {

            try
            {
              
                string tempS = s;
                Stopwatch w1 = new Stopwatch();
                w1.Start();
                if (tempS.Contains("$"))
                {
                    List<String> listContaint = new List<string>();


                   
                 
             
                 
                    
                    foreach (string sub_query in tempS.Split('$'))
                    {
                        if (listContaint.Contains(sub_query) == false)
                        {
                            if (sub_query.Contains("@"))
                            {
                                if (sub_query.Contains("#") == false)
                                {
                                    string a = sub_query.Split('@')[1];

                                    string value = listFinalResultValue.Where(o => o.FieldName == a).ToList()[0].AnswerText;//  data._FindValue("AnswerText", "FieldName", sub_query.Split('@')[1]);
                                    tempS = tempS.Replace("$" + sub_query + "$", value);
                                }
                                else
                                {
                                    string check = sub_query.Split('#')[1];
                                    string ifvalue = sub_query.Split('#')[2];
                                    string elsevalue = "";
                                    try
                                    {
                                        elsevalue = sub_query.Split('#')[3]; ;
                                    }
                                    catch
                                    {
                                        elsevalue = "";
                                    }
                                    DataTable result = PivotTable(listFinalResultValue, tblResultEmpty);
                                    if (result.Select(check).Count() > 0)
                                    {
                                        //tempS = ifvalue;
                                        if (ifvalue.Contains("@"))
                                        {
                                            string findValue = ifvalue.Split('@')[1];
                                            string value = "";
                                            
                                            //var value =  data._FindValue("AnswerText", "FieldName", ifvalue.Split('@')[1]);
                                            if (ifvalue.Contains("_K"))
                                            {
                                                value = listFinalResultValue.Where(o => o.FieldName == findValue).ToList()[0].FieldValue;
                                                //value = data._FindValue("FieldValue", "FieldName", ifvalue.Split('@')[1]);
                                            }
                                            else
                                            {
                                                 value = listFinalResultValue.Where(o => o.FieldName == findValue).ToList()[0].AnswerText;
                                            }
                                            tempS = tempS.Replace("$" + sub_query + "$", value);

                                        }
                                        else
                                        {
                                            tempS = tempS.Replace("$" + sub_query + "$", ifvalue);
                                        }
                                    }
                                    else
                                    {
                                        tempS = tempS.Replace("$" + sub_query + "$", elsevalue);
                                        //tempS = elsevalue;
                                    }

                                }
                            }
                            else
                            {

                            }
                        }
                    }


                }
                else
                {

                }

                w1.Stop();
                Debug.Write("text display in " + w1.ElapsedMilliseconds);
                return tempS;
            }
            catch (Exception ex)
            {
                return s;
            }

        }
        public bool checkLogic(DataTable filterTable, DataTable dtValue, string _variableName, List<string> listAccessPageCode)
        {
            if (filterTable == null || dtValue == null)
                return true;
            DataRow[] logicRows = filterTable.Select(string.Format("VariableName='{0}'", _variableName));
            if (logicRows.Count() == 0)
            {
                //nextx page
                return true;
                // ko hien len
            }
            else
            {
                //var dtValue = PivotTable(dataTable);
                foreach (DataRow row in logicRows)
                {
                    //var s = dtValue.DataTableToJSON();
                    var a = row.Item("FilterCondition");
                    var resultlogic = dtValue.Select(row.Item("FilterCondition"));

                    if (resultlogic.Count() > 0)
                    {
                        return false;
                        //logic OK go to next page
                    }
                    else
                    {

                    }


                }
            }
            return true;
        }

        public DataTable PivotTable(List<RmGenericFormValueItem> dt, DataTable tblResultEmpty)
        {
            DataTable result = tblResultEmpty.Clone();
            Stopwatch w1 = new Stopwatch();
            w1.Start();

            DataRow new_row = result.NewRow();
            w1 = new Stopwatch();
            w1.Start();
            foreach (var row in dt)
            {
                DataColumn dc = result.Columns[row.FieldName];
                if (dc.DataType == typeof(Double))
                {
                    if (row.FieldValue.IsEmpty())
                    {
                        new_row[row.FieldName] = 0;
                    }
                    else
                    {
                        new_row[row.FieldName] = Convert.ToDouble(row.FieldValue);
                    }
                }
                else
                {

                    new_row[row.FieldName] = row.FieldValue;
                }
            }
            result.Rows.Add(new_row);
            result.AcceptChanges();
            return result;
            w1.Stop();
            System.Diagnostics.Debug.WriteLine("Logic s2: " + w1.ElapsedMilliseconds);
        }

        public DataTable PivotTable(DataTable dt, DataTable tblResultEmpty)
        {
            DataTable result = tblResultEmpty.Clone();
            Stopwatch w1 = new Stopwatch();
            w1.Start();
           
           DataRow new_row = result.NewRow();
            w1 = new Stopwatch();
            w1.Start();
            foreach (DataRow row in dt.Rows)
            {
                DataColumn dc = result.Columns[row.Item("FieldName")];
                if (dc.DataType == typeof(Double))
                {
                    if (row.Item("FieldValue").IsEmpty())
                    {
                        new_row[row.Item("FieldName")] = 0;
                    }
                    else
                    {
                        new_row[row.Item("FieldName")] = Convert.ToDouble(row.Item("FieldValue"));
                    }
                }
                else
                {
                   
                    new_row[row.Item("FieldName")] = row.Item("FieldValue");
                }
            }
            result.Rows.Add(new_row);
            result.AcceptChanges();
            return result;
            w1.Stop();
            System.Diagnostics.Debug.WriteLine("Logic s2: " + w1.ElapsedMilliseconds);
        }
    
        public DataTable ToTableQuestionList(List<RmGenericFormQuestionAndroidAnswerItem> list)
        {
            return list.ToTable<RmGenericFormQuestionAndroidAnswerItem>();
        }

        //public Dictionary<string, string> checkLogicCheckPostSkip(DataTable tblQuestion, List<RmGenericFormQuestionAndroidAnswerItem> listPage, List<RmGenericFormQuestionAndroidAnswerItem> listQuestion, List<RmGenericFormValueItem> listValue, List<RmLogicCheckItem> listLogicCheck, string _currPageCode, List<string> listPageCodeAccessed,DataTable tblResultEmpty)
        //{
        //    Stopwatch w1 = new Stopwatch();
        //    w1.Start();


        //    var result = new Dictionary<string, string>();
        //    try
        //    {
        //        var tblLogicCheck = listLogicCheck.ToTable<RmLogicCheckItem>();
        //        w1.Stop();
        //        Debug.Write("l0 " + w1.ElapsedMilliseconds);
        //        w1.Start();
        //        //var drsPage = listPage.ToTable<RmGenericFormQuestionAndroidAnswerItem>().Select("1=1");


        //        w1.Restart();
        //        DataRow[] logicRows = tblLogicCheck.Select(string.Format("FromPage='{0}' AND SkipType='{1}'", _currPageCode, "Post-Skip"));
        //        w1.Stop();

        //        Debug.Write("table logic selec " + w1.ElapsedMilliseconds);
        //        if (logicRows.Count() == 0)
        //        {
        //            //nextx page
        //            if (_currPageCode ==listPage[listPage.Count-1].QuestionCode)// drsPage[drsPage.Count() - 1].Item("QuestionCode"))
        //            {
        //                result.Add(_currPageCode, "");
        //                w1.Stop();
        //                Debug.Write("L1 " + w1.ElapsedMilliseconds);
        //                return result;
        //            }
        //            else
        //            {
        //                //vi tri row hien tai
        //                int index = _FindCurrentIndexPage(listPage, _currPageCode)+1;
        //                //var index = (drsPage._FindIndex("QuestionCode", _currPageCode) + 1);
        //                // kiem tra xem co nam trong danh sach cam ko?
        //                // neu ko return 
        //                // neu co tra ve trang tiep theo.. neu trang tiep theo thoa man chay vong lap
        //                //var nextQuestionCode = drsPage[index].Item("QuestionCode");
        //                var nextQuestionCode = listPage[index].QuestionCode;

        //                int _nextPage = 0;

        //                //int _currentPage = ConvertSafe.ToInt32(drsPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));

        //                int _currentPage = _FindOrderIndexOfQuestionCode(listPage, nextQuestionCode);
        //                if (_currentPage ==listPage[listPage.Count-1].OrderIndex)// (int)Convert.ToInt32((drsPage[drsPage.Count() - 1].Item("OrderIndex"))))
        //                {
        //                    _nextPage = 99999;
        //                }
        //                else
        //                {
        //                    //vi tri row hien tai

        //                    var code_next = listPage[index + 1].QuestionCode;// drsPage[index + 1].Item("QuestionCode");

        //                    _nextPage = ConvertSafe.ToInt32(tblQuestion._FindValue("OrderIndex", "QuestionCode", code_next));

        //                }

        //                //var tbltempPage = tblQuestion.Select(string.Format("OrderIndex>'{0}' AND OrderIndex<='{1}'", _currentPage, _nextPage));
        //                //string xxxx = tblQuestion.DataTableToJSON();


        //                if (tblQuestion.Rows[tblQuestion._FindIndex("QuestionCode", nextQuestionCode) + 1].Item("HideCondition").IsEmpty())
        //                {
        //                    result.Add(listPage[index].QuestionCode, "");/// drsPage[index].Item("QuestionCode"), "");
        //                    w1.Stop();
        //                    Debug.Write("L1 " + w1.ElapsedMilliseconds);
        //                    return result;//  return drsPage[index].Item("QuestionCode");
        //                }
        //                else
        //                {

        //                    for (int i = _currentPage; i <= tblQuestion.Rows.Count; i++)
        //                    {

        //                        int orderindexcurrent = tblQuestion._FindIndex("OrderIndex", i.ToString()) + 1;
        //                        //var x1 = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");
        //                        //int orderindexcurrent = tblQuestion._FindIndex("QuestionCode", tblQuestion.Rows[i].Item("QuestionCode"));
        //                        if (orderindexcurrent != -1)
        //                        {
        //                            if (tblQuestion.Rows[orderindexcurrent].Item("QuestionTypeID") != "0")
        //                            {
        //                                if (tblQuestion.Rows[orderindexcurrent].Item("HideCondition").IsEmpty())
        //                                {
        //                                    int returnIndex = Convert.ToInt32(tblQuestion.Rows[i].Item("OrderIndex")) - 1;

        //                                    result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
        //                                    w1.Stop();
        //                                    Debug.Write("L1 " + w1.ElapsedMilliseconds);
        //                                    return result;
        //                                    //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
        //                                }
        //                                else
        //                                {
        //                                    var data = listValue.ToTable<RmGenericFormValueItem>();
        //                                    for (int indexcount = data.Rows.Count - 1; indexcount >= 0; indexcount--)
        //                                    {
        //                                        DataRow c_row = data.Rows[indexcount];
        //                                        if (listPageCodeAccessed.Contains(c_row.Item("PageCode")) == false && c_row.Item("PageCode") != _currPageCode)
        //                                        {
        //                                            data.Rows[indexcount]["FieldValue"] = "";
        //                                            data.Rows[indexcount]["AnswerText"] = "";
        //                                        }
        //                                    }
        //                                    data.AcceptChanges();

        //                                    var dtValue = PivotTable(data,tblResultEmpty);
        //                                    //var sques = ConvertShortQuestion(tblQuestion);
        //                                    var code_current = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

        //                                    //var s = dtValue.DataTableToJSON();
        //                                    var logic = tblQuestion.Rows[orderindexcurrent].Item("HideCondition");
        //                                    var resultlogic = dtValue.Select(tblQuestion.Rows[orderindexcurrent].Item("HideCondition"));

        //                                    if (resultlogic.Count() == 0)
        //                                    {
        //                                        //var a = tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
        //                                        //var x2 = tblQuestion.DataTableToJSON();
        //                                        result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
        //                                        w1.Stop();
        //                                        Debug.Write("L1 " + w1.ElapsedMilliseconds);
        //                                        return result;// tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
        //                                    }

        //                                }
        //                            }
        //                        }
        //                    }

        //                    result.Add(listPage[index].QuestionCode, "");// drsPage[index].Item("QuestionCode"), "");
        //                    return result;// return drsPage[index].Item("QuestionCode");
        //                }
        //            }
        //        }
        //        else
        //        {
        //            var data = listValue.ToTable<RmGenericFormValueItem>();
        //            for (int indexcount = data.Rows.Count - 1; indexcount >= 0; indexcount--)
        //            {
        //                DataRow c_row = data.Rows[indexcount];
        //                if (listPageCodeAccessed.Contains(c_row.Item("PageCode")) == false && c_row.Item("PageCode") != _currPageCode)
        //                {
        //                    data.Rows[indexcount]["FieldValue"] = "";
        //                    data.Rows[indexcount]["AnswerText"] = "";
        //                }
        //                //else
        //                //{
        //                //    string x = c_row["FieldValue"].ToString();
        //                //}
        //            }
        //            data.AcceptChanges();


        //            var dtValue = PivotTable(data,tblResultEmpty);

        //            foreach (DataRow row in logicRows)
        //            {
        //                try
        //                {
        //                    var resultlogic = dtValue.Select(row.Item("SkipLogic"));

        //                    if (resultlogic.Count() == 0)
        //                    {
        //                        //return ConvertSafe.ToInt32(gvQuestion.GetDataRow(_focusRowIndex + 1).Item("PageOrderIndex"));
        //                        //logic OK go to next page

        //                    }
        //                    else
        //                    {
        //                        string errorComment = "";
        //                        if (row.Item("Comments").IsNotEmpty())
        //                        {
        //                            //IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
        //                            errorComment = row.Item("Comments");
        //                            //Dialogs.ShowError(row.Item("Comments"), 3000);

        //                        }
        //                        result.Add(row.Item("SkipToPage"), errorComment);
        //                        w1.Stop();
        //                        Debug.Write("L1 " + w1.ElapsedMilliseconds);
        //                        return result;
        //                        //return row.Item("SkipToPage");
        //                    }
        //                }
        //                catch (Exception ex)
        //                {

        //                }

        //            }

        //        }


        //        if (_currPageCode ==listPage[listPage.Count-1].QuestionCode)// drsPage[drsPage.Count() - 1].Item("QuestionCode"))
        //        {
        //            result.Add(_currPageCode, "");
        //            return result;
        //            //return _currPageCode;
        //        }
        //        else
        //        {
        //            //vi tri row hien tai
        //            #region check Hide condition

        //            var index = _FindCurrentIndexPage(listPage, _currPageCode) + 1;// (drsPage._FindIndex("QuestionCode", _currPageCode) + 1);
        //            // kiem tra xem co nam trong danh sach cam ko?
        //            // neu ko return 
        //            // neu co tra ve trang tiep theo.. neu trang tiep theo thoa man chay vong lap
        //            var nextQuestionCode = listPage[index].QuestionCode;// drsPage[index].Item("QuestionCode");

        //            int _nextPage = 0;

        //            int _currentPage = _FindOrderIndexOfQuestionCode(listPage, nextQuestionCode);// ConvertSafe.ToInt32(drsPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));

        //            if (_currentPage ==listPage[listPage.Count-1].OrderIndex)// (int)Convert.ToInt32((drsPage[drsPage.Count() - 1].Item("OrderIndex"))))
        //            {
        //                _nextPage = 99999;
        //            }
        //            else
        //            {
        //                //vi tri row hien tai

        //                var code_next = listPage[index + 1].QuestionCode;// drsPage[index + 1].Item("QuestionCode");

        //                _nextPage = ConvertSafe.ToInt32(tblQuestion._FindValue("OrderIndex", "QuestionCode", code_next));

        //            }

        //            var tbltempPage = tblQuestion.Select(string.Format("OrderIndex>'{0}' AND OrderIndex<='{1}'", _currentPage, _nextPage));

        //            if (tblQuestion.Rows[tblQuestion._FindIndex("QuestionCode", nextQuestionCode) + 1].Item("HideCondition").IsEmpty())
        //            {
        //                result.Add(listPage[index].QuestionCode, "");// drsPage[index].Item("QuestionCode"), "");
        //                return result;
        //                //return drsPage[index].Item("QuestionCode");
        //            }
        //            else
        //            {

        //                var data = listValue.ToTable<RmGenericFormValueItem>();
        //                for (int indexcount = data.Rows.Count - 1; indexcount >= 0; indexcount--)
        //                {
        //                    DataRow c_row = data.Rows[indexcount];
        //                    if (listPageCodeAccessed.Contains(c_row.Item("PageCode")) == false && c_row.Item("PageCode") != _currPageCode)
        //                    {
        //                        data.Rows[indexcount]["FieldValue"] = "";
        //                        data.Rows[indexcount]["AnswerText"] = "";
        //                    }
        //                }
        //                data.AcceptChanges();

        //                var dtValue = PivotTable(data,tblResultEmpty);

        //                for (int i = _currentPage; i <= tblQuestion.Rows.Count; i++)
        //                {

        //                    int orderindexcurrent = tblQuestion._FindIndex("OrderIndex", i.ToString()) + 1;
        //                    //var x1 = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

        //                    if (orderindexcurrent != -1)
        //                    {
        //                        if (tblQuestion.Rows[orderindexcurrent].Item("QuestionTypeID") != "0")
        //                        {
        //                            if (tblQuestion.Rows[orderindexcurrent].Item("HideCondition").IsEmpty())
        //                            {
        //                                int returnIndex = Convert.ToInt32(tblQuestion.Rows[i].Item("OrderIndex")) - 1;

        //                                result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
        //                                return result;
        //                                //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
        //                            }
        //                            else
        //                            {

        //                                //var sques = ConvertShortQuestion(tblQuestion);
        //                                var code_current = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

        //                                //var s = dtValue.DataTableToJSON();
        //                                var logic = tblQuestion.Rows[orderindexcurrent].Item("HideCondition");

        //                                var resultlogic = dtValue.Select(tblQuestion.Rows[orderindexcurrent].Item("HideCondition"));



        //                                if (resultlogic.Count() == 0)
        //                                {
        //                                    var a = tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");

        //                                    //var x2 = tblQuestion.DataTableToJSON();
        //                                    result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
        //                                    return result;
        //                                    //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
        //                                }

        //                            }
        //                        }
        //                    }
        //                }

        //                result.Add(listPage[index].QuestionCode,"");// drsPage[index].Item("QuestionCode"), "");
        //                return result;
        //                //return drsPage[index].Item("QuestionCode");
        //            }

        //            #endregion

        //            var indext = _FindCurrentIndexPage(listPage, _currPageCode) + 1;// (drsPage._FindIndex("QuestionCode", _currPageCode) + 1);
        //            result.Add(listPage[indext].QuestionCode, "");// drsPage[indext].Item("QuestionCode"), "");
        //            return result;
        //            //return drsPage[indext].Item("QuestionCode");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
        //        //Dialogs.ShowError("Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message, 3000);
        //        //RunOnUiThread(() => Toast.MakeText(this, "Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message, ToastLength.Long).Show());
        //        result.Add("", "Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message);
        //        return result;
        //    }
        //}

        public Dictionary<string, string> checkLogicCheckPostSkip(List<RmLogicCheckItem> listLogicCheck, List<RmGenericFormQuestionAndroidAnswerItem> listQuestion, DataTable tblPage, List<RmGenericFormValueItem> listValue, string _currPageCode, List<string> listPageCodeAccessed, DataTable tblResultEmpty)
        {

            var result = new Dictionary<string, string>();
            Stopwatch w1 = new Stopwatch();
            w1.Start();

            try
            {

                w1.Restart();
                var logicRows = listLogicCheck.Where(o => o.FromPage == _currPageCode).ToList();
                w1.Stop();
                Debug.Write(string.Format("{1} where take {0} milisecond ", w1.ElapsedMilliseconds, "listLogicCheck.Where"));

                w1.Restart();


                if (logicRows.Count() == 0)
                {
                    //nextx page
                    if (_currPageCode == tblPage.Rows[tblPage.Rows.Count - 1].Item("QuestionCode"))// listPage[listPage.Count - 1].QuestionCode)// drsPage[drsPage.Count() - 1].Item("QuestionCode"))
                    {
                        result.Add(_currPageCode, "");
                        w1.Stop();
                        Debug.Write("L1 " + w1.ElapsedMilliseconds);
                        return result;
                    }
                    else
                    {
                        //vi tri row hien tai
                        //int index = _FindCurrentIndexPage(listPage, _currPageCode) + 1;
                        var index = (tblPage._FindIndex("QuestionCode", _currPageCode) + 1);
                        // kiem tra xem co nam trong danh sach cam ko?
                        // neu ko return 
                        // neu co tra ve trang tiep theo.. neu trang tiep theo thoa man chay vong lap
                        //var nextQuestionCode = drsPage[index].Item("QuestionCode");
                        var nextQuestionCode = tblPage.Rows[index].Item("QuestionCode");// listPage[index].QuestionCode;

                        int _nextPage = 0;

                        //int _currentPage = ConvertSafe.ToInt32(drsPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));

                        int _currentPage = ConvertSafe.ToInt32(tblPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));//.  _FindOrderIndexOfQuestionCode(listPage, nextQuestionCode);
                        if (_currentPage == (int)tblPage.Rows[tblPage.Rows.Count - 1]["OrderIndex"])// (int)Convert.ToInt32((drsPage[drsPage.Count() - 1].Item("OrderIndex"))))
                        {
                            _nextPage = 99999;
                        }
                        else
                        {
                            //vi tri row hien tai

                            var code_next = tblPage.Rows[index + 1].Item("QuestionCode");// listPage[index + 1].QuestionCode;// drsPage[index + 1].Item("QuestionCode");

                            _nextPage = _FindOrderIndexOfQuestionCode(listQuestion, code_next);//  ConvertSafe.ToInt32(tblQuestion._FindValue("OrderIndex", "QuestionCode", code_next));

                        }
                        
                        if (listQuestion[_FindIndexOfPageByQuestionCode(listQuestion,nextQuestionCode)+1].HideCondition.IsEmpty())// tblQuestion._FindIndex("QuestionCode", nextQuestionCode) + 1].Item("HideCondition").IsEmpty())
                        {
                            result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                            w1.Stop();
                            Debug.Write("L1 " + w1.ElapsedMilliseconds);
                            return result;//  return drsPage[index].Item("QuestionCode");
                        }
                        else
                        {
                            int countQuestion = listQuestion.Count;
                            for (int i = _currentPage; i <= countQuestion; i++)// tblQuestion.Rows.Count; i++)
                            {
                                //int orderindexcurrent = tblQuestion._FindIndex("OrderIndex", i.ToString()) + 1;
                                int orderindexcurrent = _FindIndexOfPageByOrderIndex(listQuestion, i) + 1;
                                //var x1 = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");
                                //int orderindexcurrent = tblQuestion._FindIndex("QuestionCode", tblQuestion.Rows[i].Item("QuestionCode"));
                                if (orderindexcurrent != -1)
                                {
                                    if (listQuestion[orderindexcurrent].QuestionTypeID != 0)
                                    {
                                        if (listQuestion[orderindexcurrent].HideCondition.IsEmpty())
                                        {
                                            int returnIndex = listQuestion[i].OrderIndex - 1;

                                            result.Add(listQuestion[orderindexcurrent - 1].QuestionCode, "");
                                            w1.Stop();
                                            Debug.Write("L1 " + w1.ElapsedMilliseconds);
                                            return result;
                                            //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                        }
                                        else
                                        {
                                            w1.Restart();
                                            var dtValue = PivotTable(listValue, tblResultEmpty);
                                            w1.Stop();
                                            Debug.Write(string.Format("{1} with list take {0} milisecond ", w1.ElapsedMilliseconds, "PivotTable"));

                                            var code_current = listQuestion[orderindexcurrent].QuestionCode;
                                      
                                            var logic = listQuestion[orderindexcurrent].HideCondition;
                                            var resultlogic = dtValue.Select(logic);

                                            if (resultlogic.Count() == 0)
                                            {
                                                result.Add(listQuestion[orderindexcurrent - 1].QuestionCode, "");
                                                return result;// tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                            }

                                        }
                                    }
                                }
                            }
                            result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                            //result.Add(listPage[index].QuestionCode, "");// drsPage[index].Item("QuestionCode"), "");
                            return result;// return drsPage[index].Item("QuestionCode");
                        }
                    }
                }
                else
                {
                    w1.Restart();

                    w1.Stop();
                    Debug.Write(string.Format("{1} with datatable take {0} milisecond ", w1.ElapsedMilliseconds, "PivotTable"));
                    w1.Restart();
                    var dtValue = PivotTable(listValue, tblResultEmpty);
                    w1.Stop();
                    Debug.Write(string.Format("{1} with list take {0} milisecond ", w1.ElapsedMilliseconds, "PivotTable"));

                    w1.Restart();
                    foreach (var row in logicRows)
                    {
                        try
                        {
                            var resultlogic = dtValue.Select(row.SkipLogic);

                            if (resultlogic.Count() == 0)
                            {
                                //return ConvertSafe.ToInt32(gvQuestion.GetDataRow(_focusRowIndex + 1).Item("PageOrderIndex"));
                                //logic OK go to next page

                            }
                            else
                            {
                                string errorComment = "";
                                if (row.Comments.IsNotEmpty())
                                {
                                    //IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
                                    errorComment = row.Comments;
                                    //Dialogs.ShowError(row.Item("Comments"), 3000);

                                }
                                result.Add(row.SkipToPage, errorComment);
                                w1.Stop();
                                Debug.Write("L1 " + w1.ElapsedMilliseconds);
                                return result;
                                //return row.Item("SkipToPage");
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }


                if (_currPageCode == tblPage.Rows[tblPage.Rows.Count - 1].Item("QuestionCode"))// listPage[listPage.Count - 1].QuestionCode)// drsPage[drsPage.Count() - 1].Item("QuestionCode"))
                {
                    result.Add(_currPageCode, "");
                    return result;
                    //return _currPageCode;
                }
                else
                {
                    //vi tri row hien tai
                    #region check Hide condition

                    var index = tblPage._FindIndex("QuestionCode", _currPageCode) + 1;// _FindCurrentIndexPage(listPage, _currPageCode) + 1;// (drsPage._FindIndex("QuestionCode", _currPageCode) + 1);
                    // kiem tra xem co nam trong danh sach cam ko?
                    // neu ko return 
                    // neu co tra ve trang tiep theo.. neu trang tiep theo thoa man chay vong lap
                    var nextQuestionCode = tblPage.Rows[index].Item("QuestionCode");// listPage[index].QuestionCode;// drsPage[index].Item("QuestionCode");

                    int _nextPage = 0;

                    int _currentPage = ConvertSafe.ToInt32(tblPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));//_FindOrderIndexOfQuestionCode(listPage, nextQuestionCode);// ConvertSafe.ToInt32(drsPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));

                    if (_currentPage == (int)tblPage.Rows[tblPage.Rows.Count - 1]["OrderIndex"]) //listPage[listPage.Count - 1].OrderIndex)// (int)Convert.ToInt32((drsPage[drsPage.Count() - 1].Item("OrderIndex"))))
                    {
                        _nextPage = 99999;
                    }
                    else
                    {
                        //vi tri row hien tai

                        var code_next = tblPage.Rows[index + 1].Item("QuestionCode");// listPage[index + 1].QuestionCode;// drsPage[index + 1].Item("QuestionCode");

                        _nextPage = _FindOrderIndexOfQuestionCode(listQuestion, code_next);//  ConvertSafe.ToInt32(tblQuestion._FindValue("OrderIndex", "QuestionCode", code_next));

                    }

                    //var tbltempPage = tblQuestion.Select(string.Format("OrderIndex>'{0}' AND OrderIndex<='{1}'", _currentPage, _nextPage));

                    if (listQuestion[_FindIndexOfPageByQuestionCode(listQuestion,nextQuestionCode)+1].HideCondition.IsEmpty())// tblQuestion._FindIndex("QuestionCode", nextQuestionCode) + 1].HideCondition.IsEmpty())
                    {
                        result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                        /* result.Add(listPage[index].QuestionCode, "");*/// drsPage[index].Item("QuestionCode"), "");
                        return result;
                        //return drsPage[index].Item("QuestionCode");
                    }
                    else
                    {

                        var data = listValue.ToTable<RmGenericFormValueItem>();
                        for (int indexcount = data.Rows.Count - 1; indexcount >= 0; indexcount--)
                        {
                            DataRow c_row = data.Rows[indexcount];
                            if (listPageCodeAccessed.Contains(c_row.Item("PageCode")) == false && c_row.Item("PageCode") != _currPageCode)
                            {
                                data.Rows[indexcount]["FieldValue"] = "";
                                data.Rows[indexcount]["AnswerText"] = "";
                            }
                        }
                        data.AcceptChanges();

                        var dtValue = PivotTable(data, tblResultEmpty);

                        for (int i = _currentPage; i <= listQuestion.Count; i++)
                        {
                            int orderindexcurrent = _FindIndexOfPageByOrderIndex(listQuestion, i) + 1;// tblQuestion._FindIndex("OrderIndex", i.ToString()) + 1;
                            //var x1 = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

                            if (orderindexcurrent != -1)
                            {
                                if (listQuestion[orderindexcurrent].QuestionTypeID != 0)
                                {
                                    if (listQuestion[orderindexcurrent].HideCondition.IsEmpty())
                                    {
                                        int returnIndex = listQuestion[i].OrderIndex - 1;

                                        result.Add(listQuestion[orderindexcurrent - 1].QuestionCode, "");
                                        return result;
                                        //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                    }
                                    else
                                    {

                                        //var sques = ConvertShortQuestion(tblQuestion);
                                        var code_current = listQuestion[orderindexcurrent].QuestionCode;

                                        //var s = dtValue.DataTableToJSON();
                                        var logic = listQuestion[orderindexcurrent].HideCondition;

                                        var resultlogic = dtValue.Select(listQuestion[orderindexcurrent].HideCondition);



                                        if (resultlogic.Count() == 0)
                                        {
                                            var a = listQuestion[orderindexcurrent - 1].QuestionCode;

                                            //var x2 = tblQuestion.DataTableToJSON();
                                            result.Add(listQuestion[orderindexcurrent - 1].QuestionCode, "");
                                            return result;
                                            //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                        }

                                    }
                                }
                            }
                        }

                        result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                        //result.Add(listPage[index].QuestionCode, "");// drsPage[index].Item("QuestionCode"), "");
                        return result;
                        //return drsPage[index].Item("QuestionCode");
                    }

                    #endregion

                    var indext = tblPage._FindIndex("QuestionCode", _currPageCode) + 1;//_FindCurrentIndexPage(listPage, _currPageCode) + 1;// 
                    result.Add(tblPage.Rows[indext].Item("QuestionCode"), "");// drsPage[indext].Item("QuestionCode"), "");
                    return result;
                    //return drsPage[indext].Item("QuestionCode");
                }
            }
            catch (Exception ex)
            {               
                result.Add("", "Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message);
                return result;
            }
        }

        public Dictionary<string, string> checkLogicCheckPostSkip2(List<RmLogicCheckItem> listLogicCheck, DataTable tblQuestion, DataTable tblPage, List<RmGenericFormValueItem> listValue, string _currPageCode, List<string> listPageCodeAccessed, DataTable tblResultEmpty)
        {

            var result = new Dictionary<string, string>();
            Stopwatch w1 = new Stopwatch();
            w1.Start();
            
            try
            {
               
                w1.Restart();
                var logicRows = listLogicCheck.Where(o => o.FromPage == _currPageCode).ToList();
                w1.Stop();
                Debug.Write(string.Format("{1} where take {0} milisecond ", w1.ElapsedMilliseconds, "listLogicCheck.Where"));

                w1.Restart();

             
                if (logicRows.Count() == 0)
                {
                    //nextx page
                    if (_currPageCode ==tblPage.Rows[tblPage.Rows.Count-1].Item("QuestionCode"))// listPage[listPage.Count - 1].QuestionCode)// drsPage[drsPage.Count() - 1].Item("QuestionCode"))
                    {
                        result.Add(_currPageCode, "");
                        w1.Stop();
                        Debug.Write("L1 " + w1.ElapsedMilliseconds);
                        return result;
                    }
                    else
                    {
                        //vi tri row hien tai
                        //int index = _FindCurrentIndexPage(listPage, _currPageCode) + 1;
                        var index = (tblPage._FindIndex("QuestionCode", _currPageCode) + 1);
                        // kiem tra xem co nam trong danh sach cam ko?
                        // neu ko return 
                        // neu co tra ve trang tiep theo.. neu trang tiep theo thoa man chay vong lap
                        //var nextQuestionCode = drsPage[index].Item("QuestionCode");
                        var nextQuestionCode = tblPage.Rows[index].Item("QuestionCode");// listPage[index].QuestionCode;

                        int _nextPage = 0;

                        //int _currentPage = ConvertSafe.ToInt32(drsPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));

                        int _currentPage = ConvertSafe.ToInt32(tblPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));//.  _FindOrderIndexOfQuestionCode(listPage, nextQuestionCode);
                        if (_currentPage ==(int) tblPage.Rows[tblPage.Rows.Count - 1]["OrderIndex"])// (int)Convert.ToInt32((drsPage[drsPage.Count() - 1].Item("OrderIndex"))))
                        {
                            _nextPage = 99999;
                        }
                        else
                        {
                            //vi tri row hien tai

                            var code_next = tblPage.Rows[index + 1].Item("QuestionCode");// listPage[index + 1].QuestionCode;// drsPage[index + 1].Item("QuestionCode");

                            _nextPage = ConvertSafe.ToInt32(tblQuestion._FindValue("OrderIndex", "QuestionCode", code_next));

                        }

                        //var tbltempPage = tblQuestion.Select(string.Format("OrderIndex>'{0}' AND OrderIndex<='{1}'", _currentPage, _nextPage));
                        //string xxxx = tblQuestion.DataTableToJSON();


                        if (tblQuestion.Rows[tblQuestion._FindIndex("QuestionCode", nextQuestionCode) + 1].Item("HideCondition").IsEmpty())
                        {
                            //result.Add(listPage[index].QuestionCode, "");/// drsPage[index].Item("QuestionCode"), "");
                            result.Add(tblPage.Rows[index].Item("QuestionCode"), "");

                            w1.Stop();
                            Debug.Write("L1 " + w1.ElapsedMilliseconds);
                            return result;//  return drsPage[index].Item("QuestionCode");
                        }
                        else
                        {

                            for (int i = _currentPage; i <= tblQuestion.Rows.Count; i++)
                            {

                                int orderindexcurrent = tblQuestion._FindIndex("OrderIndex", i.ToString()) + 1;
                                //var x1 = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");
                                //int orderindexcurrent = tblQuestion._FindIndex("QuestionCode", tblQuestion.Rows[i].Item("QuestionCode"));
                                if (orderindexcurrent != -1)
                                {
                                    if (tblQuestion.Rows[orderindexcurrent].Item("QuestionTypeID") != "0")
                                    {
                                        if (tblQuestion.Rows[orderindexcurrent].Item("HideCondition").IsEmpty())
                                        {
                                            int returnIndex = Convert.ToInt32(tblQuestion.Rows[i].Item("OrderIndex")) - 1;

                                            result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
                                            w1.Stop();
                                            Debug.Write("L1 " + w1.ElapsedMilliseconds);
                                            return result;
                                            //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                        }
                                        else
                                        {
                                            w1.Restart();
                                            var dtValue = PivotTable(listValue, tblResultEmpty);
                                            w1.Stop();
                                            Debug.Write(string.Format("{1} with list take {0} milisecond ", w1.ElapsedMilliseconds, "PivotTable"));

                                            var code_current = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

                                            //var s = dtValue.DataTableToJSON();
                                            var logic = tblQuestion.Rows[orderindexcurrent].Item("HideCondition");
                                            var resultlogic = dtValue.Select(tblQuestion.Rows[orderindexcurrent].Item("HideCondition"));

                                            if (resultlogic.Count() == 0)
                                            {
                                              
                                                result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
                                                w1.Stop();
                                              
                                                return result;// tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                            }

                                        }
                                    }
                                }
                            }
                            result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                            //result.Add(listPage[index].QuestionCode, "");// drsPage[index].Item("QuestionCode"), "");
                            return result;// return drsPage[index].Item("QuestionCode");
                        }
                    }
                }
                else
                {
                    w1.Restart();
                 
                    var dtValue = PivotTable( listValue,tblResultEmpty);
                    w1.Stop();
                    Debug.Write(string.Format("{1} with list take {0} milisecond ", w1.ElapsedMilliseconds, "PivotTable"));

                    w1.Restart();
                    foreach (var row in logicRows)
                    {
                        try
                        {
                            var resultlogic = dtValue.Select(row.SkipLogic);

                            if (resultlogic.Count() == 0)
                            {
                                //return ConvertSafe.ToInt32(gvQuestion.GetDataRow(_focusRowIndex + 1).Item("PageOrderIndex"));
                                //logic OK go to next page

                            }
                            else
                            {
                                string errorComment = "";
                                if (row.Comments.IsNotEmpty())
                                {
                                    errorComment = row.Comments;
                                 
                                }
                                result.Add(row.SkipToPage, errorComment);
                                w1.Stop();
                                Debug.Write("L1 " + w1.ElapsedMilliseconds);
                                return result;
                                //return row.Item("SkipToPage");
                            }
                        }
                        catch (Exception ex)
                        {

                        }

                    }

                }


                if (_currPageCode ==tblPage.Rows[tblPage.Rows.Count-1].Item("QuestionCode"))// listPage[listPage.Count - 1].QuestionCode)// drsPage[drsPage.Count() - 1].Item("QuestionCode"))
                {
                    result.Add(_currPageCode, "");
                    return result;
                    //return _currPageCode;
                }
                else
                {
                    //vi tri row hien tai
                    #region check Hide condition

                    var index = tblPage._FindIndex("QuestionCode", _currPageCode) + 1;// _FindCurrentIndexPage(listPage, _currPageCode) + 1;// (drsPage._FindIndex("QuestionCode", _currPageCode) + 1);
                    // kiem tra xem co nam trong danh sach cam ko?
                    // neu ko return 
                    // neu co tra ve trang tiep theo.. neu trang tiep theo thoa man chay vong lap
                    var nextQuestionCode = tblPage.Rows[index].Item("QuestionCode");// listPage[index].QuestionCode;// drsPage[index].Item("QuestionCode");

                    int _nextPage = 0;

                    int _currentPage =  ConvertSafe.ToInt32(tblPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));//_FindOrderIndexOfQuestionCode(listPage, nextQuestionCode);// ConvertSafe.ToInt32(drsPage._FindValue("OrderIndex", "QuestionCode", nextQuestionCode));

                    if (_currentPage ==(int)tblPage.Rows[tblPage.Rows.Count-1]["OrderIndex"]) //listPage[listPage.Count - 1].OrderIndex)// (int)Convert.ToInt32((drsPage[drsPage.Count() - 1].Item("OrderIndex"))))
                    {
                        _nextPage = 99999;
                    }
                    else
                    {
                        //vi tri row hien tai

                        var code_next = tblPage.Rows[index + 1].Item("QuestionCode");// listPage[index + 1].QuestionCode;// drsPage[index + 1].Item("QuestionCode");

                        _nextPage = ConvertSafe.ToInt32(tblQuestion._FindValue("OrderIndex", "QuestionCode", code_next));

                    }

                    var tbltempPage = tblQuestion.Select(string.Format("OrderIndex>'{0}' AND OrderIndex<='{1}'", _currentPage, _nextPage));

                    if (tblQuestion.Rows[tblQuestion._FindIndex("QuestionCode", nextQuestionCode) + 1].Item("HideCondition").IsEmpty())
                    {
                        result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                       /* result.Add(listPage[index].QuestionCode, "");*/// drsPage[index].Item("QuestionCode"), "");
                        return result;
                        //return drsPage[index].Item("QuestionCode");
                    }
                    else
                    {

                        var data = listValue.ToTable<RmGenericFormValueItem>();
                        for (int indexcount = data.Rows.Count - 1; indexcount >= 0; indexcount--)
                        {
                            DataRow c_row = data.Rows[indexcount];
                            if (listPageCodeAccessed.Contains(c_row.Item("PageCode")) == false && c_row.Item("PageCode") != _currPageCode)
                            {
                                data.Rows[indexcount]["FieldValue"] = "";
                                data.Rows[indexcount]["AnswerText"] = "";
                            }
                        }
                        data.AcceptChanges();

                        var dtValue = PivotTable(data, tblResultEmpty);

                        for (int i = _currentPage; i <= tblQuestion.Rows.Count; i++)
                        {

                            int orderindexcurrent = tblQuestion._FindIndex("OrderIndex", i.ToString()) + 1;
                            //var x1 = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

                            if (orderindexcurrent != -1)
                            {
                                if (tblQuestion.Rows[orderindexcurrent].Item("QuestionTypeID") != "0")
                                {
                                    if (tblQuestion.Rows[orderindexcurrent].Item("HideCondition").IsEmpty())
                                    {
                                        int returnIndex = Convert.ToInt32(tblQuestion.Rows[i].Item("OrderIndex")) - 1;

                                        result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
                                        return result;
                                        //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                    }
                                    else
                                    {

                                        //var sques = ConvertShortQuestion(tblQuestion);
                                        var code_current = tblQuestion.Rows[orderindexcurrent].Item("QuestionCode");

                                        //var s = dtValue.DataTableToJSON();
                                        var logic = tblQuestion.Rows[orderindexcurrent].Item("HideCondition");

                                        var resultlogic = dtValue.Select(tblQuestion.Rows[orderindexcurrent].Item("HideCondition"));



                                        if (resultlogic.Count() == 0)
                                        {
                                            var a = tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");

                                            //var x2 = tblQuestion.DataTableToJSON();
                                            result.Add(tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode"), "");
                                            return result;
                                            //return tblQuestion.Rows[orderindexcurrent - 1].Item("QuestionCode");
                                        }

                                    }
                                }
                            }
                        }

                        result.Add(tblPage.Rows[index].Item("QuestionCode"), "");
                        //result.Add(listPage[index].QuestionCode, "");// drsPage[index].Item("QuestionCode"), "");
                        return result;
                        //return drsPage[index].Item("QuestionCode");
                    }

                    #endregion

                    var indext = tblPage._FindIndex("QuestionCode", _currPageCode) + 1;//_FindCurrentIndexPage(listPage, _currPageCode) + 1;// 
                    result.Add(tblPage.Rows[indext].Item("QuestionCode"), "");// drsPage[indext].Item("QuestionCode"), "");
                    return result;
                    //return drsPage[indext].Item("QuestionCode");
                }
            }
            catch (Exception ex)
            {
                //IUserDialogs Dialogs = Acr.UserDialogs.UserDialogs.Instance;
                //Dialogs.ShowError("Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message, 3000);
                //RunOnUiThread(() => Toast.MakeText(this, "Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message, ToastLength.Long).Show());
                result.Add("", "Lỗi checklogic tại trang " + _currPageCode + " " + ex.Message);
                return result;
            }
        }

        private int _FindOrderIndexOfQuestionCode(List<RmGenericFormQuestionAndroidAnswerItem> listPage, string nextQuestionCode)
        {
           foreach(var item in listPage)
            {
                if (item.QuestionCode == nextQuestionCode)
                    return item.OrderIndex;
            }
            return -1;
        }

        private int _FindIndexOfPageByOrderIndex(List<RmGenericFormQuestionAndroidAnswerItem> listPage, int _OrderIndex)
        {
            foreach (var item in listPage)
            {
                if (item.OrderIndex == _OrderIndex)
                    return item.OrderIndex;
            }
            return -1;
        }

        private int _FindIndexOfPageByQuestionCode(List<RmGenericFormQuestionAndroidAnswerItem> listPage, string currPageCode)
        {
            int count = listPage.Count;
            for(int i=0;i<count;i++)
            {
                if (listPage[i].QuestionCode == currPageCode)
                    return i;
            }
            return -1;
        }
 
        public DataTable CreatePivotTableEmptyData(List<RmGenericFormValueItem> dt)
        {
            DataTable result = new DataTable();
            Stopwatch w1 = new Stopwatch();
            w1.Start();

            foreach(var item in dt)
            {
                if (result.Columns.Contains(item.FieldName) == false)
                {
                    Type fieldType;
                    fieldType = typeof(String);
                    if (item.FieldType == "string")
                    {
                        fieldType = typeof(String);
                    }
                    if (item.FieldType == "double")
                    {
                        fieldType = typeof(Double);
                    }
                    DataColumn dc = new DataColumn(item.FieldName, fieldType);
                    result.Columns.Add(dc);
                }
            }
           
            w1.Stop();

            System.Diagnostics.Debug.WriteLine("Create template s1: " + w1.ElapsedMilliseconds);

            return result;
        }
    }
}
