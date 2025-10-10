# План задач: Визуальный калькулятор WP

**Feature**: [spec.md](wp-calculator/spec.md)
**Plan**: [plan.md](wp-calculator/plan.md)

## Фаза 1: Настройка и основы (Setup)

- **T001**: [Setup] **[РУЧНАЯ ЗАДАЧА]** Убедиться, что решение содержит проекты `ClassLibraryWPCalculator`, `WpfAppWPCalculator`, `TestProjectWPCalculator1` и между ними настроены корректные ссылки.
- **T002**: [Setup] **[РУЧНАЯ ЗАДАЧА]** Убедиться, что в `ClassLibraryWPCalculator` добавлен NuGet-пакет для символьных вычислений, если он необходим для `SimplificateInequality.cs`.
- **T003**: [Setup] Проверить наличие базовых файлов и интерфейсов (`IExpressionParser.cs`, `ISimplificateInequality.cs`, `IWPCalculatorService.cs`, `WPCalculator.cs`, `ExpressionParser.cs`, `SimplificateInequality.cs`, `WpTrace.cs`, `WPCalculatorService.cs`).

## Фаза 2: Реализация и рефакторинг ядра логики

### User Story 1 & 3: Присваивание и Ветвление (UC-1, UC-3)

- **T004**: [US1, US3] [Тест] Проверить и при необходимости дописать тесты в `TestProjectWPCalculator1` для `CalculateForAssignment` и `CalculateForIf`, чтобы они соответствовали актуальной логике в `WPCalculator.cs`.
- **T005**: [US1, US3] [Реализация] Убедиться, что методы `CalculateForAssignment` и `CalculateForIf` в `WPCalculator.cs` корректно вызывают `SimplificateInequality` и добавляют шаги в `WpTrace`.

### User Story 2: Последовательность (UC-2)

- **T006**: [US2] [Тест] Раскомментировать и адаптировать тест для `CalculateForSequence` в `TestProjectWPCalculator1`, используя `Stack<string>` в качестве входных данных.
- **T007**: [US2] [Реализация] Проверить и финализировать метод `CalculateForSequence(Stack<string> assignments, ...)` в `WPCalculator.cs`.

### Новые требования

- **T008**: [FR-9] [Валидация] Убедиться, что `ExpressionParser.cs` покрывает все необходимые сценарии валидации для `if` и `assignment`, как описано в `IExpressionParser.cs`.
- **T009**: [FR-8] [Упрощение] Проверить, что `SimplificateInequality.cs` корректно обрабатывает основные случаи и выбрасывает исключения, как описано в `ISimplificateInequality.cs`.
- **T010**: [FR-10] [Трассировка] Убедиться, что все публичные методы вычислений в `WPCalculator.cs` добавляют осмысленные шаги в `WpTrace`.

## Фаза 3: Интеграция и сервисный слой (Integration)

- **T011**: [Интеграция] Реализовать `WPCalculatorService.cs`, чтобы он использовал `WPCalculator`, `ExpressionParser` и `WpTrace` для выполнения полного цикла операций.
- **T012**: [Интеграция] Реализовать `MainViewModel.cs` в `WpfAppWPCalculator` для взаимодействия с `IWPCalculatorService`.

## Фаза 4: Пользовательский интерфейс (UI) - Низкий приоритет

- **T013**: [UI] Создать разметку в `MainWindow.xaml` для ввода данных, вывода результата и отображения лога трассировки из `WpTrace`.