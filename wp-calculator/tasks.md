# План задач: Визуальный калькулятор WP

**Feature**: [spec.md](wp-calculator/spec.md)
**Plan**: [plan.md](wp-calculator/plan.md)

## Фаза 1: Настройка и основы (Setup)

- **T001**: [Setup] **[РУЧНАЯ ЗАДАЧА]** Создайте в Visual Studio решение с тремя проектами: `ClassLibraryWPCalculator`, `WpfAppWPCalculator`, `TestProjectWPCalculator` (.NET Framework 4.7.2).
- **T002**: [Setup] **[РУЧНАЯ ЗАДАЧА]** Настройте ссылки между проектами: `WpfAppWPCalculator` должен ссылаться на `ClassLibraryWPCalculator`, а `TestProjectWPCalculator` — на `ClassLibraryWPCalculator`.
- **T003**: [Setup] Определить базовые классы и интерфейсы в `ClassLibraryWPCalculator`, следуя плану (`IWPCalculatorService.cs`, `WPCalculatorService.cs`, `WPCalculator.cs`, `ExpressionParser.cs`). Файлы могут быть пока пустыми или с базовой структурой.

## Фаза 2: Реализация ядра логики (Core Logic)

### User Story 1: Вычисление для присваивания (UC-1)

- **T004**: [US1] [Реализация] В `ClassLibraryWPCalculator/WPCalculator.cs` реализовать логику вычисления `wp` для оператора присваивания.
- **T005**: [US1] [Парсер] В `ClassLibraryWPCalculator/ExpressionParser.cs` реализовать базовый парсинг для простого присваивания и постусловия. Можно использовать простую замену строк на этом этапе.
- **T006**: [US1] [Тест] Написать юнит-тест в `TestProjectWPCalculator/WPCalculatorTests.cs` для проверки `wp(x := e, R)` с использованием парсера. Пример: `wp(x := 2*x + 5, x > 15)` должно давать `2*x + 5 > 15`.

### User Story 2: Вычисление для последовательности (UC-2)

- **T007**: [US2] [Тест] Написать юнит-тест в `TestProjectWPCalculator/WPCalculatorTests.cs` для `wp(S1; S2, R)`. Пример: `wp(y := x + 1; x := y * 2, x > 10)`.
- **T008**: [US2] [Реализация] В `ClassLibraryWPCalculator/WPCalculator.cs` реализовать рекурсивную логику для последовательности операторов: `wp(S1, wp(S2, R))`.

### User Story 3: Вычисление для ветвления (UC-3)

- **T009**: [US3] [Тест] Написать юнит-тест в `TestProjectWPCalculator/WPCalculatorTests.cs` для `wp(if B then S1 else S2, R)`.
- **T010**: [US3] [Реализация] В `ClassLibraryWPCalculator/WPCalculator.cs` реализовать логику для ветвления, включая дизъюнкцию результатов обеих веток.

## Фаза 3: Интеграция и сервисный слой (Integration)

- **T011**: [Интеграция] [P] В `ClassLibraryWPCalculator/WPCalculatorService.cs` реализовать метод `CalculateWeakestPrecondition`, который вызывает `ExpressionParser` и `WPCalculator` для выполнения полного цикла вычислений.
- **T012**: [Интеграция] [P] Настроить Dependency Injection (если планируется) или прямое создание экземпляра `WPCalculatorService` в `WpfAppWPCalculator`.
- **T013**: [Интеграция] В `WpfAppWPCalculator/MainViewModel.cs` добавить команду, которая принимает строки из UI, вызывает метод сервиса `IWPCalculatorService.CalculateWeakestPrecondition` и сохраняет результат для отображения.

## Фаза 4: Пользовательский интерфейс (UI) - Низкий приоритет

- **T014**: [UI] [P] Создать базовую разметку в `MainWindow.xaml` с текстовыми полями для ввода программы и постусловия, кнопкой "Вычислить" и текстовым блоком для вывода результата.
- **T015**: [UI] [P] Привязать элементы управления в `MainWindow.xaml` к свойствам и командам в `MainViewModel.cs`.

## Зависимости и порядок выполнения

1.  **Фаза 1 (T001-T003)** должна быть выполнена первой.
2.  **Фаза 2 (T004-T010)** может выполняться по историям. Задачи внутри каждой истории должны выполняться в порядке: Тест -> Реализация. Истории (US1, US2, US3) можно реализовывать параллельно, если над ними работают разные люди, но логичнее последовательно.
3.  **Фаза 3 (T011-T013)** выполняется после завершения основной логики в Фазе 2.
4.  **Фаза 4 (T014-T015)** может выполняться параллельно с Фазой 3, но не может быть завершена до готовности `MainViewModel`.

Задачи с маркером **[P]** (parallel) могут выполняться одновременно с другими задачами с таким же маркером в той же фазе.
- **T016**: [UI] [P] После успешного вычисления предусловия, отобразить итоговую триаду Хоара `{P} S {R}` в отдельном текстовом блоке.