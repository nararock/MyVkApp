using MyVkApp.SerializationClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVkApp
{
    public class FindExtraFriends
    {
        private string AccessToken;
        private string OwnerId;
        private int PostNumber;
        private int YearNumber;

        public FindExtraFriends()
        {
            Console.WriteLine("Токен пользователя. Получить свой токен можно по ссылке:\r\n     " +
                "https://oauth.vk.com/authorize?client_id=51417040&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=offline&response_type=token&v=5.131\r\n " +
                "Произойдет переадресация на другую страницу и в адресной строке можно скопировать свой токен.\r\n");
            Console.WriteLine("Токен пользователя: ");
            AccessToken = Console.ReadLine();
            Console.WriteLine("ID страницы пользователя. Скопировать id можно из адресной строки страницы пользователя ВК.");
            Console.WriteLine("ID страницы пользователя");
            OwnerId = Console.ReadLine();
            Console.WriteLine("Количество постов со стены");
            int.TryParse(Console.ReadLine(), out PostNumber);
            Console.WriteLine("Промежуток времени, за который посты актуальны (количество лет): ");
            bool yearParse = int.TryParse(Console.ReadLine(), out YearNumber);            
        }

        public async void Do()
        {
            List<VKUserProfile> userProfiles = await GetNotActiveUsers();
            ConsoleNotActiveUsers(userProfiles);
        }

        private async Task<List<VKUserProfile>> GetNotActiveUsers()
        {
            DateTime curDate = DateTime.Today.AddYears(YearNumber * (-1));
            int countRead = PostNumber < 50 ? PostNumber : 50;
            int offset = 0;
            List<int> idPost = [];
            //получение списка постов за указанный срок
            while (idPost.Count < PostNumber)
            {
                List<int> tempID = await ExtraFriends.GetPosts(OwnerId, offset, countRead, AccessToken, curDate);
                if (tempID == null) break;
                idPost.AddRange(tempID);
                offset += countRead;
            }
            //получение списка активных пользователей постов 
            List<string> idActiveUsers = [];
            foreach (var id in idPost)
            {
                VKLikes res = await ExtraFriends.GetLikes(AccessToken, OwnerId, id);
                foreach (var item in res.response.items)
                {
                    if (!idActiveUsers.Contains(item)) idActiveUsers.Add(item);
                }
                Thread.Sleep(300);
            }

            idActiveUsers.Distinct();

            //получение списка всех друзей
            VKUser vKUser = await ExtraFriends.GetUsers(AccessToken, OwnerId);

            List<VKUserProfile> NotActiveUsers = vKUser.response.items.Where(i => !idActiveUsers.Contains(i.id)).ToList();
            return NotActiveUsers;
        }

        private void ConsoleNotActiveUsers(List<VKUserProfile> NotActiveUsers)
        {
            foreach (var item in NotActiveUsers)
            {
                Console.WriteLine(item.first_name + " " + item.last_name);
            }
            Console.WriteLine();
        }
    }
}
