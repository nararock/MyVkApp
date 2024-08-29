using MyVkApp.SerializationClass;
using Newtonsoft.Json;

namespace MyVkApp
{
    public static class ExtraFreinds
    {
        private static DateTime date = new DateTime(1970, 1, 1);
        private static HttpClient httpClient = new();

        public static async Task<List<int>> GetPosts(string owner_id, int offset, int countRead, string access_token, DateTime curDate)
        {            
            var message = await httpClient.PostAsync("https://api.vk.com/method/wall.get" +
            $"?owner_id={owner_id}" +
            $"&offset={offset}" +
            $"&count={countRead}" +
            $"&access_token={access_token}" +
            "&v=5.199", null);
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
            string result = await message.Content.ReadAsStringAsync();
            VKLikes res = JsonConvert.DeserializeObject<VKLikes>(result);
            return res;
        }

        public static async Task<VKUser> GetUsers(string access_token, string owner_id)
        {
            var users = await httpClient.PostAsync("https://api.vk.com/method/friends.get" +
                $"?access_token={access_token}" +
                $"&user_id={owner_id}" +
                "&fields=nickname" +
                "&v=5.199", null);
            string userResult = await users.Content.ReadAsStringAsync();
            VKUser vKUser = JsonConvert.DeserializeObject<VKUser>(userResult);
            return vKUser;
        }
    }
}
