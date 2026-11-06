using HumanResource.Service.PayrollAccounting.Commands;
using HumanResource.Service.PayrollAccounting.Contracts;
using HumanResource.Service.PayrollAccounting.Engine;
using FluentValidation;

namespace HumanResource.Service.PayrollAccounting.Validators;

public sealed class PostJvCommandValidator : AbstractValidator<PostJvCommand>
{
	public PostJvCommandValidator()
	{
		RuleFor(x => x.ClientId).GreaterThan(0);
		RuleFor(x => x.EntityId).GreaterThan(0);
		RuleFor(x => x.TransactionDate).NotEqual(default(DateTime)).WithMessage("Transaction date is required.");
		RuleFor(x => x.Narration).MaximumLength(500);
		RuleFor(x => x.Code).MaximumLength(50);

		RuleFor(x => x.Lines)
			.Must(lines => JvLineRules.Check(lines) == null)
			.WithMessage(x => JvLineRules.Check(x.Lines)!);

		When(x => x.SystemGenerated, () =>
		{
			RuleFor(x => x.Source)
				.Must(JvSources.IsKnown)
				.WithMessage(x => $"Unknown JV source '{x.Source}'.");
			RuleFor(x => x.SourceId).NotEmpty().WithMessage("Source document id is required.");
		}).Otherwise(() =>
		{
			RuleFor(x => x.Source).Empty().WithMessage("A manual JV cannot carry a source document.");
		});
	}
}

public sealed class RepostJvCommandValidator : AbstractValidator<RepostJvCommand>
{
	public RepostJvCommandValidator()
	{
		RuleFor(x => x.NewPosting).NotNull().SetValidator(new PostJvCommandValidator());
		RuleFor(x => x.TransactionId)
			.NotNull()
			.When(x => x.NewPosting is { SystemGenerated: false })
			.WithMessage("A manual JV is reposted by its transaction id.");
	}
}

public sealed class ReverseJvCommandValidator : AbstractValidator<ReverseJvCommand>
{
	public ReverseJvCommandValidator()
	{
		RuleFor(x => x.ClientId).GreaterThan(0);
		RuleFor(x => x.Source).Must(JvSources.IsKnown).WithMessage(x => $"Unknown JV source '{x.Source}'.");
		RuleFor(x => x.SourceId).NotEmpty();
		RuleFor(x => x.Reason).MaximumLength(400);
	}
}

public sealed class PostJvBacklogCommandValidator : AbstractValidator<PostJvBacklogCommand>
{
	public PostJvBacklogCommandValidator()
	{
		RuleFor(x => x.ClientId).GreaterThan(0);
		RuleFor(x => x.EntityId).GreaterThan(0);
		RuleFor(x => x.Source).Must(JvSources.IsKnown).WithMessage(x => $"Unknown JV source '{x.Source}'.");
		RuleFor(x => x.SourceIds).NotEmpty();
	}
}
