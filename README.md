# EmptyBox

EmptyBox - фреймворк для разработки приложений с использованием подхода квалифицированного доступа. Подход позволяет определить для описываемого объекта *квалификатор* и данный квалификатор будет использован для контроля допустимых операций над объектом компилятором языка, на котором ведётся разработка.

## Содержание

Внутри можно найти:

1. Квалификаторы - инфраструктура для идентификации квалификации объекта;
2. Квалифицированная машина состояний - механизм изменения квалификации объекта во время исполнения программы, а так же - декларативно описываемый конечный автомат на основе контрактов;
3. Инструменты кодогенерации - автоматизируют написание однотипного кода, связанного с квалификаторами и машинами состояний.

## Использование

* Пример определения квалифицированной машины состояний находится в проекте [EmptyBox.Application.Services.Shared](Sources/Core/Lare/EmptyBox.Application.Services.Shared/) в файлах `ITeapot.cs` и `Teapot.cs`;
* Пример использования такого объекта находится в файле [`NormalExecution.cs`](Sources/Core/Lare/Tests/EmptyBox.Application.Services.Tests/QualifiedStateMachine/NormalExecution.cs).

### Установка пакетов NuGet

Фреймворк находится в состоянии POC и попадать в ленту реестра NuGet ему рановато, поэтому для установки пакетов потребуются следующие шаги:

1. Авторизация на GitHub;
2. Генерация Personal Access Token [по инструкции](https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/managing-your-personal-access-tokens#creating-a-fine-grained-personal-access-token), необходимо указать в процессе разрешение `read:packages` (можно только его). Токен будет показан лишь единожды, не закрывайте страницу до выполнения 4 пункта;
3. Создать в редакторе проект C#, нацеленный на .NET 9;
4. Создать в проекте файл с названием `NuGet.config` и следующим содержимым (соответственно заменяя `YOUR_GITHUB_USERNAME` и `YOUR_GITHUB_PAT` на ваше имя и сгенерированный в пункте 2 токен):
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <clear/> <!--Удаляет все реестры пакетов из проекта-->
        <add key="NuGet" value="https://api.nuget.org/v3/index.json"/> <!--Возвращает в проект реестр NuGet-->
        <add key="EmptyBox" value="https://nuget.pkg.github.com/eb-f/index.json"/> <!--Добавляет в проект реестр EmptyBox-->
    </packageSources>
    <packageSourceCredentials>
        <EmptyBox>
            <add key="Username" value="YOUR_GITHUB_USERNAME" />
            <add key="ClearTextPassword" value="YOUR_GITHUB_PAT"/>
        </EmptyBox>
    </packageSourceCredentials>
</configuration>
```
5. Модифицировать файл проекта, добавив в него ссылку на пакет `EmptyBox.SDK`. Пример файла:
```xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net9.0</TargetFramework>
        <LangVersion>preview</LangVersion>
        <Nullable>enable</Nullable>
        <ImplicitUsings>true</ImplicitUsings>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="EmptyBox.SDK" Version="0.0.1.1-concept" /> <!--Достаточно добавить данную строку-->
    </ItemGroup>
    
</Project>
```
6. Выполнить команду `dotnet restore` в папке с проектом (потребуется командная строка). Это позволит без перезапуска Visual Studio подобрать параметры из файла `NuGet.config`.

## Участие в разработке

Обратитесь к файлу [`Contribution.md`](Documents/Contribution.md) за подробностями.