using System;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using ApiWebsite.Core.Services;
using ApiWebsite.Models;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using ApiWebsite.Model;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http;
using System.Text;
using AutoMapper.Configuration;

namespace ApiWebsite.Helper
{
    public class Util
    {   
        // public static void SendNotify(Post post, string categorySlug, string domain)
        // {
        //     using (var client = new HttpClient())
        //     {
        //         // System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //         // SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
        //         //set up client
        //         client.BaseAddress = new Uri(domain);
        //         client.DefaultRequestHeaders.Clear();
        //         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //         var obj = new
        //         {
        //             slugToRevalidatePost = post != null ? String.Format("{0}/{1}", categorySlug, post.Slug) : "",
        //             slugToRevalidateCategory = categorySlug,
        //         };


        //         var opt = new JsonSerializerOptions() { WriteIndented = true };
        //         string strJson = System.Text.Json.JsonSerializer.Serialize<dynamic>(obj, opt);

        //         HttpResponseMessage responseMessage = client.PostAsync("api/revalidate?secret=notifyapp", new StringContent(strJson, Encoding.UTF8, "application/json")).Result;

        //         if (responseMessage.StatusCode == HttpStatusCode.Created)
        //         {
        //             Console.WriteLine(responseMessage);
        //         }
        //         // Blocking call! Program will wait here until a response is received or a timeout occurs.
        //     }
        // }

        // public static void SendNotifyCategory(string categorySlug, string domain)
        // {
        //     using (var client = new HttpClient())
        //     {
        //         // System.Net.ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
        //         // SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls
        //         //set up client
        //         client.BaseAddress = new Uri(domain);
        //         client.DefaultRequestHeaders.Clear();
        //         client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //         var obj = new
        //         {
        //             slugToRevalidateCategory = categorySlug
        //         };


        //         var opt = new JsonSerializerOptions() { WriteIndented = true };
        //         string strJson = System.Text.Json.JsonSerializer.Serialize<dynamic>(obj, opt);

        //         HttpResponseMessage responseMessage = client.PostAsync("api/revalidate?secret=notifyapp", new StringContent(strJson, Encoding.UTF8, "application/json")).Result;

        //         if (responseMessage.StatusCode == HttpStatusCode.Created)
        //         {
        //             Console.WriteLine(responseMessage);
        //         }
        //         // Blocking call! Program will wait here until a response is received or a timeout occurs.
        //     }
        // }
        
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static bool IsValidGuid(string str)
        {
            Guid guid;
            return Guid.TryParse(str, out guid);
        }

