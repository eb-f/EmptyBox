using EmptyBox.Application.Services.Operation;
using EmptyBox.Construction.Machines;

using System.ComponentModel;

namespace EmptyBox.Application.Services.Shared;

/// <summary>
///     Содержит сопутствующие службе <see cref="Teapot{SQ}"/> состояния и конфигурацию. 
/// </summary>
public partial interface ITeapot
{
    /// <summary>
    ///     Конфигурация службы <see cref="Teapot{SQ}"/>
    /// </summary>
    public readonly struct Configuration
    {
        public required double HeatingRate { get; init; }
        public double? BaseTemperature { get; init; }
    }

    /// <summary>
    ///     Контракт состояния "Запущено".
    /// </summary>
    /// <remarks>
    ///     Указание атрибута <see cref="StateAttribute"/> приводит к генерации класса <see cref="Launched"/> - реализации данного контракта.
    /// </remarks>
    [State]
    public interface ILaunched : ISL<Configuration>
    {
        public double Temperature { get; set; }
    }

    /// <summary>
    ///     Состояние "Нагревание".
    /// </summary>
    /// <remarks>
    ///     Пример определения состояния без использования кодогенерации.
    /// </remarks>
    public class Heating : ILaunched, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        
        public double Temperature
        {
            get;
            set
            {
                if (value == field)
                {
                    field = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Temperature)));
                }
            }
        }
        public Configuration Configuration { get; set; }

        void IState.Map<S>(S state)
        {
            if (state is ISC<Configuration> configurable)
            {
                configurable.Configuration = Configuration;
            }    

            if (state is ILaunched launched)
            {
                launched.Temperature = Temperature;
            }
        }
    }

    /// <summary>
    ///     Контракт состояния "Требует обслуживания".
    /// </summary>
    /// <remarks>
    ///     Указание атрибута <see cref="StateAttribute"/> приводит к генерации класса <see cref="RequireMaintenance"/> - реализации данного контракта.
    /// </remarks>
    [State]
    public interface IRequireMaintenance : ILaunched; 
}
