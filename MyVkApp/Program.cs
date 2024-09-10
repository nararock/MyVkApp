using MyVkApp;
using MyVkApp.SerializationClass;
using System.Collections.Generic;
using System.Net.Http;


bool answer = false;
int number = 0;
while (answer == false)
{
    Console.WriteLine("Выберите номер варианта действия:\n " +
        "1 - Найти всех неактивных подписчиков страницы за выбранный промежуток времени;\n" +
        "2 - Найти общего друга по принципу \"шести рукопожатий\".");
    answer =  int.TryParse(Console.ReadLine(), out number);
}

switch (number)
{
    case 1:
        FindExtraFriends findExtra = new FindExtraFriends();
        findExtra.Do();
        break;
    case 2:
        SixHandshakes sixHandshakes = new SixHandshakes();
        await sixHandshakes.Do();
        break;
}
