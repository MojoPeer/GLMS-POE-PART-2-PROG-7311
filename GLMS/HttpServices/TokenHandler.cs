// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using System.Net.Http.Headers;

namespace GLMS.HttpServices
{
    /// <summary>
    /// DelegatingHandler that attaches the JWT token from the user's session
    /// to every outgoing request made to the GLMS API.
    /// </summary>
    public class TokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenHandler(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _contextAccessor.HttpContext?.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
