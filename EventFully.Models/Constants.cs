using System;
using System.Collections.Generic;
using System.Text;

namespace EventFully
{
    public static class Constant
    {
        public static class SortDirections
        {
            public const string Ascending = "Ascending";
            public const string Descending = "Descending";
        }

        public static class SecurityRole
        {
            public const int Administrator = 1;
        }

        public static class Permission
        {
            public const int AdministrateEvent = 1;
        }

        public static class RequirementTypes
        {
            public const int Event = 1;
        }
    }
}
