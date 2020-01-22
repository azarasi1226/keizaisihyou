using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Model.EconomicIndicators
{
    public class EIManager
    {
        // ダウンロード先URL
        private static readonly string URL =
            @"https://info.finance.yahoo.co.jp/fx/marketcalendar/";

        // 日ごとにカテゴライズする正規表現 language=regex
        private static readonly string ExtractDay_regex =
            @"(?si)""date"">(?<Day>.*?)</th>(?<Content>.*?)(<th|</table)";

        // 日ごとにカテゴライズされたデータから経済指標を取り出す正規表現 language=regex
        private static readonly string ExtractEI_regex =
            @"(?sxi)""time"">              (?<AnnouncementTime>\d\d:\d\d)  </td.*?  #形式（時：分）
                    ""event"">.*?\sico     (?<Country>.{3})                .*?      #形式 (Jpy, Gbr)
                              span>        (?<Content>.*?)                 </td.*?  #<a>タグ未処理
                    ""priority"">.*?Rating (?<Importance>\d)               .*?      #形式 (1~3の数字)
                    ""last"">              (?<LastResult>.*?)              </td.*?  #<br>タグより先未処理
                    ""expectation"">       (?<Prediction>.*?)              </td>    #存在しなかったら空文字";

        /// <summary>
        /// 経済指標データを非同期にWEBから抜き取ってくる（非常に時間がかかる場合がある）
        /// </summary>
        public static async Task<IEnumerable<EIBean>> GetEIAsync()
        {
             // タスク作成・実行
             Task<IEnumerable<EIBean>> result = Task.Run(() =>
              {
                  var days = ExtractDays();
                  var eis  = ExtractEI(days);

                  return eis;
              });

            return await result;
        }

        /// <summary>
        /// 指定されたURLからダウンロードしたHTMLソース内の、日ごとにカテゴライズしてデータを抜き取る
        /// </summary>
        private static MatchCollection ExtractDays()
        {
            using (var client = new HttpClient())
            {
                // HTMLソースを同期的にダウンロード
                var html = client.GetStringAsync(URL).Result;
                var mathes = Regex.Matches(html, ExtractDay_regex);

                return mathes;
            }
        }

        /// <summary>
        /// カテゴライズされたデータの中から、経済指標のデータだけを更に抜き取る
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static IEnumerable<EIBean> ExtractEI(MatchCollection str)
        {
            var eiBeans = new List<EIBean>();

            foreach (Match item in str)
            {
                //　大分類要素である日付だけを取り出す
                string date = Regex.Match(item.Groups["Day"].Value, @"\d{1,2}\/\d{1,2}").Value;

                //　日でカテゴライズされた中にある、経済指標のデータだけ取り出す
                var matches = Regex.Matches(item.Groups["Content"].Value, ExtractEI_regex);

                foreach(Match item2 in matches)
                {
                    var eiBean = new EIBean(date, item2);

                    eiBeans.Add(eiBean);
                }
            }

            return eiBeans;
        }
    }
}
