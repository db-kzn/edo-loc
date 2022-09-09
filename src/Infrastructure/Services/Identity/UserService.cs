using AutoMapper;
using EDO_FOMS.Application.Exceptions;
using EDO_FOMS.Application.Extensions;
using EDO_FOMS.Application.Interfaces.Services;
using EDO_FOMS.Application.Interfaces.Services.Identity;
using EDO_FOMS.Application.Requests.Identity;
using EDO_FOMS.Application.Requests.Mail;
using EDO_FOMS.Application.Responses.Docums;
using EDO_FOMS.Application.Responses.Identity;
using EDO_FOMS.Domain.Enums;
using EDO_FOMS.Infrastructure.Contexts;
using EDO_FOMS.Infrastructure.Models.Identity;
using EDO_FOMS.Infrastructure.Specifications;
using EDO_FOMS.Shared.Constants.Role;
using EDO_FOMS.Shared.Wrapper;
using EDO_FOMS.Application.Requests.Admin;
using EDO_FOMS.Domain.Entities.Org;
using EDO_FOMS.Shared.Constants.User;
using EDO_FOMS.Application.Requests.Person;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EDO_FOMS.Application.Models;
using MediatR;
using EDO_FOMS.Application.Features.Orgs.Commands;
using EDO_FOMS.Domain.Entities.Public;
using EDO_FOMS.Application.Features.System.Command;
using System.Linq.Expressions;
using EDO_FOMS.Shared.Models;
using EDO_FOMS.Application.Features.Orgs.Queries;

namespace EDO_FOMS.Infrastructure.Services.Identity
{
    public class UserService : IUserService
    {
        private readonly UserManager<EdoFomsUser> _userManager;
        private readonly RoleManager<EdoFomsRole> _roleManager;
        private readonly EdoFomsContext _db;
        private readonly IMapper _mapper;
        private readonly IMailService _mailService;
        private readonly IMediator _meditor;
        private readonly IStringLocalizer<UserService> _localizer;
        private readonly IExcelService _excelService;
        private readonly ICurrentUserService _currentUserService;

        public UserService(
            UserManager<EdoFomsUser> userManager,
            RoleManager<EdoFomsRole> roleManager,
            EdoFomsContext db,
            IMapper mapper,
            IMailService mailService,
            IMediator mediator,
            IStringLocalizer<UserService> localizer,
            IExcelService excelService,
            ICurrentUserService currentUserService
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _mapper = mapper;
            _mailService = mailService;
            _meditor = mediator;
            _localizer = localizer;
            _excelService = excelService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<UserResponse>>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = _mapper.Map<List<UserResponse>>(users);
            return await Result<List<UserResponse>>.SuccessAsync(result);
        }

        public async Task<PaginatedResult<UserResponse>> GetPagedUsersAsync(GetPagedUsersRequest request)
        {
            Expression<Func<EdoFomsUser, UserResponse>> expression = e => new UserResponse
            {
                Id = e.Id,
                InnLe = e.InnLe,
                Snils = e.Snils,
                Inn = e.Inn,

                Title = e.Title,
                UserName = e.UserName,
                Surname = e.Surname,
                GivenName = e.GivenName,

                OrgType = e.OrgType,
                BaseRole = e.BaseRole,
                IsActive = e.IsActive,

                Email = e.Email,
                EmailConfirmed = e.EmailConfirmed,
                PhoneNumber = e.PhoneNumber,
                PhoneNumberConfirmed = e.PhoneNumberConfirmed,

                ProfilePictureDataUrl = e.ProfilePictureDataUrl,
                CreatedOn = e.CreatedOn
            };

            var userSpec = new UserFilterSpecification(request);

            var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

            return await _userManager.Users
                .Specify(userSpec)
                .Select(expression)
                .OrderBy(sort)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
        }

