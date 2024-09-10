using MyVkApp.SerializationClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyVkApp
{
    public class SixHandshakes
    {
        public string OwnerId1;
        public string OwnerId2;
        public string AccessToken;

        public SixHandshakes()
        {
            Console.WriteLine("Введите Id первого пользователя");
            OwnerId1 = Console.ReadLine();
            Console.WriteLine("Введите Id второго пользователя");
            OwnerId2 = Console.ReadLine();
            Console.WriteLine("Токен пользователя. Получить свой токен можно по ссылке:\r\n     " +
                "https://oauth.vk.com/authorize?client_id=51417040&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=offline&response_type=token&v=5.131\r\n " +
                "Произойдет переадресация на другую страницу и в адресной строке можно скопировать свой токен.\r\n");
            Console.WriteLine("Токен пользователя: ");
            AccessToken = Console.ReadLine();
        }

        public async Task Do()
        {
            string commonFriend = await FindSolution(OwnerId1, OwnerId2);
            if (commonFriend == "") Console.WriteLine("Общих друзей не обнаружено");
            else { 
                Console.WriteLine(commonFriend); 
            }
            Console.WriteLine($"Количество запросов: {ExtraFriends.Counter}");
        }

        private async Task<List<VKUserProfile>> GetFriends(string Id, List<string> sources = null)
        {
            VKUser vKUser = await ExtraFriends.GetUsers(AccessToken, Id,sources);
            return vKUser.response.items;
        }

        private async Task<string> FindIntersection(List<VKUserProfile> userFreinds1, List<VKUserProfile> userFreinds2)
        {
            if (userFreinds1 == null || userFreinds2 == null || userFreinds1.Count == 0 || userFreinds2.Count == 0) return "";
            StringBuilder builder = new();
            
            VKUserProfile answer = null;
            foreach (var item in userFreinds1)
            {
                answer = userFreinds2.FirstOrDefault(x => x.id == item.id, null);
                if (answer != null)
                {
                    List<string> answerChain = [];
                    answerChain.AddRange(answer.sourceID);
                    answerChain.Add(answer.id);
                    item.sourceID.Reverse();
                    answerChain.AddRange(item.sourceID);
                    
                    foreach(var id in answerChain)
                    {
                        string temp = await GetName(id);
                        builder.Append(temp + " |");
                    }
                    break;
                }
            }
            return  answer != null ? builder.ToString() : "";
        }

        private async Task<string> FindSolution(string id1, string id2)
        {
            if (id1 == id2) return "";

            List<VKUserProfile> vKUsers1 = await GetFriends(id1);
            List<VKUserProfile> vKUsers2 = await GetFriends(id2);

            string commonFriend = await FindIntersection(vKUsers1, vKUsers2);
            if (commonFriend != "") return commonFriend;

            List<VKUserProfile> vKUsers12 = [];
            string commonFriend12 = await FindCommonFriend(vKUsers1, vKUsers2, vKUsers12);
            if (commonFriend12 != "") return commonFriend12;

            List<VKUserProfile> vKUsers22 = [];
            string commonFriend22 = await FindCommonFriend(vKUsers2, vKUsers12, vKUsers22);
            if (commonFriend22 != "") return commonFriend22;

            List<VKUserProfile> vKUsers123 = [];
            string commonFriend123 = await FindCommonFriend(vKUsers12, vKUsers22, vKUsers123);
            if (commonFriend123 != "") return commonFriend123;

            List<VKUserProfile> vKUsers223 = [];
            string commonFriend223 = await FindCommonFriend(vKUsers22, vKUsers123, vKUsers223);
            if (commonFriend223 != "") return commonFriend223;

            return "";
        }

        private async Task<string> FindCommonFriend(List<VKUserProfile> sourceFriends, List<VKUserProfile> compareFriends, List<VKUserProfile> result)
        {
            if (sourceFriends.Count == 0 || compareFriends.Count == 0) return "";
            for (int i = 0; i < sourceFriends.Count; i++)
            {
                if (!sourceFriends[i].is_closed && sourceFriends[i].deactivated != "banned" && sourceFriends[i].deactivated != "deleted")//проверка на приватный аккаунт
                {
                    List<VKUserProfile> vKUsersTemp = await GetFriends(sourceFriends[i].id, sourceFriends[i].sourceID);
                    if (i > 20 && i % 2 == 0) Thread.Sleep(500);
                    string commonFriend1 = await FindIntersection(vKUsersTemp, compareFriends);
                    result.AddRange(vKUsersTemp);
                    if (commonFriend1 != "") return commonFriend1;
                }                              
            }
            return "";
        }

        private async Task<string> GetName(string id)
        {
            HttpClient httpClient = new HttpClient();
            var users = await httpClient.PostAsync("https://api.vk.com/method/users.get" +
                $"?access_token={AccessToken}" +
                $"&user_id={id}" +
                "&v=5.199", null);
            string userResult = await users.Content.ReadAsStringAsync();
            VKCommonUser vKUser = JsonConvert.DeserializeObject<VKCommonUser>(userResult);
            return vKUser.response[0].first_name + " " + vKUser.response[0].last_name;
        }
    }
}
