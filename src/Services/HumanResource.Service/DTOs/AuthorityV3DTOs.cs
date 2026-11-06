namespace HumanResource.Service.DTOs
{
    public record AuthorityV3Dto(
        int Id,
        string Code,
        int? AuthorityTypeId,
        string AuthorityType,
        string Authority,
        string Description,
        string ContactNo,
        string Phone,
        string Web,
        string Fax,
        string EmailId,
        bool IsActive);

    public record CreateAuthorityV3Request(
        string Code,
        int? AuthorityTypeId,
        string Authority,
        string Description,
        string ContactNo,
        string Phone,
        string Web,
        string Fax,
        string EmailId,
        bool IsActive);

    public record UpdateAuthorityV3Request(
        int Id,
        string Code,
        int? AuthorityTypeId,
        string Authority,
        string Description,
        string ContactNo,
        string Phone,
        string Web,
        string Fax,
        string EmailId,
        bool IsActive);
}