        public async Task<PaginatedResult<UserResponse>> SearchUsersAsync(SearchUsersRequest request)
        {
            Expression<Func<EdoFomsUser, UserResponse>> expression = e => new UserResponse
            {
                Id = e.Id,
                InnLe = e.InnLe,
                Snils = e.Snils,
                Inn = e.Inn,

                Title = e.Title,
                UserName = e.UserName,
                Surname = e.Surname,
                GivenName = e.GivenName,

                OrgType = e.OrgType,
                BaseRole = e.BaseRole,
                IsActive = e.IsActive,

                Email = e.Email,
                EmailConfirmed = e.EmailConfirmed,
                PhoneNumber = e.PhoneNumber,
                PhoneNumberConfirmed = e.PhoneNumberConfirmed,

                ProfilePictureDataUrl = e.ProfilePictureDataUrl,
                CreatedOn = e.CreatedOn
            };

            var userSpec = new UserFilterSpecification(request);

            var sort = (request.OrderBy?.Any() == true) ? string.Join(",", request.OrderBy) : "Id Descending";

            return await _userManager.Users
                .Specify(userSpec)
                .Select(expression)
                .OrderBy(sort)
                .ToPaginatedListAsync(request.PageNumber, request.PageSize);
        }

        public async Task<Result<List<UserResponse>>> GetAllByOrgIdAsync(int orgId)
        {
            var users = await _userManager.Users.Where(u => u.OrgId == orgId).ToListAsync();
            var result = _mapper.Map<List<UserResponse>>(users);
            return await Result<List<UserResponse>>.SuccessAsync(result);
        }
        public async Task<Result<List<ContactResponse>>> GetFoundContacts(OrgTypes orgType, UserBaseRoles baseRole, string searchString, int take = 10, int? orgId = null)
        {
            var users = _userManager.Users;

            if (orgId is not null) { users = users.Where(u => u.OrgId == orgId); }
            if (orgType != OrgTypes.Undefined) { users = users.Where(u => u.OrgType == orgType); }
            if (baseRole != UserBaseRoles.Undefined) { users = users.Where(u => u.BaseRole == baseRole); }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                var search = searchString.ToUpper();
                users = users.Where(u =>
                    u.Surname.ToUpper().Contains(search) || u.GivenName.ToUpper().Contains(search) || u.InnLe.Contains(search));
            }

            //if (orgType == OrgTypes.Undefined && baseRole == UserBaseRoles.Undefined && string.IsNullOrWhiteSpace(searchString))

            var list = await users.Take(10)
                .Select(u => new { u.Id, u.Surname, u.GivenName, u.OrgId, u.InnLe, u.IsActive })
                //.Take(take)
                .ToListAsync();

            var orgIds = list.Select(u => u.OrgId).Distinct().ToArray();
            var result = await _meditor.Send(new GetOrgShortNamesQuery(orgIds));
            var dicNames = result.Data;

            var contacts = new List<ContactResponse>();// _mapper.Map<List<ContactResponse>>(list);

            list.ForEach(u =>
            {
                ContactResponse contact = new()
                {
                    Id = u.Id,

                    IsActive = u.IsActive,
                    Surname = u.Surname,
                    GivenName = u.GivenName,

                    OrgId = u.OrgId,
                    OrgShortName = dicNames[u.OrgId],
                    InnLe = u.InnLe
                };

                contacts.Add(contact);
            });

