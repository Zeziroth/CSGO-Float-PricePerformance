using Agility = HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace PricePerformance
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        Dictionary<string, Bitmap> imgCache = new Dictionary<string, Bitmap>();
        public Form1()
        {
            InitializeComponent();
            Web.Initialize();
        }
        private void GetListings(string hash_name)
        {
            string search = Web.MakeRequest("https://steamcommunity.com/market/listings/730/" + hash_name + "/render/?query=&start=0&count=1&country=DE&language=german&currency=3");
            dynamic searchJson = JsonConvert.DeserializeObject<dynamic>(search);
            int start = 0;
            int pageSize = 100;
            int debugLimit = 100;
            int total_count = searchJson.total_count;

            List<Article> articles = new List<Article>();

            do
            {
                search = Web.MakeRequest("https://steamcommunity.com/market/listings/730/" + hash_name + "/render/?query=&start=" + start + "&count=" + pageSize + "&country=DE&language=german&currency=3");
                var searchJsonTMP = JObject.Parse(search);
                var listings = searchJsonTMP["listinginfo"].ToObject<Dictionary<string, Article>>();

                foreach (var entry in listings)
                {
                    articles.Add(entry.Value);
                }

                start += pageSize;
            } while ((start + pageSize) <= debugLimit);


            double bestResult = double.MaxValue;
            double bestFloat = 0d;
            Article bestArticle = new Article();

            double lowResult = double.MaxValue;
            double lowFloat = 1d;
            Article lowArticle = new Article();

            double maxResult = double.MaxValue;
            double maxFloat = 0d;
            Article maxArticle = new Article();

            foreach (Article article in articles)
            {
                string apiUrl = article.asset.market_actions[0].link;
                apiUrl = apiUrl.Replace("%listingid%", article.listingid);
                apiUrl = apiUrl.Replace("%assetid%", article.asset.id);
                apiUrl = "https://api.csgofloat.com/?url=" + apiUrl;

                string floatResp = Web.MakeRequest(apiUrl);
                FloatArticle floatArticle = JsonConvert.DeserializeObject<FloatArticle>(floatResp);


                double floatVal = floatArticle.iteminfo.floatvalue;
                double articlePrice = ((article.converted_price + article.converted_fee) / 100d);
                double result = articlePrice * floatVal;

                if (result < bestResult)
                {
                    bestArticle = article;
                    bestFloat = floatVal;
                    bestResult = result;
                }

                if (floatVal < lowFloat)
                {
                    lowFloat = floatVal;
                    lowResult = result;
                    lowArticle = article;
                }

                if (floatVal > maxFloat)
                {
                    maxFloat = floatVal;
                    maxResult = result;
                    maxArticle = article;
                }
            }


            StringBuilder strBuild = new StringBuilder("===== RESULT =====");
            strBuild.AppendLine("");
            strBuild.AppendLine("Lowest Float-Value: " + lowResult + " (" + ((lowArticle.converted_price + lowArticle.converted_fee) / 100d) + "€) | Float-Value: " + lowFloat);
            strBuild.AppendLine("Best Price/Performance Float-Value: " + bestResult + " (" + ((bestArticle.converted_price + bestArticle.converted_fee) / 100d) + "€) | Float-Value: " + bestFloat);
            strBuild.AppendLine("Highest Float-Value: " + maxResult + " (" + ((maxArticle.converted_price + maxArticle.converted_fee) / 100d) + "€) | Float-Value: " + maxFloat);

            Clipboard.SetText(strBuild.ToString());
            MessageBox.Show("Watch your clipboard ;)");
        }
        private void metroButton_Search_Click(object sender, EventArgs e)
        {
            Core.RunThread(PerformSearch);
        }

        private void PerformSearch()
        {
            Invoker.ClearControls(panel1);
            string search = Web.MakeRequest(string.Format(Base.MARKET_URL, Base.APP_ID, searchTextBox.Text, "any", 0));
            SearchQuery searchJson = JsonConvert.DeserializeObject<SearchQuery>(search);

            int start = searchJson.start;
            int pageSize = searchJson.pagesize;
            int total_count = searchJson.total_count;

            Invoker.SetPrgbState(metroProgressBar1, total_count, Invoker.Mode.SetMaximum);

            List<Result> results = new List<Result>();

            do
            {
                search = Web.MakeRequest(string.Format(Base.MARKET_URL, Base.APP_ID, searchTextBox.Text, "any", start));
                searchJson = JsonConvert.DeserializeObject<SearchQuery>(search);

                foreach (Result result in searchJson.results)
                {
                    results.Add(result);
                }

                start += pageSize;
                Invoker.SetPrgbState(metroProgressBar1, start, Invoker.Mode.SetValue);
            } while ((start + pageSize) <= total_count);


            int y = 10;

            Invoker.SetPrgbState(metroProgressBar1, 0, Invoker.Mode.SetValue);
            Invoker.SetPrgbState(metroProgressBar1, results.Count, Invoker.Mode.SetMaximum);

            foreach (Result result in results)
            {
                Invoker.SetPrgbState(metroProgressBar1, metroProgressBar1.Value + 1, Invoker.Mode.SetValue);
                Panel panel = new Panel();
                panel.BackColor = Color.Black;
                panel.Height = 72;
                panel.Width = panel1.Width - 22;
                panel.Location = new Point(2, y);
                panel.Cursor = Cursors.Hand;
                panel.MouseEnter += (s, arg) => ((Panel)s).BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                panel.MouseLeave += (s, arg) => ((Panel)s).BorderStyle = System.Windows.Forms.BorderStyle.None;
                panel.MouseClick += (s, arg) => GetListings(result.hash_name);

                //PictureBox
                PictureBox picBox = new PictureBox();
                picBox.BackColor = System.Drawing.ColorTranslator.FromHtml("#333333");
                string imgUrl = "https://steamcommunity-a.akamaihd.net/economy/image/" + result.asset_description.icon_url + "/62fx62f";
                if (!imgCache.ContainsKey(imgUrl))
                {
                    System.Net.WebRequest request = System.Net.WebRequest.Create(imgUrl);
                    System.Net.WebResponse response = request.GetResponse();
                    System.IO.Stream responseStream = response.GetResponseStream();
                    Bitmap bitmap = new Bitmap(responseStream);

                    imgCache.Add(imgUrl, bitmap);
                }


                picBox.Image = imgCache[imgUrl];
                picBox.Load("https://steamcommunity-a.akamaihd.net/economy/image/" + result.asset_description.icon_url + "/62fx62f");
                picBox.SizeMode = PictureBoxSizeMode.Zoom;
                picBox.Size = new Size(62, 62);
                picBox.Location = new Point(5, 5);
                picBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                picBox.Cursor = Cursors.Hand;
                panel.Controls.Add(picBox);

                //Labels

                //Label Name
                Label labelName = new Label();
                labelName.AutoSize = true;
                labelName.Text = result.name;
                labelName.Location = new Point(picBox.Right + 7, 21);
                labelName.ForeColor = System.Drawing.ColorTranslator.FromHtml("#" + result.asset_description.name_color);
                labelName.Font = new Font(new FontFamily("Arial"), 11, FontStyle.Bold);
                labelName.Cursor = Cursors.Hand;
                panel.Controls.Add(labelName);

                //Label Amount
                Label labelAmount = new Label();
                labelAmount.AutoSize = true;
                labelAmount.Text = "x" + result.sell_listings + " | " + result.sell_price_text;
                labelAmount.Location = new Point(picBox.Right + 7, labelName.Bottom + 3);
                labelAmount.ForeColor = System.Drawing.ColorTranslator.FromHtml("#8F98A0");
                labelAmount.Font = new Font(new FontFamily("Arial"), 9, FontStyle.Bold);
                labelAmount.Cursor = Cursors.Hand;
                panel.Controls.Add(labelAmount);

                Invoker.AddControl(panel1, panel);
                y += 86;
            }
        }
    }
}
