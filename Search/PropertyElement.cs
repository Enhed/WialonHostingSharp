namespace WialonHostingSharp
{
    /// <summary>
    /// Свойства элементов (указываются в поле «propName» и «sortType»)
    /// </summary>
    public enum PropertyElement
    {
        /// <summary>
        /// имя элемента
        /// </summary>
        sys_name,

        /// <summary>
        /// ID элемента
        /// </summary>
        sys_id,

        /// <summary>
        /// уникальный ID объекта (IMEI)
        /// </summary>
        sys_unique_id,

        /// <summary>
        /// телефонный номер объекта
        /// </summary>
        sys_phone_number,

        /// <summary>
        /// второй телефонный номер объекта
        /// </summary>
        sys_phone_number2,

        /// <summary>
        /// ID создателя
        /// </summary>
        sys_user_creator,

        /// <summary>
        /// имя создателя
        /// </summary>
        rel_user_creator_name,

        /// <summary>
        /// ID учетной записи
        /// </summary>
        sys_billing_account_guid,

        /// <summary>
        /// имя учётной записи
        /// </summary>
        rel_billing_account_name,

        /// <summary>
        /// имя тарифного плана
        /// </summary>
        rel_billing_plan_name,

        /// <summary>
        /// состояние оборудования (1 - подключено, 0 - отключено)
        /// </summary>
        sys_comm_state,

        /// <summary>
        /// имя оборудования
        /// </summary>
        rel_hw_type_name,

        /// <summary>
        /// ID оборудования
        /// </summary>
        rel_hw_type_id,

        /// <summary>
        /// баланс учётной записи
        /// </summary>
        sys_account_balance,

        /// <summary>
        /// количество дней для учётной записи
        /// </summary>
        sys_account_days,

        /// <summary>
        /// права дилера (1 - выданы, 0 - не выданы)
        /// </summary>
        sys_account_enable_parent,

        /// <summary>
        /// учётная запись блокирована (1 - да, 0 - нет)
        /// </summary>
        sys_account_disabled,

        /// <summary>
        /// время последнего изменения свойства sys_account_disabled, UNIX-time
        /// </summary>
        rel_account_disabled_mod_time,

        /// <summary>
        /// количество объектов в учётной записи
        /// </summary>
        rel_account_units_usage,

        /// <summary>
        /// время последнего сообщения, UNIX-time
        /// </summary>
        rel_last_msg_date,

        /// <summary>
        /// является ли ресурс учётной записью (1 - да, 0 - нет)
        /// </summary>
        rel_is_account,

        /// <summary>
        /// время последнего входа в систему, UNIX-time
        /// </summary>
        login_date,

        /// <summary>
        /// включен ли ретранслятор ( 1 - да, 0 - нет)
        /// </summary>
        retranslator_enabled,

        /// <summary>
        /// дата создания
        /// </summary>
        rel_creation_time,

        /// <summary>
        /// количество объектов в группе
        /// </summary>
        rel_group_unit_count,

        /// <summary>
        /// имя произвольного свойства объекта
        /// </summary>
        rel_customfield_name,

        /// <summary>
        /// значение произвольного свойства объекта
        /// </summary>
        rel_customfield_value,

        /// <summary>
        /// имя характеристики объекта
        /// </summary>
        rel_profilefield_name,

        /// <summary>
        /// значение характеристики объекта
        /// </summary>
        rel_profilefield_value,

        /// <summary>
        /// имя административного свойства
        /// </summary>
        rel_adminfield_name,

        /// <summary>
        /// значение административного свойства
        /// </summary>
        rel_adminfield_value,

        /// <summary>
        /// имя и значение произвольного свойства объекта, через «:»*
        /// </summary>
        rel_customfield_name_value,

        /// <summary>
        /// имя и значение характеристики объекта, через «:»
        /// </summary>
        rel_profilefield_name_value,

        /// <summary>
        /// имя и значение административного свойства, через «:»
        /// </summary>
        rel_adminfield_name_value,
    }
}