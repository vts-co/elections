using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using ExcelDataReader;
using ImportExcel.Models;

namespace ImportExcel.Controllers
{
    public class HomeController : Controller
    {

        public List<VoterInfo> ReadVotersFromExcel()
        {
            string filePath = @"C:\Users\Remon\Downloads\Tamia 2.xlsx";

            var result = new List<VoterInfo>();

            using (var workbook = new XLWorkbook(filePath))
            {
                var ws = workbook.Worksheet(1);

                string generalCommittee = "";
                string subCommitteeNumber = "";
                string schoolName = "";
                string School = "";
                string village = "";

                foreach (var row in ws.RowsUsed())
                {
                    string rowText = string.Join(" ", row.Cells().Select(c => c.GetString().Trim()));
                    if (string.IsNullOrWhiteSpace(rowText)) continue;

                    rowText = ConvertArabicDigitsToEnglish(rowText);
                    var sasa = ws.RowsUsed().Count();
                    if (rowText.Contains("اللجنة الفرعية رقم") || rowText.Contains("رقم اللجنة"))
                    {

                        var match = Regex.Match(rowText, @"اللجنة الفرعية رقم\s*(\d+)");
                        if (!match.Success)
                            match = Regex.Match(rowText, @"رقم اللجنة\s*(\d+)");

                        if (match.Success)
                            subCommitteeNumber = match.Groups[1].Value;

                       

                        if (rowText.Contains("مدرسة"))
                        {
                             match = Regex.Match(rowText, @"مدرسة\s*(.+)", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                 schoolName = "مدرسة " + match.Groups[1].Value.Trim();

                                // نحذف الكلمات الزائدة الشائعة
                                schoolName = schoolName
                                    .Replace("ومـقـرهـا :", "")
                                    .Replace("وعنوانها :", "")
                                    .Replace("ومقرها :", "")
                                    .Replace("وعنوان :", "")
                                    .Trim();

                                // نحذف المسافات والفواصل المكررة
                                schoolName = Regex.Replace(schoolName, @"\s{2,}", " ");
                                var villageMatch = Regex.Match(
                                    schoolName,
                                    @"-\s*(?:(?:عزبة|نجع|كفر|قرية|ريه|مدينة|حي|بندر)\s*)?(.+)",
                                    RegexOptions.IgnoreCase
                                );
                                if (villageMatch.Success)
                                {
                                    village = villageMatch.Groups[1].Value.Trim();
                                }
                                // نحفظ النتيجة
                                School = schoolName;
                            }
                        }
                           else if (rowText.Contains("مدرسه"))
                        {
                             match = Regex.Match(rowText, @"مدرسه\s*(.+)", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                 schoolName = "مدرسه " + match.Groups[1].Value.Trim();

                                // نحذف الكلمات الزائدة الشائعة
                                schoolName = schoolName
                                    .Replace("ومـقـرهـا :", "")
                                    .Replace("وعنوانها :", "")
                                    .Replace("ومقرها :", "")
                                    .Replace("وعنوان :", "")
                                    .Trim();

                                // نحذف المسافات والفواصل المكررة
                                schoolName = Regex.Replace(schoolName, @"\s{2,}", " ");
                                var villageMatch = Regex.Match(
        schoolName,
        @"-\s*(?:(?:عزبة|نجع|كفر|قرية|ريه|مدينة|حي|بندر)\s*)?(.+)",
        RegexOptions.IgnoreCase
    );


                                if (villageMatch.Success)
                                {
                                    village = villageMatch.Groups[1].Value.Trim();
                                }

                                // نحفظ النتيجة
                                School = schoolName;
                            }
                        }
                        else if (rowText.Contains("معهد"))
                        {
                             match = Regex.Match(rowText, @"معهد\s*(.+)", RegexOptions.Singleline);

                            if (match.Success)
                            {
                                 schoolName = "معهد " + match.Groups[1].Value.Trim();

                                // نحذف الكلمات الزائدة الشائعة
                                schoolName = schoolName
                                    .Replace("ومـقـرهـا :", "")
                                    .Replace("وعنوانها :", "")
                                    .Replace("ومقرها :", "")
                                    .Replace("وعنوان :", "")
                                    .Trim();

                                // نحذف المسافات والفواصل المكررة
                                schoolName = Regex.Replace(schoolName, @"\s{2,}", " ");
                                var villageMatch = Regex.Match(
        schoolName,
        @"-\s*(?:(?:عزبة|نجع|كفر|قرية|ريه|مدينة|حي|بندر)\s*)?(.+)",
        RegexOptions.IgnoreCase
    );

                                if (villageMatch.Success)
                                {
                                    if (rowText.Contains("بندر"))
                                    {
                                        village ="بندر "+ villageMatch.Groups[1].Value.Trim();

                                    }
                                }

                                // نحفظ النتيجة
                                School = schoolName;
                            }
                        }


                    }

                    // 🔹 استخراج رقم اللجنة العامة
                    if (rowText.Contains("اللجنة العامة رقم") || rowText.Contains("العامة رقم") || rowText.Contains("التابعة للجنة العامة رقم"))
                    {
                        var match = Regex.Match(
                            rowText,
                            @"(?:التابعة\s*)?(?:للجنة\s*)?العامة\s*رقم\s*[:\s\(]*([0-9٠-٩]+)"
                        );

                        if (match.Success)
                        {
                            var num = match.Groups[1].Value.Trim();
                            generalCommittee = ConvertArabicDigitsToEnglish(num);
                        }
                    }



                    var nameMatches = Regex.Matches(rowText, @"([\u0621-\u064A\s]+)\s+(\d+)");
                    if (nameMatches.Count == 3)
                    {

                        foreach (Match m in nameMatches)
                        {
                            string name = m.Groups[1].Value.Trim();
                            string serial = m.Groups[2].Value.Trim();

                            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(serial))
                            {
                                result.Add(new VoterInfo
                                {
                                    Serial = serial,
                                    Name = name,
                                    SubCommitteeNumber = subCommitteeNumber,
                                    GeneralCommittee = generalCommittee,
                                    School = School,
                                    Village = village,
                                    Center = "طاميه",
                                   
                                });
                            }
                             var asasa = result.LastOrDefault();
                        }
                    }
                }
            }
            int batchSize = 2000;
            int counter = 0;
            ElectionsDbEntities electionsDb = new ElectionsDbEntities();

