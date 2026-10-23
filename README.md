# Hello Tap

Первая мини-игра для Android на Unity и C#: по нажатию кнопки увеличивается счётчик.
Практическая работа № 1 по дисциплине «Программирование мобильных игр».

Дундуков Никита ТРИ43 

## Описание

На экране заголовок «Hello Tap», счёт и кнопка «Тапни меня». Каждое нажатие
увеличивает счёт на 1.

## Технологии

- Unity (2022.3 LTS / Unity 6, укажите свою версию)
- C#, Unity UI (Canvas, Canvas Scaler, Button, Text)
- Целевая платформа: Android

## Структура

| Файл | Назначение |
|---|---|
| `Assets/Scripts/TapCounter.cs` | Счётчик нажатий и вывод на экран |
| `Assets/Editor/HelloTapSetup.cs` | Автосборка сцены (только в редакторе) |
| `Assets/Scenes/Main.unity` | Сцена |

## Как запустить

1. Откройте проект в Unity Hub.
2. Откройте сцену `Assets/Scenes/Main.unity` и нажмите Play.

## Как собрать APK

File → Build Settings → Android → Switch Platform → Add Open Scenes →
подключить телефон (USB-отладка) → Build And Run.