        public static string ExtractFileAndGetFilePath(FileManager file, IOptions<List<VirtualPathConfig>> iVirtualPathConfig)
        {
            var resultFile = new System.IO.FileInfo(file.PhysicalPath);
            if (resultFile.Exists)
            {
                (string directory, string virtualPath, string fileType) = FilePathHelper.GetDirectoryPath(iVirtualPathConfig, ".zip");
                // var extractPath = directory.Replace("\\", @"\");
                var directoryPath = directory + "\\" + file.Name;
                if (!Directory.Exists(directoryPath))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(file.PhysicalPath, directory);
                }

                string[] fileEntries = Directory.GetFiles(directoryPath);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.Contains(".html"))
                    {
                        var lastPart = fileName.Split("\\").Where(x => !string.IsNullOrWhiteSpace(x)).LastOrDefault();
                        return "/document/" + file.Name + "/" + lastPart;
                    }
                }
            }

            return null;
        }

        // public static StringBuilder DownloadCommaSeperatedFile(CreateExcelPostRequest model, List<PostPrivate> lstPostPrivate, List<Categories> lstCategories)
        // {
        //     var builder = new StringBuilder();
        //     int DayInterval = 1;

        //     DateTime StartDate = model.FromDay;
        //     DateTime EndDate = model.UntilDay;
        //     List<DateTime> dateList = new List<DateTime>();
        //     dateList.Add(StartDate);
        //     while (StartDate.AddDays(DayInterval) <= EndDate)
        //     {
        //         StartDate = StartDate.AddDays(DayInterval);
        //         dateList.Add(StartDate);
        //     }

        //     foreach (var date in dateList)
        //     {
        //         builder.AppendLine(date.ToString("ddMMyyyy"));
        //         // builder.AppendLine("1234556");

        //         var startDay = new DateTime(date.Year, date.Month, date.Day);
        //         var endDay = startDay.AddDays(1);

        //         var listPostPrivateCompare = lstPostPrivate.Where(x => x.PublishAtUtc >= startDay && x.PublishAtUtc < endDay).OrderBy(x => x.PublishAtUtc);

        //         if (listPostPrivateCompare.Count() == 0)
        //         {
        //             continue;
        //         }

        //         foreach (var post in listPostPrivateCompare)
        //         {
        //             var categoryNames = "";

        //             foreach (var cateId in post.CateId)
        //             {
        //                 var cates = lstCategories.Where(x => x.Id == cateId);
        //                 var cateName = "";
        //                 if (cates.Count() > 0)
        //                 {
        //                     cateName = cates.FirstOrDefault().Name;
        //                 }
        //                 else
        //                 {
        //                     continue;
        //                 }
        //                 cateName = cateName + "; ";
        //                 categoryNames = categoryNames + cateName;
        //             }

        //             var desc = post.Description;
        //             desc = desc.Replace("<p>", "");
        //             desc = desc.Replace("</p>", "");
        //             desc = desc.Replace("\n", "");

        //             builder.AppendLine($"{post.Title},{categoryNames}");
        //             builder.AppendLine($"{desc}");
        //             // builder.AppendLine("123456");
        //         }
        //     }

        //     return builder;
        // }

        // public static byte[] DownloadCommaSeperatedFileXLSX(CreateExcelPostRequest model, List<PostPrivate> lstPostPrivate, List<Categories> lstCategories)
        // {
        //     using (var workbook = new XLWorkbook())
        //     {
        //         var worksheet = workbook.Worksheets.Add("BaoCao");
        //         int DayInterval = 1;

        //         DateTime StartDate = model.FromDay;
        //         DateTime EndDate = model.UntilDay;
        //         List<DateTime> dateList = new List<DateTime>();
        //         dateList.Add(StartDate);
        //         while (StartDate.AddDays(DayInterval) <= EndDate)
        //         {
        //             StartDate = StartDate.AddDays(DayInterval);
        //             dateList.Add(StartDate);
        //         }

        //         var currentRow = 1;
        //         foreach (var date in dateList)
        //         {
        //             var startDay = new DateTime(date.Year, date.Month, date.Day);
        //             var endDay = startDay.AddDays(1);

        //             var listPostPrivateCompare = lstPostPrivate.Where(x => x.PublishAtUtc >= startDay && x.PublishAtUtc < endDay).OrderBy(x => x.PublishAtUtc).ToList();

        //             if (listPostPrivateCompare.Count() == 0)
        //             {
        //                 continue;
        //             }
        //             else
        //             {
        //                 currentRow++;
        //                 worksheet.Cell(currentRow, 1).Value = date.ToString("dd/MM/yyyy");
        //             }

        //             foreach (var post in listPostPrivateCompare)
        //             {
        //                 var tempName = lstCategories.Where(x => post.CateId.Contains(x.Id)).Select(x => x.Name).ToArray();
        //                 var categoryNames = String.Join(", ", tempName.ToArray());

        //                 // foreach (var cateId in post.CateId)
        //                 // {
        //                 //     var cates = lstCategories.Where(x => x.Id == cateId);
        //                 //     var cateName = "";
        //                 //     if (cates.Count() > 0)
        //                 //     {
        //                 //         cateName = cates.FirstOrDefault().Name;
        //                 //     }
        //                 //     else
        //                 //     {
        //                 //         continue;
        //                 //     }
        //                 //     cateName = cateName + "; ";
        //                 //     categoryNames = categoryNames + cateName;
        //                 // }

        //                 var desc = "";
        //                 if (!string.IsNullOrEmpty(post.Description))
        //                 {
        //                     var strSpan = "<span style=\"font-size:14px\"><span style=\"font-family:Verdana,Geneva,sans-serif\">";
        //                     desc = post.Description;
        //                     desc = desc.Replace("<p>", "");
        //                     desc = desc.Replace("</p>", "");
        //                     desc = desc.Replace("\n", "");
        //                     desc = desc.Replace(strSpan, "");
        //                 }
        //                 // builder.AppendLine($"{post.Title},{categoryNames}");
        //                 // builder.AppendLine($"{desc}");
        //                 currentRow++;
        //                 worksheet.Cell(currentRow, 1).Value = post.Title;
        //                 worksheet.Cell(currentRow, 2).Value = categoryNames;
        //                 if (!string.IsNullOrEmpty(desc))
        //                 {
        //                     currentRow++;
        //                     worksheet.Cell(currentRow, 1).Value = desc;
        //                 }
        //             }
        //         }

        //         using (var stream = new MemoryStream())
        //         {
        //             workbook.SaveAs(stream);
        //             var content = stream.ToArray();

        //             return content;


        //         }
        //     }
        // }
        public static byte[] DownloadPostCreateByFileXLSX(IEnumerable<dynamic> lstPost)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("BaoCao");
                worksheet.Cell(1, 1).Value = "STT";
                worksheet.Cell(1, 2).Value = "Tiêu đề";
                worksheet.Cell(1, 3).Value = "Ngày xuất bản";
                worksheet.Cell(1, 4).Value = "Tác giả";
                worksheet.Cell(1, 5).Value = "Nguồn";
                worksheet.Cell(1, 6).Value = "Biên tập viên";
                worksheet.Cell(1, 7).Value = "Người xuất bản";
                worksheet.Cell(1, 8).Value = "Danh mục";
                worksheet.Cell(1, 9).Value = "Người đăng";
                worksheet.Cell(1, 10).Value = "Số lượng ảnh";
                worksheet.Cell(1, 11).Value = "Số lượng từ";
                var currentRow = 1;
                foreach (var post in lstPost)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = currentRow-1;
                    worksheet.Cell(currentRow, 2).Value = post.Title;
                    worksheet.Cell(currentRow, 3).Value = post.PublishTime.ToString("dd/MM/yyyy HH:mm");
                    worksheet.Cell(currentRow, 4).Value = post.Author;
                    worksheet.Cell(currentRow, 5).Value = post.SourceLink;
                    worksheet.Cell(currentRow, 6).Value = post.CreatedBy;
                    worksheet.Cell(currentRow, 7).Value = post.PublishBy;
                    worksheet.Cell(currentRow, 8).Value = post.NameCategory;
                    worksheet.Cell(currentRow, 9).Value = post.CreatedBy;
                    worksheet.Cell(currentRow, 10).Value = post.NumberImage;
                    worksheet.Cell(currentRow, 11).Value = post.NumberWord;
                   
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return content;
                }
            }
        }
        // public static byte[] DownloadCommaSeperatedFileXLSX(CreateExcelPostRequest model, IEnumerable<dynamic> lstPost, string domain)
        // {
        //     using (var workbook = new XLWorkbook())
        //     {
        //         var worksheet = workbook.Worksheets.Add("BaoCao");
        //         int DayInterval = 1;

        //         DateTime StartDate = model.FromDay;
        //         DateTime EndDate = model.UntilDay;
        //         List<DateTime> dateList = new List<DateTime>();
        //         dateList.Add(StartDate);
        //         while (StartDate.AddDays(DayInterval) <= EndDate)
        //         {
        //             StartDate = StartDate.AddDays(DayInterval);
        //             dateList.Add(StartDate);
        //         }

        //         var currentRow = 1;
        //         foreach (var date in dateList)
        //         {
        //             var startDay = new DateTime(date.Year, date.Month, date.Day);
        //             var endDay = startDay.AddDays(1);

        //             var listPostCompare = lstPost.Where(x => x.PublishTime >= startDay && x.PublishTime < endDay).OrderBy(x => x.PublishTime).ToList();

        //             if (listPostCompare.Count() == 0)
        //             {
        //                 continue;
        //             }
        //             else
        //             {
        //                 currentRow++;
        //                 worksheet.Cell(currentRow, 1).Value = date.ToString("dd/MM/yyyy");
        //             }

        //             foreach (var post in listPostCompare)
        //             {
        //                 var desc = GetPlainTextFromHtml(post.Description);
        //                 var link = domain + "/" + post.CategorySlug + "/" + post.PostSlug;
        //                 currentRow++;
        //                 worksheet.Cell(currentRow, 1).Value = post.PostTitle;
        //                 worksheet.Cell(currentRow, 1).Hyperlink = new XLHyperlink(link);
        //                 worksheet.Cell(currentRow, 2).Value = post.CategoryName;
        //                 if (!string.IsNullOrEmpty(desc))
        //                 {
        //                     currentRow++;
        //                     worksheet.Cell(currentRow, 1).Value = desc;
        //                 }
        //             }
        //         }

        //         using (var stream = new MemoryStream())
        //         {
        //             workbook.SaveAs(stream);
        //             var content = stream.ToArray();

        //             return content;
        //         }
        //     }
        // }

        public static string GetPlainTextFromHtml(string htmlString)
        {
            string htmlTagPattern = "<.*?>";
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
            htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            htmlString = htmlString.Replace("&nbsp;", string.Empty);

            return htmlString;
        }
        public static int CountImageHtml(string HTMLCode)
        {
            var regex = new System.Text.RegularExpressions.Regex("<img[^>]*>");
            var plainString = regex.Replace(HTMLCode, "");
            var cnt = regex.Matches(HTMLCode).Count;
            return cnt;
        }

        public static async Task SendEmail(IEmailConfigService _iEmailConfigService, string subjectEmail, string email, string contentEmail)
        {
            var emailConfig = await _iEmailConfigService.GetOneAsync(x => x.IsActive);
            if (!(emailConfig is null))
            {
                // Gửi email theo danh sách
                using (MailMessage mail = new MailMessage())
                {
                    using (SmtpClient smtpServer = new SmtpClient(emailConfig.MailServer.Trim())
                    {
                        Credentials = new System.Net.NetworkCredential(emailConfig.Email.Trim(), emailConfig.Password),
                        Port = emailConfig.Port,
                        EnableSsl = emailConfig.EnableSSl
                    })
                    {
                        mail.From = new MailAddress(emailConfig.Email);
                        mail.Subject = subjectEmail;
                        mail.IsBodyHtml = true;

                        if (!string.IsNullOrEmpty(email))
                        {
                            try
                            {
                                mail.Body = contentEmail;
                                mail.To.Add(email);
                                smtpServer.Send(mail);

                                Console.WriteLine($"Send email success: " + email);
                                Thread.Sleep(500);
                            }
                            catch (System.Exception ex)
                            {
                                Console.WriteLine($"Send email error: " + email + " - " + ex.Message);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Email config invalid.");
            }
        }

        public static DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime StartOfDay(DateTime theDate)
        {
            return theDate.Date;
        }

        public static DateTime EndOfDay(DateTime theDate)
        {
            return theDate.Date.AddDays(1).AddTicks(-1);
        }

        public static DateTime? CeilDate(DateTime? date)
        {
            if (date != null)
            {
                var newDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
                newDate = newDate.AddDays(1);
                return newDate;
            }
            return null;
        }
        public static DateTime? FloorDate(DateTime? date)
        {
            if (date != null)
            {
                var newDate = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
                newDate = newDate.AddDays(-1);
                return newDate;
            }
            return null;
        }
    }
}