using MyVkApp.SerializationClass;
using MyVkApp;

int offset = 0;
List<int> idPost = [];

/*
 * Токен пользователя. Получить свой токен можно по ссылке:
 * https://oauth.vk.com/authorize?client_id=51417040&display=page&redirect_uri=https://oauth.vk.com/blank.html&scope=offline&response_type=token&v=5.131
 * Произойдет переадресация на другую страницу и в адресной строке можно скопировать свой токен.
 */
string access_token = "";

Console.WriteLine("Количество постов со стены");
bool postParse = int.TryParse(Console.ReadLine(),out int postNumber);
Console.WriteLine("Промежуток времени, за который посты актуальны (количество лет): ");
bool yearParse = int.TryParse(Console.ReadLine(), out int yearNumber);
DateTime curDate = DateTime.Today.AddYears(yearNumber * (-1));
int countRead = postNumber < 50 ? postNumber : 50;
/*
 * ID страницы пользователя. Скопировать id можно из адресной строки страницы пользователя ВК.
 */
string owner_id = "";

//получение списка постов за указанный срок
while (postParse == true && idPost.Count < postNumber)
{
    List<int> tempID = await ExtraFreinds.GetPosts(owner_id, offset, countRead, access_token, curDate);
    if (tempID == null) break;
    idPost.AddRange(tempID);
    offset += countRead;
}

//получение списка активных пользователей постов 
List<string> idActiveUsers = [];
foreach(var id in idPost)
{
    VKLikes res = await ExtraFreinds.GetLikes(access_token, owner_id, id);
    foreach(var item in res.response.items)
    {
        if (!idActiveUsers.Contains(item)) idActiveUsers.Add(item);
    }
    Thread.Sleep(300);
}

idActiveUsers.Distinct();

//получение списка всех друзей
VKUser vKUser = await ExtraFreinds.GetUsers(access_token, owner_id);

List<VKUserProfile> NotActiveUsers = vKUser.response.items.Where(i => !idActiveUsers.Contains(i.id)).ToList();

foreach (var item in NotActiveUsers)
{
    Console.WriteLine(item.first_name + " " + item.last_name);
}
Console.WriteLine();

