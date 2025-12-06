namespace SRP.Model.Helper.Helpers
{
    public static class Constants
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 15;
        public const int MaxPageSize = 100;
        public const int MinPageNumber = 1;

        public const int MinPasswordLength = 6;
        public const int MaxPasswordLength = 50;

        public const int JwtExpirationMinutes = 60;
        public const int RefreshTokenExpirationDays = 7;

        public static class Roles
        {
            public const string Admin = "Admin";
            public const string HOD = "HOD";
            public const string Teacher = "Teacher";
            public const string Student = "Student";
        }

        public static class ExamTypes
        {
            public const string Midterm = "Midterm";
            public const string Final = "Final";
            public const string Quiz = "Quiz";
            public const string Assignment = "Assignment";
        }

        public static class StudentSubjectStatus
        {
            public const string Enrolled = "Enrolled";
            public const string Completed = "Completed";
            public const string Dropped = "Dropped";
        }

        public static class ErrorMessages
        {
            public const string Unauthorized = "Unauthorized access";
            public const string NotFound = "Resource not found";
            public const string ValidationFailed = "Validation failed";
            public const string InternalError = "An internal error occurred";
            public const string InvalidCredentials = "Invalid username or password";
            public const string UserNotFound = "User not found";
            public const string UserAlreadyExists = "User already exists";
            public const string DepartmentNotFound = "Department not found";
            public const string TeacherNotFound = "Teacher not found";
            public const string StudentNotFound = "Student not found";
            public const string SubjectNotFound = "Subject not found";
        }

        public static class Grades
        {
            public const string APlus = "A+";
            public const string A = "A";
            public const string BPlus = "B+";
            public const string B = "B";
            public const string CPlus = "C+";
            public const string C = "C";
            public const string D = "D";
            public const string F = "F";
        }
    }
}