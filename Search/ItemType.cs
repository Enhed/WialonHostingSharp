using System.Runtime.Serialization;

namespace WialonHostingSharp.Search
{
    /// <summary>
    /// Типы элементов системы Wialon (указываются в поле «itemsType»)
    /// </summary>
    public enum ItemType
    {
        /// <summary>
        /// ресурс
        /// </summary>
        [EnumMember(Value = "avl_resource")]
        Resource,

        /// <summary>
        /// ретранслятор
        /// </summary>
        [EnumMember(Value = "avl_retranslator")]
        Retranslator,

        /// <summary>
        /// объект
        /// </summary>
        [EnumMember(Value = "avl_unit")]
        Object,

        /// <summary>
        /// группа объектов
        /// </summary>
        [EnumMember(Value = "avl_unit_group")]
        GroupObjects,

        /// <summary>
        /// пользователь
        /// </summary>
        [EnumMember(Value = "user")]
        User,

        /// <summary>
        /// маршрут
        /// </summary>
        [EnumMember(Value = "avl_route")]
        Route
    }
}