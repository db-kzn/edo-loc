using System.Security.Claims;

namespace EDO_FOMS.Client.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static string GetUserId(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
        internal static string GetSurname(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.Surname);
        internal static string GetGivenName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.GivenName);
        internal static string GetUserName(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.Name);
        internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);
        internal static string GetSnils(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.Sid);
        internal static string GetInnLe(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.PrimarySid);
        internal static string GetEmail(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        internal static string GetTitle(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue("Title");
        internal static string GetInn(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue("INN");
        internal static string GetOrgId(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue("OrgId");
        internal static string GetBaseRole(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue("BaseRole");
        internal static string GetOrgType(this ClaimsPrincipal claimsPrincipal) => claimsPrincipal.FindFirstValue("OrgType");
    }
}