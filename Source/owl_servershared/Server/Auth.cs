public static class Auth
{
	private const string ms_strPrefix = "bcrypt_sha256$";
	private const int m_iWork = 12;

	public enum ELoginResult
	{
		Success,
		Failed,
		AccountDoesNotExist,
		NotActivated,
	}

	public static ELoginResult VerifyPassword(string strInput, string strVerify)
	{
		string strInputPasswordHashed = HelperFunctions.Hashing.sha256(strInput);
		return Verify(strInputPasswordHashed, strVerify.Replace(ms_strPrefix, "")) ? ELoginResult.Success : ELoginResult.Failed;
	}

	public static string GeneratePassword(string strInput)
	{
		return ms_strPrefix + HashPassword(HelperFunctions.Hashing.sha256(strInput), m_iWork);
	}

	public static bool Verify(string strInput, string strVerify)
	{
		return BCrypt.Net.BCrypt.Verify(strInput, strVerify);
	}

	public static string HashPassword(string strInput, int iWork)
	{
		return BCrypt.Net.BCrypt.HashPassword(strInput, iWork);
	}

	public static bool IsEmailValid(string strEmail)
	{
		try
		{
			System.Net.Mail.MailAddress emailObj = new System.Net.Mail.MailAddress(strEmail);
			return (emailObj.Address == strEmail);
		}
		catch
		{
			return false;
		}
	}

	/*
	public static bool IsPasswordSecure(string strPassword)
	{
		bool bPasswordSecure = true;

		bPasswordSecure &= strPassword.Length >= 6; // length
		bPasswordSecure &= strPassword.Any(ch => !char.IsLetterOrDigit(ch)); // Do we have at least one special char?
		bPasswordSecure &= strPassword.Any(ch => !char.IsUpper(ch)); // Do we have at least one upper case?
		bPasswordSecure &= strPassword.Any(ch => !char.IsDigit(ch)); // Do we have at least one number?

		return bPasswordSecure;
	}
	*/
}