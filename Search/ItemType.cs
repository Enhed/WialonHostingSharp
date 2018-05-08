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
        avl_resource,

        /// <summary>
        /// ретранслятор
        /// </summary>
        avl_retranslator,

        /// <summary>
        /// объект
        /// </summary>
        avl_unit,

        /// <summary>
        /// группа объектов
        /// </summary>
        avl_unit_group,

        /// <summary>
        /// пользователь
        /// </summary>
        user,

        /// <summary>
        /// маршрут
        /// </summary>
        avl_route
    }
}