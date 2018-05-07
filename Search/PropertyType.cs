namespace WialonHostingSharp
{
    /// <summary>
    /// Типы свойств (указываются в поле «propType»)
    /// </summary>
    public enum PropertyType
    {
        /// <summary>
        /// свойство
        /// </summary>
        property,

        /// <summary>
        /// список
        /// </summary>
        list,

        /// <summary>
        /// имя подэлемента (например геозона является подэлементом ресурса)
        /// </summary>
        propitemname,

        /// <summary>
        /// цепочка создателей (поиск с таким типом вернет список элементов, у которых в цепочке создателей есть создатель, указанный в условии поиска)
        /// </summary>
        creatortree,

        /// <summary>
        /// цепочка учетных записей (поиск с таким типом вернет список элементов, у которых в цепочке учетных записей есть учетная запись, указанная в условии поиска)
        /// </summary>
        accounttree,

        /// <summary>
        /// произвольные поля
        /// </summary>
        customfield,
        
        /// <summary>
        /// характеристики объекта
        /// </summary>
        profilefield,

        /// <summary>
        /// административные записи
        /// </summary>
        adminfield,
    }

}