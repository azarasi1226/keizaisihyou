using System;
using System.Text.RegularExpressions;

namespace Model.EconomicIndicators
{
    public class EIBean
    {
        // 発表日付
        public DateTime AnnouncementDate { get; }

        // 発表国
        public string Country { get; }

        // 重要度(1~3)
        public string Importance { get; }

        // 発表内容
        public string Content { get; }

        // 前回の結果
        public string LastResult { get; }

        // 予測値
        public string Prediction { get; }

        public EIBean(string date, Match match)
        {
            // 為替の世界は30時間制なので、日付をTimeSpanで加算して求める（普通にやるとエラーになる）
            DateTime announcementDate = DateTime.Parse($"{DateTime.Now.Year}/{date}");
            var time = Regex.Split(match.Groups["AnnouncementTime"].Value, @"\D");
            TimeSpan span = new TimeSpan(int.Parse(time[0]), int.Parse(time[1]), 0);
            AnnouncementDate = announcementDate.Add(span);
            
            // 内容から<a>タグでハイパーリンクされた箇所を削除
            Content = Regex.Replace(match.Groups["Content"].Value, @"(.*?)<a.*?>(.*?)</a>", "$1$2");

            // <br>タグより先を削除
            LastResult = Regex.Replace(match.Groups["LastResult"].Value, @"<br>.*", String.Empty);

            Country = match.Groups["Country"].Value;
            Importance = match.Groups["Importance"].Value;
            Prediction = match.Groups["Prediction"].Value;
        }

        public override string ToString()
        {
            return  $"--------------------------------\n" +
                    $"発表日時　：{AnnouncementDate}\n" +
                    $"発表国　　:{Country}\n" +
                    $"重要度　　:{Importance}\n" +
                    $"発表内容　：{Content}\n" +
                    $"前回の結果：{LastResult}\n" +
                    $"予想値　　：{Prediction}\n\n";
        }
    }
}