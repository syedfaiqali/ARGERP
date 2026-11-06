using Organization.Service.Entities;

namespace Organization.Service.Infrastructure.Data;

internal sealed record OrganizationAuditColumn(string Name, string SqlType, int Length, bool Nullable);
internal sealed record OrganizationAuditTable(Type Type, string Table, OrganizationAuditColumn[] Columns, int? DocType, int? SubType, bool Filter);

// Column types and document classifications verified against existing database metadata.
internal sealed record OrganizationAuditSqlTable(string Table, OrganizationAuditColumn[] Columns, int DocType, int SubType);

internal static class OrganizationAuditSchema
{
    private static readonly OrganizationAuditColumn[] Columns0 = [
        new("utaddappversion", "varchar", 10, true),
        new("utadddbversion", "varchar", 10, true),
        new("utaddid", "bigint", 8, true),
        new("utaddip", "varchar", 25, true),
        new("utaddsessionid", "int", 4, true),
        new("utaddsystem", "varchar", 25, true),
        new("utaddtime", "datetime", 8, true),
        new("utadduser", "varchar", 25, true),
        new("utblock", "bit", 1, false),
        new("utdoctype", "int", 4, true),
        new("uteditappversion", "varchar", 10, true),
        new("uteditdbversion", "varchar", 10, true),
        new("uteditid", "bigint", 8, true),
        new("uteditip", "varchar", 25, true),
        new("uteditsessionid", "int", 4, true),
        new("uteditsystem", "varchar", 25, true),
        new("utedittime", "datetime", 8, true),
        new("utedituser", "varchar", 25, true),
        new("utispartial", "bit", 1, false),
        new("utrecordversion", "int", 4, false),
        new("utsubdoctype", "int", 4, true),
        new("utsysgen", "bit", 1, false)
    ];

    private static readonly OrganizationAuditColumn[] Columns1 = [
        new("utblock", "bit", 1, false)
    ];

    private static readonly OrganizationAuditColumn[] Columns2 = [
        new("utaddappversion", "nvarchar", 200, true),
        new("utadddbversion", "nvarchar", 200, true),
        new("utaddid", "bigint", 8, true),
        new("utaddip", "nvarchar", 200, true),
        new("utaddsessionid", "int", 4, true),
        new("utaddsystem", "nvarchar", 200, true),
        new("utaddtime", "datetime", 8, true),
        new("utadduser", "nvarchar", 200, true),
        new("utblock", "bit", 1, false),
        new("utdoctype", "int", 4, true),
        new("uteditappversion", "nvarchar", 100, true),
        new("uteditdbversion", "nvarchar", 100, true),
        new("uteditid", "bigint", 8, true),
        new("uteditip", "nvarchar", 100, true),
        new("uteditsessionid", "int", 4, true),
        new("uteditsystem", "nvarchar", 100, true),
        new("utedittime", "datetime", 8, true),
        new("utedituser", "nvarchar", 100, true),
        new("utispartial", "bit", 1, false),
        new("utrecordversion", "int", 4, false),
        new("utsubdoctype", "int", 4, true),
        new("utsysgen", "bit", 1, false)
    ];

    internal static readonly OrganizationAuditTable[] Tables = [
        new(typeof(Country), "Country", Columns0, 1000000731, 10002488, true),
        new(typeof(State), "State", Columns0, 1000000731, 10002490, true),
        new(typeof(City), "City", Columns0, 1000000731, 10002491, true),
        new(typeof(OrgRegion), "OrgRegion", Columns0, 1000000731, 10002489, true),
        new(typeof(OrgSite), "Orgsite", Columns0, 1000000731, 10002492, true),
        new(typeof(OrgRoomLegacy), "Orgroom", Columns0, 1000000772, 1000000882, true),
        new(typeof(OrgBuildingLegacy), "OrgBuilding", Columns0, 1000000772, 1000000880, true),
        new(typeof(OrgFloorLegacy), "OrgFloor", Columns0, 1000000772, 1000000881, true),
        new(typeof(OrgDepartmentLegacy), "OrgDepartment", Columns0, 1000000732, 10002495, true),
        new(typeof(OrgDesignationLegacy), "OrgDesignation", Columns0, 1000000732, 10002496, true),
        new(typeof(OrgDonorLegacy), "OrgDonor", Columns0, null, null, true),
        new(typeof(OccupationLegacy), "Occupation", Columns0, 1000000732, 10002498, true),
        new(typeof(AcademicRegisterLegacy), "SMAcademicregister", Columns0, 1000000732, 10002493, true),
        new(typeof(AcademicYearLegacy), "SMAcademicYear", Columns0, 1000000732, 10002494, true),
        new(typeof(AcademicYearDtlLegacy), "SMAcademicYeardtl", Columns0, 1000000732, 10002494, true),
        new(typeof(SMAdditionalregistrationLegacy), "SMAdditionalregistration", Columns0, 1000000732, 1000000724, true),
        new(typeof(SMAdditionalinfoconfigLegacy), "SMAdditionalinfoconfig", Columns1, 1000000732, 1000001074, true),
        new(typeof(OrgLegalentityLegacy), "OrgLegalentity", Columns0, 1000000732, 1000000879, true),
        new(typeof(ORGentitycontactLegacy), "ORGentitycontact", Columns0, 1000000732, 1000000879, true),
        new(typeof(OrgLegalentitydtlLegacy), "OrgLegalentitydtl", Columns2, 1000000732, 1000000879, true),
        new(typeof(OrgLegalentitypvdtlLegacy), "OrgLegalentitypvdtl", Columns0, 1000000732, 1000000879, true),
        new(typeof(PublishDocumentHdrLegacy), "SMPublishdocumenthdr", Columns0, 1000000727, 10002544, true),
        new(typeof(PublishDocumentDtlLegacy), "SMPublishdocumentdtl", Columns0, 1000000727, 10002544, true),
        new(typeof(GNImageLegacy), "GNImage", Columns0, null, null, true),
        new(typeof(FOCalllogLegacy), "FOCalllog", Columns0, 1000000734, 10002687, true)
    ];

    internal static readonly OrganizationAuditSqlTable[] ImportTables = [
        new("EREmployeeinfohdr", Columns0, 1000000749, 1000000889),
        new("OrgDepartment", Columns0, 1000000732, 10002495),
        new("OrgDesignation", Columns0, 1000000732, 10002496),
        new("OrgLegalentity", Columns0, 1000000732, 1000000879),
        new("SMCourse", Columns0, 1000000719, 445),
        new("SMSection", Columns0, 1000000719, 1000000875),
        new("SMSubjecttype", Columns0, 1000000720, 10002502),
        new("SMSubject", Columns0, 1000000720, 10002504),
        new("SMBook", Columns0, 1000000722, 10002515),
        new("SMStudentinfo", Columns0, 1000000740, 10002562),
        new("SMFeechallanhdr", Columns0, 1000000743, 10002572),
        new("SMFeechallandtl", Columns0, 1000000743, 10002572)
    ];
}
