using System.Collections.Generic;

namespace EDO_FOMS.Shared.Constants.Permission
{
    public static class Permissions
    {
        public static class DocumentTypes
        {
            public const string View = "Permissions.DocumentTypes.View";
            public const string Edit = "Permissions.DocumentTypes.Edit";
            public const string Create = "Permissions.DocumentTypes.Create";
            public const string Delete = "Permissions.DocumentTypes.Delete";
            public const string Export = "Permissions.DocumentTypes.Export";
            public const string Search = "Permissions.DocumentTypes.Search";
        }
        public static class Roles
        {
            public const string View = "Permissions.Roles.View";
            public const string Edit = "Permissions.Roles.Edit";
            public const string Create = "Permissions.Roles.Create";
            public const string Delete = "Permissions.Roles.Delete";
            //public const string Export = "Permissions.Roles.Export";
            //public const string Search = "Permissions.Roles.Search";
        }
        public static class RoleClaims
        {
            public const string View = "Permissions.RoleClaims.View";
            public const string Edit = "Permissions.RoleClaims.Edit";
            public const string Create = "Permissions.RoleClaims.Create";
            public const string Delete = "Permissions.RoleClaims.Delete";
            //public const string Export = "Permissions.RoleClaims.Export";
            //public const string Search = "Permissions.RoleClaims.Search";
        }
        public static class Documents
        {
            public const string View = "Permissions.Documents.View";
            public const string Edit = "Permissions.Documents.Edit";
            public const string Signing = "Permissions.Documents.Signing";
            public const string Approving = "Permissions.Documents.Approving";

            public const string Create = "Permissions.Documents.Create";
            public const string Delete = "Permissions.Documents.Delete";
            public const string Export = "Permissions.Documents.Export";
            public const string Search = "Permissions.Documents.Search";
        }
        public static class SelfOrg
        {
            public const string Edit = "Permissions.SelfOrg.Edit";
            public const string View = "Permissions.SelfOrg.View";
        }
        //public static class Acts
        //{
        //    public const string View = "Permissions.Acts.View";
        //    public const string Edit = "Permissions.Acts.Edit";
        //}
        public static class System
        {
            public const string View = "Permissions.System.View";
            public const string Edit = "Permissions.System.Edit";
            public const string Create = "Permissions.System.Create";

            public const string Delete = "Permissions.System.Delete";
            public const string Export = "Permissions.System.Export";
            public const string Search = "Permissions.System.Search";
        }
        public static class Management
        {
            public const string Users = "Permissions.Management.Users";
            public const string Employees = "Permissions.Management.Employees";
            public const string Managers = "Permissions.Management.Managers";
            public const string Chiefs = "Permissions.Management.Chiefs";
            public const string Admins = "Permissions.Management.Admins";
        }
        public static class WorkerOf
        {
            public const string Fund = "Permissions.WorkerOf.Fund";
            public const string SMO = "Permissions.WorkerOf.SMO";
            public const string MO = "Permissions.WorkerOf.MO";
        }
        public static class UserRoles
        {
            public const string User = "User";
            public const string Employee = "Employee";
            public const string Manager = "Manager";
            public const string Chief = "Chief";
            public const string Admin = "Admin";
        }

        /// <summary>
        /// Returns a list of User Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetUserPermissions()
        {
            return new()
            {
                SelfOrg.View,
                Documents.View,
                Documents.Approving
            };
        }

        /// <summary>
        /// Returns a list of Employee Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEmployeePermissions()
        {
            return new()
            {
                SelfOrg.View,
                Documents.View,
                Documents.Edit,
                Documents.Approving,
                Documents.Search
            };
        }

        /// <summary>
        /// Returns a list of Manager Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetManagerPermissions()
        {
            return new()
            {
                SelfOrg.Edit,
                SelfOrg.View,
                Documents.View,
                Documents.Edit,
                Documents.Signing,
                Documents.Approving,
                Documents.Search,
                Management.Users,
                Management.Employees,
                Management.Managers
            };
        }

        /// <summary>
        /// Returns a list of Chief Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetChiefPermissions()
        {
            return new()
            {
                SelfOrg.Edit,
                SelfOrg.View,
                Documents.View,
                Documents.Edit,
                Documents.Signing,
                Documents.Approving,
                Documents.Search,
                Documents.Export,
                Management.Users,
                Management.Employees,
                Management.Managers,
                Management.Chiefs
            };
        }

        /// <summary>
        /// Returns a list of Admin Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAdminPermissions()
        {
            return new()
            {
                SelfOrg.Edit,
                SelfOrg.View,
                Documents.Create,
                Documents.Edit,
                Documents.View,
                Documents.Signing,
                Documents.Approving,
                Documents.Search,
                Documents.Export,
                System.View,
                System.Edit,
                System.Create,
                System.Delete,
                System.Export,
                System.Search
            };
        }

        /// <summary>
        /// Returns a list of Fund Worker Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFundWorkerPermissions()
        {
            return new()
            {
                WorkerOf.Fund,
                Documents.Create,
                Documents.Edit
            };
        }

        /// <summary>
        /// Returns a list of SMO Worker Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSmoWorkerPermissions()
        {
            return new()
            {
                WorkerOf.SMO
            };
        }

        /// <summary>
        /// Returns a list of MO Worker Permissions.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetMoWorkerPermissions()
        {
            return new()
            {
                WorkerOf.MO
            };
        }

        ///// <summary>
        ///// Returns a list of Permissions.
        ///// </summary>
        ///// <returns></returns>
        //public static List<string> GetRegisteredPermissions()
        //{
        //    var permssions = new List<string>();

        //    foreach (var prop in typeof(Permissions).GetNestedTypes().SelectMany(c => c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
        //    {
        //        var propertyValue = prop.GetValue(null);
        //        if (propertyValue is not null)
        //            permssions.Add(propertyValue.ToString());
        //    }

        //    return permssions;
        //}
    }
}