            return await Result<List<ContactResponse>>.SuccessAsync(contacts);
        }
        public async Task<Result<List<ContactResponse>>> GetAgreementMembersAsync(int orgId, string search)
        {
            var users = _userManager.Users.Where(u => u.OrgId == orgId);
            var foundUsers = (string.IsNullOrEmpty(search)) ? users
                : users.Where(u => u.Surname.Contains(search) || u.GivenName.Contains(search));

            var list = await foundUsers.ToListAsync();
            var result = _mapper.Map<List<ContactResponse>>(list);

            return await Result<List<ContactResponse>>.SuccessAsync(result);
        }
        public async Task<Result<int>> UpdateUsersOrgType(UpdateUsersOrgTypeRequest request, string origin)
        {
            //var users = await _userManager.Users.Where(u => u.OrgId == request.OrgId).ToListAsync();
            var org = _db.Organizations.FirstOrDefault(o => o.Id == request.OrgId);

            //if (org.TypeIx == request.OrgTypeIx) { return await Result<int>.SuccessAsync(0); }

            var newWorkerOf = (request.OrgType == OrgTypes.MO) ? RoleConstants.WorkerOf.MO
                    : (request.OrgType == OrgTypes.SMO) ? RoleConstants.WorkerOf.SMO
                    : (request.OrgType == OrgTypes.Fund) ? RoleConstants.WorkerOf.Fund : "";

            if (string.IsNullOrWhiteSpace(newWorkerOf)) { return await Result<int>.SuccessAsync(0); }

            var users = _db.Users.Where(u => u.OrgId == request.OrgId).ToList();

            foreach (var user in users)
            {
                var oldWorkerOf = (user.OrgType == OrgTypes.MO) ? RoleConstants.WorkerOf.MO
                    : (user.OrgType == OrgTypes.SMO) ? RoleConstants.WorkerOf.SMO
                    : (user.OrgType == OrgTypes.Fund) ? RoleConstants.WorkerOf.Fund : "";

                if (!string.IsNullOrWhiteSpace(oldWorkerOf))
                {
                    var isInOldRole = await _userManager.IsInRoleAsync(user, oldWorkerOf);
                    if (isInOldRole) await _userManager.RemoveFromRoleAsync(user, oldWorkerOf);
                }

                user.OrgType = request.OrgType;
                var isInNewRole = await _userManager.IsInRoleAsync(user, newWorkerOf);
                if (!isInNewRole) await _userManager.AddToRoleAsync(user, newWorkerOf);
            }

            _db.UpdateRange(users);
            var result = await _db.SaveChangesAsync();

            return await Result<int>.SuccessAsync(result);
        }
        public async Task<IResult> RegisterByCertAsync(RegisterByCertRequest request, string origin, IMediator mediator)
        {
            var userName = UserName(request.InnLe, request.Snils); // $"{request.InnLe}-{request.Snils}";
            var user = await _userManager.FindByNameAsync(userName);

            if (user != null)
            {
                return await Result.FailAsync(_localizer["User with pair of INN and SNILS is already taken"]);
            }

            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Inn == request.InnLe);

            if (request.BaseRole == UserBaseRoles.Chief && org == null)
            {
                var company = await _db.Companies.FirstOrDefaultAsync(c => c.Inn == request.InnLe);

                org = new Organization()
                {
                    OmsCode = company?.Code ?? string.Empty,
                    Type = company?.Type ?? OrgTypes.MO,

                    Inn = request.InnLe,
                    Ogrn = request.Ogrn,
                    Name = request.Org,

                    //UserId = user.Id ?? "",
                    //UserSnils = user.Snils,
                    Email = request.Email,

                    IsPublic = true,
                    State = OrgStates.OnSubmit,

                    CreatedOn = DateTime.Now
                    //CreatedBy = user.Id,
                };

                //await _db.Organizations.AddAsync(org);
                //await _db.SaveChangesAsync();

                AddEditOrgCommand addEditOrg = new()
                {
                    Inn = org.Inn,
                    Name = org.Name,
                    ShortName = "",

                    IsPublic = org.IsPublic,
                    Type = org.Type,
                    State = org.State,

                    Ogrn = org.Ogrn,
                    Phone = "",
                    Email = org.Email
                };

                var res = await mediator.Send(addEditOrg);
                org.Id = (res.Succeeded) ? res.Data : 0;
            }

            user = new()
            {
                OrgId = org.Id,
                InnLe = org.Inn,
                OrgType = org.Type,

                UserName = userName,
                Snils = request.Snils,
                Inn = request.Inn,

                Title = request.Title,
                Surname = request.Surname,
                GivenName = request.GivenName,

                BaseRole = request.BaseRole,
                IsActive = false, //(request.BaseRoleIx == (int)UserBaseRoles.Chief),

                Email = request.Email,
                CreatedOn = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, UserConstants.DefaultPassword);

            if (!result.Succeeded)
            {
                return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
            }

            var subscribe = new Subscribe() { UserId = user.Id };
            await mediator.Send(new SubscribeCommand(subscribe));

            var role = UserRoleByIx(user.BaseRole);
            if (!string.IsNullOrWhiteSpace(role))
            {
                await _userManager.AddToRoleAsync(user, role);

                //var workerOf = (org.TypeIx == (int)OrgTypes.MO) ? RoleConstants.WorkerOf.MO
                //    : (org.TypeIx == (int)OrgTypes.SMO) ? RoleConstants.WorkerOf.SMO
                //    : (org.TypeIx == (int)OrgTypes.Fund) ? RoleConstants.WorkerOf.Fund : "";

                //if (!string.IsNullOrWhiteSpace(workerOf))
                //{
                //    await _userManager.AddToRoleAsync(user, workerOf);
                //}

            }

