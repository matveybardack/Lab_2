# План реализации: Визуальный калькулятор WP

**Feature**: [spec.md](wp-calculator/spec.md)

## 1. Технический контекст

- **Язык**: C#
- **Платформа**: .NET Framework 4.7.2
- **UI Framework**: WPF
- **Архитектура**: 4-слойная (UI -> Service Layer -> Business Logic -> Data (in-memory))
  - `WpfAppWPCalculator`: Пользовательский интерфейс.
  - `ClassLibraryWPCalculator`: Сервисный слой и основная бизнес-логика.
  - `TestProjectWPCalculator1`: Модульные тесты для библиотеки классов (xUnit Test Project).
- **Основной фокус**: `ClassLibraryWPCalculator`. Пользовательский интерфейс имеет низкий приоритет.

## 2. Проверка соответствия конституции

*Предполагается, что конституции нет, поэтому этот раздел пропускается.*

## 3. Фаза 0: Исследование (Research)

На данном этапе нет неясностей, требующих исследования. Технологический стек ясен.

## 4. Фаза 1: Дизайн и контракты

### 4.1. Модель данных (`data-model.md`)

Модель данных будет простой и в основном состоять из строковых представлений выражений и предикатов.

- **ProgramStatement**: Абстракция для операторов (`x := e`, `if B then S1 else S2`, `S1; S2`).
- **Predicate**: Представление логических условий (постусловие `R` и промежуточные предусловия).

### 4.2. Структура файлов и классов

- **`ClassLibraryWPCalculator/`**
  - **`Interfaces/`**: Директория с интерфейсами (`IExpressionParser`, `ISimplificateInequality`, `IWPCalculatorService`).
  - **`Services/WPCalculatorService.cs`**: Реализация сервиса `IWPCalculatorService`. Инкапсулирует вызовы к парсеру и калькулятору.
  - **`ExpressionParser.cs`**: Класс, реализующий `IExpressionParser` для валидации выражений.
  - **`WPCalculator.cs`**: Основной класс для вычисления слабейшего предусловия (внутренняя логика).
  - **`SimplificateInequality.cs`**: Класс, реализующий `ISimplificateInequality` для упрощения выражений в соответствии с FR-8.
  - **`GLOBAL_TRACE.cs`**: **[Новое]** Статический класс `WpTrace` для сбора и хранения лога вычислений в соответствии с FR-10.
- **`WpfAppWPCalculator/`**
  - **`MainWindow.xaml` / `MainWindow.xaml.cs`**: Основное и единственное окно приложения.
  - **`MainViewModel.cs`**: ViewModel для связи UI с сервисным слоем (`IWPCalculatorService`).
- **`TestProjectWPCalculator1/`**
  - **`WPCalculatorTests.cs`**: Набор тестов (xUnit), проверяющих корректность вычислений `wp` для всех трех сценариев.

## 5. Задачи (Предварительно)

*Этот раздел будет обновлен командой `/speckit.tasks`.*