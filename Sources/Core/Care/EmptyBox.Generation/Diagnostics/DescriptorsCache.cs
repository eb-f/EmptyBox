using Microsoft.CodeAnalysis;

using System.Collections.Frozen;
using System.Collections.Generic;

namespace EmptyBox.Generation.Diagnostics;

internal static class DescriptorsCache
{
    public static FrozenDictionary<DiagnosticIdentifier, DiagnosticDescriptor> Descriptors { get; }

    static DescriptorsCache()
    {
        Dictionary<DiagnosticIdentifier, DiagnosticDescriptor> descriptors = new()
        {
            {
                DiagnosticIdentifier.EBSG0000,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0000.ToString(),
                                         "Объявление типа не является разделяемым",
                                         "Объявление типа {0} не является разделяемым. Генерация кода для типа {0} и вложенных в него типов будет пропущена.",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0001,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0001.ToString(),
                                         "Множественная реализация интерфейса недопустима",
                                         "Множественная реализация интерфейса {1} недопустима. Генерация прослоек для типа {0} будет пропущена.",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0002,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0002.ToString(),
                                         "Аргумент типа не является параметром типа",
                                         "Аргумент {2} в инсталляции {1} не является параметром типа {0}. Генерация прослоек для типа {0} будет пропущена.",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0003,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0003.ToString(),
                                         "Тип не реализует функционально необходимый интерфейс",
                                         "Тип {0} не реализует интерфейс {1}, требуемый для соответствующей функциональности. Создаваемые методы-прослойки для типа {0} не будут содержать зависимую от {1} функциональность.",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0004,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0004.ToString(),
                                         "Атрибут не имеет действия на статические члены типа",
                                         "Применение атрибута {0} не оказывает никакого влияния на генерацию кода",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0005,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0005.ToString(),
                                         "Недопустимый модификатор защиты члена типа",
                                         "Генерация прослойки доступна только для членов типа с модификатором доступа protected, или private, если тип запечатан",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0006,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0006.ToString(),
                                         "Неподходящий тип возвращаемого значения функции",
                                         "Функция {0} не имеет требуемого для функциональности атрибута {1} типа возвращаемого значения. Создаваемые методы-прослойки для функции {0} не будут содержать зависимую от {1} функциональность.",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0007,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0007.ToString(),
                                         "Атрибут не имеет действия на ссылочноподобные типы и параметры, передающиеся по ссылке",
                                         "Применение атрибута {0} не оказывает никакого влияния на генерацию кода",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            },
            {
                DiagnosticIdentifier.EBSG0008,
                new DiagnosticDescriptor(DiagnosticIdentifier.EBSG0008.ToString(),
                                         "Невозможно разрешить символ квалификации",
                                         "Применение атрибута {0} не оказывает никакого влияния на генерацию кода",
                                         category: string.Empty,
                                         DiagnosticSeverity.Warning,
                                         isEnabledByDefault: true)
            }
        };

        Descriptors = descriptors.ToFrozenDictionary();
    }
}