            var status = WorkerStatusByIx(user.OrgType);
            if (!string.IsNullOrWhiteSpace(status))
            {
                await _userManager.AddToRoleAsync(user, status);
            }

            var cert = new Certificate()
            {
                Thumbprint = request.Thumbprint,
                UserId = user.Id,
                Snils = request.Snils,

                IsActive = true,
                SignAllowed = (request.BaseRole == UserBaseRoles.Chief),

                FromDate = request.FromDate,
                TillDate = request.TillDate,

                CreatedBy = user.Id,
                CreatedOn = DateTime.Now
            };

            await _db.Certificates.AddAsync(cert);
            await _db.SaveChangesAsync();

            return await Result<string>.SuccessAsync(user.Id, _localizer["User {0} Registered.", UserFullName(user)]);
        }
        public async Task<IResult> NewUserAsync(NewUserRequest request, string origin)
        {
            var userName = UserName(request.InnLe, request.Snils); // $"{request.InnLe}-{request.Snils}";
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return await Result.FailAsync(_localizer["User with pair of INN and SNILS is already taken"]);
            }

            user = new()
            {
                UserName = userName,

                InnLe = request.InnLe,
                Snils = request.Snils,
                Inn = request.Inn,

                Title = request.Title,
                Surname = request.Surname,
                GivenName = request.GivenName,

                BaseRole = request.BaseRole,
                OrgType = request.OrgType,

                IsActive = request.IsActive,
                Email = request.Email,
                EmailConfirmed = request.EmailConfirmed,
                CreatedOn = DateTime.Now
            };

            var result = await _userManager.CreateAsync(user, UserConstants.DefaultPassword);

            if (result.Succeeded)
            {
                var role = UserRoleByIx(user.BaseRole);
                if (!string.IsNullOrWhiteSpace(role))
                {
                    await _userManager.AddToRoleAsync(user, role);
                }

                var status = WorkerStatusByIx(user.OrgType);
                if (!string.IsNullOrWhiteSpace(role))
                {
                    await _userManager.AddToRoleAsync(user, status);
                }

                var cert = new Certificate()
                {
                    Thumbprint = request.Thumbprint,
                    UserId = user.Id ?? "",
                    Snils = request.Snils,

                    IsActive = true,
                    SignAllowed = false,

                    FromDate = request.FromDate ?? DateTime.Now,
                    TillDate = request.TillDate ?? DateTime.Now,

                    CreatedBy = user.Id,
                    CreatedOn = DateTime.Now
                };

                await _db.Certificates.AddAsync(cert);
                await _db.SaveChangesAsync();

                return await Result<string>.SuccessAsync(user.Id, _localizer["User {0} Registered.", UserFullName(user)]);
            }

            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
        public async Task<IResult> EditUserAsync(EditUserRequest request, string origin)
        {
            var userName = UserName(request.InnLe, request.Snils); //$"{request.InnLe}-{request.Snils}";
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
            {
                return await Result.FailAsync(_localizer["User is not found"]);
            }

            user.UserName = userName;

            user.InnLe = request.InnLe;
            user.Snils = request.Snils;
            user.Inn = request.Inn;

            user.Title = request.Title;
            user.Surname = request.Surname;
            user.GivenName = request.GivenName;

            if (user.BaseRole != request.BaseRole)
            {
                var userRole = UserRoleByIx(user.BaseRole);
                var requestRole = UserRoleByIx(request.BaseRole);

                if (!string.IsNullOrWhiteSpace(userRole))
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole);
                }

                if (!string.IsNullOrWhiteSpace(requestRole))
                {
                    await _userManager.AddToRoleAsync(user, requestRole);
                }

                user.BaseRole = request.BaseRole;
            }

            if (user.OrgType != request.OrgType)
            {
                var userStatus = WorkerStatusByIx(user.OrgType);
                var requestStatus = WorkerStatusByIx(request.OrgType);

                if (!string.IsNullOrWhiteSpace(userStatus))
                {
                    await _userManager.RemoveFromRoleAsync(user, userStatus);
                }

                if (!string.IsNullOrWhiteSpace(requestStatus))
                {
                    await _userManager.AddToRoleAsync(user, requestStatus);
                }

                user.OrgType = request.OrgType;
            }


            user.IsActive = request.IsActive;
            user.Email = request.Email;
            user.EmailConfirmed = request.EmailConfirmed;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                //var cert = await _db.Certificates.FirstOrDefaultAsync(c => c.Thumbprint == request.Thumbprint) ?? new Certificate();

                //if (cert.Thumbprint == "")
                //{
                //    var c = new Certificate()
                //    {
                //        Thumbprint = request.Thumbprint,
                //        UserId = user.Id ?? "",
                //        Snils = request.Snils,

                //        IsActive = false,
                //        SignAllowed = false,

                //        FromDate = request.FromDate ?? DateTime.Now,
                //        TillDate = request.TillDate ?? DateTime.Now
                //    };
                //    await _db.Certificates.AddAsync(c);
                //}
                //else
                //{
                //    cert.Thumbprint = request.Thumbprint;
                //    cert.UserId = user.Id ?? "";
                //    cert.Snils = request.Snils;

                //    cert.IsActive = false;
                //    cert.SignAllowed = false;

                //    cert.FromDate = request.FromDate ?? DateTime.Now;
                //    cert.TillDate = request.TillDate ?? DateTime.Now;

                //    _db.Certificates.Add(cert);
                //}

                //await _db.SaveChangesAsync();

                return await Result<string>.SuccessAsync(user.Id, _localizer["User {0} Updated", UserFullName(user)]);

                //string.Format(_localizer["User {0} Updated"], name)
                //return await Result<string>.SuccessAsync(employee.Id, _localizer["User {0} Updated", name]);
            }

            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
        public async Task<IResult> AddEditEmployeeAsync(AddEditEmployeeRequest request, string origin)
        {
            EdoFomsUser employee;
            IdentityResult result;

            //var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Inn == request.InnLe);

            if (string.IsNullOrWhiteSpace(request.Id))
            {
                // Add Employee
                employee = new()
                {
                    Title = request.Title,

                    Surname = request.Surname,
                    GivenName = request.GivenName,

                    Snils = request.Snils,
                    Inn = request.Inn,

                    Email = request.Email,
                    EmailConfirmed = false,

                    PhoneNumber = request.PhoneNumber,
                    PhoneNumberConfirmed = false,

                    BaseRole = request.BaseRole,
                    IsActive = request.IsActive
                };

                result = await _userManager.CreateAsync(employee, UserConstants.DefaultPassword);

                var role = UserRoleByIx(employee.BaseRole);

                if (!string.IsNullOrWhiteSpace(role))
                {
                    await _userManager.AddToRoleAsync(employee, role);
                }
            }
            else
            {
                // Update Employee
                employee = await _userManager.FindByIdAsync(request.Id);

                employee.Title = request.Title;

                employee.Surname = request.Surname;
                employee.GivenName = request.GivenName;

                employee.Snils = request.Snils;
                employee.Inn = request.Inn;

                if (employee.Email != request.Email)
                {
                    employee.EmailConfirmed = false;
                    employee.Email = request.Email;
                }
                if (employee.PhoneNumber != request.PhoneNumber)
                {
                    employee.PhoneNumberConfirmed = false;
                    employee.PhoneNumber = request.PhoneNumber;
                }

                if (employee.BaseRole != request.BaseRole)
                {
                    var employeeRole = UserRoleByIx(employee.BaseRole);
                    var requestRole = UserRoleByIx(request.BaseRole);

                    if (!string.IsNullOrWhiteSpace(employeeRole))
                    {
                        await _userManager.RemoveFromRoleAsync(employee, employeeRole);
                    }

                    if (!string.IsNullOrWhiteSpace(requestRole))
                    {
                        await _userManager.AddToRoleAsync(employee, requestRole);
                    }

                    employee.BaseRole = request.BaseRole;
                }

                employee.IsActive = request.IsActive;

                result = await _userManager.UpdateAsync(employee);
            }

            if (result.Succeeded)
            {
                var name = employee.Surname + " " + employee.GivenName;
                //string.Format(_localizer["User {0} Updated"], name)
                return await Result<string>.SuccessAsync(employee.Id, _localizer["User {0} Updated", name]);
            }

            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        }
        public async Task<IResult<bool>> GetUserOrgExists(string inn)
        {
            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Inn == inn);
            var userOrgExists = org != null;

            return await Result<bool>.SuccessAsync(userOrgExists);
        }
        public async Task<Employee> GetEmployeeAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            Employee employee = new()
            {
                EmployeeId = 0,
                UserId = user.Id,
                OrgId = user.OrgId,

                InnLe = user.InnLe,
                Snils = user.Snils,
                Inn = user.Inn,

                Title = user.Title,
                Surname = user.Surname,
                GivenName = user.GivenName,

                OrgType = user.OrgType,
                BaseRole = user.BaseRole,
                IsActive = user.IsActive
            };

            return employee;
        }
        public async Task<ContactResponse> GetContactAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) { return null; }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) { return null; }

            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == user.OrgId);

            return new()
            {
                Id = userId,

                IsActive = user.IsActive,
                Surname = user.Surname,
                GivenName = user.GivenName,

                OrgId = user.OrgId,
                InnLe = user.InnLe,
                OrgShortName = org is not null ? org.ShortName : ""
            };
        }

        public async Task<IResult<Organization>> GerUserOrgAsync(string inn)
        {
            var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Inn == inn) ?? new();
            return await Result<Organization>.SuccessAsync(org);
        }
        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            //var currentId = _currentUserService.UserId;
            //if (string.IsNullOrWhiteSpace(currentId))
            //    return await Result<UserResponse>.SuccessAsync(_mapper.Map<UserResponse>(new EdoFomsUser()));

            var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            var result = _mapper.Map<UserResponse>(user);
            return await Result<UserResponse>.SuccessAsync(result);
        }
        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
            var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.Admin);
            if (isAdmin)
            {
                return await Result.FailAsync(_localizer["Administrators Profile's Status cannot be toggled"]);
            }
            if (user != null)
            {
                user.IsActive = request.ActivateUser;
                var identityResult = await _userManager.UpdateAsync(user);
            }
            return await Result.SuccessAsync();
        }
        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
        {
            var viewModel = new List<UserRoleModel>();
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                viewModel.Add(userRolesViewModel);
            }
            var result = new UserRolesResponse { UserRoles = viewModel };
            return await Result<UserRolesResponse>.SuccessAsync(result);
        }
        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            //if (user.Email == "admin@EdoFoms.ru") { return await Result.FailAsync(_localizer["Not Allowed."]); }

            var roles = await _userManager.GetRolesAsync(user);
            var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.Admin))
            {
                var tryToAddAdministratorRole = selectedRoles
                    .Any(x => x.RoleName == RoleConstants.Admin);
                var userHasAdministratorRole = roles.Any(x => x == RoleConstants.Admin);
                if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
                {
                    return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
                }
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
            return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }
        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.Email));
            }
            else
            {
                throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
            }
        }
        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            var mailRequest = new MailRequest
            {
                Body = string.Format(_localizer["Please reset your password by <a href='{0}>clicking here</a>."], HtmlEncoder.Default.Encode(passwordResetURL)),
                Subject = _localizer["Reset Password"],
                ToAddress = request.Email
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
        }
        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
            }
            else
            {
                return await Result.FailAsync(_localizer["An Error has occured!"]);
            }
        }
        public async Task<int> GetCountAsync()
        {
            var count = await _userManager.Users.CountAsync();
            return count;
        }
        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            var userSpec = new UserFilterSpecification(searchString);

            var users = await _userManager.Users
                .Specify(userSpec)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();

            var result = await _excelService.ExportAsync(users, sheetName: _localizer["Users"],
                mappers: new Dictionary<string, Func<EdoFomsUser, object>>
                {
                    { _localizer["Id"], item => item.Id },
                    { _localizer["Snils"], item => item.Snils },
                    { _localizer["GivenName"], item => item.GivenName },
                    { _localizer["Surname"], item => item.Surname },
                    { _localizer["UserName"], item => item.UserName },
                    { _localizer["Email"], item => item.Email },
                    { _localizer["EmailConfirmed"], item => item.EmailConfirmed },
                    { _localizer["PhoneNumber"], item => item.PhoneNumber },
                    { _localizer["PhoneNumberConfirmed"], item => item.PhoneNumberConfirmed },
                    { _localizer["IsActive"], item => item.IsActive },
                    { _localizer["CreatedOn (Local)"], item => DateTime.SpecifyKind(item.CreatedOn, DateTimeKind.Utc).ToLocalTime().ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["CreatedOn (UTC)"], item => item.CreatedOn.ToString("G", CultureInfo.CurrentCulture) },
                    { _localizer["ProfilePictureDataUrl"], item => item.ProfilePictureDataUrl },
                });

            return result;
        }
        private static string UserRoleByIx(UserBaseRoles role)
        {
            return (role == UserBaseRoles.Chief) ? RoleConstants.Chief :
                   (role == UserBaseRoles.Manager) ? RoleConstants.Manager :
                   (role == UserBaseRoles.Employee) ? RoleConstants.Employee :
                   (role == UserBaseRoles.User) ? RoleConstants.User :
                   (role == UserBaseRoles.Admin) ? RoleConstants.Admin : "";
        }
        private static string WorkerStatusByIx(OrgTypes type)
        {
            return (type == OrgTypes.Fund) ? RoleConstants.WorkerOf.Fund :
                   (type == OrgTypes.SMO) ? RoleConstants.WorkerOf.SMO :
                   (type == OrgTypes.MO) ? RoleConstants.WorkerOf.MO : "";
        }
        private static string UserName(string innLe, string snils)
        {
            return $"{innLe}-{snils}";
        }
        private static string UserFullName(EdoFomsUser user)
        {
            return $"{user.Surname} {user.GivenName}";
        }
        private async Task<string> SendVerificationEmail(EdoFomsUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/identity/user/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);

            return verificationUri;
        }

        public async Task<IResult> SendMailAsync(MailModel mail)
        {
            //var user = await _userManager.FindByIdAsync(mail.UserId);

            var mailRequest = new MailRequest
            {
                Body = mail.Text,
                Subject = mail.Theme,
                ToAddress = mail.Email
            };

            await _mailService.SendAsync(mailRequest);
            //BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));

            return await Result.SuccessAsync(_localizer["Mail Sended"]);
        }

        public async Task MailToUserAsync(MailToUser mail)
        {
            var user = await _userManager.FindByIdAsync(mail.UserId);

            if (user?.EmailConfirmed == true)
            {
                var mailRequest = new MailRequest
                {
                    Body = $"{user.GivenName}. {mail.Text}",
                    Subject = mail.Theme,
                    ToAddress = user.Email,
                    ToName = user.GivenName
                };

                await _mailService.SendAsync(mailRequest);
                //BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            }
        }

        public async Task MailsToUsersAsync(List<MailToUser> mails)
        {
            //mails.ForEach(async mail => await MailToUserAsync(mail));
            foreach (var mail in mails)
            {
                await MailToUserAsync(mail);
            }
        }

        //public async Task<Organization> GetUserOrg(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    var org = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == user.OrgId);

        //    return org;
        //}

        //public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        //{
        //    var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        //    if (userWithSameUserName != null)
        //    {
        //        return await Result.FailAsync(string.Format(_localizer["Username {0} is already taken."], request.UserName));
        //    }

        //    var user = new EdoFomsUser
        //    {
        //        Email =          request.Email,
        //        GivenName =      request.GivenName,
        //        Surname =        request.Surname,
        //        Snils =          request.Snils,
        //        UserName =       request.UserName,
        //        PhoneNumber =    request.PhoneNumber,
        //        IsActive =       request.ActivateUser,
        //        EmailConfirmed = request.AutoConfirmEmail
        //    };

        //    if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        //    {
        //        var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
        //        if (userWithSamePhoneNumber != null)
        //        {
        //            return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
        //        }
        //    }

        //    var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        //    if (userWithSameEmail == null)
        //    {
        //        var result = await _userManager.CreateAsync(user, request.Password);
        //        if (result.Succeeded)
        //        {
        //            await _userManager.AddToRoleAsync(user, RoleConstants.User);
        //            if (!request.AutoConfirmEmail)
        //            {
        //                var verificationUri = await SendVerificationEmail(user, origin);
        //                var mailRequest = new MailRequest
        //                {
        //                    From = "clinic@EdoFoms.ru",
        //                    To = user.Email,
        //                    Body = string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."], verificationUri),
        //                    Subject = _localizer["Confirm Registration"]
        //                };
        //                BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        //                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"], user.UserName));
        //            }
        //            return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["User {0} Registered."], user.UserName));
        //        }
        //        else
        //        {
        //            return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
        //        }
        //    }
        //    else
        //    {
        //        return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
        //    }
        //}
    }
}