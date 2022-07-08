using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using XSystem.Security.Cryptography;

namespace NigWpfTest.Utils
{

    public class MyMD5
    {
        public static string GetMD5Hash(string str)
        {
            //就是比string往后一直加要好的优化容器
            StringBuilder sb = new StringBuilder();
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                //将输入字符串转换为字节数组并计算哈希。
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

                //X为     十六进制 X都是大写 x都为小写
                //2为 每次都是两位数
                //假设有两个数10和26，正常情况十六进制显示0xA、0x1A，这样看起来不整齐，为了好看，可以指定"X2"，这样显示出来就是：0x0A、0x1A。 
                //遍历哈希数据的每个字节
                //并将每个字符串格式化为十六进制字符串。
                int length = data.Length;
                for (int i = 0; i < length; i++)
                    sb.Append(data[i].ToString("X2"));

            }
            return sb.ToString();
        }

        //验证
        public static bool VerifyMD5Hash(string str, string hash)
        {
            string hashOfInput = GetMD5Hash(str);
            if (hashOfInput.CompareTo(hash) == 0)
                return true;
            else
                return false;
        }
    }
    //贴吧极速版Api
    class MiniTiebaApiUtil
    {
        public class MyFR
        {
            public class LikeForum
            {
                public string forum_id { get; set; }
                public string forum_name { get; set; }
                public string level_id { get; set; }
                public string is_sign { get; set; }
                public string avatar { get; set; }
            }
            public List<LikeForum> like_forum { get; set; }
        }
        private HttpClient myClient = new();

        public string jsons = null;
        /*public override string ToString()
        {        }*/
        public List<MyFR.LikeForum> TBData = null;
        public string ApiSign = null;
        //Cuid找时间物了
        private string getCUID()
        {
            string androidId = "9774d56d682e549c";//得到安卓id，可以是“”
            string imei = "206078481719601";//得到imei
            return MyMD5.GetMD5Hash("com.baidu" + androidId).ToUpper();//如果是第二种局面："com.baidu"再加上安卓id再转成大写取md5
        }
        public string getFinalCUID()
        {
            if (true)
            {
                return "D9F8CA041D76F7D5138DBE482DDFA210|106917184870602";//实机
            }
            string imei = "206078481719601";
            return getCUID() + "|" + new string(imei.ToCharArray().Reverse<char>().ToArray<char>());
        }
        //初始化工具
        public MiniTiebaApiUtil()
        {
            myClient.BaseAddress = new("http://c.tieba.baidu.com/");
            // myClient.DefaultRequestHeaders.Add("HOST", "tieba.baidu.com");
            /* myClient.DefaultRequestHeaders.Add("Cookie", """		
                hCekIxcDdzWWRuaEhjdU1MTll0OVJJaXBrR3NpdXNxdjZkNUlmQnU5MThFcWRpSVFBQUFBJCQAAAAAAAAAAAEAAAAGTtrtu6zKz7Srxua5pNf3ytIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAHyFf2J8hX9iVm
                """);*/
            // Console.WriteLine($"Final CUID: {getFinalCUID()}");
            myClient.DefaultRequestHeaders.Add("Cookie", "ka=open");
            myClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            myClient.DefaultRequestHeaders.Add("cuid", getFinalCUID());
            myClient.DefaultRequestHeaders.Add("cuid_galaxy2", getFinalCUID());
            myClient.DefaultRequestHeaders.Add("User_Agent", "bdtb for Android 7.2.0.0");
        }
        //取校验密钥
        private string GetSignByParam(Dictionary<string, string> Values, string Secret)
        {
            string LinShi = "";
            foreach (var a in Values)
            {
                LinShi += a.Key + "=" + a.Value;
            }
            LinShi += Secret;
            LinShi = MyMD5.GetMD5Hash(LinShi);

            return LinShi;
        }
        //取用户关注的贴吧列表
        public async Task GetTiebaList(Action CallBack)
        {
            Dictionary<string, string> kknd = new();
            kknd.Add("BDUSS", """		
            lhNd2kzfmQtdkE1VHlnUFVCWU5GMlU5czk5LWJVWnlRdFlhblpNWWRteEVuZHhpSUFBQUFBJCQAAAAAAAAAAAEAAAAGTtrtu6zKz7Srxua5pNf3ytIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEQQtWJEELViM
            """);
            string TimeCC = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
            //Console.WriteLine(TimeCC);
            kknd.Add("_client_id", $"wappc_{TimeCC}_{new Random().Next(100, 999)}");// "wappc_1656754562848_377");//
            kknd.Add("_client_type", "2");
            kknd.Add("_client_version", "7.2.0.0");
            kknd.Add("_os_version", "25");
            kknd.Add("_phone_imei", "206078481719601");
            kknd.Add("cuid", getFinalCUID());
            kknd.Add("cuid_galaxy2", getFinalCUID());
            kknd.Add("from", "1021636m");
            kknd.Add("like_forum", "1");
            kknd.Add("model", "SM-G988N");
            kknd.Add("net_type", "1");
            kknd.Add("recommend", "0");
            kknd.Add("stErrorNums", "1");
            kknd.Add("stMethod", "1");
            kknd.Add("stMode", "1");
            kknd.Add("stSize", "1204");
            kknd.Add("stTime", "154");
            kknd.Add("stTimesNum", "1");
            kknd.Add("subapp_type", "mini");
            kknd.Add("timestamp", TimeCC);//"1656754563279");// 
            kknd.Add("topic", "0");
            ApiSign = GetSignByParam(kknd, "tiebaclient!!!");
            kknd.Add("sign", ApiSign);// "8c6cdc3c4cea600bb8d4c689bcb9f6e5");//

            var encodedContent = new FormUrlEncodedContent(kknd);
            //在没有搞定sign算法之前凑合用着前面的坏康
            //var bbb = new StringContent("BDUSS=lhNd2kzfmQtdkE1VHlnUFVCWU5GMlU5czk5LWJVWnlRdFlhblpNWWRteEVuZHhpSUFBQUFBJCQAAAAAAAAAAAEAAAAGTtrtu6zKz7Srxua5pNf3ytIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAEQQtWJEELViM&_client_id=wappc_1656754562848_377&_client_type=2&_client_version=7.2.0.0&_os_version=25&_phone_imei=206078481719601&cuid=D9F8CA041D76F7D5138DBE482DDFA210%7C106917184870602&cuid_galaxy2=D9F8CA041D76F7D5138DBE482DDFA210%7C106917184870602&from=1021636m&like_forum=1&model=SM-G988N&net_type=1&recommend=0&stErrorNums=1&stMethod=1&stMode=1&stSize=1204&stTime=154&stTimesNum=1&subapp_type=mini&timestamp=1656754563279&topic=0&sign=20641290689D319A0B8BC815BEA22D50");// encodedContent).ConfigureAwait(false);
            /*string ccc = await encodedContent.ReadAsStringAsync();
            Console.WriteLine(ccc);*/
            //Console.WriteLine(await bbb.ReadAsStringAsync());
            //return;
            try
            {
                var something = await myClient.PostAsync("/c/f/forum/forumrecommend", encodedContent);// new StringContent( ccc));
                something.EnsureSuccessStatusCode();
                jsons = await something.Content.ReadAsStringAsync();
                MyFR fR = Newtonsoft.Json.JsonConvert.DeserializeObject<MyFR>(jsons);
                //Trace.WriteLine(infoBean.data.name_show);
                TBData = fR.like_forum;
                CallBack();
            }
            catch (Exception e)
            {
                Console.WriteLine("！报错（取贴吧列表）！头\n转文本：" + e.ToString() + "\n消息：\n" + e.Message + "！报错！尾");
            }
        }
    }
}
