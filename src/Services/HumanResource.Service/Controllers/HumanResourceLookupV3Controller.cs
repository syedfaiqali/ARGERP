using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/Lookups/v3")]
    [Route("api/HumanResource/Lookups/v3")]
    public sealed class HumanResourceLookupV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public HumanResourceLookupV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet("religions")]
        public Task<ActionResult<List<LookupRow>>> GetReligions(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    id as Id,
                    religioncode as Code,
                    stxt as Name,
                    ltxt as Description,
                    cast(null as int) as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from religion
                order by stxt
                """,
                cancellationToken);

        [HttpGet("departments")]
        public Task<ActionResult<List<LookupRow>>> GetDepartments(
            [FromQuery] int? entityId,
            CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    Id,
                    depcode as Code,
                    depstxt as Name,
                    depltxt as Description,
                    cast(null as int) as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from OrgDepartment
                where (@ClientId is null or clientid = @ClientId)
                  -- Legacy Department lookup is client-scoped.  Prefer rows
                  -- assigned to the selected entity, but do not blank the
                  -- dropdown when this client's legacy departments have no
                  -- opunitid assignment.
                  and (
                      @EntityId is null
                      or opunitid = @EntityId
                      or not exists (
                          select 1
                          from OrgDepartment scoped
                          where (@ClientId is null or scoped.clientid = @ClientId)
                            and scoped.opunitid = @EntityId
                      )
                  )
                order by depstxt
                """,
                cancellationToken,
                clientId: GetClientId(),
                entityId: entityId ?? GetEntityId());

        [HttpGet("designations")]
        public Task<ActionResult<List<LookupRow>>> GetDesignations(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    Id,
                    cast(null as nvarchar(50)) as Code,
                    stxt as Name,
                    cast(null as nvarchar(500)) as Description,
                    cast(null as int) as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from OrgDesignation
                where (@ClientId is null or clientid = @ClientId)
                order by stxt
                """,
                cancellationToken,
                clientId: GetClientId());

        [HttpGet("institutions")]
        public Task<ActionResult<List<LookupRow>>> GetInstitutions(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    id as Id,
                    entityno as Code,
                    coalesce(campusname, stxt, shortname, entityno) as Name,
                    shortname as Description,
                    countryid as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from OrgLegalentity
                  where (@EntityId is null or id = @EntityId)
                order by coalesce(campusname, stxt, shortname, entityno)
                """,
                cancellationToken,
                entityId: GetEntityId());

        [HttpGet("entities")]
        public Task<ActionResult<List<LookupRow>>> GetEntities(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    id as Id,
                    entityno as Code,
                    coalesce(campusname, stxt, shortname, entityno) as Name,
                    shortname as Description,
                    countryid as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from OrgLegalentity
                where  (@ClientId is null or clientid = @ClientId)
                order by coalesce(campusname, stxt, shortname, entityno)
                """,
                cancellationToken,
                clientId: GetClientId());

        [HttpGet("countries")]
        public Task<ActionResult<List<LookupRow>>> GetCountries(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    id as Id,
                    countrycode as Code,
                    countryname as Name,
                    dialcode as Description,
                    cast(null as int) as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from Country
                order by countryname
                """,
                cancellationToken);

        [HttpGet("states")]
        public Task<ActionResult<List<LookupRow>>> GetStates(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    Id,
                    statecode as Code,
                    stxt as Name,
                    cast(null as nvarchar(500)) as Description,
                    countryid as CountryId,
                    Id as StateId,
                    cast(null as int) as HeadEntityId
                from State
                order by stxt
                """,
                cancellationToken);

        [HttpGet("cities")]
        public Task<ActionResult<List<LookupRow>>> GetCities(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    Id,
                    citycode as Code,
                    citystxt as Name,
                    dialcode as Description,
                    ctryid as CountryId,
                    stateid as StateId,
                    cast(null as int) as HeadEntityId
                from City
                order by citystxt
                """,
                cancellationToken);

        [HttpGet("sites")]
        public Task<ActionResult<List<LookupRow>>> GetSites(CancellationToken cancellationToken)
            => QueryLookup(
                """
                select
                    Id,
                    code as Code,
                    stxt as Name,
                    cast(null as nvarchar(500)) as Description,
                    cast(null as int) as CountryId,
                    cast(null as int) as StateId,
                    cast(null as int) as HeadEntityId
                from Orgsite
                order by stxt
                """,
                cancellationToken);

        private async Task<ActionResult<List<LookupRow>>> QueryLookup(
            string sql,
            CancellationToken cancellationToken,
            int? clientId = null,
            int? entityId = null)
        {
            var connection = _context.Database.GetDbConnection();
            await using var _ = await OpenIfNeeded(connection, cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            if (sql.Contains("@ClientId", StringComparison.Ordinal))
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@ClientId";
                parameter.DbType = DbType.Int32;
                parameter.Value = clientId.HasValue ? clientId.Value : DBNull.Value;
                command.Parameters.Add(parameter);
            }
            if (sql.Contains("@EntityId", StringComparison.Ordinal))
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@EntityId";
                parameter.DbType = DbType.Int32;
                parameter.Value = entityId.HasValue ? entityId.Value : DBNull.Value;
                command.Parameters.Add(parameter);
            }

            var rows = new List<LookupRow>();
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                rows.Add(new LookupRow(
                    GetInt(reader, "Id"),
                    GetString(reader, "Code"),
                    GetString(reader, "Name"),
                    GetString(reader, "Description"),
                    GetNullableInt(reader, "CountryId"),
                    GetNullableInt(reader, "StateId"),
                    GetNullableInt(reader, "HeadEntityId")));
            }

            return Ok(new
            {
                Items = rows,
                TotalCount = rows.Count,
                PageNumber = 1,
                PageSize = rows.Count
            });
        }

        private int? GetClientId()
            => GetScopedId("ClientId", "clientId", "client_id", "X-Client-Id");

        private int? GetEntityId()
            => GetScopedId("CampusId", "campusId", "campus_id", "X-Campus-Id", "Campus-Id")
                ?? GetScopedId("EntityId", "entityId", "entity_id", "X-Entity-Id", "Entity-Id");

        private int? GetScopedId(string primaryClaim, string alternateClaim, string snakeCaseClaim, params string[] headers)
        {
            foreach (var header in headers)
            {
                if (Request.Headers.TryGetValue(header, out var value) && int.TryParse(value, out var parsed))
                    return parsed;
            }

            var claim = User.Claims.FirstOrDefault(x =>
                x.Type == primaryClaim || x.Type == alternateClaim || x.Type == snakeCaseClaim);
            return claim != null && int.TryParse(claim.Value, out var claimValue) ? claimValue : null;
        }

        private static async Task<IAsyncDisposable> OpenIfNeeded(DbConnection connection, CancellationToken cancellationToken)
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                return new NoopAsyncDisposable();
            }

            await connection.OpenAsync(cancellationToken);
            return new ConnectionCloser(connection);
        }

        private static int GetInt(DbDataReader reader, string name)
            => reader[name] == DBNull.Value ? 0 : Convert.ToInt32(reader[name]);

        private static int? GetNullableInt(DbDataReader reader, string name)
            => reader[name] == DBNull.Value ? null : Convert.ToInt32(reader[name]);

        private static string? GetString(DbDataReader reader, string name)
            => reader[name] == DBNull.Value ? null : Convert.ToString(reader[name]);

        private sealed class ConnectionCloser : IAsyncDisposable
        {
            private readonly DbConnection _connection;

            public ConnectionCloser(DbConnection connection)
            {
                _connection = connection;
            }

            public ValueTask DisposeAsync()
            {
                return new ValueTask(_connection.CloseAsync());
            }
        }

        private sealed class NoopAsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }

    public sealed record LookupRow(
        int Id,
        string? Code,
        string? Name,
        string? Description,
        int? CountryId,
        int? StateId,
        int? HeadEntityId);
}
