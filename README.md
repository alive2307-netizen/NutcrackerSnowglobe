# Щелкунчик: Снежный шар

Учебный 3D-раннер по сфере. Unity 6000.0.29f1, Built-in Render Pipeline, Input System.
В репозитории этапы 0-4 плана: сцена с планетой, гравитация, бег и прыжок, сугробы, лёд, ёлки.

## Запуск

1. Unity Hub: Add -> Add project from disk, выбрать папку проекта, редактор 6000.0.29f1.
2. Дождаться импорта. Сцена `Assets/Scenes/SnowballLevel.unity` создаётся автоматически
   (Console: `[Щелкунчик] Сцена создана`). Если Unity показал окно про Active Input Handling,
   перезапустить редактор.
3. Нажать Play. Если Hierarchy пуста, выбрать меню Nutcracker -> Build Level Scene (Stages 1-4).

## Управление

| Платформа | Поворот | Скорость | Прыжок |
|---|---|---|---|
| ПК | A/D или стрелки | W/S или стрелки | Space |
| Телефон | экранный стик | экранный стик | кнопка ПРЫЖОК |
| Телефон | наклон устройства (гироскоп) | - | кнопка |

## Что на сцене

| Объект | Назначение |
|---|---|
| SnowballPlanet | сфера, Scale 60, слой Planet |
| Player | капсула, Rigidbody, PlanetGravity, PlayerMovement |
| Main Camera | CameraFollow |
| InputReader | клавиатура, стик и гироскоп в одном Vector2 |
| GameManager | реакция на гибель игрока |
| Obstacles | сугробы (SlowZone), лёд (SpeedBoost), ёлки (TreeCollision) |
| Canvas, EventSystem | стик, кнопка прыжка, подсказка, показ скорости |

Префабы: `Assets/Prefabs/Snowdrift`, `IcePatch`, `Tree`.

## Правила этапов 0-4

- Сугроб: скорость x0.5 на 5 с, его можно перепрыгнуть.
- Лёд: скорость x1.6 на 10 с.
- Ёлка: столкновение завершает игру, сцена перезапускается через 1.5 с.
