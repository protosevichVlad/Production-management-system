using ProductionManagementSystem.Models;

namespace ProductionManagementSystem.UnitTests.ServicesTests
{
    public class TaskServiceDataSource
    {
        public static object[] TestRoles =
        {
            new object[] {new[] {RoleEnum.Admin}, StatusEnum.Assembly | StatusEnum.Customization | StatusEnum.Done | StatusEnum.Equipment | StatusEnum.Equipment | StatusEnum.Montage | StatusEnum.Validate | StatusEnum.Warehouse},
            new object[] {new[] {RoleEnum.Assembler, RoleEnum.Collector}, StatusEnum.Montage | StatusEnum.Assembly},
            new object[] {new[] {RoleEnum.OrderPicker, RoleEnum.Validating}, StatusEnum.Equipment | StatusEnum.Validate},
            new object[] {new[] {RoleEnum.Shipper}, StatusEnum.Warehouse},
            new object[] {new[] {RoleEnum.Tuner}, StatusEnum.Customization}
        };
        
        public static object[] StatusName =
        {
            new object[] { "Комплектация, Монтаж, Настройка, Сборка, Проверка, Задача выполнена", StatusEnum.Assembly | StatusEnum.Customization | StatusEnum.Done | StatusEnum.Equipment | StatusEnum.Equipment | StatusEnum.Montage | StatusEnum.Validate},
            new object[] { "Комплектация", StatusEnum.Equipment},
            new object[] { "Монтаж", StatusEnum.Montage},
            new object[] { "Настройка", StatusEnum.Customization},
            new object[] { "Сборка", StatusEnum.Assembly},
            new object[] { "Проверка", StatusEnum.Validate},
            new object[] { "Склад", StatusEnum.Warehouse},
            new object[] { "Задача выполнена", StatusEnum.Done},
        };
    }
}