namespace Organization.Service.DTOs
{
    /// <summary>The rules written for one AI feature on the Institution screen's AI Rules tab.</summary>
    /// <param name="FeatureCode">OrgAiRule.featurecodeid: TTG (Timetable Generation) or CBG (AI Chatbot).</param>
    /// <param name="Prompt">The rules as bullet-point text; empty when none are written.</param>
    public sealed record InstitutionAiRuleV3Dto(string FeatureCode, string Prompt);

    /// <param name="Available">False when this database has no OrgAiRule table yet, so nothing can be saved.</param>
    public sealed record InstitutionAiRulesV3Dto(bool Available, List<InstitutionAiRuleV3Dto> Rules);

    public sealed record SaveInstitutionAiRulesV3Request(List<InstitutionAiRuleV3Dto> Rules);
}
