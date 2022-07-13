using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Application.Configurations;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Models;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Infrastructure.Contexts;
using EDO_FOMS.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace EDO_FOMS.Infrastructure.Services.Identity
{
    public class IdentityService : ITokenService
    {
        private readonly TimeSpan ExpiryAccess = new(2, 0, 0, 0);
        private readonly TimeSpan ExpiryRefresh = new(8, 0, 0, 0);

        private const string InvalidErrorMessage = "Invalid email or password";

        private readonly UserManager<EdoFomsUser> _userManager;
        private readonly RoleManager<EdoFomsRole> _roleManager;
        private readonly EdoFomsContext _db;
        //private readonly SignInManager<EdoFomsUser> _signInManager;
        private readonly AppConfiguration _appConfig;
        private readonly IStringLocalizer<IdentityService> _localizer;

        public IdentityService(
            EdoFomsContext db,
            UserManager<EdoFomsUser> userManager,
            RoleManager<EdoFomsRole> roleManager,
            //SignInManager<EdoFomsUser> signInManager,
            IOptions<AppConfiguration> appConfig,
            IStringLocalizer<IdentityService> localizer
            )
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            //_signInManager = signInManager;
            _appConfig = appConfig.Value;
            _localizer = localizer;
        }

        public async Task<Result<TokenResponse>> LoginAsync(TokenRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found"]);
            }
            if (!user.IsActive)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Active. Please contact the administrator."]);
            }
            if (!user.EmailConfirmed)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["E-Mail not confirmed"]);
            }
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Credentials"]);
            }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.Add(ExpiryRefresh);
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtAsync(user);
            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, UserImageURL = user.ProfilePictureDataUrl };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<TokenResponse>> SignInAsync(CertCheckRequest model)
        {
            var cert = _db.Certificates.FirstOrDefault(c => c.Thumbprint == model.Thumbprint);

            if (cert == null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Certificate Not Found"]);
            }

            if (!cert.IsActive)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Certificate Not Active"]);
            }

            EdoFomsUser user = null;

            if (cert == null)
            {
                if (!string.IsNullOrWhiteSpace(model.OrgInn))
                {
                    user = _db.Users.FirstOrDefault(u => u.Snils == model.Snils && u.InnLe == model.OrgInn);
                }
                else
                {
                    var users = _db.Users.Where(u => u.Snils == model.Snils).ToList();

                    if (users.Count == 0)
                    {
                        return await Result<TokenResponse>.FailAsync(_localizer["User not registered"]);
                    }
                    else if (users.Count > 1)
                    {
                        return await Result<TokenResponse>.FailAsync(_localizer["The user is registered in several organizations"]);
                    }

                    user = users[0];
                    user = _db.Users.FirstOrDefault(u => u.Snils == model.Snils);
                }

                if (user == null) { return await Result<TokenResponse>.FailAsync(_localizer["Certificate Not Found"]); }

                var newCert = new Certificate()
                {
                    Thumbprint = model.Thumbprint,
                    FromDate = model.FromDate,
                    TillDate = model.TillDate,

                    Snils = model.Snils,
                    IsActive = true,
                    SignAllowed = true,

                    UserId = user.Id,
                    CreatedBy = user.Id,
                    CreatedOn = DateTime.Now
                };

                await _db.Certificates.AddAsync(newCert);
                await _db.SaveChangesAsync();

                cert = newCert;
            } 
            else if (!string.IsNullOrWhiteSpace(model.OrgInn))
            {
                var userName = GetUserName(model.OrgInn, model.Snils); //$"{model.OrgInn}-{model.Snils}";
                user = await _userManager.FindByNameAsync(userName);
            }
            else
            {
                user = await _userManager.FindByIdAsync(cert.UserId);
            }

            if (user == null) { return await Result<TokenResponse>.FailAsync(_localizer["User Not Found"]); }
            if (!user.IsActive) { return await Result<TokenResponse>.FailAsync(_localizer["User Not Active. Please contact the administrator"]); }

            var org = _db.Organizations.FirstOrDefault(o => o.Inn == user.InnLe);

            if (org == null) { return await Result<TokenResponse>.FailAsync(_localizer["Org Not Found"]); }
            if (org.State != OrgStates.Active) { return await Result<TokenResponse>.FailAsync(_localizer["Org Not Active. Please contact the administrator"]); }

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.Now.Add(ExpiryRefresh);
            await _userManager.UpdateAsync(user);

            var token = await GenerateJwtAsync(user);
            var response = new TokenResponse
            {
                UserName = $"{user.InnLe}-{user.Snils}",
                Token = token,
                RefreshToken = user.RefreshToken,
                UserImageURL = user.ProfilePictureDataUrl
            };

            return await Result<TokenResponse>.SuccessAsync(response);
        }

        public async Task<Result<CertCheckResponse>> CertCheckAsync(CertCheckRequest model)
        {
            EdoFomsUser user;
            Organization org;
            CertCheckResponse response = new();

            var cert = _db.Certificates.FirstOrDefault(c => c.Thumbprint == model.Thumbprint);

            // Сертификат есть в системе
            if (cert != null)
            {
                if (!cert.IsActive)
                {
                    response.Result = CertCheckResults.IsBlocked;
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Сообщение об ошибке #
                }

                user = await _userManager.FindByIdAsync(cert.UserId);

                if (user == null)
                {
                    response.Result = CertCheckResults.UserNotFound;
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Сообщение об ошибке #
                }

                if (!user.IsActive)
                {
                    response.Result = CertCheckResults.UserIsBlocked;
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Сообщение об ошибке #
                }

                org = _db.Organizations.FirstOrDefault(o => o.Id == user.OrgId);

                if (org == null)
                {
                    response.Result = CertCheckResults.OrgNotFound;
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Сообщение об ошибке #
                }

                if (org.State != OrgStates.Active)
                {
                    response.Result = CertCheckResults.OrgIsBlocked;
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Сообщение об ошибке #
                }

                response.Result = CertCheckResults.IsValid;                        // Сертификат валиден
                return await Result<CertCheckResponse>.SuccessAsync(response);     // Проверка сертификата для входа в систему #
            }

            // Сертификат новый. Указано ИНН ЮЛ
            if (!string.IsNullOrWhiteSpace(model.OrgInn))
            {
                var userName = GetUserName(model.OrgInn, model.Snils);
                user = await _userManager.FindByNameAsync(userName);

                if (user != null)
                {
                    // Есть организация и пользователь. Регистрируем новый сертификат пользователю
                    Certificate newCert = new()
                    {
                        Thumbprint = model.Thumbprint,
                        UserId = user.Id,
                        Snils = model.Snils,

                        IsActive = true,
                        SignAllowed = (user.BaseRole == UserBaseRoles.Chief),

                        FromDate = model.FromDate,
                        TillDate = model.TillDate,

                        CreatedBy = user.Id,
                        CreatedOn = DateTime.Now
                    };

                    await _db.Certificates.AddAsync(newCert);
                    await _db.SaveChangesAsync();

                    var nameChanged = (!string.IsNullOrWhiteSpace(model.Surname) && user.Surname != model.Surname)
                        || (!string.IsNullOrWhiteSpace(model.GivenName) && user.GivenName != model.GivenName);

                    if (nameChanged)
                    {
                        user.Surname = model.Surname;
                        user.GivenName = model.GivenName;
                        await _userManager.UpdateAsync(user);
                    }

                    response.Result = CertCheckResults.AddedNewCert;               // Сообщение о успешном добавлении сертификата #
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Автоматический вход в систему
                }

                org = _db.Organizations.FirstOrDefault(o => o.Inn == model.OrgInn);
                if (org != null)
                {
                    // Есть организация. Регистрируем нового сотрудника и новый сертификат, по указанному ИНН
                    // Нужные доп.данные о пользователе. Регистрация, через клиента
                    response.Result = CertCheckResults.RegNewEmpl;
                    return await Result<CertCheckResponse>.SuccessAsync(response); // Автоматическая регистрация пользователя и сертификата #
                }

                // Организации нет. Требуется регистрация организации с указанным ИНН и руководителя
                response.Result = CertCheckResults.RegOrgByInn;
                return await Result<CertCheckResponse>.SuccessAsync(response); // Если шеф -> Новая Орг-я #
            }

            // Сертификат новый. Указано Наименование Организации
            // Искать по наименованию?

            // Сертификата нет. ИНН ЮЛ не указано. Есть только СНИЛС:
            var users = _db.Users.Where(u => u.Snils == model.Snils).ToList();
            if (users?.Count > 0)
            {
                if (users.Count == 1)
                {
                    // Найден один пользователь -> вернуть один ИНН, не указанный - возможен другой
                    user = users[0];
                    org = _db.Organizations.FirstOrDefault(o => o.Inn == user.InnLe);

                    OrgCardModel card = new()
                    {
                        Inn = user.InnLe,
                        ShortName = org?.ShortName ?? "",
                        Name = org?.Name ?? ""
                    };

                    response.OrgCards = new();
                    response.OrgCards.Add(card);
                    response.Result = CertCheckResults.RegOneBySnils;

                    return await Result<CertCheckResponse>.SuccessAsync(response); // Указание ИНН ЮЛ на клиенте #
                }

                // Найдено несколько пользователей на одном СНИЛС, с разными ИНН ЮЛ -> вернуть список + другой
                string[] inns = users.Select(u => u.InnLe).ToArray();
                response.OrgCards = _db.Organizations.Where(o => inns.Contains(o.Inn))
                    .Select(o => new OrgCardModel() { Inn = o.Inn, ShortName = o.ShortName, Name = o.Name})
                    .ToList();

                response.Result = CertCheckResults.RegSomeBySnils;
                return await Result<CertCheckResponse>.SuccessAsync(response); // Указание ИНН ЮЛ на клиенте #
            }

            // Сертификата нет. ИНН ЮЛ не указано. Пользователи по СНИЛС не найдены.
            // Регистрация нового пользователя в системе с указанием ИНН ЮЛ
            response.Result = CertCheckResults.RegNewUser;
            return await Result<CertCheckResponse>.SuccessAsync(response); // Указание ИНН ЮЛ на клиенте #
        }
        public async Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model)
        {
            if (model is null)
            {
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token"]);
            }
            var userPrincipal = GetPrincipalFromExpiredToken(model.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
                return await Result<TokenResponse>.FailAsync(_localizer["User Not Found"]);
            if (user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                return await Result<TokenResponse>.FailAsync(_localizer["Invalid Client Token"]);
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            user.RefreshToken = GenerateRefreshToken();
            await _userManager.UpdateAsync(user);

            var response = new TokenResponse { Token = token, RefreshToken = user.RefreshToken, RefreshTokenExpiryTime = user.RefreshTokenExpiryTime };
            return await Result<TokenResponse>.SuccessAsync(response);
        }

        private async Task<string> GenerateJwtAsync(EdoFomsUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(EdoFomsUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();
            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var thisRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForThisRoles = await _roleManager.GetClaimsAsync(thisRole);
                permissionClaims.AddRange(allPermissionsForThisRoles);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new("Title", user.Title ?? ""),

                new(ClaimTypes.Surname, user.Surname),
                new(ClaimTypes.GivenName, user.GivenName),

                new(ClaimTypes.PrimarySid, user.InnLe ?? ""),
                new(ClaimTypes.Sid, user.Snils),

                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? ""),
                new(ClaimTypes.Email, user.Email),

                new("OrgId", user.OrgId.ToString()),
                new("INN", user.Inn),
                new("BaseRole", user.BaseRole.ToString()),
                new("OrgType", user.OrgType.ToString())
            }
            .Union(userClaims)
            .Union(roleClaims)
            .Union(permissionClaims);

            return claims;
        }

        private static string GetUserName(string orgInn, string snils)
        {
            return $"{orgInn}-{snils}";
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
               claims: claims,
               expires: DateTime.UtcNow.Add(ExpiryAccess),
               signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfig.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(_localizer["Invalid token"]);
            }

            return principal;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_appConfig.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }
    }
}