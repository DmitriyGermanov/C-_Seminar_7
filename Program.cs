using Seminar_7;
using System.Reflection;
using System.Text;
var test = TestClassBuilder(1, "test", 1.1m, new char[] { 'a', 'b', 'c' });
Console.WriteLine(ObjectToString(test));
var obj = StringToObject(ObjectToString(test));
Console.WriteLine(ObjectToString(obj));

static TestClass TestClassBuilderWithEmptyConstructor()
{
    Type testclass = typeof(TestClass);
    return Activator.CreateInstance(testclass) as TestClass;
}

static TestClass TestClassBuilderWithSimpleConstructor(int i)
{
    Type testclass = typeof(TestClass);

    return Activator.CreateInstance(testclass, new object[] { i }) as TestClass;
}

static TestClass TestClassBuilder(int i, string s, decimal d, char[] c)
{
    Type testclass = typeof(TestClass);

    return Activator.CreateInstance(testclass, new object[] { i, s, d, c }) as TestClass;
}

/*Напишите 2 метода использующие рефлексию
1 - сохраняет информацию о классе в строку
2- позволяет восстановить класс из строки с информацией о методе
В качестве примере класса используйте класс TestClass.
Шаблоны методов для реализации:
static object StringToObject(string s) { }
static string ObjectToString(object o) { }
Подсказка:
Строка должна содержать название класса, полей и значений
Ограничьтесь диапазоном значений представленном в классе
Если класс находится в тоже сборке (наш вариант) то можно не указывать имя сборки в паремтрах активатора.
Activator.CreateInstance(null, “TestClass”) - сработает;
Для простоты представьте что есть только свойства. Не анализируйте поля класса.
Пример того как мог быть выглядеть сохраненный в строку объект: “TestClass, test2, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null:TestClass | I:1 | S:STR | D:2.0 |”
Ключ - значения разделяются двоеточием а сами пары - вертикальной чертой.*/

static string ObjectToString(object o)
{
    var sb = new StringBuilder();
    Type type = o.GetType();
    sb.Append(type.AssemblyQualifiedName);
    sb.Append(":");
    sb.Append(type.Name + " |");
    var properties = type.GetProperties();
    foreach (var pr in properties)
    {
        var temp = pr.GetValue(o);
        var customNameAttribute = pr.GetCustomAttribute<CustomNameAttribute>();
        var propertyName = customNameAttribute?.Name ?? pr.Name;

        sb.Append(propertyName + ":");
        if (pr.PropertyType == typeof(char[]))
        {
            sb.Append(new string(temp as char[]) + " |");
        }
        else
        {
            sb.Append(temp + " |");
        }
    }
    return sb.ToString();
}

static object StringToObject(string s)
{
    var str = s.Split("|");
    /*   foreach (var spl in str)
        {
            Console.WriteLine(spl);
        }*/
    var classInformationString = str[0].Split(",");
    /*    foreach (var spl in classInformationString)
        {
            Console.WriteLine(spl);
        }*/
    var targetObjectHandler = Activator.CreateInstance(null, classInformationString[0]);
    var targetObject = targetObjectHandler?.Unwrap();
    var targetType = targetObject?.GetType();

    for (int i = 1; i < str.Length - 1; i++)
    {
        var propertyPair = str[i].Split(":", StringSplitOptions.RemoveEmptyEntries);
        var propertyName = propertyPair[0].Trim();
        var propertyValue = propertyPair[1].Trim();
        //Console.WriteLine(propertyPair[0] + " " + propertyPair[1]);
        var propertyInfo = targetType.GetProperties().FirstOrDefault(p => (p.GetCustomAttribute<CustomNameAttribute>()?.Name ?? p.Name) == propertyName);
        /*        Console.WriteLine(propertyInfo.PropertyType);
                Console.WriteLine(typeof(int));*/
        switch (propertyInfo?.PropertyType)
        {
            case var t when t == typeof(int):
                propertyInfo.SetValue(targetObject, int.Parse(propertyValue));
                break;
            case var t when t == typeof(string):
                propertyInfo.SetValue(targetObject, propertyValue);
                break;
            case var t when t == typeof(decimal):
                propertyInfo.SetValue(targetObject, decimal.Parse(propertyValue));
                break;
            case var t when t == typeof(char[]):
                propertyInfo.SetValue(targetObject, propertyValue.ToCharArray());
                break;


            default:
                break;

        }
    }


    PropertyInfo firstProperty = targetType.GetProperty("I", BindingFlags.Instance | BindingFlags.Public);
    return targetObject;
}