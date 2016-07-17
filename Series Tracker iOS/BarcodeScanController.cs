using Foundation;
using System;
using System.Collections;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using UIKit;
using System.Net.Http;
using System.Threading.Tasks;

namespace Series_Tracker_iOS
{
    partial class BarcodeScanController : UIViewController
    {
        public BarcodeScanController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            b_Scan.TouchUpInside += BarcodeButtonClicked;
            b_Submit.TouchUpInside += SubmitButtonClicked;
            b_Spinner.Hidden = true;
        }

        void SubmitButtonClicked(object sender, EventArgs e)
        {
            ISBN = t_ISBN.Text.ToString();

            FindBookInformation();
        }

        async void BarcodeButtonClicked(object sender, EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var isbn = await scanner.Scan();

            ISBN = isbn.ToString();

            FindBookInformation();
        }

        public static string ISBN;
        public static List<string> TitleURL = new List<string>();
        public static List<string> ImgURL = new List<string>();
        public static List<string> PubDateURL = new List<string>();
        public static List<string> DescriptionsURL = new List<string>();
        public static int numberSeries = 3;

        async void FindBookInformation()
        {
            b_Spinner.Hidden = false;
            b_Spinner.StartAnimating();

            TitleURL = new List<string>();
            ImgURL = new List<string>();
            PubDateURL = new List<string>();
            DescriptionsURL = new List<string>();

            var client = new HttpClient();

            //get Series information
            string GR_url = "https://www.goodreads.com/book/isbn_to_id/" + ISBN;
            var GR_html = await client.GetStringAsync(GR_url);
            string GR_SeriesCode = getBetween(GR_html, "<meta property=\"og:url\" content=\"https://www.goodreads.com/work/best_book/", "\"/>");

            GR_url = "https://www.goodreads.com/work/" + GR_SeriesCode + "/series?format=xml&key=" + Config.GR_Key;
            var GR_XML = await client.GetStringAsync(GR_url);
            string GR_SeriesID = getBetween(GR_XML, "<series>", "<title>");
            GR_SeriesID = getBetween(GR_SeriesID, "<id>", "</id>");

            GR_url = "https://www.goodreads.com/series/" + GR_SeriesID + "?format=xml&key=" + Config.GR_Key;
            GR_XML = await client.GetStringAsync(GR_url);

            if (s_IncludeAll.On)
            {
                numberSeries = Int32.Parse(getBetween(GR_XML, "<series_works_count>", "</series_works_count>"));
            }
            else
            {
                numberSeries = Int32.Parse(getBetween(GR_XML, "<primary_work_count>", "</primary_work_count>"));
            }

            GR_XML = RemoveTop(GR_XML);

            if (s_IncludeAll.On)
            {
                for (int i = 0; i < numberSeries; i++)
                {
                    await GetBookInformation(GR_XML);
                    GR_XML = RemoveLastBook(GR_XML);
                }
            }
            else
            {
                for (int i = 0; i < numberSeries; i++)
                {
                    if (getBetween(GR_XML, "<user_position>", "</user_position>").Length <= 2)
                    {
                        await GetBookInformation(GR_XML);
                    }
                    else
                    {
                        i--;
                    }
                    GR_XML = RemoveLastBook(GR_XML);
                }
            }

            b_Spinner.Hidden = true;
            b_Spinner.StopAnimating();
            this.PerformSegue("ScanComplete", this);
        }

        async Task GetBookInformation(string XML)
        {
            TitleURL.Add(getBetween(XML, "<title>", " (").Replace("</title>", ""));
            ImgURL.Add(getBetween(XML, "<![CDATA[", "]]>"));
            PubDateURL.Add(getBetween(XML, "<original_publication_year>", "</original_publication_year>"));

            var client = new HttpClient();

            string XMLFirstPass = getBetween(XML, "<best_book>", "<title>");
            string XMLId = getBetween(XMLFirstPass, "<id>", "</id>");
            string GR_url = "https://www.goodreads.com/book/show/" + XMLId + ".xml?key=" + Config.GR_Key;
            var GR_XML = await client.GetStringAsync(GR_url);
            XMLFirstPass = getBetween(GR_XML, "<description>", "</description>");
            DescriptionsURL.Add(getBetween(XMLFirstPass, "<![CDATA[", "]]>"));
        }

        string RemoveTop(string text)
        {
            string finalString = text.Remove(0, text.IndexOf("<series_work>"));
            return finalString;
        }

        string RemoveLastBook(string text)
        {
            string finalString = text.Remove(0, text.IndexOf("</work>") + 7);
            return finalString;
        }

        public static string getBetween(string strSource, string strStart, string strEnd)
        {
            int Start, End;
            if (strSource.Contains(strStart) && strSource.Contains(strEnd))
            {
                Start = strSource.IndexOf(strStart, 0) + strStart.Length;
                End = strSource.IndexOf(strEnd, Start);
                return strSource.Substring(Start, End - Start);
            }
            else
            {
                return "";
            }
        }
    }
}
