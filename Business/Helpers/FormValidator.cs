using Domain.Dtos;

namespace Business.Helpers
{
    public interface IFormValidator
    {
        ResponseResult ValidateDateFields(params DateTime[] dates);
        ResponseResult ValidateFormFields(params string?[] fields);
        ResponseResult ValidateNumberFields(params int[] fields);
        ResponseResult ValidatePasswordMatchAndTerms(SignUpForm form);
    }

    public class FormValidator : IFormValidator
    {
        public ResponseResult ValidateFormFields(params string?[] fields)
        {
            if (fields.Any(string.IsNullOrEmpty))
                return ResponseResult.BadRequest("Required fields are missing in form");

            return ResponseResult.Ok();
        }

        public ResponseResult ValidatePasswordMatchAndTerms(SignUpForm form)
        {
            if (form.Password != form.ConfirmPassword)
                return ResponseResult.BadRequest("Passwords do not match");

            if (!form.TermsAndConditions)
                return ResponseResult.BadRequest("Must accept Terms and Conditions");

            return ResponseResult.Ok();
        }

        public ResponseResult ValidateDateFields(params DateTime[] dates)
        {
            if (dates.Any(dates => dates == DateTime.MinValue))
                return ResponseResult.BadRequest("Required dates are missing or invalid");

            return ResponseResult.Ok();
        }

        public ResponseResult ValidateNumberFields(params int[] fields)
        {
            if (fields.Any(value => value == 0))
                return ResponseResult.BadRequest("Required fields are missing in form");

            return ResponseResult.Ok();
        }
    }
}
