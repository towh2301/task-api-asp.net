using FluentValidation;
using MyApp.Models;

namespace Validator.TaskItemValidator
{
    public class TaskItemValidator : AbstractValidator<TaskItem>
    {
        public TaskItemValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
            RuleFor(x => x.DueDate).GreaterThan(DateTime.UtcNow);
        }
    }
}