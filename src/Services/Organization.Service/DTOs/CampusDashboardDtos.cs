namespace Organization.Service.DTOs
{
    public sealed class CampusDashboardDto
    {
        public int? SelectedEntityId { get; set; }
        public int Duration { get; set; }
        public List<CampusLookupDto> Entities { get; set; } = new();
        public List<int> Durations { get; set; } = new();
        public CampusSummaryDto Summary { get; set; } = new();
        public List<CampusEnrollmentPointDto> Enrollment { get; set; } = new();
        public List<CampusAdmissionPointDto> Admission { get; set; } = new();
        public List<CampusTypeStatusDto> CampusTypes { get; set; } = new();
        public List<CampusRegionCourseDto> CourseWise { get; set; } = new();
        public List<CampusDashboardBreakdownDto> CampusBreakdown { get; set; } = new();
        public CampusCapacityDto Capacity { get; set; } = new();
        public ClientDashboardAttendanceDto Attendance { get; set; } = new();
        public ClientDashboardFinanceDto Finance { get; set; } = new();
    }

    public sealed class CampusLookupDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public sealed class CampusSummaryDto
    {
        public int TotalCampus { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeacher { get; set; }
        public int TotalParent { get; set; }
        public int TotalCity { get; set; }
        public int Operational { get; set; }
        public int Establish { get; set; }
        public int Suspended { get; set; }
        public int TotalStudentBoys { get; set; }
        public int TotalStudentGirls { get; set; }
    }

    public sealed class CampusEnrollmentPointDto
    {
        public string Name { get; set; } = string.Empty;
        public int Enrolled { get; set; }
        public int Withdrawl { get; set; }
        public int Passout { get; set; }
    }

    public sealed class CampusAdmissionPointDto
    {
        public string Name { get; set; } = string.Empty;
        public int Enquiry { get; set; }
        public int Admission { get; set; }
    }

    public sealed class CampusTypeStatusDto
    {
        public string Type { get; set; } = string.Empty;
        public int Operational { get; set; }
        public int UnderEstablish { get; set; }
        public int Suspend { get; set; }
    }

    public sealed class CampusRegionCourseDto
    {
        public int? RegionId { get; set; }
        public string Region { get; set; } = string.Empty;
        public int TotalEnrolled { get; set; }
        public int TotalWithdrawl { get; set; }
        public List<CampusCourseSummaryDto> Courses { get; set; } = new();
    }

    public sealed class CampusCourseSummaryDto
    {
        public int? CourseId { get; set; }
        public string Course { get; set; } = string.Empty;
        public int Enrolled { get; set; }
        public int Withdrawl { get; set; }
    }

    public sealed class CampusDashboardBreakdownDto
    {
        public int EntityId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Enquiries { get; set; }
        public int Registrations { get; set; }
        public int Enrolled { get; set; }
        public int ActiveLearners { get; set; }
        public int Classrooms { get; set; }
        public int SeatingCapacity { get; set; }
    }

    public sealed class CampusCapacityDto
    {
        public int TotalClassrooms { get; set; }
        public int SeatingCapacity { get; set; }
        public int ActiveLearners { get; set; }
        public int AvailableSeats => Math.Max(0, SeatingCapacity - ActiveLearners);
    }

    public sealed class ClientDashboardAttendanceDto
    {
        public int StudentsMarked { get; set; }
        public int StudentPresent { get; set; }
        public int StudentNotMarked { get; set; }
        public int StaffPresent { get; set; }
        public int StaffAbsent { get; set; }
    }

    public sealed class ClientDashboardFinanceDto
    {
        public double Collection { get; set; }
        public double TodayCollection { get; set; }
        public double Outstanding { get; set; }
        public double Fine { get; set; }
        public double Income { get; set; }
        public double Expense { get; set; }
        public double NetIncome => Income - Expense;
    }
}