            foreach (var voter in result)
            {
                electionsDb.VoterInfoes.Add(voter);
                counter++;

                if (counter % batchSize == 0)
                {
                    electionsDb.SaveChanges();
                    electionsDb.Dispose();
                    electionsDb = new ElectionsDbEntities();
                    Console.WriteLine($"✅ Saved {counter} records so far...");
                }
            }

            electionsDb.SaveChanges();
            electionsDb.Dispose();

            

            //ElectionsDbEntities electionsDb = new ElectionsDbEntities();
            //electionsDb.VoterInfoes.AddRange(result);
            //electionsDb.SaveChanges();
            return result;
        }





        public  List<VoterInfo> ReadExcelWithIExcelDataReader()
        {
            var people = new List<VoterInfo>();
            string filePath = @"C:\Users\Remon\Downloads\Data_Sheet (1).xlsx";
            ElectionsDbEntities electionsDb = new ElectionsDbEntities();

            // لازم نفعّل System.Text.Encoding.RegisterProvider علشان ملفات XLSX
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var result1 = new List<VoterInfo>();

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                // نحدد نوع القارئ بناءً على امتداد الملف
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // نقرأ كل البيانات في DataSet (Workbook)
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // أول صف يعتبر رؤوس أعمدة
                        }
                    });

                    // نختار أول شيت في الـ Workbook
                    var table = result.Tables[0];

                    foreach (DataRow row in table.Rows)
                    {
                        var name = row["الاسم"]?.ToString().Trim();
                        var country = row["البلد"]?.ToString().Trim();

                        if (!string.IsNullOrEmpty(name))
                        {
                            result1.Add(new VoterInfo
                            {
                                Name = name,
                                Farm = country,
                                School= "مدرسة اللواء احمد عبدالتواب تعليم اساسى مركز طاميه - عزبة محمد عبدالجليل العزيزية",
                                Center="طاميه",
                                Village= "محمد عبدالجليل العزيزية(عزب)",
                                SubCommitteeNumber="81",
                                GeneralCommittee="3",
                                
                            });
                        }
                    }
                    electionsDb.VoterInfoes.AddRange(result1);
                    electionsDb.SaveChanges();

                }
            }

            return people;
        }
        private string ConvertArabicDigitsToEnglish(string input)
        {
            return input
                .Replace("٠", "0")
                .Replace("١", "1")
                .Replace("٢", "2")
                .Replace("٣", "3")
                .Replace("٤", "4")
                .Replace("٥", "5")
                .Replace("٦", "6")
                .Replace("٧", "7")
                .Replace("٨", "8")
                .Replace("٩", "9");
        }

    }
}