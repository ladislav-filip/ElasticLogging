namespace ElasticLoggingWeb.Models;

public record UserSignIn(string Username, string Password);

public record SignInSuccess(string Token);
