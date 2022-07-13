namespace EDO_FOMS.Shared.Constants.Role
{
    public static class RoleConstants
    {
        public const string User = "User";               // Пользователь - доступ на просмотр документов и реквизитов организации
        public const string Employee = "Employee";       // Сотрудник -  доступ на работу с документами и просмотр реквизитов организации
        public const string Manager = "Manager";         // Управляющий - доступ на работу с документами, изменение реквизитов компании и управление сотруниками
        public const string Chief = "Chief";             // Руководитель - доступ на работу с документами, изменение реквизитов компании, управление менеджерами и сотрудниками
        public const string Admin = "Admin";             // Администратор ЭДО 


        public static class WorkerOf
        {
            public const string Fund = "WorkerOf.Fund";
            public const string SMO = "WorkerOf.SMO";
            public const string MO = "WorkerOf.MO";
        }

        public const string DefaultPassword = "Qwerty_01";

        //private static readonly string[] StatusRoles = { };

        //public static IList<string> List
        //{
        //    get
        //    {
        //        return roles;
        //    }
        //}

        public static readonly string[] MainRoles = {
            User, Employee, Manager, Chief, Admin,
            WorkerOf.Fund, WorkerOf.SMO, WorkerOf.MO
        };
    }
}