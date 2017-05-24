namespace MyDependencyServices
{
    public interface IMd5HashExtensions
    {
        // Hash an input string and return the hash as
        // a 32 character hexadecimal string.
          string GetMd5Hash( byte[] input);
        // Verify a hash against a string.
          bool VerifyMd5Hash( byte[] input, string hash);
          string GetMd5Hash( string input);
          bool VerifyMd5Hash( string input, string hash);
       
    }
}