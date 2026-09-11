# VR Training — тренажёр прохождения сценария

Unity/URP-тренажёр: игрок проходит сценарий, состоящий из групп шагов
(например «Проверка документов»). Каждый шаг — одно ожидаемое действие:
дойти до точки, взять предмет, кликнуть по объекту или нажать кнопку.
Порядок и цель каждого действия проверяются автоматически; по завершении
показывается экран результатов с кнопками «Попытаться ещё» и «Возврат в
Лобби».

## Управление (тестирование мышью/клавиатурой, без VR-шлема)

- **WASD** — ходьба
- **ПКМ (удерживать)** — осмотреться (это стандартное поведение XR Device
  Simulator: сам по себе курсор камеру не поворачивает)
- **ЛКМ** — навести прицел (белая точка в центре экрана) на предмет/кнопку
  и нажать, чтобы активировать его

Это единственный официальный способ взаимодействия в проекте — реализован
в `CrosshairInteractor` (пускает луч из центра камеры по нажатию ЛКМ).
Специально не используются собственные события выбора XR Interaction
Toolkit (`XRGrabInteractable.selectEntered`) и `IPointerClickHandler`,
чтобы не было двух независимых систем, реагирующих на один и тот же клик.

## Архитектура

Событийная архитектура без DI-контейнеров:

- `VRTraining.Core` — чистые данные сценария (никакого Unity API), грузятся
  из JSON через `JsonUtility` (`ScenarioDefinitionAsset`,
  `Assets/Resources/example_scenario.json`). **Важно:** `expectedAction` в
  JSON хранится как число (0=MoveToPoint, 1=GrabObject, 2=ClickObject,
  3=PressUIButton) — `JsonUtility` не умеет надёжно парсить текстовые имена
  enum-значений.
- Объекты на сцене — независимые друг от друга наследники
  `PlayerActionSource` (точка интереса, предмет, кликабельный объект,
  UI-кнопка). Они ничего не знают о сценарии — просто публикуют факт
  действия в статическую шину `ScenarioEvents`.
- `ScenarioController` — конечный автомат: сверяет действие с ожидаемым
  шагом через `ActionValidator`, решает переходы между шагами/группами.
  `ActionValidator` сверяет действие со **всем сценарием**, а не только с
  текущей группой: если игрок тронул что-то из ещё не наступившего шага
  (хоть из текущей группы, хоть из следующей) — это ошибка, шаг **не
  завершается и не пропускается**, только показывается предупреждение, игра
  ждёт нужное действие. Действие, относящееся к уже пройденному шагу
  (например, игрок снова зашёл в уже проверенную зону), тихо игнорируется,
  а не считается ошибкой.
- На события `ScenarioEvents` независимо друг от друга подписаны UI
  (`StepGroupInfoPanel`, `ScenarioResultsPanel`, `WrongTargetNotifier`),
  звук (`AudioFeedbackController`) и подсветка (`StepHighlightController`
  через абстракцию `IHighlightable`). Любую из этих систем можно заменить
  или отключить, не трогая остальные.

## Структура скриптов

```
Assets/Scripts/
  Core/            — чистые данные сценария
  Events/          — ScenarioEvents, событийная шина
  Actions/         — источники действий игрока (включая CrosshairInteractor)
  Scenario/        — ActionValidator, ScenarioController, ScenarioDefinitionAsset
  Highlighting/     — IHighlightable, OutlineAdapter, StepHighlightController
  Audio/           — AudioFeedbackController
  UI/              — LobbyUIController, StepGroupInfoPanel, ScenarioResultsPanel, WrongTargetNotifier
  SceneManagement/ — SceneLoader
  Debugging/       — KeyboardTestLocomotion (вспомогательная ходьба для тестов)
Assets/Resources/example_scenario.json — сценарий: 3 группы по 3 шага
Assets/Scenes/Lobby.unity, Training.unity
```

## Как изменить сценарий

Отредактировать `Assets/Resources/example_scenario.json`: каждый шаг —
`description` (текст для игрока), `expectedAction` (число, см. выше) и
`target` (строка-идентификатор, должен совпадать с полем `targetId` на
нужном объекте сцены).

## Открыть проект на другом компьютере

Обе сцены (`Lobby`, `Training`) уже добавлены в Build Settings — сборка и
переход между сценами работают "из коробки". Чтобы открыть проект у себя:
Unity Hub → Add project (указать папку с `Assets`) → версия редактора
**6000.3.10f1** (или новее в той же основной ветке) → Unity сам подтянет
пакеты из `Packages/manifest.json` (URP, XR Interaction Toolkit, Input
System, OpenXR) при первом открытии — это может занять несколько минут.

## Известное предупреждение в консоли

`Failed to get haptic capabilities of XRSimulatedController..., error code
-1` — безобидное сообщение от XR Device Simulator (он не умеет отдавать
характеристики вибрации виртуального контроллера). На геймплей не влияет и
не связано с кодом проекта; на реальном VR-шлеме не появляется.
