using MyVkApp.SerializationClass;
using Newtonsoft.Json;

namespace MyVkApp
{
    public static class ExtraFriends
    {
        private static DateTime date = new DateTime(1970, 1, 1);
        private static HttpClient httpClient = new HttpClient();
        private static Dictionary<string, List<VKUserProfile>> CashFriends = new Dictionary<string, List<VKUserProfile>>();
        public static int Counter { get; set; }

        public static async Task<List<int>> GetPosts(string owner_id, int offset, int countRead, string access_token, DateTime curDate)
        {
            var message = await httpClient.PostAsync("https://api.vk.com/method/wall.get" +
            $"?owner_id={owner_id}" +
            $"&offset={offset}" +
            $"&count={countRead}" +
            $"&access_token={access_token}" +
            "&v=5.199", null);

            Counter++;

            string result = await message.Content.ReadAsStringAsync();
            VKData res = JsonConvert.DeserializeObject<VKData>(result);
            if (res.response.items.Count == 0) return null;
            else if (date.AddSeconds(res.response.items[0].date).Year < curDate.Year) return null;
            List<int> tempID = res.response.items.Where(v =>
            {
                DateTime dateVK = date.AddSeconds(v.date);
                return dateVK.Year >= curDate.Year;})
                .Select(d => d.id).ToList();
            return tempID;
        }

        public static async Task<VKLikes> GetLikes(string access_token, string owner_id, int item_id)
        {
            var message = await httpClient.PostAsync("https://api.vk.com/method/likes.getList" +
                $"?access_token={access_token}" +
                "&type=post" +
                $"&owner_id={owner_id}" +
                $"&item_id={item_id}" +
                "&v=5.199", null);

            Counter++;

            string result = await message.Content.ReadAsStringAsync();
            VKLikes res = JsonConvert.DeserializeObject<VKLikes>(result);
            return res;
        }

        public static async Task<VKUser> GetUsers(string access_token, string owner_id, List<string> sources = null)
        {
            VKUser vKUser = GetFromDictionary(owner_id);
            if (vKUser != null) return vKUser;
            else {
                var users = await httpClient.PostAsync("https://api.vk.com/method/friends.get" +
                $"?access_token={access_token}" +
                $"&user_id={owner_id}" +
                "&fields=nickname" +
                "&v=5.199", null);

                Counter++;

                string userResult = await users.Content.ReadAsStringAsync();
                vKUser = JsonConvert.DeserializeObject<VKUser>(userResult);
                foreach (var user in vKUser.response.items)
                {
                    if (sources != null) user.sourceID.AddRange(sources);
                    user.sourceID.Add(owner_id);
                }
                AddToDictionary(owner_id, vKUser);
                Console.WriteLine($"У юзера с id {owner_id} получено {vKUser.response.items.Count} друзей");
                return vKUser;
            }           
        }

        private static void AddToDictionary(string id, VKUser vKUser)
        {
            if (!CashFriends.ContainsKey(id)) CashFriends.Add(id, vKUser.response.items);
        }

        private static bool FindInDictionary(string id)
        {
            return CashFriends.ContainsKey(id); 
        }

        private static VKUser? GetFromDictionary(string id)
        {            
            if (FindInDictionary(id))
            {
                VKUser vKUser = new VKUser();
                List<VKUserProfile> vKUserProfiles = CashFriends[id];
                vKUser.response.items = vKUserProfiles;
                vKUser.response.count = vKUserProfiles.Count;
                Console.WriteLine($"Пользователь {id} найден в словаре.");
                return vKUser;
            }
            return null;
        }
    }
}
